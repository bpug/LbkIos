using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.UrlImageStore;

namespace MonoTouch.ImageGallery
{
	public class IGView<TKey> : UIView, IUrlImageUpdated<TKey>
	{
		public delegate void ImageTappedDelegate (int imageIndex);

		public event ImageTappedDelegate ImageTapped;

		private UIScrollView scrollView;
		private const float ThumbnailSize = 75f;
		public const float ImageWidth = 75f;
		public const float ImageHeight = 75f;
		public const float Padding = 4f;
		
		public IGView (List<IGImage<TKey>> images, UrlImageStore<TKey> imageStore, UrlImageStore<TKey> thumbnailImageStore) : base()
		{
			this.Images = images;
			this.ImageStore = imageStore;
			this.ThumbnailImageStore = thumbnailImageStore;
		}
		
		public IGView (RectangleF frame, List<IGImage<TKey>> images, UrlImageStore<TKey> imageStore, UrlImageStore<TKey> thumbnailImageStore) : base(frame)
		{
			this.Images = images;
			this.ImageStore = imageStore;
			this.ThumbnailImageStore = thumbnailImageStore;
		}
	
		public List<IGImage<TKey>> Images {
			get;
			set;	
		}
		
		public UrlImageStore<TKey> ImageStore {
			get;
			set;	
		}
		
		public UrlImageStore<TKey> ThumbnailImageStore {
			get;
			set;	
		}
		
		public void UrlImageUpdated (TKey id, bool local)
		{
			
			int imageIndex = Images.FindIndex (p => EqualityComparer<TKey>.Default.Equals (p.Id, id));
			this.BeginInvokeOnMainThread (delegate {
				
				var image = this.ThumbnailImageStore.GetImage (this.Images [imageIndex].Id);
				foreach (var view in scrollView.Subviews) {
					var button = view as UIButton;
					if (button != null && button.Tag == imageIndex) {
						var imageView = button.Subviews [0] as UIImageView;
						var thumbnail = ScaleImage (image);
						ShowImage (imageView, thumbnail, !local);
					}
				}
			});
			
			SetNeedsDisplay ();
		}
		
		private void ShowImage (UIImageView imageView, UIImage image, bool animated)
		{
			if (!animated) {
				imageView.Image = image;
			} else {
				UIView.BeginAnimations ("imageout");
				UIView.SetAnimationDuration (0.5f);
				imageView.Alpha = 0.2f;
				UIView.CommitAnimations ();
						
				imageView.Image = image;
						
				UIView.BeginAnimations ("imagein");
				UIView.SetAnimationDuration (1.5f);
				UIView.SetAnimationCurve (UIViewAnimationCurve.EaseIn);
				UIView.SetAnimationTransition (UIViewAnimationTransition.FlipFromRight, imageView, true);
				imageView.Alpha = 1.0f;
				UIView.CommitAnimations ();
			}
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			CreateScrollView ();
			CreateButtons ();
			
		}

		private void CreateScrollView ()
		{
			scrollView = new UIScrollView (new RectangleF (0, 0, 320, 440));
			scrollView.PagingEnabled = true;
			scrollView.ShowsHorizontalScrollIndicator = false;
			scrollView.ShowsVerticalScrollIndicator = false;
			scrollView.ScrollsToTop = true;
			//scrollView.Delegate = this;
			scrollView.ContentSize = new SizeF (320 * ((Images.Count - 1) / 25 + 1), 440);
			
			scrollView.BackgroundColor = UIColor.Black;
			this.AddSubview (scrollView);
		
		}
		
		private void CreateButtons ()
		{
			int i = 0;
			
			foreach (var img in Images) {
				{
					var image = this.ThumbnailImageStore.RequestImage (img.Id, img.Url, this);
					image = ScaleImage (image);
					var thumbNailButton = UIButton.FromType (UIButtonType.Custom);
//					float x = 6 + 62 * (i % 5) + 320 * (i / 25);
//					float y = 5 + 65 * ((i / 5) % 5);
					float x = 2 + 80 * (i % 4) + 320 * (i / 16);
					float y = 5 + 80 * ((i / 4) % 4);
					thumbNailButton.Frame = new RectangleF (x, y, ThumbnailSize, ThumbnailSize);

					var imgView = new UIImageView (new RectangleF (0, 0, ThumbnailSize, ThumbnailSize));
					imgView.Image = image;
					
					thumbNailButton.Tag = i;
					thumbNailButton.TouchUpInside += (sender, e) => 
					{
						if (this.ImageTapped != null) {
							var button = sender as UIButton;
							this.ImageTapped (button.Tag);
						}
						
					};
      
					thumbNailButton.AddSubview (imgView);
					scrollView.AddSubview (thumbNailButton);
					i++;
				}

			}	
		}
		
