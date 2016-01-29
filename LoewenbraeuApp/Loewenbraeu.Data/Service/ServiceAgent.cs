//wsdl -l:CS -n:"Loewenbraeu.Data.Service" -o:/${ProjectDir}/ServiceProxy/LbkProxy.cs  http://loewenbraeuservice.ip-connect.de/service1.asmx?WSDL

//http://lbkmobile.loewenbraeukeller.com/Service1.asmx
using Loewenbraeu.Data.Service;
using System;
using System.Linq;
using System.Timers;
using Loewenbraeu.Core;
using MonoTouch.UIKit;
using System.Net;
using System.Threading;

namespace Loewenbraeu.Data.Service
{
	public class ServiceAgent
	{
		private const int CLIENT_TIMEOUT = 15*1000;
		
		private static readonly ServiceAgent _instance =  new ServiceAgent();
		private static object _threadSynchronizationHandle = null;

		private Service1 _serviceClient;
        //private static string accessKey = "E06163458B670154564E415883783E9A";

        #region - Constructors and Destructors -
        public  ServiceAgent()
        {
        }
        #endregion

        #region - Properties -

        public static ServiceAgent Current
        {
            //get { return _instance; }
			get {
				lock (GetThreadSynchronizationHandle()) {
					return _instance;
				}
			}
        }
		
        public  Service1 ServiceClient {
			get {
				if (this._serviceClient == null) {
					this.InitializeProxy ();
				}
				return this._serviceClient;
			}
		}
        #endregion
		
		public void CloseService ()
		{
			if (_serviceClient != null) {
				_serviceClient.Abort ();
				_serviceClient = null;
			}
		}
		
		#region Thread Synchronization Handles
 
		private static object GetThreadSynchronizationHandle ()
		{
			//When the thread synchronization handle object is requested, we use the CompareExchange
			//method of the Interlocked class to see if the value is null.  If it is null, then we 
			//will create the new object.  If it is not null then we will return the previously
			//allocated lock target.
			Interlocked.CompareExchange (ref _threadSynchronizationHandle, new object (), null);
			return _threadSynchronizationHandle;
		}
 
        #endregion

        private void InitializeProxy ()
		{
			this._serviceClient = new Service1 ();
			//Keine Wirkung -?????
			//this._serviceClient.Timeout = CLIENT_TIMEOUT;
			/*
			AuthHeader header = new AuthHeader ();
			header.AccessKey = accessKey;
			this._serviceClient.AuthHeaderValue = header;
			header = null;
			*/
		}
		
		//http://stackoverflow.com/questions/3747948/c-sharp-how-can-i-overload-a-delegate
		//http://stackoverflow.com/questions/3509959/c-sharp-action-and-func-parameter-overloads
		//public delegate void ExecuteAction<T> ( params T[] arg);
		
		public static bool Execute<T> (Action<T, object> action, T obj, object userState = null)
		{
			if (!IsReachable ()) {
				return false;
			}
			System.Timers.Timer serviceTimer = GetServiceTimer ();
			var state = new Tuple<System.Timers.Timer, object> (serviceTimer, userState);
			action.Invoke (obj, state);
			//action.Invoke (obj, serviceTimer);
			return true;
		}
		
		public static bool Execute<T1, T2> (Action<T1, T2, object> action, T1 obj1, T2 obj2, object userState = null)
		{
			if (!IsReachable ()) {
				return false;
			}
			System.Timers.Timer serviceTimer = GetServiceTimer ();
			var state = new Tuple<System.Timers.Timer, object> (serviceTimer, userState);
			action.Invoke (obj1, obj2, state);
			//action.Invoke (obj1, obj2, serviceTimer);
			return true;
		}
		
		public static bool Execute<T1, T2, T3> (Action<T1, T2, T3, object> action, T1 obj1, T2 obj2, T3 obj3, object userState = null)
		{
			if (!IsReachable ()) {
				return false;
			}
			System.Timers.Timer serviceTimer = GetServiceTimer ();
			var state = new Tuple<System.Timers.Timer, object> (serviceTimer, userState);
			action.Invoke (obj1, obj2, obj3, state);
			//action.Invoke (obj1, obj2, obj3, serviceTimer);
			return true;
		}
		
		private static bool IsReachable ()
		{
			if (!Util.IsReachable ()) {
				using (var alert = new UIAlertView(Locale.GetText("Keine Internetverbindung"),"",null,"OK")) {
					alert.Show ();
					return false;
				}
			}
			return true;
		}
		
