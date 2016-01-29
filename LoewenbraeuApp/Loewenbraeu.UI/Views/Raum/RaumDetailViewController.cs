using System.Linq;
using System.Drawing;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Model;
using MonoTouch.Dialog.Utilities;
using MonoTouch.UIKit;
using System;


namespace Loewenbraeu.UI
{
	public partial class RaumDetailViewController : UIViewController
	{
		const int X_OFFSET = 10;
		const int TOP_OFFSET = 10;
		const int X_INNER_OFFSET = 8;
		const float IMAGE_OFFSET = 5;
		const float TEXT_SIZE = 14;
		
		static UIFont titleFont = UIFont.SystemFontOfSize (TEXT_SIZE);
		static UIFont subTitleFont = UIFont.BoldSystemFontOfSize (TEXT_SIZE);
		
		private RoundedRectView _viewContainer;
		private UILabel lblSubtitle;
		private UILabel lblDescription;
		private UILabel lblSeats;
		private UILabel lblArea;
		private UIImageView imgViewBackground;
		//private UIImageView imgViewRaum;
		
		private Raum _raum;
		
		private UIPageControl _pageControl;
		private UIScrollView _scrollView;
		
		public RaumDetailViewController (Raum raum) : base ()
		{
			_raum = raum;
			Initialize ();
		}
		
		private void Initialize ()
		{
			Title = _raum.Title;
			
			lblSubtitle = new UILabel (){
				Font = subTitleFont,
				TextColor = Resources.Colors.Background,
				Lines = 0,
			};
			
			lblDescription = new UILabel (){
				Font = titleFont,
				TextColor = Resources.Colors.Background,
				Lines = 0,
			};
			
			lblSeats = new UILabel (){
				Font = titleFont,
				TextColor = Resources.Colors.Background,
				Lines = 0,
			};
			
			lblArea = new UILabel (){
				Font = titleFont,
				TextColor = Resources.Colors.Background,
				Lines = 0,
			};
			
			imgViewBackground = new UIImageView ();
			/*
			imgViewRaum = new UIImageView (){
				BackgroundColor = UIColor.Clear,
			};
			*/
			
			_pageControl = new UIStyledPageControl{
				HidesForSinglePage = true,
			//Pages = 0,
            };
			_pageControl.ValueChanged += HandlePageControlValueChanged;
			
			_scrollView = new UIScrollView (){
				ShowsHorizontalScrollIndicator = false,
				ShowsVerticalScrollIndicator = false,
				Bounces = true,
				PagingEnabled = true,
			};
			_scrollView.DecelerationEnded += HandleScrollViewDecelerationEnded;
			
		}
		
		
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			float y = 0;
			float textWidth = 0;
			float textHeight = 0;
			
			_viewContainer = new RoundedRectView (
				new RectangleF (X_OFFSET, TOP_OFFSET, View.Bounds.Width - 2 * X_OFFSET, View.Bounds.Height - 80), UIColor.White, 8);
			
			textWidth = _viewContainer.Frame.Width - 2 * X_INNER_OFFSET;
			
			imgViewBackground.Frame = new RectangleF (IMAGE_OFFSET, IMAGE_OFFSET, _viewContainer.Bounds.Width - IMAGE_OFFSET, 203);
			var bgColor = UIColor.FromPatternImage (UIImage.FromBundle ("image/raueme/raum-HG.png"));
			imgViewBackground.BackgroundColor = bgColor;
			
			//Image
			//imgViewRaum.Frame = new RectangleF (IMAGE_OFFSET + 5, IMAGE_OFFSET + 5, _viewContainer.Bounds.Width - 4 * IMAGE_OFFSET, 187);
			//imgViewRaum.Image = UIImage.FromBundle (_raum.ImageUrl.FirstOrDefault ()); //ImageLoader.DefaultRequestImage (_raum.ImageUri, null);
			
