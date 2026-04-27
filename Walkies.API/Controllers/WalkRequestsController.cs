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

        /// <summary>
        /// Retrieves the walk request with the specified identifier.
        /// Related to US06 - Post Walk Request
        /// </summary>
        /// <param name="id">The unique identifier of the walk request</param>
        /// <returns>
        /// 200 OK with the walk request details if found
        /// 404 Not Found if the walk request does not exist
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWalkRequest(int id)
        {
            var walkRequest = await _context.WalkRequests
                .Include(wr => wr.Owner)
                .Include(wr => wr.Dog)
                .FirstOrDefaultAsync(wr => wr.Id == id);

            if (walkRequest == null)
            {
                return NotFound(new {message="Walk request not found"});
            }

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
    }
}