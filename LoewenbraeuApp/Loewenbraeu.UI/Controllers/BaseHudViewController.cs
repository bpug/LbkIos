using System;
using Loewenbraeu.Core;
using MonoTouch.Foundation;

namespace Loewenbraeu.UI
{
	public abstract class BaseHudViewController : BaseViewController
	{
		//protected MBProgressHUD Hud;
		
		public BaseHudViewController () : base()
		{
		}
		
		public BaseHudViewController (IntPtr handle) : base (handle){
			
		}
		
		public BaseHudViewController (string nibName, NSBundle bundle ) : base(nibName, bundle)
		{
			
		}
		
		protected void ShowHud ()
		{
			Hud = new MBProgressHUD (this.View){
				Mode = MBProgressHUDMode.Indeterminate,
			};
			this.View.AddSubview (Hud);
			Hud.Show (true);
		}

		/*
		protected void HideHud ()
		{
			if (Hud == null)
				return;
			
			Hud.Hide (true);
			Hud.RemoveFromSuperview ();
			Hud = null;
		}
		*/
	}
}

