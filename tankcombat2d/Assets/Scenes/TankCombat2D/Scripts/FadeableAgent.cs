using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankCombat2D
{
	public class FadeableAgent : MonoBehaviour
	{
		public static FadeableAgent Attach( GameObject agent, float alpha = 1.0f)
		{
			var fa = agent.AddComponent<FadeableAgent>();
			fa.currentAlpha = alpha;
			fa.desiredAlpha = fa.currentAlpha;
			fa.DriveCurrentAlpha();
			return fa;
		}

		public void SetDesiredAlpha( float alpha, bool instantaneous = false)
		{
			desiredAlpha = alpha;
			if (instantaneous)
			{
				currentAlpha = desiredAlpha;
				DriveCurrentAlpha();
			}
		}

		List<SpriteRenderer> Sprites;

		void DiscoverSprites()
		{
			Sprites = new List<SpriteRenderer>(
				GetComponentsInChildren<SpriteRenderer>()
			);
		}

		float drivenAlpha = -1;
		float currentAlpha;
		float desiredAlpha;
		void DriveCurrentAlpha()
		{
			if (Sprites == null)
			{
				DiscoverSprites();
			}

			if (currentAlpha != drivenAlpha)
			{
				foreach( var sr in Sprites)
				{
					var c = sr.color;
					c.a = currentAlpha;
					sr.color = c;
				}

				drivenAlpha = currentAlpha;
			}
		}

		const float FadeUpRate = 5.0f;
		const float FadeDownRate = 2.0f;

		void Update()
		{
			if (currentAlpha < desiredAlpha)
			{
				currentAlpha = Mathf.MoveTowards( currentAlpha, desiredAlpha, FadeUpRate * Time.deltaTime);
				DriveCurrentAlpha();
			}
			if (currentAlpha > desiredAlpha)
			{
				currentAlpha = Mathf.MoveTowards( currentAlpha, desiredAlpha, FadeDownRate * Time.deltaTime);
				DriveCurrentAlpha();
			}
		}
	}
}
