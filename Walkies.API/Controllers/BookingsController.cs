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
    /// <remarks>
    /// Initialises a new instance of the BookingsController
    /// </remarks>
    /// <param name="context">The databse context</param>
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        /// Defining constants for use in the controller
        /// </summary>
        private const string BookingNotFoundMessage = "Booking Not Found";
        private const string WalkRequestNotFoundMessage = "Walk Request Not Found";
        private const string WalkerNotFoundMessage = "Walker Not Found";
        private const string WalkRequestAlreadyAcceptedMessage = "Walk Request Already Accepted";
        private const string BookingNotFoundAfterCreationMessage = "Booking Not Found After Creation";

        /// <summary>
        /// Creates a new walk booking when a walkeraccepts a walk request.
        /// Relates to US09 - Accept Walk Request. 
        /// </summary>
        /// <param name="dto">The booking details</param>
        /// <returns>201 Created with booking data n success.
        /// 400 Bad Request if the request is invalid
        /// 404 Not Found if the walk request or walker does not exist
        /// </returns>
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
                return NotFound(new { message = WalkRequestNotFoundMessage });
            }

            var walker = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.WalkerId);

            if (walker == null)
            {
                return NotFound(new { message = WalkerNotFoundMessage });
            }

            if (walkRequest.Status == "Accepted")
            {
                return BadRequest(new { message = WalkRequestAlreadyAcceptedMessage });
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

            var createdBooking = await BookingsWithIncludes()
                .FirstOrDefaultAsync(b => b.Id == booking.Id);

            if (createdBooking == null)
            {
                return NotFound(new { message = BookingNotFoundAfterCreationMessage });
            }

            return CreatedAtAction(nameof(GetBooking), new { id = createdBooking.Id }, MapToDto(createdBooking));
        }

        /// <summary>
        /// Declines a walk request on behalf of a walker
        /// Updates the walk request to declined
        /// Related to US09 - Accept or decline Request
        /// </summary>
        /// <param name="dto">booking details identifying the request to be declined</param>
        /// <returns>
        /// 200 Ok with updated walk request data on success
        /// 4040 not found if the walk request does not exist
        /// </returns>
        [HttpPost("decline")]
        public async Task<IActionResult> DeclineBooking([FromBody] CreateBookingDto dto)
        {
            var walkRequest = await _context.WalkRequests
                .Include(wr => wr.Owner)
                .Include(wr => wr.Dog)
                .FirstOrDefaultAsync(wr => wr.Id == dto.WalkRequestId);

            if (walkRequest == null)
            {
                return NotFound(new { message = WalkRequestNotFoundMessage });
            }

            walkRequest.Status = "Declined";
            await _context.SaveChangesAsync();

            return Ok(new WalkRequestDto
            {
                Id = walkRequest.Id,
                OwnerId = walkRequest.OwnerId,
                OwnerName = $"{walkRequest.Owner.FirstName} {walkRequest.Owner.LastName}",
                DogId = walkRequest.DogId,
                DogName = walkRequest.Dog.Name,
                RequestedDate = walkRequest.RequestedDate,
                DurationMinutes = walkRequest.DurationMinutes,
                Location = walkRequest.Location,
                Latitude = walkRequest.Latitude,
                Longitude = walkRequest.Longitude,
                Status = walkRequest.Status
            });
        }

        /// <summary>
        /// Retrieves a booking bu its unique identifier
        /// Related to US10 - View Booking
        /// </summary>
        /// <param name="id">The unique identifier of the booking</param>
        /// <returns>
        /// 200 OK with booking data on success
        /// 400 not Found of the booking does not exist
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await BookingsWithIncludes()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new { message = BookingNotFoundMessage });
            }

            return Ok(MapToDto(booking));
        }

        /// <summary>
        /// Retrieves all Bookings optionally filtered by owner.
        /// Results are returned in chronological order by scheduled date.
        /// Relates to US11 - View All Bookings, US13 - View Confirmed Bookings
        /// </summary>
        /// <returns>
        /// 200 Ok with a list of bookings in chronological order on success
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetBookings([FromQuery] int? ownerId = null)
        {
            var query = BookingsWithIncludes()
                .AsQueryable();

            if (ownerId.HasValue)
            {
                query = query.Where(b => b.WalkRequest.OwnerId == ownerId.Value);
            }

            var bookings = await query
                .OrderBy(b => b.WalkRequest.RequestedDate)
                .ToListAsync();

            return Ok(bookings.Select(MapToDto).ToList());
        }

        /// <summary>
        /// Records when the walker checks in for a walk.
        /// Updates the booking status to "Active" and sets the check-in time.
        /// Related to US12 - Check In
        /// </summary>
        /// <param name="id">The unique identifer of the booking</param>
        /// <returns>
        /// 200 Ok with updated booking data on success
        /// 404 Not Found if the booking does not exist
        /// </returns>
        [HttpPut("{id}/checkin")]
        public async Task<IActionResult> CheckIn(int id)
        {
            var booking = await BookingsWithIncludes()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new { message = BookingNotFoundMessage });
            }

            if (booking.Status != "Confirmed")
            {
                return BadRequest(new { message = "Only confirmed bookings can be checked in." });
            }

            booking.Status = "Active";
            booking.CheckInTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(MapToDto(booking));
        }

        /// <summary>
        /// Records the walker checking out at the end of a walk
        /// Updates the booking status to "Completed"
        /// Related to US13 - Check Out
        /// </summary>
        /// <param name="id">The bookings unique identifer</param>
        /// <returns>
        /// 200 OK with updated booking data on success
        /// 404 Not Found if the booking does not exist
        /// </returns>
        [HttpPut("{id}/checkout")]
        public async Task<IActionResult> CheckOut(int id)
        {
            var booking = await BookingsWithIncludes()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new { message = BookingNotFoundMessage });
            }

            booking.Status = "Completed";
            booking.CheckOutTime = DateTime.UtcNow;

            // Calculate payment amount based on walk duration and fixed rate
            const decimal ratePerMinute = 0.50m;
            var amount = booking.WalkRequest.DurationMinutes * ratePerMinute;

            var payment = new PaymentRecord
            {
                WalkBookingId = booking.Id,
                Amount = amount,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentRecords.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(MapToDto(booking));
        }

        /// <summary>
        /// Cancels a confirmed booking for either an owner or walker.
        /// Updates the booking status to "Cancelled" and returns the
        /// walk request to Open status
        /// Related to US11 - Cancellation
        /// </summary>
        /// <param name="id">Unique identifier for booking</param>
        /// <returns>
        /// 200 OK with updated booking data on success
        /// 404 not found if the booking does not exist
        /// </returns>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await BookingsWithIncludes()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new { message = BookingNotFoundMessage });
            }

            booking.Status = "Cancelled";
            booking.WalkRequest.Status = "Open";

            await _context.SaveChangesAsync();

            return Ok(MapToDto(booking));
        }

        /// <summary>
        /// Updates a walkers current location during an active walk.
        /// Called at regular intervals by the MAUI frontend to track
        /// the walkers location in real time.
        /// Related to US15 - GPS Tracking
        /// </summary>
        /// <param name="id">Unique identifier for booking</param>
        /// <param name="dto">The unique location coordinates</param>
        /// <returns>
        /// 200 OK with updated booking data on success
        /// 400 Bad Request if the booking is not active
        /// 404 not found if the booking does not exist
        /// </returns>
        [HttpPut("{id}/location")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateLocationDto dto)
        {
            var booking = await BookingsWithIncludes()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new { message = BookingNotFoundMessage });
            }

            if (booking.Status != "Active")
            {
                return BadRequest(new { message = "Location can only be updated for active bookings." });
            }

            booking.CurrentLatitude = dto.Latitude;
            booking.CurrentLongitude = dto.Longitude;

            await _context.SaveChangesAsync();

            return Ok(MapToDto(booking));
        }

        /// <summary>
        /// Maps a WalkBooking to a BookingDto
        /// </summary>
        private static BookingDto MapToDto(WalkBooking booking) => new()
        {
            Id = booking.Id,
            WalkRequestId = booking.WalkRequestId,
            WalkerId = booking.WalkerId,
            WalkerName = $"{booking.Walker.FirstName} {booking.Walker.LastName}",
            OwnerId = booking.WalkRequest.OwnerId,
            OwnerName = $"{booking.WalkRequest.Owner.FirstName} {booking.WalkRequest.Owner.LastName}",
            DogName = booking.WalkRequest.Dog.Name,
            ScheduledDate = booking.WalkRequest.RequestedDate,
            DurationMinutes = booking.WalkRequest.DurationMinutes,
            Location = booking.WalkRequest.Location,
            Status = booking.Status,
            CheckInTime = booking.CheckInTime,
            CheckOutTime = booking.CheckOutTime,
            CurrentLatitude = booking.CurrentLatitude,
            CurrentLongitude = booking.CurrentLongitude,
            CreatedAt = booking.CreatedAt
        };

        /// <summary>
        /// Returns a base query for WalkBookings with all required
        /// navigation properties included. This ccentralises the 
        /// include logic to avoid duplication across endpoints
        /// </summary>
        private IQueryable<WalkBooking> BookingsWithIncludes() =>
            _context.WalkBookings
                .Include(b => b.Walker)
                .Include(b => b.WalkRequest)
                .ThenInclude(wr => wr.Owner)
                .Include(b => b.WalkRequest)
                .ThenInclude(wr => wr.Dog);
    }
}