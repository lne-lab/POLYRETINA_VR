using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LNE.IO;
using LNE.UI.Attributes;

namespace LNE.Studies.FadingV2
{
	public class RotationRecorder : MonoBehaviour
	{
		[Path]
		public string path;
		public string participant;

		[Space]

		public Transform target;

		private FadingStudy2 _study;
		private List<FrameData_Rotation> _frames;

		private CSV _csv;

		void Awake()
		{
			_study = FindObjectOfType<FadingStudy2>();
			_frames = new List<FrameData_Rotation>();

			_csv = new CSV();
			_csv.AppendRow("participant", "trial", "time", "px", "py", "pz", "rx", "ry", "rz", "rw");
		}

		void FixedUpdate()
		{
			if (_study.trialId < 0)
				return;

			_frames.Add(new FrameData_Rotation
			{
				participant = participant,
				trialId = _study.trialId,
				time = Time.time,
				px = target.position.x,
				py = target.position.y,
				pz = target.position.z,
				rx = target.rotation.x,
				ry = target.rotation.y,
				rz = target.rotation.z,
				rw = target.rotation.w
			});

			_csv.AppendRow(
				participant,
				_study.trialId,
				Time.time,
				target.position.x,
				target.position.y,
				target.position.z,
				target.rotation.x,
				target.rotation.y,
				target.rotation.z,
				target.rotation.w
			);
		}

		void OnApplicationQuit()
		{
			//SaveAsJson(fileId);
			SaveAsCsv();
		}

		public void Save(int trial)
		{
			_csv.SaveWStream(path + $"Fad_Cnd_Rot_{trial}_{participant}.csv");
		}

		//private void SaveAsJson(long id)
		//{
		//	var data = new TestData_Rotation { frames = _frames.ToArray() };
		//	var json = JsonUtility.ToJson(data, true);

		//	File.WriteAllText(
		//		path + $"Fading_Rotation_{participant}_json_{id}.json", json
		//	);
		//}

		private void SaveAsCsv()
		{
			var csv = new CSV();
			csv.AppendRow("participant", "trial", "time", "px", "py", "pz", "rx", "ry", "rz", "rw");

			foreach (var frame in _frames)
			{
				csv.AppendRow(
					frame.participant,
					frame.trialId,
					frame.time,
					frame.px,
					frame.py,
					frame.pz,
					frame.rx,
					frame.ry,
					frame.rz,
					frame.rw
				);
			}

			csv.SaveWStream(path + $"Fad_Cnd_Rot_All_{participant}.csv");
		}
	}

	[Serializable]
	public class TestData_Rotation
	{
		public FrameData_Rotation[] frames;
	}

	[Serializable]
	public class FrameData_Rotation
	{
		public string participant;
		public int trialId;

		public float time;

		// position
		public float px;
		public float py;
		public float pz;

		// rotation
		public float rx;
		public float ry;
		public float rz;
		public float rw;
	}
}
