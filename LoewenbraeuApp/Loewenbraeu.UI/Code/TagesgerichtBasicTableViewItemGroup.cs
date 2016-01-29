using System;
using System.Collections.Generic;

namespace LoewenbraeuApp5
{
	//========================================================================
	/// <summary>
	/// A group that contains table items
	/// </summary>
	public class TagesgerichtBasicTableViewItemGroup
	{
		public string Name { get; set; }

		public string Footer { get; set; }
		
		public List<TagesgerichtBasicTableViewItem> Items
		{
			get { return this._items; }
			set { this._items = value; }
		}
		protected List<TagesgerichtBasicTableViewItem> _items = new List<TagesgerichtBasicTableViewItem>();
		
		public TagesgerichtBasicTableViewItemGroup ()
		{
		}
	}
	//========================================================================
}
