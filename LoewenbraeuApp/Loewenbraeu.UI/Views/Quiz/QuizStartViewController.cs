using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Data;
using Loewenbraeu.Data.Mappings;
using Loewenbraeu.Data.Service;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Model = Loewenbraeu.Model;

namespace Loewenbraeu.UI
{
	public partial class QuizStartViewController : BaseViewController
	{
		private const int QUESTION_COUNT = 10;
		UIButton btnStart;
		UIButton btnVoucher;
		UIButton btnHelp;
		//UILabel lblDescr;
		//QuizHelpViewController _qhvc;
		QuizViewController _qvc;
		QuizVoucherViewController _qvvc;
		bool _busy = false;
		
		bool Busy {
			get {
				return _busy;
			}
			set {
				_busy = value;
				//this.NavigationItem.RightBarButtonItem.Enabled = !_busy;
				if (_busy == true) {
					ShowHud ();
				} else {
					HideHud ();
				}
			}
		}
		
		public QuizStartViewController () : base ( UIImage.FromBundle ("image/background/quiz_start.png"))
		{
			Initialize ();
		}
		
		private void Initialize ()
		{
			Title = Locale.GetText ("Bavaria Quiz Gaudi");
			
			ServiceAgent.Current.ServiceClient.GetQuizCompleted += HandleQuizCompleted;
			/*	
			lblDescr = new UILabel (){
				Lines = 0,
				TextColor = Resources.Colors.Background,
				BackgroundColor = UIColor.Clear,
				Font = UIFont.SystemFontOfSize (15),
				Text = Locale.GetText ("Gewinnen Sie eine Löwenbräu Maß Bier mit unserer Bavaria Quiz Gaudi!"),
			};
			
			float descrX = 30;
			float descrWidth = 340 - (descrX * 2);
			float testHeight = lblDescr.GetHeight4Text (descrWidth);
			lblDescr.Frame = new RectangleF (descrX, 100, descrWidth, testHeight);
			*/
			
			btnStart = new LBKButton ();
			btnStart.Frame = new RectangleF (70, 210, 190, 37);
			btnStart.Font = UIFont.BoldSystemFontOfSize (15);
			btnStart.SetTitle (Locale.GetText ("Neues Spiel"), UIControlState.Normal);
			//btnStart.TouchUpInside += LoadQuitz; 
			
			btnStart.TouchUpInside += HandleStart; 
			
			btnHelp = new LBKButton (new RectangleF (70, 312, 190, 37)){
		    	Font = UIFont.BoldSystemFontOfSize (15),
			//AutoresizingMask = UIViewAutoresizing.FlexibleMargins,
		     };
			btnHelp.SetTitle (Locale.GetText ("Spielanleitung"), UIControlState.Normal);
			btnHelp.TouchUpInside += delegate {
				var qhvc = new QuizHelpViewController ();
				qhvc.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;					
				this.PresentViewController (qhvc, true, null);
			};
			
			btnVoucher = new LBKButton (new RectangleF (70, 362, 190, 37)){
		          Font = UIFont.BoldSystemFontOfSize (15),
			//AutoresizingMask = UIViewAutoresizing.FlexibleMargins,
		    };
			
			btnVoucher.SetTitle (Locale.GetText ("Gutschein"), UIControlState.Normal);
			//btnVoucher.SetTitle ("Kein Gutschein", UIControlState.Disabled);
			this.btnVoucher.TouchUpInside += delegate {
				_qvvc = new QuizVoucherViewController (true);
				this.NavigationController.PushViewController (_qvvc, true);
			};
			
		}

		void HandleStart (object button, EventArgs ea)
		{
			
			if (!UserDefaults.Jugendschutz) {
				var alert = new UIAlertView (Locale.GetText ("Hiermit bestätige ich, dass ich mindestens 18 Jahre alt bin."), "",
				                             null, Locale.GetText ("Ja"), Locale.GetText ("Nein"));
				alert.Clicked += delegate(object sender, UIButtonEventArgs e) {
					if (e.ButtonIndex == 0) {
						UserDefaults.Jugendschutz = true;
						LoadQuitz ();
					}
				};
				alert.Show ();
			} else {
				LoadQuitz ();
			}
			
		}

		void LoadQuitz ()
		{
			if (Busy)
				return;
			if (ServiceAgent.Execute (ServiceAgent.Current.ServiceClient.GetQuizAsync, Util.DeviceUid, QUESTION_COUNT)) {
				Busy = true;
			}
		}

		void HandleQuizCompleted (object sender, GetQuizCompletedEventArgs args)
		{
			bool error = ServiceAgent.HandleAsynchCompletedError (args, "GetQuiz");
			using (var pool = new NSAutoreleasePool()) {
			
				pool.InvokeOnMainThread (delegate	{
					Busy = false;
					if (error)
						return;
					
					Model.Quiz result = args.Result.ToLbkQuiz ();
					//result.Id = 14;
					_qvc = new QuizViewController (result);
					this.NavigationController.PushViewController (_qvc, true);
				});
			}
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
			/*
			UIButton btnHelp = UIButton.FromType (UIButtonType.InfoLight);
			btnHelp.Title (UIControlState.Normal);
			btnHelp.TouchDown += OnHelp;
			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem (btnHelp);
			*/
			//View.Add (lblDescr);
			View.Add (btnStart);
			View.Add (btnHelp);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			var vouchers = QuizVoucherRepository.GetNotUsedVouchers ();
			if (vouchers.Count () > 0) {
				//this.btnVoucher.Enabled = true;
				View.Add (btnVoucher);
			} else {
				//this.btnVoucher.Enabled = false;
				btnVoucher.RemoveFromSuperview ();
			}
			
		}

		/*
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			//_qhvc = null;
			_qvc = null;
		}
		*/

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				_qvc = null;
			}
			base.Dispose (disposing);
		}

		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown || toInterfaceOrientation == UIInterfaceOrientation.Portrait);
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown;
		}
		
		/*
		public void OnHelp (Object sender, EventArgs args)
		{	
			if (_qhvc == null)
				_qhvc = new QuizHelpViewController ();
			_qhvc.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;					
			this.PresentModalViewController (_qhvc, true);
		
		}
		*/
	}
}

