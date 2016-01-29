// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Loewenbraeu.UI
{
	[Register ("TageskarteViewController")]
	partial class TageskarteViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tblTageskarte { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblTageskarte != null) {
				tblTageskarte.Dispose ();
				tblTageskarte = null;
			}
		}
	}
}
