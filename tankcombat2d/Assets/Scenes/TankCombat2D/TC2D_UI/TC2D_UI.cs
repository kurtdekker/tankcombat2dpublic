using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankCombat2D
{
	public class TC2D_UI : MonoBehaviour
	{
		public GameObject Hider;
		public Text ScoreText;
		public GameObject OneLife;

		void Start()
		{
			OneLife.SetActive( false);
		}

		// the last amount we "rendered"
		int livesRendered;
		GameObject[] livesGameObjects;

		void LateUpdate ()
		{
			// Hider
			{
				Hider.SetActive( TankCombat2DGameManager.Instance.GameIsHidden);
			}

			// Score
			{
				var s = TankCombat2DGameManager.Instance.Score.ToString();
				ScoreText.text = s;
			}

			// lives
			{
				int x = TankCombat2DGameManager.Instance.Lives;

				if (x != livesRendered)
				{
					livesRendered = x;

					if (livesGameObjects != null)
					{
						foreach( var go in livesGameObjects) Destroy(go);
					}

					livesGameObjects = new GameObject[ livesRendered];

					for (int i = 0; i < livesRendered; i++)
					{
						var copy = Instantiate<GameObject>( OneLife, OneLife.transform.parent);
						copy.SetActive(true);
						livesGameObjects[i] = copy.gameObject;
					}
				}
			}
		}
	}
}
