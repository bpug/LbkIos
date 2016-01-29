using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace Loewenbraeu.Model
{
	public class Raum : BaseEntity
	{
		public string Title { get; set; }
		public string Subtitle { get; set; }
		public string Description { get; set; }
		public List<string> ImageUrl { get; set; }
		public string ThumbnailUrl { get; set; }
		public string Seats { get; set; }
		public int? Area { get; set; }
		
		//[XmlIgnore]
		//public int PageIndex { get; set; }
		
/*
		[XmlIgnore]
		public Uri ImageUri {
			get { 
				if (!string.IsNullOrEmpty (this.ImageUrl)) {
					return 	new Uri ("file://" + Path.GetFullPath (ImageUrl));
				}
				return null;
			}
		}
*/
		[XmlIgnore]
		public Uri ThumbnailUri { 
			get { 
				if (!string.IsNullOrEmpty (this.ThumbnailUrl)) {
					return 	new Uri ("file://" + Path.GetFullPath (ThumbnailUrl));
				}
				return null;
			}
		}
	}
}