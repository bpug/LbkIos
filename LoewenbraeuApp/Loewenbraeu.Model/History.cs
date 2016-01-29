using System;
using System.Xml.Serialization;
using System.IO;

namespace Loewenbraeu.Model
{
	public class History : BaseEntity
	{
		public string Title { get; set; }

		public string Description { get; set; }
		
		public string Date { get; set; }
		
		public string ImageUrl { get; set; }
		
		public int PageIndex { get; set; } 
		
	}
}

