using System.Drawing;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Data.Service;
using MonoTouch.Dialog;
using MonoTouch.EventKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;


namespace Loewenbraeu.UI
{
	public partial class ConfirmReservationController : BaseViewController
	{
		private const float TEXT_SIZE = 16;
		const float TEXT_YOFFSET = 10;
		const float TEXT_XOFFSET = 10;
		const float BUTTON_WIDTH = 200;
		const float BUTTON_HEIGTH = 40;
		
		static UIFont textFont = UIFont.SystemFontOfSize (TEXT_SIZE);
		
		private Reservation _reservation;
		private UIButton _btnAddCalendarEvent;
		private UILabel _lblAddCalendarEvent;
		private UILabel _lblConfirm;
		
		public Reservation Reservation {
			get { return _reservation;}
			set { _reservation = value;}
		}
		
		public ConfirmReservationController (Reservation reservation) : base ()
		{
			_reservation = reservation;
			Initialize ();
		}
		
		private void Initialize ()
		{
			Title = Locale.GetText ("Bestätigung");
			
			_lblConfirm = new UILabel (){
				TextColor = UIColor.White,
				Font = textFont,
				Lines =0,
				AutoresizingMask = UIViewAutoresizing.All
			};
			
			_lblAddCalendarEvent = new UILabel (){
				TextColor = UIColor.White,
				Font = textFont,
				Lines =0,
				AutoresizingMask = UIViewAutoresizing.All
			};
			
			_btnAddCalendarEvent = new LBKButton (){
		          Font = UIFont.BoldSystemFontOfSize (17),
				  AutoresizingMask = UIViewAutoresizing.FlexibleMargins,
		     };
			_btnAddCalendarEvent.SetTitle (Locale.GetText ("Eintragen"), UIControlState.Normal);
			_btnAddCalendarEvent.TouchUpInside += AddCalendarEvent;
		}

		
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			_lblConfirm.Frame = new RectangleF (TEXT_XOFFSET, TEXT_YOFFSET, this.View.Bounds.Width - 2 * TEXT_XOFFSET, 26);
			
			this.View.AddSubview (_lblConfirm);
			this.View.AddSubview (_lblAddCalendarEvent);
			this.View.SetLabelsBGColor (UIColor.Clear);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			float y = 0;
			float textHeight = 0;
			float textWidth = View.Bounds.Width - 2 * TEXT_XOFFSET;
			
			string confirmText = string.Empty;
			
			switch (_reservation.Status) {
			case StatusArt.ConfirmedByCustomer:
				confirmText = Locale.GetText ("Die Plätze sind reserviert.\nSie bekommen noch eine SMS mit den Reservierungsdaten.");
				break;
			case StatusArt.DeclinedByRestaurant:
				confirmText = Locale.GetText ("Liber Gast, leider können wir Ihnen Reservierungswunsch nicht bestätigen.\nBitte rufen Sie uns unter:\n089 54726690 an.\nIhr Löwenbräukeler Team.");
				break;
			case StatusArt.AbortedByCustomer:
				confirmText = Locale.GetText ("Die Reservierung wurde abgebrochen.");
				break;
			//default:
			}
			_lblConfirm.Text = confirmText;
			_lblConfirm.SetLabelHeight4Text ();
			
			_lblAddCalendarEvent.Text = "";
			//Add Calendar event
			if (_reservation.Status == StatusArt.ConfirmedByCustomer) {
				y += _lblConfirm.Bounds.Size.Height + 2 * TEXT_YOFFSET;
				
				_lblAddCalendarEvent.Text = Locale.GetText ("Wollen Sie die Resiervierung in Ihren Kalender eintragen?");
				textHeight = _lblAddCalendarEvent.GetHeight4Text (textWidth);
				_lblAddCalendarEvent.Frame = new RectangleF (TEXT_XOFFSET, y, textWidth, textHeight);
				
				y += textHeight + TEXT_YOFFSET;
				_btnAddCalendarEvent.Frame = new RectangleF ((View.Bounds.Width - BUTTON_WIDTH) / 2, y, BUTTON_WIDTH, BUTTON_HEIGTH);
				this.View.AddSubview (_btnAddCalendarEvent);
			}
		}
		

		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		private void AddCalendarEvent (object sender, System.EventArgs e)
		{
			EKEventStore store = new EKEventStore ();
			EKCalendar calendar = store.DefaultCalendarForNewEvents;
			NSError error = null;
			
			if (calendar != null) {
				EKEvent newEvent = EKEvent.FromStore (store);
				newEvent.Title = "Löwenbräukeller";
				newEvent.Calendar = calendar;
				newEvent.StartDate = _reservation.ReservationTime;
				newEvent.EndDate = _reservation.ReservationTime.AddMinutes (30);
				newEvent.Availability = EKEventAvailability.Free;
				newEvent.Notes = _reservation.Advice;
				newEvent.Location = "Löwenbräukeller Gastronomie GmbH\n" +
							"Nymphenburgerstrasse 2\n" + 
							"80335 München";
				
				store.SaveEvent (newEvent, EKSpan.ThisEvent, out error);
				
				if (error == null) {
					var alert = new UIAlertView ( Locale.GetText ("Ihre Reservierung wurde in Ihrem Kalender eingetragen."), "", null, "OK", null);
					alert.Clicked += delegate {
						_lblAddCalendarEvent.Text = "";
						_btnAddCalendarEvent.RemoveFromSuperview ();
					};
					alert.Show ();
				} else {
					using (var alert =  new UIAlertView (Locale.GetText("Error"),error.ToString(),null, "OK", null)) {
						alert.Show ();
					}
				}
			}
		}
	}
}

