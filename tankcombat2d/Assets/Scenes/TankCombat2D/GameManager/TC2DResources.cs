using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankCombat2D
{
	public static class TC2DResources
	{
		// TODO: let's centralize this in a GameManager-driven resource manager

		public static GameObject Splatch1Prefab
		{
			get { return Resources.Load<GameObject>( "TC2DPrefabs/Splatch1Prefab"); }
		}

		public static GameObject Splatch2Prefab
		{
			get { return Resources.Load<GameObject>( "TC2DPrefabs/Splatch2Prefab"); }
		}

		public static TC2DBullet BulletPrefab
		{
			get { return Resources.Load<TC2DBullet>( "TC2DPrefabs/PlayerBullet_Prefab"); }
		}

		public static GameObject EnemyTankPrefab
		{
			get { return Resources.Load<GameObject>( "TC2DPrefabs/EnemyTankBozo_Prefab"); }
		}

		public static GameObject DestroyedTankPrefab
		{
			get { return Resources.Load<GameObject>( "TC2DPrefabs/DestroyedTank_Prefab"); }
		}
	}
}
