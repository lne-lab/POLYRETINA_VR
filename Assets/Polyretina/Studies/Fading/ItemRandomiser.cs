using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LNE.ArrayExts;
using LNE.IO;
using LNE.StringExts;
using LNE.UI.Attributes;

public class ItemRandomiser : MonoBehaviour
{
	[Path]
	public string path;
	public string participant;

	public int startingTrial;

	[Space]

	public string[] itemNames;

	private Text[] _textBoxes;
	private List<TrialData_Items> _trials;

	private int _seed;
	private int _trialId;

	void Awake()
	{
		_textBoxes = FindObjectsOfType<Text>();
		_trials = new List<TrialData_Items>();

		_seed = participant.AsUid();
		_trialId = startingTrial;
	}

	void Start()
	{
		if (itemNames.Length != _textBoxes.Length)
		{
			Debug.LogError("Wrong number of items or text boxes.");
			Application.Quit();
		}
	}

	void OnApplicationQuit()
	{
		var fileId = DateTime.Now.Ticks;
		SaveAsJson(fileId);
		SaveAsCsv(fileId);
	}

	private void SaveAsJson(long id)
	{
		var data = new TestData_Items { trials = _trials.ToArray() };
		var json = JsonUtility.ToJson(data, true);

		File.WriteAllText(
			path + $"Fading_Items_{participant}_json_{id}.json", json
		);
	}

	private void SaveAsCsv(long id)
	{
		var csv = new CSV();
		csv.AppendRow(GetHeaders());

		foreach (var trial in _trials)
		{
			csv.AppendRow(GetRow(trial.participant, trial.trialId, trial.itemNames, trial.chosenItem));
		}

		csv.Save(path + $"Fading_Items_{participant}_csv_{id}.csv");
	}

	private string[] GetHeaders()
	{
		var header = new string[2 + itemNames.Length + 1];
		header[0] = "participant";
		header[1] = "trial";
		header[header.Length - 1] = "chosen";

		for (int i = 0; i < itemNames.Length; i++)
		{
			header[2 + i] = i.ToString();
		}

		return header;
	}

	private string[] GetRow(string participant, int trialId, string[] items, string chosenItem)
	{
		var row = new string[2 + items.Length + 1];
		row[0] = participant;
		row[1] = trialId.ToString();
		row[row.Length - 1] = chosenItem;

		for (int i = 0; i < items.Length; i++)
		{
			row[2 + i] = items[i];
		}

		return row;
	}

	public void UpdateItems()
	{
		var trialSeed = _seed + _trialId;

		itemNames.Randomise(trialSeed).ForEach((i, name) => { _textBoxes[i].text = name; });

		_textBoxes.ForEach((textBox) => { textBox.color = Color.black; });
		var chosenTextBox = _textBoxes.Random(trialSeed);
		chosenTextBox.color = Color.red;

		_trials.Add(
			new TrialData_Items { 
				participant = participant, 
				trialId = _trialId, 
				itemNames = itemNames.Copy(), 
				chosenItem = chosenTextBox.text 
			}
		);

		_trialId++;
	}
}

[Serializable]
public class TestData_Items
{
	public TrialData_Items[] trials;
}

[Serializable]
public class TrialData_Items
{
	public string participant;
	public int trialId;

	public string[] itemNames;
	public string chosenItem;
}
