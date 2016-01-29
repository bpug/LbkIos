using System;
using System.Drawing;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


namespace Loewenbraeu.UI
{
	public abstract class BaseViewController: UIViewController
	{
		NSObject _keyboardObserverWillShow;
		NSObject _keyboardObserverWillHide;
		
		protected MBProgressHUD Hud;
		
		//private UIImageView imageView;
		private BackgroundView _backgroundView;
		private UIImage _backgroundImage;
		
		public UIImage BackgroundImage {
			get {
				return _backgroundImage;
			}
			set {
				_backgroundImage = value;
				LoadView ();
			}
		}
		private MBProgressHUDMode _hudMode = MBProgressHUDMode.Indeterminate;

		protected MBProgressHUDMode HUDMode{
			get { return _hudMode;}
			set { _hudMode = value;}
		}
		
		public BaseViewController () : base ()
		{
			Initialize (null);
		}
		
		public BaseViewController (UIImage bgImage ) : base ()
		{
			Initialize (bgImage);
		}
		
		
		public BaseViewController (IntPtr handle) : base (handle)
		{
			Initialize (null);
		}
		
		public BaseViewController (string nibName, NSBundle bundle) : base(nibName, bundle)
		{
			Initialize (null);
		}
		
		private void Initialize (UIImage bgImage)
		{
			if (bgImage == null) {
				this._backgroundImage = UIImage.FromBundle ("image/background/background.png");
			} else {
				this._backgroundImage = bgImage;
			}
			
		}
		
		public override void LoadView ()
		{
			base.LoadView ();
			
			if (_backgroundImage != null) {
				/*
				this.imageView = new UIImageView (bgImage);
				this.imageView.Frame = new RectangleF (0, 0, this.View.Frame.Size.Width, this.View.Frame.Size.Height);
				this.imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				this.imageView.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin |
				UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleBottomMargin;
				//imageView.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
				View.BackgroundColor = Resources.Colors.Background;
				View.AddSubview (imageView);
				*/
				// new code
				_backgroundView = new BackgroundView (this);
				_backgroundView.Frame = View.Frame;
				_backgroundView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
				_backgroundView.AutosizesSubviews = true;
				_backgroundView.UserInteractionEnabled = true;
				
				//Copy View from Interface designer
				if (View.Subviews != null && View.Subviews.Length > 0) {
					_backgroundView.AddSubviews (View.Subviews);
					foreach (var view in View.Subviews) {
						view.RemoveFromSuperview ();
					}
				}
				_backgroundView.ContentMode = UIViewContentMode.Redraw;

				View = _backgroundView;
				// new code
			}
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.All;
		}
		
		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			_backgroundView.SetNeedsDisplay ();
		}
		
		public override void ViewWillAppear (bool animated)
		{
			//this.imageView.Center = new System.Drawing.PointF (this.View.Bounds.Width / 2, this.View.Bounds.Height / 2);
			base.ViewWillAppear (animated);
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			//UIView container = this.View.Subviews [0];
			//this.View.BringSubviewToFront (container);
			
			// Setup keyboard event handlers
			RegisterForKeyboardNotifications ();
			
		}

		/*
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			UnregisterKeyboardNotifications ();
		}
		*/

		protected override void Dispose (bool disposing)
		{
			UnregisterKeyboardNotifications ();
			base.Dispose (disposing);
		}
		
