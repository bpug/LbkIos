using System;
using System.Collections;

namespace Loewenbraeu.Core.Extensions
{
	public static class ICollectionExtensions
	{
		public static bool IsNullOrEmpty (this ICollection  source)
		{
			return source == null || source.Count == 0;
		} 
	}
}

