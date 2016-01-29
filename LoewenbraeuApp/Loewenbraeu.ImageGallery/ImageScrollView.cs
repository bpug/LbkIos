using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using System.Threading;

namespace Loewenbraeu.ImageGallery
{
	public class ImageScrollView : UIScrollView
	{
		UIImageView imageView;
		public int index {get;set;}
		
		public ImageScrollView (RectangleF frame) : base (frame)
		{
			ShowsVerticalScrollIndicator = false;
			
			
			
			
			ShowsHorizontalScrollIndicator = false;
			BouncesZoom = true;
			DecelerationRate = 0.990f; //UIScrollViewDecelerationRateFast;
			this.ScrollEnabled = false;
			//this.BackgroundColor = UIColor.Red;
			
			#region UIScrollView delegate methods
			ViewForZoomingInScrollView = 
				delegate (UIScrollView scrollView)
				{
					return imageView;
				};
			#endregion
			
		}
		
		#region Override layoutSubviews to center content
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			//Console.WriteLine ("Bounds: " + this.Bounds.Size);
			
			// center the image as it becomes smaller than the size of the screen
			var boundsSize = this.Bounds.Size;
			var frameToCenter = imageView.Frame;
			
			// center horizontally
			if (frameToCenter.Size.Width < boundsSize.Width)
				frameToCenter.X = (boundsSize.Width - frameToCenter.Size.Width) / 2;
			else
				frameToCenter.X = 0;
			
			// center vertically
			if (frameToCenter.Size.Height < boundsSize.Height)
				frameToCenter.Y = (boundsSize.Height - frameToCenter.Size.Height) / 2;
			else
				frameToCenter.Y = 0;
			
			imageView.Frame = frameToCenter;
		}
		#endregion
		
		#region Configure scrollView to display new image (tiled or not)
		public void DisplayImage (UIImage image)
		{
			// clear the previous imageView
			if (imageView != null)
			{
				imageView.RemoveFromSuperview();
				imageView.Dispose();
				imageView = null; // Not sure we need this [CD]
			}
			// reset our zoomScale to 1.0 before doing any further calculations
			this.ZoomScale = 1.0f;


			
			// make a new UIImageView for the new image
			imageView = new UIImageView (image);
			this.AddSubview (imageView);
			
			this.ContentSize = image.Size;
			this.setMaxMinZoomScalesForCurrentBounds();
			this.ZoomScale = this.MinimumZoomScale;
		}

		
		public void setMaxMinZoomScalesForCurrentBounds()
		{
			var boundsSize = this.Bounds.Size;
			var imageSize = imageView.Bounds.Size;
			
			float xScale = boundsSize.Width / imageSize.Width;    // the scale needed to perfectly fit the image width-wise
			float yScale = boundsSize.Height / imageSize.Height;  // the scale needed to perfectly fit the image height-wise
			float minScale = Math.Min(xScale, yScale);            // use minimum of these to allow the image to become fully visible
    
		    // on high resolution screens we have double the pixel density, so we will be seeing every pixel if we limit the
		    // maximum zoom scale to 0.5.
			float maxScale = 1.0f / UIScreen.MainScreen.Scale; // beware pre-iOS4 [CD]
			
			// don't let minScale exceed maxScale. (If the image is smaller than the screen, we don't want to force it to be zoomed.) 
		    if (minScale > maxScale)
			{
		        minScale = maxScale;
		    }
		    
		    this.MaximumZoomScale = maxScale;
		    this.MinimumZoomScale = minScale;
		}
		#endregion
		
		#region Methods called during rotation to preserve the zoomScale and the visible portion of the image
		// returns the center point, in image coordinate space, to try to restore after rotation. 
		public PointF pointToCenterAfterRotation()
		{
			var boundsCenter = new PointF(this.Bounds.GetMidX(), this.Bounds.GetMidY());
			return this.ConvertPointToView (boundsCenter, imageView);
		}
		
		// returns the zoom scale to attempt to restore after rotation. 
		public float scaleToRestoreAfterRotation ()
		{
			var contentScale = this.ZoomScale;
			
			// If we're at the minimum zoom scale, preserve that by returning 0, which will be converted to the minimum
		    // allowable scale when the scale is restored.
		    if (contentScale <= this.MinimumZoomScale + float.Epsilon)
		        contentScale = 0;
		    
		    return contentScale;
		}
		
		PointF maximumContentOffset 
		{
			get
			{
				var contentSize = this.ContentSize;
				var boundsSize = this.Bounds.Size;
				return new PointF(contentSize.Width - boundsSize.Width, contentSize.Height - boundsSize.Height);
			}
		}
		PointF minimumContentOffset
		{
			get
			{
				return new PointF(0,0); //PointF.Empty; // zero? [CD]
			}
		}
		
		// Adjusts content offset and scale to try to preserve the old zoomscale and center.
		public void restoreCenterPoint (PointF oldCenter, float oldScale)
		{
			 // Step 1: restore zoom scale, first making sure it is within the allowable range.
		    this.ZoomScale = Math.Min(this.MaximumZoomScale, Math.Max(this.MinimumZoomScale, oldScale));
		    
		    
		    // Step 2: restore center point, first making sure it is within the allowable range.
		    
		    // 2a: convert our desired center point back to our own coordinate space
		    var boundsCenter = this.ConvertPointFromView(oldCenter,imageView);
		    // 2b: calculate the content offset that would yield that center point
		    var offset = new PointF (boundsCenter.X - this.Bounds.Size.Width / 2.0f, 
		                                 boundsCenter.Y - this.Bounds.Size.Height / 2.0f);
		    // 2c: restore offset, adjusted to be within the allowable range
		    var maxOffset = this.maximumContentOffset;
		    var minOffset = this.minimumContentOffset;
		    offset.X = Math.Max(minOffset.X, Math.Min(maxOffset.X, offset.X));
		    offset.Y = Math.Max(minOffset.Y, Math.Min(maxOffset.Y, offset.Y));
		    this.ContentOffset = offset;
		}
		#endregion
		
		public override string ToString ()
		{
			return string.Format ("[ImageScrollView: index={0}, frame={1}]", index, Frame);
		}
	}
}



