/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2021 Kurt Dekker/PLBM Games All rights reserved.

	http://www.twitter.com/kurtdekker

	Redistribution and use in source and binary forms, with or without
	modification, are permitted provided that the following conditions are
	met:

	Redistributions of source code must retain the above copyright notice,
	this list of conditions and the following disclaimer.

	Redistributions in binary form must reproduce the above copyright
	notice, this list of conditions and the following disclaimer in the
	documentation and/or other materials provided with the distribution.

	Neither the name of the Kurt Dekker/PLBM Games nor the names of its
	contributors may be used to endorse or promote products derived from
	this software without specific prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
	IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
	TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
	PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
	HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
	SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
	TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
	PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
	LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
	NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
	SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankCombat2D
{
	public partial class TC2DTankBozo : MonoBehaviour
	{
		// Idea: bozo tank... has two states:
		//	- wandering aimlessly
		//		- chooses wander direction to tend to go towards player
		//	- turning to engage the player
		//
		// only periodically switches between them
		//	- only leave wandering when player in sight
		//	- only leave shooting when player out of sight
		//

		[Header( "Tank parts")]
		public Transform Turret;
		public Transform Muzzle;

		const float PlayerSpottingRange = 20;

		public enum BozoActions { INITIAL, WANDERING, SHOOTING, }

		BozoActions action;

		// replace with movement params
		const float TankTurnRate = 90;
		const float DriveAcceleration = 4;		// normalized (1 is full travel in 1 second), 4 would be 1/4 of a second
		const float TurretTurnRate = 90;
		const float MaxTurretAngle = 90;
		const float NormalDrive = 1.0f;
		const float TurningDrive = 0.1f;
		const float Acceleration = 2.0f;

		float tankHeading;
		float turretHeading;			// relative!
		Rigidbody2D rb2d;

		float currentDrive;		// normalized
		float desiredDrive;

		public static GameObject Create( GameObject EnemyPrefab, Vector3 position, float heading)
		{
			var enemy = Instantiate<GameObject>( EnemyPrefab, position, Quaternion.Euler( 0, 0, heading));
			enemy.AddComponent<EnemyRegistrator>();
			enemy.AddComponent<IsDeadlyToPlayer>();
			FadeableAgent.Attach( enemy, 0);
			OffscreenableAgent.Attach( enemy, Color.red);
			return enemy;
		}

		TC2DWeapon weapon;

		IEnumerator Start ()
		{
			rb2d = GetComponent<Rigidbody2D>();

			tankHeading = transform.eulerAngles.z;
			turretHeading = 0;

			weapon = TC2DWeapon.Attach( gameObject);

			DestructibleVehicle.Attach( gameObject, 1.0f, DestroyMe);

			// master "what am I doing next" loop
			while( true)
			{
				if (TankCombat2DGameManager.Instance.IsGameRunning())
				{
					// almost everything we do is player-centric so when
					// the player dies, well, not much to decide anymore.
					if (TC2DPlayer.Instance)
					{
						ChooseWhatToDo();
					}
				}

				// continue doing things
				yield return new WaitForSeconds( Random.Range( 0.5f, 1.2f));
			}
		}

		#if UNITY_EDITOR
		void Update()
		{
			if (Input.GetKeyDown( KeyCode.W))
			{
				DestroyMe();
			}
		}
		#endif

		void DestroyMe()
		{
			TankCombat2DGameManager.Instance.AddScore(1);

			Instantiate<GameObject>( TC2DResources.Splatch2Prefab, transform.position, Quaternion.Euler( 0, 0, Random.Range( 0, 360)));

			Instantiate<GameObject>(
				TC2DResources.DestroyedTankPrefab,
				transform.position,
				Quaternion.Euler( 0, 0, Random.Range( 0, 360)));
			
#if false
			{
				var corpse = Instantiate<GameObject>(
					TC2DResources.DestroyedTankPrefab,
					transform.position,
					Quaternion.Euler( 0, 0, Random.Range( 0, 360)));
				DestructibleVehicle.Attach(
					corpse,
					Random.Range( 0.5f, 3.0f),
					() => {
						Instantiate<GameObject>(
							TC2DResources.Splatch1Prefab,
							corpse.transform.position,
							Quaternion.Euler( 0, 0, Random.Range( 0, 360)));

						// TODO: spread the parts out a bit?
					}
				);
			}
#endif

			Destroy(gameObject);
		}

		void ChooseWhatToDo()
		{
			// can we see the player?
			Vector3 myPosition = transform.position;
			Vector3 playerPosition = TC2DPlayer.Instance.transform.position;
			Vector3 vectorToPlayer = playerPosition - myPosition;

			RaycastHit2D hit = Physics2D.Raycast(
				origin: myPosition,
				direction: vectorToPlayer,
				distance: PlayerSpottingRange,
				layerMask: MyLayers.NotEnemyLayerMask);

			switch( action)
			{
			case BozoActions.WANDERING :
				if (hit.collider)
				{
					if (hit.collider.gameObject == TC2DPlayer.Instance.gameObject)
					{
						StartShooting();
					}
				}
				break;

			case BozoActions.SHOOTING :
				// try to see the player, if you can't, go back to wandering
				if (!hit.collider)
				{
					StartWandering();
					break;
				}
				if (hit.collider.gameObject != TC2DPlayer.Instance.gameObject)
				{
					StartWandering();
				}
				break;
			}
		}

		void StartShooting()
		{
			action = BozoActions.SHOOTING;
			DoSitStill();
		}

		void StartWandering()
		{
			action = BozoActions.WANDERING;

			Vector3 best = transform.position;

			var playerPosition = TC2DPlayer.Instance.transform.position;

			// pick 3 nearby destinations randomly, use the one closest to player
			for (int i = 0; i < 3; i++)
			{
				// relative to us
				Vector3 proposed = Random.onUnitSphere;
				proposed *= 15;
				proposed.z = 0;

				proposed += transform.position;

				if (Vector3.Distance( proposed, playerPosition) <
					Vector3.Distance( best, playerPosition))
				{
					best = proposed;
				}
			}

			currentDestination = best;

			destinationShifter = Random.insideUnitCircle.normalized;
		}

		Vector3 currentDestination;
		// to keep from orbiting an unreachable destination, we slowly move the destination.
		Vector3 destinationShifter;

		void DoWandering()
		{
			desiredDrive = NormalDrive;

			currentDestination += destinationShifter * Time.deltaTime;

			// if not facing destination, turn to face it, moving only slowly
			Vector3 destinationOffset = currentDestination - transform.position;

			float destinationHeading = Mathf.Atan2( -destinationOffset.x, destinationOffset.y) * Mathf.Rad2Deg;

			float headingDelta = Mathf.DeltaAngle( destinationHeading, tankHeading);

			if (headingDelta > 5)
			{
				desiredDrive = TurningDrive;
			}

			tankHeading = Mathf.MoveTowardsAngle( tankHeading, destinationHeading, TankTurnRate * Time.deltaTime);

			rb2d.MoveRotation( tankHeading);

			// drive according to how fast we have chosen above
			currentDrive = Mathf.MoveTowards( currentDrive, desiredDrive, DriveAcceleration * Time.deltaTime);

			var DriveSpeed = TankCombat2DGameManager.Instance.CurrentEnemyDriveSpeed;
			Vector3 movement = transform.up * currentDrive * DriveSpeed * Time.deltaTime;

			Vector3 position = transform.position;

			position += movement;

			position.z = 0;		// just in case anyone gets "funny"

			rb2d.MovePosition( position);

			// when you reach it, choose a new destination
			var distance = Vector3.Distance( currentDestination, transform.position);
			if (distance < 1)
			{
				StartWandering();
			}

			// center up the gun while wandering
			turretHeading = Mathf.MoveTowardsAngle( turretHeading, 0, TurretTurnRate * Time.deltaTime);

			Turret.localRotation = Quaternion.Euler( 0, 0, turretHeading);
		}

		void OnCollisionEnter2D( Collision2D collision)
		{
			var contact = collision.contacts[0];

			// if you bump a collider, reset destination choice AWAY from impact spot
			currentDestination = contact.point;

			// randomly away from normal
			currentDestination += Quaternion.Euler( 0, 0, Random.Range( -50, 50)) *
				contact.normal * Random.Range( 5.0f, 10.0f);
		}

		void DoShooting()
		{
			// find the player heading
			Vector3 myPosition = transform.position;
			Vector3 playerPosition = TC2DPlayer.Instance.transform.position;
			Vector3 destinationOffset = playerPosition - myPosition;

			float destinationHeading = Mathf.Atan2( -destinationOffset.x, destinationOffset.y) * Mathf.Rad2Deg;

			float headingDelta = Mathf.DeltaAngle( destinationHeading, tankHeading);

			// turn both body and turret to face
			//first the tank, ULTRA slowly:
			tankHeading = Mathf.MoveTowardsAngle( tankHeading, destinationHeading, TankTurnRate * Time.deltaTime);

			rb2d.MoveRotation( tankHeading);

			// now back the tank heading out of the destinationHeading
			destinationHeading -= tankHeading;

			while (turretHeading < -180) turretHeading += 360;
			while (turretHeading > +180) turretHeading -= 360;

			turretHeading = Mathf.Clamp( turretHeading, -MaxTurretAngle, +MaxTurretAngle);

			turretHeading = Mathf.MoveTowardsAngle( turretHeading, destinationHeading, TurretTurnRate * Time.deltaTime);

			Turret.localRotation = Quaternion.Euler( 0, 0, turretHeading);

			DoSitStill();

			headingDelta = Mathf.DeltaAngle( turretHeading, destinationHeading);

			// when close enough to pointing, fire!
			if (headingDelta < 15)
			{
				HandleFiring();
			}
		}

		void HandleFiring()
		{
			bool fireRequested = true;

			weapon.MaxBullets = 1;
			weapon.Muzzle = Muzzle;
			weapon.BulletDamage = 1;
			weapon.BulletSpeed = TankCombat2DGameManager.Instance.CurrentEnemyShot1Speed;
			weapon.BulletMaxDistance = TankCombat2DGameManager.Instance.CurrentEnemyShot1MaxDistance;
			weapon.LayerMask = MyLayers.NotEnemyLayerMask;
			weapon.CooldownInterval = 1.0f;

			weapon.HandleFiring( ref fireRequested);

		}

		void DoSitStill()
		{
			rb2d.velocity = Vector3.zero;
			rb2d.angularVelocity = 0;
		}

		void FixedUpdate ()
		{
			if (!TankCombat2DGameManager.Instance.IsGameRunning())
			{
				return;
			}

			if (!TC2DPlayer.Instance)
			{
				DoSitStill();
				return;
			}

			switch(action)
			{
			default :		// just in case...
			case BozoActions.INITIAL :
				StartWandering();
				DoSitStill();
				break;

			case BozoActions.WANDERING :
				action = BozoActions.WANDERING;
				DoWandering();
				break;

			case BozoActions.SHOOTING :
				DoShooting();
				break;
			}
		}
	}
}
