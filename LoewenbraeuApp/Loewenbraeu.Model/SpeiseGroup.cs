using System;
using System.Collections.Generic;

namespace Loewenbraeu.Model
{
	public class SpeiseGroup
	{
		private List<SpeiseItem> _items = new List<SpeiseItem>();
		
		public string Name { get; set; }

		public string Footer { get; set; }
		
		public List<SpeiseItem> SpeiseItems
		{
			get { return this._items; }
			set { this._items = value; }
		}
		
	}
}

