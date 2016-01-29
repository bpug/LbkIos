using System.Drawing;
using MonoTouch.UIKit;

namespace Loewenbraeu.ImageGallery
{

    public class UIBarLabelItem : UIBarButtonItem
    {
        private UILabel _label;

        public UIBarLabelItem (string title, float width)
            : base()
		{
			_label = new UILabel (new RectangleF (0, 0, width, 23));
			_label.Text = title;
			_label.BackgroundColor = UIColor.Clear;
			_label.Opaque = false;
			_label.TextColor = UIColor.White;
			_label.Alpha = 1;
			_label.TextAlignment = UITextAlignment.Center;
			_label.Font = UIFont.SystemFontOfSize (13.0f);
			_label.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
			
            //Multiline
			_label.LineBreakMode = UILineBreakMode.WordWrap;
			_label.Lines = 0;
			UIView labelContainer = new UIView (new RectangleF (0, 0, width, 24));
			labelContainer.AddSubview (_label);
			labelContainer.BackgroundColor = UIColor.Clear;
			labelContainer.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
            this.Style = UIBarButtonItemStyle.Plain;
            this.CustomView = labelContainer;
            //self = [self initWithCustomView:labelContainer];
        }

        public UIBarLabelItem(string title)
            : this(title, 320)
        {

        }

        public string Text
        {
            get
            {
                if (_label != null)
                    return _label.Text;
                return string.Empty;
            }
            set
            {
                if (_label != null)
                    _label.Text = value;
            }
        }
    }


}