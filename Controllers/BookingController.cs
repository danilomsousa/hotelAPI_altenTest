using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

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
        public ActionResult<List<String>> GetAvaliability()
        {    
            return reservationService.GetAvaliability().Select(index => 
                index.ToString("dd/MM/yyyy")                
            )
            .ToList();               
        }

        [HttpPost]
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
        /// This method updates a reservation.
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
