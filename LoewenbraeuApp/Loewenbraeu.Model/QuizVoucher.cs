using System;

namespace Loewenbraeu.Model
{
	public class QuizVoucher : BaseEntity
	{
		public string Code { get; set; }
		public int  QuizId { get; set; }
		public bool IsUsed { get; set; }
		public bool IsActivated { get; set; }
		public decimal Price { get; set; }
		
		
	}
}

