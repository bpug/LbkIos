using System;
using MonoTouch.UIKit;

namespace Loewenbraeu.Core.Extensions
{
	public static class UIViewControllerExtensions
	{
		public static void Release (this UIViewController source)
		{
			if (source != null) {
				source.Dispose ();
				source = null;
			}
		}
	}
}

