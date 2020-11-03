//using UnityEngine;

//namespace LNE.Studies.FadingV1
//{
//	using IO;
//	using UI.Attributes;

//	public class ViewReconstruction : MonoBehaviour
//	{
//#pragma warning disable 649
//		[SerializeField, Path(isFile = true)]
//		private string path;

//		[SerializeField]
//		private Transform target;

//		[SerializeField]
//		private Vector3 positionOffset;

//		[SerializeField]
//		private int speed = 1;
//#pragma warning restore 649

//		private FrameData_Rotation[] frames;
//		private int firstId = 0;
//		private int frameId = 0;
//		private int finalId = 0;
//		private bool playing = false;

//		private int trialId => frames[firstId + frameId].trialId;

//		void Start()
//		{
//			var csv = new CSV();
//			csv.LoadWStream(path);

//			frames = new FrameData_Rotation[csv.Height - 2];
//			for (int i = 0; i < csv.Height - 2; i++)
//			{
//				frames[i] = new FrameData_Rotation();
//				frames[i].participant = csv.GetCell("participant", i + 1);
//				frames[i].trialId = int.Parse(csv.GetCell("trial", i + 1));
//				frames[i].time = float.Parse(csv.GetCell("time", i + 1));

//				frames[i].px = float.Parse(csv.GetCell("px", i + 1));
//				frames[i].py = float.Parse(csv.GetCell("py", i + 1));
//				frames[i].pz = float.Parse(csv.GetCell("pz", i + 1));

//				frames[i].rx = float.Parse(csv.GetCell("rx", i + 1));
//				frames[i].ry = float.Parse(csv.GetCell("ry", i + 1));
//				frames[i].rz = float.Parse(csv.GetCell("rz", i + 1));
//				frames[i].rw = float.Parse(csv.GetCell("rw", i + 1));
//			}

//			Play(0);
//		}

//		void FixedUpdate()
//		{
//			if (playing)
//			{
//				UpdateTransform();
//				frameId += speed;

//				var frame = Mathf.Clamp(firstId + frameId, 0, frames.Length - 2);

//				if (frames[frame].trialId != frames[frame + 1].trialId)
//				{
//					Pause();
//				}
//			}
//		}

//		void OnGUI()
//		{
//			GUILayout.Label($"Trial: {trialId}");

//			var nowTime = frames[firstId + frameId].time - frames[firstId].time;
//			var endTime = frames[finalId].time - frames[firstId].time;
//			GUILayout.Label(nowTime.ToString("N2") + " / " + endTime.ToString("N2"));
//		}

//		public void Play(int trial)
//		{
//			UpdateTrial(trial);
//			UpdateTransform();
//		}

//		public void PlayPrevious()
//		{
//			Play(trialId - 1);
//		}

//		public void PlayNext()
//		{
//			Play(trialId + 1);
//		}

//		public void Play()
//		{
//			playing = true;
//		}

//		public void Pause()
//		{
//			playing = false;
//		}

//		public void PlayPause()
//		{
//			playing = !playing;
//		}

//		public void Skip(int amount)
//		{
//			frameId += amount;
//			UpdateTransform();
//		}

//		private void UpdateTrial(int trial)
//		{
//			// firstId
//			for (int i = 0; i < frames.Length; i++)
//			{
//				if (frames[i].trialId == trial)
//				{
//					firstId = i;
//					break;
//				}
//			}

//			// frameId
//			frameId = 0;

//			// finalId
//			for (int i = 0; i < frames.Length; i++)
//			{
//				if (frames[i].trialId > trial)
//				{
//					finalId = i - 1;
//					break;
//				}

//				if (i == frames.Length - 1)
//				{
//					finalId = frames.Length - 1;
//				}
//			}

//			playing = false;

//			FindObjectOfType<TableReconstruction>().SetTable(trial);
//			FindObjectOfType<ConditionReconstruction>().SetCondition(trial);
//		}

//		private void UpdateTransform()
//		{
//			var frame = frames[firstId + frameId];
//			target.position = new Vector3(frame.px, frame.py, frame.pz) - positionOffset;
//			target.rotation = new Quaternion(frame.rx, frame.ry, frame.rz, frame.rw);
//		}
//	}
//}
