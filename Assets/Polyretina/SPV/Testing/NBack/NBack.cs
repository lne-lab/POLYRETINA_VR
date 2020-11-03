using System;
using System.Linq;
using UnityEngine;

namespace LNE.Testing.NBack
{
	using ArrayExts;
	using IO;

    public class NBack : MonoBehaviour
    {
		/*
		 * Editor fields
		 */

#pragma warning disable 649
		[SerializeField]
		private int _n;

		[SerializeField]
		private float _chance;

		[SerializeField]
		private int _trials;

		[SerializeField]
		private float _displayTime;

		[SerializeField]
		private float _trialTime;

		[SerializeField]
		private bool _trainingBlock;

		[SerializeField]
		private GameObject[] _objects;
#pragma warning restore 649

		/*
		 * Private fields
		 */

		private int trialId;
		private float trialStartTime;
		private GameObject currObject;
		private GameObject[] prevObject;
		private bool hasAnswered;
		private CSV csv;

		private bool[] trainingAnswers;

		/*
		 * Private properties
		 */

		private GameObject nthObject => _n == 0 ? _objects[0] : prevObject.Last();

		private GameObject nextObject
		{
			get
			{
				var same = trialId > _n && _chance > UnityEngine.Random.value;
				return same ? nthObject : _objects.Remove((c) => c == nthObject).Random();
			}
		}

		private Color objectColour
		{
			get
			{
				return default;
			}

			set
			{
				currObject.GetComponent<SymbolColour>().colour = value;
			}
		}

		/*
		 * Public methods
		 */

		public void Begin()
		{
			trialId = 0;
			prevObject = new GameObject[_n];
			csv = new CSV();
			csv.AppendRow("Trial ID", "Time Taken", "Success");

			trainingAnswers = new bool[20]; // for training only

			Invoke(nameof(BeginTrial), 3);  // in 3... 2... 1...
		}

		public void BeginTrial()
		{
			trialId++;
			trialStartTime = Time.time;
			currObject = nextObject;
			hasAnswered = false;

			ShowObject();                               // now
			Invoke(nameof(HideObject), _displayTime);   // in _displayTime seconds
			Invoke(nameof(EndTrial), _trialTime);       // in _trialTime seconds
		}

		public void ShowObject()
		{
			objectColour = Color.white;
			currObject.SetActive(true);
		}

		public void HideObject()
		{
			currObject.SetActive(false);

			// set answer to incorrect if no answer
			OnAnswer(currObject != nthObject);
		}

		public void OnAnswer(bool answer)
		{
			if (hasAnswered == false)
			{
				var success = currObject == nthObject == answer;
				csv.AppendRow(trialId, Time.time - trialStartTime, success);

				objectColour = success ? Color.green : Color.red;

				if (_trainingBlock)
				{
					trainingAnswers.Shift(1);
					trainingAnswers[0] = success;
				}

				hasAnswered = true;

				var result = success ? "Correctly" : "Incorrectly";
				Debug.Log($"Answer saved for trial {trialId}. You took {Time.time - trialStartTime} and answered {result}.");
			}
		}

		public void EndTrial()
		{
			if (_n > 0)
			{
				prevObject.Shift(1);
				prevObject[0] = currObject;
			}

			var lt20 = trainingAnswers.Converge(true, (a, b) => a && b) == false;
			if (_trainingBlock ? lt20 : trialId < _trials)
			{
				BeginTrial();
			}
			else
			{
				End();
			}
		}

		public void End()
		{
			var path = Application.dataPath + $"/Polyretina/SPV/Testing/NBack/Results/{DateTime.Now.Ticks}.csv";
			csv.Save(path);

			Debug.Log($"Saving to: {path}");
		}
	}
}
