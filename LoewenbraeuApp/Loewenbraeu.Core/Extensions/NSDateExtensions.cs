using System;
using MonoTouch.Foundation;

namespace Loewenbraeu.Core.Extensions
{
	public static class NSDateExtensions
	{
		public static DateTime ToDateTime (this NSDate date)
		{
			return (new DateTime (2001, 1, 1, 0, 0, 0)).AddSeconds (date.SecondsSinceReferenceDate);
		}
	}
}

