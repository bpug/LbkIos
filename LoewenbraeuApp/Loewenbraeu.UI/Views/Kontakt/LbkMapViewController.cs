using System.Drawing;
using Loewenbraeu.Core;
using Loewenbraeu.Core.Extensions;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using System;

namespace Loewenbraeu.UI
{
	public class LbkMapViewController : BaseViewController
	{
		private MKMapView _mapView;
		CLLocationCoordinate2D _lbkCoords;
		private CLLocationCoordinate2D _userCoords;
		private UISegmentedControl _sgMapType;
		
		public LbkMapViewController (CLLocationCoordinate2D lbkCoords, CLLocationCoordinate2D userCoords) : base()
		{
			_lbkCoords = lbkCoords;
			_userCoords = userCoords;
			Initialize ();
		}
		
		private void Initialize ()
		{
			Title = Locale.GetText ("");
			_mapView = new MKMapView ();
			_mapView.AutoresizingMask = UIViewAutoresizing.All;
			_mapView.MapType = MKMapType.Standard;
			_mapView.ShowsUserLocation = true;
			
			_sgMapType = new UISegmentedControl ();
			_sgMapType.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			_sgMapType.Opaque = true;
			_sgMapType.Alpha = 0.75f;
			_sgMapType.ControlStyle = UISegmentedControlStyle.Bar;
			_sgMapType.InsertSegment ("Map", 0, true);
			_sgMapType.InsertSegment ("Satellite", 1, true);
			_sgMapType.InsertSegment ("Hybrid", 2, true);
			_sgMapType.SelectedSegment = 0;
			_sgMapType.ValueChanged += (s, e) => {
				switch (_sgMapType.SelectedSegment) {
				case 0:
					_mapView.MapType = MKMapType.Standard;
					break;
				case 1:
					_mapView.MapType = MKMapType.Satellite;
					break;
				case 2:
					_mapView.MapType = MKMapType.Hybrid;
					break;
				}
			};
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			float segmentOffsetX = 20;
			_mapView.Frame = this.View.Bounds;
			_sgMapType.Frame = new RectangleF (segmentOffsetX, this.View.Bounds.Height - 40, this.View.Bounds.Width - 2 * segmentOffsetX, 30);
			
			
			_mapView.Delegate = new MapDelegate ();
			//_mapView.SetCenterCoordinate (_lbkCoords, 16, true);
			
			var annotation = new MapAnnotationBase (_lbkCoords, "Löwenbräukeller", "Nymphenburgerstrasse 2, 80335 München");
			_mapView.AddAnnotation (annotation);
			
			
			this.View.AddSubview (_mapView);
			this.View.AddSubview (_sgMapType);
			//_mapView.ZoomToFitMapAnnotations (true, _userCoords);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			_mapView.ZoomToFitMapAnnotations (_userCoords);
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			_mapView.ShowsUserLocation = false;
		}

		/*
		public override void ViewDidUnload ()
		{
			_mapView = null;
			base.ViewDidUnload ();
		}
		*/

		protected override void Dispose (bool disposing)
		{
			if (disposing){
				_mapView = null;
			}
			base.Dispose (disposing);
		}
		
		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			_mapView.ZoomToFitMapAnnotations (_userCoords);
		}

		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		class MapDelegate :  MKMapViewDelegate{
			private const string ANNOTATION_ID = "LbKAnnotation";
			
			
			
			public override MKAnnotationView GetViewForAnnotation (MKMapView mapView, MonoTouch.Foundation.NSObject annotation)
			{
				// if it's the user location, just return nil.
				if (annotation is MKUserLocation)
					return null;
				
				
				MKAnnotationView pinView = mapView.DequeueReusableAnnotation (ANNOTATION_ID);
				//MKPinAnnotationView pinView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation (ANNOTATION_ID);
				if (pinView == null) {
					pinView = new MKPinAnnotationView (annotation, ANNOTATION_ID);
					/*
					MKPinAnnotationView customPinView = new MKPinAnnotationView (annotation, ANNOTATION_ID);
					customPinView.PinColor = MKPinAnnotationColor.Red;
					customPinView.AnimatesDrop = true;
					customPinView.CanShowCallout = true;
					customPinView.Selected = true;
					
					float height = customPinView.Frame.Height;
					UIImage img = UIImage.FromFile ("image/buttons/lbk_app.png");
					
					height -= 6;
					UIImageView imgView = new UIImageView (img.Thumbnail (new Size ((int)height, (int)height)));
					customPinView.LeftCalloutAccessoryView = imgView;
					
					return customPinView;
					*/
				} else {
					pinView.Annotation = annotation;
				}
				
				(pinView as MKPinAnnotationView).PinColor = MKPinAnnotationColor.Red;
				(pinView as MKPinAnnotationView).AnimatesDrop = true;
				pinView.CanShowCallout = true;
				
				UIImageView imgView = new UIImageView (UIImage.FromFile ("image/buttons/lbk_map.png"));
				pinView.LeftCalloutAccessoryView = imgView;
				pinView.Selected = true;
				
				return pinView;
			}
		}
	}
}

