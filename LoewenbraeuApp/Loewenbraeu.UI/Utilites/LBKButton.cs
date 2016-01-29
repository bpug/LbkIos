using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace Loewenbraeu.UI
{
	public class LBKButton : UIButton
	{
		public LBKButton () : base()
		{
			Initializer ();
		}
		
		public LBKButton (RectangleF frame) : base(frame)
		{
			Initializer ();
		}
		
		private void  Initializer ()
		{
			//Layer.CornerRadius = 12;
			//Layer.MasksToBounds = true;
			//Layer.BackgroundColor = Resources.Colors.Button.CGColor;
			SetTitleColor (Resources.Colors.ButtonTitle, UIControlState.Normal);
			SetTitleColor (UIColor.LightGray, UIControlState.Disabled);
			
			 
			BackgroundColor = UIColor.Clear;
			VerticalAlignment = UIControlContentVerticalAlignment.Center;
			HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			
			var background = UIImage.FromBundle ("image/buttons/lbk_button.png");
			//var backgroundPressed = UIImage.FromFile ("images/blueButton.png");
			var newImage = background.StretchableImage(12, 0); 
			SetBackgroundImage (newImage, UIControlState.Normal);
			//var newPressedImage = backgroundPressed.StretchableImage (12, 0);
			//SetBackgroundImage (newPressedImage, UIControlState.Highlighted);
			
			
		}
		
	}
}

