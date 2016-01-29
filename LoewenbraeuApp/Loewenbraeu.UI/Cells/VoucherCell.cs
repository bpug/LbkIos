using System;
using System.Drawing;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Model;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public abstract class VoucherCellBase : UITableViewCell
	{
		private UILabel lblCode; 
		protected  QuizVoucher Voucher {get;set;}
		
		public VoucherCellBase (UITableViewCellStyle style, NSString ident, QuizVoucher quizVoucher) : base (style, ident)
		{
			Initialize (quizVoucher);
		}
		
		private void Initialize (QuizVoucher quizVoucher)
		{
			this.Voucher = quizVoucher;
			
			lblCode = new UILabel (new RectangleF (10, 14, 127, 21)){
				Font = UIFont.BoldSystemFontOfSize (18),
				TextColor = UIColor.White,
				BackgroundColor = UIColor.Clear,
				AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleTopMargin,
			};
			
			UpdateCell (this.Voucher);
			ContentView.Add (lblCode);
		}
		
		public void UpdateCell (QuizVoucher quizVoucher)
		{
			this.Voucher = quizVoucher;
			this.lblCode.Text = quizVoucher.Code;
			
			SetNeedsDisplay ();
		}
		
		public static float GetCellHeight (RectangleF bounds)
		{
			return 50f;
		}
		
	}
	
	
	public  class VoucherUseCell : VoucherCellBase
	{
		private UIButton btnUseVoucher;
		public event EventHandler<EventArgs<QuizVoucher>> VoucherUsed = delegate {};
		
		public VoucherUseCell (NSString ident, QuizVoucher voucher) : this(UITableViewCellStyle.Default, ident, voucher)
		{
			
		}
		
		public VoucherUseCell (UITableViewCellStyle style, NSString ident, QuizVoucher voucher) : base (style, ident, voucher)
		{
			this.Initialize ();
		}
		
		private void Initialize ()
		{
			btnUseVoucher = new LBKButton (new RectangleF (201, 6, 115, 37)){
		          Font = UIFont.BoldSystemFontOfSize (15),
				  AutoresizingMask = UIViewAutoresizing.FlexibleMargins,
		     };
			
			btnUseVoucher.SetTitle (Locale.GetText ("Bier erhalten"), UIControlState.Normal);
			
			btnUseVoucher.TouchUpInside += delegate(object sender, EventArgs e) {
				
				VoucherUsed.RaiseEvent (this, new EventArgs<QuizVoucher> (this.Voucher));
			};
			ContentView.Add (btnUseVoucher);
		}
	}
	
	public  class VoucherActivateCell : VoucherCellBase
	{
		private UIButton btnActivateVoucher;
		public event EventHandler<EventArgs<QuizVoucher>> VoucherActivated = delegate {};
		
		public VoucherActivateCell (NSString ident, QuizVoucher voucher) : this(UITableViewCellStyle.Default, ident, voucher)
		{
			
		}
		
		public VoucherActivateCell (UITableViewCellStyle style, NSString ident, QuizVoucher voucher) : base (style, ident, voucher)
		{
			this.Initialize ();
		}
		
		private void Initialize ()
		{
			btnActivateVoucher = new LBKButton (new RectangleF (201, 6, 115, 37)){
		          Font = UIFont.BoldSystemFontOfSize (15),
				  AutoresizingMask = UIViewAutoresizing.FlexibleMargins,
		     };
			
			btnActivateVoucher.SetTitle (Locale.GetText ("Aktivieren"), UIControlState.Normal);
			btnActivateVoucher.TouchUpInside += delegate(object sender, EventArgs e) {
				VoucherActivated.RaiseEvent (this, new EventArgs<QuizVoucher> (this.Voucher));
			};
			ContentView.Add (btnActivateVoucher);
		}
	}
	
	
	public class VoucherElement : Element, IElementSizing
	{
		public readonly QuizVoucher voucher;
		
		static NSString useKey = new NSString ("VoucherUseElement");
		static NSString actvateKey = new NSString ("VoucherActivateElement");
		
		
		public event EventHandler<EventArgs<QuizVoucher>> VoucherUsed = delegate {};
		public event EventHandler<EventArgs<QuizVoucher>> VoucherActivated = delegate {};
		
		public VoucherElement (QuizVoucher video) : base (null)
		{
			this.voucher = video;
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			//VoucherCellBase cell;
			
			if (!this.voucher.IsActivated) {
				var cell = tv.DequeueReusableCell (actvateKey) as VoucherActivateCell;
				if (cell == null) {
					cell = new VoucherActivateCell (actvateKey, this.voucher);
					cell.VoucherActivated += delegate(object sender, EventArgs<QuizVoucher> e) {
						VoucherActivated.RaiseEvent (this, e);
					};
				} else {
					cell.UpdateCell (this.voucher);
				}
				return cell;
			} else {
				var cell = tv.DequeueReusableCell (useKey) as VoucherUseCell;
				if (cell == null) {
					cell = new VoucherUseCell (useKey, this.voucher);
					cell.VoucherUsed += delegate(object sender, EventArgs<QuizVoucher> e) {
						VoucherUsed.RaiseEvent (this, e);
					};
				} else {
					cell.UpdateCell (this.voucher);
				}
				return cell;
			}
		}

		#region IElementSizing implementation
		float IElementSizing.GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return VoucherCellBase.GetCellHeight (tableView.Bounds);
		}
		#endregion
		
	}
}