		protected  void RemoteSubviews(){
			foreach (UIView view in this.View.Subviews) {
				if (!(view is UIImageView)) //Without background
					view.RemoveFromSuperview ();
			}
		}
		
		
		protected virtual void RegisterForKeyboardNotifications ()
		{
			_keyboardObserverWillShow = NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification, KeyboardWillShowNotification);
			_keyboardObserverWillHide = NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, KeyboardWillHideNotification);
		}
		
		protected virtual void UnregisterKeyboardNotifications ()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (_keyboardObserverWillShow);
			NSNotificationCenter.DefaultCenter.RemoveObserver (_keyboardObserverWillHide);
		}
		
		/// <summary>
		/// Gets the UIView that represents the "active" user input control (e.g. textfield, or button under a text field)
		/// </summary>
		/// <returns>
		/// A <see cref="UIView"/>
		/// </returns>
		protected virtual UIView KeyboardGetActiveView ()
		{
			return this.View.FindFirstResponder ();
		}
		
		protected virtual void ResignResponder ()
		{
			UIView activeView = this.KeyboardGetActiveView ();
			if (activeView != null) {
				activeView.ResignFirstResponder ();
			}
		}

		protected virtual void KeyboardWillShowNotification (NSNotification notification)
		{
			UIView activeView = KeyboardGetActiveView ();
			if (activeView == null)
				return;

			UIScrollView scrollView = activeView.FindSuperviewOfType (this.View, typeof(UIScrollView)) as UIScrollView;
			if (scrollView == null)
				return;

			//RectangleF keyboardFrameBeginRect = UIKeyboard.BoundsFromNotification (notification);
			NSValue keyboardFrameBegin = ((NSValue)notification.UserInfo [UIKeyboard.FrameBeginUserInfoKey]);
			RectangleF keyboardFrameBeginRect = keyboardFrameBegin.RectangleFValue;
			

			UIEdgeInsets contentInsets = new UIEdgeInsets (0.0f, 0.0f, keyboardFrameBeginRect.Size.Height, 0.0f);
			scrollView.ContentInset = contentInsets;
			scrollView.ScrollIndicatorInsets = contentInsets;

			// If activeField is hidden by keyboard, scroll it so it's visible
			RectangleF viewRectAboveKeyboard = new RectangleF (this.View.Frame.Location, new SizeF (this.View.Frame.Width, this.View.Frame.Size.Height - keyboardFrameBeginRect.Size.Height));

			RectangleF activeFieldAbsoluteFrame = activeView.Superview.ConvertRectToView (activeView.Frame, this.View);
			// activeFieldAbsoluteFrame is relative to this.View so does not include any scrollView.ContentOffset

			// Check if the activeField will be partially or entirely covered by the keyboard
			if (!viewRectAboveKeyboard.Contains (activeFieldAbsoluteFrame)) {
				// Scroll to the activeField Y position + activeField.Height + current scrollView.ContentOffset.Y - the keyboard Height
				PointF scrollPoint = new PointF (0.0f, activeFieldAbsoluteFrame.Location.Y + activeFieldAbsoluteFrame.Height + scrollView.ContentOffset.Y - viewRectAboveKeyboard.Height);
				scrollView.SetContentOffset (scrollPoint, true);
			}
		}

		protected virtual void KeyboardWillHideNotification (NSNotification notification)
		{
			UIView activeView = KeyboardGetActiveView ();
			if (activeView == null)
				return;

			UIScrollView scrollView = activeView.FindSuperviewOfType (this.View, typeof(UIScrollView)) as UIScrollView;
			if (scrollView == null)
				return;

			// Reset the content inset of the scrollView and animate using the current keyboard animation duration
			double animationDuration = UIKeyboard.AnimationDurationFromNotification (notification);
			UIEdgeInsets contentInsets = new UIEdgeInsets (0.0f, 0.0f, 0.0f, 0.0f);
			UIView.Animate (animationDuration, delegate{
				scrollView.ContentInset = contentInsets;
				scrollView.ScrollIndicatorInsets = contentInsets;
			});
		}
		
		
		protected void  SetLabelsTextColor (UIView container, UIColor color)
		{
			foreach (var view in container.Subviews) {
				var label = view as UILabel;
				if (label != null) {
					label.TextColor = color;
				}
			}
		}


		
		
		protected void ShowHud (string title = null)
		{
			Hud = new MBProgressHUD (Util.MainWindow){
				Mode = this.HUDMode,
				TitleText = title,
				TitleFont = UIFont.SystemFontOfSize(14f),
				//DetailText = title,
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
		
		
		private class BackgroundView: UIView
		{
			BaseViewController controller;
                        
			public BackgroundView (BaseViewController controller)
			{
				this.controller = controller;
			}
                        
			public override void Draw (RectangleF rect)
			{
				base.Draw (rect);

				if (controller.BackgroundImage != null)
					controller.BackgroundImage.Draw (rect);
				/*else if (controller.Background9Image != null)
					controller.Background9Image.Draw (rect);
				*/
			}
		}
	}
}

