using System.CodeDom;
using System.Collections.Generic;
using UnityEngine;

namespace LNE.ProstheticVision
{
	public enum ElectrodePattern
	{
		POLYRETINA,
		ArgusII
	}

	public static class ElectrodePatternExtensions
	{
		public static Vector2[] GetElectrodePositions(this ElectrodePattern that, ElectrodeLayout layout, float fov = 120)
		{
			switch (that)
			{
				case ElectrodePattern.POLYRETINA:	return Polyretina(layout, fov);
				case ElectrodePattern.ArgusII:		return ArgusII();
				default:							return null;
			}
		}
		
		private static Vector2[] Polyretina(ElectrodeLayout layout, float fov)
		{
			var pattern = new List<Vector2>();
			
			var retinalRadius = CoordinateSystem.FovToRetinalRadius(fov);

			var verticalSpacing = layout.GetSpacing(LayoutUsage.Anatomical);
			var horizontalSpacing = verticalSpacing * Mathf.Sqrt(3) / 2f;

			var offset = 0f;
			for (var x = -retinalRadius; x < retinalRadius; x += horizontalSpacing)
			{
				offset = offset == 0f ? verticalSpacing / 2f : 0f;
				for (var y = -retinalRadius + offset; y < retinalRadius; y += verticalSpacing)
				{
					var position = new Vector2(x, y);
					if (position.magnitude <= retinalRadius)
					{
						pattern.Add(position);
					}
				}
			}

			return pattern.ToArray();
		}

		private static Vector2[] ArgusII()
		{
			var pattern = new Vector2[60];

			// general pattern
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 6; j++)
				{
					pattern[i * 6 + j] = new Vector2(i * 575, j * 575);
				}
			}

			// centralise device
			for (int i = 0; i < 60; i++)
			{
				pattern[i].x -= 575 * 4.5f;
				pattern[i].y -= 575 * 2.5f;
			}

			return pattern;
		}
	}
}
