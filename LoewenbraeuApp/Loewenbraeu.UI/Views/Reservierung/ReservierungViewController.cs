using System;
using System.Drawing;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Data;
using Loewenbraeu.Data.Service;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public partial class ReservierungViewController : BaseHudViewController
	{
		private const int HOUR_LIMIT = 1;
		private const  int MAX_SEATS = 10;
		private Reservation _reservation ;
		private UIDatePicker _reservationDatePicker;
		private UIPickerView _seatsPicker;
		private UIToolbar _inputAccessoryView;
		private UIButton btnReserve;
		
		public event EventHandler<EventArgs<Reservation>> Reserved;
		
		public ReservierungViewController () : base ("ReservierungViewController", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
			Title = Locale.GetText ("Reservierung");
			
			ServiceAgent.Current.ServiceClient.CreateReservationByObjectCompleted += this.HandleReservationCompleted;
			/*
			_hud = new LoadingHUDView (){ 
			//HudBackgroundColor =  Resources.Colors.BackgroundHUD,
				ShowRoundedRectangle = true
			};	
			*/
			_reservationDatePicker = new UIDatePicker ();
			_reservationDatePicker.Locale = NSLocale.CurrentLocale;
			_reservationDatePicker.TimeZone = NSTimeZone.FromAbbreviation ("GMT"); //NSTimeZone.LocalTimeZone;
			_reservationDatePicker.ValueChanged += delegate {
				this.txtDate.Text = _reservationDatePicker.Date.ToDateTime().ToLongDateTimeString();
			};
			
			var seatsModel = new SeatsPickerModel (MAX_SEATS);
			_seatsPicker = new UIPickerView (){
				Model = seatsModel,
				ShowSelectionIndicator = true,
			};
			seatsModel.ValueChanged += delegate(object sender, EventArgs<int> e) {
				txtSeats.Text = e.Value.ToString ();
			};
			_seatsPicker.Frame = new RectangleF (_seatsPicker.Frame.X, _seatsPicker.Frame.Y, _seatsPicker.Frame.Width, 80f);
			
		}
		
		private void OnReserved (Reservation reservation)
		{
			if (this.Reserved != null) {
				Reserved (this, new EventArgs<Reservation> (reservation));
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
			scrollContainer.ContentSize = new SizeF (this.View.Bounds.Size.Width, this.View.Bounds.Size.Height - 44); //new SizeF (320f, 550f);
			
			_reservationDatePicker.MinimumDate = DateTime.Now.AddHours (HOUR_LIMIT).ToNSDate ();
			_reservationDatePicker.Date = DateTime.Now.AddHours (HOUR_LIMIT).ToNSDate ();
			_reservationDatePicker.Mode = UIDatePickerMode.DateAndTime;
			_reservationDatePicker.MinuteInterval = 30;
			
			
			lblDescription.TextColor = UIColor.White;
			lblName.TextColor = UIColor.White;
			lblSeats.TextColor = UIColor.White;
			lblDate.TextColor = UIColor.White;
			lblNotice.TextColor = UIColor.White;
			lblPhone.TextColor = UIColor.White;
			
			txtNotice.Layer.CornerRadius = 8;
			txtDate.InputView = this._reservationDatePicker;
			txtSeats.InputView = this._seatsPicker;
			//txtDate.InputAccessoryView = GetAccessoryView ();
			
			txtPhone.KeyboardType = UIKeyboardType.PhonePad;
			txtPhone.ShouldReturn += (textField) => {
				textField.ResignFirstResponder ();
				return true;
			};
			txtName.ShouldReturn += (textField) => {
				textField.ResignFirstResponder ();
				return true;
			};
			txtSeats.ShouldReturn += (textField) => {
				textField.ResignFirstResponder ();
				return true;
			};
			
			txtDate.EditingDidEnd += (textField, e) => {
				txtDate.ResignFirstResponder ();
			};
			
			txtDate.Ended += OnEditEnded;
			txtName.Ended += OnEditEnded;
			txtSeats.Ended += OnEditEnded;
			txtPhone.Ended += OnEditEnded;
			
			btnReserve = new LBKButton (new RectangleF (15, 354, 290, 37)) {
				Font = UIFont.BoldSystemFontOfSize (18),
			};
			btnReserve.SetTitle (Locale.GetText ("Anfragen"), UIControlState.Normal);
			btnReserve.TouchUpInside += OnReserve;
			scrollContainer.AddSubview (btnReserve);
			
			lblHeaderText.Text = Locale.GetText ("Ich möchte gerne verbindlich im Restaurant Bräustüberl reservieren.");
			
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			txtDate.Text = "";
			txtName.Text = "";
			txtSeats.Text = "2";
			txtPhone.Text = "";
			txtNotice.Text = "";
			btnReserve.Enabled = false;
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			ResignResponder ();
		}

		/*
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
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
			return (toInterfaceOrientation == UIInterfaceOrientation.Portrait || 
			        toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown);
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown;
		}
		
		public override UIView InputAccessoryView {
			get {
				
				return GetAccessoryView ();
			}
		}
		
		private UIView GetAccessoryView ()
		{
			if (_inputAccessoryView == null) {
				_inputAccessoryView = new UIToolbar (new RectangleF (0, 0, 320, 30));
				_inputAccessoryView.Items = new[]{
					new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
					new UIBarButtonItem (UIBarButtonSystemItem.Done, OnDoneButton)
				};
			}
			return _inputAccessoryView;
		}
		
		private void OnDoneButton (object sender, EventArgs evt)
		{
			ResignResponder ();
		}
		
		private void OnEditEnded (object sender, EventArgs e)
		{
			this.btnReserve.Enabled = false;
			if (!txtName.Text.IsEmpty () && !txtDate.Text.IsEmpty () &&
			    !txtSeats.Text.IsEmpty () && !txtPhone.Text.IsEmpty ()) {
				
				this.btnReserve.Enabled = true;
			}
		}
		
		private bool Validate ()
		{
			string failText = string.Empty;
			
			if (txtName.Text.IsEmpty ()) {
				failText = Locale.GetText ("Name ???");
			} else if (txtDate.Text.IsEmpty ()) {
				failText = Locale.GetText ("Date ???");
			} else if (txtSeats.Text.IsEmpty ()) {
				failText = Locale.GetText ("Seats ???");
			} else if (txtPhone.Text.IsEmpty ()) {
				failText = Locale.GetText ("Telefon ???");
			}
			
			if (!string.IsNullOrEmpty (failText)) {
				using (var alert =  new UIAlertView ("", failText,null,"OK",null)) {
					alert.Show ();
				}
				return false;
			}
			return true;
		}
		
		private void OnReserve (object sender, EventArgs e)
		{
			ResignResponder ();
			if (Validate ()) {
				_reservation = new Reservation ();
				_reservation.ReservationTime = _reservationDatePicker.Date.ToDateTime ();
				_reservation.GuestName = txtName.Text;
				_reservation.Mobile = txtPhone.Text;
				_reservation.Seats = int.Parse (txtSeats.Text);
				_reservation.Fingerprint = Util.DeviceUid;
				_reservation.ConfirmCode = Util.RandomString (4);
				_reservation.Advice = txtNotice.Text;
			
				//ServiceAgent.Reserve (_reservation);
				if (ServiceAgent.Execute(ServiceAgent.Current.ServiceClient.CreateReservationByObjectAsync, _reservation)) {
					//_hud.StartAnimating ();
					ShowHud ();
				}
			}
		}
		
		private void HandleReservationCompleted (object sender, CreateReservationByObjectCompletedEventArgs args)
		{
			bool error = ServiceAgent.HandleAsynchCompletedError (args, "GetEvents");
			
			InvokeOnMainThread (delegate	{
				HideHud ();
				if (error)
					return;
				
				string result = args.Result;
				_reservation.ReservationId = result;
				_reservation.Status = StatusArt.Requested;
				ReservationRepository.Update (_reservation);
				this.OnReserved (_reservation);
			});
		}
		
		/*
		public override UIView InputAccessoryView {
			get {
				UIView activeView = this.KeyboardGetActiveView ();
				UITextField textfied = activeView as UITextField;
				
				if (textfied == null || textfied.KeyboardType != UIKeyboardType.PhonePad) {
					return null;
				}
				if (_inputAccessoryView == null) {
					_inputAccessoryView = GetAccessoryView ();
				}
				return _inputAccessoryView;
			}
		}
		
		private UIView GetAccessoryView ()
		{
			UIView accessoryView = new UIView (new RectangleF (0, 0, 320, 27));
			UIView activeView = this.KeyboardGetActiveView ();
			
			accessoryView.BackgroundColor = UIColor.ViewFlipsideBackgroundColor; //UIColor.FromPatternImage (new UIImage ("Images/accessoryBG.png"));          
			UIButton dismissBtn = UIButton.FromType (UIButtonType.RoundedRect);
			dismissBtn.Frame = new RectangleF (255, 2, 58, 23);
			dismissBtn.SetTitle ("Fertig", UIControlState.Normal);
			dismissBtn.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			//dismissBtn.SetBackgroundImage (new UIImage ("Images/dismissKeyboard.png"), UIControlState.Normal);        
			dismissBtn.TouchDown += delegate {
				if (activeView != null)
					activeView.ResignFirstResponder ();
			};
			accessoryView.AddSubview (dismissBtn);
			return accessoryView;
		}
		*/
		
		
	}
}

