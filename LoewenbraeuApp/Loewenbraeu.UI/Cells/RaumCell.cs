using System;
using System.Drawing;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Model;
using MonoTouch.Dialog;
using MonoTouch.Dialog.Utilities;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


namespace Loewenbraeu.UI
{
	public partial class RaumCell : UITableViewCell, IImageUpdated
	{
		const int titleSize = 18;
		const int subTitleSize = 15;
		const int PicSize = 60;
		const int PicXPad = 5;
		const int PicYPad = 4;
		const int TextLeftStart = 3 * PicXPad + PicSize;
		const int TextHeightPadding = 12;
		const int TextYOffset = 0;
		const int MinHeight = PicSize + 2 * PicYPad + 1;
		
		static UIFont titleFont = UIFont.SystemFontOfSize (titleSize);
		static UIFont subTitleFont = UIFont.SystemFontOfSize (subTitleSize);
		
		UILabel lblTitle;
		UILabel lblSubtitle;
		UIImageView imageView;
		
		Raum _raum;
		
		public RaumCell (IntPtr handle) : base (handle)
		{
			//Console.WriteLine (Environment.StackTrace);
		}
		
		public RaumCell (UITableViewCellStyle style, NSString ident, Raum raum) : base (style, ident)
		{
			this.Initialize (raum);
		}
		
		
		public RaumCell (NSString ident, Raum raum) : base(UITableViewCellStyle.Default, ident)
		{
			this.Initialize (raum);
		}
		
		private void Initialize (Raum raum)
		{
			_raum = raum;
			
			SelectionStyle = UITableViewCellSelectionStyle.Gray;
			Accessory = UITableViewCellAccessory.DisclosureIndicator;
			
			lblTitle = new UILabel () {
				Font = titleFont,
				TextColor = Resources.Colors.CellText,
				BackgroundColor = UIColor.Clear,
				TextAlignment = UITextAlignment.Left,
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap
			};
			
			lblSubtitle = new UILabel () {
				Font = subTitleFont,
				TextColor = Resources.Colors.CellText,
				BackgroundColor = UIColor.Clear,
				TextAlignment = UITextAlignment.Left,
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap
			};
			
			imageView = new UIImageView (new RectangleF (PicXPad, PicYPad, PicSize, PicSize));
			
			BindDataToCell (_raum);
			
			ContentView.Add (lblTitle);
			ContentView.Add (lblSubtitle);
			ContentView.Add (imageView);
		}
		
		public static float GetCellHeight (RectangleF bounds, Raum raum)
		{
			/*
			bounds.Height = 999;
			
			// Keep the same as LayoutSubviews
			bounds.X = TextLeftStart;
			bounds.Width -= TextLeftStart + TextHeightPadding;
			
			float height = 0;
			
			using (var nss = new NSString (raum.Title)) {
				height += nss.StringSize (titleFont, bounds.Size, UILineBreakMode.WordWrap).Height;
				
			}
			using (var nss = new NSString (raum.Subtitle)) {
				height += nss.StringSize (subTitleFont, bounds.Size, UILineBreakMode.WordWrap).Height;
			}
			
			return Math.Max (height + TextYOffset + 2 * TextHeightPadding, MinHeight);
			*/
			return MinHeight;
		}
		
		// 
		// Layouts the views, called before the cell is shown
		//
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			BackgroundColor = Resources.Colors.Cell;
			
			var full = ContentView.Bounds;
			/*
			var tmp = full;
			tmp = full;
			tmp.Y += TextYOffset;
			tmp.Height -= TextYOffset;
			tmp.X = TextLeftStart;
			tmp.Width -= TextLeftStart + TextHeightPadding + 10;
			lblTitle.Frame = tmp;
			*/
			
			float textWidth = full.Width - TextLeftStart;
			float textHeight = lblTitle.GetHeight4Text (textWidth);
			float y = TextHeightPadding;
			
			lblTitle.Frame = new RectangleF (TextLeftStart, y, textWidth, textHeight);
			
			y += textHeight;
			textHeight = lblSubtitle.GetHeight4Text (textWidth);
			lblSubtitle.Frame = new RectangleF (TextLeftStart, y, textWidth, textHeight);
			
		}
		
		public void BindDataToCell (Raum raum)
		{
			this._raum = raum;
			
			this.lblTitle.Text = raum.Title;
			this.lblSubtitle.Text = raum.Subtitle;
			this.imageView.Image = null;
			
			if (raum.ThumbnailUri != null) {
				var img = UIImage.FromBundle (raum.ThumbnailUrl);//ImageLoader.DefaultRequestImage (raum.ThumbnailUri, this);
				if (img != null)
					this.imageView.Image = img.RoundCorners ();
			}
			SetNeedsDisplay ();
			
		}
		
		#region IImageUpdated implementation
		void IImageUpdated.UpdatedImage (Uri uri)
		{
			// Discard notifications that might have been queued for an old cell
			if (this._raum.ThumbnailUri != uri) {
				return;
			}
			
			UIView.BeginAnimations (null, IntPtr.Zero);
			UIView.SetAnimationDuration (0.5);
			
			var image = Util.GetImageFromLoaderCache(uri);
			if (image != null) {
				imageView.Alpha = 0;
				imageView.Image = image.RoundCorners ();
				//imageView.SizeToFit ();
			}
						
			this.imageView.Alpha = 1;
			UIView.CommitAnimations ();
		}
		#endregion
	}
	
	
	public class RaumElement : Element, IElementSizing 
	{
		public readonly Raum raum;
		static NSString rkey = new NSString ("RaumElement");
		
		public event EventHandler<EventArgs<Raum>> RaumSelected = delegate {};
		
		public RaumElement (Raum raum) : base (null)
		{
			this.raum = raum;
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			base.GetCell (tv);
			var cell = tv.DequeueReusableCell (rkey) as RaumCell;
			if (cell == null) {
				cell = new RaumCell (rkey, this.raum);
				//cell = new RaumCell (this.raum);
			} else {
				//Console.WriteLine ("Raum: " + this.raum.Id);
				cell.BindDataToCell (this.raum);
			}
			return cell;
		}
		
		protected override NSString CellKey {
			get {
				return rkey;
			}
		}
		
		public override void Selected (DialogViewController dvc, UITableView tableView, MonoTouch.Foundation.NSIndexPath path)
		{
			//dvc.ActivateController
			RaumSelected.RaiseEvent(this, new EventArgs<Raum> (this.raum)) ;
			tableView.DeselectRow (path, true);
		}

		#region IElementSizing implementation
		float IElementSizing.GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return RaumCell.GetCellHeight (tableView.Bounds, this.raum);
		}
		#endregion
		
		
	}
}

