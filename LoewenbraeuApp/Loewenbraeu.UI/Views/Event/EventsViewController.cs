using System;
using System.Collections.Generic;
using System.Linq;
using Loewenbraeu.Core;
using Loewenbraeu.Data.Service;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Timers;
using System.Net;

namespace Loewenbraeu.UI
{
	public class EventsViewController : LbkDialogViewController
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
		
		
		public EventsViewController (bool pushing) : base (null, pushing)
		{
			Initialize ();
		}
		
		void Initialize ()
		{
			Title = Locale.GetText ("Events");
			ServiceAgent.Current.ServiceClient.GetEventsCompleted += HandleGetEventsCompleted;
			
			RefreshRequested += delegate {
				Load ();
			};
			Style = UITableViewStyle.Grouped;
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			TableView.SectionHeaderHeight = 8;
			TableView.SectionFooterHeight = 0;
			//NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Refresh, OnEventRefresh);
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Refresh, OnEventRefresh);
			Load ();
		}
		
		public void Load ()
		{
			if (Busy)
				return;
			
			if (ServiceAgent.Execute (ServiceAgent.Current.ServiceClient.GetEventsAsync, Util.DeviceUid)) {
				Busy = true;
			}
		}
		
		private void HandleGetEventsCompleted (object sender, Loewenbraeu.Data.Service.GetEventsCompletedEventArgs args)
		{
			bool error = ServiceAgent.HandleAsynchCompletedError (args, "GetEvents");
			
			InvokeOnMainThread (delegate {
				Busy = false;
				if (error)
					return;
				
				List<Event> result = args.Result.ToList ();
				this.BindEvents (result);
			});
		}
		
		private void BindEvents (List<Event> events)
		{
			if (events == null)
				return;
			/*
			var root = new RootElement (Title){
					new Section () {
						from levent in events select (Element)new EventElement (levent, this)
					}
			};
			*/
			
			var root = new RootElement (Title);
			foreach (var @event in events.OrderBy(p => p.DateOrder)) {
				var element = new EventElement (@event, this);
				root.Add (new Section { element });
			}
			
			Root = root;
			//TableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
		}
		
		private void OnEventRefresh (Object sender, EventArgs args)
		{	
			Load ();
		}
	}
}

