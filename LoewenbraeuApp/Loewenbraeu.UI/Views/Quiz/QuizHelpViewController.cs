using System;
using MonoTouch.UIKit;
using Loewenbraeu.Core;

namespace Loewenbraeu.UI
{
	public partial class QuizHelpViewController : BaseViewController
	{
		public QuizHelpViewController () : base ("QuizHelpViewController", null)
		{
			
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.navItem.Title = Locale.GetText("Spielanleitung");
			this.navItem.LeftBarButtonItem = new UIBarButtonItem (Locale.GetText("zurück"), UIBarButtonItemStyle.Plain, OnBackButton);
			
			this.View.BringSubviewToFront (this.navBar);
			this.View.BringSubviewToFront (this.txtHelp);
			txtHelp.TextColor = UIColor.White;
			txtHelp.BackgroundColor = UIColor.Clear;
		}

		/*
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Release any retained subviews of the main view.
			// e.g. myOutlet = null;
		}


		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
*/
		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.AllButUpsideDown;
		}
		
		private void OnBackButton(object sender, EventArgs args)
		{
			this.DismissViewController (true, null); 
		}
	}
}

