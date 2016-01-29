using System;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public class BaseNavigationController : UINavigationController
	{
		public BaseNavigationController (UIViewController rootconroller) : base (rootconroller)
		{
		}

		public override bool ShouldAutorotate ()
		{
			if (this.TopViewController == null) return false;

			return this.TopViewController.ShouldAutorotate();
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			if (this.TopViewController == null) return UIInterfaceOrientationMask.Portrait;

			return this.TopViewController.GetSupportedInterfaceOrientations ();
		}
	}
}

