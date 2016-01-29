using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public class UIStyledPageControl : UIPageControl
	{
		private const string imgActivePath = "image/activeDot.jpg";
		private const string imgInactivePath = "image/inactiveDot.jpg";
		
		private  UIImage imgActive;
		private  UIImage imgInactive;
		
		public UIStyledPageControl () : base()
		{
			imgActive = UIImage.FromFile (imgActivePath);
			imgInactive = UIImage.FromFile (imgInactivePath);
		}

		public override int CurrentPage {
			get {
				return base.CurrentPage;
			}
			set {
				base.CurrentPage = value;
				for (int subviewIndex = 0; subviewIndex < this.Subviews.Length; subviewIndex++) {
					UIImageView subview = this.Subviews [subviewIndex] as UIImageView;
					if (subviewIndex == value) 
						subview.Image = imgActive;
					else
						subview.Image = imgInactive;
				}
			}
		}
		/*
		public override int Pages {
			get {
				return base.Pages;
			}
			set {
				base.Pages = value;
				for (int subviewIndex = 0; subviewIndex < this.Subviews.Length; subviewIndex++) {
					UIImageView subview = this.Subviews [subviewIndex] as UIImageView;
					subview.Image = imgInactive;
				}
			}
		}
		*/
		
	}
}

