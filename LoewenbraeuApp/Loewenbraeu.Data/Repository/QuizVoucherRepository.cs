using System;
using System.Collections.Generic;
using System.Linq;
using Loewenbraeu.Model;

namespace Loewenbraeu.Data
{
	public class QuizVoucherRepository : BaseDBRepository
	{
		/*
		static Database db;
		
		static QuizVoucherRepository ()
		{
			db = Database.Db;
		}
		*/
		
		public static QuizVoucher GetVoucher (int id)
		{
			
			return db.Table<QuizVoucher> ().Where (p => p.Id == id).FirstOrDefault ();
		}
		
		public static IEnumerable<QuizVoucher> GetAll ()
		{
			
			return db.Table<QuizVoucher> ().Where (p => (p.Deleted == false)).ToList ();
		}
		
		public static QuizVoucher GetVoucher4Quiz (int quizID)
		{
			
			return db.Table<QuizVoucher> ().Where (p => (p.Deleted == false && p.QuizId == quizID)).FirstOrDefault();
		}
		
		public static List<QuizVoucher> GetNotUsedVouchers ()
		{
			
			return db.Table<QuizVoucher> ().Where (p => (p.Deleted == false && p.IsUsed == false)).ToList();
		}
		
		
		
		public static void Update (QuizVoucher voucher)
		{
			voucher.ModifyAt = DateTime.Now;
			if (voucher.Id == 0) {
				voucher.CreateAt = DateTime.Now;
				db.Insert (voucher);
			} else {
				db.Update (voucher);
			}
		}
		
		
	}
}

