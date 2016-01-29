using Loewenbraeu.Core;
using Loewenbraeu.Data.Service;
using Loewenbraeu.Model;
using SQLite;

namespace Loewenbraeu.Data
{
	public class Database : SQLiteConnection
	{
		internal Database (string file) : base (file)
		{
			CreateTable<QuizVoucher> ();
			CreateTable<Reservation> ();
		}
		
		static Database ()
		{
			var lbkDb = Util.BaseDir + "/Documents/lbk.db3";
			// For debugging
			//System.IO.File.Delete (lbkDb);
			
			Db = new Database (lbkDb);
		}
		
		static public Database Db { get; private set; }
	}
}

