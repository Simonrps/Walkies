using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;

namespace Walkies.API.Controllers
{
    /// <summary>
    /// Handles booking management for the Walkies API. Provides endpoints
    /// for creating, retrieveing and managing the booking lifecycle.
    /// Relates to US09 - Accept Walk Request, US10 - View Booking,
    /// US11 - View All Bookings, US12 - Check In, US13 - Check out
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initialises a new instance of the BookingsController with the provided
        /// </summary>
        /// <param name="context">The databse context</param>
        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var walkRequest = await _context.WalkRequests
                .Include(wr => wr.Owner)
                .Include(wr => wr.Dog)
                .FirstOrDefaultAsync(wr => wr.Id == dto.WalkRequestId);

            if (walkRequest == null)
            {
                return NotFound(new {message="Walk Request Not Found"});
            }

            var walker = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.WalkerId);

            if (walker == null)
            {
                return NotFound(new {message="Walker Not Found"});
            }

            var booking = new WalkBooking
            {
                WalkRequestId = dto.WalkRequestId,
                WalkerId = dto.WalkerId,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow
            };

            walkRequest.Status = "Accepted";

            _context.WalkBookings.Add(booking);
            await _context.SaveChangesAsync();

            var bookingDto = new BookingDto
            {
                Id = booking.Id,
                WalkRequestId = booking.WalkRequestId,
                WalkerId = booking.WalkerId,
                WalkerName = $"{walker.FirstName} {walker.LastName}",
                OwnerId = walkRequest.OwnerId,
                OwnerName = $"{walkRequest.Owner.FirstName} {walkRequest.Owner.LastName}",
                DogName = walkRequest.Dog.Name,
                ScheduledDate = walkRequest.CreatedAt,
                DurationMinutes = walkRequest.DurationMinutes,
                Status = booking.Status,
                CheckInTime = booking.CheckInTime,
                CheckOutTime = booking.CheckOutTime,
                CreatedDate = booking.CreatedAt
            };

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, bookingDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            await Task.CompletedTask;
            return StatusCode(501);
        }
    }
}
