using System;
using System.Linq;
using Loewenbraeu.Data.Service;
using Loewenbraeu.Model;
using Loewenbraeu.Core.Extensions;
using System.Collections.Generic;

namespace Loewenbraeu.Data.Mappings
{
	public static class VideoMapping
	{
		public static Loewenbraeu.Model.Video ToLbkVideo (this Loewenbraeu.Data.Service.Video source)
		{
			var video = new Loewenbraeu.Model.Video (){
			  	Title = source.Description,
				Url = source.Link
			};
			
			if (!source.ThumbnailLink.IsEmpty ()) {
				video.ThumbnailUri = new Uri (source.ThumbnailLink);
			}
			
			return video;
		}
		
		public static List<Loewenbraeu.Model.Video> ToLbkVideo (this  IEnumerable<Loewenbraeu.Data.Service.Video> sourceList)
		{
			if (!sourceList.IsNullOrEmpty ())
				return sourceList.Select (p => p.ToLbkVideo ()).ToList ();
			return null;
		}
	}
}

