using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {      
        private ReservationService reservationService;
        public BookingController(ReservationDbContext context)
        {
            reservationService = new ReservationService(context);
        }         

       
        [HttpGet]
        /// <summary>
        /// This method gets all the dates available for booking.
        /// </summary>
        public ActionResult<List<String>> GetAvaliability()
        {    
            return reservationService.GetAvaliability().Select(index => 
                index.ToString("dd/MM/yyyy")                
            )
            .ToList();               
        }

        [HttpGet("{id}")]
        /// <summary>
        /// This method gets the select reservation.
        /// </summary>
        public IActionResult GetReservation(int id)
        {    
            var existingReservation = reservationService.Get(id);
            if(existingReservation is null)
                return NotFound();

            return Ok(existingReservation);
        }

        [HttpPost]
        /// <summary>
        /// This method creates a new reservation.
        /// </summary>
        public IActionResult Create(Reservation reservation)
        {
            String errorMessage = "";
            if(reservationService.ValidateReservation(reservation, out errorMessage)
            && reservationService.ValidateAvailability(reservation, out errorMessage)){
                reservationService.Add(reservation);
                return CreatedAtAction(nameof(Create), new { id = reservation.Id }, reservation);
            }                               
            return BadRequest(errorMessage);
        }

        [HttpPut("{id}")]
        /// <summary>
        /// This method updates a reservation. Body id and put id should be the same
        /// </summary>
        public IActionResult Update(int id, Reservation reservation)
        {
            if (id != reservation.Id)
                return BadRequest();
                
            var existingReservation = reservationService.Get(id);
            if(existingReservation is null)
                return NotFound();

            String errorMessage = "";
            if(reservationService.ValidateReservation(reservation, out errorMessage)){
                reservationService.Delete(id);
                if(reservationService.ValidateAvailability(reservation, out errorMessage)){
                    reservationService.Add(reservation);  
                    return NoContent();         
                }
                else
                {
                   reservationService.Add(existingReservation);  
                }                
            }
            return BadRequest(errorMessage);
        }

        [HttpDelete("{id}")]
        /// <summary>
        /// This method deletes a reservation.
        /// </summary>
        public IActionResult Delete(int id)
        {
            var reservation = reservationService.Get(id);
        
            if (reservation is null)
                return NotFound();
            
            reservationService.Delete(id);
        
            return NoContent();
        }
    }
}
