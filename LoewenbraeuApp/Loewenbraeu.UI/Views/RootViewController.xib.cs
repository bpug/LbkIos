using System;
using Loewenbraeu.Core;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;
//using MonoTouch.TestFlight;

namespace Loewenbraeu.UI
{
	public partial class RootViewController : BaseViewController//UIViewController
	{
		//private BilderViewController _bilderViewController;
		private DishOfDayViewController _tageskarteViewController ;
		private EventsViewController _eventsViewController;
		private KontaktViewController _kontaktViewController;
		private RaumViewController _raeumeViewController;
		private VideoViewController _videosViewController;
		private QuizStartViewController _quizViewController;
		private HistorieViewController _historieViewController;
		private FacebookViewController _facebookViewController;
		private ReservierungController _reservierungController;
		private GalleryController _galeryController;
		private DocumentViewController _triumphatorController;
		private DirektKontakController _direktKontakController;
		private SpeisekarteViewController _speisekarteViewController;
		
		public RootViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}
		
		public RootViewController () : base ("RootViewController", null)
		{
			Initialize ();
		}

		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return toInterfaceOrientation == UIInterfaceOrientation.Portrait;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}
		
		private void Initialize(){
			Title = Locale.GetText("Löwenbräukeller");
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.NavigationItem.BackBarButtonItem = new UIBarButtonItem ("zurück", UIBarButtonItemStyle.Plain, null);
			
			/*
			UIButton btnFeedBack = UIButton.FromType (UIButtonType.InfoLight);
			btnFeedBack.Title (UIControlState.Normal);
			btnFeedBack.TouchDown += delegate {
				//TestFlight.OpenFeedbackView ();
			};
			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem (btnFeedBack);
			*/
			
			//Show an edit button
			//NavigationItem.RightBarButtonItem = EditButtonItem;
			
			this.btnTageskarte.TouchUpInside += (sender, e) => {
				
				if (_tageskarteViewController == null) {
					_tageskarteViewController = new DishOfDayViewController (true);
				}
				this.NavigationController.PushViewController (_tageskarteViewController, true);
				//_tageskarteViewController.Load ();
			};
			
			
			this.btnEvents.TouchUpInside += (sender, e) => {
			
				if (_eventsViewController == null) {
					_eventsViewController = new EventsViewController (true);
				}
				this.NavigationController.PushViewController (_eventsViewController, true);
				//_eventsViewController.Load ();
			};
			
			this.btnVideos.TouchUpInside += (sender, e) => {
				
				if (_videosViewController == null) {
					_videosViewController = new VideoViewController (true);
				}
				this.NavigationController.PushViewController (_videosViewController, true);
				//_videosViewController.Load ();
				
				
			};
			
			this.btnRaueme.TouchUpInside += (sender, e) => {
			
				if (_raeumeViewController == null) {
					_raeumeViewController = new RaumViewController (true);
				}
				this.NavigationController.PushViewController (_raeumeViewController, true);
			};
			
			this.btnQuiz.TouchUpInside += (sender, e) => {
			
				if (_quizViewController == null) {
					_quizViewController = new QuizStartViewController ();
				}
				this.NavigationController.PushViewController (_quizViewController, true);
			};
			this.btnHistorie.TouchUpInside += (sender, e) => {
			
				if (_historieViewController == null) {
					_historieViewController = new HistorieViewController ();
				}
				this.NavigationController.PushViewController (_historieViewController, true);
			};
			
			this.btnFacebook.TouchUpInside += (sender, e) => {
			
				if (_facebookViewController == null) {
					_facebookViewController = new FacebookViewController ();
				}
				this.NavigationController.PushViewController (_facebookViewController, true);
			};
			
			this.btnKontakt.TouchUpInside += (sender, e) => {
			
				if (_kontaktViewController == null) {
					_kontaktViewController = new KontaktViewController ();
				}
				this.NavigationController.PushViewController (_kontaktViewController, true);
			};
			
			this.btnReservierung.TouchUpInside += (sender, e) => {
			
				if (_reservierungController == null) {
					_reservierungController = new ReservierungController (this);
				}
				_reservierungController.Display ();
				//this.NavigationController.PushViewController (_reservierungViewController, true);
			};
			
			this.btnBilder.TouchUpInside += (sender, e) => {
				
				if (_galeryController == null) {
					_galeryController = new GalleryController (this);
				}
				this.NavigationController.PushViewController (_galeryController, true);
				//_galeryController.Display ();
			};
/*			
			this.btnSpeisekarte.TouchUpInside += (sender, e) => {
			
				//string docPath ="http://lbkmobile.loewenbraeukeller.com/media/docs/Triumphator.pdf";
				if (_triumphatorController == null) {
					
					_triumphatorController = new DocumentViewController (new DocPreviewItem ("Triumphator", "Docs/Triumphator120725.pdf"));
					//_triumphatorController = new DocumentViewController (new DocPreviewItem ("Triumphator", new NSUrl (docPath)));
					             
				}
				this.NavigationController.PushViewController (_triumphatorController, true);
			};
*/
			this.btnSpeisekarte.TouchUpInside += (sender, e) => {
			
				if (_speisekarteViewController == null) {
					_speisekarteViewController = new SpeisekarteViewController (this);
				}
				_speisekarteViewController.Display ();

			};


			
			
			this.btnDirectContact.TouchUpInside += (sender, e) => {
				/*
				if (_direktKontakController == null) {
					_direktKontakController = new DirektKontakController (this.NavigationController);
				}
				_direktKontakController.Display ();
				*/
				string body = @"Hallo,

					also die kostenlose App vom Münchner Löwenbräukeller solltest Du Dir unbedingt mal anschauen.

					http://itunes.apple.com/de/app/lowenbraukeller/id523578226

					Da findest Du auch ein witziges Quiz, bei dem Du eine frische Maß Löwenbräu-Bier gewinnen kannst.

					Servus, bis demnächst im Löwenbräukeller";
				
				if (MFMailComposeViewController.CanSendMail) {
					var mail = new MFMailComposeViewController ();
					//mail.SetToRecipients (new string[] { EMAIL});
					mail.SetSubject (Locale.GetText ("Löwenbräukeller-APP Empfehlung"));
					mail.SetMessageBody (body, false);
					mail.Finished += HandleMailFinished;
					this.PresentViewController (mail, true, null);
				} 
			};
		}
		
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			//this.NavigationController.SetNavigationBarHidden (true, animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			//this.NavigationController.SetNavigationBarHidden (false, animated);
		}

		
		/*
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}
		*/
		
		/*
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}
		*/

		/*
		// Override to allow orientations other than the default portrait orientation
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			//return true for supported orientations
			return (InterfaceOrientation == UIInterfaceOrientation.Portrait);
		}
		*/

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		/*
		public override void ViewDidUnload ()
		{
			// Release anything that can be recreated in viewDidLoad or on demand.
			// e.g. this.myOutlet = null;
			
			base.ViewDidUnload ();
		}
		*/
		private void HandleMailFinished (object sender, MFComposeResultEventArgs e)
		{
			switch (e.Result) {
			case MFMailComposeResult.Cancelled:
				break;
			case MFMailComposeResult.Saved:
				break;
			case MFMailComposeResult.Sent:
				using (var alert = new UIAlertView ("Email",Locale.GetText("Die Weiterempfehlung wurde gesendet."), null,"OK", null)) {
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
