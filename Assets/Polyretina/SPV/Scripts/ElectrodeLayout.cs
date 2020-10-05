using System;

namespace LNE.ProstheticVision
{
	/// <summary>
	/// Electrode layouts, diameter and spacing
	/// </summary>
	public enum ElectrodeLayout
	{
		_40x60,
		_60x90,
		_80x120,
		_100x150,

		_80x150,

		_225x575
	}

	public enum LayoutUsage
	{
		Theoretical,
		Anatomical
	}

	public static class ElectrodeLayoutExtensions
	{
		public static float[] ToValue(this ElectrodeLayout layout)
		{
			switch (layout)
			{
				case ElectrodeLayout._40x60: return new[] { 40f, 60f };
				case ElectrodeLayout._60x90: return new[] { 60f, 90f };
				case ElectrodeLayout._80x120: return new[] { 80f, 120f };
				case ElectrodeLayout._100x150: return new[] { 100f, 150f };

				case ElectrodeLayout._80x150: return new[] { 80f, 150f };

				case ElectrodeLayout._225x575: return new[] { 225f, 575f };

				default: throw new Exception();
			}
		}

		public static float[] ToAnatomicalValue(this ElectrodeLayout layout)
		{
			switch (layout)
			{
				case ElectrodeLayout._40x60: return new[] { 40f * (58.3f / 60f), 58.3f };
				case ElectrodeLayout._60x90: return new[] { 60f * (87.5f / 90f), 87.5f };
				case ElectrodeLayout._80x120: return new[] { 80f * (116.8f / 120f), 116.8f };
				case ElectrodeLayout._100x150: return new[] { 100f * ( 136f / 150f), 136f };

				case ElectrodeLayout._80x150: return new[] { 80f * (136f / 150f), 136f };

				case ElectrodeLayout._225x575: return new[] { 225f, 575f };

				default: throw new Exception();
			}
		}

		public static float GetDiameter(this ElectrodeLayout layout, LayoutUsage usage)
		{
			switch (usage)
			{
				case LayoutUsage.Theoretical:	return layout.ToValue()[0];
				case LayoutUsage.Anatomical:	return layout.ToAnatomicalValue()[0];
				default:						return layout.ToAnatomicalValue()[0];
			}
		}

		public static float GetRadius(this ElectrodeLayout layout, LayoutUsage usage)
		{
			return layout.GetDiameter(usage) / 2f;
		}

		public static float GetSpacing(this ElectrodeLayout layout, LayoutUsage usage)
		{
			switch (usage)
			{
				case LayoutUsage.Theoretical:	return layout.ToValue()[1];
				case LayoutUsage.Anatomical:	return layout.ToAnatomicalValue()[1];
				default:						return layout.ToAnatomicalValue()[1];
			}
		}
	}
}
