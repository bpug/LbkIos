using System.Collections.Generic;
using System.Linq;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Data.Service;
using Loewenbraeu.ImageGallery;

namespace Loewenbraeu.UI.Mappings
{
	public static  class PictureMapping
	{
		public static IGImage<string> ToIGImage (this Picture source)
		{
			var image = new IGImage<string> (source.Link.GetMD5 (), source.Link, source.Title, source.Description);	
			return image;
		}
		
		
		public static List<IGImage<string>> ToIGImage (this  IEnumerable<Picture> sourceList)
		{
			return sourceList.Select (s => s.ToIGImage ()).ToList ();
		}
	}
}

