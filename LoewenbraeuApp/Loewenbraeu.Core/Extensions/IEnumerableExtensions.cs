using System;
using System.Linq;
using System.Collections.Generic;

namespace Loewenbraeu.Core.Extensions
{
	public static class IEnumerableExtensions
	{
		public static bool IsNullOrEmpty<T> (this IEnumerable<T> source)
		{
			return source == null || !source.Any ();
		}
	}
}

