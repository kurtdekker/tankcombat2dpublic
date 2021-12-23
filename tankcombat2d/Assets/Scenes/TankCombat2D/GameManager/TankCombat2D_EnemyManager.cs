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
		const float EnemyPositionSpawnSpacing = 5.0f;

		const float EnemyLOSSpawnSpacing = 20.0f;

		int EnemiesRemainingToCreate;

		void EnemyManagerStartLevel()
		{
			EnemiesRemainingToCreate = 3 + WaveNo;
		}

		bool TryToCreateAnEnemy()
		{
			Vector3 position;

			if (GetSafeEnemySpawnLocation( out position))
			{
				float heading = Random.Range( 0, 360);

				// TODO: decide what kind of tank to make

				TC2DTankBozo.Create( TC2DResources.EnemyTankPrefab, position, heading);

				return true;
			}

			return false;
		}

		void ManageEnemies()
		{
			Enemies.RemoveAll( x => !x);

			if (EnemiesRemainingToCreate > 0)
			{
				timeWithoutEnemies = 0.0f;

				enemyCreationCooldown += Time.deltaTime;

				if (enemyCreationCooldown > EnemySpawnInterval)
				{
					enemyCreationCooldown = 0.0f;

					if (TryToCreateAnEnemy())
					{
						EnemiesRemainingToCreate--;
					}
					else
					{
						enemyCreationCooldown -= EnemySpawnBackoff;
					}
				}
			}
			else
			{
				if (Enemies.Count > 0)
				{
					timeWithoutEnemies = 0.0f;
				}
				else
				{
					timeWithoutEnemies += Time.deltaTime;

					if (timeWithoutEnemies > EnemyWaveOverTime)
					{
						timeWithoutEnemies = 0;

						FinishLevel();
					}
				}
			}
		}

		void RemoveSurvivingEnemies()
		{
			Enemies.RemoveAll( x => !x);

			foreach( var enemy in Enemies.ToArray())
			{
				Destroy( enemy);

				EnemiesRemainingToCreate++;		// put him back
			}
		}

		List<GameObject> Enemies = new List<GameObject>();

		public void RegisterEnemy( GameObject e)
		{
			Enemies.Add(e);
		}
		public void UnregisterEnemy( GameObject e)
		{
			Enemies.Remove(e);
		}
	}
}
