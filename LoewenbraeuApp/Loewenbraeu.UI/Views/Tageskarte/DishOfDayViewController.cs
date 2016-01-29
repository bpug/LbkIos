using System;
using System.Linq;
using Loewenbraeu.Core;
using Loewenbraeu.Data.Service;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public class DishOfDayViewController : LbkDialogViewController
	{
		private DateTime _currentDay = DateTime.Now;
		bool _busy = false;
		
		
		bool Busy {
			get {
				return _busy;
			}
			set {
				_busy = value;
				this.NavigationItem.RightBarButtonItem.Enabled = !_busy;
				if (_busy == true) {
					//_mhud.Show (true);
					ShowHud ();
				} else {
					//_mhud.Hide (true);
					HideHud ();
					ReloadComplete ();
				}
			}
		}
		
		
		public DishOfDayViewController (bool pushing) : base (null, pushing, null)
		{
			Initialize ();
		}
		
		private void Initialize ()
		{
			ServiceAgent.Current.ServiceClient.TodaysMenuCompleted += HandleTodaysMenuCompleted;
			
			RefreshRequested += delegate {
				Load ();
			};
			Style = UITableViewStyle.Grouped;
			Autorotate = true;
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			
			
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Refresh, OnMenuRefresh);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			_currentDay = DateTime.Now;
			Title = _currentDay.ToString ("dd.MM.yyyy");
			Load ();
		}


		
		public void OnMenuRefresh (Object sender, EventArgs args)
		{	
			Load ();
		}
		
		public void Load ()
		{
			if (Busy) {
				return;
			}
			
			if (ServiceAgent.Execute (ServiceAgent.Current.ServiceClient.TodaysMenuAsync, _currentDay.ToString ("yyyy-MM-dd"), Util.DeviceUid)) {
				Busy = true;
			}
		}
		
		private void HandleTodaysMenuCompleted (object sender, TodaysMenuCompletedEventArgs args)
		{
			bool error = ServiceAgent.HandleAsynchCompletedError (args, "GetEvents");
			InvokeOnMainThread (delegate	{
				Busy = false;
				if (error)
					return;
				
				DishesOfTheDay result = args.Result;
				BindDishes (result);
			});
		}
		
		private void BindDishes (DishesOfTheDay dishesOfTheDay)
		{
			if (dishesOfTheDay == null)
				return;
			
			var categories = dishesOfTheDay.DishOfTheDay.ToList ();
			
			
			var root = new RootElement (Title){
				from cat in categories
					select new Section (cat.Title, cat.Subtitle){
						from  dish in cat.Dishes select (Element)new DishElement(dish)
					}
			};
			
			Root = root;
		}
		
		
	}
}

