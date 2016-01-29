using System;
using Loewenbraeu.Core;
using Loewenbraeu.Data;
using Loewenbraeu.Data.Service;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Net;
using System.IO;
using System.Text;


namespace Loewenbraeu.UI
{
	public class SpeisekarteViewController : BaseViewController
	{
		private readonly UIViewController _rootViewConroller;

		private const string localFileName = "speisekarte.pdf";
		private const string downloadPdfUrl = "http://lbkmobile.loewenbraeukeller.com/media/speisekarte/speisekarte.pdf";
		private DateTime updateDate;

		bool _busy = false;
		
		bool Busy {
			get {
				return _busy;
			}
			set {
				_busy = value;
				//this.NavigationItem.RightBarButtonItem.Enabled = !_busy;
				if (_busy == true) {
					if (HUDMode == MBProgressHUDMode.Indeterminate){
						ShowHud ();
					}
					else{
						ShowHud ("Download");
					}

				} else {
					HideHud ();
				}
			}
		}

		public SpeisekarteViewController (UIViewController rootViewConroller)
		{
			_rootViewConroller = rootViewConroller;
			this.Initialize ();
		}

		private void Initialize ()
		{
			HUDMode = MBProgressHUDMode.Indeterminate;
			ServiceAgent.Current.ServiceClient.GetMenuLastUpdateCompleted += HandleGetMenuLastUpdateCompleted;
		}



		public void Display ()
		{
			/*
			if (!UserDefaults.SpeisekarteUpdateDate.HasValue) {
				this.DownloadKarte (DateTime.Now);
			}else{
				this.GetUpdateDate();
			}
			*/
			this.GetUpdateDate();
		}



		private void GetUpdateDate(){
			//return new DateTime(2012,8,30);
			HUDMode = MBProgressHUDMode.Indeterminate;
			if (Busy)
				return;
			if (ServiceAgent.Execute (ServiceAgent.Current.ServiceClient.GetMenuLastUpdateAsync, Util.DeviceUid)) {
				Busy = true;
			}
		}

		void HandleGetMenuLastUpdateCompleted (object sender, GetMenuLastUpdateCompletedEventArgs args)
		{
			bool error = ServiceAgent.HandleAsynchCompletedError (args, "GetMenuLastUpdate");
			using (var pool = new NSAutoreleasePool()) {
				
				pool.InvokeOnMainThread (delegate	{
					Busy = false;
					if (error)
						return;
					var updateDate = args.Result;
					this.Display(updateDate);
					
				});
			}
		}

		private void Display (DateTime? updateDate)
		{
			if (!UserDefaults.SpeisekarteUpdateDate.HasValue) {
				this.DownloadKarte (updateDate);
			}
			else if(updateDate.HasValue && UserDefaults.SpeisekarteUpdateDate.HasValue && !(Math.Abs((updateDate.Value - UserDefaults.SpeisekarteUpdateDate.Value).TotalSeconds) < 1)) {
				this.updateDate = updateDate.Value;
				var alert = new UIAlertView (Locale.GetText ("Es ist eine eine neue Speisekarte gültig, die nun geladen werden kann."), "",
				                             null, Locale.GetText ("Ja"), Locale.GetText ("Nein"));
				alert.Clicked += delegate(object sender, UIButtonEventArgs e) {
					if (e.ButtonIndex == 0) {
						this.DownloadKarte (updateDate);
					}else{
						//ShowKarte();
						return;
					}
				};
				alert.Show ();
			} else {
				ShowKarte ();
			}
		}

		private void DownloadKarte(DateTime? updateDate){

			HUDMode = MBProgressHUDMode.Determinate;
			this.Busy = true;

			using (var webClient = new WebClient()){
				webClient.DownloadDataCompleted += HandleDownloadDataCompleted;
				webClient.DownloadProgressChanged += HandleDownloadProgressChanged;
				var uri = new Uri (downloadPdfUrl); 
				webClient.DownloadDataAsync (uri, updateDate);
			}
		}

		void HandleDownloadProgressChanged (object sender, DownloadProgressChangedEventArgs e)
		{
//			double bytesIn = double.Parse(e.BytesReceived.ToString());
//		    double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
//		    double percentage = bytesIn / totalBytes * 100;
//			var percentValue = int.Parse(Math.Truncate(percentage).ToString());
			InvokeOnMainThread (() => {
				this.Hud.Progress = e.ProgressPercentage;
			});

			Console.WriteLine("ProgressPercentage: " + e.ProgressPercentage);
		}

		void HandleDownloadDataCompleted (object sender, DownloadDataCompletedEventArgs e)
		{
			if (e.Error != null || e.Cancelled == true){
				InvokeOnMainThread (() => {
					this.Busy = false;
					var alert = new UIAlertView(
						Locale.GetText("Es ist ein Fehler beim Download aufgetreten"), 
						Locale.GetText ("Bitte versuchen Sie es später noch einmal."),
						null, Locale.GetText ("OK"));
					alert.Show ();
				});

				return;
			}

			var bytes = e.Result;
			var updateDate = e.UserState as DateTime?;
		    var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		    
		    var localPath = Path.Combine (documentsPath, localFileName);
			//Console.WriteLine("localPath:"+localPath);

		    File.WriteAllBytes(localPath, bytes);

			InvokeOnMainThread (() => {
				this.Busy = false;
				if (updateDate != null){
					UserDefaults.SpeisekarteUpdateDate = updateDate;
				}
				this.ShowKarte();
			});
		}

		private void ShowKarte()
		{
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		    var localPath = Path.Combine (documentsPath, localFileName);

			if (!File.Exists(localPath)) {
				return;
			}

			var docController = new DocumentViewController (new DocPreviewItem ("Triumphator", localPath));
				             

			this.PushViewController (docController);
		}

		private void PushViewController (UIViewController controller)
		{
			//_rootViewConroller.NavigationController.PopToRootViewController (false);
			//_rootViewConroller.NavigationController.PushViewController (controller, true);
			_rootViewConroller.PresentViewController(controller, true, null);
		}
	}
}

