using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankCombat2D
{
	public static class PermanentSettings
	{
		private static string s_TankInputScheme = "TankInputScheme";
		public static TankInputScheme CurrentTankInputScheme
		{
			get
			{
				var tis = (TankInputScheme)PlayerPrefs.GetInt(s_TankInputScheme, (int)DefaultTankInputScheme);
				return ValidateTankInputScheme( tis);
			}
			set
			{
				int i = (int)ValidateTankInputScheme( value);
				PlayerPrefs.SetInt(s_TankInputScheme, i);
			}
		}
		static TankInputScheme DefaultTankInputScheme = TankInputScheme.DPAD_DIRECTIONAL;
		static TankInputScheme ValidateTankInputScheme( TankInputScheme tis)
		{
			if ((tis < 0) || (tis >= TankInputScheme.MAXIMUM))
			{
				// not the default: this is chosen to allow easy cycling / wrapping
				tis = 0;
			}

			return tis;
		}

		private static string s_CameraRotationEnabled = "CameraRotationEnabled";
		public static bool CameraRotationEnabled
		{
			get
			{
				return PlayerPrefs.GetInt(s_CameraRotationEnabled, 0) != 0;
			}
			set
			{
				PlayerPrefs.SetInt( s_CameraRotationEnabled, value ? 1 : 0);
			}
		}
	}
}
