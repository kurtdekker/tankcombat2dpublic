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
	public partial class TC2DPlayer
	{
		// inputs:
		float turnTank;
		float desiredDrive;
		bool fireRequested;

		void GatherInputs()
		{
			turnTank = 0.0f;
			desiredDrive = 0.0f;
			desiredTurretHeading = currentTurretHeading;
			fireRequested = false;

			switch( PermanentSettings.CurrentTankInputScheme)
			{
			default :
			case TankInputScheme.TURN_AND_DRIVE :
				GatherInput_TurnAndDrive();
				break;
			case TankInputScheme.DPAD_DIRECTIONAL :
				GatherInput_DPADDirectional();
				break;
			}

			GatherInput_TurretControl();

			GatherInput_FiringIntent();

			GatherInput_FinalClamp();
		}

		void GatherInput_TurnAndDrive()
		{
			turnTank -= Input.GetAxisRaw( "Horizontal");
			desiredDrive += Input.GetAxisRaw( "Vertical");

			if (Input.GetKeyDown( KeyCode.Space) ||
				Input.GetKeyDown( KeyCode.LeftControl) ||
				false)
			{
				fireRequested = true;
			}
		}

		void GatherInput_DPADDirectional()
		{
			// Easy hack to keep down jittering around a heading.
			// The proper way would be to make a smaller final turn to the
			// requested angle, but that would require more plumbing.
			const float CloseEnoughAngle = 5.0f;

			Vector2 direction = new Vector2( Input.GetAxisRaw( "Horizontal"), Input.GetAxisRaw( "Vertical"));

			var magnitude = direction.magnitude;

			if (magnitude > 0.2f)
			{
				float angle = Mathf.Atan2( -direction.x, +direction.y) * Mathf.Rad2Deg;

				float deltaAngle = Mathf.DeltaAngle( angle, tankHeading);

				float driveSign = +1.0f;		// forward

				// are you commanding essentially a "back up" maneuver?
				if (deltaAngle < -90 || deltaAngle > 90)
				{
					angle += 180.0f;
					driveSign = -1.0f;		// backwards
				}

				// recompute delta from now-possibly-inverted heading angle
				deltaAngle = Mathf.DeltaAngle( angle, tankHeading);

				if (deltaAngle < CloseEnoughAngle)
				{
					turnTank = +1.0f;
				}
				if (deltaAngle > CloseEnoughAngle)
				{
					turnTank = -1.0f;
				}

				// Attenuate drive magnitude if you're too far off of direction,
				// so that you first tend to turn in place with less movement.
				float offAxisNess = Mathf.InverseLerp( 50, CloseEnoughAngle * 2, Mathf.Abs( deltaAngle));
				magnitude *= offAxisNess;

				desiredDrive += magnitude * driveSign;
			}
		}

		void GatherInput_FiringIntent()
		{
			if (Input.GetKeyDown( KeyCode.Space) ||
				Input.GetKeyDown( KeyCode.LeftControl) ||
				false)
			{
				fireRequested = true;
			}
		}

		void GatherInput_TurretControl()
		{
			if (Input.GetKey( KeyCode.Alpha1))
			{
				desiredTurretHeading = +MaxTurretAngle;
			}
			if (Input.GetKey( KeyCode.Alpha2))
			{
				desiredTurretHeading = 0;
			}
			if (Input.GetKey( KeyCode.Alpha3))
			{
				desiredTurretHeading = -MaxTurretAngle;
			}
		}

		void GatherInput_FinalClamp()
		{
			turnTank = Mathf.Clamp( turnTank, -1, 1);
			desiredDrive = Mathf.Clamp( desiredDrive, -1, 1);
		}
	}
}
