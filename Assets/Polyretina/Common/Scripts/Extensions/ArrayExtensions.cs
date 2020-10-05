using System;
using System.Collections.Generic;

namespace LNE.ArrayExts
{
	using Threading;

	/// <summary>
	/// Collection of useful methods for arrays
	/// </summary>
	public static class ArrayExtensions
	{
		/*
		 * Create
		 */ 

		/// <summary>
		/// Create array of length using func
		/// </summary>
		public static T[] CreateArray<T>(int length, Func<int, T> func)
		{
			var result = new T[length];

			for (int i = 0; i < length; i++)
			{
				result[i] = func(i);
			}

			return result;
		}

		/// <summary>
		/// Create array of length using func
		/// </summary>
		public static T[] CreateArray<T>(int length, Func<T> func)
		{
			return CreateArray(length, (i) => func());
		}

		/// <summary>
		/// Create an array of experimental conditions from several factors
		/// </summary>
		public static T0[] CreateArray<T0, T1>(T1[] factor1, int repetitions, Func<T1, T0> func)
		{
			var result = new T0[factor1.Length * repetitions];

			var i = 0;
			foreach (var t1 in factor1)
			{
				for (int j = 0; j < repetitions; j++)
				{
					result[i++] = func(t1);
				}
			}

			return result;
		}

