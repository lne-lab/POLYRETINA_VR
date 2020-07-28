using UnityEngine;

#if VIVE_PRO_EYE
using ViveSR.anipal.Eye;
#endif

namespace LNE.ProstheticVision
{
	using VectorExts;

	public static class EyeGaze
	{
		public enum Source { None, EyeTracking, Mouse }

#if VIVE_PRO_EYE
		private static EyeData_v2 eyeData = new EyeData_v2();
#endif

		private static Vector2 lastPos;
		private static Vector2 lastMousePos;

		public static Vector2 VivePro
		{
			get
			{
#if VIVE_PRO_EYE
				// looking camera
				var camera = Prosthesis.Instance.Camera;

				// looking plane (placed infront of camera)
				var plane = new Plane(-camera.transform.forward, camera.transform.forward);

				// looking direction (start from camera with camera normal as vec3(0, 0, 1))
				var dir = new Vector3(0, 0, 1);

				try
				{
					SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out Vector3 origin, out dir);
				}
				catch 
				{
					dir = new Vector3(0, 0, 1);
				}

				var direction = new Ray(camera.transform.position, camera.transform.rotation * dir);

				// THIS
				//SRanipal_Eye_API.GetEyeData_v2(ref eyeData);
				//eyeData.verbose_data.right.pupil_position_in_sensor_area;
				// NOT THIS
				//SRanipal_Eye_v2.GetPupilPosition(EyeIndex.RIGHT, out Vector2 eyePos);
				//var theCube = GameObject.Find("The Cube");
				//theCube.transform.SetX(eyePos.x, Space.Self);
				//theCube.transform.SetY(eyePos.y, Space.Self);

				// looking position
				var position = AuxMath.Intersection(plane, direction);

				// position in screen space
				var screenPoint = camera.WorldToScreenPoint(position)
										.DivideXY(camera.pixelWidth, camera.pixelHeight)
										.SubtractXY(0.5f);

				return screenPoint;
#else
			return Vector2.zero;
#endif
			}
		}

		public static Vector2 Fove
		{
			get
			{
#if FOVE
				if (FoveInterface.IsEnabled() == false)
					return Vector2.zero;

				// looking camera
				var camera = Prosthesis.Instance.Camera;

				// looking plane (placed infront of camera)
				var plane = new Plane(-camera.transform.forward, camera.transform.forward);

				// looking direction (start from camera with camera normal as vec3(0, 0, 1))
				var direction = new Ray(camera.transform.position, camera.transform.rotation * FoveInterface.GetRightEyeVector());

				// looking position
				var position = AuxMath.Intersection(plane, direction);

				// position in screen space
				var screenPoint = camera.WorldToScreenPoint(position)
										.DivideXY(camera.pixelWidth, camera.pixelHeight)
										.SubtractXY(0.5f);

				return screenPoint;
#else
			return Vector2.zero;
#endif
			}
		}

		public static Vector2 Screen
		{
			get
			{
				if (Input.GetMouseButton(1))
				{
					var camera = Prosthesis.Instance.Camera;
					lastMousePos = Input.mousePosition.DivideXY(camera.pixelWidth, camera.pixelHeight).SubtractXY(0.5f);
					return lastMousePos;
				}
				else
				{
					return lastMousePos;
				}
			}
		}

		public static Vector2 LastPosition
		{
			get => lastPos;
			set => lastPos = value;
		}

		public static void Initialise(HeadsetModel headset)
		{
			LastPosition = Get(headset);
		}

		public static void Initialise(Source source, HeadsetModel headset = HeadsetModel.None60)
		{
			LastPosition = Get(source, headset);
		}

		public static Vector2 Get(HeadsetModel headset)
		{
			switch (headset)
			{
				case HeadsetModel.VivePro:	return VivePro;
				case HeadsetModel.Fove:		return Fove;
				default:					return Vector2.zero;
			}
		}

		public static Vector2 Get(Source source, HeadsetModel headset = HeadsetModel.None60)
		{
			switch (source)
			{
				case Source.EyeTracking:	return Get(headset);
				case Source.Mouse:			return Screen;
				case Source.None:			return Vector2.zero;
				default:					return Vector2.zero;
			}
		}

		public static Vector2 GetDelta(HeadsetModel headset)
		{
			var position = Get(headset);
			var delta = LastPosition - position;
			LastPosition = position;

			return delta;
		}

		public static Vector2 GetDelta(Source source, HeadsetModel headset = HeadsetModel.None60)
		{
			var position = Get(source, headset);
			var delta = LastPosition - position;
			LastPosition = position;

			return delta;
		}

		public static float GetPupilDilation()
		{
#if VIVE_PRO_EYE
			SRanipal_Eye_API.GetEyeData_v2(ref eyeData);
			return eyeData.verbose_data.right.pupil_diameter_mm;
#else
			return 0;
#endif
		}
	}
}
