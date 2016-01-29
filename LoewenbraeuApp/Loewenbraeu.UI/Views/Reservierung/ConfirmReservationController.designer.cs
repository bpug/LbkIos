// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Loewenbraeu.UI
{
	[Register ("ConfirmReservationController")]
	partial class ConfirmReservationController
	{
		[Outlet]
		MonoTouch.UIKit.UIView viewContainer { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblConfirm { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (viewContainer != null) {
				viewContainer.Dispose ();
				viewContainer = null;
			}

			if (lblConfirm != null) {
				lblConfirm.Dispose ();
				lblConfirm = null;
			}
		}
	}
}
