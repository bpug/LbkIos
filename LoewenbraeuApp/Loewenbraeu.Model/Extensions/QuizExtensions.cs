using System;
using System.Linq;
using Loewenbraeu.Model;

namespace Loewenbraeu.Model.Extensions
{
	public static class QuizExtensions
	{
		
		public static int GetTotalQuestionsCount (this Quiz source)
		{
			return  source.Questions.Count;
			
		}
		
		/*
		public static int GetTotalPoints (this Quiz source)
		{
			return  source.Questions.Count * source.PointsProAnswer;
			
		}
		
		public static int GetRightPoints (this Quiz source)
		{
			int rightCount = source.GetRightAnswerCount ();
			
			return rightCount * source.PointsProAnswer;
		}
		
		*/
		
		public static int GetTotalPoints (this Quiz source)
		{
			return source.Questions.Sum(p => p.Points);
		}
		
		
		public static int GetRightAnswerCount (this Quiz source)
		{
			int rightCount = source.Questions.Where (p => p.IsRight == true).Count();
			
			return rightCount;
		}
		
		public static int GetRightPoints (this Quiz source)
		{
			int rightPoints = source.Questions.Where (p => p.IsRight == true).Sum (p => p.Points);
			
			return rightPoints;
		}
		
		public static Question GetNextQuestion (this Quiz source)
		{
			return  source.Questions.Where (p => p.IsRight == null).FirstOrDefault ();
			
		}
		
		public static bool IsRightAnswered (this Quiz source)
		{
			int answerCount = source.Questions.Where (p => (p.IsRight == false || p.IsRight == null)).Count ();
			
			return (answerCount == 0)?true:false;
		}
	}
}

