#pragma warning disable 649

using System;
using System.IO;
using UnityEngine;

namespace LNE.Studies
{
	using UI.Attributes;
	using ArrayExts;
	using ProstheticVision;
	using Threading;
	using System.Linq;

	/// <summary>
	/// A study involving the Polyretina Device
	/// </summary>
	public abstract class Study : Singleton<Study>
	{
		private enum Step { Start, Showing, Hiding, Answered };

		/*
		 * Inspector fields
		 */
		 
		[Header("Participant")]

		[SerializeField]
		protected int _identifier;

		[Space]
		[SerializeField]
		protected int _session;

		[SerializeField]
		protected int _of;

		[SerializeField]
		protected int _startingItem;

		[Space]
		[SerializeField, Path]
		private string _saveLocation;

		[Header("Conditions")]

		[SerializeField]
		private ElectrodeLayout[] _electrodeLayouts;

		[SerializeField, Range(0, 46.3f)]
		private float[] _fieldOfViews;

		[SerializeField, Range(1, 95)]
		private float[] _visualAngles;

		[SerializeField, Range(0, 3)]
		private float[] _tailLengths;

		/*
		 * Protected / Private fields
		 */

		protected object instantiatedItem;
		protected int exposureIndex;

		private object[] items;
		private int[] conditionIndices;

		private StudyData data;
		private DateTime startTime;
		private DateTime endTime;

		private Step currentStep;

		/*
		 * Public / Protected / Private properties
		 */

		public object currentItem
		{
			get
			{
				return items[exposureIndex];
			}
		}

		public ElectrodeLayout currentElectrodeLayout
		{
			get
			{
				var conditionIndex = conditionIndices[exposureIndex];
				var itemsPerResolution = items.Length / _electrodeLayouts.Length;
				var rollingIndex = conditionIndex / itemsPerResolution;
				var resolutionIndex = rollingIndex % _electrodeLayouts.Length;

				return _electrodeLayouts[resolutionIndex];
			}
		}

		public float currentVisualAngle
		{
			get
			{
				var conditionIndex = conditionIndices[exposureIndex];
				var itemsPerVisualAngle = items.Length / _electrodeLayouts.Length / _visualAngles.Length;
				var rollingIndex = conditionIndex / itemsPerVisualAngle;
				var visualAngleIndex = rollingIndex % _visualAngles.Length;

				return _visualAngles[visualAngleIndex];
			}
		}

		public float currentTailLength
		{
			get
			{
				var conditionIndex = conditionIndices[exposureIndex];
				var itemsPerTailLength = items.Length / _electrodeLayouts.Length / _visualAngles.Length / _tailLengths.Length;
				var rollingIndex = conditionIndex / itemsPerTailLength;
				var tailLengthIndex = rollingIndex % _tailLengths.Length;

				return _tailLengths[tailLengthIndex];
			}
		}

		public float currentFieldOfView
		{
			get
			{
				var conditionIndex = conditionIndices[exposureIndex];
				var itemsPerFieldOfView = items.Length / _electrodeLayouts.Length / _visualAngles.Length / _tailLengths.Length / _fieldOfViews.Length;
				var rollingIndex = conditionIndex / itemsPerFieldOfView;
				var fieldOfViewIndex = rollingIndex % _fieldOfViews.Length;

				return _fieldOfViews[fieldOfViewIndex];
			}
		}

		protected int numConditions
		{
			get
			{
				return _electrodeLayouts.Length * _visualAngles.Length * _tailLengths.Length * _fieldOfViews.Length;
			}
		}

		private EpiretinalImplant implant => Prosthesis.Instance.Implant as EpiretinalImplant;

		private string path
		{
			get
			{
				return _saveLocation + "/" +                                                                                            // directory
						_identifier.ToString() + "_" + _session.ToString() + "_" + data.date + "_" + data.startTime.Replace(':', '-') + // filename
						".json";                                                                                                        // extension
			}
		}

		/*
		 * Virtual / Abstract methods
		 */

		protected virtual object[] InitialiseItems() { return items; }

		protected abstract object InstantiateItem(object item, float visualAngle);
		protected abstract void ShowItem();
		protected abstract void HideItem();
		protected abstract void DestroyItem();

		public abstract string GetItemName();
		public abstract string GetItemName(object item);
		public abstract void SetVisualAngle(float visualAngle);

		/*
		 * Unity callbacks
		 */

		protected virtual void Start()
		{
			exposureIndex = _startingItem - 1;

			items = InitialiseItems();
			items.Randomise(_identifier);

			conditionIndices = ArrayExtensions.CreateArray(items.Length, (i) => i);
			conditionIndices.Randomise(_identifier + 1);

			AssertConditionParameters();
			StartSession();

			data = new StudyData {
				identifier = _identifier,
				date = DateTime.UtcNow.ToString("dd.MM.yyyy"),
				startTime = DateTime.UtcNow.ToString("HH:mm:ss.ff"),
				exposures = new StudyData.Exposure[items.Length]
			};
		}

