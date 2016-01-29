using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Loewenbraeu.Core;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using Loewenbraeu.Core.Extensions;


namespace Loewenbraeu.ImageGallery
{
	public class ImageSingleViewController<TKey> : UIViewController, IUrlImageUpdated<TKey>
	{
		private const float TOOLBAR_HEIGHT =44f;
		private UITapGestureRecognizer tapGesture;
		static Selector TapSelector = new Selector ("HandleTap");
		
		private UIScrollView _pagingScrollView;
		private UIToolbar _toolbar;
		private UILabel _barLabel;
		
		private HashSet<ImageScrollView> recycledPages;
		private HashSet<ImageScrollView> visiblePages;
		
		private UIColor originalBarColour = UIColor.Black;
		private float originalBarAlpha = 1.0f;
		
	    // these values are stored off before we start rotation so we adjust our content offset appropriately during rotation
    	private int   firstVisiblePageIndexBeforeRotation;
    	private float percentScrolledIntoFirstVisiblePage;
		
		public UrlImageStore<TKey> ImageStore {
			get;
			set;	
		}
		
		public List<IGImage<TKey>> Images {
			get;
			set;	
		}
		
		private int _imageIndex =0;
		
		public int ImageIndex {
			get{ return _imageIndex;}
			set {
				if (_imageIndex != value ) {
					_imageIndex = value;
					SetTitle (_imageIndex);
					
				}
			}	
		}
		
		private void SetTitle (int index)
		{
			var img = this.Images [index];
			this.Title = img.Title;
			if (_barLabel != null) {
				_barLabel.Text = img.Caption;
				
				SetToolBarPosition ();
			}
			
		}
		
		public ImageSingleViewController (List<IGImage<TKey>> images, int imageIndex, UrlImageStore<TKey> imageStore)
		{
			this.Images = images;
			this.ImageIndex = imageIndex;
			this.ImageStore = imageStore;
			
			tapGesture = new UITapGestureRecognizer ();
			tapGesture.NumberOfTapsRequired = 1;
			tapGesture.DelaysTouchesEnded = true;
			tapGesture.DelaysTouchesBegan = true;
			
			tapGesture.AddTarget (this, TapSelector);
			this.View.AddGestureRecognizer (tapGesture);
			
			this.View.AutoresizingMask = UIViewAutoresizing.FlexibleMargins | UIViewAutoresizing.FlexibleDimensions;
			
		}
		
		#region View loading and unloading
		public override void LoadView ()
		{
			base.LoadView ();
			// Step 1: make the outer paging scroll view
			//var pagingScrollViewFrame = this.FrameForPagingScrollView;
			//pagingScrollView = new UIScrollView (pagingScrollViewFrame);
			
			_pagingScrollView = new UIScrollView ();
			_pagingScrollView.PagingEnabled = true;
			_pagingScrollView.BackgroundColor = UIColor.Black;
			_pagingScrollView.ShowsVerticalScrollIndicator = false;
			_pagingScrollView.ShowsHorizontalScrollIndicator = false;
			_pagingScrollView.AutoresizingMask = UIViewAutoresizing.FlexibleMargins | UIViewAutoresizing.FlexibleDimensions;
			
			//pagingScrollView.ContentSize = this.ContentSizeForPagingScrollView;
			
			#region ScrollView delegate methods
			_pagingScrollView.Scrolled += delegate {
				this.TilePages ();
			};
			#endregion
			
			//this.View = pagingScrollView;
			this.View.AddSubview (_pagingScrollView);
			
			// Step 2: prepare to tile content
			recycledPages = new HashSet<ImageScrollView> ();
			visiblePages = new HashSet<ImageScrollView> ();
			
			//this.tilePages();
			
			//this.ShowPage (ImageIndex);
			//pagingScrollView.ScrollRectToVisible (this.FrameForPageAtIndex (ImageIndex), false);
		}
		
		
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			// hidden Navigationbar, for Bound.Height without Navigationbar.Height
			this.NavigationController.SetNavigationBarHidden (true, true);
		}
		
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			//Console.WriteLine ("Height:" + this.View.Bounds.Height);
			
