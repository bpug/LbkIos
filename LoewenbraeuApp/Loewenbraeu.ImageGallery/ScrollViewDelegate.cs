using MonoTouch.UIKit;

namespace Loewenbraeu.ImageGallery
{	
	public class ScrollViewDelegate : UIScrollViewDelegate
	{
		UIView zoomView;
		
		public ScrollViewDelegate(UIView zoomView)
		{
			this.zoomView = zoomView;	
		}
		
		public override UIView ViewForZoomingInScrollView (UIScrollView scrollView)
		{
			return this.zoomView;
		}
	}	
}

