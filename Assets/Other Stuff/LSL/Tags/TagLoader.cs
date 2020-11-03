using UnityEngine;

namespace LNE.LabStreamingLayer.Tags
{
	using IO;
	using UI.Attributes;

	public class TagLoader : Singleton<TagLoader>
	{
		/*
		 * Editor fields
		 */

#pragma warning disable 649
		[SerializeField, Path(isFile = true, isRelative = true)]
		private string _path;

		[Space]

		[SerializeField]
		private GameObject _tagUI;

		[Space]

		[SerializeField]
		private Canvas _canvas;

		[SerializeField]
		private GameObject _customTag;

		[SerializeField]
		private GameObject _noLabelTag;
#pragma warning restore 649

		/*
		 * Private fields
		 */

		private CSV csv;

		/*
		 * Unity callbacks
		 */

		void Start()
		{
			csv = new CSV();
			csv.Load(_path);

			var i = 0;

			PositionTagUI(_customTag, i++);
			PositionTagUI(_noLabelTag, i++);

			foreach (var label in csv.GetColumn(0, true))
			{
				if (label == "")
					continue;

				var ui = Instantiate(_tagUI, _canvas.transform);
				PositionTagUI(ui, i);

				var tag = ui.GetComponent<Tag>();
				tag.index = i;
				tag.label = label;
				tag.isSelected = i == 2;

				i++;

				if (i >= 40)
				{
					break;
				}
			}
		}

		private void PositionTagUI(GameObject ui, int position)
		{
			var canvasRt = _canvas.transform as RectTransform;
			var rt = ui.transform as RectTransform;
			var x = -canvasRt.sizeDelta.x / 2 + rt.sizeDelta.x / 2;
			var y = +canvasRt.sizeDelta.y / 2 - rt.sizeDelta.y / 2;

			rt.anchoredPosition = new Vector2(x, y - position * rt.sizeDelta.y);

			while (rt.anchoredPosition.y < -canvasRt.sizeDelta.y / 2)
			{
				rt.anchoredPosition += new Vector2(rt.sizeDelta.x, canvasRt.sizeDelta.y);
			}
		}
	}
}
