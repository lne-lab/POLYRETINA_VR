using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LNE.StringExts;
using LNE.ProstheticVision;
using LNE.Studies.FadingV2;

using static LNE.ArrayExts.ArrayExtensions;

namespace LNE.Studies.FadingV2
{
	public class TrialReconstruction : MonoBehaviour
	{
		[SerializeField]
		private string _participant = "pilot";

		[SerializeField]
		private TextAsset _words = default;

		[SerializeField]
		private int _wordLength = 6;

		[SerializeField]
		private int _rows = 3;

		[SerializeField]
		private int _columns = 1;

		private int _seed;
		private int _trialId;
		private List<TrialData_Conditions> _trials;
		private TrialData_Conditions _trial;
		private Strategy[] _strategies;

		private SaccadeSimulator simulator => Prosthesis.Instance.ExternalProcessor as SaccadeSimulator;
		private EpiretinalImplant implant => Prosthesis.Instance.Implant as EpiretinalImplant;

		private string[] randomWords
		{
			get
			{
				var allWords = _words
								.text
								.Split('\n')
								.Apply((word) => word.Trim('\r'))
								.Where((word) => word.Length == _wordLength)
								.Randomise(_seed - _trialId);

				return allWords.Subarray(0, 8);
			}
		}

		void Awake()
		{
			_trials = new List<TrialData_Conditions>();
			_seed = _participant.AsUid();

			var strategyCombos = new[]
			{
				Strategy.None,
				Strategy.Saccade,
				Strategy.Random,
				Strategy.Interrupt,
				Strategy.Saccade | Strategy.Random,
				Strategy.Saccade | Strategy.Interrupt,
				Strategy.Random | Strategy.Interrupt,
				Strategy.Saccade | Strategy.Random | Strategy.Interrupt,
				Strategy.None | Strategy.EyeTracking,
				Strategy.None | Strategy.NoFading
			};

			_strategies = CreateArray(strategyCombos, 5, (s) => s).Randomise(_seed);
		}

		void OnGUI()
		{
			var style = new GUIStyle(GUI.skin.label);
			style.fontSize = 100;

			GUILayout.Label("", style);
			GUILayout.Label("", style);
			GUILayout.Label("", style);
			GUILayout.Label(_trial != null ? _trial.strategy.ToString() : "", style);
		}

		public void SetStrategyParameters()
		{
			// update saccade/interrupt simulator
			simulator.saccadeType = _trial.strategy.HasFlag(Strategy.Saccade)
									? SaccadeSimulator.SaccadeType.Left
									: SaccadeSimulator.SaccadeType.None;

			simulator.offTime = _trial.strategy.HasFlag(Strategy.Interrupt)
								? 0.6f
								: 0.0f;

			// update implant
			switch (_trial.strategy)
			{
				case Strategy.None:
					implant.decayT1 = .1f;
					implant.decayT2 = .3f;
					break;
				case Strategy.Saccade:
					implant.decayT1 = .175f;
					implant.decayT2 = .525f;
					break;
				case Strategy.Random:
					implant.decayT1 = .2f;
					implant.decayT2 = .6f;
					break;
				case Strategy.Interrupt:
					implant.decayT1 = .1f;
					implant.decayT2 = .3f;
					break;
				case Strategy.Saccade | Strategy.Random:
					implant.decayT1 = .175f;
					implant.decayT2 = .525f;
					break;
				case Strategy.Saccade | Strategy.Interrupt:
					implant.decayT1 = .275f;
					implant.decayT2 = .825f;
					break;
				case Strategy.Random | Strategy.Interrupt:
					implant.decayT1 = .8f;
					implant.decayT2 = 2.4f;
					break;
				case Strategy.Saccade | Strategy.Random | Strategy.Interrupt:
					implant.decayT1 = .55f;
					implant.decayT2 = .165f;
					break;
			}

			// eye tracking
			implant.eyeGazeSource = _trial.strategy.HasFlag(Strategy.EyeTracking) ?
									EyeGaze.Source.EyeTracking :
									EyeGaze.Source.None;

			// no fading
			implant.useFading = !_trial.strategy.HasFlag(Strategy.NoFading);

#if !UNITY_EDITOR
			simulator.Update();
			implant.Update();
#endif
		}

		public void ShowWords()
		{
			var words = randomWords;
			var text = "";

			for (int i = 0; i < _rows; i++)
			{
				for (int j = 0; j < _columns; j++)
				{
					text += words[(i * _columns) + j] + " ";
				}

				text += "\n";
			}

			FindObjectOfType<Text>().text = text;
		}

		public void PreviousTrial()
		{
			_trialId = Mathf.Clamp(_trialId - 1, 0, 49);
			_trial = new TrialData_Conditions
			{
				participant = _participant,
				trialId = _trialId,
				strategy = _strategies[_trialId],
				startTime = Time.time
			};
		}

		public void NextTrial()
		{
			_trialId = Mathf.Clamp(_trialId + 1, 0, 49);
			_trial = new TrialData_Conditions
			{
				participant = _participant,
				trialId = _trialId,
				strategy = _strategies[_trialId],
				startTime = Time.time
			};
		}
	}
}
