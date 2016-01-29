using System;
using System.Linq;
using Loewenbraeu.Data.Service;
using Loewenbraeu.Model;
using Loewenbraeu.Core.Extensions;
using System.Collections.Generic;

namespace Loewenbraeu.Data.Mappings
{
	public static class EventMapping
	{
		public static EventOrder ToOrder (this Loewenbraeu.Data.Service.Event source)
		{
			var eventOrder = new EventOrder (){
			  	Title = source.Title,
				Url = source.ReservationLink
			};
			
			return eventOrder;
		}
	}
}

