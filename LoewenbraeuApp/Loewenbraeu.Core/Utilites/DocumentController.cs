using System.Collections.Generic;
using MonoTouch.QuickLook;
using MonoTouch.UIKit;
using Loewenbraeu.Core;

namespace Loewenbraeu.UI
{
	public class DocumentController
	{
		private UINavigationController _navigation;
		private QLPreviewController pc;
		
		public DocumentController (UINavigationController navigation)
		{
			_navigation = navigation;
			pc = new QLPreviewController ();
			pc.ModalInPopover = false;
		}
		
		public void Display (List<DocPreviewItem> docPreviewItems)
		{
			pc.DataSource = new DocumentDataSource (docPreviewItems);
			_navigation.PresentViewController (pc, true, null);
		}
		
		private  class DocumentDataSource : QLPreviewControllerDataSource
		{
			
			private List<DocPreviewItem> _docPreviewItems;
			
			public DocumentDataSource (List<DocPreviewItem> docPreviewItems)
			{
				_docPreviewItems = docPreviewItems;
			}
			
			public override int PreviewItemCount (QLPreviewController controller)
			{
				return _docPreviewItems.Count;
			}

			public override QLPreviewItem GetPreviewItem (QLPreviewController controller, int index)
			{
				return _docPreviewItems [index];
			}
		}
		
	}
	
}

