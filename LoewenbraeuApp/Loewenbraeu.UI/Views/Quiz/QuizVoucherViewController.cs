using System;
using System.Timers;
using Loewenbraeu.Core;
using Loewenbraeu.Data;
using Loewenbraeu.Data.Service;
using Loewenbraeu.Model;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Loewenbraeu.UI
{
	public class QuizVoucherViewController : LbkDialogViewController
	{
		private ServiceAgent _serviceAgent;
		
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
					ReloadComplete ();
				}
			}
		}
		
		
		public QuizVoucherViewController (bool pushing) : base (null, pushing, null)
		{
			Initialize ();
		}
		
		void Initialize ()
		{
			Title = Locale.GetText ("Gutscheine");
			
			_serviceAgent = new ServiceAgent ();
			_serviceAgent.ServiceClient.ActivateVoucherCompleted += HandleActivateVoucherCompleted;
			/*
			RefreshRequested += delegate {
				Load ();
			};
			*/
			
			Style = UITableViewStyle.Plain;
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			Autorotate = true;
			Load ();
		}
		
		private void Load ()
		{
			var vouchers = QuizVoucherRepository.GetNotUsedVouchers ();
			
			RootElement root = new RootElement (Title);
			
			foreach (var voucher in vouchers) {
				var element = new VoucherElement (voucher);
				element.VoucherActivated += VoucherActivated;
				element.VoucherUsed += VoucherUsed;
				root.Add (new Section { element });
			}
			Root = root;
			ReloadComplete ();
		}

		private void VoucherUsed (object sender, EventArgs<QuizVoucher> e)
		{
			string text = "Haben Sie den Gutschein jetzt eingelöst. Haben Sie ihr Getränk erhalten?";
					var alert = new UIAlertView (Locale.Format (text, e.Value.Code), "", null, "Cancel", "OK");
					alert.Clicked += delegate(object asender, UIButtonEventArgs be) {
						if (be.ButtonIndex == 1) {
							e.Value.IsUsed = true;
							QuizVoucherRepository.Update (e.Value);
							this.NavigationController.PopViewControllerAnimated (true);
						}
					};
					alert.Show ();
		}

		private void VoucherActivated (object sender, EventArgs<QuizVoucher> e)
		{
			string text = "Wollen Sie den Gutschein {0} jetzt aktivieren?";
			var alert = new UIAlertView (Locale.Format (text, e.Value.Code), "", null, "Cancel", "OK");
			alert.Clicked += delegate(object asender, UIButtonEventArgs be) {
				if (be.ButtonIndex == 1) {
					VoucherActivate (e.Value);
				}
			};
			alert.Show ();
		}
		
		
		private void VoucherActivate (QuizVoucher voucher)
		{
			if (Busy)
				return;
			
			if (ServiceAgent.Execute (_serviceAgent.ServiceClient.ActivateVoucherAsync, Util.DeviceUid, voucher.QuizId, voucher.Code, voucher)) {
				Busy = true;
			}
		}
		
		private void HandleActivateVoucherCompleted (object sender, ActivateVoucherCompletedEventArgs args)
		{
			Tuple<Timer, object> userState;
			
			bool error = ServiceAgent.HandleAsynchCompletedError (args, "ActivateVoucher");
			InvokeOnMainThread (delegate	{
				Busy = false;
				if (error)
					return;
					
				QuizVoucher voucher = null;
				bool result = args.Result;
				userState = args.UserState as Tuple<Timer, object>;
				if (userState != null && userState.Item2 != null) {
					voucher = userState.Item2 as QuizVoucher;
				} else {
					using (var alert = new UIAlertView (Locale.GetText ("Unbekannte Fehler"), "", null, "OK", null)) {
						alert.Show ();
					}
					return;
				}
				
				if (result == true) {
					voucher.IsActivated = true;
					QuizVoucherRepository.Update (voucher);
					Load ();
					var text = "Den Gutschein {0} wurde erfolgreich aktiviert.";
					using (var alert = new UIAlertView (Locale.Format (text, voucher.Code), "", null, "OK", null)) {
						alert.Show ();
					}
				} else {
					voucher.IsUsed = true;
					voucher.Deleted = true;
					QuizVoucherRepository.Update (voucher);
					var text = "Sie haben Ihren Gutschein-Code bereits bekommen.";
					using (var alert = new UIAlertView (Locale.GetText (text), "", null, "OK", null)) {
						alert.Show ();
					}
				}
					
			});
			
		}
	}
}