		//http://spinningtheweb.blogspot.com/2011/02/async-web-service-timout.html
		private static System.Timers.Timer GetServiceTimer ()
		{
			//Timer is set to go off one time after 15 seconds
			System.Timers.Timer serviceTimer = new System.Timers.Timer (CLIENT_TIMEOUT);
			serviceTimer.AutoReset = false;
			serviceTimer.Elapsed += delegate(object source, ElapsedEventArgs e) {
				Current.ServiceClient.Abort ();
				throw new WebException ("Timeout expired!"); 
			};
			serviceTimer.Enabled = true;
			return serviceTimer;
		}
		
		public static bool HandleAsynchCompletedError (System.ComponentModel.AsyncCompletedEventArgs args, string quelle)
		{
			//Timer serviceTimer = args.UserState as Timer;
			var userState = args.UserState as Tuple<System.Timers.Timer, object>;
			var serviceTimer = userState.Item1;
			if (serviceTimer != null) {
				serviceTimer.Enabled = false;
				serviceTimer.Dispose ();
			}
#if DEBUG
			string error = string.Empty;
			if (args.Cancelled == true) {
				using (UIAlertView alert = new UIAlertView(quelle,"Async reguest is Cancelled.",null,"OK",null)) {
					alert.Show ();
				}
				return true;
			}
			if (args.Error != null) {
				if (args.Error is System.Net.WebException) {
					error = "WebException";
				} else if (args.Error is System.Web.Services.Protocols.SoapException) {
					error = "SoapException";
				} else {
					error = "Unbekannte Fehler";
				}
				using (UIAlertView alert = new UIAlertView(quelle + ":" + error, args.Error.Message,null,"OK",null)) {
					alert.Show ();
				}
				return true;
			} else {
				return false;
			}
#else
			if (args.Error != null || args.Cancelled == true) {
				using (UIAlertView alert = new UIAlertView(Locale.GetText("Zeitüberschreitung"),Locale.GetText("Der Server ist momentan nicht erreichbar.") ,null,"OK",null)) {
					alert.Show ();
				}
				return true;
			}
			return false;
#endif
		}
		
		/*
		

		public static void  GetTodaysMenu (DateTime date)
		{
			string strDate = date.ToString ("yyyy-MM-dd");
			try {
				Current.ServiceClient.TodaysMenuAsync (strDate);
				//Util.PushNetworkActive ();
			} catch (Exception ex) {
				//Util.PopNetworkActive ();
				using (UIAlertView alert = new UIAlertView("TodaysMenuAsync",ex.Message,null,"OK",null)) {
					alert.Show ();
				}
			}
			
		}
		
		public static void  Reserve (Reservation reservation)
		{
			
			try {
				Current.ServiceClient.CreateReservationByObjectAsync (reservation);
				//Util.PushNetworkActive ();
			} catch (Exception ex) {
				//Util.PopNetworkActive ();
				using (UIAlertView alert = new UIAlertView("Reserve",ex.Message,null,"OK",null)) {
					alert.Show ();
				}
			}
			
		}
		
		public static void  ConfirmReservation (string reservationId)
		{
			try {
				Current.ServiceClient.SetReservationConfirmCustomerAsync (reservationId);
				//Util.PushNetworkActive ();
			} catch (Exception ex) {
				//Util.PopNetworkActive ();
				using (UIAlertView alert = new UIAlertView("ConfirmReservation",ex.Message,null,"OK",null)) {
					alert.Show ();
				}
			}
			
		}
		
		public static void  AbortReservation (string reservationId)
		{
			try {
				Current.ServiceClient.SetDecliningAsync (reservationId);
				//Util.PushNetworkActive ();
			} catch (Exception ex) {
				//Util.PopNetworkActive ();
				using (UIAlertView alert = new UIAlertView("AbortReservation",ex.Message,null,"OK",null)) {
					alert.Show ();
				}
			}
		}
		
		
		public static void  GetReservation (string reservationId)
		{
			try {
				Current.ServiceClient.GetReservationByIdAsync (reservationId);
				//Util.PushNetworkActive ();
			} catch (Exception ex) {
				//Util.PopNetworkActive ();
				using (UIAlertView alert = new UIAlertView("GetReservation",ex.Message,null,"OK",null)) {
					alert.Show ();
				}
			}
		}
		
		public static void  IsDeclinedByRestaurantAsync (string reservationId)
		{
			try {
				Current.ServiceClient.IsDeclinedByRestaurantAsync (reservationId);
				//Util.PushNetworkActive ();
			} catch (Exception ex) {
				//Util.PopNetworkActive ();
				using (UIAlertView alert = new UIAlertView("IsDeclinedByResaturantAsync",ex.Message,null,"OK",null)) {
					alert.Show ();
				}
			}
		}
		*/
	}
}

