using System;
using System.Collections.Generic;
using System.Drawing;
using Loewenbraeu.Core;
using MonoTouch.QuickLook;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	//https://github.com/rob-brown/RBFilePreviewer/blob/master/RBFilePreviewer.m
	public class DocumentViewController : QLPreviewController
	{
		private List<DocPreviewItem> _items;

		private UIBarButtonItem _leftButton;
		private UIBarButtonItem _rightButton;
		private UIToolbar _toolbar;
		public UIColor BarTintColor { get; set; }
		
		
		public List<DocPreviewItem> Items {
			get {
				return this._items;
			}
			set {
				_items = value;
				this.DataSource  = new DocumentDataSource (_items);
				ReloadData ();
			}
		}
		
		public DocumentViewController (DocPreviewItem item) : this( new List<DocPreviewItem> () {item})
		{
			
		}
		
		public DocumentViewController (List<DocPreviewItem> items) : base()
		{
			_items = items;
			this.DataSource = new DocumentDataSource (_items);
		}
		
		private bool IsModalViewController ()
		{
			//return  (NavigationController.ParentViewController.ModalViewController != null || ParentViewController.ModalViewController != null);
			return  (NavigationController.ParentViewController.PresentedViewController != null || ParentViewController.PresentedViewController != null);
		}
		
		private void SetCurrentPreviewItemIndex (int index)
		{
			int max = _items.Count;
			if (index < 0 || index >= max)
				return;
			base.CurrentPreviewItemIndex = index;
			UpdateArrows ();
			//RemoveActionButtonIfApplicable;
		}
		
		
		private void UpdateArrows ()
		{
			int index = CurrentPreviewItemIndex;
			int max = _items.Count;
    
			_rightButton.Enabled = index < max - 1;
			_leftButton.Enabled = index != 0;
		}
		
		private void ShowPreviousDocument (object sender, EventArgs e)
		{
			this.SetCurrentPreviewItemIndex (CurrentPreviewItemIndex - 1);
		}
		
		private void ShowNextDocument (object sender, EventArgs e)
		{
			this.SetCurrentPreviewItemIndex (CurrentPreviewItemIndex + 1);
		}
		
		private void AddToolbarIfApplicable ()
		{
			// Adds a toolbar to the view so it's available to both pushed views and modal views.
			//if (this._toolbar == null && _items.Count > 1) {
			if (_items.Count > 1) {
        
				const float kStandardHeight = 44.0f;
				float superViewWidth = this.View.Frame.Size.Width;
				float superViewHeight = this.View.Frame.Size.Height;
				RectangleF frame = new RectangleF (0, 
                                  superViewHeight - kStandardHeight, 
                                  superViewWidth, 
                                  kStandardHeight);
        
				_toolbar = new UIToolbar (frame);
				
				_leftButton = new UIBarButtonItem (UIImage.FromFile ("image/buttons/arrow-left.png"), 
					                                   UIBarButtonItemStyle.Bordered, ShowPreviousDocument);
				_rightButton = new UIBarButtonItem (UIImage.FromFile ("image/buttons/arrow-right.png"), 
				                                    UIBarButtonItemStyle.Bordered, ShowNextDocument);
			
        
				UIBarButtonItem flexibleSpace = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace, null);
					
				_toolbar.SetItems (new UIBarButtonItem[] {flexibleSpace, _leftButton, flexibleSpace, _rightButton, flexibleSpace}, true);
				_toolbar.AutoresizingMask = UIViewAutoresizing.FlexibleMargins | UIViewAutoresizing.FlexibleWidth;
				
				//leftButton = left;
				//rightButton = right;
				this.View.AddSubview (_toolbar);
				//_toolbar = toolbar;
				/*
				left = null;
				right = null;
				toolbar = null;
				*/
				flexibleSpace = null;
				
				UpdateArrows ();
				_toolbar.TintColor = BarTintColor;
			}
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			if ( this.NavigationController != null)
				this.NavigationController.NavigationBar.TintColor = BarTintColor;
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			AddToolbarIfApplicable ();
		}

		/*
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			_rightButton = null;
			_leftButton = null;
			_toolbar = null;
			_items = null;
		}
		*/

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				_rightButton = null;
				_leftButton = null;
				_toolbar = null;
				_items = null;
			}
			base.Dispose (disposing);
		}
		
		
		private  class DocumentDataSource : QLPreviewControllerDataSource
		{
			
			private List<DocPreviewItem> _items;
			
			public DocumentDataSource (List<DocPreviewItem> items)
			{
				_items = items;
			}
			
			public override int PreviewItemCount (QLPreviewController controller)
			{
				return _items.Count;
			}

			public override QLPreviewItem GetPreviewItem (QLPreviewController controller, int index)
			{
				return _items [index];
			}
		}
		
	}
	
	
}

