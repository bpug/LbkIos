using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Data;
using Loewenbraeu.Model;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public partial class HistorieViewController : UIViewController
	{
		private const string xmlPath = "Data/history.xml";
		
		private UIPageViewController _pageViewController;
		private UIPageControl _pageControl;
		
		private List<History> _historys;
		
		private int _currentPage;
		private int _totalPages;

		public int CurrentPage {
			get { return _currentPage; }
			set {
				_pageControl.CurrentPage = value;
				_currentPage = value;
			}
		}
		
		public int TotalPages {
			get {
				return _totalPages;
			}
			private set {
				_totalPages = value;
			}
		}
		
		public HistorieViewController ()
		{
			Initialize ();
		}
		
		private void Initialize ()
		{
			Title = Locale.GetText ("Historie");
			
			_historys = HistoryRepository.GetHistorys (xmlPath);
			_totalPages = _historys.Count;
			
			_pageControl = new UIPageControl (){
                  Pages = TotalPages,
                  Frame = new RectangleF (0, 390, 320, 20)
            };
			
			_pageControl.ValueChanged += HandlePageControlValueChanged;
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this._pageViewController = new UIPageViewController (UIPageViewControllerTransitionStyle.PageCurl, 
				UIPageViewControllerNavigationOrientation.Horizontal, UIPageViewControllerSpineLocation.Min);


			this._pageViewController.DataSource = new PageDataSource (this);
			
			this._pageViewController.DidFinishAnimating += delegate(object sender, UIPageViewFinishedAnimationEventArgs e) {
				if (e.Completed) {
					//_pageControl.CurrentPage =  this.CurrentPage;
					this.CurrentPage = ((PageDataSource)this._pageViewController.DataSource).CurrentPage;
				}
			};
			this._pageViewController.GetSpineLocation = GetSpineLocation;
			
			this._pageViewController.View.Frame = this.View.Bounds;
			this.View.AddSubview (this._pageViewController.View);
			
			View.AddSubview (_pageControl);
			
		}

		public UIPageViewControllerSpineLocation GetSpineLocation(UIPageViewController pageViewController, UIInterfaceOrientation orientation)
		{
			return UIPageViewControllerSpineLocation.Min;
		}
		
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			this.CurrentPage = 0;
			
			// Initialize the first page
			//HistoriePageController firstPageController = new HistoriePageController (0);
			HistoriePageController firstPageController = new HistoriePageController (GetHistory(0));
			
			this._pageViewController.SetViewControllers (
				new UIViewController[] { firstPageController },
				UIPageViewControllerNavigationDirection.Forward, 
				false, s => { }
			);
		}

		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown || toInterfaceOrientation == UIInterfaceOrientation.Portrait);
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown;
		}
		
		private void HandlePageControlValueChanged (object sender, EventArgs e)
		{
			//CurrentPage = _pageControl.CurrentPage;
			_pageControl.CurrentPage = CurrentPage;
		}
		
		public History GetHistory (int index)
		{
			return _historys.ElementAt (index);
		}

		/*
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			// Release any retained subviews of the main view.
		}
		*/
		protected override void Dispose (bool disposing)
		{
			if (disposing){
				_pageControl.Release();
				_pageViewController.Release ();
			}
			base.Dispose (disposing);
		}
		
		
		
		private class PageDataSource : UIPageViewControllerDataSource
		{
			private HistorieViewController parentController;
			public int CurrentPage = 0;
			
			public PageDataSource (HistorieViewController parentController)
			{
				this.parentController = parentController;
			}
			
			public override UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				
				HistoriePageController currentPageController = referenceViewController as HistoriePageController;
				
				// Determine if we are on the first page
				if (currentPageController.PageIndex <= 0) {
					// We are on the first page, so there is no need for a controller before that
					return null;
					
				} else {
					int previousPageIndex = currentPageController.PageIndex - 1;
					//parentController.CurrentPage = previousPageIndex;
					this.CurrentPage = previousPageIndex;
					
					var history = parentController.GetHistory (previousPageIndex);
					return new HistoriePageController (history);
					//return new HistoriePageController (previousPageIndex);
				}					
				
			}
			
			public override UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				
				HistoriePageController currentPageController = referenceViewController as HistoriePageController;
				
				// Determine if we are on the last page
				if (currentPageController.PageIndex >= (this.parentController.TotalPages - 1)) {
					
					// We are on the last page, so there is no need for a controller after that
					return null;
					
				} else {
					int nextPageIndex = currentPageController.PageIndex + 1;
					//parentController.CurrentPage = nextPageIndex;
					this.CurrentPage = nextPageIndex;
					var history = parentController.GetHistory (nextPageIndex);
					return new HistoriePageController (history);
					//return new HistoriePageController (nextPageIndex);
				}			
				
			}
			
		}
	}
}

