using System;
using System.Drawing;
using Loewenbraeu.Core;
using Loewenbraeu.Data;
using Loewenbraeu.Model;
using Loewenbraeu.Model.Extensions;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public partial class QuizViewController : BaseViewController
	{
		private const float VIEW_OFFSET = 10;
		
		private QuizQuestionView _questionView;
		private QuizResultView _resultView;
		private Quiz _quiz;
		private UIAlertView _alert;
		private int _questionNumber;
		
		private UIBarButtonItem _btnDone;
		private UIBarButtonItem _btnAbort;
		private UIView _inputAccessoryView;
		
		public QuizViewController () : base() //base ("QuizViewController", null)
		{
			
			Initialize ();
		}
		
		public QuizViewController (Quiz quiz) : base()
		{
			_quiz = quiz;
			Initialize ();
		}
		
		
		private void Initialize(){
			this.Title = "Bavaria Quiz Gaudi";
			_btnDone = new UIBarButtonItem ("ok", UIBarButtonItemStyle.Bordered, OnDone);
			 _btnAbort = new UIBarButtonItem ("abbrechen", UIBarButtonItemStyle.Bordered, OnAbort);
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return (toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown || toInterfaceOrientation == UIInterfaceOrientation.Portrait);
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown;
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.NavigationItem.SetHidesBackButton (true, true);
			this.NavigationItem.SetRightBarButtonItem (_btnAbort, true);
		}

		/*
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			// Release any retained subviews of the main view.
			_quiz = null;
			_questionView = null;
			_resultView = null;
		}
		*/

		protected override void Dispose (bool disposing)
		{
			if (disposing){
				_quiz = null;
				_questionView = null;
				_resultView = null;
			}
			base.Dispose (disposing);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			this.StartQuiz ();
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}
		
		public override UIView InputAccessoryView {
			get {
				if (_inputAccessoryView == null) {
					_inputAccessoryView = new UIView (new RectangleF (0, 0, 320, 27));
					_inputAccessoryView.BackgroundColor = UIColor.ViewFlipsideBackgroundColor; //UIColor.FromPatternImage (new UIImage ("Images/accessoryBG.png"));          
					UIButton dismissBtn = UIButton.FromType (UIButtonType.RoundedRect);
					dismissBtn.Frame = new RectangleF (255, 2, 58, 23);
					dismissBtn.SetTitle ("Fertig", UIControlState.Normal);
					dismissBtn.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
					//dismissBtn.SetBackgroundImage (new UIImage ("Images/dismissKeyboard.png"), UIControlState.Normal);        
					dismissBtn.TouchUpInside += delegate {
						UIView activeView = this.KeyboardGetActiveView ();
						if (activeView != null)
							activeView.ResignFirstResponder ();
					};
					_inputAccessoryView.AddSubview (dismissBtn);
				}
				return _inputAccessoryView;
			}
		}
		
		public void OnAbort (object btn, EventArgs args)
		{
			_alert = new UIAlertView ("Wollen Sie Quiz verlassen ?", "", null, "Cancel", "OK");
			_alert.Clicked += delegate(object sender, UIButtonEventArgs e) {
				if (e.ButtonIndex == 1) {
					this.NavigationController.PopViewControllerAnimated (true);
				}
			};
			_alert.Show ();
		}
		
		public void OnDone (object btn, EventArgs args)
		{
			this.NavigationController.PopViewControllerAnimated (true);
		}
		
		public void StartQuiz ()
		{
			_questionNumber = 1;
			//_quiz = QuizRepository.GetQuiz ();
			
			this.RemoteSubviews ();
			
			
			this._questionView = new QuizQuestionView (new RectangleF (VIEW_OFFSET, VIEW_OFFSET, this.View.Bounds.Width - 2* VIEW_OFFSET, this.View.Bounds.Height - 2 * VIEW_OFFSET));
			this._questionView.QuestionChanged += OnQuestionChanged;
			
			this.View.AddSubview (_questionView);
			//this.View =  (_questionView);
			
			this.NavigationItem.SetRightBarButtonItem (_btnAbort, true);
			this._questionView.TotalQuestionCount = _quiz.GetTotalQuestionsCount ();
			this._questionView.CurrentQuestionNumber = _questionNumber;
			this._questionView.TotalPoints = _quiz.GetTotalPoints ();
			this._questionView.ShowQuestion (_quiz.GetNextQuestion ());
		}

		void OnQuestionChanged (object sender, EventArgs e)
		{
			QuizQuestionView qv = sender as QuizQuestionView;
			if (qv == null)
				return;
			_questionNumber ++;
			qv.TotalQuestionCount = _quiz.GetTotalQuestionsCount ();
			qv.CurrentQuestionNumber = _questionNumber;
			qv.CurrentPoints = _quiz.GetRightPoints ();
			qv.RightAnswerCount = _quiz.GetRightAnswerCount ();
			var nextQuestion = _quiz.GetNextQuestion ();
			if (nextQuestion != null) {
				this.NavigationItem.SetRightBarButtonItem (_btnAbort, true);
				UIView.BeginAnimations ("CurlUp");
				UIView.SetAnimationDuration (1.25);
				UIView.SetAnimationCurve (UIViewAnimationCurve.EaseInOut);
				UIView.SetAnimationTransition (UIViewAnimationTransition.CurlUp, this.View, true);
					
				qv.ShowQuestion (nextQuestion);
					
				UIView.CommitAnimations ();
					
			} else {
				
				this.NavigationItem.SetRightBarButtonItem (_btnDone, true);
				
				_resultView = new QuizResultView (this.View.Bounds, _quiz);
				
				if (_quiz.IsRightAnswered ()) {
					_resultView.Voucher = GetVoucher ();
				}
				
				/*
				_resultView.GetVoucher += (phoneNumber) => {
					Console.WriteLine ("Phonenumber:" + phoneNumber);
				};
				*/
				
				UIView.BeginAnimations ("Flipper");
				UIView.SetAnimationDuration (1.25);
				UIView.SetAnimationCurve (UIViewAnimationCurve.EaseInOut);
				UIView.SetAnimationTransition (UIViewAnimationTransition.FlipFromRight, this.View, true);
				//questionView.ViewWillAppear (true);
				//resultView.ViewWillDisappear (true);
					
				qv.RemoveFromSuperview ();
				this.View.AddSubview (_resultView);

				//resultView.ViewDidDisappear (true);
				//questionView.ViewDidAppear (true);
				UIView.CommitAnimations ();
			
				/*
				UIView.Animate (2, 6, 
				      UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.TransitionFlipFromRight | UIViewAnimationOptions.AllowAnimatedContent | UIViewAnimationOptions.LayoutSubviews, 
				      () => {
					qv.RemoveFromSuperview ();
					this.View.AddSubview (_resultView);}, 
				null);
				*/
			}
		}
		
		private QuizVoucher GetVoucher ()
		{
			if (QuizVoucherRepository.GetVoucher4Quiz (_quiz.Id) == null) {
				// Get from Service
				string code = Util.RandomString(5);
				var voucher = new QuizVoucher (){ Code = code, QuizId=_quiz.Id};
				QuizVoucherRepository.Update (voucher);
				return voucher;
			}
			return null;
		}
		
		
	}
}

