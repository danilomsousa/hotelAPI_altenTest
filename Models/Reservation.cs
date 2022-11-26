using System;

namespace HotelApi
{
    /// <summary>
    /// This class holds the information for the reservation.
    /// </summary>
    public class Reservation
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }         

        public string GuestName { get; set; }

        private int _daysCount = 1;
        public int ReservationsDays{
            get {
                return _daysCount + (EndDate.Date - StartDate.Date).Days;
            }             
        }
    }
}
