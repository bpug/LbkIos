using System;
using MonoTouch.UIKit;
using Loewenbraeu.Core;
using Loewenbraeu.UI.ServiceAgents;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Dialog;
using Loewenbraeu.Data.Service;
using Loewenbraeu.Core.Extensions;

namespace Loewenbraeu.UI
{
	public class EventsViewController_OLD : BaseViewController
	{
		
		private LoadingHUDView _hud;
		private DialogViewController _dvc; 
		
		public EventsViewController_OLD ()
		{
			Initialize ();
		}
		
		void Initialize ()
		{
			Title = Locale.GetText ("Events");
			ServiceAgent.Current.ServiceClient.GetEventsCompleted += HandleGetEventsCompleted;
			
			_hud = new LoadingHUDView (){ 
				HudBackgroundColor = Util.LbkBackgroundColor (0.8f),
				ShowRoundedRectangle = true
			};
			
			_dvc = new DialogViewController (null, true);
			_dvc.RefreshRequested += delegate {
				GetEvents ();
			};
			_dvc.Style = UITableViewStyle.Plain;
			_dvc.TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			_dvc.TableView.BackgroundColor = UIColor.Clear;
		}
		
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.View.AddSubview (_hud);
			
			UIBarButtonItem btnRefresh = new UIBarButtonItem (UIBarButtonSystemItem.Refresh, OnEventRefresh);			
			this.NavigationItem.SetRightBarButtonItem (btnRefresh, true);
			
			this.View.AddSubview (_dvc.View);
			
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			GetEvents ();
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				this._dvc.Release ();
				this._hud.Release ();
			}
			base.Dispose (disposing);
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation == UIInterfaceOrientation.Portrait || 
			        toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown);
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		private void GetEvents ()
		{
			if (Util.ExecuteService (() => ServiceAgent.Current.ServiceClient.GetEventsAsync (Util.DeviceUid))) {
				_hud.StartAnimating ();
				this.NavigationItem.RightBarButtonItem.Enabled = false;
			}
		}

		private void HandleGetEventsCompleted (object sender, Loewenbraeu.Data.Service.GetEventsCompletedEventArgs args)
		{
			InvokeOnMainThread (delegate	{
				if (Util.IsAsynchCompletedError (args, "GetEvents")) {
					_hud.StopAnimating ();
					this.NavigationItem.RightBarButtonItem.Enabled = true;
					_dvc.ReloadComplete ();
					return;
				}
				try {
					List<Event> result = args.Result.ToList ();
					this.BindEvents (result);
				} catch (Exception ex) {
					using (UIAlertView alert = new UIAlertView("GetEventsCompleted",ex.Message,null,"OK",null)) {
						alert.Show ();
					}
				} finally {
					this.NavigationItem.RightBarButtonItem.Enabled = true;
					_hud.StopAnimating ();
					// Notify the dialog view controller that we are done
					// this will hide the progress info
					_dvc.ReloadComplete ();
				}
			});
		}
		
		private void BindEvents (List<Event> events)
		{
			if (events == null)
				return;
			
			var root = new RootElement ("Events"){
					new Section () {
						from even in events select (Element)new EventElement (even)
					}
			};
			_dvc.Root = root;
			_dvc.TableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
		}
		
		public void OnEventRefresh (Object sender, EventArgs args)
		{	
			GetEvents ();
		}
		
	}
}

