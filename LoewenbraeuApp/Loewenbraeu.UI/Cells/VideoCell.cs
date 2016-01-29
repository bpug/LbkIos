using System;
using System.Drawing;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Model;
using MonoTouch.Dialog;
using MonoTouch.Dialog.Utilities;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


namespace Loewenbraeu.UI
{
	public abstract class VideoCellBase : UITableViewCell
	{
		protected const int TEXT_SIZE = 18;
		protected const int PIC_SIZE = 60;
		protected const int PIC_XPAD = 8;
		protected const int PIC_YPAD = 5;
		protected const int TEXT_LEFT_START = 4 * PIC_XPAD + PIC_SIZE;
		protected const int TEXT_HEIGHT_PADDING = 4;
		protected const int TEXE_YOFFSET = 0;
		protected const int MIN_HEIGHT = PIC_SIZE + 2 * PIC_YPAD;
		protected static UIFont TextFont = UIFont.SystemFontOfSize (TEXT_SIZE);
		protected UILabel LblText;
		
		protected Video Video;
		
		public VideoCellBase (IntPtr handle) : base (handle)
		{
			//Console.WriteLine (Environment.StackTrace);
		}
		
		public VideoCellBase (UITableViewCellStyle style, NSString ident, Video video) : base (style, ident)
		{
			Video = video;
			this.Initialize ();
		}
		
		private void Initialize ()
		{
			SelectionStyle = UITableViewCellSelectionStyle.Gray;
			BackgroundColor = UIColor.White;
			//Accessory = UITableViewCellAccessory.DetailDisclosureButton;
			
			LblText = new UILabel () {
				Font = TextFont,
				TextColor = Resources.Colors.CellText,
				BackgroundColor = UIColor.Clear,
				TextAlignment = UITextAlignment.Left,
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap
			};
			ContentView.Add (LblText);
		}
		
		public void UpdateCell (Video video)
		{
			this.Video = video;
			this.LblText.Text = video.Title;
			Update (video);
			SetNeedsDisplay ();
		}
	
		protected abstract void Update (Video video);
		
		public static float GetCellHeight (RectangleF bounds, string caption)
		{
			bounds.Height = 999;
			
			// Keep the same as LayoutSubviews
			bounds.X = TEXT_LEFT_START;
			bounds.Width -= TEXT_LEFT_START + TEXT_HEIGHT_PADDING;
			if (!string.IsNullOrEmpty (caption)) {
				using (var nss = new NSString (caption)) {
					var dim = nss.StringSize (TextFont, bounds.Size, UILineBreakMode.WordWrap);
					return Math.Max (dim.Height + TEXE_YOFFSET + 2 * TEXT_HEIGHT_PADDING, MIN_HEIGHT);
				}
			}
			return MIN_HEIGHT;
		}
		
		// 
		// Layouts the views, called before the cell is shown
		//
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			var full = ContentView.Bounds;
			var tmp = full;

