using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TC2DBitmapLevel : MonoBehaviour
{
	public Texture2D t2d;

	public GameObject BlockSolid;
	public GameObject BlockHalf;
	public GameObject Empty;

	public float CellSize = 2.56f;

	void OnValidate()
	{
		// to throw an error if you forgot to read/write enable
		if (t2d)
		{
			t2d.GetPixel(0,0);
		}
	}

	void Start ()
	{
		var Geometry = new GameObject( "Geometry");
		Geometry.transform.SetParent( transform);

		var SpawnPoints = new GameObject( "SpawnPoints");
		SpawnPoints.transform.SetParent( transform);

		Vector3 center = new Vector3( t2d.height, t2d.width) * CellSize / 2;

		for (int j = 0; j < t2d.height; j++)
		{
			for (int i = 0; i < t2d.width; i++)
			{
				var c = t2d.GetPixel( i, j);

				var pos = new Vector3( i, j) * CellSize - center;

				var b = Bandit(c);

				switch( b)
				{
				case "000":		// black
					break;

				case "111":		// darkgray
					Instantiate<GameObject>( BlockHalf, pos, Quaternion.identity, Geometry.transform);
					break;

				case "333":		// white
					Instantiate<GameObject>( BlockSolid, pos, Quaternion.identity, Geometry.transform);
					break;

				case "300":
					{
						var p = new GameObject( "x");
						p.transform.position = pos;
						p.transform.SetParent( SpawnPoints.transform);
					}
					break;

				default :
					Debug.LogError( GetType() + ": unhandled Bandy:" + b);
					break;
				}
			}
		}
	}

	static string Bandy( float f)
	{
		if (f <= 0.25f) return "0";
		if (f <= 0.50f) return "1";
		if (f <= 0.75f) return "2";
		return "3";
	}

	static string Bandit( Color c)
	{
		return Bandy(c.r) + Bandy(c.g) + Bandy(c.b);
	}
}
