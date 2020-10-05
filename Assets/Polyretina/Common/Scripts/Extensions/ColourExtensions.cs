using UnityEngine;

namespace LNE.ColourExts
{
	public static class ColourExtensions
	{
		public static bool EqualTo(this Color that, Color other)
		{
			if (that == null || other == null)
				return false;

			return Mathf.Approximately(that.r, other.r) &&
					Mathf.Approximately(that.g, other.g) &&
					Mathf.Approximately(that.b, other.b) &&
					Mathf.Approximately(that.a, other.a);
		}
	}
}
