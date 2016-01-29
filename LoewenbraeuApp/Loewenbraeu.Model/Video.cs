using System;

namespace Loewenbraeu.Model
{
	public class Video : BaseEntity
	{
		public string Title { get; set; }
		public Uri ThumbnailUri { get; set; }
		public string Url { get; set; }
		
		public bool IsYoutube { 
			get {
				var uri = new Uri (Url);
				if (uri.Host.Contains("youtube")) {
					return true;
				}
				return false;
			}
		}
		
	}
}

