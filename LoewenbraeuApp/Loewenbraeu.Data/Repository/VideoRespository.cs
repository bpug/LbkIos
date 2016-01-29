using System;
using System.Collections.Generic;
using Loewenbraeu.Model;

namespace Loewenbraeu.Data
{
	public class VideoRespository
	{
		public static List<Video> GetVideos ()
		{
			var videos = new List<Video> ();
			videos.Add (new Video (){ 
				Title="Löwenbräukeller Imagefilm 2012", 
				ThumbnailUri= new Uri ("http://lbkmobile.loewenbraeukeller.com/media/pictures/video/video1.jpg"),
				Url="http://lbkmobile.loewenbraeukeller.com/media/video/video1.mp4"
			});
			
			
			videos.Add (new Video (){ Title="Löwenbräukeller from youtube 1", 
				Url="http://www.youtube.com/v/ZnnuFuvMlL8"});
			
			videos.Add (new Video (){ Title="Löwenbräukeller from youtube 2", 
				Url="http://www.youtube.com/v/d1DTysM8osw"});
			videos.Add (new Video (){ Title="Löwenbräukeller from youtube 3", 
				Url="http://www.youtube.com/v/CDgPq85z8PU"});
			videos.Add (new Video (){ Title="Löwenbräukeller from youtube 4", 
				Url="http://www.youtube.com/v/wHo9g_eSzLU"});
			/*
			videos.Add (new Video (){ Title="Löwenbräukeller from youtube 5", 
				Url="http://www.youtube.com/v/xlQ7MvTE5jQ"});
			videos.Add (new Video (){ Title="Löwenbräukeller from youtube 6", 
				Url="http://www.youtube.com/v/HBfqDOHziAw"});
			videos.Add (new Video (){ Title="Löwenbräukeller from youtube 7", 
				Url="http://www.youtube.com/v/mM7mv9tMva4"});
			videos.Add (new Video (){ Title="Löwenbräukeller from youtube 8", 
				Url="http://www.youtube.com/v/X_yF9-1Y-54"});
			videos.Add (new Video (){ Title="Löwenbräukeller from youtube 9", 
				Url="http://www.youtube.com/v/S7GJ8PXLh1k"});
			videos.Add (new Video (){ Title="Löwenbräukeller from youtube 10", 
				Url="http://www.youtube.com/v/SYLtdBZAl1M"});
			videos.Add (new Video (){ Title="Löwenbräukeller from youtube 11", 
				Url="http://www.youtube.com/v/PCbwjYcUBu4"});
			videos.Add (new Video (){ Title="Löwenbräukeller from youtube 12", 
				Url="http://www.youtube.com/v/TVfO6i2xSdc"});
			videos.Add (new Video (){ Title="Löwenbräukeller from youtube 13", 
				Url="http://www.youtube.com/v/z7UVm1_XcYc"});
			*/
			return videos;
		}
	}
}