			//y += TOP_OFFSET + imgViewBackground.Frame.Height;
			
			y += imgViewBackground.Frame.Height;
			
			var svFrame = new RectangleF (IMAGE_OFFSET + 5, IMAGE_OFFSET + 5, _viewContainer.Bounds.Width - 4 * IMAGE_OFFSET, 187);
			var svSize = new SizeF (_viewContainer.Bounds.Width - 4 * IMAGE_OFFSET, 187);
			
			_scrollView.Frame = svFrame;
			
			int i = 0;
			foreach (var imageUrl in _raum.ImageUrl) {
				var imgView = new UIImageView ();
				imgView.Frame = new RectangleF (svSize.Width * i, 0, svSize.Width, svSize.Height);
				imgView.Image = UIImage.FromBundle (imageUrl);
				_scrollView.AddSubview (imgView);
				i++;
			}
			
			_scrollView.ContentSize = new SizeF (svSize.Width * (i == 0 ? 1 : i), svSize.Height);
			_pageControl.Pages = i;
			_pageControl.CurrentPage = 0;
			_pageControl.Frame = new RectangleF (0, y, 320, 15);
			
			
			y += TOP_OFFSET + 5;
			
			lblSubtitle.Text = _raum.Subtitle;
			textHeight = lblSubtitle.GetHeight4Text (textWidth);
			lblSubtitle.Frame = new RectangleF (X_INNER_OFFSET, y, textWidth, textHeight);
			
			y += TOP_OFFSET + textHeight;
			
			lblDescription.Text = _raum.Description;
			textHeight = lblDescription.GetHeight4Text (textWidth);
			lblDescription.Frame = new RectangleF (X_INNER_OFFSET, y, textWidth, textHeight);
			
			y += 2 * TOP_OFFSET + textHeight;
			if (!string.IsNullOrEmpty (_raum.Seats)) {
				lblSeats.Text = Locale.Format ("Plätze:{0}", _raum.Seats.Indent (8));
				textHeight = lblSeats.GetHeight4Text (textWidth);
				lblSeats.Frame = new RectangleF (X_INNER_OFFSET, y, textWidth, textHeight);
				y += textHeight;
			}
			if (_raum.Area.HasValue) {
				lblArea.Text = Locale.Format ("Nutzfläche: {0:0 m²} ", _raum.Area); //
				textHeight = lblArea.GetHeight4Text (textWidth);
				lblArea.Frame = new RectangleF (X_INNER_OFFSET, y, textWidth, textHeight);
			}
			
			_viewContainer.AddSubview (imgViewBackground);
			//_viewContainer.AddSubview (imgViewRaum);
			_viewContainer.AddSubview (_scrollView);
			_viewContainer.AddSubview (_pageControl);
			_viewContainer.AddSubview (lblSubtitle);
			_viewContainer.AddSubview (lblDescription);
			_viewContainer.AddSubview (lblSeats);
			_viewContainer.AddSubview (lblArea);
			View.AddSubview (_viewContainer);
		}
		
		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown || toInterfaceOrientation == UIInterfaceOrientation.Portrait);
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown;
		}
		
		
		void HandleScrollViewDecelerationEnded (object sender, EventArgs e)
		{
			int page = (int)Math.Floor ((_scrollView.ContentOffset.X - _scrollView.Frame.Width / 2) / _scrollView.Frame.Width) + 1;
			_pageControl.CurrentPage = page;
		}
		
		void HandlePageControlValueChanged (object sender, EventArgs e)
		{
			int page = _pageControl.CurrentPage;
			_pageControl.CurrentPage = page;
			/*
			RectangleF frame = _scrollView.Frame;
			frame.X = frame.Size.Width * page;
			frame.Y = 0;
			_scrollView.ScrollRectToVisible (frame, true);
			*/
			_scrollView.SetContentOffset (new PointF ((page * _scrollView.Frame.Size.Width), 0), true);
		}
		
	}
}

