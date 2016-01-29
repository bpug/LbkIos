using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace Loewenbraeu.Core.Extensions
{
	public static class UIViewExtensions
	{
		/// <summary>
		/// Find the first responder in the <paramref name="view"/>'s subview hierarchy
		/// </summary>
		/// <param name="view">
		/// A <see cref="UIView"/>
		/// </param>
		/// <returns>
		/// A <see cref="UIView"/> that is the first responder or null if there is no first responder
		/// </returns>
		public static UIView FindFirstResponder (this UIView view)
		{
			if (view.IsFirstResponder) {
				return view;
			}
			foreach (UIView subView in view.Subviews) {
				var firstResponder = subView.FindFirstResponder ();
				if (firstResponder != null)
					return firstResponder;
			}
			return null;
		}

		/// <summary>
		/// Find the first Superview of the specified type (or descendant of)
		/// </summary>
		/// <param name="view">
		/// A <see cref="UIView"/>
		/// </param>
		/// <param name="stopAt">
		/// A <see cref="UIView"/> that indicates where to stop looking up the superview hierarchy
		/// </param>
		/// <param name="type">
		/// A <see cref="Type"/> to look for, this should be a UIView or descendant type
		/// </param>
		/// <returns>
		/// A <see cref="UIView"/> if it is found, otherwise null
		/// </returns>
		public static UIView FindSuperviewOfType (this UIView view, UIView stopAt, Type type)
		{
			if (view.Superview != null) {
				if (type.IsAssignableFrom (view.Superview.GetType ())) {
					return view.Superview;
				}

				if (view.Superview != stopAt)
					return view.Superview.FindSuperviewOfType (stopAt, type);
			}

			return null;
		}
		
		public static void DrawRoundRectangle (this UIView view, RectangleF rrect, float radius, UIColor color)
		{
			var context = UIGraphics.GetCurrentContext ();
	
			color.SetColor ();	
			
			float minx = rrect.Left;
			float midx = rrect.Left + (rrect.Width) / 2;
			float maxx = rrect.Right;
			float miny = rrect.Top;
			float midy = rrect.Y + rrect.Size.Height / 2;
			float maxy = rrect.Bottom;
	
			context.MoveTo (minx, midy);
			context.AddArcToPoint (minx, miny, midx, miny, radius);
			context.AddArcToPoint (maxx, miny, maxx, midy, radius);
			context.AddArcToPoint (maxx, maxy, midx, maxy, radius);
			context.AddArcToPoint (minx, maxy, minx, midy, radius);
			context.ClosePath ();
			context.DrawPath (CGPathDrawingMode.Fill); // test others?
		}
		
		public static void Release (this UIView source)
		{
			if (source != null) {
				source.Dispose ();
				source = null;
			}
		}
		
		
		public static void  SetLabelsBGColor (this UIView source, UIColor color)
		{
			if (source == null)
				return;
			foreach (var view in source.Subviews) {
				if (view is UILabel) {
					((UILabel)view).BackgroundColor = color;
				}
			}
		}
		
		public static void  SetLabelsTextColor (this UIView source, UIColor color)
		{
			if (source == null)
				return;
			foreach (var view in source.Subviews) {
				if (view is UILabel) {
					((UILabel)view).TextColor = color;
				}
			}
		}
		
		
		public static TSource FirstOrDefault<TSource> (this UIView source, Func<TSource, bool> predicate) where TSource : UIView
		{
			Type targetType = typeof(TSource);
			foreach (UIView view in source.Subviews) {
				if (view.GetType () == targetType && predicate ((TSource)view)) {
					return view as TSource;
				}
				TSource recMatch = view.FirstOrDefault<TSource> (predicate);
				if (recMatch != null) {
					return recMatch;
				}
			}
			return null;
		}
		
		public static TSource FirstOrDefault<TSource> (this UIView source) where TSource : UIView
		{
			return source.FirstOrDefault<TSource> (c => true);
		}
		
		
		public static IEnumerable<TSource> FindRecursive<TSource> (this UIView source) where TSource : UIView
		{
			return source.FindRecursive<TSource> (c => true);
		}
		
		public static IEnumerable<TSource> FindRecursive<TSource> (this UIView source, int depthLimit) where TSource : UIView
		{
			return source.FindRecursive<TSource> (c => true, depthLimit);
		}
		
		public static IEnumerable<TSource> FindRecursive<TSource> (this UIView source, Func<TSource, bool> predicate) where TSource : UIView
		{
			if (source == null || source.Subviews.Count () == 0)
				return new List<TSource> ();
  
			return source.Subviews.OfType<TSource> ().Where (predicate).Union (
                source.Subviews.Cast<UIView> ().SelectMany (c => c.FindRecursive<TSource> (predicate)));
		}
		
		
		public static IEnumerable<TSource> FindRecursive<TSource> (this UIView source, Func<TSource, bool> predicate, int depthLimit) where TSource : UIView
		{
			if (source == null || source.Subviews.Count() == 0)
				return new List<TSource> ();
  
			if (depthLimit == 0) {
				return source.Subviews.OfType<TSource> ().Where (predicate);
			} else {
				return source.Subviews.OfType<TSource> ().Where (predicate).Union (
                    source.Subviews.Cast<UIView> ().SelectMany (c => c.FindRecursive<TSource> (predicate, depthLimit - 1)));
			}
		}
		
	}

}

