using System.Collections.Generic;
using UnityEngine;

namespace LNE.UI
{
	public class GUIOptions
	{
		public int width = -1;
		public int height = -1;
		public int maxWidth = -1;
		public int maxHeight = -1;

		private GUIStyle _style;
		public GUIStyle style
		{
			get => _style;
			set => _style = new GUIStyle(value);
		}

		public Color? textColour
		{
			get => style != null ? style.normal.textColor : default;
			set
			{
				if (style != null && value.HasValue)
				{
					style.normal.textColor = value.Value;
				}
			}
		}

		public int? textSize
		{
			get => style != null ? style.fontSize : default;
			set
			{
				if (style != null && value.HasValue)
				{
					style.fontSize = value.Value;
				}
			}
		}

		public GUILayoutOption[] layout
		{
			get
			{
				var options = new List<GUILayoutOption>();

				if (width >= 0)
				{
					options.Add(GUILayout.Width(width));
				}

				if (height >= 0)
				{
					options.Add(GUILayout.Height(height));
				}

				if (maxWidth >= 0)
				{
					options.Add(GUILayout.Width(maxWidth));
				}

				if (maxHeight >= 0)
				{
					options.Add(GUILayout.Height(maxHeight));
				}

				return options.ToArray();
			}
		}
	}
}
