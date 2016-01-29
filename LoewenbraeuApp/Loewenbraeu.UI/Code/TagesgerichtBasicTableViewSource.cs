using System;
using System.Collections.Generic;
using MonoTouch.UIKit;

namespace LoewenbraeuApp5
{
	//========================================================================
	/// <summary>
	/// Combined DataSource and Delegate for our UITableView
	/// </summary>
	public class TagesgerichtBasicTableViewSource : UITableViewSource
	{
		//---- declare vars
		protected List<TagesgerichtBasicTableViewItemGroup> _tableItems;
		protected string _cellIdentifier = "BasicTableViewCell";
		
		public TagesgerichtBasicTableViewSource (List<TagesgerichtBasicTableViewItemGroup> items)
		{
			this._tableItems = items;
		}

		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override int NumberOfSections (UITableView tableView)
		{
			return this._tableItems.Count;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override int RowsInSection (UITableView tableview, int section)
		{
			return this._tableItems[section].Items.Count;
		}

		/// <summary>
		/// Called by the TableView to retrieve the header text for the particular section(group)
		/// </summary>
		public override string TitleForHeader (UITableView tableView, int section)
		{
			return this._tableItems[section].Name;
		}

		/// <summary>
		/// Called by the TableView to retrieve the footer text for the particular section(group)
		/// </summary>
		public override string TitleForFooter (UITableView tableView, int section)
		{
			return this._tableItems[section].Footer;
		}

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			//---- declare vars
			UITableViewCell cell = tableView.DequeueReusableCell (this._cellIdentifier);
			
			//---- if there are no cells to reuse, create a new one
			if (cell == null)
			{
				//cell = new UITableViewCell (UITableViewCellStyle.Default, this._cellIdentifier);
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, this._cellIdentifier);
			}
		
			//---- create a shortcut to our item
			TagesgerichtBasicTableViewItem item = this._tableItems[indexPath.Section].Items[indexPath.Row];
			
			cell.TextLabel.Text = item.Name;
			cell.Accessory = UITableViewCellAccessory.Checkmark;
			cell.DetailTextLabel.Text = item.SubHeading;
	
			
	
			if(!string.IsNullOrEmpty(item.ImageName))
			{
				cell.ImageView.Image = UIImage.FromFile("Images/" + item.ImageName );
			}

			return cell;
		}

	}
	//========================================================================
}
