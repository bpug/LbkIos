using System;
using System.Drawing;
using System.Linq;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;

namespace Loewenbraeu.Core.Extensions
{
	public static class MKMapViewExtensions
	{
		#region ZoomToFitMapAnnotations
		
		public static  void ZoomToFitMapAnnotations (this MKMapView source, bool useUserLocation)
		{
			CLLocationCoordinate2D userCoordinate;
			userCoordinate.Latitude = -999999;
			userCoordinate.Longitude = -999999;
			
			ZoomToFitMapAnnotations (source, useUserLocation, userCoordinate);
		}
		
		public static  void ZoomToFitMapAnnotations (this MKMapView source, CLLocationCoordinate2D userCoordinate)
		{
			ZoomToFitMapAnnotations (source, true, userCoordinate);
		}

		private static  void ZoomToFitMapAnnotations (this MKMapView source, bool useUserLocation, CLLocationCoordinate2D userCoordinate)
		{
			if (source.Annotations.Count () == 0)
				return;

			CLLocationCoordinate2D topLeftCoord;
			topLeftCoord.Latitude = -90;
			topLeftCoord.Longitude = 180;

			CLLocationCoordinate2D bottomRightCoord;
			bottomRightCoord.Latitude = 90;
			bottomRightCoord.Longitude = -180;
			
			
			
			if (useUserLocation == true) {
				if (source.UserLocationVisible == true) {
					userCoordinate = source.UserLocation.Coordinate;
				} 
				if (userCoordinate.IsValid ()) {
					topLeftCoord.Longitude = Math.Min (topLeftCoord.Longitude, userCoordinate.Longitude);
					topLeftCoord.Latitude = Math.Max (topLeftCoord.Latitude, userCoordinate.Latitude);

					bottomRightCoord.Longitude = Math.Max (bottomRightCoord.Longitude, userCoordinate.Longitude);
					bottomRightCoord.Latitude = Math.Min (bottomRightCoord.Latitude, userCoordinate.Latitude);
				}
			}
			
			foreach (var ann in source.Annotations) {
				var annotation = ann as MKAnnotation;
				if (annotation != null  ) {
					topLeftCoord.Longitude = Math.Min (topLeftCoord.Longitude, annotation.Coordinate.Longitude);
					topLeftCoord.Latitude = Math.Max (topLeftCoord.Latitude, annotation.Coordinate.Latitude);

					bottomRightCoord.Longitude = Math.Max (bottomRightCoord.Longitude, annotation.Coordinate.Longitude);
					bottomRightCoord.Latitude = Math.Min (bottomRightCoord.Latitude, annotation.Coordinate.Latitude);
				}
			}

			MKCoordinateRegion region;
			region.Center.Latitude = topLeftCoord.Latitude - (topLeftCoord.Latitude - bottomRightCoord.Latitude) * 0.5;
			region.Center.Longitude = topLeftCoord.Longitude + (bottomRightCoord.Longitude - topLeftCoord.Longitude) * 0.5;
			region.Span.LatitudeDelta = Math.Abs (topLeftCoord.Latitude - bottomRightCoord.Latitude) * 1.175; // Add a little extra space on the sides
			region.Span.LongitudeDelta = Math.Abs (bottomRightCoord.Longitude - topLeftCoord.Longitude) * 1.175; // Add a little extra space on the sides

			region = source.RegionThatFits (region);  
			source.SetRegion (region, true); 
		}
		#endregion
		
		#region SetCenterCoordinate
		
		static double MERCATOR_OFFSET = 268435456;
		static double MERCATOR_RADIUS = 85445659.44705395;
		
		public static void SetCenterCoordinate (this MKMapView source, CLLocationCoordinate2D centerCoordinate, int zoomLevel, bool animated)
		{
			// clamp large numbers to 28
			zoomLevel = Math.Min (zoomLevel, 28);

			// use the zoom level to compute the region
			MKCoordinateSpan span = CoordinateSpanWithMapView (source, centerCoordinate, zoomLevel);
			MKCoordinateRegion region = new MKCoordinateRegion (centerCoordinate, span);

			// set the region like normal
			source.SetRegion (region, animated);
		}

		static double LongitudeToPixelSpaceX (double longitude)
		{
			return Math.Round (MERCATOR_OFFSET + MERCATOR_RADIUS * longitude * Math.PI / 180.0);
		}

		static double LatitudeToPixelSpaceY (double latitude)
		{
			return Math.Round (MERCATOR_OFFSET - MERCATOR_RADIUS * Math.Log ((1 + Math.Sin (latitude * Math.PI / 180.0)) / (1 - Math.Sin (latitude * Math.PI / 180.0))) / 2.0);
		}

		static double PixelSpaceXToLongitude (double pixelX)
		{
			return ((Math.Round (pixelX) - MERCATOR_OFFSET) / MERCATOR_RADIUS) * 180.0 / Math.PI;
		}

		static double PixelSpaceYToLatitude (double pixelY)
		{
			return (Math.PI / 2.0 - 2.0 * Math.Tan (Math.Exp ((Math.Round (pixelY) - MERCATOR_OFFSET) / MERCATOR_RADIUS))) * 180.0 / Math.PI;
		}

		static MKCoordinateSpan CoordinateSpanWithMapView (MKMapView mapView, CLLocationCoordinate2D centerCoordinate, int zoomLevel)
		{
			// convert center coordiate to pixel space
			double centerPixelX = LongitudeToPixelSpaceX (centerCoordinate.Longitude);
			double centerPixelY = LatitudeToPixelSpaceY (centerCoordinate.Latitude);

			// determine the scale value from the zoom level
			int zoomExponent = 20 - zoomLevel;
			double zoomScale = Math.Pow (2, zoomExponent);

			// scale the map’s size in pixel space
			SizeF mapSizeInPixels = mapView.Bounds.Size;
			double scaledMapWidth = mapSizeInPixels.Width * zoomScale;
			double scaledMapHeight = mapSizeInPixels.Height;

			// figure out the position of the top-left pixel
			double topLeftPixelX = centerPixelX - (scaledMapWidth / 2);
			double topLeftPixelY = centerPixelY - (scaledMapHeight / 2);

			// find delta between left and right longitudes
			double minLng = PixelSpaceXToLongitude (topLeftPixelX);
			double maxLng = PixelSpaceXToLongitude (topLeftPixelX + scaledMapWidth);
			double longitudeDelta = maxLng - minLng;

			// find delta between top and bottom latitudes
			double minLat = PixelSpaceYToLatitude (topLeftPixelY);
			double maxLat = PixelSpaceYToLatitude (topLeftPixelY + scaledMapHeight);
			double latitudeDelta = -1 * (maxLat - minLat);

			// create and return the lat/lng span
			MKCoordinateSpan span = new MKCoordinateSpan (latitudeDelta, longitudeDelta);

			return span;
		}
		#endregion
	}
}

