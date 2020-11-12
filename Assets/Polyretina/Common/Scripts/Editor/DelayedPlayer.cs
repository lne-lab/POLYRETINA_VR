using UnityEditor;
using System.Threading;

namespace LNE
{
	[InitializeOnLoad]
	public static class DelayedPlayer
	{
		static DelayedPlayer()
		{
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.EnteredPlayMode)
			{
				Thread.Sleep((int)Settings.DelayPlay * 1000);
			}
		}
	}
}
