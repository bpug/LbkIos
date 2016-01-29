using System;
using System.Drawing;
using Loewenbraeu.Core.Extensions;
using MonoTouch.Dialog;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using Loewenbraeu.Core;

namespace Loewenbraeu.UI
{
	
	public class LbkDialogViewController : DialogViewController 
	{
		UIImage bgImage;
		
		protected MBProgressHUD Hud;
		
		public UIImage BgImage {
			get {
				return bgImage;
			}
			set {
				bgImage = value;
				LoadView ();
			}
		}
		
		public event EventHandler ViewAppearing;
		
		public LbkDialogViewController (RootElement root, bool pushing) 
	    	: base (root, pushing)
		{
			this.bgImage = UIImage.FromFile ("image/background/background.png");
		}
		
		public LbkDialogViewController (RootElement root, bool pushing, UIImage bgImage) 
	    	: base (root, pushing)
		{
			if (bgImage == null){
				this.bgImage = UIImage.FromFile ("image/background/background.png");
			}else{
				this.bgImage = bgImage;
			}
	    }
		
		public override void LoadView ()
	    {
	        base.LoadView ();
			
			if(bgImage != null)
			{
		        var color = UIColor.FromPatternImage(bgImage);
		        
				if(TableView.RespondsToSelector(new Selector("backgroundView")))
					TableView.BackgroundView = new UIView();
				
				if(ParentViewController != null)
				{
					TableView.BackgroundColor = UIColor.Clear;
					ParentViewController.View.BackgroundColor = color;
				}
				else
				{
					TableView.BackgroundColor = color;
				}
			}
	    }
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (ViewAppearing != null)
				ViewAppearing (this, EventArgs.Empty);
		}
		
		protected void ShowHud ()
		{
			Hud = new MBProgressHUD (Util.MainWindow){
				Mode = MBProgressHUDMode.Indeterminate,
			};
			Util.MainWindow.AddSubview (Hud);
			Hud.Show (true);
		}
		
		protected void HideHud ()
		{
			if (Hud == null)
				return;
			
			Hud.Hide (true);
			Hud.RemoveFromSuperview ();
			Hud = null;
		}
		
		
		public override DialogViewController.Source CreateSizingSource (bool unevenRows)
		{
			return unevenRows ? new LbkSizingSource (this) : new LbkSource (this);
		}
		
		public class LbkSizingSource : LbkSource{
			public LbkSizingSource (DialogViewController dvc) : base (dvc) {}
			
			public override float GetHeightForRow (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				var section = Container.Root [indexPath.Section];
				var element = section.Elements [indexPath.Row];
				
				var sizable = element as IElementSizing;
				if (sizable == null)
					return tableView.RowHeight;
				return sizable.GetHeight (tableView, indexPath);
			}
		}
			
		public class LbkSource : DialogViewController.Source
		{
			private const float HEADER_PADDING =  20;
			
			private static UIFont fontHeader = UIFont.BoldSystemFontOfSize (17f);
			private static UIFont fontFooter = UIFont.SystemFontOfSize (14f);
			
			public LbkSource (DialogViewController dvc) : base (dvc)
			{
			}
			
			public override float GetHeightForHeader (UITableView tableView, int section)
			{
				string caption = Container.Root [section].Caption;
				
				if (string.IsNullOrEmpty (caption)) {
					return base.GetHeightForHeader (tableView, section);
				}
				
				float textHeight = tableView.StringSize (caption, fontHeader, new SizeF (300.0f, 1000.0f), UILineBreakMode.WordWrap).Height;
				return textHeight + HEADER_PADDING;
			}
			
			public override float GetHeightForFooter (UITableView tableView, int section)
			{
				string footer = Container.Root [section].Footer;
				
				if (string.IsNullOrEmpty (footer)) {
					return base.GetHeightForHeader (tableView, section);
				}
				
				float textHeight = tableView.StringSize (footer,fontFooter,new SizeF (300.0f, 1000.0f),UILineBreakMode.WordWrap).Height;
				return textHeight ;
			}
			
			public override UIView GetViewForHeader (UITableView tableView, int sectionIdx)
			{
				var section = Container.Root [sectionIdx];
				if (!string.IsNullOrEmpty (section.Caption)) {
					return GetSectionView (section.Caption, true);
				}
				return section.HeaderView;
			}
			
			public override UIView GetViewForFooter (UITableView tableView, int sectionIdx)
			{
				var section = Container.Root [sectionIdx];
				if (!string.IsNullOrEmpty (section.Footer)) {
					return GetSectionView (section.Footer, false);
				}
				return section.FooterView;
			}
			
			private UIView GetSectionView (string text, bool isHeader)
			{
				if (string.IsNullOrEmpty (text))
					return null;
				
				float viewHeight = 0f;
				float labelHeight = 0f;
				
				UIView view = new UIView ();
				UILabel label = new UILabel ();
				
				if (isHeader) {
					label.Font  = fontHeader;
					label.TextAlignment = UITextAlignment.Left;
					label.ShadowColor = UIColor.Black;
					label.ShadowOffset = new SizeF (0, 1f);
					viewHeight = HEADER_PADDING;
				} else {
					label.Font  = fontFooter;
					label.TextAlignment = UITextAlignment.Center;
					label.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
				}
				
				label.Text = text;
				labelHeight = label.GetHeight4Text (300);
				
				viewHeight += labelHeight;
				view.Frame = new RectangleF (0f, 0f, 300f, viewHeight);
				view.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
				
				label.Frame = isHeader ? new RectangleF (12f, viewHeight - labelHeight - 5, 300f, labelHeight) : new RectangleF (10f, 0, 300f, labelHeight);
				label.Lines = 0;
				label.BackgroundColor = UIColor.Clear;
				label.Opaque = false;
				label.TextColor = UIColor.White;
				
				view.AddSubview (label);
				return view;
			}
		}
	}
}