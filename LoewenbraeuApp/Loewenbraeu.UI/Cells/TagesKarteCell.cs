using System;
using System.Drawing;
using Loewenbraeu.Data.Service;
using Loewenbraeu.Model;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public partial class TagesKarteCell : UIViewController
	{
		
		public TagesKarteCell (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public TagesKarteCell (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		
		
		public TagesKarteCell () //: base ("TageskarteSpeisenContainer", null)
		{
			MonoTouch.Foundation.NSBundle.MainBundle.LoadNib ("TagesKarteCell", this, null);
			Initialize ();
		}
		
		private void Initialize (){}
		
		
		public UITableViewCell SpeisenZelle
		{
			get { return this.Sp_Zelle; }
		}
		
		public string SpeisenTitel{
			get { return this.Sp_Titel.Text;}
			set { this.Sp_Titel.Text = value;}
		}
		
		public string SpeisenPreis{
			get { return this.Sp_Preis.Text;}
			set { this.Sp_Preis.Text = value;}
		}
		
		public string SpeisenText {
			get { return this.Sp_Text.Text;}
			set { this.Sp_Text.Text = value;}
		}
		
		public void BindDataToCell (dish dishItem)
		{
			SpeisenTitel = dishItem.Headline;
			SpeisenPreis = string.Format ("{0} €", dishItem.Price);
			SpeisenText = dishItem.Description;
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			//any additional setup after loading the view, typically from a nib.
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Release any retained subviews of the main view.
			// e.g. myOutlet = null;
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
		

		
		
	}
}

