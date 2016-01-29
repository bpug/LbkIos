// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Loewenbraeu.UI
{
	[Register ("KontaktViewController")]
	partial class KontaktViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIScrollView scrollContainer { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel txtAddress { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel txtPhone { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel txtMail { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnPhone { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel txtFax { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnMail { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView txtPlan { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel txtDistance { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnMap { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (scrollContainer != null) {
				scrollContainer.Dispose ();
				scrollContainer = null;
			}

			if (txtAddress != null) {
				txtAddress.Dispose ();
				txtAddress = null;
			}

			if (txtPhone != null) {
				txtPhone.Dispose ();
				txtPhone = null;
			}

			if (txtMail != null) {
				txtMail.Dispose ();
				txtMail = null;
			}

			if (btnPhone != null) {
				btnPhone.Dispose ();
				btnPhone = null;
			}

			if (txtFax != null) {
				txtFax.Dispose ();
				txtFax = null;
			}

			if (btnMail != null) {
				btnMail.Dispose ();
				btnMail = null;
			}

			if (txtPlan != null) {
				txtPlan.Dispose ();
				txtPlan = null;
			}

			if (txtDistance != null) {
				txtDistance.Dispose ();
				txtDistance = null;
			}

			if (btnMap != null) {
				btnMap.Dispose ();
				btnMap = null;
			}
		}
	}
}
