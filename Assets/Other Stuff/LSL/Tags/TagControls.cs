using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Tags
{
	public class TagControls : Singleton<TagControls>
	{
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				SendSelected();
			}
		}

		private void SendSelected()
		{
			var tags = FindObjectsOfType<Tag>();
			var selectedIndex = 0;

			// send selected tag
			for (int i = 0; i < tags.Length; i++)
			{
				var tag = tags[i];

				if (tag.isSelected)
				{
					tag.GetComponentInChildren<Button>().onClick.Invoke();

					selectedIndex = tag.index;
					break;
				}
			}

			// select next tag
			for (int i = 0; i < tags.Length; i++)
			{
				var tag = tags[i];

				if (tag.index == selectedIndex + 1)
				{
					tag.isSelected = true;
					break;
				}
			}
		}
	}
}
