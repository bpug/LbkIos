using System;
using System.Collections.Generic;

namespace Loewenbraeu.Model
{
	public partial class Quiz
	{
		public List<Question> Questions { get; set; }
		
		public int PointsProAnswer { get; set; }
		
		public int Id  { get; set; }
	}
}

