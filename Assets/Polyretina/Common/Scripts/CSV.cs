using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LNE.IO
{
	using ArrayExts;

	/// <summary>
	/// Represents a CSV file
	/// </summary>
	public class CSV
	{
		public char Separator { get; set; } = ';';

		public int Width { get; private set; } = 0;
		public int Height { get; private set; } = 0;

		public string AsString
		{
			get
			{
				var retval = "";
				for (int i = 0; i <= Height; i++)
				{
					for (int j = 0; j <= Width; j++)
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

		public string[] GetRow(int row)
		{
			var retval = new string[Width];
			for (int i = 0; i < Width; i++)
			{
				retval[i] = GetCell(i, row);
			}

			return retval;
		}

		public string[] GetColumn(int column, bool includeHeader)
		{
			var offset = includeHeader ? 0 : 1;

			var retval = new string[Height - offset];
			for (int i = offset; i < Height; i++)
			{
				retval[i - offset] = GetCell(column, i);
			}

			return retval;
		}
		
		public string[] GetColumn(string header, bool includeHeader)
		{
			return GetColumn(GetRow(0).IndexOf(header), includeHeader);
		}

		public string GetCell(int x, int y)
		{
			return cells.TryGetValue((x, y), out var val) ? val : null;
		}

		public string GetCell(string header, int index)
		{
			return GetCell(GetRow(0).IndexOf(header), index);
		}

		public void AppendRow(params object[] row)
		{
			SetRow(Height, row);
		}

		public void AppendColumn(params object[] col)
		{
			SetColumn(Width, col);
		}

		public void SetRow(int y, params object[] row)
		{
			for (int i = 0; i < row.Length; i++)
			{
				SetCell(i, y, row[i]);
			}

			Width = Mathf.Max(Width, row.Length);
			Height = Mathf.Max(Height, y + 1);
		}

		public void SetColumn(int x, params object[] col)
		{
			for (int i = 0; i < col.Length; i++)
			{
				SetCell(x, i, col[i]);
			}

			Width = Mathf.Max(Width, x + 1);
			Height = Mathf.Max(Height, col.Length);
		}

		public void SetCell(int x, int y, object val)
		{
			cells.Add((x, y), val.ToString());
		}

		/// <summary>
		/// Load from a file.
		/// </summary>
		public bool Load(string path)
		{
			if (File.Exists(path) == false)
				return false;

			var data = File.ReadAllText(path);

			// remove double eol characters
			data = data.Replace("\r\n", "\n");
			data = data.Replace("\n\r", "\n");

			foreach (var line in data.Split('\n', '\r'))
			{
				AppendRow(line.Split(Separator));
			}

			return true;
		}

		/// <summary>
		/// Load using a Stream Reader. Safer for large databases.
		/// </summary>
		public bool LoadWStream(string path)
		{
			if (File.Exists(path) == false)
				return false;

			using (var sr = new StreamReader(path))
			{
				while (sr.Peek() >= 0)
				{
					var line = sr.ReadLine();
					var row = line.Split(Separator);

					AppendRow(row);
				}
			}

			return true;
		}

		/// <summary>
		/// Save to a file.
		/// </summary>
		public void Save(string path)
		{
			File.WriteAllText(path, AsString);
		}

		/// <summary>
		/// Save using a Stream Writer. Safer for large databases.
		/// </summary>
		public void SaveWStream(string path)
		{
			using (var sw = new StreamWriter(path))
			{
				for (int i = 0; i <= Height; i++)
				{
					var line = "";
					for (int j = 0; j <= Width; j++)
					{
						var hasValue = cells.TryGetValue((j, i), out var val);
						line += hasValue && val != null ? val.ToString() : "";
						line += Separator;
					}

					sw.WriteLine(line);
				}
			}
		}
	}
}