		protected virtual void OnGUI()
		{
			var font = new GUIStyle { fontSize = 96, normal = new GUIStyleState { textColor = Color.white } };

			GUILayout.Label(currentStep.ToString(), font);

			if (exposureIndex >= 0 && exposureIndex < items.Length)
			{
				font.fontSize = 32;
				GUILayout.Label("Exposure: " + (exposureIndex + 1).ToString(), font);
				GUILayout.Label("Item: " + GetItemName(), font);
				GUILayout.Label("Electrode Layout: " + currentElectrodeLayout.ToString(), font);
				GUILayout.Label("Field of View: " + currentFieldOfView.ToString(), font);
				GUILayout.Label("Visual Angle: " + currentVisualAngle.ToString(), font);
				GUILayout.Label("Tail Length: " + currentTailLength.ToString(), font);
			}
		}

		protected virtual void OnApplicationQuit()
		{
			var realResults = data.exposures.Converge(false, (StudyData.Exposure exposure, bool result) =>
			{
				return (exposure != null && exposure.result) || result;
			});

			if (realResults)
			{
				var json = JsonUtility.ToJson(data, true);
				File.WriteAllText(path, json);
			}
		}

		/*
		 * public methods
		 */

		public void ShowItem(string name)
		{
			var item = items.First((i) => GetItemName(i) == name);

			if (item != null)
			{
				DestroyItem();
				instantiatedItem = InstantiateItem(item, currentVisualAngle);
			}
		}

		public void ShowFirst()
		{
			if (currentStep == Step.Start)
			{
				ShowNext();
			}
		}

		public void ShowNext(float delay)
		{
			CallbackManager.InvokeOnce(delay, ShowNext);
		}

		public void ShowNext()
		{
			if (UpdateStep(Step.Showing) == false)
				return;

			DestroyItem();
			exposureIndex++;

			if (exposureIndex < items.Length)
			{
				instantiatedItem = InstantiateItem(currentItem, currentVisualAngle);

				SetElectrodeLayout(currentElectrodeLayout);
				SetTailLength(currentTailLength);
				SetFieldOfView(currentFieldOfView);

				startTime = DateTime.Now;
			}
			else
			{
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
			}
		}

		public void ShowPrevious()
		{
			if (currentStep == Step.Hiding)
			{
				currentStep = Step.Showing;
				ShowItem();
			}
		}

		public void HidePrevious()
		{
			if (UpdateStep(Step.Hiding) == false)
				return;

			endTime = DateTime.Now;

			HideItem();
		}

		public void SaveAnswer(KeyCode answer)
		{
			if (UpdateStep(Step.Answered) == false)
				return;

			data.exposures[exposureIndex] = new StudyData.Exposure {
				electrodeLayout = currentElectrodeLayout.ToString(),
				visualAngle = currentVisualAngle,
				fieldOfView = currentFieldOfView,
				tailLength = currentTailLength,
				itemName = GetItemName(),
				result = answer == KeyCode.Y,
				elapsedTime = (float)(endTime - startTime).TotalSeconds,
				endTime = endTime.ToUniversalTime().ToString("HH:mm:ss.ff"),
				startTime = startTime.ToUniversalTime().ToString("HH:mm:ss.ff")
			};
		}

		public void SetElectrodeLayout(ElectrodeLayout layout)
		{
			implant.layout = layout;
		}
		
		public void SetTailLength(float tailLength)
		{
			implant.tailLength = tailLength;
		}

		public void SetFieldOfView(float fieldOfView)
		{
			implant.fieldOfView = fieldOfView;
		}

		/*
		 * Private methods
		 */

		private void StartSession()
		{
			var split = (int)((float)items.Length / _of);

			var start = split * (_session - 1);
			var end = _session < _of ? split * _session : items.Length;

			items = items.Subarray(start, end - start);
		}

		private void AssertConditionParameters()
		{
			if (IsDivisibleBy(items.Length, _electrodeLayouts.Length) == false)
			{
				Debug.LogError("The number of items is not divisible by the number of layouts.");
				return;
			}

			if (IsDivisibleBy(items.Length / _electrodeLayouts.Length, _visualAngles.Length) == false)
			{
				Debug.LogError("The number of items per layout is not divisible by the number of visual angles.");
				return;
			}

			if (IsDivisibleBy(items.Length / _electrodeLayouts.Length / _visualAngles.Length, _tailLengths.Length) == false)
			{
				Debug.LogError("The number of items per visual angle is not divisible by the number of tail lengths.");
				return;
			}

			if (IsDivisibleBy(items.Length / _electrodeLayouts.Length / _visualAngles.Length / _tailLengths.Length, _fieldOfViews.Length) == false)
			{
				Debug.LogError("The number of items per tail length is not divisible by the number of field of views.");
				return;
			}
		}

		private bool UpdateStep(Step nextStep)
		{
			var success =	(nextStep == Step.Showing	&& (currentStep == Step.Answered || currentStep == Step.Start)) ||
							(nextStep == Step.Hiding	&& currentStep == Step.Showing)	||
							(nextStep == Step.Answered	&& currentStep == Step.Hiding);

			if (success)
			{
				currentStep = nextStep;
			}

			return success;
		}

		private bool IsDivisibleBy(int a, int b)
		{
			if (a < 0 || b < 0)
			{
				throw new NotImplementedException();
			}

			return (a < b || b == 0) ? false : a % b == 0;
		}
	}
}
