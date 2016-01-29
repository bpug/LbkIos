using System;
using System.Collections.Generic;
using System.Drawing;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Model;
using Loewenbraeu.Model.Extensions;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public class QuizQuestionView : RoundedRectView
	{
		//public delegate void QuestionChangedHandler(int questionIndex);
		public event EventHandler QuestionChanged;
		
		private UITableView  _tableView;
		private UIImageView _imageView;
		private float _imageHeight = 179f;
		private static float quizSize = 16f;
		private static float pointSize = 14f;
		UIFont quizFont = UIFont.BoldSystemFontOfSize (quizSize);
		UIFont pointFont = UIFont.SystemFontOfSize (pointSize);
	
		public QuizQuestionView (RectangleF frame) : base (frame, UIColor.White, 8)
		{
			this.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions | UIViewAutoresizing.FlexibleMargins;
			//this.BackgroundColor = UIColor.Red;
			_tableView = new UITableView (new RectangleF (0, 0, frame.Width, frame.Height - _imageHeight), UITableViewStyle.Plain);
			_tableView.ScrollEnabled = false;
			_tableView.BackgroundColor = UIColor.Clear;
			_tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			//_tableView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions | UIViewAutoresizing.FlexibleMargins;
			//_tableView.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			this.AddSubview (_tableView);
			
			
			_imageView = new UIImageView ();
			//_imageView.AutoresizingMask =  UIViewAutoresizing.FlexibleMargins;
			this.AddSubview (_imageView);
			
		}
		
		public Question CurrentQuestion {get;set;}
		
		public int CurrentPoints {
			get;
			set;
		}
		
		public int RightAnswerCount {
			get;
			set;
		}
		
		public int TotalQuestionCount {
			get;
			set;
		}
		
		public int TotalPoints {
			get;
			set;
		}
		
		public int CurrentQuestionNumber {
			get;
			set;
		}
		
		public void OnQuestionChanged (EventArgs e)
		{
			if (this.QuestionChanged != null) {
				
				this.QuestionChanged (this, e);
			}
		}
		/*
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
		}
		*/
		public void ShowQuestion (Question question)
		{
			CurrentQuestion = question;
			
			this.BeginInvokeOnMainThread (delegate { 
				_tableView.TableHeaderView = GetHeaderView ();
				this._tableView.Source = new QuizTableViewsource (this, CurrentQuestion.Answers);
				this._tableView.ReloadData ();
				
			});
			
			SetGategoryImage ();
		}
		
		private void  SetGategoryImage ()
		{
			string category = CurrentQuestion.Category.ToString ();
			string imagaPath = string.Format ("image/quiz/{0}.jpg", category);
			UIImage image = UIImage.FromFile (imagaPath);
			
			_imageView.Image = image;
			_imageView.Frame = new RectangleF (0, this.Frame.Height - _imageHeight, this.Frame.Width, _imageHeight);
		}
		
		private UIView GetHeaderView ()
		{
			float paddingLeft = 10f;
			float paddingBottom = 10f;
			float textHeight = 0;
			float y = 3;
			float textWidth = this.Frame.Width - paddingLeft * 2;
			
			var lblCounter = new UILabel (){
				TextColor = Resources.Colors.Background,
				Font = pointFont,
				BackgroundColor = UIColor.Clear,
				Text = Locale.Format ("Frage {0} von {1}", CurrentQuestionNumber, TotalQuestionCount),
			};
			textHeight = lblCounter.GetHeight4Text (textWidth);
			lblCounter.Frame = new RectangleF (paddingLeft, y, textWidth, textHeight);
			
			
			y += textHeight;
			var lblPunkten = new UILabel (){
				TextColor = Resources.Colors.Background,
				BackgroundColor = UIColor.Clear,
				Font = pointFont,
				Text = Locale.Format ("{0} von {1} Punkten erreicht", CurrentPoints, TotalPoints)
			};
			textHeight = lblPunkten.GetHeight4Text (textWidth);
			lblPunkten.Frame = new RectangleF (paddingLeft, y, textWidth, textHeight);
			y += textHeight;
			
			var topView = new UIView ();
			topView.BackgroundColor = UIColor.FromRGB (219, 219, 219);
			topView.Frame = new RectangleF (0, 0, this.Frame.Width, y + 5);
			
			y += 5;
			/*
			var lblFrage = new UILabel (){
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
				TextColor = Resources.Colors.Button,
				BackgroundColor = UIColor.Clear,
				Font = quizFont,
				Text = CurrentQuestion.Text,
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0,
			};
			textHeight = lblFrage.GetHeight4Text (textWidth);
			lblFrage.Frame = new RectangleF (paddingLeft, y, textWidth, textHeight);
			*/
			var txtFrage = new UITextView (){
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
				TextColor = Resources.Colors.Button,
				BackgroundColor = UIColor.Clear,
				Font = Util.IsPhone5() ? UIFont.BoldSystemFontOfSize (quizSize) : UIFont.BoldSystemFontOfSize (quizSize - 2),
				Text = CurrentQuestion.Text,
				UserInteractionEnabled = true,
				Editable= false,
				ShowsVerticalScrollIndicator = false,
				ScrollsToTop = true
			};
		

			textHeight = Util.IsPhone5() ? 150 : 95;
			txtFrage.Frame = new RectangleF (paddingLeft-5, y, textWidth + 5, textHeight);
			
			y += textHeight;
			var view = new UIView (){
			//AutoresizingMask = UIViewAutoresizing.FlexibleDimensions | UIViewAutoresizing.FlexibleMargins,
				BackgroundColor = UIColor.Clear,
				Frame = new RectangleF (0, 0, this.Frame.Width, y),
			};
			
			topView.AddSubview (lblCounter);
			topView.AddSubview (lblPunkten);
			
			view.AddSubview (topView);
			view.AddSubview (txtFrage);
			//view.AddSubview (lblFrage);
			return view;
		}
		
		class QuizTableViewsource : UITableViewSource
		{
			private List<Answer> _tableItems;
			private QuizQuestionView _tvc;
			protected string _customCellIdentifier = "AnswerCell";
			
			public QuizTableViewsource (QuizQuestionView tvc, List<Answer> tableItems)
			{
				this._tvc = tvc;
				this._tableItems = tableItems;
			}
			
			public QuizTableViewsource (List<Answer> tableItems)
			{
				this._tableItems = tableItems;
			}
			
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
				
			}

			/// <summary>
			/// Called by the TableView to determine how many cells to create for that particular section.
			/// </summary>
			public override int RowsInSection (UITableView tableview, int section)
			{
				return this._tableItems.Count;
			}
			
			public override float GetHeightForRow (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				return Util.IsPhone5() ? 33 : 25f;
			}
			
			/// <summary>
			/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
			/// </summary>
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell (this._customCellIdentifier);
				Answer item = this._tableItems [indexPath.Row];
				
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, "Answer");
				}
				
				var backgroundView = new UIView (cell.Frame);
				backgroundView.BackgroundColor = UIColor.Clear;
				cell.SelectedBackgroundView = backgroundView;
				cell.TextLabel.Text = string.Format ("{0}  {1}", indexPath.Row.ToLetter(), item.Text);
				cell.TextLabel.TextColor = Resources.Colors.Background;
				cell.TextLabel.Font = UIFont.BoldSystemFontOfSize (18f);
				cell.TextLabel.Frame.Width = 20f;
				cell.TextLabel.HighlightedTextColor = UIColor.Red;
				
