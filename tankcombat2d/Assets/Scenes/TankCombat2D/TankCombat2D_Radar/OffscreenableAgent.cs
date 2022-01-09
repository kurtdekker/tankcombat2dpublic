using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankCombat2D
{
	public class OffscreenableAgent : MonoBehaviour
	{
		Color color;

		public static OffscreenableAgent Attach( GameObject go, Color color)
		{
			var oi = go.AddComponent<OffscreenableAgent>();
			oi.color = color;
			TankCombat2D_Radar.Load();
			return oi;
		}

		OffscreenBlip blip;

		void Update ()
		{
			if (!blip)
			{
				blip = TankCombat2D_Radar.Instance.GetBlip();
				blip.SetColor( color);
			}

			if (blip)
			{
				blip.SetWorldPosition( transform.position);
			}
		}

		void OnDisable()
		{
			if (blip)
			{
				blip.DestroyThyself();
			}
		}
	}
}
