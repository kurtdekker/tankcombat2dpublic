using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker

namespace TankCombat2D
{
	public class StrayObjectToCleanUp : MonoBehaviour
	{
		public static StrayObjectToCleanUp Attach( GameObject go)
		{
			var stray = go.AddComponent<StrayObjectToCleanUp>();
			return stray;
		}

		public static void CleanThemAllUp()
		{
			var strays = FindObjectsOfType<StrayObjectToCleanUp>();

			foreach( var stray in strays)
			{
				stray.DestroyThyself();
			}
		}

		public virtual void DestroyThyself()
		{
			Destroy(gameObject);
		}
	}
}
