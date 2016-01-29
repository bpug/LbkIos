using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Loewenbraeu.Core
{
	public class PhoneCaller : NSObject
	{
		private UIWebView _webview;
		
		public PhoneCaller () :base()
		{
			_webview = new UIWebView ();
		}
		
		public void Call (string phone)
		{
			NSUrl nurl = new NSUrl ("tel:" + phone);
			_webview.LoadRequest (new NSUrlRequest (nurl));
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				_webview = null;
			}
			base.Dispose (disposing);
		}
	}
}

