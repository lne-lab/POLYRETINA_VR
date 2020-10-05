using System;
using System.Linq;

namespace LNE.StringExts
{
	/// <summary>
	/// Collection of useful methods for strings
	/// </summary>
	public static class StringExtensions
    {
        public static string FirstCharToUpper(this string that)
        {
            switch (that)
            {
                case null:  throw new ArgumentNullException(nameof(that));
                case "":    throw new ArgumentException($"{nameof(that)} cannot be empty", nameof(that));
                default:    return that.First().ToString().ToUpper() + that.Substring(1);
            }
        }

        public static int AsUid(this string that)
		{
            var result = 0;
            foreach (char c in that)
			{
                result += c;
			}

            return result;
		}
    }
}
