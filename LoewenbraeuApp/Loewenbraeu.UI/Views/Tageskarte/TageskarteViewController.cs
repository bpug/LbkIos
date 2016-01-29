using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Loewenbraeu.Data.Service;
using Loewenbraeu.UI.ServiceAgents;
using Loewenbraeu.Core;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public partial class TageskarteViewController : BaseViewController
	{
		private DateTime _currentDay = DateTime.Now; //new DateTime (2011, 7, 14);
		private LoadingHUDView _hud;
		
		public TageskarteViewController () : base("TageskarteViewController", null)
		{
			Initialize ();
		}
		
		void Initialize ()
		{
			Title = _currentDay.ToString ("dd.MM.yyyy");
			ServiceAgent.Current.ServiceClient.TodaysMenuCompleted += HandleTodaysMenuCompleted;
			
			_hud = new LoadingHUDView (){ 
				HudBackgroundColor = Util.LbkBackgroundColor (0.8f),
				ShowRoundedRectangle = true
			};
		}

		void HandleTodaysMenuCompleted (object sender, TodaysMenuCompletedEventArgs args)
		{
			InvokeOnMainThread (delegate	{
				if (Util.IsAsynchCompletedError (args, "TodaysMenu")) {
					//Util.PopNetworkActive ();
					_hud.StopAnimating ();
					//_hud.Hide (true);
					this.NavigationItem.RightBarButtonItem.Enabled = true;
					return;
				}
				try {
					DishesOfTheDay result = args.Result;
					this.tblTageskarte.Source = new TableSource (result.DishOfTheDay.ToList ());
					this.tblTageskarte.ReloadData ();
				} catch (Exception ex) {
					using (UIAlertView alert = new UIAlertView("TodaysMenuCompleted",ex.Message,null,"OK",null)) {
						alert.Show ();
					}
				} finally {
					//Util.PopNetworkActive ();
					this.NavigationItem.RightBarButtonItem.Enabled = true;
					
					_hud.StopAnimating ();
					//_hud.Hide (true);
					//_hud.RemoveFromSuperview ();
					//_hud = null;
				}
			});
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
			
		}

		public override void LoadView ()
		{
			base.LoadView ();
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			/*
			_hud = new ProgressHUD (this.View){
				Mode = ProgressHUDMode.Indeterminate
			};
			*/
			this.View.AddSubview (_hud);
			
			UIBarButtonItem btnRefresh = new UIBarButtonItem (UIBarButtonSystemItem.Refresh, OnMenuRefresh);			
			this.NavigationItem.SetRightBarButtonItem (btnRefresh, true);
			this.tblTageskarte.BackgroundColor = UIColor.Clear;
			this.View.BringSubviewToFront (this.tblTageskarte);
			
			GetAktuelleTagesKarte ();
			
			//this.tblTageskarte.Source = new TableSource(this);
		}
		
		private void GetAktuelleTagesKarte ()
		{
			if (Util.ExecuteService (() => ServiceAgent.Current.ServiceClient.TodaysMenuAsync (_currentDay.ToString ("yyyy-MM-dd"), Util.DeviceUid ))) {
				_hud.StartAnimating ();
				this.NavigationItem.RightBarButtonItem.Enabled = false;
			}
			//ServiceAgent.GetTodaysMenu (_currentDay);
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Release any retained subviews of the main view.
			// e.g. myOutlet = null;
			ReleaseDesignerOutlets ();
			_hud = null;
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
			
		public void OnMenuRefresh (Object sender, EventArgs args)
		{	
			GetAktuelleTagesKarte ();
		}	
		
		private class TableSource : UITableViewSource
		{
			private List<category> _tableItems;
			private TageskarteViewController _tvc;
			private int _counter = Environment.TickCount;
			protected string _customCellIdentifier = "CustomTagesKarteTableCellView";
			protected Dictionary<int, TagesKarteCell> _cellControllers = 
			new Dictionary<int, TagesKarteCell> ();
			
			public TableSource (TageskarteViewController tvc)
			{
				this._tvc = tvc;
			}
			
			public TableSource (List<category> tableItems)
			{
				this._tableItems = tableItems;
			}
			
			public override int NumberOfSections (UITableView tableView)
			{
				return this._tableItems.Count;
				
			}
			
			//http://simon.nureality.ca/simon-says-project-d-uitableviewcells-autosize-based-on-text-height-in-monotouch-heres-how/
			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return 95f;
			}
			
			
			/// <summary>
			/// Called by the TableView to determine how many cells to create for that particular section.
			/// </summary>
			public override int RowsInSection (UITableView tableview, int section)
			{
				return this._tableItems [section].Dishes.Count ();
				
			}
			
			public override float GetHeightForHeader (UITableView tableView, int section)
			{
				float textHeight = tableView.StringSize (this._tableItems [section].Title,
				                                         UIFont.BoldSystemFontOfSize (17f), 
				                                         new SizeF (300.0f, 1000.0f), 
				                                         UILineBreakMode.WordWrap).Height;
				return textHeight + 20;
			}
			
			public override float GetHeightForFooter (UITableView tableView, int section)
			{
				float textHeight = tableView.StringSize (this._tableItems [section].Subtitle,
				                                         UIFont.SystemFontOfSize (14f), 
				                                         new SizeF (300.0f, 1000.0f), 
				                                         UILineBreakMode.WordWrap).Height;
				return textHeight;
			}
			
			public override UIView GetViewForHeader (UITableView tableView, int section)
			{
				return this.GetSectionView (tableView, this._tableItems [section].Title, true);
			}
			 
			public override UIView GetViewForFooter (UITableView tableView, int section)
			{
				return this.GetSectionView (tableView, this._tableItems [section].Subtitle, false);
			}
			
			private UIView GetSectionView (UITableView tableView, string text, bool header)
			{
				UIFont font; 
				float textHeight = 0f;
				
				UIView view = new UIView ();
				UILabel label = new UILabel ();
				
				if (header) {
					font = UIFont.BoldSystemFontOfSize (17f);
					label.TextAlignment = UITextAlignment.Left;
					textHeight = 20;
				} else {
					font = UIFont.SystemFontOfSize (14f);
					label.TextAlignment = UITextAlignment.Center;
					label.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
				}
				
				textHeight += tableView.StringSize (text, font, new SizeF (300.0f, 1000.0f), UILineBreakMode.WordWrap).Height;
				view.Frame = new RectangleF (10f, 0f, 300f, textHeight);
				view.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
				
				label.Frame = new RectangleF (10f, 0f, 300f, textHeight);
				label.Lines = 0;
				label.BackgroundColor = UIColor.Clear;
				label.Opaque = false;
				label.TextColor = UIColor.White;
				label.Text = text;
				label.Font = font;
				
				view.AddSubview (label);
				return view;

			}
			
			/// <summary>
			/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
			/// </summary>
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				//---- declare vars
				UITableViewCell cell = tableView.DequeueReusableCell (this._customCellIdentifier);
				TagesKarteCell customCellController = null;
			
				//---- if there are no cells to reuse, create a new one
				if (cell == null) {
					customCellController = new TagesKarteCell ();
					// retreive the cell from our custom cell controller
					cell = customCellController.SpeisenZelle;
					// give the cell a unique ID, so we can match it up to the controller
					cell.Tag = _counter++; //Environment.TickCount ;
					// store our controller with the unique ID we gave our cell
					this._cellControllers.Add (cell.Tag, customCellController);
				} else {
					// retreive our controller via it's unique ID
					customCellController = this._cellControllers [cell.Tag];
				}
			
				//---- create a shortcut to our item
				dish item = this._tableItems [indexPath.Section].Dishes [indexPath.Row];
				
				customCellController.BindDataToCell (item);
				return cell;
			}
		}
	}
}

