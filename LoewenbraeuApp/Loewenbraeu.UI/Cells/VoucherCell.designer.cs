// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Loewenbraeu.UI
{
	[Register ("VoucherCell")]
	partial class VoucherCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblCode { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableViewCell cellVoucher { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblCode != null) {
				lblCode.Dispose ();
				lblCode = null;
			}

			if (cellVoucher != null) {
				cellVoucher.Dispose ();
				cellVoucher = null;
			}
		}
	}
}
