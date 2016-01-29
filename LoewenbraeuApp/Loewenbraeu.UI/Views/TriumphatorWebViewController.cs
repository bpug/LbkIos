using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Loewenbraeu.Core;
using System.IO;
using MonoTouch.QuickLook;
using System.Drawing;

namespace Loewenbraeu.UI
{
	public class TriumphatorWebViewController : BaseViewController
	{
		private UIWebView _webView;
		
		public TriumphatorWebViewController () : base()
		{
			Title = Locale.GetText ("Speisekarte");
			_webView = new UIWebView ();
			_webView.ScalesPageToFit = true;
			//_webView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			_webView.LoadError += (webview, args) => {
				if (_webView != null)
					_webView.LoadHtmlString (
						String.Format ("<html><center><font color='red'>{0}:<br>{1}</font></center></html>",
						"An error occurred:", args.Error.LocalizedDescription), null);
			};
		}
		
		 public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			_webView.Frame = this.View.Bounds;
			this.View.AddSubview (_webView);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			var fileUrl = NSUrl.FromFilename ("Docs/Triumphator.pdf");
			_webView.LoadRequest (new NSUrlRequest (fileUrl));
		}
	}
}

