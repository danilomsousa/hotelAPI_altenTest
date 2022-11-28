using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace HotelApi
{
    public class ReservationService
    {
        private List<Reservation> Reservations { get {return _context.Reservations.ToList();} }
        private ReservationDbContext _context; 
    
        public ReservationService(ReservationDbContext context)
        {            
            _context = context;              
        }

        public List<Reservation> GetAll() => Reservations;

        public Reservation Get(int id) => Reservations.FirstOrDefault(p => p.Id == id);

        /// <summary>
        /// This method adds a new reservation.
        /// </summary>
        public void Add(Reservation reservation)
        {           
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
        }

        /// <summary>
        /// This method deletes a reservation based on the reservation id.
        /// </summary>
        public void Delete(int id)
        {
            var reservation = Get(id);
            if(reservation is null)
                return;

            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
        }
      
        /// <summary>
        /// This method generates a list of the booked dates.
        /// </summary>
        public List<DateTime> GetBookedDays(){
            List<DateTime> bookedDays = new List<DateTime>();
            foreach (var item in Reservations)
            {
                bookedDays.AddRange(Enumerable.Range(0, item.ReservationsDays).Select(index => 
                        item.StartDate.AddDays(index).Date               
                    ));
            }
            return bookedDays;
        }

        /// <summary>
        /// This method generates a list of the available dates to booking.
        /// </summary>
        public List<DateTime> GetAvaliability(){
            IEnumerable<DateTime> allDates = Enumerable.Range(1, 30).Select(index => 
                                DateTime.Now.AddDays(index).Date                
                            ).ToList();
                        
            return allDates.Except(GetBookedDays()).ToList();
        }
        

        /// <summary>
        /// This method checks the rules for a valid reservation.
        /// </summary>
        public bool ValidateReservation(Reservation reservation, out String errorMessage){
            if(reservation.EndDate.Date < reservation.StartDate.Date){
                errorMessage = "End of reservation should be before the Start!";
                return false;
            }
            if(reservation.ReservationsDays > 3){
                errorMessage = "Reservation should not have more than 3 days!";
                return false;
            }            
            errorMessage = "";
            return true;            
        }

        /// <summary>
        /// This method checks if the reservation dates are available for booking.
        /// </summary>
        public bool ValidateAvailability(Reservation reservation, out String errorMessage){            
            if(GetBookedDays().Contains(reservation.StartDate.Date)
            ||GetBookedDays().Contains(reservation.EndDate.Date)){
                errorMessage = "Dates selected are already booked! Check the available dates.";
                return false;
            }
            if(!GetAvaliability().Contains(reservation.StartDate.Date)
            || !GetAvaliability().Contains(reservation.EndDate.Date)){
                errorMessage = "Dates selected are not available for booking! Check the available dates.";
                return false;
            }
            errorMessage = "";
            return true;            
        }
    }
}