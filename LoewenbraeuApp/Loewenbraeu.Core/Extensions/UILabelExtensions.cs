using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

namespace Loewenbraeu.Core.Extensions
{
	public static class UILabelExtensions
	{
		public static float GetHeight4Text (this UILabel source)
		{
			return GetHeight4Text (source, source.Frame.Width);
		}
		
		public static float GetHeight4Text (this UILabel source, float width)
		{
			if (string.IsNullOrEmpty (source.Text)) {
				return 0;
			}
			using (var nss = new NSString(source.Text)) {
				float height = nss.StringSize (source.Font, new SizeF (width, 1000.0f), 
			                      UILineBreakMode.WordWrap).Height;
				return height;
			}
			
		}
		
		public static void SetLabelHeight4Text (this UILabel source)
		{
			SetLabelHeight4Text (source, source.Frame.Width);
		}
		
		public static void SetLabelHeight4Text (this UILabel source, float width)
		{
			float height = 0f;
			using (var nss = new  NSString (source.Text)) {
				height = nss.StringSize (source.Font, new SizeF (width, 1000.0f), 
			                      UILineBreakMode.WordWrap).Height;
			}
			
			source.Lines = 0;
			source.Frame = new RectangleF (source.Frame.X, source.Frame.Y, width, height);
		}
	}
}

