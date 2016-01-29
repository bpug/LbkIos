using System;
using MonoTouch.MapKit;
using MonoTouch.CoreLocation;

namespace Loewenbraeu.Core
{
	public class MapAnnotationBase : MKAnnotation
	{
		private string _title;
		private string _subtitle;
		
		
		public override CLLocationCoordinate2D Coordinate { get; set; }

		public override string Title { get { return _title; }}
		
		public override string Subtitle { get { return _subtitle; } }
		
		public MapAnnotationBase (CLLocationCoordinate2D Coordinate, string title, string subtitle)
		{
			this.Coordinate = Coordinate;
			this._title = title;
			this._subtitle = subtitle;
		}
		
	}
}

