using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankCombat2D
{
	public class TankCombat2D_Radar : MonoBehaviour
	{
		const string s_SceneName = "TankCombat2D_Radar_";

		static bool Loading;

		public OffscreenBlip ExemplarOffscreenBlip;

		public static void Load()
		{
			if (!Loading)
			{
				Loading = true;

				SceneHelper.LoadScene( s_SceneName, additive: true);
			}
		}

		public static TankCombat2D_Radar Instance { get; private set; }

		void Start()
		{
			Instance = this;

			ExemplarOffscreenBlip.gameObject.SetActive(false);
		}

		public OffscreenBlip GetBlip()
		{
			var blip = Instantiate<OffscreenBlip>( ExemplarOffscreenBlip, ExemplarOffscreenBlip.transform.parent);

			blip.gameObject.SetActive(true);

			return blip;
		}

		void OnDisable()
		{
			Loading = false;
		}
	}
}
