using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankCombat2D
{
	public class EnemyRegistrator : MonoBehaviour
	{
		void OnEnable()
		{
			TankCombat2DGameManager.Instance.RegisterEnemy( gameObject);
		}
		void OnDisable()
		{
			TankCombat2DGameManager.Instance.UnregisterEnemy( gameObject);
		}
	}
}
