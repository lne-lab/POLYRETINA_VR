#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LNE.Studies.FadingV2
{
	using IO;

	public class FadingHelper : MonoBehaviour
	{
		[MenuItem("Polyretina/.../Calculate Rotation")]
		static void CalculateRotation()
		{
			var path = EditorUtility.OpenFilePanel("Select json file", "", "csv");
			var csv = new CSV();
			csv.LoadWStream(path);

			var xs = csv.GetColumn("rx", false);
			var ys = csv.GetColumn("ry", false);
			var zs = csv.GetColumn("rz", false);
			var ws = csv.GetColumn("rw", false);

			var ds = new List<object>();
			ds.Add("diff");

			for (int i = 0; i + 1 < xs.Length && float.TryParse(xs[i + 1], out _); i++)
			{
				var a = new Quaternion(
					float.Parse(xs[i]),
					float.Parse(ys[i]),
					float.Parse(zs[i]),
					float.Parse(ws[i])
				);

				var b = new Quaternion(
					float.Parse(xs[i + 1]),
					float.Parse(ys[i + 1]),
					float.Parse(zs[i + 1]),
					float.Parse(ws[i + 1])
				);

				var d = Quaternion.Angle(a, b);
				ds.Add(d);
			}

			Debug.Log(csv.Width);

			csv.AppendColumn(ds.ToArray());
			csv.SaveWStream(path.Replace(".csv", "-diff.csv"));
		}
	}
}
#endif
