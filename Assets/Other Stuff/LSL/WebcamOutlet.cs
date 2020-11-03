#pragma warning disable 649

using UnityEngine;
using LSL;

namespace LNE.LabStreamingLayer
{
	/// <summary>
	/// Stream a webcam 
	/// </summary>
	public class WebcamOutlet : Outlet<char>
	{
		/*
		 * Editor fields
		 */

		[Space]

		[SerializeField]
		private Webcam webcam;

		/*
		 * Private fields
		 */

		private Color32[] pixels;

		/*
		 * Public properties
		 */

		public override string StreamType => "!VideoRaw";

		public override int NumChannels => webcam.Texture.width * webcam.Texture.height * 3;

		/*
		 * Unity callbacks
		 */

		protected override void Start()
		{
			base.Start();

			pixels = new Color32[webcam.Texture.width * webcam.Texture.height];
		}

		/*
		 * Protected methods
		 */

		protected override void DefineMetaData(liblsl.StreamInfo info)
		{
			Debug.LogWarning("VideoOutlet meta data not defined.");
		}

		protected override void UpdateData(char[] data)
		{
			webcam.Texture.GetPixels32(pixels);

			int i = 0;
			foreach (var pixel in pixels)
			{
				data[i++] = (char)pixel.r;
				data[i++] = (char)pixel.g;
				data[i++] = (char)pixel.b;
			}
		}
	}
}