		/// <summary>
		/// Create an array of experimental conditions from several factors
		/// </summary>
		public static T0[] CreateArray<T0, T1, T2, T3>(T1[] factor1, T2[] factor2, T3[] factor3, int repetitions, Func<T1, T2, T3, T0> func)
		{
			var result = new T0[factor1.Length * factor2.Length * factor3.Length * repetitions];

			var i = 0;
			foreach (var t1 in factor1)
			{
				foreach (var t2 in factor2)
				{
					foreach (var t3 in factor3)
					{
						for (int j = 0; j < repetitions; j++)
						{
							result[i++] = func(t1, t2, t3);
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Create an array of experimental conditions from several factors
		/// </summary>
		public static T0[] CreateArray<T0, T1, T2, T3, T4>(T1[] factor1, T2[] factor2, T3[] factor3, T4[] factor4, int repetitions, Func<T1, T2, T3, T4, T0> func)
		{
			var result = new T0[factor1.Length * factor2.Length * factor3.Length * factor4.Length * repetitions];

			var i = 0;
			foreach (var t1 in factor1)
			{
				foreach (var t2 in factor2)
				{
					foreach (var t3 in factor3)
					{
						foreach (var t4 in factor4)
						{
							for (int j = 0; j < repetitions; j++)
							{
								result[i++] = func(t1, t2, t3, t4);
							}
						}
					}
				}
			}

			return result;
		}

		public static T[] Copy<T>(this T[] that)
		{
			var from = 0;
			var to = that.Length;

			var retval = new T[to - from];

			for (int i = 0; i < retval.Length; i++)
			{
				retval[i] = that[from + i];
			}

			return retval;
		}

		/*
		 * For Each
		 */

		/// <summary>
		/// Take action for each array element
		/// </summary>
		public static void ForEach<T>(this T[] that, Action<int, T> action)
		{
			for (int i = 0; i < that.Length; i++)
			{
				action(i, that[i]);
			}
		}

		/// <summary>
		/// Take action for each array element
		/// </summary>
		public static void ForEach<T>(this T[] that, Action<T> action)
		{
			that.ForEach((i, t) => action(t));
		}

		/*
		 * Conversions
		 */

		/// <summary>
		/// Convert array using func
		/// </summary>
		public static T2[] Convert<T1, T2>(this T1[] that, Func<int, T1, T2> func)
		{
			return CreateArray(that.Length, (i) => func(i, that[i]));
		}

		/// <summary>
		/// Convert array using func
		/// </summary>
		public static T2[] Convert<T1, T2>(this T1[] that, Func<T1, T2> func)
		{
			return that.Convert((i, t1) => func(t1));
		}

		/// <summary>
		/// Converge into single value
		/// </summary>
		public static T2 Converge<T1, T2>(this T1[] that, T2 value, Func<int, T1, T2, T2> func)
		{
			for (int i = 0; i < that.Length; i++)
			{
				value = func(i, that[i], value);
			}

			return value;
		}

		/// <summary>
		/// Converge into single value
		/// </summary>
		public static T2 Converge<T1, T2>(this T1[] that, T2 value, Func<T1, T2, T2> func)
		{
			return that.Converge(value, (i, t1, t2) => func(t1, t2));
		}

		/// <summary>
		/// Apply func to each array elemtn
		/// </summary>
		public static T[] Apply<T>(this T[] that, Func<int, T, T> func)
		{
			for (int i = 0; i < that.Length; i++)
			{
				that[i] = func(i, that[i]);
			}

			return that;
		}

		/// <summary>
		/// Apply func to each array element
		/// </summary>
		public static T[] Apply<T>(this T[] that, Func<T, T> func)
		{
			return that.Apply((i, t) => func(t));
		}

		/// <summary>
		/// Combine values and prevent duplicates
		/// </summary>
		public static T[] Combine<T>(this T[] that, params T[] other)
		{
			var result = new List<T>(that);
			foreach (var t in other)
			{
				if (result.Contains(t) == false)
				{
					result.Add(t);
				}
			}

			return result.ToArray();
		}

		/// <summary>
		/// Flatten 2D array into 1D
		/// </summary>
		public static T[] Flatten<T>(this T[,] that, bool columnMajor = true, bool leftToRight = true, bool topToBottom = true)
		{
			var ni = that.GetLength(0);
			var nj = that.GetLength(1);

			var result = new T[ni * nj];

			if (columnMajor)
			{
				for (int i = 0; i < ni; i++)
				{
					for (int j = 0; j < nj; j++)
					{
						var x = leftToRight ? i : ni - 1 - i;
						var y = topToBottom ? j : nj - 1 - j;

						result[i * nj + j] = that[x, y];
					}
				}
			}
			else
			{
				for (int j = 0; j < nj; j++)
				{
					for (int i = 0; i < ni; i++)
					{
						var x = leftToRight ? i : ni - 1 - i;
						var y = topToBottom ? j : nj - 1 - j;

						result[j * ni + i] = that[x, y];
					}
				}
			}

			return result;
		}

		public static string ToCSV<T>(bool columnMajor, string separator, params T[][] data)
		{
			return data.Converge("", (col, result2) =>
			{
				var ccol = col.Converge("", (cell, result1) =>
				{
					return result1 + cell.ToString() + (columnMajor ? "\n" : separator);
				});
				
				return result2 + ccol + (columnMajor ? separator : "\n");
			});
		}

		/*
		 * Find
		 */

		/// <summary>
		/// Returns all elements for which func returns true
		/// </summary>
		public static T[] Where<T>(this T[] that, Func<int, T, bool> func)
		{
			var result = new List<T>();
			for (int i = 0; i < that.Length; i++)
			{
				if (func(i, that[i]))
				{
					result.Add(that[i]);
				}
			}

			return result.ToArray();
		}

		/// <summary>
		/// Returns all elements for which func returns true
		/// </summary>
		public static T[] Where<T>(this T[] that, Func<T, bool> func)
		{
			return that.Where((i, t) => func(t));
		}

		/// <summary>
		/// Returns index of first instance of value in array
		/// </summary>
		public static int IndexOf<T>(this T[] that, T value)
		{
			for (int i = 0; i < that.Length; i++)
			{
				if (that[i].Equals(value))
				{
					return i;
				}
			}

			throw new Exception();
		}

		/// <summary>
		/// Returns index of first instance of value in array
		/// </summary>
		public static int IndexOfOrZero<T>(this T[] that, T value)
		{
			int index;
			try
			{
				index = that.IndexOf(value);
			}
			catch 
			{
				index = 0;
			}

			return index;
		}

		/// <summary>
		/// Returns random element
		/// </summary>
		public static T Random<T>(this T[] that)
		{
			return that[new Random().Next(0, that.Length - 1)];
		}

		/// <summary>
		/// Returns random element
		/// </summary>
		public static T Random<T>(this T[] that, int seed)
		{
			return that[new Random(seed).Next(0, that.Length - 1)];
		}

		/// <summary>
		/// Check if array contains the value
		/// </summary>
		public static bool Contains<T>(this T[] that, T value)
		{
			return that.Converge(false, (e, result) => result || e.Equals(value));
		}

		/// <summary>
		/// Analogous to Substring
		/// </summary>
		public static T[] Subarray<T>(this T[] that, int startIndex, int length)
		{
			var result = new T[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = that[startIndex + i];
			}

			return result;
		}

		/*
		 * Organise
		 */

		/// <summary>
		/// Swap two elements
		/// </summary>
		public static T[] Swap<T>(this T[] that, int i, int j)
		{
			var tmp = that[i];
			that[i] = that[j];
			that[j] = tmp;

			return that;
		}

		/// <summary>
		/// Randomise elements
		/// </summary>
		public static T[] Randomise<T>(this T[] that, int seed = -1)
		{
			var random = seed >= 0 ? new System.Random(seed) : new System.Random();
			for (int i = 0; i < that.Length; i++)
			{
				that.Swap(i, random.Next(that.Length));
			}

			return that;
		}

		/// <summary>
		/// Remove element
		/// </summary>
		public static T[] Remove<T>(this T[] that, T value)
		{
			var result = new List<T>(that);
			result.Remove(value);

			return result.ToArray();
		}

		/// <summary>
		/// Remove element(s)
		/// </summary>
		public static T[] Remove<T>(this T[] that, Func<T, bool> func)
		{
			var result = new List<T>();
			foreach (var t in that)
			{
				if (func(t) == false)
				{
					result.Add(t);
				}
			}

			return result.ToArray();
		}

		/// <summary>
		/// Shift all elements
		/// </summary>
		public static T[] Shift<T>(this T[] that, int shifts)
		{
			var n = that.Length;

			while (shifts < 0)
			{
				shifts += n;
			}

			var cached = that[shifts % n];

			for (int i = 0; i < n; i++)
			{
				var from = i;
				var to = (i + shifts) % n;

				that[to] = that[from];
			}

			that[(n - 1 + shifts) % n] = cached;

			return that;
		}

		/*
		 * Equality
		 */

		public static bool EqualsArray<T>(this T[] that, T[] other)
		{
			// true if both are null
			if (that == null && other == null)
				return true;

			// false is only one is null
			if (that != null && other == null || that != null && other == null)
				return false;

			// false if different length
			if (that.Length != other.Length)
				return false;

			// main
			return that.Converge(true, (i, e, result) => result && e.Equals(other[i]));
		}

		/*
		 * Threaded
		 */

		public static T[] Create_Parallelised<T>(int length, Func<int, T> func)
		{
			var result = new T[length];

			Parallelise(length, (i) => {
				result[i] = func(i);
			});

			return result;
		}

		public static T[] Create_Parallelised<T>(int length, Func<T> func)
		{
			return Create_Parallelised(length, (i) => func());
		}

		public static T[] ForEach_Parallelised<T>(this T[] that, Action<int, T> action)
		{
			Parallelise(that.Length, (i) => action(i, that[i]));
			return that;
		}

		public static T[] ForEach_Parallelised<T>(this T[] that, Action<T> action)
		{
			return that.ForEach_Parallelised((i, t) => action(t));
		}

		public static T2[] Convert_Parallelised<T1, T2>(this T1[] that, Func<int, T1, T2> func)
		{
			return Create_Parallelised(that.Length, (i) => func(i, that[i]));
		}

		public static T2[] Convert_Parallelised<T1, T2>(this T1[] that, Func<T1, T2> func)
		{
			return that.Convert_Parallelised((i, t1) => func(t1));
		}

		public static T2 Converge_Parallelised<T1, T2>(this T1[] that, T2 value, Func<int, T1, T2, T2> func)
		{
			Parallelise(that.Length, (i) =>
			{
				value = func(i, that[i], value);
			});

			return value;
		}

		public static T2 Converge_Parallelised<T1, T2>(this T1[] that, T2 value, Func<T1, T2, T2> func)
		{
			return that.Converge_Parallelised(value, (i, t1, t2) => func(t1, t2));
		}

		public static T[] Apply_Parallelised<T>(this T[] that, Func<int, T, T> func)
		{
			Parallelise(that.Length, (i) =>
			{
				that[i] = func(i, that[i]);
			});

			return that;
		}

		public static T[] Apply_Parallelised<T>(this T[] that, Func<T, T> func)
		{
			return that.Apply_Parallelised((i, t) => func(t));
		}

		private static void Parallelise(int length, Action<int> action)
		{
			var threads = new ThreadGroup();
			threads.ProcessArray(length, action);
			threads.Join();
		}
	}
}
