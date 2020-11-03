//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

//namespace LNE.Studies.FadingV1
//{
//	using UI.Attributes;

//	public class TableReconstruction : MonoBehaviour
//	{
//#pragma warning disable 649
//		[SerializeField, Path(isFile = true)]
//		private string path;

//		[SerializeField]
//		private Transform plate, bowl, mug, bottle, saucepan, book, stapler, headphones, hat, kettle;
//#pragma warning restore 649

//		private TrialData_Items[] trials;

//		private Transform _chosen;
//		private Transform chosen
//		{
//			get
//			{
//				return _chosen;
//			}

//			set
//			{
//				if (_chosen != null)
//					_chosen.GetComponent<Renderer>().material.color = Color.white;

//				_chosen = value;

//				if (_chosen != null)
//					_chosen.GetComponent<Renderer>().material.color = Color.red;
//			}
//		}

//		void Start()
//		{
//			var json = File.ReadAllText(path);
//			trials = JsonUtility.FromJson<TestData_Items>(json).trials;
//		}

//		public void SetTable(int trialId)
//		{
//			var itemDic = new Dictionary<string, Transform> {
//			{ nameof(plate), plate },
//			{ nameof(bowl), bowl },
//			{ nameof(mug), mug },
//			{ nameof(bottle), bottle },
//			{ nameof(saucepan), saucepan },
//			{ nameof(book), book },
//			{ nameof(stapler), stapler },
//			{ nameof(headphones), headphones},
//			{ nameof(hat), hat },
//			{ nameof(kettle), kettle }
//		};

//			var trial = trials[Mathf.Clamp(trialId, 0, trials.Length - 1)];

//			var i = 0;
//			foreach (var item in trial.itemNames)
//			{
//				SetItem(itemDic[item], i++);
//			}

//			chosen = itemDic[trial.chosenItem];
//		}

//		private void SetItem(Transform item, int positionIndex)
//		{
//			var positions = new[] {
//			new Vector3(-.90f, -.45f, 1.8f),
//			new Vector3(-.45f, -.45f, 1.8f),
//			new Vector3( .00f, -.45f, 1.8f),
//			new Vector3( .45f, -.45f, 1.8f),
//			new Vector3( .90f, -.45f, 1.8f),
//			new Vector3(-.90f, -.45f, 1.2f),
//			new Vector3(-.45f, -.45f, 1.2f),
//			new Vector3( .00f, -.45f, 1.2f),
//			new Vector3( .45f, -.45f, 1.2f),
//			new Vector3( .90f, -.45f, 1.2f)
//		};

//			item.transform.position = positions[positionIndex];
//		}
//	}
//}
