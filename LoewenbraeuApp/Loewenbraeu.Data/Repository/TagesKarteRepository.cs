using System.Collections.Generic;
using Loewenbraeu.Model;

namespace Loewenbraeu.Data
{
	public class TagesKarteRepository
	{
		public static List<SpeiseGroup> GetSpeiseGroups ()
		{
			var speiseGroups = new List<SpeiseGroup> ();
			
			SpeiseGroup tGroup;
			
			tGroup = new SpeiseGroup() { Name = "Donnerstag, 1.Dezember 2011", Footer = "" };
			tGroup.SpeiseItems.Add (new SpeiseItem() { Name = "Eiskaffee:", Preis = "€ 3,90", ImageName = "PawIcon.png" , Text="2 Kugeln Vanilleeis mit Kaffee und Schlagsahne"});
			tGroup.SpeiseItems.Add (new SpeiseItem() { Name = "Aus der Löwenkopfterrine", Preis = "€ 4,20", ImageName = "PushPinIcon.png", Text="Grießnockerlsuppe mit Gemüse und vui Schnittlauch" });
			tGroup.SpeiseItems.Add (new SpeiseItem() { Name = "Leicht & Frisch", Preis = "€ 11,50", ImageName = "LightBulbIcon.png", Text="Großer Marktsalat mit Hausdressing, Ochsenfetzen und frischen Malzbrot"});
			speiseGroups.Add (tGroup);
			
			tGroup = new SpeiseGroup() { Name = "Freitag, 2.Dezember 2011", Footer = "" };
			tGroup.SpeiseItems.Add (new SpeiseItem() { Name = "Eiskaffee:", Preis = "€ 3,90", ImageName = "PawIcon.png" , Text="2 Kugeln Vanilleeis mit Kaffee und Schlagsahne"});
			tGroup.SpeiseItems.Add (new SpeiseItem() { Name = "Aus der Löwenkopfterrine", Preis = "€ 4,20", ImageName = "PushPinIcon.png", Text="Grießnockerlsuppe mit Gemüse und vui Schnittlauch" });
			tGroup.SpeiseItems.Add (new SpeiseItem() { Name = "Leicht & Frisch", Preis = "€ 11,50", ImageName = "LightBulbIcon.png", Text="Großer Marktsalat mit Hausdressing, Ochsenfetzen und frischen Malzbrot"});
			speiseGroups.Add (tGroup);
			
			tGroup = new SpeiseGroup() { Name = "Samstag, 3.Dezember 2011", Footer = "" };
			tGroup.SpeiseItems.Add (new SpeiseItem() { Name = "Eiskaffee:", Preis = "€ 3,90", ImageName = "PawIcon.png" , Text="2 Kugeln Vanilleeis mit Kaffee und Schlagsahne"});
			tGroup.SpeiseItems.Add (new SpeiseItem() { Name = "Aus der Löwenkopfterrine", Preis = "€ 4,20", ImageName = "PushPinIcon.png", Text="Grießnockerlsuppe mit Gemüse und vui Schnittlauch" });
			tGroup.SpeiseItems.Add (new SpeiseItem() { Name = "Leicht & Frisch", Preis = "€ 11,50", ImageName = "LightBulbIcon.png", Text="Großer Marktsalat mit Hausdressing, Ochsenfetzen und frischen Malzbrot"});
			speiseGroups.Add (tGroup);
			
			tGroup = new SpeiseGroup() { Name = "Sonntag, 4.Dezember 2011", Footer = "" };
			tGroup.SpeiseItems.Add (new SpeiseItem() { Name = "Eiskaffee:", Preis = "€ 3,90", ImageName = "PawIcon.png" , Text="2 Kugeln Vanilleeis mit Kaffee und Schlagsahne"});
			tGroup.SpeiseItems.Add (new SpeiseItem() { Name = "Aus der Löwenkopfterrine", Preis = "€ 4,20", ImageName = "PushPinIcon.png", Text="Grießnockerlsuppe mit Gemüse und vui Schnittlauch" });
			tGroup.SpeiseItems.Add (new SpeiseItem() { Name = "Leicht & Frisch", Preis = "€ 11,50", ImageName = "LightBulbIcon.png", Text="Großer Marktsalat mit Hausdressing, Ochsenfetzen und frischen Malzbrot"});
			speiseGroups.Add (tGroup);
			
			return speiseGroups;
		}
	}
}

