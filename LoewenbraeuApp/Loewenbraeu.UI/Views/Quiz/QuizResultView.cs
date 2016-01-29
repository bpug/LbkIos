using System.Drawing;
using Loewenbraeu.Core.Extensions;
using Model = Loewenbraeu.Model;
using Loewenbraeu.Model.Extensions;
using MonoTouch.UIKit;
using Loewenbraeu.Core;
using Loewenbraeu.Data.Service;
using System;
using System.Timers;
using Loewenbraeu.Data;

namespace Loewenbraeu.UI
{
	public class QuizResultView: UIView
	{
		private const float sizeTitle = 18f;
		private const float sizeMessage = 14f;
		private const float sizeVoucher = 24f;
		private const float paddingLeft = 10f;
		private const float paddingBottom = 10f;
		private float y = 0f;
		
		static UIFont fontTitle = UIFont.BoldSystemFontOfSize(sizeTitle);
		static UIFont fontMessage = UIFont.BoldSystemFontOfSize(sizeMessage);
		static UIFont fontVoucher = UIFont.BoldSystemFontOfSize(sizeVoucher);
		
		private Model.Quiz _quiz;
		private UILabel _lblTitle;
		private UILabel _lblPoints;
		private UILabel _lblMessage;
		private UIScrollView _scrollView;
		private UILabel _lblVoucher;
		private UIButton _btnActivate;
		
		private ServiceAgent _serviceAgent;
		
		/*
		private UITextField _txtPhoneNumber;
		private UIButton _btnSend;
		*/
		
		public delegate void GetVoucherHandler (string phoneNumber);
		public event GetVoucherHandler GetVoucher;
		
		bool _busy = false;
		
		bool Busy {
			get {
				return _busy;
			}
			set {
				_busy = value;
				if (_busy == true) {
					//ShowHud ();
					Util.PushNetworkActive ();
				} else {
					//HideHud ();
					Util.PopNetworkActive ();
				}
			}
		}
		
		public QuizResultView (RectangleF frame, Model.Quiz quiz): base (frame)
		{
			_quiz = quiz;
			Initialize ();
		}
		
		
		private void Initialize ()
		{
			_serviceAgent = new ServiceAgent ();
			_serviceAgent.ServiceClient.ActivateVoucherCompleted += HandleActivateVoucher;
			
			_scrollView = new UIScrollView (new RectangleF (0, 0, this.Bounds.Width, this.Bounds.Height));
			
			_scrollView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions | UIViewAutoresizing.FlexibleMargins;
			
			
			_lblTitle = new UILabel (){
				AutoresizingMask = UIViewAutoresizing.FlexibleDimensions | UIViewAutoresizing.FlexibleMargins,
				TextColor = UIColor.White,
				BackgroundColor = UIColor.Clear,
				Font = fontTitle,
				Lines = 0,
				TextAlignment = UITextAlignment.Center 
			};
			
			_lblPoints = new UILabel (){
				AutoresizingMask = UIViewAutoresizing.FlexibleDimensions | UIViewAutoresizing.FlexibleMargins,
				TextColor = UIColor.White,
				BackgroundColor = UIColor.Clear,
				Font = fontMessage,
				Lines = 0,
			};
				
			_lblMessage = new UILabel (){
				AutoresizingMask = UIViewAutoresizing.FlexibleDimensions | UIViewAutoresizing.FlexibleMargins,
				TextColor = UIColor.White,
				BackgroundColor = UIColor.Clear,
				Font = fontMessage,
				Lines = 0,
			};
			
			_lblVoucher = new UILabel (){
				AutoresizingMask = UIViewAutoresizing.FlexibleDimensions | UIViewAutoresizing.FlexibleMargins,
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Center,
				Font = fontVoucher,
			};
			
			_btnActivate = new LBKButton (){
					Font = UIFont.BoldSystemFontOfSize (15),
			 };
			_btnActivate.SetTitle (Locale.GetText ("Aktivieren"), UIControlState.Normal);
			_btnActivate.TouchUpInside += delegate(object sender, System.EventArgs e){
				VoucherActivate (Voucher);
			};
			
		}
		
		public Model.QuizVoucher Voucher {get;set;}
		
		public void OnGetVoucher (string phoneNumber)
		{
			if (this.GetVoucher != null) {
				
				this.GetVoucher (phoneNumber);
			}
		}
		/*
		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);
			
			//CreateResult ();
		}
		*/
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			this.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			CreateResult ();
		}
		
