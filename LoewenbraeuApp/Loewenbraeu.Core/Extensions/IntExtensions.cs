using System;

namespace Loewenbraeu.Core.Extensions
{
	public static class IntExtensions
	{
		public static string ToLetter (this int source)
		{

			string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string retVal = string.Empty;
			double dvalue = Convert.ToDouble(source);
			do {

				double remainder = dvalue - (26 * Math.Truncate (dvalue / 26));
				retVal = retVal + letters.Substring ((int)remainder, 1);
				dvalue = Math.Truncate (dvalue / 26);
			} while (dvalue > 0);
			return retVal;

		}
	}
}

