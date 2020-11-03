//#if UNITY_EDITOR

//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using UnityEditor;

//namespace LNE.Studies.FadingV1
//{
//	using IO;

//	public class JsonToCsv_Fading : MonoBehaviour
//	{
//		[MenuItem("Polyretina/Fading Study/Json To CSV 1")]
//		static void Run1()
//		{
//			var path = EditorUtility.OpenFilePanel("Select json file", "", "json");
//			var json = File.ReadAllText(path);
//			var data = JsonUtility.FromJson<TestData_Rotation>(json);

//			var tuples = new List<(int, float)>();

//			var startTime = 0f;
//			for (int i = 0; i < data.frames.Length; i++)
//			{
//				var lastFrame = i > 0 ? data.frames[i - 1] : null;
//				var thisFrame = data.frames[i];
//				var nextFrame = i < (data.frames.Length - 1) ? data.frames[i + 1] : null;

//				var lastTrial = lastFrame != null ? lastFrame.trialId : -1;
//				var nextTrial = nextFrame != null ? nextFrame.trialId : -1;

//				if (thisFrame.trialId != lastTrial)
//				{
//					startTime = thisFrame.time;
//				}

//				if (thisFrame.trialId != nextTrial)
//				{
//					var timeElapsed = thisFrame.time - startTime;
//					tuples.Add((thisFrame.trialId, timeElapsed));
//				}
//			}

//			var csv = new CSV();
//			csv.AppendRow("trial", "elapsed");

//			foreach (var tuple in tuples)
//			{
//				csv.AppendRow(tuple.Item1, tuple.Item2);
//			}

//			csv.Save(path.Replace(".json", ".csv"));
//		}

//		[MenuItem("Polyretina/Fading Study/Json To CSV 2")]
//		static void Run2()
//		{
//			var path = EditorUtility.OpenFilePanel("Select json file", "", "json");
//			var json = File.ReadAllText(path);
//			var data = JsonUtility.FromJson<TestData_Rotation>(json);

//			var csv = new CSV();
//			csv.AppendRow("participant", "trial", "time", "px", "py", "pz", "rx", "ry", "rz", "rw");

//			foreach (var frame in data.frames)
//			{
//				csv.AppendRow(
//					frame.participant,
//					frame.trialId,
//					frame.time,
//					frame.px,
//					frame.py,
//					frame.pz,
//					frame.rx,
//					frame.ry,
//					frame.rz,
//					frame.rw
//				);
//			}

//			csv.SaveWStream(path.Replace(".json", ".csv"));
//		}

//		[MenuItem("Polyretina/Fading Study/Calc Diff")]
//		static void Run3()
//		{
//			var path = EditorUtility.OpenFilePanel("Select json file", "", "csv");
//			var csv = new CSV();
//			csv.LoadWStream(path);

//			var xs = csv.GetColumn("rx", false);
//			var ys = csv.GetColumn("ry", false);
//			var zs = csv.GetColumn("rz", false);
//			var ws = csv.GetColumn("rw", false);

//			var ds = new List<float>();
//			ds.Add(0);  // title

//			for (int i = 0; i + 1 < xs.Length && float.TryParse(xs[i + 1], out _); i++)
//			{
//				var a = new Quaternion(
//					float.Parse(xs[i]),
//					float.Parse(ys[i]),
//					float.Parse(zs[i]),
//					float.Parse(ws[i])
//				);

//				var b = new Quaternion(
//					float.Parse(xs[i + 1]),
//					float.Parse(ys[i + 1]),
//					float.Parse(zs[i + 1]),
//					float.Parse(ws[i + 1])
//				);

//				var d = Quaternion.Angle(a, b);
//				ds.Add(d);
//			}

//			//csv.AppendColumn(ds.ToArray().Cast<object>().ToArray());
//			//csv.SaveWStream(path.Replace(".csv", "-diff.csv"));
//		}
//	}
//}
//#endif
