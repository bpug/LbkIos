using System;
using Loewenbraeu.Core;

namespace Loewenbraeu.UI
{
	public static class UserDefaults
	{
		private const string jugendschutzKey = "IsJugendschutz";
		private const string speizekarteUpdateDate = "SpeisekarteUpdateDate6";
		
		public static bool Jugendschutz {
			get {
				return Util.Defaults.BoolForKey (jugendschutzKey);
			}
			set {
				Util.Defaults.SetBool (value, jugendschutzKey);
			}
		}
		

		public static DateTime? SpeisekarteUpdateDate {
			get {
				var dateStr =  Util.Defaults.StringForKey(speizekarteUpdateDate);
				DateTime date;
				if (DateTime.TryParse(dateStr, out date)){
					return date;
				}
				return null;
			}
			set {
				string date = value.ToString();
				Util.Defaults.SetString (date, speizekarteUpdateDate);
			}
		}

	}
}

