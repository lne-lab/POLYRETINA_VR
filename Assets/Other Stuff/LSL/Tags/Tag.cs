using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Tags
{
	public class Tag : MonoBehaviour
	{
#pragma warning disable 649
		[SerializeField]
		private int _index;

		[SerializeField]
		private Text _label;
	
		[SerializeField]
		private Image _background;

		[SerializeField]
		private Color _defaultColour;

		[SerializeField]
		private Color _highlightColour;

		[SerializeField]
		private Color _selectedColour;
#pragma warning restore 649

		public int index 
		{
			get => _index;
			set => _index = value;
		}

		private bool _isSelected;
		public bool isSelected 
		{
			get
			{
				return _isSelected;
			}

			set
			{
				_isSelected = value;

				if (_isSelected)
				{
					colour = _selectedColour;

					// unselect all other tags
					foreach (var tag in FindObjectsOfType<Tag>())
					{
						if (tag != this)
						{
							tag.isSelected = false;
						}
					}
				}
				else
				{
					colour = _defaultColour;
				}
			}
		}

		public string label
		{
			get => _label.text;
			set => _label.text = value;
		}

		public Color colour
		{
			get => _background.color;
			set => _background.color = value;
		}

		public void Highlight()
		{
			if (isSelected == false)
			{
				colour = _highlightColour;
			}
		}

		public void Dehighlight()
		{
			colour = isSelected ? _selectedColour : _defaultColour;
		}

		public void Send()
		{
			var outlet = FindObjectOfType<TagOutlet>();
			outlet.PushSample(label);

			TagConsole.Instance.WriteLine(label);
		}

		public void SendFromInput(InputField input)
		{
			var outlet = FindObjectOfType<TagOutlet>();
			outlet.PushSample(input.text);

			TagConsole.Instance.WriteLine(input.text);
		}
	}
}
