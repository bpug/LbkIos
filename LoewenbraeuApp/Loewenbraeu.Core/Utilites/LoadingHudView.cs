using System;
using System.Drawing;
using Loewenbraeu.Core.Extensions;
using MonoTouch.UIKit;

namespace Loewenbraeu.Core
{
	public class LoadingHUDView : UIView
	{
	
		public static int WIDTH_MARGIN = 20;
		public static int HEIGHT_MARGIN = 20;
		private string _title, _message;
		private UIActivityIndicatorView _activity;
		private bool _hidden;
		private UIFont titleFont = UIFont.BoldSystemFontOfSize (16);
		private UIFont messageFont = UIFont.SystemFontOfSize (13);
		private UIColor _hudBackgroundColor = UIColor.FromRGBA (0.0f, 0.0f, 0.0f, 0.75f);
	
		public string Title {
			get { return _title; }
			set {
				_title = value; 
				this.SetNeedsDisplay ();
			}
		}
	
		public string Message {
			get { return _message; }
			set {
				_message = value; 
				this.SetNeedsDisplay ();
			}
		}
		
		public bool ShowRoundedRectangle {
			get;
			set;
		}
		
		public UIColor HudBackgroundColor {
			get { return _hudBackgroundColor;}
			set {_hudBackgroundColor = value;}
		}
	
		public LoadingHUDView (string title, string message) : base()
		{
			Title = title;
			Message = message;
			_activity = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.WhiteLarge);
			_hidden = true;
			
			this.Frame = new RectangleF (0, 0, 320, 460);
			this.AutoresizingMask = UIViewAutoresizing.FlexibleMargins ;
			this.AutosizesSubviews = false;
			this.BackgroundColor = UIColor.Clear;
			
			this.AddSubview (_activity);
		}
	
		public LoadingHUDView (string title) : this(title, null)
		{
		}
		
		public LoadingHUDView () : this( null)
		{
		}
	
		public void StartAnimating ()
		{
			if (!_hidden)
				return;
	
			_hidden = false;
			this.SetNeedsDisplay ();
			this.Superview.BringSubviewToFront (this);
			_activity.StartAnimating ();
		}
	
		public void StopAnimating ()
		{
			if (_hidden)
				return;
			
			_hidden = true;
			this.SetNeedsDisplay ();
			this.Superview.SendSubviewToBack (this);
			_activity.StopAnimating ();
		}
		
		/*
		protected void AdjustHeight ()
		{
			SizeF titleSize = calculateHeightOfTextForWidth (_title, titleFont, 200, UILineBreakMode.TailTruncation);
			SizeF messageSize = calculateHeightOfTextForWidth (_message, messageFont, 200, UILineBreakMode.WordWrap);
	
			var textHeight = titleSize.Height + messageSize.Height;
			
			RectangleF r = this.Frame;
			r.Size = new SizeF (300, textHeight + 20);
			this.Frame = r;
		}
		*/
	
		public override void Draw (RectangleF rect)
		{
			if (_hidden)
				return;
	
			float width, rWidth, rHeight, x, y;
			SizeF titleSize = calculateHeightOfTextForWidth (_title, titleFont, 200, UILineBreakMode.TailTruncation);
			SizeF messageSize = calculateHeightOfTextForWidth (_message, messageFont, 200, UILineBreakMode.WordWrap);
	
			if (_title == null || _title.Length < 1)
				titleSize.Height = 0;
			if (_message == null || _message.Length < 1)
				messageSize.Height = 0;
			
			rHeight = (int)(titleSize.Height + HEIGHT_MARGIN * 2 + _activity.Frame.Size.Height);
			rHeight += (int)(messageSize.Height > 0 ? messageSize.Height + 10 : 0);
			rWidth = width = (int)Math.Max (titleSize.Width, messageSize.Width);
			rWidth += (rWidth >0 )? WIDTH_MARGIN * 2 : rHeight;
			
			x = (this.Bounds.Width - rWidth) / 2;
			y = (this.Bounds.Height - rHeight) / 2;
	
			//_activity.Center = new PointF (this.Bounds.Width / 2, HEIGHT_MARGIN + 120 + _activity.Frame.Size.Height / 2);
			_activity.Center = new PointF (this.Bounds.Width / 2, y + _activity.Frame.Size.Height / 2 + HEIGHT_MARGIN);
	
			// Rounded rectangle
			if (ShowRoundedRectangle) {
				//RectangleF areaRect = new RectangleF (x, 100 + HEIGHT_MARGIN, rWidth, rHeight);
				RectangleF areaRect = new RectangleF (x, y, rWidth, rHeight);
				this.DrawRoundRectangle(areaRect, 8, _hudBackgroundColor); // alpha = 0.75
			}
			
			// Title
			UIColor.White.SetColor ();
			/*var textRect = new RectangleF (x + WIDTH_MARGIN, 95 + _activity.Frame.Size.Height + 25 + HEIGHT_MARGIN,
				width, titleSize.Height);*/
			var textRect = new RectangleF (x + WIDTH_MARGIN, _activity.Center.Y + _activity.Frame.Size.Height / 2 + 5,
				width, titleSize.Height);
			SizeF titleDrawSize;
			if (_title != null)
				 titleDrawSize = this.DrawString (_title, textRect, titleFont, UILineBreakMode.TailTruncation, UITextAlignment.Center);
	
			// Description
			UIColor.White.SetColor ();
			textRect.Y += titleDrawSize.Height + 10;
			textRect = new RectangleF (textRect.Location, new SizeF (textRect.Size.Width, messageSize.Height));
			
			if (_message != null)
				this.DrawString (_message, textRect, messageFont, UILineBreakMode.WordWrap, UITextAlignment.Center);
		}
	
		protected SizeF calculateHeightOfTextForWidth (string text, UIFont font, float width, UILineBreakMode lineBreakMode)
		{
			return text == null ? new SizeF (0, 0) : this.StringSize (text, font, new SizeF (width, 300), lineBreakMode);
		}
	}
}

