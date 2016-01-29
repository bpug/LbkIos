using System;
using MonoTouch.Foundation;

namespace Loewenbraeu.Core.Extensions
{
	public static class DateTimeExtensions
	{
		public static NSDate ToNSDate (this DateTime source)
		{
			return NSDate.FromTimeIntervalSinceReferenceDate 
				((source - (new DateTime (2001, 1, 1, 0, 0, 0))).TotalSeconds);
			
		}
		
		public static string ToLongDateTimeString (this DateTime source)
		{
			return string.Format ("{0} {1}", source.ToLongDateString(), source.ToShortTimeString());
		}
	}
}

