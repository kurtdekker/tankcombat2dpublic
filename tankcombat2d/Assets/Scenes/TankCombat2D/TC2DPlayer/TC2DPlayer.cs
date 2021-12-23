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
	public partial class TC2DPlayer : MonoBehaviour
	{
		static string s_SceneName = "TC2DPlayer_";

		public static void Load()
		{
			SceneHelper.LoadScene( TC2DPlayer.s_SceneName, additive: true);
		}
		public static void Unload()
		{
			SceneHelper.UnloadScene( TC2DPlayer.s_SceneName);
		}

		[Header( "Tank parts")]
		public Transform Turret;
		public Transform Muzzle;

		// replace with movement params
		const float TankTurnRate = 180;
		const float DriveSpeed = 10;
		const float DriveAcceleration = 4;		// normalized (1 is full travel in 1 second), 4 would be 1/4 of a second
		const float TurretTurnRate = 90;
		const float MaxTurretAngle = 90;

		// TODO: move this kinda stuff into a difficulty manager
		const float BulletSpeed = 40;
		const int MaxBullets = 2;
		const float BulletDamage = 1.0f;
		const float BulletMaxDistance = 30.0f;

		float tankHeading;
		float currentTurretHeading;
		float desiredTurretHeading;
		Rigidbody2D rb2d;
		Camera cam;

		public static TC2DPlayer Instance { get; private set; }

		TC2DWeapon weapon;

		IEnumerator Start ()
		{
			Instance = this;

			Vector3 position = Vector3.zero;

			while( !TankCombat2DGameManager.Instance.GetSafePlayerSpawnLocation( out position))
			{
				yield return null;
			}

			transform.position = position;

			rb2d = GetComponent<Rigidbody2D>();

			tankHeading = transform.eulerAngles.z;

			weapon = TC2DWeapon.Attach( gameObject);

			DestructibleVehicle.Attach( gameObject, 1.0f, YouHaveDied);

			cam = Camera.main;

			ready = true;
		}

		bool ready;

		// inputs:
		float turnTank;
		float desiredDrive;
		bool fireRequested;

		// status
		float currentDrive;

		void GatherInputs()
		{
			turnTank = 0.0f;
			desiredDrive = 0.0f;
			desiredTurretHeading = currentTurretHeading;
			fireRequested = false;

			turnTank -= Input.GetAxisRaw( "Horizontal");
			desiredDrive += Input.GetAxisRaw( "Vertical");

			if (Input.GetKey( KeyCode.Alpha1))
			{
				desiredTurretHeading = +MaxTurretAngle;
			}
			if (Input.GetKey( KeyCode.Alpha2))
			{
				desiredTurretHeading = 0;
			}
			if (Input.GetKey( KeyCode.Alpha3))
			{
				desiredTurretHeading = -MaxTurretAngle;
			}

			if (Input.GetKeyDown( KeyCode.Space) ||
				Input.GetKeyDown( KeyCode.LeftControl) ||
				false)
			{
				fireRequested = true;
			}

			turnTank = Mathf.Clamp( turnTank, -1, 1);
			desiredDrive = Mathf.Clamp( desiredDrive, -1, 1);
		}

		void TurnPlayer()
		{
			tankHeading += turnTank * TankTurnRate * Time.deltaTime;

			rb2d.MoveRotation( tankHeading);
		}

		void MovePlayer()
		{
			currentDrive = Mathf.MoveTowards( currentDrive, desiredDrive, DriveAcceleration * Time.deltaTime);

			Vector3 movement = transform.up * currentDrive * DriveSpeed * Time.deltaTime;

			Vector3 position = transform.position;

			position += movement;

			position.z = 0;		// just in case anyone gets "funny"

			rb2d.MovePosition( position);
		}

		void DriveTurret()
		{
			currentTurretHeading = Mathf.MoveTowardsAngle( currentTurretHeading, desiredTurretHeading, TurretTurnRate * Time.deltaTime);

			while (currentTurretHeading < -180) currentTurretHeading += 360;
			while (currentTurretHeading > +180) currentTurretHeading -= 360;

			currentTurretHeading = Mathf.Clamp( currentTurretHeading, -MaxTurretAngle, MaxTurretAngle);

			Turret.localRotation = Quaternion.Euler (0, 0, currentTurretHeading);
		}

		void MoveCamera()
		{
			// leave the Z unmolested, track only in X/Y

			Vector3 position = cam.transform.position;

			position.x = transform.position.x;
			position.y = transform.position.y;

			cam.transform.position = position;
		}

		void Update ()
		{
			if (!TankCombat2DGameManager.Instance.IsGameRunning()) return;

			if (!ready) return;

			GatherInputs();
		}

		bool dieOnceOnly;
		void YouHaveDied()
		{
			if (!dieOnceOnly)
			{
				dieOnceOnly = true;

				TankCombat2DGameManager.Instance.PlayerHasDied( transform.position);

				Destroy(gameObject);

				Unload();
			}
		}

		void OnCollisionEnter2D( Collision2D collision)
		{
			var deadly = collision.collider.GetComponent<IsDeadlyToPlayer>();
			if (deadly)
			{
				YouHaveDied();
			}
		}

		void HandleFiring()
		{
			if (fireRequested)
			{
				weapon.MaxBullets = MaxBullets;
				weapon.Muzzle = Muzzle;
				weapon.BulletDamage = BulletDamage;
				weapon.BulletMaxDistance = BulletMaxDistance;
				weapon.BulletSpeed = BulletSpeed;
				weapon.LayerMask = MyLayers.NotPlayerLayerMask;

				weapon.HandleFiring( ref fireRequested);
			}
		}

		void FixedUpdate ()
		{
			if (!TankCombat2DGameManager.Instance.IsGameRunning()) return;

			if (!ready) return;

			TurnPlayer();

			MovePlayer();

			DriveTurret();

			HandleFiring();

			MoveCamera();

			#if UNITY_EDITOR
			// kill off the player
			if (Input.GetKeyDown( KeyCode.K))
			{
				YouHaveDied();
			}
			#endif
		}
	}
}
