using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankCombat2D
{
	public class TC2DWeapon : MonoBehaviour
	{
		// fill all this out yourself
		public int MaxBullets { get; set; }

		public Transform Muzzle { get; set; }

		public float BulletSpeed { get; set; }
		public float BulletDamage { get; set; }
		public float BulletMaxDistance { get; set; }
		public int LayerMask { get; set; }
		public float CooldownInterval { get; set; }

		public static TC2DWeapon Attach( GameObject go)
		{
			var w = go.AddComponent<TC2DWeapon>();
			return w;
		}

		List<TC2DBullet> Bullets = new List<TC2DBullet>();

		float cooling;

		public void HandleFiring( ref bool fireRequested)
		{
			if (cooling > 0)
			{
				cooling -= Time.deltaTime;
				return;
			}

			if (fireRequested)
			{
				if (Bullets.Count < MaxBullets)
				{
					var bullet = TC2DBullet.Create(
						transform,
						Muzzle,
						TC2DResources.BulletPrefab,
						layerMask: LayerMask,
						speed: BulletSpeed,
						damage: BulletDamage,
						maxDistance: BulletMaxDistance);

					Bullets.Add( bullet);

					cooling += CooldownInterval;
				}
				fireRequested = false;
			}
		}

		void Update()
		{
			Bullets.RemoveAll( x => !x);
		}
	}
}
