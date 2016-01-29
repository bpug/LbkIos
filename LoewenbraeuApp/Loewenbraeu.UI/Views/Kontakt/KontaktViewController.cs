using System.Drawing;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using MonoTouch.CoreLocation;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;
using System;
using System.IO;

namespace Loewenbraeu.UI
{
	public partial class KontaktViewController : BaseViewController
	{
		private const string PHONE_NUMBER = "+498954726690";
		private const string EMAIL = "info@loewenbraeukeller.com";
		private const double LATITUDE_LBK = 48.147849;
		private const double LONGITUDE_LBK = 11.558634;
		private CLLocationCoordinate2D _userCoords;
		private CLLocationCoordinate2D _lbkCoords = new CLLocationCoordinate2D (LATITUDE_LBK, LONGITUDE_LBK);
		private CLLocationManager _locationManager = null;
		
		UILabel lblImpressum;
		UIWebView wvImpressum;
		
		public KontaktViewController () : base ("KontaktViewController", null)
		{
			Title = "Kontakt";
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
			
			
			UIButton btnIpConnect = UIButton.FromType (UIButtonType.InfoLight);
			btnIpConnect.Title (UIControlState.Normal);
			btnIpConnect.TouchDown += OnIpConnect;
			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem (btnIpConnect);
			
			
			//this.View.BringSubviewToFront (this.scrollContainer);
			
			scrollContainer.ContentSize = new SizeF (this.View.Bounds.Size.Width, this.View.Bounds.Size.Height - 44);
			
			_locationManager = new CLLocationManager ();
			_locationManager.DesiredAccuracy = CLLocation.AccuracyBest;


			// deprecated in ios 6.0
			_locationManager.UpdatedLocation += (object sender, CLLocationUpdatedEventArgs e) => {
				_userCoords = e.NewLocation.Coordinate;
				// get the distance from here to Löwenbräukeller
				var distance = e.NewLocation.DistanceFrom (new CLLocation (LATITUDE_LBK, LONGITUDE_LBK));
				string distanceText = Locale.Format("Bis zum Ziel ca: {0}", Util.DistanceToString (distance));
				txtDistance.Text = distanceText;
				if (CLLocationManager.LocationServicesEnabled) {
					_locationManager.StopUpdatingLocation ();
				}
			};

			// for ios 6.0
			_locationManager.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) => {

				CLLocation recentLocation = e.Locations[e.Locations.Length - 1];
				_userCoords = recentLocation.Coordinate;
				// get the distance from here to Löwenbräukeller
				var distance = recentLocation.DistanceFrom (new CLLocation (LATITUDE_LBK, LONGITUDE_LBK));
				string distanceText = Locale.Format("Bis zum Ziel ca: {0}", Util.DistanceToString (distance));
				txtDistance.Text = distanceText;
				if (CLLocationManager.LocationServicesEnabled) {
					_locationManager.StopUpdatingLocation ();
				}
			};
			
			txtPlan.TextColor = UIColor.White;
			txtPlan.BackgroundColor = UIColor.Clear;
			
			scrollContainer.SetLabelsTextColor (UIColor.White);
			scrollContainer.SetLabelsBGColor (UIColor.Clear);
			
			txtAddress.Text = "Löwenbräukeller Gastronomie GmbH\n" +
							"Nymphenburgerstrasse 2\n" + 
							"80335 München";
			txtPhone.Text = "Tel.: +49 (0)89 - 547 2669-0";
			txtFax.Text = "Fax: +49 (0)89 - 547 2669-25";
			txtMail.Text = EMAIL;
		
			
			btnPhone.SetImage (UIImage.FromBundle ("image/buttons/phone.png"), UIControlState.Normal);
			btnPhone.TouchUpInside += delegate {
				var phoneCaller = new PhoneCaller ();
				phoneCaller.Call (PHONE_NUMBER);
			};
			
			btnMail.SetImage (UIImage.FromBundle ("image/buttons/mail.png"), UIControlState.Normal);
			/*
			btnMail.TouchDown += delegate {
				var alert = new UIAlertView (EMAIL, "", null, Locale.GetText ("Cancel"), Locale.GetText ("Mailen"));
				alert.Clicked += (sender, e) => {
					if (e.ButtonIndex == 1) {
						Util.OpenUrl ("mailto:" + EMAIL);
					}
				};
				alert.Show ();
			};
			*/
			btnMail.TouchUpInside += (o, e) =>
			{
				if (MFMailComposeViewController.CanSendMail) {
					var mail = new MFMailComposeViewController ();
					mail.SetToRecipients (new string[] { EMAIL});
					mail.SetSubject ("Löwenbräu");
					mail.SetMessageBody ("", false);
					mail.Finished += HandleMailFinished;
					this.PresentViewController (mail, true, null);
				} 
			};
			
