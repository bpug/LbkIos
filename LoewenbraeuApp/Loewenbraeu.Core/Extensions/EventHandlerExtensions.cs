using System;

namespace Loewenbraeu.Core.Extensions
{
	public static  class EventHandlerExtensions
	{
		static public void RaiseEvent (this EventHandler @event, object sender, EventArgs e)
		{
			if (@event != null)
				 @event (sender, e);
		}

		static public void RaiseEvent<T> (this EventHandler<T> @event, object sender, T e)
    where T : EventArgs
		{
			if (@event != null)
				 @event (sender, e);
		}
	}
}

