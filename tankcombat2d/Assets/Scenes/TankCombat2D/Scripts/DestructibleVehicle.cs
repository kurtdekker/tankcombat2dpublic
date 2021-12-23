using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankCombat2D
{
	public class DestructibleVehicle : MonoBehaviour, IDamageable
	{
		float health;
		System.Action OnDeath;

		public static DestructibleVehicle Attach(
			GameObject go,
			float health,
			System.Action OnDeath)
		{
			var dv = go.AddComponent<DestructibleVehicle>();

			dv.health = health;
			dv.OnDeath = OnDeath;

			return dv;
		}

		public void TakeDamage( float damage)
		{
			health -= damage;

			if (health <= 0)
			{
				OnDeath();
			}
		}
	}
}
