using System;
using System.Drawing;
using Loewenbraeu.Data.Service;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public class DishCell : UITableViewCell
	{
		const int TITLE_SIZE = 16;
		const int DESCR_SIZE = 14;
		const int PRICE_SIZE = 17;
		private const float OFFSET_X = 10;
		static UIFont titleFont = UIFont.BoldSystemFontOfSize (TITLE_SIZE);
		static UIFont descrFont = UIFont.SystemFontOfSize (DESCR_SIZE);
		static UIFont priceFont = UIFont.SystemFontOfSize (PRICE_SIZE);
		private UILabel _lblHeadline;
		private UILabel _lblDescription;
		private UILabel _lblPrice;
		private dish _dish;
		
		#region Constructors
		// Should never happen
		public DishCell (IntPtr handle) : base (handle)
		{
			//Console.WriteLine (Environment.StackTrace);
		}
		
		public DishCell (NSString ident, dish dish) : this(UITableViewCellStyle.Default, ident, dish)
		{
			
		}
		
		public DishCell (dish dish) : this(UITableViewCellStyle.Default, new NSString("DishCell"), dish)
		{
			
		}
		
		public DishCell (UITableViewCellStyle style, NSString ident, dish dish) : base (style, ident)
		{
			this.Initialize (dish);
		}
		 #endregion
		
		private void Initialize (dish dish)
		{
			SelectionStyle = UITableViewCellSelectionStyle.None;
			BackgroundColor = Resources.Colors.Cell;
			
			_lblHeadline = new UILabel (){
				Font = titleFont,
				TextColor = Resources.Colors.CellText,
				BackgroundColor = UIColor.Clear,
				AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth,
			};
				
			_lblDescription = new UILabel (){
				Font = descrFont,
				TextColor = Resources.Colors.CellText,
				BackgroundColor = UIColor.Clear,
				Lines = 0,
				AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth,
			};
				
			_lblPrice = new UILabel (){
				Font = priceFont,
				TextColor = Resources.Colors.CellText,
				BackgroundColor = UIColor.Clear,
				TextAlignment = UITextAlignment.Right,
				AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth,
			};
			
			Update (dish);
			
			ContentView.Add (_lblHeadline);
			ContentView.Add (_lblDescription);
			ContentView.Add (_lblPrice);
			
		}
		
		// 
		// This method is called when the cell is reused to reset
		// all of the cell values
		//
		public void Update (dish dish)
		{
			_dish = dish;
			
			_lblHeadline.Text = _dish.Headline;
			_lblDescription.Text = _dish.Description;
			_lblPrice.Text = string.Format ("{0} €", _dish.Price);
			
			SetNeedsDisplay ();
		}
		
		public static float GetCellHeight (RectangleF bounds, dish dish)
		{
			return 95f;
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			RectangleF full = ContentView.Bounds;
			
			_lblHeadline.Frame = new RectangleF (OFFSET_X, 6, full.Width - 2 * OFFSET_X, 21);
			_lblDescription.Frame = new RectangleF (OFFSET_X, 27, full.Width - 2 * OFFSET_X, 45);
			_lblPrice.Frame = new RectangleF (full.Width - OFFSET_X - 90, 69, 90, 21);
		}
	
	}
	
	public class DishElement : Element, IElementSizing
	{
		public readonly dish _dish;
		static NSString rkey = new NSString ("DishElement");
		
		public DishElement (dish dish) : base (null)
		{
			this._dish = dish;
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			base.GetCell (tv);
			var cell = tv.DequeueReusableCell (rkey) as DishCell;
			if (cell == null) {
				cell = new DishCell (rkey, this._dish);
			} else {
				cell.Update (this._dish);
			}
			return cell;
		}
		
		protected override NSString CellKey {
			get {
				return rkey;
			}
		}

		#region IElementSizing implementation
		float IElementSizing.GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return DishCell.GetCellHeight (tableView.Bounds, this._dish);
		}
		#endregion
		
		
	}
}

