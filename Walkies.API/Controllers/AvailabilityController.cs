using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;

namespace Walkies.API.Controllers
{
    /// <summary>
    /// Handles the availability management for dog walkers.
    /// Provides endpoints for setting, retrieving and removing
    /// walker availability slots. Related to US12 - Walker Availability
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AvailabilityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initialises a new instance of the AvailabilityController
        /// </summary>
        /// <param name="context">The database context</param>
        public AvailabilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new availability slot for a walker.
        /// Relates to US12 - Walker Availability
        /// </summary>
        /// <param name="dto">The availability slot details</param>
        /// <returns>
        /// 201 created with availability slot details  on success
        /// 400 bad request if the request is invalid
        /// 404 not found if the walker does not exist
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SetAvailability([FromBody] CreateAvailabilityDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var walker = await _context.Users.FirstOrDefaultAsync(w => w.Id == dto.WalkerId);
            if (walker == null)
            {
                return NotFound(new {message="Walker not found"});
            }

            var availability = new WalkerAvailability
            {
                WalkerId = dto.WalkerId,
                AvailableFrom = dto.AvailableFrom,
                AvailableTo = dto.AvailableTo,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.WalkerAvailabilities.Add(availability);
            await _context.SaveChangesAsync();

            var availabilityDto = new AvailabilityDto
            {
                Id = availability.Id,
                WalkerId = availability.WalkerId,
                WalkerName = $"{walker.FirstName} {walker.LastName}",
                AvailableFrom = availability.AvailableFrom,
                AvailableTo = availability.AvailableTo,
                IsAvailable = availability.IsAvailable,
                CreatedAt = availability.CreatedAt
            };

            return CreatedAtAction(nameof(GetAvailability), new { walkerId = walker.Id }, availabilityDto);
        }

        [HttpGet("{walkerId}")]
        public async Task<IActionResult> GetAvailability(int walkerId)
        {
            await Task.CompletedTask;
            return StatusCode(501);
        }
    }
}
