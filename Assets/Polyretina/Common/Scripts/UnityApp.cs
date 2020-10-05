using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LNE
{
	using ArrayExts;

	/// <summary>
	/// Helpful Unity-related functions
	/// </summary>
	public static class UnityApp
	{
		/*
		 * Public properties
		 */

		public static string DataPath => Application.dataPath + "/";

		public static string ProjectPath => DataPath + "../";

		/*
		 * Private properties
		 */

		private static MonoBehaviour invoker;
		private static MonoBehaviour Invoker
		{
			get
			{
				if (invoker == null || invoker.isActiveAndEnabled == false)
				{
					var allBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();
					var activeBehaviours = allBehaviours.Where(behaviour => behaviour.isActiveAndEnabled);
					invoker = activeBehaviours.Length > 0 ? activeBehaviours[0] : null;
				}

				return invoker;
			}
		}

		/*
		 * Public methods
		 */

		/// <summary>
		/// Make path relative to Unity's data path
		/// </summary>
		public static string MakeRelative(string absolute, bool includeAssetFolder)
		{
			var assetFolder = includeAssetFolder ? "Assets/" : "";

			return assetFolder + new Uri(DataPath).MakeRelativeUri(new Uri(absolute))
													.ToString()
													.Replace("%20", " ");
		}

		/// <summary>
		/// Make path absolute to system drive
		/// </summary>
		public static string MakeAbsolute(string relative)
		{
			if (relative.StartsWith("Assets/"))
			{
				relative = relative.Substring("Assets/".Length);
			}

			return Path.GetFullPath(Path.Combine(DataPath, relative));
		}

		/// <summary>
		/// Start a coroutine
		/// </summary>
		public static Coroutine StartCoroutine(IEnumerator coroutine)
		{
			return Invoker?.StartCoroutine(coroutine);
		}

		/// <summary>
		/// Check if input axis exists
		/// </summary>
		public static bool InputAxisExists(string axis)
		{
			try
			{
				Input.GetAxis(axis);
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Returns string as Unity would display it in the editor
		/// </summary>
		public static string ToDisplayFormat(string str)
		{
			// asserts
			Debug.Assert(str != null, "Bad string");

			// remove leading underscore
			if (str.StartsWith("_"))
			{
				str = str.Remove(0, 1);
			}

			// replace underscores with spaces
			str = str.Replace("_", " ");

			// capitalise first character
			str = char.ToUpper(str[0]) + str.Substring(1);

			// add space where a lower case letter is followed by an uppercase letter
			for (int i = 1; i < str.Length; i++)
			{
				var lowerThenUpper = char.IsUpper(str[i]) && char.IsLower(str[i - 1]);
				var charThenNum = char.IsNumber(str[i]) && char.IsLetter(str[i - 1]);
				var numThenChar = char.IsLetter(str[i]) && char.IsNumber(str[i - 1]);

				if (lowerThenUpper || charThenNum || numThenChar)
				{
					str = str.Insert(i, " ");
					i++;
				}
			}

			return str;
		}

		public static string ToDisplayFormat(object obj)
		{
			return ToDisplayFormat(obj.ToString());
		}

		/// <summary>
		/// Quit in either the editor or player
		/// </summary>
		public static void Quit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		/// <summary>
		/// Find all objects of type in the scene (included inactive/disabled)
		/// </summary>
		public static T[] FindObjectsOfTypeAll<T>()
		{
			List<T> results = new List<T>();
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				var s = SceneManager.GetSceneAt(i);
				if (s.isLoaded)
				{
					var allGameObjects = s.GetRootGameObjects();
					for (int j = 0; j < allGameObjects.Length; j++)
					{
						var go = allGameObjects[j];
						results.AddRange(go.GetComponentsInChildren<T>(true));
					}
				}
			}
			return results.ToArray();
		}
	}
}
