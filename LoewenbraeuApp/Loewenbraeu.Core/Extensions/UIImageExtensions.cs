using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace Loewenbraeu.Core.Extensions
{
	public static class UIImageExtensions
	{
		const float M_PI = 3.141592653589793238462643f;
		
		// Child proof the image by rounding the edges of the image
		public static UIImage RoundCorners (this UIImage source)
		{
			if (source == null)
				throw new ArgumentNullException ("image");
			
			UIGraphics.BeginImageContext (source.Size);
			float imgWidth = source.Size.Width;
			float imgHeight = source.Size.Height;

			var c = UIGraphics.GetCurrentContext ();

			c.BeginPath ();
			c.MoveTo (imgWidth, imgHeight / 2);
			c.AddArcToPoint (imgWidth, imgHeight, imgWidth / 2, imgHeight, 4);
			c.AddArcToPoint (0, imgHeight, 0, imgHeight / 2, 4);
			c.AddArcToPoint (0, 0, imgWidth / 2, 0, 4);
			c.AddArcToPoint (imgWidth, 0, imgWidth, imgHeight / 2, 4);
			c.ClosePath ();
			c.Clip ();

			source.Draw (new PointF (0, 0));
			var converted = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return converted;
		}
		
		public static UIImage Scale2 (this UIImage source, float maxWidthAndHeight)
		{
			//Perform Image manipulation, make the source fit into a 48x48 tile without clipping.  
			
			float fWidth = source.Size.Width;
			float fHeight = source.Size.Height;
			float fTotal = fWidth >= fHeight ? fWidth : fHeight;
			float fDifPercent = maxWidthAndHeight / fTotal;
			float fNewWidth = fWidth * fDifPercent;
			float fNewHeight = fHeight * fDifPercent;
			
			SizeF newSize = new SizeF (fNewWidth, fNewHeight);
			
			UIGraphics.BeginImageContext (newSize);
			var context = UIGraphics.GetCurrentContext ();
			context.TranslateCTM (0, newSize.Height);
			context.ScaleCTM (1f, -1f);
	
			context.DrawImage (new RectangleF (0, 0, newSize.Width, newSize.Height), source.CGImage);
	
			var scaledImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
	
			return scaledImage;
		}
		
		public static UIImage Thumbnail (this UIImage source, SizeF newSize)
		{
			SizeF size = source.Size; 
			float offsetY = 0;
			float offsetX = 0;
			SizeF croppedSize; 
			
			RectangleF rect = new RectangleF (0, 0, newSize.Width, newSize.Height);

			// check the size of the image, we want to make it
			// a square with sides the size of the smallest dimension
			if (size.Width > size.Height) {
				offsetX = (size.Height - size.Width) / 2;
				croppedSize = new SizeF (size.Height, size.Height);
			} else {
				offsetY = (size.Width - size.Height) / 2;
				croppedSize = new SizeF (size.Width, size.Width);
			}

			// Crop the image before resize
			RectangleF clippedRect = new RectangleF (offsetX * -1, offsetY * -1, croppedSize.Width, croppedSize.Height);

			UIImage cropped = UIImage.FromImage (source.CGImage.WithImageInRect (clippedRect));
			// Done cropping

			UIGraphics.BeginImageContext (newSize);
			var ctx = UIGraphics.GetCurrentContext ();

			ctx.DrawImage (rect, cropped.CGImage);

			var ret = UIGraphics.GetImageFromCurrentImageContext ();
			
			//Flip
			ctx.DrawImage (rect, ret.CGImage);
			ctx.RotateCTM (M_PI);
			ret = UIGraphics.GetImageFromCurrentImageContext ();
			
			UIGraphics.EndImageContext ();

			//ret = ret.FlipImage (newSize);
			return ret;

		}
		
		public static UIImage FlipImage (this UIImage source, SizeF size)
		{
			UIGraphics.BeginImageContext (size);
			var ctx = UIGraphics.GetCurrentContext ();
			RectangleF rect = new RectangleF (0, 0, size.Width, size.Height);

			ctx.DrawImage (rect, source.CGImage);
			ctx.RotateCTM (M_PI);

			var ret = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return ret;
		}
		
		public static UIImage ScaleImage (this UIImage source, int maxSize)
		{

			UIImage res;

			using (CGImage imageRef = source.CGImage) {
				CGImageAlphaInfo alphaInfo = imageRef.AlphaInfo;
				CGColorSpace colorSpaceInfo = CGColorSpace.CreateDeviceRGB ();
				if (alphaInfo == CGImageAlphaInfo.None) {
					alphaInfo = CGImageAlphaInfo.NoneSkipLast;
				}

				int width, height;

				width = imageRef.Width;
				height = imageRef.Height;


				if (height >= width) {
					width = (int)Math.Floor ((double)width * ((double)maxSize / (double)height));
					height = maxSize;
				} else {
					height = (int)Math.Floor ((double)height * ((double)maxSize / (double)width));
					width = maxSize;
				}


				CGBitmapContext bitmap;

				if (source.Orientation == UIImageOrientation.Up || source.Orientation == UIImageOrientation.Down) {
					bitmap = new CGBitmapContext (IntPtr.Zero, width, height, imageRef.BitsPerComponent, imageRef.BytesPerRow, colorSpaceInfo, alphaInfo);
				} else {
					bitmap = new CGBitmapContext (IntPtr.Zero, height, width, imageRef.BitsPerComponent, imageRef.BytesPerRow, colorSpaceInfo, alphaInfo);
				}

				switch (source.Orientation) {
				case UIImageOrientation.Left:
					bitmap.RotateCTM ((float)Math.PI / 2);
					bitmap.TranslateCTM (0, -height);
					break;
				case UIImageOrientation.Right:
					bitmap.RotateCTM (-((float)Math.PI / 2));
					bitmap.TranslateCTM (-width, 0);
					break;
				case UIImageOrientation.Up:
					break;
				case UIImageOrientation.Down:
					bitmap.TranslateCTM (width, height);
					bitmap.RotateCTM (-(float)Math.PI);
					break;
				}

				bitmap.DrawImage (new Rectangle (0, 0, width, height), imageRef);


				res = UIImage.FromImage (bitmap.ToImage ());
				bitmap = null;

			}


			return res;
		}
		
		public static UIImage ImageToFitSize (this UIImage image, SizeF fitSize)
		{
			double imageScaleFactor = 1.0;
			imageScaleFactor = image.CurrentScale;
 
			double sourceWidth = image.Size.Width * imageScaleFactor;
			double sourceHeight = image.Size.Height * imageScaleFactor;
			double targetWidth = fitSize.Width;
			double targetHeight = fitSize.Height;
 
			double sourceRatio = sourceWidth / sourceHeight;
			double targetRatio = targetWidth / targetHeight;
 
			bool scaleWidth = (sourceRatio <= targetRatio);
			scaleWidth = !scaleWidth;
 
			double scalingFactor, scaledWidth, scaledHeight;
 
			if (scaleWidth) {
				scalingFactor = 1.0 / sourceRatio;
				scaledWidth = targetWidth;
				scaledHeight = Math.Round (targetWidth * scalingFactor);
			} else {
				scalingFactor = sourceRatio;
				scaledWidth = Math.Round (targetHeight * scalingFactor);
				scaledHeight = targetHeight;
			}
 
			RectangleF destRect = new RectangleF (0, 0, (float)scaledWidth, (float)scaledHeight);
 
			UIGraphics.BeginImageContextWithOptions (destRect.Size, false, 0.0f);
			image.Draw (destRect); 
			UIImage newImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
 
			return newImage;
		}
	}
}

