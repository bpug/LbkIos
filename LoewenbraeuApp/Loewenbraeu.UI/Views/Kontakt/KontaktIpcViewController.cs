using System;
using MonoTouch.UIKit;
using Loewenbraeu.Core;
using MonoTouch.Foundation;
using System.IO;
using MonoTouch.MessageUI;

namespace Loewenbraeu.UI
{
	public class KontaktIpcViewController : BaseViewController
	{
		private UIWebView _webView;
		
		enum Action{
			Http,
			Tel,
			Fax,
			Email
		}
		
		public KontaktIpcViewController () :base()
		{
			_webView = new UIWebView (){
				DataDetectorTypes = UIDataDetectorType.Link,
				ShouldStartLoad = HandleLoadStarted,
				BackgroundColor = UIColor.Clear,
				Opaque = false,
				AutoresizingMask = UIViewAutoresizing.FlexibleMargins | UIViewAutoresizing.FlexibleDimensions,
			};
			_webView.ScrollView.BackgroundColor = UIColor.Clear;
			_webView.ScrollView.Opaque = false;
			
			//this.View = _webView;
			//this.View.BackgroundColor = Resources.Colors.Background;
		}
		
		public override void ViewDidLoad ()
		{
			
			base.ViewDidLoad ();
			
			string html = string.Empty;
			try {
				using (TextReader textReader = new StreamReader("Data/kontakt2.html")) {
					html = textReader.ReadToEnd ();
				}
			} catch {
			}
			
			string bundlePath = NSBundle.MainBundle.BundlePath + "/";
			_webView.LoadHtmlString (html, new NSUrl (bundlePath, true));
			//_webView.LoadRequest (new NSUrlRequest (NSUrl.FromFilename ("data/kontakt.html")));
			
			_webView.Frame = this.View.Bounds;
			this.View.AddSubview (_webView);
		}

		public bool HandleLoadStarted (UIWebView webView, MonoTouch.Foundation.NSUrlRequest request, UIWebViewNavigationType navigationType)
		{
			if (navigationType == UIWebViewNavigationType.LinkClicked) {
				string scheme = request.Url.Scheme;
				if (scheme.StartsWith ("tel")) {
					DoAction (Action.Tel, request);
					return false;
				} else if (scheme.StartsWith ("mailto")) {
					DoAction (Action.Email, request);
					return false;
				} else if (scheme.StartsWith ("http")) {
					DoAction (Action.Http, request);
					return false;
				} else if (scheme.StartsWith ("fax")) {
					return false;
				}
			}
			return true;
		}
		
		private void DoAction(Action action, MonoTouch.Foundation.NSUrlRequest request){
			switch (action) {
				case Action.Tel:
					var phoneCaller = new PhoneCaller ();
					phoneCaller.Call (request.Url.ResourceSpecifier);
					break ;
				case Action.Email:
					if (MFMailComposeViewController.CanSendMail) {
						var mail = new MFMailComposeViewController ();
						mail.SetToRecipients (new string[] { request.Url.ResourceSpecifier});
						mail.SetSubject ("");
						mail.SetMessageBody ("", false);
						mail.Finished += HandleMailFinished;
						this.PresentViewController (mail, true, null);
					} 
					break;
				case Action.Http:
					if (UIApplication.SharedApplication.CanOpenUrl(request.Url)) {
						UIApplication.SharedApplication.OpenUrl (request.Url);
					}
					break;
			}
		}
		
		private void HandleMailFinished (object sender, MFComposeResultEventArgs e)
		{
			switch (e.Result) {
			case MFMailComposeResult.Cancelled:
				break;
			case MFMailComposeResult.Saved:
				break;
			case MFMailComposeResult.Sent:
				using (var alert = new UIAlertView ("Email",Locale.GetText("Ihre Nachricht wurde gesendet."), null,"OK", null)) {
					alert.Show ();
				}
				break;
			case MFMailComposeResult.Failed:
				break;
			default:
				using (var alert = new UIAlertView ("Email",Locale.GetText("Sending Failed - Unknown Error"), null,"OK", null)) {
					alert.Show ();
				} 
				break;
			}
			e.Controller.DismissViewController (true, null);
		}
	}
}

