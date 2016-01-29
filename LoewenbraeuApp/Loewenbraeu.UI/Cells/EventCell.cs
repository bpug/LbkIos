using System;
using System.Drawing;
using System.IO;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Data.Service;
using MonoTouch.Dialog;
using MonoTouch.Dialog.Utilities;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Loewenbraeu.Core;
using Loewenbraeu.Data;
using Loewenbraeu.Data.Mappings;

namespace Loewenbraeu.UI
{
	public partial class EventCell : UITableViewCell, IImageUpdated
	{
		const int titleSize = 15;
		const int descrSize = 13;
		const int PicSize = 60;
		protected const int PicXPad = 6;
		const int PicYPad = 6;
		const int TextLeftStart = 2 * PicXPad + PicSize;
		const int TextHeightPadding = 4;
		protected const int TextYOffset = 5;
		const int MinHeight = PicSize + 2 * PicYPad;
		
		
		static UIFont titleFont = UIFont.BoldSystemFontOfSize (titleSize);
		static UIFont descrFont = UIFont.SystemFontOfSize (descrSize);
		
		private UILabel _lblTitle;
		private UILabel _lblDescription;
		private UILabel _lblDate;
		//private UIButton _btnOrder;
		private UIImageView _imageView;
		
		protected Event _event;
		
		
		
		public EventCell (IntPtr handle) : base (handle)
		{
			//Console.WriteLine (Environment.StackTrace);
		}
		
		public EventCell (UITableViewCellStyle style, NSString ident, Event lEvent) : base (style, ident)
		{
			this.Initialize (lEvent);
		}
		
		public EventCell (NSString ident, Event lEvent) : base(UITableViewCellStyle.Default, ident)
		{
			this.Initialize (lEvent);
		}
		
		public EventCell (Event lEvent) : this(UITableViewCellStyle.Default, new NSString("EventCell"), lEvent)
		{
			//this.Initialize (lEvent);
		}
		
		private void Initialize (Event lEvent)
		{
			_event = lEvent;
			
			this.SelectionStyle = UITableViewCellSelectionStyle.None;
			this.Accessory = UITableViewCellAccessory.None;
			
			_lblTitle = new UILabel () {
				Font = titleFont,
				TextAlignment = UITextAlignment.Left,
				TextColor =  Resources.Colors.CellText,
				BackgroundColor = UIColor.Clear,
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap
			};
			
			
			_lblDescription = new UILabel () {
				Font = descrFont,
				TextAlignment = UITextAlignment.Left,
				TextColor = Resources.Colors.CellText,
				BackgroundColor = UIColor.Clear,
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap
			};
			
			_lblDate = new UILabel () {
				Font = descrFont,
				TextAlignment = UITextAlignment.Left,
				TextColor = Resources.Colors.CellText,
				BackgroundColor = UIColor.Clear,
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap
			};
			/*
			_btnOrder = new LBKButton () {
				Font = UIFont.BoldSystemFontOfSize (13),
			};
			_btnOrder.SetTitle (Locale.GetText ("Event buchen"), UIControlState.Normal);
			
			_btnOrder.TouchUpInside += delegate {
				EventOrdered.RaiseEvent (this, new EventArgs<string> (_event.ReservationLink));
			};
			*/
			//_btnOrder.SetBackgroundImage (UIImage.FromBundle ("image/buttons/orderevent.png"), UIControlState.Normal);
			
			_imageView = new UIImageView (new RectangleF (PicXPad, PicYPad, PicSize, PicSize));
			
			BindDataToCell (_event);
			
			ContentView.Add (_lblTitle);
			ContentView.Add (_lblDate);
			ContentView.Add (_lblDescription);
			ContentView.Add (_imageView);
			/*
			if (!string.IsNullOrWhiteSpace (_event.ReservationLink)) {
				ContentView.Add (_btnOrder);
			}
			*/
		}
		
		
		
