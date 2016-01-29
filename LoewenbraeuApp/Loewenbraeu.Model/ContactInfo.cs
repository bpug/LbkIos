using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Loewenbraeu.Model
{
	public class ContactInfo
	{
		[Section ("Nachricht an Löwenbräu", "")]
		
		[Caption ("Name"), Entry ("Ihre Name")]
		public string Name { get; set; }
		
		[Caption ("Telefon"), Entry (KeyboardType=UIKeyboardType.PhonePad,Placeholder="Ihr Telefonnummer")]
		public string Phone { get; set; } 
		
		[Caption ("E-Mail"), Entry (Placeholder="Ihre Email", KeyboardType=UIKeyboardType.EmailAddress)]
		public string Email { get; set; }
		
		[MultilineEntry(Height=80),Caption ("Thema")]
		public string Theme { get; set; }
		
		
		
		[Section ("\n\n")]
		[OnTap ("SendEmail")]
		[Preserve]
		[Alignment (UITextAlignment.Center)]
		public string Senden;
		
	}
}

