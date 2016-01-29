using System;
using Loewenbraeu.Core;
using Loewenbraeu.Data;
using Loewenbraeu.Data.Service;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public class ReservierungController
	{
		private ReservierungViewController _reservierungViewController;
		private CodeConfirmViewController _codeConfirmViewController;
		private ConfirmReservationController _confirmReservationController;
		private UIViewController _rootViewConroller;
		private Reservation _reservation;
		
		public ReservierungController (UIViewController rootViewConroller)
		{
			_rootViewConroller = rootViewConroller;
			ServiceAgent.Current.ServiceClient.IsDeclinedByRestaurantCompleted += HandleIsDeclinedByRestaurantCompleted;
		}
		
		public void Display ()
		{
			_reservation = ReservationRepository.GetRequestedReservation ();
			
			//CreateNotification (_reservation);
			if (_reservation != null) {
				/* 
				//Test
				_reservation.Status = StatusArt.ConfirmedByCustomer;
				ShowConfirmReservationView (_reservation);
				return;
				*/
				if (_reservation.ReservationTime < DateTime.Now) {
					_reservation.Status = StatusArt.AbortedByCustomer; // Abgelaufen
					ReservationRepository.Update (_reservation);
					ShowReservationView ();
					return;
				}
				CheckDeclinedByRestaurant ();
				
			} else {
				ShowReservationView ();
			}
			
		}
		
		private void CheckDeclinedByRestaurant(){
			//Util.PushNetworkActive ();
			//ServiceAgent.IsDeclinedByRestaurantAsync (_reservation.ReservationId);
			if (ServiceAgent.Execute(ServiceAgent.Current.ServiceClient.IsDeclinedByRestaurantAsync ,_reservation.ReservationId)) {
				Util.PushNetworkActive ();
			}
		}
		
		private void HandleIsDeclinedByRestaurantCompleted (object sender, IsDeclinedByRestaurantCompletedEventArgs args)
		{
			bool error = ServiceAgent.HandleAsynchCompletedError (args, "GetEvents");
			
			using (var pool = new NSAutoreleasePool()) {
			
				pool.InvokeOnMainThread (delegate	{
					Util.PopNetworkActive ();
					if (error)
						return;
					
					if (args.Result == true) {
						_reservation.Status = StatusArt.DeclinedByRestaurant;
						ReservationRepository.Update (_reservation);
						ShowConfirmReservationView (_reservation);
						//StopTimer ();
					} else {
						ShowCodeConfirmView (_reservation);
					}
				});
			}
		}
		
		private void ShowReservationView ()
		{
			if (_reservierungViewController == null) {
				_reservierungViewController = new ReservierungViewController ();
				_reservierungViewController.Reserved += delegate(object sender, EventArgs<Reservation> e) {
					//StartTimer (e.Reservation);
					//CreateNotification (e.Reservation);
					ShowCodeConfirmView (e.Value);
				};
			}
			//_rootViewConroller.NavigationController.PushViewController (_reservierungViewController, true);
			this.PushViewController (_reservierungViewController);
		}
		
		private void ShowCodeConfirmView (Reservation reservation)
		{
			if (_codeConfirmViewController == null) {
				_codeConfirmViewController = new CodeConfirmViewController (reservation);
				_codeConfirmViewController.ReservationConfirmed += delegate(object sender, EventArgs<Reservation> e) {
					ShowConfirmReservationView (e.Value);
				};
				_codeConfirmViewController.ReservationAborted += delegate(object sender, EventArgs<Reservation> e) {
					ShowConfirmReservationView (e.Value);
				};
			} else {
				_codeConfirmViewController.Reservation = reservation;
			}
			
			this.PushViewController (_codeConfirmViewController);
		}
		
		private void ShowConfirmReservationView (Reservation reservation)
		{
			if (_confirmReservationController == null) {
				_confirmReservationController = new ConfirmReservationController (reservation);
			} else {
				_confirmReservationController.Reservation = reservation;
			}
			this.PushViewController (_confirmReservationController);
		}
		
		private void PushViewController (UIViewController controller)
		{
			_rootViewConroller.NavigationController.PopToRootViewController (false);
			_rootViewConroller.NavigationController.PushViewController (controller, true);
		}
		
		private NSTimer _timer;
		
		private void StartTimer (Reservation reservation)
		{
			_timer = NSTimer.CreateRepeatingScheduledTimer (TimeSpan.FromSeconds (10), 
			        delegate {
						Util.PushNetworkActive ();
						//ServiceAgent.IsDeclinedByRestaurantAsync (reservation.ReservationId);
						if (ServiceAgent.Execute (ServiceAgent.Current.ServiceClient.IsDeclinedByRestaurantAsync, _reservation.ReservationId)) {
							Util.PushNetworkActive ();
						}
				
					}
			);
		}

		private void StopTimer ()
		{
			_timer.Invalidate ();
			_timer = null;
		}
		
		/*
		void CreateNotification (Reservation reservation)
		{   
			Util.CancelAllLocalNotifications ();
			
			DateTime fireDate = DateTime.UtcNow.Add (TimeSpan.FromSeconds (30));
			UILocalNotification alert = new UILocalNotification ();
			alert.TimeZone = NSTimeZone.LocalTimeZone;
			alert.FireDate = DateTime.Now.AddSeconds (30); //;fireDate.ToNSDate ();
			alert.RepeatInterval = NSCalendarUnit.Minute;
			alert.ApplicationIconBadgeNumber = 6;
			//alert.RepeatCalendar = 
			alert.AlertBody = string.Format ("This notification is scheduled for {0}", fireDate.ToString ());
			alert.AlertAction = "Click me"; // This will create a button which will launch your application
			
			
			object[] keys = new object[] { "reservation" };
			object[] objects = new object[] { reservation.ReservationId };
			
			var userInfo = NSDictionary.FromObjectsAndKeys (objects, keys);
			alert.UserInfo = userInfo;
			
			UIApplication.SharedApplication.ScheduleLocalNotification (alert);
			//UIApplication.SharedApplication.back
			
			Console.WriteLine ("ScheduledLocalNotifications: " + UIApplication.SharedApplication.ScheduledLocalNotifications.Length);
		} 
		*/
	}
}

