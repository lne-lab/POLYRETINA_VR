using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LNE.IO
{
	/// <summary>
	/// Represents a CSV file
	/// </summary>
	public class CSV
	{
		public string Separator { get; set; } = ";";

		public string AsString
		{
			get
			{
				var retval = "";
				for (int i = 0; i <= height; i++)
				{
					for (int j = 0; j <= width; j++)
					{
						var hasValue = cells.TryGetValue((j, i), out var val);
						retval += hasValue && val != null ? val.ToString() : "";
						retval += Separator;
					}

					retval += "\n";
				}

				return retval;
			}
		}

		private Dictionary<(int, int), string> cells = new Dictionary<(int, int), string>();
		private int width = 0;
		private int height = 0;

		public CSV() : this(";")
		{

		}

		public CSV(string separator)
		{
			Separator = separator;
		}

		public void AppendRow(params object[] row)
		{
			SetRow(height, row);
		}

		public void AppendColumn(params object[] col)
		{
			SetColumn(width, col);
		}

		public void SetRow(int y, params object[] row)
		{
			for (int i = 0; i < row.Length; i++)
			{
				SetCell(i, y, row[i]);
			}

			width = Mathf.Max(width, row.Length);
			height = Mathf.Max(height, y + 1);
		}

		public void SetColumn(int x, params object[] col)
		{
			for (int i = 0; i < col.Length; i++)
			{
				SetCell(x, i, col[i]);
			}

			width = Mathf.Max(width, x + 1);
			height = Mathf.Max(height, col.Length);
		}

		public void SetCell(int x, int y, object val)
		{
			cells.Add((x, y), val.ToString());
		}

		public void Save(string path)
		{
			File.WriteAllText(path, AsString);
		}
	}
}
