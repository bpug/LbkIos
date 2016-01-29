using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using Loewenbraeu.Core.Extensions;
using MonoTouch.Dialog.Utilities;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Timers;
using System.Net;

namespace Loewenbraeu.Core
{
	public static class Util
	{
		/// <summary>
		///   A shortcut to the main application
		/// </summary>
		public static UIApplication MainApp = UIApplication.SharedApplication;
		public static UIWindow MainWindow = UIApplication.SharedApplication.KeyWindow;
		public static UIViewController RootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
		public static NSUserDefaults Defaults = NSUserDefaults.StandardUserDefaults;
		public readonly static string BaseDir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "..");
		
		public static bool  OpenUrl (string url)
		{
			NSUrl nurl = new NSUrl (url);
			if (Util.MainApp.CanOpenUrl (nurl)) {
				Util.MainApp.OpenUrl (nurl);
				return true;
			}
			return false;
		}

		//
		// Since we are a multithreaded application and we could have many
		// different outgoing network connections (api.twitter, images,
		// searches) we need a centralized API to keep the network visibility
		// indicator state
		//
		static object networkLock = new object ();
		static int active;
		
		public static void PushNetworkActive ()
		{
			lock (networkLock) {
				active++;
				MainApp.NetworkActivityIndicatorVisible = true;
			}
		}
		
		public static void PopNetworkActive ()
		{
			lock (networkLock) {
				active--;
				if (active == 0)
					MainApp.NetworkActivityIndicatorVisible = false;
			}
		}
		
		public static string DistanceToString (double distance)
		{
			if (distance < 100)
				return  string.Format ("{0} m", Math.Round (distance));
			else if (distance < 1000)
				return  string.Format ("{0} m", Math.Round (distance / 5) * 5);
			else if (distance < 10000)
				return  string.Format ("{0} km", Math.Round (distance / 100) / 10);
			else
				return  string.Format ("{0} km", Math.Round (distance / 1000));
		}
		
		public static string DeviceUid {
			get {
				string defaultsName = "DeviceUniqueIdentifier";
				string id = Util.Defaults.StringForKey (defaultsName);
				if (string.IsNullOrEmpty (id)) {
					string mac = GetDeviceMAC ();
					if (string.IsNullOrEmpty (mac)) {
						id = Guid.NewGuid ().ToString ().ToUpper ();
					} else {
						id = mac.GetMD5 ().ToUpper ();
					}
					Util.Defaults.SetString (id, defaultsName);
				}
				return id;
			}	
		}

		public static bool IsPhone5(){
			if (UIScreen.MainScreen.Bounds.Height > 480) {
				return true;
			}
			return false;
		}
		
		
		//private static Random random = new Random ((int)DateTime.Now.Ticks);
		
		public static string RandomString (int size)
		{
			Random random = new Random ((int)DateTime.Now.Ticks);
			StringBuilder builder = new StringBuilder ();
			char ch;
			for (int i = 0; i < size; i++) {
				ch = Convert.ToChar (Convert.ToInt32 (Math.Floor (26 * random.NextDouble () + 65)));                 
				builder.Append (ch);
			}

			return builder.ToString ();
		}
		
		public static bool IsReachable ()
		{
			//Reachability.RemoteHostStatus () == NetworkStatus.NotReachable
			return Reachability.InternetConnectionStatus () != NetworkStatus.NotReachable 
				&& Reachability.IsHostReachable (Reachability.HostName);
		}
		
		public static bool IsReachable ( string hostName)
		{
			return Reachability.InternetConnectionStatus () != NetworkStatus.NotReachable 
				&& Reachability.IsHostReachable (hostName);
		}
		
		
		public static void CancelAllLocalNotifications ()
		{
			foreach (UILocalNotification sNote in UIApplication.SharedApplication.ScheduledLocalNotifications) {
				//Cancel the Notification'
				UIApplication.SharedApplication.CancelLocalNotification (sNote);
			}
		
		}
		
		public static UIImage GetImageFromLoaderCache (Uri uri)
		{
			UIImage image = null;
			string picDir = Path.Combine (ImageLoader.BaseDir, "Library/Caches/Pictures.MonoTouch.Dialog/");
			string picfile = uri.IsFile ? uri.LocalPath : picDir + uri.AbsoluteUri.GetMD5 ();
			
			if (File.Exists (picfile)) {
				image = UIImage.FromFile (picfile);
			}
			return image;
		}
		
		public static string GetDeviceMAC ()
		{
			string mac = null;
			try {
				mac = (from i in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces ()
		        where i.Id.Equals ("en0")
		        orderby i.Id ascending
		        select string.Join (":", i.GetPhysicalAddress ().GetAddressBytes ().Select (x => x.ToString ("X2")))).FirstOrDefault ();
			} catch {
			} 
			return mac;
		}
		
		public static void ShowNetworkInterfaces ()
		{
			/*IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties ();
			Console.WriteLine ("Interface information for {0}.{1}     ",
                    computerProperties.HostName, computerProperties.DomainName);*/
			NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces ();
			

			if (nics == null || nics.Length < 1) {
				Console.WriteLine ("  No network interfaces found.");
				return;
			}

			Console.WriteLine ("  Number of interfaces .................... : {0}", nics.Length);

			foreach (NetworkInterface adapter in nics) {
				Console.WriteLine ();
				Console.WriteLine (adapter.Description);
				Console.WriteLine (String.Empty.PadLeft (adapter.Description.Length, '='));
				Console.WriteLine ("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
				Console.Write ("  Physical address ........................ : ");
				PhysicalAddress address = adapter.GetPhysicalAddress ();
				byte[] bytes = address.GetAddressBytes ();
				for (int i = 0; i < bytes.Length; i++) {
					// Display the physical address in hexadecimal.
					Console.Write ("{0}", bytes [i].ToString ("X2"));
					// Insert a hyphen after each byte, unless we are at the end of the
					// address.
					if (i != bytes.Length - 1) {
						Console.Write ("-");
					}
				}
				Console.WriteLine ();
			}
		}
		
		public static UIButton FindButtonInView (UIView view)
		{
			UIButton button = null;

			if (view is UIButton) {
				return (UIButton)view;
			}

			if (view.Subviews != null && view.Subviews.Count () > 0) {
				foreach (UIView subview in view.Subviews) {
					button = FindButtonInView (subview);
					if (button != null)
						return button;
				}
			}

			return button;
		}
		
		public static List<UITextField> FindTextFieldsInView (UIView view)
		{
			List<UITextField> fields = new List<UITextField> ();

			if (view is UITextField) {
				fields.Add ((UITextField)view);
			}

			if (view.Subviews != null && view.Subviews.Count () > 0) {
				foreach (UIView subview in view.Subviews) {
					FindTextFieldsInView (subview);
				}
			}

			return fields;
		}
		
		
		
		/*
		public static  void SetLabelHeight4Text (UILabel label)
		{
			float height = 0f;
			using (var nss = new  NSString (label.Text)) {
				height = nss.StringSize (label.Font, new SizeF (label.Frame.Width, 1000.0f), 
			                      UILineBreakMode.WordWrap).Height;
			}
			
			label.Lines = 0;
			label.Frame = new RectangleF (label.Frame.X, label.Frame.Y, label.Frame.Width, height);
		}
		*/
		
	}
}

