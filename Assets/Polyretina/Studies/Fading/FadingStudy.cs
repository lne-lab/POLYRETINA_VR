using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LNE.IO;
using LNE.ProstheticVision;
using LNE.StringExts;
using LNE.UI.Attributes;

using static LNE.ArrayExts.ArrayExtensions;

[Flags]
public enum Strategy { None = 1, Saccade = 2, Random = 4, Interrupt = 8 }

public class FadingStudy : MonoBehaviour
{
	[Path]
	public string path;
	public string participant;

	public int startingTrial;
	public int trialsPerCondition;

	[Space]

	public Image webcam;

	[Space]

	public AudioSource speakers;
	public AudioClip startSound;
	public AudioClip successSound;
	public AudioClip failedSound;

	private List<TrialData_Conditions> _trials;
	private TrialData_Conditions _trial;
	private int _seed;
	private int _trialId;
	private bool _trialStarted;
	private Strategy[] _strategies;

	public int trialId => _trialStarted ? _trialId : -1;

	void Awake()
	{
		_trials = new List<TrialData_Conditions>();
		_seed = participant.AsUid();
		_trialId = startingTrial;
		_trialStarted = false;

		var strategyCombos = new[] 
		{
			Strategy.None,
			Strategy.Saccade,
			Strategy.Random,
			Strategy.Interrupt,
			Strategy.Saccade | Strategy.Random,
			Strategy.Saccade | Strategy.Interrupt,
			Strategy.Random | Strategy.Interrupt,
			Strategy.Random | Strategy.Random | Strategy.Interrupt
		};

		_strategies = CreateArray(strategyCombos, trialsPerCondition, (s) => s).Randomise(_seed);
	}

	void OnApplicationQuit()
	{
		var fileId = DateTime.Now.Ticks;
		SaveAsJson(fileId);
		SaveAsCsv(fileId);
	}

	private void SaveAsJson(long id)
	{
		var data = new TestData_Conditions { trials = _trials.ToArray() };
		var json = JsonUtility.ToJson(data, true);

		File.WriteAllText(
			path + $"Fading_Conditions_{participant}_json_{id}.json", json
		);
	}

	private void SaveAsCsv(long id)
	{
		var csv = new CSV();
		csv.AppendRow("participant", "trial", "strategy", "start time", "end time", "success");

		foreach (var trial in _trials)
		{
			csv.AppendRow(
				trial.participant,
				trial.trialId,
				trial.strategy,
				trial.startTime,
				trial.endtime,
				trial.success
			);
		}

		csv.Save(path + $"Fading_Conditions_{participant}_csv_{id}.csv");
	}

	public void StartTrial()
	{
		// safety check
		if (_trialStarted)
			return;

		_trialStarted = true;

		// create trial
		_trial = new TrialData_Conditions
		{
			participant = participant,
			trialId = _trialId,
			strategy = _strategies[_trialId],
			startTime = Time.time
		};

		// misc stuff
		SetStrategyParameters();
		webcam.enabled = false;
		speakers.PlayOneShot(startSound);
	}

	public void EndTrial(bool success)
	{
		// safety check
		if (_trialStarted == false)
			return;

		_trialStarted = false;

		// add trial
		_trial.endtime = Time.time;
		_trial.success = success;
		_trials.Add(_trial);

		// misc stuff
		speakers.PlayOneShot(success ? successSound : failedSound);
		webcam.enabled = true;

		// incr
		_trialId++;
	}

	private void SetStrategyParameters()
	{
		var simulator = Prosthesis.Instance.ExternalProcessor as SaccadeSimulator;

		simulator.saccadeType	= _trial.strategy.HasFlag(Strategy.Saccade) 
								? SaccadeSimulator.SaccadeType.Left 
								: SaccadeSimulator.SaccadeType.None;

		simulator.offTime	= _trial.strategy.HasFlag(Strategy.Interrupt)
							? 0.6f
							: 0.0f;

		var fadeParams = FindObjectOfType<FadeParameters>();
		switch (_trial.strategy)
		{
			case Strategy.None:
				fadeParams.parameters._5HzT1 = 0.1f;
				fadeParams.parameters._5HzT2 = 0.3f;
				break;
			case Strategy.Saccade:
				fadeParams.parameters._5HzT1 = 0.175f;
				fadeParams.parameters._5HzT2 = 0.525f;
				break;
			case Strategy.Random:
				fadeParams.parameters._5HzT1 = 0.2f;
				fadeParams.parameters._5HzT2 = 0.6f;
				break;
			case Strategy.Interrupt:
				fadeParams.parameters._5HzT1 = 0.1f;
				fadeParams.parameters._5HzT2 = 0.3f;
				break;
			case Strategy.Saccade | Strategy.Random:
				fadeParams.parameters._5HzT1 = 0.175f;
				fadeParams.parameters._5HzT2 = 0.525f;
				break;
			case Strategy.Saccade | Strategy.Interrupt:
				fadeParams.parameters._5HzT1 = 0.275f;
				fadeParams.parameters._5HzT2 = 0.825f;
				break;
			case Strategy.Random | Strategy.Interrupt:
				fadeParams.parameters._5HzT1 = 0.8f;
				fadeParams.parameters._5HzT2 = 2.4f;
				break;
			case Strategy.Saccade | Strategy.Random | Strategy.Interrupt:
				fadeParams.parameters._5HzT1 = 0.55f;
				fadeParams.parameters._5HzT2 = 0.165f;
				break;
		}
	}
}

[Serializable]
public class TestData_Conditions
{
	public TrialData_Conditions[] trials;
}

[Serializable]
public class TrialData_Conditions
{
	public string participant;
	public int trialId;

	public Strategy strategy;

	public float startTime;
	public float endtime;

	public bool success;
}
