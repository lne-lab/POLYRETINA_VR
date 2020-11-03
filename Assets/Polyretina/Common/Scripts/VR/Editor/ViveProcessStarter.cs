using UnityEditor;
using Process = System.Diagnostics.Process;

namespace LNE.VR.UI
{
	using LNE.UI;

	public class ViveProcessStarter
	{
		private const string WIRELESS = "HtcConnectionUtility";
		private const string STEAM = "Steam";
		private const string STEAM_VR = "vrserver";
		private const string SRANIPAL = "SRanipal";

		private const string WIRELESS_PATH = @"C:\Program Files\VIVE Wireless\ConnectionUtility\HtcConnectionUtility.exe";
		private const string STEAM_PATH = @"C:\Program Files (x86)\Steam\Steam.exe";
		private const string STEAM_VR_PATH = @"C:\Users\thorn\Desktop\SteamVR.url";
		private const string SRANIPAL_PATH = @"C:\Users\Public\Desktop\SR_Runtime.lnk";

		[MenuItem("Polyretina/Vive Pro Eye/Start All", priority = WindowPriority.viveStartAll)]
		static void StartAll()
		{
			StartViveWireless();
			var ok = EditorUtility.DisplayDialog("LNE - Message", "Click OK once Vive Wireless has been configured.", "OK", "Cancel");
			if (ok == false)
			{
				return;
			}

			StartSteamVR();
			ok = EditorUtility.DisplayDialog("LNE - Message", "Click OK once SteamVR has been configured.", "OK", "Cancel");
			if (ok == false)
			{
				return;
			}

			StartEyeTracking();
		}

		[MenuItem("Polyretina/Vive Pro Eye/Start Vive Wireless", priority = WindowPriority.viveStartWireless)]
		static void StartViveWireless()
		{
			StartProcess(WIRELESS, Settings.WirelessPath, true, "Vive Wireless is already running.");
		}

		[MenuItem("Polyretina/Vive Pro Eye/Start Steam", priority = WindowPriority.viveStartSteam)]
		static void StartSteam()
		{
			StartProcess(STEAM, Settings.SteamPath, true, "Steam is already running.");
		}

		[MenuItem("Polyretina/Vive Pro Eye/Start SteamVR", priority = WindowPriority.viveStartSteamVR)]
		static void StartSteamVR()
		{
			StartProcess(STEAM_VR, Settings.SteamVRPath, true, "SteamVR is already running.");
		}

		[MenuItem("Polyretina/Vive Pro Eye/Start Eye Tracking", priority = WindowPriority.viveStartEyeTracking)]
		static void StartEyeTracking()
		{
			StartProcess(SRANIPAL, Settings.SRanipalPath, true, "Eye Tracking is already running.");
		}

		private static void StartProcess(string name, string path, bool displayDialog, string dialogMessage)
		{
			if (IsRunning(name))
			{
				if (displayDialog)
				{
					EditorUtility.DisplayDialog("LNE - Message", dialogMessage, "OK");
				}
			}
			else
			{
				Run(path);
			}
		}

		private static bool IsRunning(string processName)
		{
			if (processName == SRANIPAL)
			{
				return false;
			}
			else
			{
				return Process.GetProcessesByName(processName).Length > 0;
			}
		}

		private static void Run(string processPath)
		{
			try
			{
				Process.Start(processPath);
			}
			catch
			{
				EditorUtility.DisplayDialog("LNE - Message", $"The file \"{processPath}\" could not be found.", "OK");
			}
		}
	}
}
