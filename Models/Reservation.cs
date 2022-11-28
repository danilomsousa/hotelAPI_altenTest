using System;
using Microsoft.EntityFrameworkCore;

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

    /// <summary>
    /// This class holds the interaction to the Entity Framework.
    /// </summary>
    public class ReservationDbContext : DbContext
    {
        public ReservationDbContext(DbContextOptions context) : base(context) { }
        public DbSet<Reservation> Reservations { get; set; }
    }
}
