using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using Loewenbraeu.Model;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public class DirektKontakController
	{
		private UINavigationController _navigation;
		private ContactInfo _contact;
		private DialogViewController _viewController;
		private BindingContext _context;
		
		public DirektKontakController (UINavigationController navigation)
		{
			_navigation = navigation;
		}
		
		public void Display ()
		{
			_contact = new ContactInfo ();
			_context = new BindingContext (this, _contact, "Kontakt");
			_viewController = new LbkDialogViewController (_context.Root, true);
			_viewController.TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			_navigation.PushViewController (_viewController, true);
		}
		
		public void SendEmail ()
		{
			UIView activeView = _viewController.TableView.FindFirstResponder ();
			if (activeView != null) {
				activeView.ResignFirstResponder ();
			}
			_context.Fetch ();
			/*
			var test2 = _viewController.TableView.FindRecursive<UITextField> (t => t.Placeholder.Contains ("Email")).FirstOrDefault ();
			if (test2.CanBecomeFirstResponder) {
				test2.BecomeFirstResponder ();
			}
			*/
			
			if (Validate ()) {
				//TODO: Send via WebService
				Util.PushNetworkActive ();
				NSTimer.CreateScheduledTimer (2, delegate {
					Util.PopNetworkActive ();
					var alert = new UIAlertView ("", Locale.GetText ("Vielen Dank für Ihre Nachricht."), null, "OK", null);
					alert.Show ();
					alert.Clicked += delegate {
						_navigation.PopToRootViewController (true);
					};
				});
			}
		}
		
		private bool Validate ()
		{
			string failText = string.Empty;
			
			if (string.IsNullOrEmpty (_contact.Email)) {
				failText = Locale.GetText ("Email ???");
			} else if (string.IsNullOrEmpty (_contact.Name)) {
				failText = Locale.GetText ("Name ???");
			} else if (string.IsNullOrEmpty (_contact.Phone)) {
				failText = Locale.GetText ("Telefon ???");
			} else if (string.IsNullOrEmpty (_contact.Theme)) {
				failText = Locale.GetText ("Thema ???");
			}
			
			if (!string.IsNullOrEmpty (failText)) {
				using (var alert =  new UIAlertView ("", failText,null,"OK",null)) {
					alert.Show ();
				}
				return false;
			}
			return true;
		}
	}
	
	
}

