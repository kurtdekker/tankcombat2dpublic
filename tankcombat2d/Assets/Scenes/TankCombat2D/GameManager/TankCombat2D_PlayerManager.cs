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
		float playerDyingTimer;
		Vector3 playerFinalRestingPlace;

		public void PlayerHasDied( Vector3 position)
		{
			playerFinalRestingPlace = position;

			playerDyingTimer = 1.0f;
		}

		bool ManagePlayer()
		{
			if (playerDyingTimer > 0)
			{
				// explosions as the player dies
				{
					Vector3 position = Random.insideUnitCircle;
					position *= 1.5f;
					position.z = 0;
					position += playerFinalRestingPlace;
					Instantiate<GameObject>( TC2DResources.Splatch1Prefab, position, Quaternion.identity);
				}

				playerDyingTimer -= Time.deltaTime;

				if (playerDyingTimer <= 0)
				{
					RemoveSurvivingEnemies();

					StrayObjectToCleanUp.CleanThemAllUp();
				}

				return false;
			}

			if (TC2DPlayer.Instance)
			{
				timeWithoutPlayer = 0;

				GameIsHidden = false;
			}
			else
			{
				timeWithoutPlayer += Time.deltaTime;

				if (timeWithoutPlayer < PlayerSpawnTime)
				{
					return false;
				}

				// spawn!
				timeWithoutPlayer = 0;

				if (Lives > 0)
				{
					Lives--;

					TC2DPlayer.Load();
				}
				else
				{
					TankCombat2D_Gameover.Load();
				}
			}

			return true;
		}
	}
}
