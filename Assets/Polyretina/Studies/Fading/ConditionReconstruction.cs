//using System.IO;
//using UnityEngine;

//namespace LNE.Studies.FadingV1
//{
//	using UI.Attributes;

//	public class ConditionReconstruction : MonoBehaviour
//	{
//#pragma warning disable 649
//		[SerializeField, Path(isFile = true)]
//		private string path;
//#pragma warning restore 649

//		private TrialData_Conditions[] trials;
//		private TrialData_Conditions trial;

//		void Start()
//		{
//			var json = File.ReadAllText(path);
//			trials = JsonUtility.FromJson<TestData_Conditions>(json).trials;
//		}

//		void OnGUI()
//		{
//			GUILayout.Space(25);
//			//GUILayout.Label("        / " + (trial.endtime - trial.startTime).ToString("N2"));

//			GUILayout.Space(25);
//			GUILayout.Label(trial.strategy.ToString());
//		}

//		public void SetCondition(int trialId)
//		{
//			trial = trials[trialId];
//		}
//	}
//}
