using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankCombat2D
{
	public class TC2DBullet : MonoBehaviour
	{
		Vector3 velocity;

		int layerMask;
		float damage;
		float maxDistance;

		// just a simple factory, arguments slightly getting out of control though
		public static TC2DBullet Create(
			Transform Firer, Transform Muzzle,
			TC2DBullet BulletPrefab,
			int layerMask,
			float speed, float damage, float maxDistance)
		{
			// assume we fire from muzzle
			Vector3 position = Muzzle.position;

			var bullet = Instantiate<TC2DBullet>( BulletPrefab, position, Muzzle.rotation);

			bullet.layerMask = layerMask;

			bullet.velocity = Muzzle.up * speed;

			bullet.damage = damage;

			bullet.maxDistance = maxDistance;

			StrayObjectToCleanUp.Attach( bullet.gameObject);

			return bullet;
		}

		float distanceTravelled;
		bool doomed;

		void FixedUpdate ()
		{
			if (doomed)
			{
				Destroy( gameObject);
				return;
			}

			Vector3 step = velocity * Time.deltaTime;

			Vector3 currentPosition = transform.position;

			Vector3 newPosition = currentPosition + step;

			float stepDistance = step.magnitude;

			distanceTravelled += stepDistance;

			if (distanceTravelled >= maxDistance)
			{
				Destroy(gameObject);
				return;
			}

			RaycastHit2D hit = Physics2D.Raycast(
				origin: currentPosition,
				direction: velocity,
				distance: stepDistance,
				layerMask: layerMask);

			if (hit.collider)
			{
				newPosition = hit.point;

				doomed = true;

				// splatch of impact
				Instantiate<GameObject>( TC2DResources.Splatch1Prefab, newPosition, Quaternion.Euler( 0, 0, Random.Range( 0, 360)));

				var id = hit.collider.GetComponent<IDamageable>();
				if (id != null)
				{
					id.TakeDamage( damage);
				}

				// TODO: sound of impact
			}

			transform.position = newPosition;
		}
	}
}
