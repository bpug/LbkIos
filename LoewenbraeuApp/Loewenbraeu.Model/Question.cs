using System;
using System.Collections.Generic;


namespace Loewenbraeu.Model
{
	public partial class Question
	{
		public string Text { get; set; }

		public List<Answer> Answers { get; set; }
		
		public bool? IsRight { set; get; }
		
		public QuestionCategory Category { set; get; }
		
		public int Points { get; set; }
	}
	
	public enum QuestionCategory{
		NONE,
		BAY,
		BIE,
		FOD,
		LBK,
		MUC,
		SCH
	}
}

