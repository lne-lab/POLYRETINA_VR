#pragma warning disable 649

using System.IO;
using UnityEngine;

namespace LNE.Studies
{
	using IO;
	using ProstheticVision;
	using Threading;
	using TransformExts;
	using UI.Attributes;
	using static ArrayExts.ArrayExtensions;

	public class Study_DP2 : Singleton<Study_DP2>
	{
		public enum State { FadingIn, FadedIn, FadingOut, FadedOut }

		/*
		 * Editor fields
		 */

		[Header("Participant")]

		[SerializeField]
		private int _id;

		[SerializeField]
		private int _startingExposure;

		[SerializeField, Path]
		private string _savePath;

		[Header("Factors")]

		[SerializeField]
		private ElectrodeLayout[] _layouts;

		[SerializeField, Range(0, 46.3f)]
		private float[] _fieldOfViews;

		[SerializeField, Range(0, 3)]
		private float[] _decayConstants;

		[SerializeField]
		private Vector2[] _heights;

		[Space]

		[SerializeField]
		private int _exposuresPerCondition = 1;

		[Header("Scene Objects")]

		[SerializeField]
		private Transform _leftPlatform;

		[SerializeField]
		private Transform _rightPlatform;

		[Header("Other")]

		[SerializeField, Range(.01f, 2)]
		private float _fadeTime = 1;

		/*
		 * Private fields
		 */

		private Exposure_DP2[] exposures;
		private int exposureId;
		private State state;
		private float startTime;
		
		private string answer = "";

		/*
		 * Private properties
		 */

		private EpiretinalImplant implant => Prosthesis.Instance.Implant as EpiretinalImplant;

		private bool leftIsHigher => _leftPlatform.transform.position.y > _rightPlatform.transform.position.y;
		private bool rightIsHigher => !leftIsHigher;
		private float heightDifference => Mathf.Abs(_leftPlatform.transform.position.y - _rightPlatform.transform.position.y);

		/*
		 * Unity callbacks
		 */

		void Start()
		{
			exposures = CreateArray(
				_layouts,
				_fieldOfViews,
				_decayConstants,
				_heights,
				_exposuresPerCondition,
				(ed, fov, dc, h) => new Exposure_DP2(ed, fov, dc, h)
			);

			exposures.Randomise(_id);
			exposureId = _startingExposure;
			state = State.FadedOut;

			implant.brightness = 0;
		}

		void OnGUI()
		{
			GUI.skin.label.fontSize = 48;

			GUILayout.Label("Exposure: " + exposureId.ToString());
			GUILayout.Label("Electrode Layout: " + implant.layout.ToString());
			GUILayout.Label("Field of View: " + implant.fieldOfView.ToString());
			GUILayout.Label("Decay Constant: " + implant.tailLength.ToString());
			GUILayout.Label("Left Platform Height: " + _leftPlatform.localPosition.y.ToString());
			GUILayout.Label("Right Platform Height: " + _rightPlatform.localPosition.y.ToString());
			GUILayout.Label("Answer: " + answer);
		}

		void OnApplicationQuit()
		{
			Save();
		}

		/*
		 * Public methods 
		 */

		public void Begin()
		{
			ShowExposure();
		}

		public void AcceptLeft()
		{
			if (state == State.FadedIn)
			{
				exposures[exposureId]._timeTaken += Time.time - startTime;
				exposures[exposureId]._heightDifference = heightDifference;
				exposures[exposureId]._higherPlatform = leftIsHigher ? "left" : "right";

				exposures[exposureId]._answer = "left";
				exposures[exposureId]._correct = leftIsHigher;

				// debug output
				answer = leftIsHigher ? "Correct" : "Incorrect";

				FadeOut();

				CallbackManager.InvokeOnce(1, ShowNextExposure);
			}
		}

		public void AcceptRight()
		{
			if (state == State.FadedIn)
			{
				exposures[exposureId]._timeTaken += Time.time - startTime;
				exposures[exposureId]._heightDifference = heightDifference;
				exposures[exposureId]._higherPlatform = leftIsHigher ? "left" : "right";

				exposures[exposureId]._answer = "right";
				exposures[exposureId]._correct = rightIsHigher;

				// debug output
				answer = rightIsHigher ? "Correct" : "Incorrect";

				FadeOut();

				CallbackManager.InvokeOnce(1, ShowNextExposure);
			}
		}

