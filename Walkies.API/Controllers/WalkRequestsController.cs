using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;

namespace Walkies.API.Controllers
{
    /// <summary>
    /// Handles walk request management for the API. It provides
    /// endpoints for posting, retrieving, and canceling walk requests.
    /// Reated to US06 - Post Walk Request, US07 - Browse Walk Requests
    /// and US08 - Cancel Walk Request.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WalkRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initialises a new instance of the WalkRequestsController.
        /// </summary>
        /// <param name="context">The database context</param>
        public WalkRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostWalkRequest([FromBody] CreateWalkRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var owner = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.OwnerId);
            if (owner == null)
            {
                return NotFound(new { message = "Owner not found" });
            }

            var dog = await _context.Dogs.FirstOrDefaultAsync(d => d.Id == dto.DogId);
            if (dog == null)
            {
                return NotFound(new {message="Dog not found"});
            }

            var walkRequest = new WalkRequest
            {
                OwnerId = dto.OwnerId,
                DogId = dto.DogId,
                RequestedDate = dto.RequestedDate,
                DurationMinutes = dto.DurationMinutes,
                Location = dto.Location,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Status = "Open"
            };

            _context.WalkRequests.Add(walkRequest);
            await _context.SaveChangesAsync();

            var walkRequestDto = new WalkRequestDto
            {
                Id = walkRequest.Id,
                OwnerId = walkRequest.OwnerId,
                OwnerName = $"{owner.FirstName} {owner.LastName}",
                DogId = walkRequest.DogId,
                DogName = dog.Name,
                RequestedDate = walkRequest.RequestedDate,
                DurationMinutes = walkRequest.DurationMinutes,
                Location = walkRequest.Location,
                Latitude = walkRequest.Latitude,
                Longitude = walkRequest.Longitude,
                Status = walkRequest.Status
            };

            return CreatedAtAction(nameof(GetWalkRequest), new { id = walkRequest.Id }, walkRequestDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWalkRequest(int id)
        {
            await Task.CompletedTask;
            return StatusCode(501);
        }
    }
}