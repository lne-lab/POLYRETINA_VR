using System;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Tags
{
	using ArrayExts;	

	public class TagConsole : Singleton<TagConsole>
	{
		private const int MAX_LINES = 45;

#pragma warning disable 649
		[SerializeField]
		private Text _console;
#pragma warning restore 649

		public void WriteLine(string tag)
		{
			_console.text += $"[{DateTime.Now:HH:mm:ss}] {tag}\n";

			AutoScroll();
		}

		private void AutoScroll()
		{
			var lines = _console.text.Split('\n').Remove("");
			if (lines.Length > MAX_LINES)
			{
				lines = lines.Subarray(1, lines.Length - 1);
				_console.text = lines.Converge((a, b) => $"{a}\n{b}") + "\n";
			}
		}
	}
}
