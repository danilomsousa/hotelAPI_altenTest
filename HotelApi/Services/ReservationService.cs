using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelApi
{
    public static class ReservationService
    {
        static List<Reservation> Reservations { get; }
        static int nextId = 3;
    
        static ReservationService()
        {
            Reservations = new List<Reservation>
            {
                new Reservation { Id = 1, StartDate = Convert.ToDateTime("28/11/2022"), EndDate = Convert.ToDateTime("30/11/2022"), GuestName = "Danilo" },
                new Reservation { Id = 2, StartDate = Convert.ToDateTime("05/12/2022"), EndDate = Convert.ToDateTime("06/12/2022"), GuestName = "Bruna" }
            };
        }

        public static List<Reservation> GetAll() => Reservations;

        public static Reservation Get(int id) => Reservations.FirstOrDefault(p => p.Id == id);

        /// <summary>
        /// This method adds a new reservation.
        /// </summary>
        public static void Add(Reservation reservation)
        {           
            reservation.Id = nextId++;
            Reservations.Add(reservation);
        }

        /// <summary>
        /// This method deletes a reservation based on the reservation id.
        /// </summary>
        public static void Delete(int id)
        {
            var reservation = Get(id);
            if(reservation is null)
                return;

            Reservations.Remove(reservation);
        }

        /*public static void Update(Reservation reservation)
        {
            var index = Reservations.FindIndex(p => p.Id == reservation.Id);
            if(index == -1)
                return;

            Reservations[index] = reservation;
        }*/

        /// <summary>
        /// This method generates a list of the booked dates.
        /// </summary>
        public static List<DateTime> GetBookedDays(){
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
        public static List<DateTime> GetAvaliability(){
            IEnumerable<DateTime> allDates = Enumerable.Range(1, 30).Select(index => 
                                DateTime.Now.AddDays(index).Date                
                            ).ToList();
            
            return allDates.Except(GetBookedDays()).ToList();
        }
        

        /// <summary>
        /// This method checks the rules for a valid reservation.
        /// </summary>
        public static bool ValidateReservation(Reservation reservation, out String errorMessage){
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
        public static bool ValidateAvailability(Reservation reservation, out String errorMessage){            
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