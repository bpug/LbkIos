using System;
using System.IO;

namespace Loewenbraeu.Data.Service
{
	public partial class Event
	{
		public Uri ThumbnailUri {
			get { 
				if (!string.IsNullOrEmpty (this.ThumbnailLink)) {
					return new Uri (this.ThumbnailLink);
				}
				return null;
			}
		}
		
	}
}

