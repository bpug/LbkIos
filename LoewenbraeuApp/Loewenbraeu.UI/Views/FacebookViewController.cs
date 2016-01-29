using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;

namespace Loewenbraeu.UI
{
	public class FacebookViewController : BaseHudViewController
	{
		private const string START_URL= "http://www.facebook.com/Loewenbraeukeller";
		
		private UIWebView _webView;
		
		public FacebookViewController ()
		{
			Initialize ();
		}
		
		private void Initialize ()
		{
			Title = "Facebook";
			_webView = new UIWebView ();
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			UnregisterKeyboardNotifications ();
			
			_webView.Frame = this.View.Bounds;
			_webView.LoadStarted += delegate {
				Util.PushNetworkActive ();
				ShowHud ();
			};
			_webView.LoadFinished += delegate {
				Util.PopNetworkActive ();
				HideHud ();
				Title = "";
			};
			_webView.LoadError += delegate {
				Util.PopNetworkActive ();
				HideHud ();
			};
				
			_webView.BackgroundColor = UIColor.Clear;
			_webView.Opaque = false;
			_webView.AutoresizingMask = UIViewAutoresizing.FlexibleMargins | UIViewAutoresizing.FlexibleDimensions;
			
			this.View.AddSubview (_webView);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			var urlRequest = new NSUrlRequest (new NSUrl (START_URL));
			_webView.LoadRequest (urlRequest);
		}

		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.All;
		}


		/*
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
		}
		*/

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				_webView.Release ();
			}
			base.Dispose (disposing);
		}
	}
}

