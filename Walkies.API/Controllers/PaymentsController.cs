using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;

namespace Walkies.API.Controllers
{
    /// <summary>
    /// Handles the payment confirmation process. Provides endpoints
    /// for retrieving payment records associated with completed bookings
    /// No real payment process occurs - confirmation is simulated and 
    /// stored as status records only.
    /// Related to US19 - Payment Confirmation Owner and
    /// US20 - Payment Confirmation Walker.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initialises an instance of paymentsController
        /// </summary>
        /// <param name="context"></param>
        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the payment record associated with a specific booking
        /// Related to US19 - Payment Confirmation Owner
        /// </summary>
        /// <param name="bookingId">The unique id of the booking</param>
        /// <returns>
        /// 200 Ok with payment record data on success
        /// 400 Not Found if no record exists
        /// </returns>
        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetPaymentByBooking(int bookingId)
        {
            var payment = await _context.PaymentRecords
                .Include(pr => pr.WalkBooking)
                .ThenInclude(wb => wb.Walker)
                .Include(pr => pr.WalkBooking)
                .ThenInclude(wb => wb.WalkRequest)
                .ThenInclude(wb => wb.Owner)
                .Include(pr => pr.WalkBooking)
                .ThenInclude(wb => wb.WalkRequest)
                .ThenInclude(wb => wb.Dog)
                .FirstOrDefaultAsync(pr => pr.WalkBooking.Id == bookingId);

            if (payment == null)
            {
                return NotFound(new { message = "Payment record not found" });
            }

            return Ok(MapToDto(payment));
        }

        /// <summary>
        /// Retrieves all payment records associated with a specific walker
        /// Relates to US20 - Payment Confirmation Walker
        /// </summary>
        /// <param name="walkerId">The unique id of the walker</param>
        /// <returns>
        /// 200 OK with a list of payment records on success
        /// </returns>
        [HttpGet("walker/{walkerId}")]
        public async Task<IActionResult> GetPaymentsByWalker(int walkerId)
        {
            var payments = await _context.PaymentRecords
                .Include(pr => pr.WalkBooking)
                .ThenInclude(wb => wb.Walker)
                .Include(pr => pr.WalkBooking)
                .ThenInclude(wb => wb.WalkRequest)
                .ThenInclude(wb => wb.Owner)
                .Include(pr => pr.WalkBooking)
                .ThenInclude(wb => wb.WalkRequest)
                .ThenInclude(wb => wb.Dog)
                .Where(pr => pr.WalkBooking.Walker.Id == walkerId)
                .ToListAsync();
            return Ok(payments.Select(MapToDto).ToList());
        }

        /// <summary>
        /// Maps a PaymentRecord entity to a PaymentRecordDto and centralises
        /// mapping logic to avoid duplication across endpoints
        /// </summary>
        /// <param name="payment">The payment rocord entity to map</param>
        /// <returns>
        /// A paymentRecordDto representing the payment record
        /// </returns>
        private static PaymentRecordDto MapToDto(PaymentRecord payment) => new PaymentRecordDto
        {
            Id = payment.Id,
            WalkBookingId = payment.WalkBookingId,
            OwnerName = $"{payment.WalkBooking.WalkRequest.Owner.FirstName} {payment.WalkBooking.WalkRequest.Owner.LastName}",
            WalkerName = $"{payment.WalkBooking.Walker.FirstName} {payment.WalkBooking.Walker.LastName}",
            DogName = payment.WalkBooking.WalkRequest.Dog.Name,
            WalkDate = payment.WalkBooking.WalkRequest.RequestedDate,
            DurationMinutes = payment.WalkBooking.WalkRequest.DurationMinutes,
            Amount = payment.Amount,
            Status = payment.Status,
            CreatedAt = payment.CreatedAt
        };
    }
}