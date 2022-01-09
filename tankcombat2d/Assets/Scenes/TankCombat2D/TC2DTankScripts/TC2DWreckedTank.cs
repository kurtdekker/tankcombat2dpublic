using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankCombat2D
{
	public class TC2DWreckedTank : MonoBehaviour, IDamageable
	{
		[Header( "Initial wreck color:")]
		public Color color1 = new Color( 0.4f, 0.4f, 0.4f);
		[Header( "Destroyed wreck color:")]
		public Color color2 = new Color( 0.25f, 0.25f, 0.25f);

		float health;

		public void TakeDamage( float damage)
		{
			health -= damage;

			if (health <= 0)
			{
				DestroyCollidersAndSelf();
			}
		}

		void DestroyCollidersAndSelf()
		{
			var cols = GetComponentsInChildren<Collider2D>();

			foreach( var col in cols)
			{
				Destroy(col);
			}

			SetColors( color2);

			Destroy(this);
		}

		SpriteRenderer[] sprites;

		void Start ()
		{
			health = Random.Range( 0.5f, 3.0f);

			TTL.Attach( gameObject, 30);

			sprites = GetComponentsInChildren<SpriteRenderer>();

			SetColors( color1);
		}

		void SetColors( Color color)
		{
			if (sprites != null)
			{
				foreach( var spr in sprites)
				{
					spr.color = color;
				}
			}
		}
	}
}
