using System;
using System.Threading;
using UnityEngine;
using UnityEditor;

namespace LNE.UI
{
	public class SettingsWindow : EditorWindow
	{
		private string helpMessage;

		[MenuItem("Polyretina/Settings", priority = WindowPriority.settingsWindow)]
		static void ShowWindow()
		{
			var window = GetWindow<SettingsWindow>("Settings");
			window.minSize = new Vector2(450, 350);
			window.maxSize = new Vector2(450, 350);
			window.Show();
		}

		void OnGUI()
		{
			helpMessage = "";

			/*
			 * Paths
			 */

			UnityGUI.Header("Paths", true);
			Settings.QuitForInvalidPaths = UnityGUI.Toggle("Quit For Invalid Paths", Settings.QuitForInvalidPaths);
			if (UnityGUI.OnMouseHoverPrevious())
			{
				helpMessage = "Application will quit on initialisation if any fields that are using the Path Attribute have invalid paths.";
			}

			UnityGUI.Header("VR", true);

			/*
			 * Eye tracking SDKs
			 */

			UnityGUI.IndentLevel--;
			UnityGUI.Label("Eye Tracking SDKs");
			UnityGUI.IndentLevel++;
			Settings.FoveSupport = UnityGUI.Toggle("FOVE", Settings.FoveSupport);
			if (UnityGUI.OnMouseHoverPrevious())
			{
				helpMessage = "Enable FOVE eye tracking. Will produce errors if the FOVE Unity Package has not been imported.";
			}

			Settings.ViveProEyeSupport = UnityGUI.Toggle("Vive Pro Eye", Settings.ViveProEyeSupport);
			if (UnityGUI.OnMouseHoverPrevious())
			{
				helpMessage = "Enable Vive Pro Eye eye tracking. Will produce errors if the SRanipal Unity Package has not been imported.";
			}

			UnityGUI.Space();

			/*
			 * VRInput support
			 */ 

			Settings.VRInputSupport = UnityGUI.Toggle("VRInput Support", Settings.VRInputSupport);
			if (UnityGUI.OnMouseHoverPrevious())
			{
				helpMessage = "Add VRInput support to the Input Manager. Recommended if using the Vive Pro Eye.";
			}

			/*
			 * Delay play
			 */

			var floatGui = new GUIOptions
			{
				style = EditorStyles.numberField,
				width = 185
			};

			UnityGUI.BeginHorizontal();
			Settings.DelayPlay = UnityGUI.Float("Delay", Settings.DelayPlay, floatGui);
			if (UnityGUI.OnMouseHoverPrevious())
			{
				helpMessage = "Delay play for some seconds after clicking the play button. Useful when testing in VR.";
			}

			UnityGUI.Label("seconds");

			UnityGUI.EndHorizontal();

			/*
			 * Vive app paths
			 */

			UnityGUI.Space();

			var buttonGui = new GUIOptions
			{
				style = EditorStyles.miniButton,
				maxWidth = 145
			};

			if (UnityGUI.Button("Set Vive application paths", buttonGui))
			{
				var yes = EditorUtility.DisplayDialog("LNE - Message", "Do you use the HTC Connection Utility?", "Yes", "No");
				if (yes)
				{
					EditorUtility.DisplayDialog("LNE - Message", "Locate the HTC Connection Utility.", "Ok");
					Settings.WirelessPath = EditorUtility.OpenFilePanel("Locate the HTC Connection Utility", Settings.WirelessPath, "exe");
				}

				EditorUtility.DisplayDialog("LNE - Message", "Locate Steam.", "Ok");
				Settings.SteamPath = EditorUtility.OpenFilePanel("Locate Steam", Settings.SteamPath, "exe");

				EditorUtility.DisplayDialog("LNE - Message", "Locate Steam VR.", "Ok");
				Settings.SteamVRPath = EditorUtility.OpenFilePanel("Locate Steam VR", Settings.SteamVRPath, "url");

				yes = EditorUtility.DisplayDialog("LNE - Message", "Do you use the SRanipal runtime?", "Yes", "No");
				if (yes)
				{
					EditorUtility.DisplayDialog("LNE - Message", "Locate the SRanipal runtime.", "Ok");
					Settings.SRanipalPath = EditorUtility.OpenFilePanel("Locate SRanipal runtime", Settings.SRanipalPath, "lnk");
				}
			}

			var labelGui = new GUIOptions
			{
				style = EditorStyles.label,
				textColour = new Color(.2f, .2f, .2f),
				textSize = 10
			};

			UnityGUI.IndentLevel--;
			UnityGUI.Label(Settings.WirelessPath != "" ? Settings.WirelessPath : "<not set>", labelGui);
			UnityGUI.Label(Settings.SteamPath != "" ? Settings.SteamPath : "<not set>", labelGui);
			UnityGUI.Label(Settings.SteamVRPath != "" ? Settings.SteamVRPath : "<not set>", labelGui);
			UnityGUI.Label(Settings.SRanipalPath != "" ? Settings.SRanipalPath : "<not set>", labelGui);
			UnityGUI.IndentLevel++;

			/*
			 * Prosthetic vision
			 */

			UnityGUI.Header("Prosthetic Vision", true);
			Settings.SaveRuntimeChangesAutomatically = UnityGUI.Toggle("Save Runtime Changes", Settings.SaveRuntimeChangesAutomatically);
			if (UnityGUI.OnMouseHoverPrevious())
			{
				helpMessage = "Save runtime changes made to ImageRenderers automatically.";
			}

			UnityGUI.IndentLevel--;
			UnityGUI.FlexibleSpace();
			UnityGUI.HelpLabel(helpMessage, MessageType.Info);
		}

		void OnInspectorUpdate()
		{
			Repaint();
		}
	}
}