//				cell.DetailTextLabel.Text = (indexPath.Row + 1).ToString ();
//				cell.DetailTextLabel.TextAlignment = UITextAlignment.Left;
//				cell.DetailTextLabel.TextColor = UIColor.White;
//				cell.DetailTextLabel.Font = UIFont.BoldSystemFontOfSize (18f);
				
				return cell;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				Answer answer = this._tableItems [indexPath.Row];
				//var cell = tableView.CellAt (indexPath) as UITableViewCell;
				
				//tableView.DeselectRow (indexPath, true);
				//NSIndexPath[] arIndexPath = {indexPath};
				//tableView.ReloadRows(arIndexPath, UITableViewRowAnimation.Fade);
				string text = string.Empty;
				string message = string.Empty;//UIDevice.CurrentDevice.UniqueIdentifier;//
				text = _tvc.CurrentQuestion.GetRightAnswer().Explanation;
				if (answer.Correct) {
					message += "Richtig!";
					this._tvc.CurrentQuestion.IsRight = true;
				} else {
					this._tvc.CurrentQuestion.IsRight = false;
					message += "Falsch!";
				}
				//message += Environment.NewLine + Environment.NewLine;
				var alert = new UIAlertView (message, text, null, null, new string[]{"Weiter"});
				alert.CancelButtonIndex = 0;
				alert.Show ();
				alert.Clicked += (sender, buttonArgs) => {
					this._tvc.OnQuestionChanged (EventArgs.Empty);
				};
				
			}

		}

	}
}

