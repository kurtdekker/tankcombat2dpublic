/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2022 Kurt Dekker/PLBM Games All rights reserved.

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
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TankCombat2D
{
	public class TankCombat2D_Mainmenu : MonoBehaviour
	{
		public Button ButtonStart;

		[Header("Input Scheme:")]
		public Button ToggleInputScheme;
		public Text DisplayInputScheme;

		[Header("Camera Rotation:")]
		public Button ToggleCameraRotation;
		public Text DisplayCameraRotation;

		void Start ()
		{
			Time.timeScale = 1.0f;

			TankCombat2DGameManager.Instance.ResetGame();

			SceneHelper.LoadScene( "TC2D_UI_", additive: true);

			ButtonStart.onClick.AddListener( delegate {
				StartGame = true;	
			});

			ToggleInputScheme.onClick.AddListener( delegate {
				PermanentSettings.CurrentTankInputScheme++;		// it will wrap in the validator

				UpdateDisplayedControlScheme();
			});

			ToggleCameraRotation.onClick.AddListener( delegate {
				PermanentSettings.CameraRotationEnabled = !PermanentSettings.CameraRotationEnabled;

				UpdateDisplayedCameraRotation();
			});

			UpdateDisplayedControlScheme();
			UpdateDisplayedCameraRotation();
		}

		bool StartGame;

		void UpdateDisplayedControlScheme()
		{
			DisplayInputScheme.text = "INPUT SCHEME:\n" + PermanentSettings.CurrentTankInputScheme;
		}

		void UpdateDisplayedCameraRotation()
		{
			DisplayCameraRotation.text = "CAM ROTATE:\n" +
				(PermanentSettings.CameraRotationEnabled ? "ENABLED" : "DISABLED");
		}

		void Update()
		{
			if (Input.GetKeyDown( KeyCode.C))
			{
				PermanentSettings.CurrentTankInputScheme++;
				UpdateDisplayedControlScheme();
			}

			// prevent the control scheme button from keeping focus so
			// that ENTER won't cycle it but will start game.
			EventSystem.current.SetSelectedGameObject( ButtonStart.gameObject);

			if (Time.timeSinceLevelLoad > 1.0f)
			{
				if (StartGame)
				{
					SceneHelper.LoadScene( "TankCombat2D_Newgame_");
				}
			}
			else
			{
				StartGame = false;
			}
		}
	}
}
