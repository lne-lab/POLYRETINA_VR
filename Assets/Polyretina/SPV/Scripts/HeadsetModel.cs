using System;
using UnityEngine;

namespace LNE.ProstheticVision
{
	/// <summary>
	/// Headset model
	/// </summary>
	public enum HeadsetModel
	{
		Fove,
		VivePro,

		None60,
		None120,

		FHD60
	}

	public enum Axis
	{
		Horizontal,
		Vertical
	}

	public static class HeadsetModelExtensions
	{
		public static float GetFieldOfView(this HeadsetModel headset, Axis axis)
		{
			float vertical;
			switch (headset)
			{
				case HeadsetModel.Fove:		vertical = 95;	break;
				case HeadsetModel.VivePro:	vertical = 110; break;

				case HeadsetModel.None60:	vertical = 60;	break;
				case HeadsetModel.None120:	vertical = 120; break;

				case HeadsetModel.FHD60:	vertical = 60;	break;

				default:					throw new Exception();
			}

			switch (axis)
			{
				case Axis.Horizontal:	return AuxMath.HorizontalFoV(vertical, headset.GetAspectRatio());
				case Axis.Vertical:		return vertical;

				default:				throw new Exception();
			}
		}

		public static Vector2 GetResolution(this HeadsetModel headset)
		{
			switch (headset)
			{
				case HeadsetModel.Fove:		return new Vector2(1792, 2016);
				case HeadsetModel.VivePro:	return new Vector2(2036, 2260);

				case HeadsetModel.None60:	return new Vector2(1000, 1000);
				case HeadsetModel.None120:	return new Vector2(1000, 1000);

				case HeadsetModel.FHD60:	return new Vector2(1920, 1080);

				default:					throw new Exception();
			}
		}

		public static int GetRefreshRate(this HeadsetModel headset)
		{
			switch (headset)
			{
				case HeadsetModel.Fove:		return 70;
				case HeadsetModel.VivePro:	return 90;

				case HeadsetModel.None60:	return 90;	// this should be 100
				case HeadsetModel.None120:	return 100;

				case HeadsetModel.FHD60:	return 100;

				default:					throw new Exception();
			}
		}

		public static int GetWidth(this HeadsetModel headset)
		{
			return (int)headset.GetResolution().x;
		}

		public static int GetHeight(this HeadsetModel headset)
		{
			return (int)headset.GetResolution().y;
		}

		public static float GetAspectRatio(this HeadsetModel headset)
		{
			return (float)headset.GetWidth() / headset.GetHeight();
		}
		
		public static float GetRetinalRadius(this HeadsetModel headset, Axis axis)
		{
			return CoordinateSystem.FovToRetinalRadius(headset.GetFieldOfView(axis));
		}

		public static Vector2 GetRetinalRadius(this HeadsetModel headset)
		{
			return new Vector2(
				headset.GetRetinalRadius(Axis.Horizontal),
				headset.GetRetinalRadius(Axis.Vertical)
			);
		}

		public static float GetRetinalDiameter(this HeadsetModel headset, Axis axis)
		{
			return CoordinateSystem.FovToRetinalDiameter(headset.GetFieldOfView(axis));
		}

		public static Vector2 GetRetinalDiameter(this HeadsetModel headset)
		{
			return new Vector2(
				headset.GetRetinalDiameter(Axis.Horizontal),
				headset.GetRetinalDiameter(Axis.Vertical)
			);
		}

		public static Texture2D CreateTexture(this HeadsetModel headset)
		{
			var texture = new Texture2D(headset.GetWidth(), headset.GetHeight(), TextureFormat.RGBAFloat, false, true)
			{
				anisoLevel = 0,
				filterMode = FilterMode.Point
			};

			return texture;
		}

		public static RenderTexture CreateRenderTexture(this HeadsetModel headset)
		{
			var texture = new RenderTexture(headset.GetWidth(), headset.GetHeight(), 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
			{
				anisoLevel = 0,
				antiAliasing = 1,
				enableRandomWrite = true,
				filterMode = FilterMode.Point
			};

			texture.Create();

			return texture;
		}
	}
}
