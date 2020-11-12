#pragma warning disable 649

using UnityEngine;
using LSL;

namespace LNE.LabStreamingLayer
{
	public abstract class Outlet<T> : MonoBehaviour
	{
		/*
		 * Editor fields
		 */

		[SerializeField]
		private string _streamName = "";

		[SerializeField]
		private float _sampleRate = 0;

		[SerializeField]
		private bool _onStart = false;

		/*
		 * Private fields
		 */

		private liblsl.StreamInfo info;
		private liblsl.StreamOutlet outlet;

		private T[] data;

		private const int VARIABLE_SRATE = 0;

		/*
		 * Private properties
		 */

		private liblsl.channel_format_t ChannelFormat
		{
			get
			{
				if (typeof(T) == typeof(float))
				{
					return liblsl.channel_format_t.cf_float32;
				}
				else if (typeof(T) == typeof(char))
				{
					return liblsl.channel_format_t.cf_int8;
				}
				else if (typeof(T) == typeof(int))
				{
					return liblsl.channel_format_t.cf_int32;
				}
				else if (typeof(T) == typeof(string))
				{
					return liblsl.channel_format_t.cf_string;
				}
				else if (typeof(T) == typeof(double))
				{
					return liblsl.channel_format_t.cf_double64;
				}
				else if (typeof(T) == typeof(short))
				{
					return liblsl.channel_format_t.cf_int16;
				}
				else
				{
					return liblsl.channel_format_t.cf_undefined;
				}
			}
		}

		/*
		 * Unity callbacks
		 */

		protected virtual void Start()
		{
			if (_onStart == false)
				return;

			Init(_streamName, _sampleRate);
		}

		protected virtual void FixedUpdate()
		{ 
			if (_sampleRate == VARIABLE_SRATE)
				return;

			var updateCount = Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime);
			var updatesPerFrame = Mathf.RoundToInt(1 / (_sampleRate * Time.fixedDeltaTime));
			
			if (updateCount % updatesPerFrame == 0)
			{
				PushSample();
			}
		}

		/*
		 * Public methods
		 */

		public void Init(string streamName, float sampleRate)
		{
			_streamName = streamName;
			_sampleRate = Mathf.Min(sampleRate, 1 / Time.fixedDeltaTime);

			info = new liblsl.StreamInfo(
				_streamName,
				StreamType,
				NumChannels,
				_sampleRate,
				ChannelFormat,
				OutletName
			);

			DefineMetaData(info);

			outlet = new liblsl.StreamOutlet(info);

			data = new T[NumChannels];
		}

		public void PushSample()
		{
			UpdateData(data);

			if (data is float[])
			{
				outlet?.push_sample(data as float[]);
			}
			else if (data is char[])
			{
				outlet?.push_sample(data as char[]);
			}
			else if (data is int[])
			{
				outlet?.push_sample(data as int[]);
			}
			else if (data is string[])
			{
				outlet?.push_sample(data as string[]);
			}
			else if (data is double[])
			{
				outlet?.push_sample(data as double[]);
			}
			else if (data is short[])
			{
				outlet?.push_sample(data as short[]);
			}
		}

		/*
		 * Abstract / Virtual interface
		 */

		public abstract string StreamType { get; }
		public abstract int NumChannels { get; }
		public virtual string OutletName => Application.productName;

		protected abstract void DefineMetaData(liblsl.StreamInfo info);

		protected abstract void UpdateData(T[] data);
	}
}
