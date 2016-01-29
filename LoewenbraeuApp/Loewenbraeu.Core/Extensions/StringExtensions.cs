using System;
using System.Security.Cryptography;
using System.Text;

namespace Loewenbraeu.Core.Extensions
{
	public static class StringExtensions
	{
		/*
		public static string GetMD5 (this string source)
		{
			
			if (string.IsNullOrEmpty (source)) {
				return string.Empty;
			}
			
			MD5 md5 = new MD5CryptoServiceProvider ();
			byte[] textToHash = Encoding.Default.GetBytes (source);
			byte[] result = md5.ComputeHash (textToHash); 

			return System.BitConverter.ToString (result); 
		} 
		*/
		public static bool IsEmpty (this string source)
		{
			
			return string.IsNullOrWhiteSpace (source); 
		} 
		
		public static string Indent(this string source, int count){
			return "".PadLeft (count) + source;
		}
		
		public static string GetMD5 (this string  source)
		{
			MD5CryptoServiceProvider checksum = new MD5CryptoServiceProvider ();
			var bytes = checksum.ComputeHash (Encoding.UTF8.GetBytes (source));
			var ret = new char [32];
			for (int i = 0; i < 16; i++) {
				ret [i * 2] = (char)hex (bytes [i] >> 4);
				ret [i * 2 + 1] = (char)hex (bytes [i] & 0xf);
			}
			return new string (ret);
		}
		
		static int hex (int v)
		{
			if (v < 10)
				return '0' + v;
			return 'a' + v - 10;
		}
	}
}

