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
	public partial class TankCombat2DGameManager : MonoBehaviour
	{
		static TankCombat2DGameManager _Instance;
		public static TankCombat2DGameManager Instance
		{
			get
			{
				if (!_Instance)
				{
					_Instance = new GameObject( "TankCombat2DGameManager").AddComponent<TankCombat2DGameManager>();
					DontDestroyOnLoad( _Instance);

					_Instance.WaveNo = 0;
					_Instance.Score = 0;
					_Instance.Lives = 4;
				}
				return _Instance;
			}
		}

		public void ResetGame()
		{
			if (_Instance)
			{
				Destroy(_Instance.gameObject);
			}
			_Instance = null;
		}

		bool gameRunning;
		public void SetRunning( bool running)
		{
			gameRunning = running;

			// TODO load UI?
		}

		// state of the game
		public int WaveNo { get; private set; }
		public int Score { get; private set; }
		public int Lives { get; private set; }
		public bool GameIsHidden { get; private set; }

		float timeWithoutPlayer;
		const float PlayerSpawnTime = 1.0f;

		float timeWithoutEnemies;
		const float EnemyWaveOverTime = 1.0f;

		float enemyCreationCooldown;
		// spawn interval between appearances
		const float EnemySpawnInterval = 0.1f;
		// when things get full, don't instantly bang on creating fresh enemies
		const float EnemySpawnBackoff = 5.0f;

		string loadedLevel;
		void ManageLevel()
		{
			if (loadedLevel == null)
			{
				GameIsHidden = true;

				WaveNo++;

				ComputeDifficultyParameters();

				// TODO: decide which level to load
				loadedLevel = "TC2DLevel1_";

				loadedLevel = "TC2DBitmapLevels_";

				SceneHelper.LoadScene( loadedLevel, additive: true, setActive: true);

				// TODO: decide difficulty, number of enemies, etc.
				EnemyManagerStartLevel();
			}
		}

		bool suspendForLevelFinished;
		void FinishLevel()
		{
			SceneHelper.UnloadScene( loadedLevel);

			TC2DPlayer.Unload();

			Lives++;

			GameIsHidden = true;

			TankCombat2D_WaveComplete.Load();

			suspendForLevelFinished = true;
		}

		public bool IsGameRunning()
		{
			if (!gameRunning)
			{
				return false;
			}

			if (Time.timeScale == 0)
			{
				return false;
			}

			return true;
		}

		public void ResumeAfterLevelFinished()
		{
			loadedLevel = null;
			timeWithoutEnemies = 0;
			timeWithoutPlayer = 0;
			suspendForLevelFinished = false;
		}

		// watch what's going on, keep the game moving forward
		void Update()
		{
			if (!IsGameRunning()) return;

			if (suspendForLevelFinished) return;

			ManageLevel();

			if (ManagePlayer())
			{
				ManageEnemies();
			}
		}
	}
}
