using System;
using System.Collections.Generic;
using System.Linq;
using Loewenbraeu.Data.Service;

namespace Loewenbraeu.Data
{
	public class ReservationRepository : BaseDBRepository
	{
		public static void Update (Reservation reservation)
		{
			reservation.ModifyAt = DateTime.Now;
			if (reservation.Id == 0) {
				reservation.CreateAt = DateTime.Now;
				db.Insert (reservation);
			} else {
				db.Update (reservation);
			}
		}
		
		public static IEnumerable<Reservation> GetAll ()
		{
			
			return db.Table<Reservation> ().Where (p => (p.Deleted == false)).ToList ();
		}
		
		public static Reservation GetRequestedReservation ()
		{
			
			return db.Table<Reservation> ().Where (p => (p.Deleted == false && p.Status == StatusArt.Requested)).FirstOrDefault ();
		}
		
		public static Reservation GetdReservationByReservationId (string reservationId)
		{
			
			return db.Table<Reservation> ().Where (p => (p.Deleted == false && p.ReservationId == reservationId )).FirstOrDefault ();
		}
		
	}
}

