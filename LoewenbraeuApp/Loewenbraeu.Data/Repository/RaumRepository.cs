using System.Collections.Generic;
using Loewenbraeu.Model;
using Loewenbraeu.Core;
using System.IO;

namespace Loewenbraeu.Data
{
	public class RaumRepository
	{
		
		public static List<Raum> GetRaums (string xmlPath)
		{
			var raums = XMLSerializer<List<Raum>>.Load (xmlPath);
			
			int i = 0;
			foreach (var raum in raums) {
				raum.Id = i;
				i++;
			}
			
			return raums;
		}
		
		public static void  SaveRaumsTest ()
		{
			
			
			var raums = new List<Raum> ();
			var urls = new List<string> ();
			urls.Add ("1111");
			urls.Add ("2222");
			
			raums.Add (new Raum () { 
				Title ="Festsaal",
				Description ="Mit Bühne, zwei Küchen, drei Schänken und zur Terrasse hin komplett zu öffnenden Glasfronten.",
				Area = 1665,
				ThumbnailUrl = "Festsaal.jpg",
				ImageUrl = urls
			//ImageUrl = "Festsaal.jpg"
			});
			
			raums.Add (new Raum () { 
				Title ="Galeriesaal",
				Description ="Der Galeriesaal vermittelt umlaufend von drei Seiten aus einen fantastischen Blick auf den Festsaal.",
				Area = 259,
				ThumbnailUrl = "Galeriesaal.jpg",
				ImageUrl = urls
			//ImageUrl = "Galeriesaal.jpg"
			});
			
			var path = Path.Combine (Util.BaseDir, "raumTest.xml");
			XMLSerializer<List<Raum>>.Save (raums, path);
			
			
		}
		
	}
}

