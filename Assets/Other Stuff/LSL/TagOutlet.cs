using LSL;

namespace LNE.LabStreamingLayer
{
	/// <summary>
	/// Stream tags
	/// </summary>
	public class TagOutlet : Outlet<string>
	{
		/*
		 * Private fields
		 */

		private string _label;

		/*
		 * Public properties
		 */

		public override string StreamType => "Tag";

		public override int NumChannels => 1;

		/*
		 * Public methods
		 */

		public void PushSample(string label)
		{
			_label = label;
			PushSample();
		}

		/*
		 * Protected methods
		 */

		protected override void DefineMetaData(liblsl.StreamInfo info)
		{
			var channels = info.desc().append_child("channels");
			var channelTypes = new[] { "Tag" };
			var channelUnits = new[] { "tag" };
			for (int i = 0; i < NumChannels; i++)
			{
				channels.append_child("channel")
						.append_child_value("label", channelTypes[i])
						.append_child_value("type", channelTypes[i])
						.append_child_value("unit", channelUnits[i]);
			}
		}

		protected override void UpdateData(string[] data)
		{
			data[0] = _label;
		}
	}
}
