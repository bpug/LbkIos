using System;

namespace Loewenbraeu.Data
{
	public class BaseDBRepository
	{
		protected static Database db;
		
		static  BaseDBRepository ()
		{
			db = Database.Db;
		}
	}
}

