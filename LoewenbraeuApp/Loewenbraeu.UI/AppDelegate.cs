using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using Loewenbraeu.Core;
using MonoTouch.AudioToolbox;
using System.Diagnostics;
//using MonoTouch.TestFlight;

namespace Loewenbraeu.UI
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		BaseNavigationController rootNavigationController;
		UIImageView splashView;
		

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
#if DEBUG
#else
			//TestFlight.TakeOff ("ac104b5ca4df6cdfc5b9c63eb448f2af_NjMyMDYyMDEyLTAyLTE2IDExOjE5OjQ4LjkxMjI3Mw");
#endif	
			
#region Notification
			// check for a notification
			if (options != null) {
				/*
				// check for a local notification
				if (options.ContainsKey (UIApplication.LaunchOptionsLocalNotificationKey)) {
					
					UILocalNotification localNotification = options [UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
					if (localNotification != null) {
						
						new UIAlertView (localNotification.AlertAction, localNotification.AlertBody, null, "OK", null).Show ();
						// reset our badge
						UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
					}
				}
				*/
				// check for a remote notification
				if (options.ContainsKey (UIApplication.LaunchOptionsRemoteNotificationKey)) {
					
					NSDictionary remoteNotification = options [UIApplication.LaunchOptionsRemoteNotificationKey] as NSDictionary;
					if (remoteNotification != null) {
						//new UIAlertView(""Notification, remoteNotification["alert"], null, "OK", null).Show();
						//PushNotifications.ProcessNotification (options, true);
						UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
					}
				}
			}
			
			//PushNotifications.Subscribe ();
#endregion			
			
			this.window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			//---- instantiate a new home screen 
			RootViewController rootController = new RootViewController ();
			
			//---- instantiate a new navigation controller 
			this.rootNavigationController = new BaseNavigationController (rootController);
    
			//---- add the home screen to the navigation controller 
			// (it'll be the top most screen) 
			//this.rootNavigationController.PushViewController (rootController, false); 
    
			//---- set the root view controller on the window. the nav 
			// controller will handle the rest 
			this.window.RootViewController = this.rootNavigationController;
			this.window.MakeKeyAndVisible ();
			//showSplash (rootController);
			showSplashScreen ();
			return true;
			
		}
		
		public void showSplash (RootViewController rootController)
		{
			var splashView = new UIImageView (UIScreen.MainScreen.Bounds);// (new RectangleF (0f, 0f, 320f, 480f));
			splashView.Image = UIImage.FromFile ("Default.png");
			
			UIViewController modalViewController = new UIViewController ();
			modalViewController.View = splashView;
			modalViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
			rootController.PresentViewController (modalViewController, false, null);
			NSTimer.CreateScheduledTimer (new TimeSpan (0, 0, 3), 
	                                delegate { 
				modalViewController.DismissViewController(true, null);
				UIApplication.SharedApplication.SetStatusBarHidden (false, false);
					
			});
		}
		
		void showSplashScreen ()
		{
			splashView = new UIImageView (UIScreen.MainScreen.Bounds);
			if (UIScreen.MainScreen.Bounds.Height > 480) {
				splashView.Image = UIImage.FromFile ("Default-568h@2x.png");
			} else {
				splashView.Image = UIImage.FromFile ("Default.png");
			}

			window.AddSubview (splashView);
			window.BringSubviewToFront (splashView);
			System.Threading.Thread.Sleep (new TimeSpan (0, 0, 3));
			UIView.BeginAnimations ("SplashScreen");
			UIView.SetAnimationDuration (1.5f);
			UIView.SetAnimationDelegate (this);
			UIView.SetAnimationTransition (UIViewAnimationTransition.None, window, true);
			UIView.SetAnimationDidStopSelector (new Selector ("completedAnimation"));
			splashView.Alpha = 0f;
			splashView.Frame = new RectangleF (-60f, -60f, 440f, 600f);
			UIView.CommitAnimations ();
		}

		[Export("completedAnimation")]
		void StartupAnimationDone ()
		{
			splashView.RemoveFromSuperview ();
			splashView.Dispose ();
			UIApplication.SharedApplication.SetStatusBarHidden (false, false);
		}


		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations (UIApplication application, UIWindow forWindow)
		{
			return UIInterfaceOrientationMask.All;
		}


		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			//PushNotifications.EnsureDeviceRegistration (deviceToken);
			
		}
		
		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			//PushNotifications.ProcessNotification (userInfo, false);
			//UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
			/*
			var aps = userInfo.ObjectForKey (new NSString (Resources.Communication.ApsRootElement)) as NSDictionary;
			var alert = aps.ObjectForKey (new NSString (Resources.Communication.ApsAlertElement)).ToString ();
			var sound = aps.ObjectForKey (new NSString (Resources.Communication.ApsSoundElement)).ToString ();
			var badge  = aps.ObjectForKey (new NSString (Resources.Communication.ApsBadgeElement)).ToString ();
     
			Debug.WriteLine ("Alert: " + alert);
     
			//var sound2 = SystemSound.FromFile (new NSUrl ("sound"));
			//sound2.PlayAlertSound ();
     
			var av = new UIAlertView ("Notification:", alert, null, "OK", null);
			av.Show ();
			*/
		
		}
		
		
		/// <summary>
		/// Registering for push notifications can fail, for instance, if the device doesn't have network access.
		/// 
		/// In this case, this method will be called.
		/// </summary>
		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			//new UIAlertView (Locale.GetText("Error registering push notifications"), error.LocalizedDescription, null, "OK", null).Show ();
		}
		
		/*
		public override void ReceivedLocalNotification (UIApplication application, UILocalNotification notification)
		{
			// Check if the application is in foreground.
			if (application.ApplicationState == UIApplicationState.Active) {
    
			}
			
			Console.WriteLine ("ScheduledLocalNotifications2: " + UIApplication.SharedApplication.ScheduledLocalNotifications.Length);
			Console.WriteLine (DateTime.Now.ToString () + "  " + application.ApplicationState + "-" + notification);
		}
		*/
	}
}