		private void CreateResult ()
		{
			float paddingTop = 5f;
			
			float width = this.Bounds.Width - 2 * paddingLeft;
			float textHeight;
			
			y = paddingTop;
			
			foreach (var view in this.Subviews) {
				view.RemoveFromSuperview ();
			}
			
			_lblTitle.Text = Locale.GetText ("Gratulation!");
			textHeight = _lblTitle.GetHeight4Text (width);
			_lblTitle.Frame = new RectangleF (0, paddingTop, width, textHeight);
			_scrollView.AddSubview (_lblTitle);
			
			y += textHeight + paddingTop;
			
			_lblPoints.Text = Locale.Format ("Sie haben {0} von {1} Punkten erreicht.", _quiz.GetRightPoints (), _quiz.GetTotalPoints ());
			textHeight = _lblPoints.GetHeight4Text (width);
			_lblPoints.Frame = new RectangleF (paddingLeft, y, width, textHeight);
			_scrollView.AddSubview (_lblPoints);
			
			y += textHeight + 5;
			
			if (_quiz.IsRightAnswered ()) {
				if (Voucher != null) {
					VoucherActivate (Voucher);
				} else {
					_lblMessage.Text = Locale.GetText ("Sie haben Ihren Gutschein-Code bereits bekommen.");
					textHeight = _lblMessage.GetHeight4Text (width);
					_lblMessage.Frame = new RectangleF (paddingLeft, y, width, textHeight);
					_scrollView.AddSubview (_lblMessage);
				
					y += textHeight + paddingBottom;
					AddKrug ();
				}
			} else {
				string message = string.Empty;
				int rightCount = _quiz.GetTotalQuestionsCount () - _quiz.GetRightAnswerCount ();
				if (rightCount > 1) {
					message = Locale.Format ("Leider fehlen Ihnen {0} richtige Antworten zu einem Freibiergutschein.", 
				                         rightCount);
				} else {
					message = Locale.Format ("Leider fehlt Ihnen {0} richtige Antwort zu einem Freibiergutschein.", 
				                         rightCount);
				}
				
				message += "\n\n";
				message += Locale.GetText ("Probieren Sie die Bavaria Quiz Gaudi noch ein mal.");
				_lblMessage.Text = message;
				textHeight = _lblMessage.GetHeight4Text (width);
				_lblMessage.Frame = new RectangleF (paddingLeft, y, width, textHeight);
				_scrollView.AddSubview (_lblMessage);
				
				y += textHeight + paddingBottom;
			}
			
			_scrollView.ContentSize = new SizeF (width, y);
			this.AddSubview (_scrollView);
		}
		
		private void CreateActivateResult (bool isActivated)
		{
			float textHeight;
			float width = this.Bounds.Width - 2 * paddingLeft;
			
			string text = string.Empty;
			if (Voucher != null) {
				
				if (isActivated) {
					text = Locale.GetText ("Sie haben einen Freibiergutschein gewonnen!\nIhre Gutschein-Code:");
				} else {
					text = Locale.GetText ("Sie haben Ihren Gutschein-Code bereits bekommen.");
				}
				
				_lblMessage.Text = text;
				textHeight = _lblMessage.GetHeight4Text (width);
				_lblMessage.Frame = new RectangleF (paddingLeft, y, width, textHeight);
				_scrollView.AddSubview (_lblMessage);
				
				y += textHeight + 5;
				
				if (isActivated) {
					_lblVoucher.Text = Voucher.Code;
					textHeight = _lblVoucher.GetHeight4Text (width);
					_lblVoucher.Frame = new RectangleF (paddingLeft, y, 300, textHeight);
					
					_scrollView.AddSubview (_lblVoucher);
					y += textHeight + paddingBottom;
				}
			}
			AddKrug ();
			_scrollView.ContentSize = new SizeF (width, y);
		}
		
		private void AddKrug ()
		{
			float imgHeight = 250f;
			float imgWidth = 138;
			
			UIImageView imgViewBeer = new UIImageView ();
			imgViewBeer.Image = UIImage.FromBundle ("image/quiz/Krug.png");
			imgViewBeer.Frame = new RectangleF ((this.Bounds.Width - imgWidth) / 2, y, imgWidth, imgHeight);
			_scrollView.AddSubview (imgViewBeer);
				
			y += imgHeight + paddingBottom;
		}
		
		
		private void VoucherActivate (Model.QuizVoucher voucher)
		{
			if (Busy)
				return;
			
			if (ServiceAgent.Execute (_serviceAgent.ServiceClient.ActivateVoucherAsync, Util.DeviceUid, voucher.QuizId, voucher.Code, voucher)) {
				Busy = true;
			}
		}
		
		
		private void HandleActivateVoucher (object sender, ActivateVoucherCompletedEventArgs args)
		{
			Tuple<Timer, object> userState;
			
			bool error = ServiceAgent.HandleAsynchCompletedError (args, "ActivateVoucher");
			InvokeOnMainThread (delegate	{
				Busy = false;
				if (error)
					return;
					
				Model.QuizVoucher voucher = null;
				
				userState = args.UserState as Tuple<Timer, object>;
				if (userState != null && userState.Item2 != null) {
					voucher = userState.Item2 as Model.QuizVoucher;
				} else {
					using (var alert = new UIAlertView (Locale.GetText ("Unbekannte Fehler"), "", null, "OK", null)) {
						alert.Show ();
					}
					return;
				}
				
				bool result = args.Result;
				
				if (result == true) {
					voucher.IsActivated = true;
					QuizVoucherRepository.Update (voucher);
					CreateActivateResult (true);
				} else {
					voucher.IsUsed = true;
					voucher.Deleted = true;
					QuizVoucherRepository.Update (voucher);
					CreateActivateResult (false);
				}
					
			});
			
		}
	}
}