		private UIImage ScaleImage (UIImage image)
		{
			var newImage = Graphics.Scale (image, ThumbnailSize);
			return newImage;
		}
		/*
		
		public override void Draw (RectangleF rect)
		{
			DateTime start = DateTime.Now;
			base.Draw (rect);
			
			var context = UIGraphics.GetCurrentContext ();
			
			UIColor.White.SetFill ();
			context.FillRect (rect);
			
			int across = 4;
			
			if (rect.Width > 320f)
				across = 6;
			
			int row = 0;
			int col = 0;
			//int rows = (int)Math.Ceiling((double)this.Images.Count / (double)across);
			
			
			foreach (var img in Images) {
				float x = col * ImageWidth + col * Padding + Padding;
				float y = row * ImageHeight + row * Padding + Padding;
				//draw image
				var imgRect = new RectangleF (x, y, ImageWidth, ImageHeight);
				
				this.ThumbnailImageStore.RequestImage (img.Id, img.Url, this).Draw (imgRect);
				
				if (touchingImage != null && img.Id.Equals (touchingImage.Id)) {
					UIColor.LightGray.SetStroke ();
					context.StrokeRectWithWidth (imgRect, 3.0f);
				}
				
				col++;
				
				if (col >= across) {
					row++;
					col = 0;
				}
			}
			
			TimeSpan len = DateTime.Now - start;
			
			Console.WriteLine ("Draw Time: " + len.TotalMilliseconds);
		}
		
		IGImage<TKey> touchingImage = null;
		RectangleF touchingImageRect = RectangleF.Empty;
		int touchingImageIndex = -1;
		
		public override void TouchesBegan (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			
			var touchLocation =	((UITouch)evt.TouchesForView(this).AnyObject).LocationInView(this);
			var touchFrame = new RectangleF(touchLocation.X, touchLocation.Y, 1, 1);
			
			int across = this.Bounds.Size.Width > 320 ? 6 : 4;
			int row = 0;
			int col = 0;
			
			int imgOn = 0;
			foreach (var img in Images)
			{
				float x = col * ImageWidth + col * Padding + Padding;
				float y = row * ImageHeight + row * Padding + Padding;
				var imgRect = new RectangleF(x, y, ImageWidth, ImageHeight);
				
				if (touchFrame.IntersectsWith(imgRect))
				{
					touchingImage = img;
					touchingImageRect = imgRect;
					touchingImageIndex = imgOn;
					SetNeedsDisplay();
					break;
				}
				
				col++;
				if (col >= across)
				{
					row++;
					col = 0;
				}
				imgOn++;
			}
		}
		
		public override void TouchesCancelled (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
			
			touchingImage = null;
			touchingImageRect = RectangleF.Empty;	
			
			SetNeedsDisplay();
		}
		
		
		public override void TouchesMoved (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);
		} 
		public override void TouchesEnded (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
						
			if (touchingImage == null || touchingImageRect == RectangleF.Empty)
			{
				touchingImage = null;
				touchingImageRect = RectangleF.Empty;
				touchingImageIndex = -1;
				//base.TouchesEnded(touches, evt);
				return;
			}
			
			var touchLocation =	((UITouch)evt.TouchesForView(this).AnyObject).LocationInView(this);
			var touchFrame = new RectangleF(touchLocation.X, touchLocation.Y, 1, 1);
			
			if (touchFrame.IntersectsWith(touchingImageRect))
			{
				int touchedImageIndex = touchingImageIndex;
				
				touchingImage = null;
				touchingImageRect = RectangleF.Empty;
				touchingImageIndex = -1;
				
				//Fire off delegate
				if (this.ImageTapped != null)
					this.ImageTapped(touchedImageIndex);
			}
			
			SetNeedsDisplay();
			
		}
		*/
		
	
	}
}

