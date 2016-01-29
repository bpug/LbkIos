using System;
using System.Collections.Generic;
using System.Linq;
using Loewenbraeu.Core;
using Loewenbraeu.Data;
using Loewenbraeu.Data.Mappings;
using Loewenbraeu.Data.Service;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
using Model = Loewenbraeu.Model;

namespace Loewenbraeu.UI
{
	public class VideoViewController : LbkDialogViewController
	{
	
		bool _busy = false;
		
		bool Busy {
			get {
				return _busy;
			}
			set {
				_busy = value;
				this.NavigationItem.RightBarButtonItem.Enabled = !_busy;
				if (_busy == true) {
					ShowHud ();
				} else {
					HideHud ();
					ReloadComplete ();
				}
			}
		}
		
		public VideoViewController (bool pushing) : base (null, pushing, null)
		{
			Initialize ();
		}
		
		void Initialize ()
		{
			Title = Locale.GetText ("Videos");
			
			ServiceAgent.Current.ServiceClient.GetVideosCompleted += HandleGetVideosCompleted;
			
			RefreshRequested += delegate {
				Load ();
			};
			
			Style = UITableViewStyle.Grouped;
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			TableView.SectionHeaderHeight = 5;
			TableView.SectionFooterHeight = 0;
			Autorotate = true;
		}

		
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Refresh, OnRefresh);
			Load ();
		}
		
		public void OnRefresh (Object sender, EventArgs args)
		{	
			Load ();
		}
		
		public void Load ()
		{
			if (Busy)
				return;
			
			if (ServiceAgent.Execute (ServiceAgent.Current.ServiceClient.GetVideosAsync, Util.DeviceUid)) {
				Busy = true;
			}
		}
		
		
		private void HandleGetVideosCompleted (object sender, Loewenbraeu.Data.Service.GetVideosCompletedEventArgs args)
		{
			bool error = ServiceAgent.HandleAsynchCompletedError (args, "GetVideos");
			
			InvokeOnMainThread (delegate {
				Busy = false;
				if (error)
					return;
				
				List<Model.Video> result = args.Result.ToList().ToLbkVideo();
				this.BindVideos (result);
			});
		}
		
		public void BindVideos (List<Model.Video> videos)
		{
			if (videos == null)
				return;
			
			RootElement root = new RootElement (Title);
			
			foreach (var video in videos) {
				var element = new VideoElement (video);
				if (!video.IsYoutube) {
					element.VideoSelected += delegate(object sender, EventArgs<Model.Video> e) {
						var mv = new MPMoviePlayerViewController (new NSUrl (e.Value.Url));
						mv.Title = e.Value.Title;
						PresentMoviePlayerViewController (mv);
					};
				}
				root.Add (new Section { element });
			}
			Root = root;
			
		}
	}
}