		// 
		// Layouts the views, called before the cell is shown
		//
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			BackgroundColor = Resources.Colors.Cell;
			
			RectangleF full = ContentView.Bounds;
			RectangleF tmp = full;
			
			// Title text
			tmp.Y += TextYOffset;
			tmp.X = TextLeftStart;
			tmp.Width -= TextLeftStart + PicXPad / 2;
			tmp.Height = _lblTitle.GetHeight4Text (tmp.Width);
			//tmp.Height -= TextYOffset;
			_lblTitle.Frame = tmp;
			
			// Date text
			if (!string.IsNullOrEmpty (_event.Date)) {
				tmp.Y += tmp.Height + TextYOffset;
				tmp.Height = _lblDate.GetHeight4Text (tmp.Width);
				_lblDate.Frame = tmp;
			}
			
			// Description text
			tmp.Y += tmp.Height + TextYOffset;
			tmp.Height = _lblDescription.GetHeight4Text (tmp.Width);
			_lblDescription.Frame = tmp;
			
			ContentBounds = tmp;
			/*
			// Order button
			tmp.Y += tmp.Height + TextYOffset;
			tmp.X = full.Width - (buttonSize.Width + PicXPad);
			_btnOrder.Frame = new RectangleF (new PointF (tmp.X, tmp.Y), buttonSize);
			*/
			
		}
		
		protected RectangleF ContentBounds;
		
		public static float GetCellHeight (UITableView tableView, Event @event)
		{
			int tetWidth = (int)tableView.Bounds.Width - (TextLeftStart + PicXPad / 2);
			
			if (tableView.Style == UITableViewStyle.Grouped) {
				tetWidth -= 20;
			}
			Size full = new Size (tetWidth, 999);
			
			//full.Height = 999;
			
			// Keep the same as LayoutSubviews
			//full.X = TextLeftStart;
			//full.Width = bounds.Width - (TextLeftStart + PicXPad);
			float height = 0;
			
			using (var nss = new NSString (@event.Title)) {
				height += nss.StringSize (titleFont, full, UILineBreakMode.WordWrap).Height;
			}
			
			if (!string.IsNullOrEmpty (@event.Date)) {
				using (var nss = new NSString (@event.Date)) {
					height += nss.StringSize (descrFont, full, UILineBreakMode.WordWrap).Height;
				}
			}
			using (var nss = new NSString (@event.Description)) {
				height += nss.StringSize (descrFont, full, UILineBreakMode.WordWrap).Height;
			}
			
			/*
			if (!string.IsNullOrWhiteSpace (@event.ReservationLink)) {
				height += buttonSize.Height;
			}
			*/
			
			return Math.Max (height + 4 * TextYOffset + 2 * TextHeightPadding, MinHeight);
		}
		
		public void BindDataToCell (Event @event)
		{
			this._event = @event;
			
			this._lblTitle.Text = @event.Title;
			this._lblDescription.Text = @event.Description;
			this._lblDate.Text = @event.Date;
			this._imageView.Image = null;
			
			if (_event.ThumbnailUri != null) {
				this._imageView.Image = ImageLoader.DefaultRequestImage (@event.ThumbnailUri, this);
			}
			
			/*
			if (string.IsNullOrEmpty (@event.ReservationLink)) {
				this._btnOrder.RemoveFromSuperview ();
			} else {
				_btnOrder.TouchUpInside += delegate {
					EventOrdered.RaiseEvent (this, new EventArgs<string> (@event.ReservationLink));
				};
				ContentView.Add (_btnOrder);
			}
			*/
			
			SetNeedsDisplay ();
			
		}
		