			tmp = full;
			tmp.Y += TEXE_YOFFSET;
			tmp.Height -= TEXE_YOFFSET;
			tmp.X = TEXT_LEFT_START;
			tmp.Width -= TEXT_LEFT_START + TEXT_HEIGHT_PADDING + 10;
			LblText.Frame = tmp;
		}
		
	}
	
	public  class VideoCell : VideoCellBase, IImageUpdated
	{
		UIImageView imageView;
		
		public VideoCell (NSString ident, Video video) : this(UITableViewCellStyle.Default, ident, video)
		{
			
		}
		
		public VideoCell (UITableViewCellStyle style, NSString ident, Video video) : base (style, ident, video)
		{
			this.Initialize ();
		}
		
		private void Initialize ()
		{
			imageView = new UIImageView (new RectangleF (PIC_XPAD, PIC_YPAD, PIC_SIZE, PIC_SIZE));
			ContentView.Add (imageView);
			
			UpdateCell (Video);
		}
	
		protected override void Update (Video video)
		{
			this.imageView.Image = null;
			if (video.ThumbnailUri != null) {
				var img = ImageLoader.DefaultRequestImage (video.ThumbnailUri, this);
				if (img != null)
					this.imageView.Image = img.RoundCorners ();
			}
		}
		
		
		#region IImageUpdated implementation
		void IImageUpdated.UpdatedImage (Uri uri)
		{
			// Discard notifications that might have been queued for an old cell
			if (this.Video.ThumbnailUri != uri) {
				return;
			}
			
			UIView.BeginAnimations (null, IntPtr.Zero);
			UIView.SetAnimationDuration (0.5);
			
			var image = Util.GetImageFromLoaderCache (uri);
			if (image != null) {
				imageView.Alpha = 0;
				imageView.Image = image.RoundCorners ();
				//imageView.SizeToFit ();
			}
						
			this.imageView.Alpha = 1;
			UIView.CommitAnimations ();
		}
		#endregion
		
	}
	
	
	public  class YoutubeCell : VideoCellBase
	{
		UIWebView webview;
		private  UIButton _btnYoutube;
		
		public YoutubeCell (NSString ident, Video video) : this(UITableViewCellStyle.Default, ident, video)
		{
			
		}
		
		public YoutubeCell (UITableViewCellStyle style, NSString ident, Video video) : base (style, ident, video)
		{
			this.Initialize ();
		}
		
		private void Initialize ()
		{
			this.webview = new UIWebView (new RectangleF (PIC_XPAD, PIC_YPAD, PIC_SIZE, PIC_SIZE));
			this.webview.ScalesPageToFit = true;
			this.webview.AutoresizingMask = UIViewAutoresizing.None;
			this.webview.LoadFinished += (sender, e) => {
				_btnYoutube = this.webview.FirstOrDefault<UIButton> ();
				/* Autoplay
					if (_btnYoutube != null) {
						_btnYoutube.SendActionForControlEvents (UIControlEvent.TouchUpInside);
					}
					*/
			};
			webview.LoadStarted += delegate {
				Util.PushNetworkActive ();
			};
			webview.LoadFinished += delegate {
				Util.PopNetworkActive ();
			};
			webview.LoadError += (web, args) => {
				Util.PopNetworkActive ();
				if (webview != null)
					webview.LoadHtmlString (
						String.Format ("<html><center><font color='red'>{0}:<br>{1}</font></center></html>",
						"An error occurred:", args.Error.LocalizedDescription), null);
			};
			
			UpdateCell (Video);
			
			ContentView.Add (this.webview);
		}
		
		
		protected override void Update (Video video)
		{
			string embedHtml = "<html><head> " +
				"</head>" +
					"<meta name = 'viewport' content = 'initial-scale = 1.0, user-scalable = no, width = {1}'/></head>" +
					"<body style=\"background:#fffff;margin:0px;\" >" +
					"<object width='{1}px' height='{2}'>" +
					"<param name=\"movie\" value='{0}'></param>" +
					"<param name=\"wmode\" value=\"transparent\"></param>" +
				    "<embed  id ='yt' src='{0}' type='application/x-shockwave-flash' " +
				 	"width='{1}' height='{2}' wmode=\"transparent\"></embed>  " +
				 	"</object>" +
				"</body>" +
				"</html>";
			string html = string.Format (embedHtml, Video.Url, PIC_SIZE, PIC_SIZE);
			this.webview.LoadHtmlString (html, null);
		}
		
		public void PlayYotube ()
		{
			if (this._btnYoutube != null) {
				this._btnYoutube.SendActionForControlEvents (UIControlEvent.TouchUpInside);
			}
		}
	}
	
	public class VideoElement : Element, IElementSizing 
	{
		public readonly Video video;
		static NSString vkey = new NSString ("VideoElement");
		static NSString ykey = new NSString ("YotubeElement");
		
		public event EventHandler<EventArgs<Video>> VideoSelected = delegate {};
		
		public VideoElement (Video video) : base (null)
		{
			this.video = video;
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			VideoCellBase cell;
			
			if (this.video.IsYoutube) {
				cell = tv.DequeueReusableCell (ykey) as YoutubeCell;
				if (cell == null) {
					cell = new YoutubeCell (ykey, this.video);
				} else {
					cell.UpdateCell (this.video);
				}
				return cell;
			} else {
				cell = tv.DequeueReusableCell (vkey) as VideoCell;
				if (cell == null) {
					cell = new VideoCell (vkey, this.video);
				} else {
					cell.UpdateCell (this.video);
				}
				return cell;
			}
		}
		
		public override void Selected (DialogViewController dvc, UITableView tableView, MonoTouch.Foundation.NSIndexPath path)
		{
			//Console.WriteLine ("VideoId:" + video.Id.ToString());
			if (video.IsYoutube) {
				var cell = tableView.CellAt (path) as YoutubeCell;
				cell.PlayYotube ();
			}
			VideoSelected.RaiseEvent (this, new EventArgs<Video> (video));
			tableView.DeselectRow (path, true);
		}

		#region IElementSizing implementation
		float IElementSizing.GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return VideoCellBase.GetCellHeight (tableView.Bounds, this.video.Title);
		}
		#endregion
		
	}
	
	/*
	public class VideoYoutubeElement : HtmlElement, IElementSizing
	{
		public readonly Video video;
		UIWebView web;
		
		public VideoYoutubeElement (Video video) : base (video.Title, video.Url)
		{
			this.video = video;
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			base.GetCell (tv);
			var cell = tv.DequeueReusableCell (this.CellKey) as VideoCell;
			if (cell == null) {
				cell = new VideoCell (UITableViewCellStyle.Default, this.CellKey, this.video);
			} else {
				cell.UpdateCell (this.video);
			}
			
			return cell;
		}
		
		
		
		#region IElementSizing implementation
		float IElementSizing.GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return VideoCell.GetCellHeight (tableView.Bounds, this.video.Title);
		}
		#endregion
		
		
	
		static bool NetworkActivity {
			set {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = value;
			}
		}
		
		
		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			var vc = new WebViewController (this) {
				Autorotate = dvc.Autorotate
			};
			var bounds = UIScreen.MainScreen.Bounds;
			//web = new UIWebView (UIScreen.MainScreen.Bounds) {
			web = new UIWebView (new RectangleF (bounds.X, bounds.Y, bounds.Width, 414)) {
				BackgroundColor = UIColor.White,
				ScalesPageToFit = true,
				AutoresizingMask = UIViewAutoresizing.All
			};
			web.LoadStarted += delegate {
				NetworkActivity = true;
				var indicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.White);
				vc.NavigationItem.RightBarButtonItem = new UIBarButtonItem (indicator);
				indicator.StartAnimating ();
			};
			web.LoadFinished += delegate {
				NetworkActivity = false;
				vc.NavigationItem.RightBarButtonItem = null;
			};
			web.LoadError += (webview, args) => {
				NetworkActivity = false;
				vc.NavigationItem.RightBarButtonItem = null;
				if (web != null)
					web.LoadHtmlString (
						String.Format ("<html><center><font size=+5 color='red'>{0}:<br>{1}</font></center></html>",
						"An error occurred:", args.Error.LocalizedDescription), null);
			};
			vc.NavigationItem.Title = Caption;
			
			vc.View.AutosizesSubviews = true;
			vc.View.AddSubview (web);
			
			dvc.ActivateController (vc);
			
			string embedHtml = "<html><head> " +
				"</head>" +
					"<meta name = 'viewport' content = 'initial-scale = 1.0, user-scalable = no, width = {1}'/></head>" +
					"<body style=\"background:#ff0;margin:0px;\" >" +
					"<object width='{1}px' height='{2}'>" +
					"<param name=\"movie\" value='{0}'></param>" +
					"<param name=\"wmode\" value=\"transparent\"></param>" +
				    "<embed  id ='yt' src='{0}' type='application/x-shockwave-flash' " +
				 	"width='{1}' height='{2}' wmode=\"transparent\"></embed>  " +
				 	"</object>" +
				"</body>" +
			"</html>";
			string html = string.Format (embedHtml, Url, web.Bounds.Width, web.Bounds.Height);
			web.LoadHtmlString (html, null);
			//web.LoadRequest (NSUrlRequest.FromUrl (new NSUrl (Url)));
		}
		
		// We use this class to dispose the web control when it is not
		// in use, as it could be a bit of a pig, and we do not want to
		// wait for the GC to kick-in.
		class WebViewController : UIViewController
		{
			VideoYoutubeElement container;
			
			public WebViewController (VideoYoutubeElement container) : base ()
			{
				this.container = container;
			}
			
			public override void ViewWillDisappear (bool animated)
			{
				base.ViewWillDisappear (animated);
				NetworkActivity = false;
				if (container.web == null)
					return;

				container.web.StopLoading ();
				container.web.Dispose ();
				container.web = null;
			}

			public bool Autorotate { get; set; }
			
			public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
			{
				return Autorotate;
			}
		}
		
	}
	*/
}

