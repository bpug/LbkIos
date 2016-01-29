using Loewenbraeu.Model;
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
using System;

namespace Loewenbraeu.UI
{
	public partial class MoviePlayerViewController : UIViewController
	{
		private Video _video;
		private MPMoviePlayerController _moviePlayer;
		
		public MoviePlayerViewController (Video video) : base ()
		{
			_video = video;
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
			
			//any additional setup after loading the view, typically from a nib.
			//_someView = new UIView();
			//_someView.Frame = window.Bounds;        
			_moviePlayer = new MPMoviePlayerController (new NSUrl (_video.Url));
			_moviePlayer.View.Frame = this.View.Frame;
			//_moviePlayer.ControlStyle = MPMovieControlStyle.Embedded;
			//_moviePlayer.Title = _video.Title;
			
			this.View.AddSubview (_moviePlayer.View);  
			_moviePlayer.ScalingMode = MPMovieScalingMode.AspectFit;
			_moviePlayer.Fullscreen = true;
			_moviePlayer.ControlStyle = MPMovieControlStyle.Fullscreen;
			//this.PresentMoviePlayerViewController (_moviePlayer);
			//window.AddSubview(_someView);          
			_moviePlayer.Play ();           
			//window.MakeKeyAndVisible ();
		}

		/*
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			//_moviePlayer.Stop ();
			_moviePlayer.Dispose ();
			// Release any retained subviews of the main view.
			// e.g. myOutlet = null;
		}
		*/

		protected override void Dispose (bool disposing)
		{
			if (disposing){
				_moviePlayer.Dispose ();
			}
			base.Dispose (disposing);
		}

		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			_moviePlayer.View.Frame = this.View.Frame;
			
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.AllButUpsideDown;
		}
	}
}

