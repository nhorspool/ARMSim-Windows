using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARMSim
{
	public static class ARMSimUtil
	{
		public enum EnvironmentType
		{
			Windows_MSCLR, //Windows with Microsoft's VM
			Windows_Mono, //Windows with Mono (unlikely)
			Linux_Mono, //Linux with Mono
			MacOSX_Mono, //MacOSX with Mono
		}
		//The code below is adapted from the first response at http://stackoverflow.com/questions/10138040/how-to-detect-properly-windows-linux-mac-operating-systems
		public static readonly bool IsRunningMono = Type.GetType("Mono.Runtime") != null;
		public static EnvironmentType RuntimeEnvironmentType
		{
			get
			{
				if (!IsRunningMono)
					return EnvironmentType.Windows_MSCLR;
				switch (Environment.OSVersion.Platform)
				{
					case PlatformID.Unix:
						return EnvironmentType.Linux_Mono;

					case PlatformID.MacOSX:
						return EnvironmentType.MacOSX_Mono;

					default:
						return EnvironmentType.Windows_Mono;
				}
			}
		}
		public static bool RunningOnWindows { get { return RuntimeEnvironmentType == EnvironmentType.Windows_Mono || RuntimeEnvironmentType == EnvironmentType.Windows_MSCLR; } }
		public static bool RunningOnUnixDerivative { get { return !RunningOnWindows; } }
	}
}
