using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace LNE
{
	public static class Settings
	{
		public static bool QuitForInvalidPaths
		{
			get => EditorPrefs.GetBool(nameof(QuitForInvalidPaths), true);
			set => EditorPrefs.SetBool(nameof(QuitForInvalidPaths), value);
		}

		public static bool FoveSupport
		{
			get => Symbols.IsDefined("FOVE");
			set
			{
				if (value)	Symbols.Define("FOVE");
				else		Symbols.Undefine("FOVE");
			}
		}

		public static bool ViveProEyeSupport
		{
			get => Symbols.IsDefined("VIVE_PRO_EYE");
			set
			{
				if (value)	Symbols.Define("VIVE_PRO_EYE");
				else		Symbols.Undefine("VIVE_PRO_EYE");
			}
		}

        public static string WirelessPath
        {
            get => EditorPrefs.GetString(nameof(WirelessPath), @"C:\Program Files\VIVE Wireless\ConnectionUtility\HtcConnectionUtility.exe");
            set => EditorPrefs.SetString(nameof(WirelessPath), value);
        }

        public static string SteamPath
        {
            get => EditorPrefs.GetString(nameof(SteamPath), @"C:\Program Files (x86)\Steam\Steam.exe");
            set => EditorPrefs.SetString(nameof(SteamPath), value);
        }

        public static string SteamVRPath
        {
            get => EditorPrefs.GetString(nameof(SteamVRPath), $@"C:\Users\{Environment.UserName}\Desktop\SteamVR.url");
            set => EditorPrefs.SetString(nameof(SteamVRPath), value);
        }

        public static string SRanipalPath
        {
            get => EditorPrefs.GetString(nameof(SRanipalPath), $@"C:\Users\{Environment.UserName}\Desktop\SR_Runtime.lnk");
            set => EditorPrefs.SetString(nameof(SRanipalPath), value);
        }

        public static bool VRInputSupport
		{
			get => EditorPrefs.GetBool(nameof(VRInputSupport), false);

			set
			{
                if (value && !VRInputSupport)
                {
                    AddVRInputSupport();
                }
                else if (!value && VRInputSupport)
                {
                    RemoveVRInputSupport();
                }

				EditorPrefs.SetBool(nameof(VRInputSupport), value);
			}
		}

		public static float DelayPlay
		{
			get => EditorPrefs.GetFloat(nameof(DelayPlay), 0);
			set => EditorPrefs.SetFloat(nameof(DelayPlay), value);
		}

		public static bool SaveRuntimeChangesAutomatically
		{
			get => EditorPrefs.GetBool(nameof(SaveRuntimeChangesAutomatically), false);
			set => EditorPrefs.SetBool(nameof(SaveRuntimeChangesAutomatically), value);
		}

        /*
         * VRInput Support
         */ 

        private static void AddVRInputSupport()
		{
            var input = GetInputManagerText();
            if (input.Contains(vrInput) == false)
            {
                File.AppendAllText(
                    GetInputManagerPath(), 
                    "\n  " + vrInput + "\n"
                );
            }
            
            Debug.Log("VRInput support added.");
		}

		private static void RemoveVRInputSupport()
		{
            var input = GetInputManagerText();
            if (input.Contains(vrInput))
            {
                File.WriteAllText(
                    GetInputManagerPath(), 
                    input.Replace("\n  " + vrInput + "\n", "")
                );
            }

            Debug.Log("VRInput support removed.");
        }

        private static string GetInputManagerText()
        {
            return File.ReadAllText(GetInputManagerPath());
        }

        private static string GetInputManagerPath()
        {
            var path = UnityApp.DataPath;
            path = path.Remove(path.IndexOf("Assets/"));
            path += "ProjectSettings/InputManager.asset";

            return path;
        }

        private const string vrInput = 
@"- serializedVersion: 3
    m_Name: Left Rift Thumbstick X Axis
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 0
    joyNum: 0
  - serializedVersion: 3
    m_Name: Left Rift Thumbstick Y Axis
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 1
    joyNum: 0
  - serializedVersion: 3
    m_Name: Right Rift Thumbstick X Axis
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 3
    joyNum: 0
  - serializedVersion: 3
    m_Name: Right Rift Thumbstick Y Axis
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 4
    joyNum: 0
  - serializedVersion: 3
    m_Name: Left Rift Index Trigger
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 8
    joyNum: 0
  - serializedVersion: 3
    m_Name: Left Rift Middle Trigger
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 10
    joyNum: 0
  - serializedVersion: 3
    m_Name: Right Rift Index Trigger
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 9
    joyNum: 0
  - serializedVersion: 3
    m_Name: Right Rift Middle Trigger
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 11
    joyNum: 0
  - serializedVersion: 3
    m_Name: Left Rift Thumbstick Hover
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 14
    joyNum: 0
  - serializedVersion: 3
    m_Name: Right Rift Thumbstick Hover
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 15
    joyNum: 0
  - serializedVersion: 3
    m_Name: Left Rift Thumb Rest Hover
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 14
    joyNum: 0
  - serializedVersion: 3
    m_Name: Right Rift Thumb Rest Hover
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 15
    joyNum: 0
  - serializedVersion: 3
    m_Name: Left Rift Index Trigger Hover
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 12
    joyNum: 0
  - serializedVersion: 3
    m_Name: Right Rift Index Trigger Hover
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 13
    joyNum: 0
  - serializedVersion: 3
    m_Name: Left Vive Trackpad X Axis
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 0
    joyNum: 0
  - serializedVersion: 3
    m_Name: Left Vive Trackpad Y Axis
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 1
    joyNum: 0
  - serializedVersion: 3
    m_Name: Right Vive Trackpad X Axis
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 3
    joyNum: 0
  - serializedVersion: 3
    m_Name: Right Vive Trackpad Y Axis
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 4
    joyNum: 0
  - serializedVersion: 3
    m_Name: Left Vive Trigger
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 8
    joyNum: 0
  - serializedVersion: 3
    m_Name: Right Vive Trigger
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 9
    joyNum: 0
  - serializedVersion: 3
    m_Name: Left Vive Grip
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 10
    joyNum: 0
  - serializedVersion: 3
    m_Name: Right Vive Grip
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 11
    joyNum: 0";
    }
}