			//*** make the outer paging scroll view ****
			_pagingScrollView.Frame = this.FrameForPagingScrollView;
			_pagingScrollView.ContentSize = this.ContentSizeForPagingScrollView;
			if (ImageIndex == 0) {
				this.ShowPage (ImageIndex);
			} else {
				_pagingScrollView.ScrollRectToVisible (this.FrameForPageAtIndex (ImageIndex), false);
			}
			
			//this.View.SetNeedsLayout ();
			//this.View.LayoutSubviews ();
			
			this.NavigationController.SetNavigationBarHidden (false, true);
			
			//this.NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
			//this.NavigationController.NavigationBar.TintColor = UIColor.Black;
			//this.NavigationController.NavigationBar.Opaque = false;
			
			this.originalBarAlpha = this.NavigationController.NavigationBar.Alpha;
			this.originalBarColour = this.NavigationController.NavigationBar.TintColor;
			
			this.NavigationController.NavigationBar.TintColor = UIColor.Black;
			this.NavigationController.NavigationBar.Translucent = true;
			this.NavigationController.NavigationBar.Alpha = 0.6f;
			InitToolBar ();
		}
		
		private void InitToolBar ()
		{
			this.NavigationController.SetToolbarHidden (false, true);
			_toolbar = this.NavigationController.Toolbar;
			_toolbar.Translucent = true;
			_toolbar.Alpha = 0.6F;
			_toolbar.TintColor = UIColor.Black;
			//_toolbar.Frame
		
			this.Title = this.Images [this.ImageIndex].Title;
			
			_barLabel = new UILabel (){
				AutoresizingMask = UIViewAutoresizing.FlexibleDimensions | UIViewAutoresizing.FlexibleMargins,
				Font = UIFont.SystemFontOfSize (13.0f),
				Text = this.Images [this.ImageIndex].Caption,
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.White,
				Lines = 0,
				TextAlignment = UITextAlignment.Center,
				LineBreakMode = UILineBreakMode.WordWrap
			};
			//_barLabel.BackgroundColor = UIColor.Red;
			float textHeight = _barLabel.GetHeight4Text (this.View.Bounds.Width - 20);
			_barLabel.Frame = new RectangleF (0, 0, this.View.Bounds.Width - 20, textHeight);
			
			UIBarButtonItem barTitle = new UIBarButtonItem (_barLabel);
			
			if (_toolbar != null) {
				//_toolbar.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
				_toolbar.SetItems (new UIBarButtonItem[]{barTitle}, true);
				SetToolBarPosition ();
			}
		}
		
		private void SetToolBarPosition ()
		{
			float textHeight = _barLabel.GetHeight4Text (this.View.Bounds.Width - 20);
			_barLabel.Frame = new RectangleF (_barLabel.Frame.X, _barLabel.Frame.Y, _barLabel.Frame.Width, textHeight);
			
			float toolBarY = (this.View.Frame.Height == 416 || this.View.Frame.Height == 268) ? this.View.Frame.Height + 44 : this.View.Frame.Height;
			float toolBarHeight = textHeight + 25;
			
			if (this.NavigationController != null && this.NavigationController.NavigationBar != null) {
				//NavigationController.NavigationBar.Frame.Height;
			}
			
			toolBarY -= toolBarHeight;
			_toolbar.Frame = new RectangleF (_toolbar.Frame.X, toolBarY + 20, _toolbar.Frame.Width, toolBarHeight);
		}
		
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			
			this.NavigationController.NavigationBar.TintColor = originalBarColour;
			this.NavigationController.NavigationBar.Alpha = originalBarAlpha;
			this.NavigationController.NavigationBar.Translucent = false;
			this.NavigationController.SetToolbarHidden (true, false);
		}

		/*
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			_pagingScrollView.Dispose();
			_pagingScrollView = null;
			recycledPages = null;
			visiblePages = null;
		}
		*/

		protected override void Dispose (bool disposing)
		{
			if (disposing){
				_pagingScrollView.Dispose();
				_pagingScrollView.Dispose();
				_pagingScrollView = null;
				recycledPages = null;
				visiblePages = null;
			}
			base.Dispose (disposing);
		}
		#endregion
		
		public void UrlImageUpdated (TKey id, bool local)
		{
			//if (id.Equals (this.Images [this.ImageIndex].Id)) {
			this.BeginInvokeOnMainThread (delegate {
				var image = this.ImageStore.GetImage (this.Images [this.ImageIndex].Id);
				//Console.WriteLine ("Image:" + image.ToString ());
					
				//this.imageView.Image = ScaleImage (image);
				//this.imageView.Image = image;
				//this.scrollView.ContentSize = this.imageView.Image.Size;
					
				//this.activityView.StopAnimating ();
				//this.activityView.Hidden = true;
				//var page = pagingScrollView.Subviews [0] as ImageScrollView;
				
				//Console.WriteLine ("Image Index:" + this.ImageIndex.ToString ());
				
				/*
				foreach (var p in visiblePages) {
					Console.WriteLine ("Visible Page Index:" + p.index.ToString ());
				}
				*/
				var page = visiblePages.Where (p => p.index == this.ImageIndex).FirstOrDefault ();
				if (page != null) {
					//Console.WriteLine ("Display Image:" + image.ToString ());
					page.DisplayImage (image);
				}
					
			});
			//}
		}
		
		#region Tiling and page configuration
		void TilePages ()
		{
			//Console.WriteLine ("Height: " + pagingScrollView.ContentSize);
			//Console.WriteLine ("ContentOffset: " + pagingScrollView.ContentOffset);
			//pagingScrollView.SetContentOffset( new PointF(pagingScrollView.ContentOffset.X,0),false);
//			return;
			// Calculate which pages are visible
			var visibleBounds = _pagingScrollView.Bounds;
			int firstNeededPageIndex = (int)Math.Floor (visibleBounds.GetMinX () / visibleBounds.Width);
			int lastNeededPageIndex = (int)Math.Floor ((visibleBounds.GetMaxX () - 1) / visibleBounds.Width);
			firstNeededPageIndex = Math.Max (firstNeededPageIndex, 0);
			lastNeededPageIndex = Math.Min (lastNeededPageIndex, this.Images.Count - 1);
		    
			// Recycle no-longer-visible pages 
			foreach (var page in visiblePages) {
				if (page.index < firstNeededPageIndex || page.index > lastNeededPageIndex) {
					recycledPages.Add (page);
					page.RemoveFromSuperview ();
				}
			}
			
			// [visiblePages minusSet:recycledPages];
			foreach (var item in recycledPages) {
				if (visiblePages.Contains (item)) {
					visiblePages.Remove (item);
				}
			}
		  
			// add missing pages
			for (int index = firstNeededPageIndex; index <= lastNeededPageIndex; index++) {
				this.ShowPage (index);
			}    
			//Schalte der Indikator um, wenn mehr als 50% previous/next page sichtbar ist.
			float pageWidth = this._pagingScrollView.Frame.Size.Width;			
			int pageIndex = (int)Math.Floor ((this._pagingScrollView.ContentOffset.X - pageWidth / 2.0) / pageWidth) + 1;
			if (pageIndex >= 0 && this.Images.Count > pageIndex) {
				this.ImageIndex = pageIndex;
			}
		}
		
		private void ShowPage (int index)
		{
			if (!this.IsDisplayingPageForIndex (index)) {
				ImageScrollView page = this.dequeueRecycledPage ();
				if (page == null) {
					page = new ImageScrollView (FrameForPageAtIndex (index));	// different from ObjC [CD]
				}
				this.ConfigurePage (page, index);
				_pagingScrollView.AddSubview (page); 
				
				visiblePages.Add (page);
			}
		}
		
		private ImageScrollView dequeueRecycledPage ()
		{
			ImageScrollView page = null;
			if (recycledPages.Count > 0) {
				page = (from r in recycledPages
					select r).First ();
				
				if (page != null) {	// HACK: is this correct ??
					recycledPages.Remove (page);
				}
			}
			
			return page;
		}

		bool IsDisplayingPageForIndex (int index)
		{
			bool foundPage = false;
			foreach (ImageScrollView page in visiblePages.ToArray<ImageScrollView>()) {
				if (page.index == index) {
					foundPage = true;
					break;
				}
			}
			return foundPage;
		}
		
		public void ConfigurePage (ImageScrollView page, int index)
		{
			page.index = index;
			page.Frame = this.FrameForPageAtIndex (index);
			//page.displayImage (this.imageAtIndex (index));
			var img = this.Images [index];
			UIImage image = this.ImageStore.RequestImage (img.Id, img.Url, this);
			
			page.DisplayImage (image);
			
			
		}
		#endregion
		
		
		#region View controller rotation methods
		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.All;
		}
		
		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			// here, our pagingScrollView bounds have not yet been updated for the new interface orientation. So this is a good
			// place to calculate the content offset that we will need in the new orientation
			var offset = _pagingScrollView.ContentOffset.X;
			var pageWidth = _pagingScrollView.Bounds.Size.Width;
		    
			if (offset >= 0) {
				firstVisiblePageIndexBeforeRotation = (int)Math.Floor (offset / pageWidth);
				percentScrolledIntoFirstVisiblePage = (offset - (firstVisiblePageIndexBeforeRotation * pageWidth)) / pageWidth;
			} else {
				firstVisiblePageIndexBeforeRotation = 0;
				percentScrolledIntoFirstVisiblePage = offset / pageWidth;
			}   
			
		}
		
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			// recalculate contentSize based on current orientation
			_pagingScrollView.ContentSize = this.ContentSizeForPagingScrollView;
		    
			// adjust frames and configuration of each visible page
			foreach (ImageScrollView page in visiblePages.ToArray<ImageScrollView>()) {
				var restorePoint = page.pointToCenterAfterRotation ();
				var restoreScale = page.scaleToRestoreAfterRotation ();
				page.Frame = this.FrameForPageAtIndex (page.index);
				page.setMaxMinZoomScalesForCurrentBounds ();
				page.restoreCenterPoint (restorePoint, restoreScale);
		        
			}
		    
			// adjust contentOffset to preserve page location based on values collected prior to location
			var pageWidth = _pagingScrollView.Bounds.Size.Width;
			var newOffset = (firstVisiblePageIndexBeforeRotation * pageWidth) + (percentScrolledIntoFirstVisiblePage * pageWidth);
			_pagingScrollView.ContentOffset = new PointF (newOffset, 0);
			
			SetToolBarPosition ();
		}
		#endregion
		
		#region Frame calculations
		//HACK: const int PADDING = 10;
		
		private RectangleF FrameForPagingScrollView {
			get {
				//var frame = UIScreen.MainScreen.Bounds;
				
				var frame = this.View.Bounds;
				
				//frame.X -= PADDING; //HACK: remove PADDING
				//frame.Size = new SizeF(frame.Size.Width + (2 * PADDING), frame.Size.Height); //HACK: remove PADDING
				return frame;
			}
		}
		
		private RectangleF FrameForPageAtIndex (int index)
		{
			// We have to use our paging scroll view's bounds, not frame, to calculate the page placement. When the device is in
			// landscape orientation, the frame will still be in portrait because the pagingScrollView is the root view controller's
			// view, so its frame is in window coordinate space, which is never rotated. Its bounds, however, will be in landscape
			// because it has a rotation transform applied.
			var bounds = _pagingScrollView.Bounds;
			var pageFrame = bounds;
			//pageFrame.Size = new SizeF(pageFrame.Size.Width - (2 * PADDING), pageFrame.Size.Height); //HACK: remove PADDING
			pageFrame.X = (bounds.Size.Width * index);// + PADDING; //HACK: remove PADDING
			return pageFrame;
		}
		
		private SizeF ContentSizeForPagingScrollView {
			get {
				// We have to use the paging scroll view's bounds to calculate the contentSize, for the same reason outlined above.
				var bounds = _pagingScrollView.Bounds;
				//return new SizeF (bounds.Size.Width * this.Images.Count, bounds.Size.Height);
				return new SizeF (bounds.Size.Width * this.Images.Count, _pagingScrollView.Frame.Size.Height);
				
			}
		}
		
		#endregion
		
		[Export("HandleTap")]
		public void HandleTap (UITapGestureRecognizer recognizer)
		{
			
			if (this.NavigationController.NavigationBarHidden) {
				
				this.NavigationController.SetNavigationBarHidden (false, true);
				this.NavigationController.SetToolbarHidden (false, true);
				InitToolBar ();
				
			} else {
				this.BeginInvokeOnMainThread (delegate {
					this.NavigationController.SetNavigationBarHidden (true, true);
					this.NavigationController.SetToolbarHidden (true, true);
				
				});
			}
		}
		
	}
	
}

