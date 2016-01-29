using System;
using System.Drawing;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Model;
using MonoTouch.UIKit;


namespace Loewenbraeu.UI
{
	public partial class HistoriePageController : UIViewController
	{
		const int YStart  = 5;
		const int TitleSize = 25;
		const int DateSize = 28;
		const int DescrSize = 15;
		const int MaxPicWidht = 300;
		const int MaxPicHeight = 180;
		const int TextHeightPadding = 4;
		const int TextLeftStart = 5;
		const int TextYOffset = 3;
		
		static UIFont titleFont = UIFont.FromName ("HelveticaNeue-CondensedBlack",TitleSize);
		static UIFont dateFont = UIFont.FromName ("HelveticaNeue-CondensedBlack", DateSize);
		static UIFont descrFont = UIFont.FromName ("HelveticaNeue",DescrSize);
		
		public int PageIndex {
			get;
			private set;
		}
		
		public History History {
			get;
			private set;
		}
		
		public HistoriePageController (IntPtr handle) : base (handle)
		{
			//Console.WriteLine ("HistoriePageController-IntPtr:" + Environment.StackTrace);
			Initialize ();
		}
		
		public HistoriePageController () : base()
		{
			Initialize ();
		}
		
		public HistoriePageController (History history) : this()
		{
			this.History = history;
			this.PageIndex = history.PageIndex;
		}
		/*
		public HistoriePageController (int pageIndex) : this()
		{
			this.PageIndex = pageIndex;
		}
		*/
		private void Initialize ()
		{
			//this._imageView = new UIImageView ();
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			float textWidth;
			float titleHeight;
			float dateHeight;
			float descriptionHeight;
			float centerX;
			float y;
			
			this.View.Frame = new RectangleF (0, 0, 320, 420);
			this.View.BackgroundColor = Resources.Colors.Background;
			
			string imagePath = History.ImageUrl;
			
			textWidth = this.View.Bounds.Width - 2 * TextLeftStart;
			centerX = this.View.Center.X;
			
			var lblTitle = new UILabel ();
			lblTitle.Lines = 0;
			lblTitle.TextColor = UIColor.FromRGB (218, 188, 0);
			lblTitle.Font = titleFont;
			lblTitle.Text = History.Title;
			titleHeight = lblTitle.GetHeight4Text (textWidth);
			y = YStart;
			lblTitle.Frame = new RectangleF (TextLeftStart, y, textWidth, titleHeight);
			lblTitle.TextAlignment = UITextAlignment.Center;
			
			var lblDate = new UILabel ();
			lblDate.Lines = 0;
			lblDate.TextColor = UIColor.White;
			lblDate.Font = dateFont;
			lblDate.Text = History.Date;
			dateHeight = lblDate.GetHeight4Text (textWidth);
			y += TextYOffset + titleHeight;
			lblDate.Frame = new RectangleF (TextLeftStart, y, textWidth, dateHeight);
			lblDate.TextAlignment = UITextAlignment.Center;
			
			
			var lblDescription = new UILabel ();
			lblDescription.Lines = 0;
			lblDescription.TextColor = UIColor.White;
			lblDescription.Font = descrFont;
			lblDescription.Text = History.Description;
			descriptionHeight = lblDescription.GetHeight4Text (textWidth);
			
			var imageView = new UIImageView ();
			y += 2 * TextYOffset + dateHeight;
			
			float maxImageHeight = this.View.Bounds.Height - descriptionHeight - y - 40;
			
			var img = UIImage.FromFile (imagePath);
			
			var resizer = new ImageResizer (img);
			resizer.RatioResize (MaxPicWidht, maxImageHeight);
			var modImage = resizer.ModifiedImage;
			
			var imgX = centerX - modImage.Size.Width / 2;
			
			imageView.Frame = new RectangleF (imgX, y, modImage.Size.Width, modImage.Size.Height);
			//imageView.Image = modImage;
			imageView.Image = img;
			
			y += 2 * TextYOffset + imageView.Bounds.Height;
			lblDescription.Frame = new RectangleF (TextLeftStart, y, textWidth, descriptionHeight);
			lblDescription.TextAlignment = UITextAlignment.Center;
			
			this.View.AddSubview (imageView);
			this.View.AddSubview (lblTitle);
			this.View.AddSubview (lblDate);
			this.View.AddSubview (lblDescription);
			this.View.SetLabelsBGColor (UIColor.Clear);
		}

		/*
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			//ReleaseDesignerOutlets ();
		}
		*/

		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown || toInterfaceOrientation == UIInterfaceOrientation.Portrait);
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown;
		}

		/*
		protected override void Dispose (bool disposing)
		{

			if (disposing) {
				if (_imageView != null) _imageView.Dispose (); _imageView = null;
			}
			base.Dispose (disposing);
		}
		*/
	}
}