		public void AcceptDontKnow()
		{
			if (state == State.FadedIn)
			{
				exposures[exposureId]._timeTaken += Time.time - startTime;
				exposures[exposureId]._heightDifference = Mathf.Abs(_leftPlatform.transform.position.y - _rightPlatform.transform.position.y);
				exposures[exposureId]._higherPlatform = _leftPlatform.transform.position.y > _rightPlatform.transform.position.y ? "left" : "right";

				exposures[exposureId]._answer = "dontknow";
				exposures[exposureId]._correct = false;

				// debug output
				answer = "Don't know";

				FadeOut();

				CallbackManager.InvokeOnce(1, ShowNextExposure);
			}
		}

		public void Cancel()
		{
			FadeIn();
		}

		/*
		 * Private methods
		 */

		private void ShowExposure()
		{
			implant.layout = exposures[exposureId].layout;
			implant.fieldOfView = exposures[exposureId].fieldOfView;
			implant.tailLength = exposures[exposureId].decayConstant;

			_leftPlatform.SetY(exposures[exposureId].height[0]);
			_rightPlatform.SetY(exposures[exposureId].height[1]);

			FadeIn();
		}

		private void ShowNextExposure()
		{
			exposureId++;
			if (exposureId < exposures.Length)
			{
				ShowExposure();
			}
			else
			{
				UnityApp.Quit();
			}
		}

		private void FadeIn()
		{
			if (state != State.FadedOut)
				return;

			state = State.FadingIn;

			//Callback.Lerp(0, 1, _fadeTime, 
			//(val) => {
			//	Device.instance.brightness = val;
			//}, 
			//() => {
			//	state = State.FadedIn;
			//	startTime = Time.time;
			//});

			CallbackManager.InvokeUntil(() =>
			{
				implant.brightness = Mathf.Min(implant.brightness + Time.deltaTime / _fadeTime, 1);
				return implant.brightness < 1;
			}, () =>
			{
				state = State.FadedIn;
				startTime = Time.time;
			});
		}

		private void FadeOut()
		{
			if (state != State.FadedIn)
				return;

			state = State.FadingOut;

			//Callback.Lerp(1, 0, _fadeTime,
			//(val) =>
			//{
			//	Device.instance.brightness = val;
			//},
			//() =>
			//{
			//	state = State.FadedOut;
			//});

			CallbackManager.InvokeUntil(() =>
			{
				implant.brightness = Mathf.Max(implant.brightness - Time.deltaTime / _fadeTime, 0);
				return implant.brightness > 0;
			}, () =>
			{
				state = State.FadedOut;
			});
		}

		private void Save()
		{
			var csvObj = new CSV();
			csvObj.AppendRow(
				"Participant", 
				"Electrode Layout", 
				"Field of View", 
				"Decay Constant", 
				"Left Height", 
				"Right Height", 
				"Time Taken", 
				"Height Difference", 
				"Answer", 
				"Higher Platform", 
				"Correct"
			);

			foreach (var exposure in exposures)
			{
				csvObj.AppendRow(
					_id,
					exposure.layout,
					exposure.fieldOfView,
					exposure.decayConstant,
					exposure.height[0],
					exposure.height[1],
					exposure._timeTaken,
					exposure._heightDifference,
					exposure._answer,
					exposure._higherPlatform,
					exposure._correct
				);
			}

			csvObj.Save(_savePath + _id.ToString() + "_" + System.DateTime.Now.ToString("dd-MM-yyy_hh-mm-ss") + ".csv");
		}
	}

	public class Exposure_DP2
	{
		public ElectrodeLayout layout;
		public float fieldOfView;
		public float decayConstant;
		public Vector2 height;

		public float _timeTaken;
		public float _heightDifference;
		public string _answer;
		public string _higherPlatform;
		public bool _correct;

		public Exposure_DP2(ElectrodeLayout l, float fov, float dc, Vector2 h)
		{
			layout = l;
			fieldOfView = fov;
			decayConstant = dc;
			height = h;

			_timeTaken = 0;

			_heightDifference = 0;
			_answer = "";
			_higherPlatform = "";
			_correct = false;
		}
	}
}