		#region IImageUpdated implementation
		void IImageUpdated.UpdatedImage (Uri uri)
		{
			// Discard notifications that might have been queued for an old cell
			if (this._event.ThumbnailUri != uri) {
				return;
			}
			
			UIView.BeginAnimations (null, IntPtr.Zero);
			UIView.SetAnimationDuration (0.5);
			
			var image = Util.GetImageFromLoaderCache (uri);
			if (image != null) {
				_imageView.Alpha = 0;
				_imageView.Image = image;
				//imageView.SizeToFit ();
			}
						
			this._imageView.Alpha = 1;
			UIView.CommitAnimations ();
		}
		#endregion
	}
	
	public partial class EventCellOrder : EventCell{
		private UIButton _btnOrder;
		static  Size buttonSize = new Size (120, 25);
		
		public event EventHandler<EventArgs<EventOrder>> EventOrdered = delegate {};
		
		public EventCellOrder (NSString ident, Event lEvent) : base( ident, lEvent)
		{
			this.Initialize ();
		}
		
		private void Initialize ()
		{
			_btnOrder = new LBKButton () {
				Font = UIFont.BoldSystemFontOfSize (13),
			};
			_btnOrder.SetTitle (Locale.GetText ("Event buchen"), UIControlState.Normal);

			_btnOrder.TouchUpInside += delegate {
				EventOrdered.RaiseEvent (this, new EventArgs<EventOrder> (_event.ToOrder()));
			};
			ContentView.Add (_btnOrder);
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			var tmp = ContentBounds;
			
			RectangleF full = ContentView.Bounds;
			
			// Order button
			tmp.Y += tmp.Height + TextYOffset;
			tmp.X = full.Width - (buttonSize.Width + PicXPad);
			_btnOrder.Frame = new RectangleF (new PointF (tmp.X, tmp.Y), buttonSize);
			
		}
		
		public static float GetCellHeight (UITableView tableView, Event @event)
		{
			return EventCell.GetCellHeight (tableView, @event) + buttonSize.Height;
		}
			
	}
	
	public class EventElement : HtmlElement, IElementSizing
	{
		public readonly Event lEvent;
		static NSString rkey = new NSString ("EventElement");
		static NSString eokey = new NSString ("EventOrderElement");
		
		public event EventHandler<EventArgs<EventOrder>> EventOrdered = delegate {};
		private DialogViewController _dvc;
		
		public EventElement (Event lEvent) : base (lEvent.Title, lEvent.ReservationLink)
		{
			this.lEvent = lEvent;
		}
		
		public EventElement (Event lEvent, DialogViewController dvc) : this (lEvent)
		{
			this._dvc = dvc;
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			
			base.GetCell (tv);
			if (string.IsNullOrEmpty (lEvent.ReservationLink)) {
				var cell = tv.DequeueReusableCell (rkey) as EventCell;
				if (cell == null) {
					cell = new EventCell (rkey, this.lEvent);
					/*
					cell.EventOrdered += delegate(object sender, EventArgs<string> e) {
						EventOrdered.RaiseEvent (this, e);
						base.Selected (_dvc, tv, null);
					};
					*/
				} else {
					cell.BindDataToCell (this.lEvent);
				}
				return cell;
			} else {
				var cell = tv.DequeueReusableCell (eokey) as EventCellOrder;
				
				if (cell == null) {
					cell = new EventCellOrder (eokey, this.lEvent);

					cell.EventOrdered += delegate(object sender, EventArgs<EventOrder> e) {
						if (e.Value != null) {
							Url = e.Value.Url;
							Caption = e.Value.Title;
							//EventOrdered.RaiseEvent (this, e);
							base.Selected (_dvc, tv, null);
						}
					};

				} else {
					cell.BindDataToCell (this.lEvent);
				}
				return cell;
			}
		}
		
		//Nicht beim Cell-Selected, sondern nur beim Button-TouchDown
		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			/*
			if (tableView == null && path == null)
				base.Selected (dvc, tableView, path);
			*/
		}
		
		#region IElementSizing implementation
		float IElementSizing.GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			if (string.IsNullOrEmpty (lEvent.ReservationLink)) {
				return EventCell.GetCellHeight (tableView, this.lEvent);
			}
			return EventCellOrder.GetCellHeight (tableView, this.lEvent);
		}
		#endregion
		
	}
}

