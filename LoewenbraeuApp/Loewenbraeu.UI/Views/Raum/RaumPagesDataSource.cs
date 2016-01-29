using System;
using System.Collections.Generic;
using Loewenbraeu.Model;
using MonoTouch.UIKit;
using Loewenbraeu.Core;

namespace Loewenbraeu.UI
{
	public class RaumPagesDataSource : IPagedViewDataSource
	{
		private List<Raum> _raums;
		public RaumPagesDataSource (List<Raum> raums)
		{
			_raums = raums;
		}
		
		public int Pages { get { return _raums.Count; } }

		public UIViewController GetPage (int i)
		{
			var rvc = new RaumDetailViewController (_raums [i]);
			return rvc;
		}

		public void Reload ()
		{
		}
	}
}

