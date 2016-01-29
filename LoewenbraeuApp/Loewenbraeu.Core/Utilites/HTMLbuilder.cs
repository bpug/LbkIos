using System;
using System.Text;
using MonoTouch.Foundation;

namespace Loewenbraeu.Core
{
	public class HTMLbuilder
	{
		private StringBuilder html;
		private bool closed = false;
		
		public HTMLbuilder ()
		{
			html = new StringBuilder ();
			html.Append ("<html>");
			html.Append ("<head>");
			html.Append ("<style type = \"text/css\">");
			html.Append ("body {font-family:helvetica;font-size:15;}");
			html.Append ("</style>");
			html.Append ("</head>");
			html.Append ("<body>");
		}
 
		public string Markup {
			get {
				if (!closed) {
					closed = true;
					html.Append ("</body");
					html.Append ("</html>");
				}
				return html.ToString ();
			}
		}
 
		public void Append (string content)
		{
			html.Append (content);
		}
 
		public void AppendBold (string content)
		{
			html.Append (string.Format ("<b>{0}</b>", content));
		}
		
		public void AppendCursiv (string content)
		{
			html.Append (string.Format ("<b>{0}</b>", content));
		}
 
		public void AddParagraph (string tekst)
		{
			html.Append (string.Format ("<p>{0}</p>", tekst));
		}
		
		public void AddTel (string phone)
		{
			html.Append (string.Format ("<a href = \"phone:{0}\">{0}</a>", phone));
		}
		
		public void AddPhone (string phone)
		{
			html.Append (string.Format ("<a href = \"tel:{0}\">{0}</a>", phone));
		}
 
		public string InlineImage (string image)
		{
			return string.Format ("<img src=\"file:{0}\" >", ImagePath (image));
		}
 
		private void AddButton (string image, string urlpart)
		{
			var url = string.Format ("<a href=\"{1}\"><img src=\"file:{0}\"></a>", ImagePath (image), urlpart);
			AddParagraph (url);
		}
		
		private string ImagePath (string image)
		{
			return NSBundle.MainBundle.PathForResource (string.Format ("image/{0}", image), "png");
		}
	}
}

