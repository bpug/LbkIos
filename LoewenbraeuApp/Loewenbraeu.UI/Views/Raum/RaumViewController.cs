using Loewenbraeu.Core;
using Loewenbraeu.Data;
using Loewenbraeu.Model;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace Loewenbraeu.UI
{
	public partial class RaumViewController : LbkDialogViewController
	{
		private const string xmlPath = "Data/raum.xml";
		
		public RaumViewController (bool pushing) : base (null, pushing, null)
		{
			Initialize ();
		}
		
		private void Initialize(){
			Title = Locale.GetText ("Räume");
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			var raums = RaumRepository.GetRaums (xmlPath);
			if (raums == null)
				return;
			
			/*
			var root = new RootElement ("Raum"){
					new Section () {
						from raum in raums select (Element)new RaumElement (raum)
					}
				};
			*/
			RootElement root = new RootElement (Title);
			
			/*
			var pvc = new PagedViewController{
				PagedViewDataSource = new RaumPagesDataSource (raums)
			};
			*/
			
			foreach (Raum raum in raums) {
				RaumElement element = new RaumElement (raum);
				element.RaumSelected += delegate(object sender, EventArgs<Raum> e) {
					this.NavigationController.PushViewController (new RaumDetailViewController (e.Value), true);
					//pvc.StartPage = e.Value.Id;
					//this.NavigationController.PushViewController (pvc, true);
				};
				root.Add (new Section { element });
			}
			
			Style = UITableViewStyle.Grouped;
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			TableView.SectionHeaderHeight = 5;
			TableView.SectionFooterHeight = 0;
			Autorotate = false;
			
			Root = root;
		}
	}
}