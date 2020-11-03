#pragma warning disable 649

using UnityEngine;
using LSL;

namespace LNE.LabStreamingLayer
{
	using ProstheticVision;
	
	/// <summary>
	/// Stream eye gaze
	/// </summary>
	public class EyeGazeOutlet : Outlet<float>
	{
		/*
		 * Editor fields
		 */

		[Space]

		[SerializeField]
		private EyeGaze.Source eyeGazeSource;

		[SerializeField]
		private HeadsetModel headset;

		/*
		 * Public properties
		 */

		public override string StreamType => "Gaze";

		public override int NumChannels => 3;

		/*
		 * Protected methods
		 */

		protected override void DefineMetaData(liblsl.StreamInfo info)
		{
			var channels = info.desc().append_child("channels");
			var channelTypes = new[] { "PositionX", "PositionY", "Diameter" };
			var channelUnits = new[] { "normalized", "normalized", "mm" };
			for (int i = 0; i < NumChannels; i++)
			{
				channels.append_child("channel")
						.append_child_value("label", channelTypes[i])
						.append_child_value("eye", "right")
						.append_child_value("type", channelTypes[i])
						.append_child_value("unit", channelUnits[i])
						.append_child_value("coordinate_system", "world-space")

						.append_child_value("manufacturer", GetManufacturer())
						.append_child_value("model", GetModel())
						.append_child_value("serial_number", GetSerialNumber());
			}
		}

		protected override void UpdateData(float[] data)
		{
			var eyeGaze = EyeGaze.Get(eyeGazeSource, headset);
			var dilation = EyeGaze.GetPupilDilation();

			data[0] = eyeGaze.x;
			data[1] = eyeGaze.y;
			data[2] = dilation;
		}
		
		/*
		 * Private methods
		 */

		private string GetManufacturer()
		{
			switch (eyeGazeSource)
			{
				case EyeGaze.Source.None:			return "None";
				case EyeGaze.Source.Mouse:			return "Unknown";
				case EyeGaze.Source.EyeTracking:
					switch (headset)
					{
						case HeadsetModel.Fove:		return "FOVE";
						case HeadsetModel.VivePro:	return "HTC";
						default:					return "Unknown";
					}
				default:							return "Unknown";
			}
		}

		private string GetModel()
		{
			switch (eyeGazeSource)
			{
				case EyeGaze.Source.None:			return "None";
				case EyeGaze.Source.Mouse:			return "Unknown";
				case EyeGaze.Source.EyeTracking:
					switch (headset)
					{
						case HeadsetModel.Fove:		return "FOVE";
						case HeadsetModel.VivePro:	return "VIVE Pro Eye";
						default:					return "Unknown";
					}
				default:							return "Unknown";
			}
		}

		private string GetSerialNumber()
		{
			return "0";
		}
	}
}
