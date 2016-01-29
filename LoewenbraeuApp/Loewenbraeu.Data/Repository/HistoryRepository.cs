using System.Collections.Generic;
using Loewenbraeu.Model;
using Loewenbraeu.Core;

namespace Loewenbraeu.Data
{
	public class HistoryRepository
	{
		
		public static List<History> GetHistorys (string xmlPath)
		{
			var historys = XMLSerializer<List<History>>.Load (xmlPath);
			//historys = historys.OrderBy (p => p.PageIndex).ToList ();
			int i = 0;
			foreach (var history in historys) {
				history.PageIndex = i;
				i++;
			}
			return historys;
		}
		
	}
}

