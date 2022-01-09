using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankCombat2D
{
	public class OffscreenBlip : MonoBehaviour
	{
		public Image ImageBlip;

		public RectTransform LowerLeft;
		public RectTransform UpperRight;

		public void DestroyThyself()
		{
			Destroy(gameObject);
		}

		public void SetColor( Color c)
		{
			ImageBlip.color = c;
		}

		Camera cam;

		public void SetWorldPosition( Vector3 position)
		{
			if (!cam)
			{
				cam = Camera.main;
			}

			// calculate the screen edge position
			Vector3 screen = cam.WorldToScreenPoint( position);

			// we hide our blip when we are nearly-onscreen

			float edge = Mathf.Min( Screen.width, Screen.height) * 0.05f;

			bool visible = false;

			if (screen.x < -edge)
			{
				visible = true;
			}
			if (screen.x > Screen.width + edge)
			{
				visible = true;
			}
			if (screen.y < -edge)
			{
				visible = true;
			}
			if (screen.y > Screen.height + edge)
			{
				visible = true;
			}

			ImageBlip.gameObject.SetActive( visible);

			var x = Mathf.Lerp( LowerLeft.position.x, UpperRight.position.x, screen.x / Screen.width);
			var y = Mathf.Lerp( LowerLeft.position.y, UpperRight.position.y, screen.y / Screen.height);

			ImageBlip.transform.position = new Vector3( x, y);
		}
	}
}
