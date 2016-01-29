using System;
using System.Linq;
using Service = Loewenbraeu.Data.Service;
using Model = Loewenbraeu.Model;
using Loewenbraeu.Core.Extensions;
using System.Collections.Generic;

namespace Loewenbraeu.Data.Mappings
{
	public static class QuizMapping
	{
		public static Model.Quiz ToLbkQuiz (this Service.Quiz source)
		{
			var quiz = new Model.Quiz (){
			  	PointsProAnswer = source.PointsProAnswer,
				Id = source.Id
			};
			
			if (!source.Questions.IsNullOrEmpty ()) {
				quiz.Questions = source.Questions.ToLbkQuestion();
			}
			
			return quiz;
		}
		
		public static Model.Question ToLbkQuestion (this Loewenbraeu.Data.Service.Question source)
		{
			
			var question = new Model.Question (){
			  	Text = source.Description,
				Category =  (Model.QuestionCategory)source.Category,
				Points = source.Points
			};
			
			if (!source.Answers.IsNullOrEmpty ()) {
				question.Answers = source.Answers.ToLbkAnswer ();
			}
			
			return question;
		}
		
		public static List<Model.Question> ToLbkQuestion (this  IEnumerable<Service.Question> sourceList)
		{
			return sourceList.Select (p => p.ToLbkQuestion ()).ToList ();
		}
		
		
		public static Model.Answer ToLbkAnswer (this  Service.Answer source)
		{
			var answer = new Model.Answer (){
			  	Text = source.Description,
				Correct = source.IsRight,
				Explanation = source.Explanation
			};
			
			return answer;
		}
		
		public static List<Model.Answer> ToLbkAnswer (this  IEnumerable<Service.Answer> sourceList)
		{
			return sourceList.Select (p => p.ToLbkAnswer ()).ToList ();
		}
		
		
	}
}

