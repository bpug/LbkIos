using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using System.Diagnostics;

namespace Loewenbraeu.UI
{
	public static class PushNotifications
	{
		private const string defaultsName = "DeviceToken";
		
		public static void Subscribe ()
		{
			//register for remote notifications and get the device token
			// set what kind of notification types we want
			UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
			// register for remote notifications
			UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (notificationTypes);
		}
     
		public static void Unsubscribe ()
		{
			UIApplication.SharedApplication.UnregisterForRemoteNotifications ();
		}
     
		public static void EnsureDeviceRegistration (NSData deviceToken)
		{
			var str = (NSString)Runtime.GetNSObject (
                Messaging.intptr_objc_msgSend (deviceToken.Handle, new Selector (
                  "description").Handle));
			var deviceTokenString = str.ToString ().Replace ("<", "").Replace (">", "")
            .Replace (" ", "");
     
			var token = LoadFromSettings ();
          
			if (String.IsNullOrEmpty (token) || token != deviceTokenString) {
				
				Debug.WriteLine (String.Format ("EnsureDeviceRegistration: {0}, {1}", deviceTokenString, deviceTokenString.Length));
                
				//TO DO: send to Server
                     
				SaveToSettings (deviceTokenString);
			} else {
				Debug.WriteLine ("EnsureDeviceRegistration: device already registered.");
			}
		}
		
		public static void ProcessNotification (NSDictionary options, bool fromFinishedLaunching)
		{
			//Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (options.ContainsKey (new NSString (Resources.Communication.ApsRootElement))) {
				//Get the aps dictionary
				var aps = options.ObjectForKey (new NSString (Resources.Communication.ApsRootElement)) as NSDictionary;

				string alert = string.Empty;
				string sound = string.Empty;
				int badge = -1;

				//Extract the alert text
				//NOTE: If you're using the simple alert by just specifying "  aps:{alert:"alert msg here"}  "
				//      this will work fine.  But if you're using a complex alert with Localization keys, etc., your "alert" object from the aps dictionary
				//      will be another NSDictionary... Basically the json gets dumped right into a NSDictionary, so keep that in mind
				if (aps.ContainsKey (new NSString (Resources.Communication.ApsAlertElement))) {
					//alert = (aps [new NSString (Resources.Communication.ApsAlertElement)] as NSString).ToString ();
					alert = aps.ObjectForKey (new NSString (Resources.Communication.ApsAlertElement)).ToString ();
				}

				//Extract the sound string
				if (aps.ContainsKey (new NSString (Resources.Communication.ApsSoundElement))) {
					//sound = (aps [new NSString (Resources.Communication.ApsSoundElement)] as NSString).ToString ();
					sound = aps.ObjectForKey (new NSString (Resources.Communication.ApsSoundElement)).ToString ();
				}

				//Extract the badge
				if (aps.ContainsKey (new NSString (Resources.Communication.ApsBadgeElement))) {
					//string badgeStr = (aps [new NSString (Resources.Communication.ApsBadgeElement)] as NSString).ToString ();
					string badgeStr = aps.ObjectForKey (new NSString (Resources.Communication.ApsBadgeElement)).ToString ();
					int.TryParse (badgeStr, out badge);
				}

				//If this came from the ReceivedRemoteNotification while the app was running,
				// we of course need to manually process things like the sound, badge, and alert.
				if (!fromFinishedLaunching) {
					//Manually set the badge in case this came from a remote notification sent while the app was open
					if (badge >= 0)
						UIApplication.SharedApplication.ApplicationIconBadgeNumber = badge;
					/*
					//Manually play the sound
					if (!string.IsNullOrEmpty (sound)) {
						//This assumes that in your json payload you sent the sound filename (like sound.caf)
						// and that you've included it in your project directory as a Content Build type.
						var soundObj = MonoTouch.AudioToolbox.SystemSound.FromFile (sound);
						soundObj.PlaySystemSound ();
					}
					*/

					//Manually show an alert
					if (!string.IsNullOrEmpty (alert)) {
						using (UIAlertView avAlert = new UIAlertView ("Notification", alert, null, "OK", null)) {
							avAlert.Show ();
						}
					}
				}

			}
			
		}
     
		private static string LoadFromSettings ()
		{
			var prefs = NSUserDefaults.StandardUserDefaults;
			return prefs.StringForKey (defaultsName);
		}
     
		private static void SaveToSettings (string deviceToken)
		{
			var prefs = NSUserDefaults.StandardUserDefaults;
			prefs.SetString(deviceToken, defaultsName);
		}
	}
}

