// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Loewenbraeu.UI
{
	[Register ("RaumDetailViewController")]
	partial class RaumDetailViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIView viewContainer { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewDescription { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblDescription { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (viewContainer != null) {
				viewContainer.Dispose ();
				viewContainer = null;
			}

			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}

			if (viewDescription != null) {
				viewDescription.Dispose ();
				viewDescription = null;
			}

			if (lblDescription != null) {
				lblDescription.Dispose ();
				lblDescription = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}
		}
	}
}
