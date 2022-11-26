using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HotelApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {      
        private readonly ILogger<BookingController> _logger;

        public BookingController(ILogger<BookingController> logger)
        {
            _logger = logger;
        }
       
        [HttpGet]
        public ActionResult<List<String>> GetAvaliability()
        {    
            return ReservationService.GetAvaliability().Select(index => 
                index.ToString("dd/MM/yyyy")                
            )
            .ToList();     
        }

        [HttpPost]
        public IActionResult Create(Reservation reservation)
        {
            String errorMessage = "";
            if(ReservationService.ValidateReservation(reservation, out errorMessage)
            && ReservationService.ValidateAvailability(reservation, out errorMessage)){
                ReservationService.Add(reservation);
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
                
            var existingReservation = ReservationService.Get(id);
            if(existingReservation is null)
                return NotFound();

            String errorMessage = "";
            if(ReservationService.ValidateReservation(reservation, out errorMessage)){
                ReservationService.Delete(id);
                if(ReservationService.ValidateAvailability(reservation, out errorMessage)){
                    ReservationService.Add(reservation);  
                    return NoContent();         
                }
                else
                {
                   ReservationService.Add(existingReservation);  
                }                
            }
            return BadRequest(errorMessage);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var reservation = ReservationService.Get(id);
        
            if (reservation is null)
                return NotFound();
            
            ReservationService.Delete(id);
        
            return NoContent();
        }
    }
}
