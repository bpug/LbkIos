using System;
using System.Linq;
using Loewenbraeu.Model;

namespace Loewenbraeu.Model.Extensions
{
	public static class QuestionExtensions
	{
		public static Answer GetRightAnswer (this Question source)
		{
			var rightAnswer = source.Answers.Where (p => p.Correct == true).FirstOrDefault ();
			return rightAnswer;
		}
	}
}

