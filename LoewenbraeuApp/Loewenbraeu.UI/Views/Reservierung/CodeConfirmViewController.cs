using System;
using System.Drawing;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Data;
using Loewenbraeu.Data.Service;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public partial class CodeConfirmViewController : BaseHudViewController
	{
		private Reservation _reservation;
		//private LoadingHUDView _hud;
		
		private UIButton btnAbort;
		private UIButton btnConfirm;
		
		public event EventHandler<EventArgs<Reservation>> ReservationConfirmed;
		public event EventHandler<EventArgs<Reservation>> ReservationAborted;
		
		public Reservation Reservation {
			get { return _reservation;}
			set { _reservation = value;}
		}
		
		public CodeConfirmViewController () : base ("CodeConfirmViewController", null)
		{
			Initialize ();
		}
		
		public CodeConfirmViewController (Reservation reservation) : this ()
		{
			_reservation = reservation;
		}
		
		private void Initialize ()
		{
			Title = Locale.GetText ("Bestätigung Code");
			/*
			_hud = new LoadingHUDView (){ 
				HudBackgroundColor = Resources.Colors.BackgroundHUD,
				ShowRoundedRectangle = true
			};
			*/
			ServiceAgent.Current.ServiceClient.SetReservationConfirmCustomerCompleted += this.HandleCodeConfirmCompleted;
			ServiceAgent.Current.ServiceClient.SetDecliningCompleted += HandleAbortReservationCompleted;
		}
		
		private void OnReservationConfirmed (Reservation reservation)
		{
			
			if (this.ReservationConfirmed != null) {
				ReservationConfirmed (this, new EventArgs<Reservation> (reservation));
			}
		}
		
		private void OnReservationAborteed (Reservation reservation)
		{
			
			if (this.ReservationAborted != null) {
				ReservationAborted (this, new EventArgs<Reservation> (reservation));
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
			
			//this.View.AddSubview (_hud);
			
			this.View.BringSubviewToFront (this.scrollContainer);
			scrollContainer.ContentSize = new SizeF (this.View.Bounds.Size.Width, this.View.Bounds.Size.Height - 44);
			
			SetLabelsTextColor (this.scrollContainer, UIColor.White);
			
			//ToUpper
			txtConfirmCode.AutocapitalizationType = UITextAutocapitalizationType.AllCharacters;
			
			txtConfirmCode.ReturnKeyType = UIReturnKeyType.Done;
			txtConfirmCode.EditingChanged += delegate {
				SetConfirmBtnEnabled ();
			};
			txtConfirmCode.ShouldReturn += (textField) => {
				textField.ResignFirstResponder ();
				return true;
			};
			
			btnConfirm = new LBKButton (new RectangleF (34, 369, 104, 37)) {
				Font = UIFont.BoldSystemFontOfSize (15),
			};
			btnConfirm.SetTitle (Locale.GetText ("Bestätigen"), UIControlState.Normal);
			btnConfirm.TouchUpInside += OnConfirm;
			scrollContainer.AddSubview (btnConfirm);
			
			btnAbort = new LBKButton (new RectangleF (190, 369, 104, 37)) {
				Font = UIFont.BoldSystemFontOfSize (15),
			};
			btnAbort.SetTitle (Locale.GetText ("Abbrechen"), UIControlState.Normal);
			btnAbort.TouchUpInside += OnAbort;
			scrollContainer.AddSubview (btnAbort);
			
			
			lblName.Text = Locale.GetText ("Name");
			lblDate.Text = Locale.GetText ("Zeit");
			lblNotice.Text = Locale.GetText ("Notiz");
			lblPhone.Text = Locale.GetText ("Telefon");
			lblSeats.Text = Locale.GetText ("Plätze");
			
			
			lblText1.Text = Locale.GetText ("Lieber Gast,\nvielen Dank für Ihre Anfrage:");
			lblText2.Text = Locale.GetText ("In Kürze erhalten Sie einen Löwenbräukeller-Code per SMS, den Sie bitte hier eintragen:");
			lblText3.Text = Locale.GetText ("Ihre Reservierung ist dann verbindlich.");
			
			lblText1.SetLabelHeight4Text ();
			lblText2.SetLabelHeight4Text ();
			lblText3.SetLabelHeight4Text ();
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			this.txtName.Text =  this._reservation.GuestName;
			this.txtDate.Text = this._reservation.ReservationTime.ToLongDateTimeString ();
			this.txtSeats.Text = this._reservation.Seats.ToString ();
			this.txtNotice.Text = this._reservation.Advice;
			this.txtPhone.Text = this._reservation.Mobile;
			this.txtConfirmCode.Text = "";
			
			//txtNotice.SetLabelHeight4Text();
			SetConfirmBtnEnabled ();
		}
		
		private void  SetConfirmBtnEnabled ()
		{
			btnConfirm.Enabled = false;
			if (txtConfirmCode.Text.Length > 3) {
				btnConfirm.Enabled = true;
			}
		}

		/*
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
			//_hud = null;
		}
		*/

		protected override void Dispose (bool disposing)
		{
			if (disposing){
				ReleaseDesignerOutlets ();
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
		
		
		void OnConfirm (object sender, EventArgs e)
		{
			if (txtConfirmCode.Text.Equals (_reservation.ConfirmCode)) {
				//ServiceAgent.ConfirmReservation (_reservation.ReservationId);
				if (ServiceAgent.Execute (ServiceAgent.Current.ServiceClient.SetReservationConfirmCustomerAsync, _reservation.ReservationId)) {
					//_hud.StartAnimating ();
					ShowHud ();
				}
			} else {
				using (UIAlertView alert = new UIAlertView("",Locale.GetText("Code ist falsch."),null,"OK",null)) {
					alert.Show ();
				}
			}
			
		}

		void OnAbort (object sender, EventArgs e)
		{
			var alert = new UIAlertView (Locale.GetText ("Wollen Sie die Reservierung abrechen?"), "", null, Locale.GetText ("Cancel"), "OK");
			alert.Clicked += delegate(object button, UIButtonEventArgs be) {
				if (be.ButtonIndex == 1) {
					if (ServiceAgent.Execute (ServiceAgent.Current.ServiceClient.SetDecliningAsync ,_reservation.ReservationId)) {
						ShowHud ();
					}
				}
			};
			alert.Show ();
		}
		
		private void HandleCodeConfirmCompleted (object sender, SetReservationConfirmCustomerCompletedEventArgs args)
		{
			bool error = ServiceAgent.HandleAsynchCompletedError (args, "GetEvents");
			InvokeOnMainThread (delegate	{
				HideHud ();
				if (error)
					return;
				
				if (args.Result == true) {
					_reservation.Status = StatusArt.ConfirmedByCustomer;
					ReservationRepository.Update (_reservation);
					this.OnReservationConfirmed (_reservation);
				} else {
					using (UIAlertView alert = new UIAlertView("",Locale.GetText("Die Reservierung wurde nicht bestätigt."),null,"OK",null)) {
						alert.Show ();
					}
				}
			});
		}
		
		void HandleAbortReservationCompleted (object sender, SetDecliningCompletedEventArgs args)
		{
			bool error = ServiceAgent.HandleAsynchCompletedError (args, "GetEvents");
			
			InvokeOnMainThread (delegate	{
				HideHud ();
				if (error)
					return;
				
				if (args.Result == true) {
					_reservation.Status = StatusArt.AbortedByCustomer;
					ReservationRepository.Update (_reservation);
					this.OnReservationAborteed (_reservation);
				} else {
					using (UIAlertView alert = new UIAlertView("",Locale.GetText("Der Abbruchwunsch konnte nicht bestätigt werden."),null,"OK",null)) {
						alert.Show ();
					}
				}
			});
			
		}
	}
}

