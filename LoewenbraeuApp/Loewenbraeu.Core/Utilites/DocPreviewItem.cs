using System.IO;
using MonoTouch.Foundation;
using MonoTouch.QuickLook;

namespace Loewenbraeu.Core
{
	public class DocPreviewItem :  QLPreviewItem
	{	
		private string _title;
		private NSUrl _url;

		public override string ItemTitle {
			get {
				return _title;
			}
		}
			
		public override NSUrl ItemUrl {
			get {
				return _url;
			}
		}
			
		public DocPreviewItem (string title, NSUrl url)
		{
			_title = title;
			_url = url;
		}
		
		public DocPreviewItem (string title, string localDocPath)
		{
			_title = title;
			_url = new NSUrl (Path.GetFullPath (localDocPath), true);
		}
	}
}

