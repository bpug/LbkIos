using System;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public static class Resources
	{
		public static class Colors
		{
			public static UIColor Background = UIColor.FromRGBA (41, 37, 99, 255);
			public static UIColor BackgroundHUD = UIColor.FromRGBA (41 / 255f, 37 / 255f, 99 / 255f, 0.8f);
			public static UIColor Cell = UIColor.FromRGBA (255, 255, 255, 255);
			public static UIColor CellText = UIColor.FromRGBA (75, 74, 100, 255);
			public static UIColor Button = UIColor.FromRGBA (209, 179, 7, 255);
			public static UIColor ButtonTitle = UIColor.FromRGBA (255, 255, 255, 255); //UIColor.FromRGBA (41, 37, 99, 255);
			
		}
		
		public static class Communication
		{
			public static string ApsRootElement ="aps";
			public static string ApsAlertElement ="alert";
			public static string ApsBadgeElement ="badge";
			public static string ApsSoundElement ="sound";
			public static string ExplodeSound = "sound";
			
		}
		
	}
}

