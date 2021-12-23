/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2021 Kurt Dekker/PLBM Games All rights reserved.

	http://www.twitter.com/kurtdekker

	Redistribution and use in source and binary forms, with or without
	modification, are permitted provided that the following conditions are
	met:

	Redistributions of source code must retain the above copyright notice,
	this list of conditions and the following disclaimer.

	Redistributions in binary form must reproduce the above copyright
	notice, this list of conditions and the following disclaimer in the
	documentation and/or other materials provided with the distribution.

	Neither the name of the Kurt Dekker/PLBM Games nor the names of its
	contributors may be used to endorse or promote products derived from
	this software without specific prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
	IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
	TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
	PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
	HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
	SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
	TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
	PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
	LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
	NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
	SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankCombat2D
{
	public partial class TankCombat2DGameManager
	{
		// this must always succeed: in the future if there is no safe
		// spawn, we can kill a random enemy and go there.
		public bool GetSafePlayerSpawnLocation( out Vector3 position)
		{
			position = Vector3.zero;

			if (HaveSpawns())
			{
				List<Transform> Spawns = new List<Transform>( GetSpawns());

				position = Spawns[ Random.Range( 0, Spawns.Count)].position;

				return true;
			}
			return false;
		}

		// This might fail, if there are no safe spots
		public bool GetSafeEnemySpawnLocation( out Vector3 position)
		{
			position = Vector3.zero;

			if (!HaveSpawns())
			{
				return false;
			}

			List<Transform> Spawns = new List<Transform>( GetSpawns());

			foreach( var spawn in Spawns.ToArray())
			{
				bool badSpawnLocation = false;

				// don't return ones that are blocked by a nearby enemy
				foreach( var enemy in Enemies)
				{
					var d = Vector3.Distance( enemy.transform.position, spawn.position);

					if (d < EnemyPositionSpawnSpacing)
					{
						badSpawnLocation = true;
						break;
					}
				}

				if (!badSpawnLocation)
				{
					var player = TC2DPlayer.Instance;

					var d = Vector3.Distance( player.transform.position, spawn.position);

					// don't return if too close to player
					if (d < EnemyPositionSpawnSpacing)
					{
						badSpawnLocation = true;
					}
					else
					{
						if (d < EnemyLOSSpawnSpacing)
						{
							// don't return ones that are LOS to the player
							var direction = player.transform.position - spawn.position;

							RaycastHit2D hit = Physics2D.Raycast(
								origin: spawn.position,
								direction: direction,
								distance: d);

							if (hit.collider)
							{
								int layer = hit.collider.gameObject.layer;

								if (layer == MyLayers.PlayerLayer)
								{
									badSpawnLocation = true;
								}
							}
						}
					}
				}

				if (badSpawnLocation)
				{
					Spawns.Remove( spawn);
				}
			}

			if (Spawns.Count > 0)
			{
				position = Spawns[ Random.Range( 0, Spawns.Count)].position;

				return true;
			}

			return false;
		}

		bool HaveSpawns()
		{
			var SpawnPoints = GameObject.Find( "SpawnPoints");
			return SpawnPoints;
		}
		IEnumerable<Transform> GetSpawns()
		{
			// Rule #1 of GameObject.Find: Do not use GameObject.Find()
			var SpawnPoints = GameObject.Find( "SpawnPoints");

			for (int i = 0; i < SpawnPoints.transform.childCount; i++)
			{
				yield return SpawnPoints.transform.GetChild(i);
			}
		}
	}
}
	