			btnMap.SetImage (UIImage.FromBundle ("image/buttons/map.png"), UIControlState.Normal);
			btnMap.TouchUpInside += delegate {
				this.NavigationController.PushViewController (new LbkMapViewController (_lbkCoords, _userCoords), true);
			};
			
			/*
			lblImpressum = new UILabel (){
				TextColor = UIColor.White,
				Text ="Impressum",
				Font = UIFont.BoldSystemFontOfSize (17f),
				BackgroundColor = UIColor.Clear,
			//AutoresizingMask = UIViewAutoresizing.FlexibleMargins | UIViewAutoresizing.FlexibleDimensions,
			};
			*/
			txtPlan.RemoveFromSuperview ();
			
			string html = string.Empty;
			try {
				using (TextReader textReader = new StreamReader("Data/kontakt.html")) {
					html = textReader.ReadToEnd ();
				}
			} catch {
			}
			
			wvImpressum = new UIWebView (){
				BackgroundColor = UIColor.Clear,
				Opaque = false,
				DataDetectorTypes = UIDataDetectorType.None,
				UserInteractionEnabled = false,
				AutoresizingMask =  UIViewAutoresizing.FlexibleDimensions,
			};
			float y = btnMail.Frame.Y + btnMail.Frame.Height + 10;
			wvImpressum.Frame = new RectangleF (10, y, this.View.Bounds.Width - 20, 580);
			wvImpressum.LoadHtmlString (html, null);
				
			//SetRest ();
			//this.scrollContainer.Add (lblImpressum);
			this.scrollContainer.Add (wvImpressum);
			scrollContainer.ContentSize = new SizeF (this.View.Bounds.Size.Width, y + wvImpressum.Frame.Height);
		}
		
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			txtDistance.Text = "";
			//Set to invalid coordinate
			_userCoords.Latitude = -999999;
			_userCoords.Longitude = -999999;
			if (CLLocationManager.LocationServicesEnabled) {
				_locationManager.StartUpdatingLocation ();
			}
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			if (CLLocationManager.LocationServicesEnabled) {
				_locationManager.StopUpdatingLocation ();
			}
		}

		/*
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			// Release any retained subviews of the main view.
			// e.g. myOutlet = null;
		}
		*/

		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
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
		
		
		private void HandleMailFinished (object sender, MFComposeResultEventArgs e)
		{
			switch (e.Result) {
			case MFMailComposeResult.Cancelled:
				break;
			case MFMailComposeResult.Saved:
				break;
			case MFMailComposeResult.Sent:
				using (var alert = new UIAlertView ("Email",Locale.GetText("Ihre Nachricht wurde an Löwenbräukeller gesendet."), null,"OK", null)) {
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
		
		private void OnIpConnect (Object sender, EventArgs args)
		{	
			
			var kvc = new KontaktIpcViewController (); //new KonatktIpcViewController ();
			//kvc.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;					
			//this.PresentModalViewController (kvc, true);
			this.NavigationController.PushViewController (kvc, true);
		
		}
		
		/*
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillAnimateRotation (toInterfaceOrientation, duration);
			if (toInterfaceOrientation == UIInterfaceOrientation.Portrait || toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown) {
				txtPlan.Frame = new RectangleF (txtPlan.Frame.X, txtPlan.Frame.Y, txtPlan.Frame.Width, 218);
			} else {
				txtPlan.Frame = new RectangleF (txtPlan.Frame.X, txtPlan.Frame.Y, txtPlan.Frame.Width, 130);
			}
			SetRest ();
		}
		*/
		
		/*
		private void SetRest ()
		{
			float y = txtPlan.Frame.Y + txtPlan.Frame.Height + 5;
			lblImpressum.Frame = new RectangleF (10, y, this.View.Bounds.Width, 20);
			
			y += 20;
			wvImpressum.Frame = new RectangleF (10, y, 200, 100);
		}
		*/
		
	}
}

