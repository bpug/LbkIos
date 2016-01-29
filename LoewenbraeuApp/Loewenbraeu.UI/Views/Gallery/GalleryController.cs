using System;
using System.Collections.Generic;
using System.Linq;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Data.Service;
using Loewenbraeu.ImageGallery;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Loewenbraeu.UI.Mappings;

namespace Loewenbraeu.UI
{
	public class GalleryController : BaseViewController
	{
		private UIViewController _rootViewConroller;
		
		public GalleryController (UIViewController rootViewConroller) : base()
		{
			Title = Locale.GetText ("Bilder");
			_rootViewConroller = rootViewConroller;
			ServiceAgent.Current.ServiceClient.GetPicturesCompleted += HandleGetPicturesCompleted;
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			Display ();
		}
		
		public void Display ()
		{
			if (ServiceAgent.Execute (ServiceAgent.Current.ServiceClient.GetPicturesAsync,Util.DeviceUid)) {
				Util.PushNetworkActive ();
			}
			
		}
		
		private void HandleGetPicturesCompleted (object sender, Loewenbraeu.Data.Service.GetPicturesCompletedEventArgs args)
		{
			bool error = ServiceAgent.HandleAsynchCompletedError (args, "GetEvents");
			using (var pool = new NSAutoreleasePool()) {
			
				pool.InvokeOnMainThread (delegate	{
					Util.PopNetworkActive ();
					if (error)
						return;
					
					List<Picture> result = args.Result.ToList ();
					ShowGalery (result);
				});
			}
		}
		
		public void ShowGalery (List<Picture> pictures)
		{
			if (pictures == null)
				return;
			
			var images = pictures.OrderBy (p => p.SortOrder).ToIGImage ();
				
			UIImage defaultImage = UIImage.FromFile ("image/preloader.png");
			
			var imageStore = new UrlImageStore<string> (50, "galery", null);
			imageStore.DefaultImage = defaultImage;
			var thumbnailImageStore = new UrlImageStore<string> (50, "galeryThumbnail", ScaleImage);
			thumbnailImageStore.DefaultImage = defaultImage;
				
			var igVC = new IGImageGalleryViewController<string> (images, imageStore, thumbnailImageStore, _rootViewConroller.NavigationController);
			igVC.View.Frame = this.View.Bounds;
			//igVC.Title = Locale.GetText ("Bilder");
			
			//_rootViewConroller.NavigationController.PushViewController (igVC, true);
			
			//this.View = igVC.View;
			this.Add (igVC.View);
		}
		
		private UIImage ScaleImage (UIImage image, string key)
		{
//			ImageResizer resizer = new ImageResizer (image);
//			resizer.CropResize (75, 75);
//			UIImage result = resizer.ModifiedImage;
			UIImage result = image.Thumbnail (new System.Drawing.SizeF (75, 75));
			return result;
		}

		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}

