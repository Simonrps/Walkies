using System.ComponentModel.DataAnnotations;

namespace Walkies.API.Models
{
    /// <summary>
    /// Represents a simulated payment confirmation record generated upon
    /// successful completion of a walk booking.
    /// As this is a prototype applicaiton, no real payment processing occurs.
    /// The record serves as a verifiable confirmation of a completed transaction
    /// visible to both the dog owner and the walker.
    /// </summary>
    public class PaymentRecord
    {
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key referencing the completed booking the 
        /// payment refers to.
        /// </summary>
        public int WalkBookingId { get; set; }

        /// <summary>
        /// Gets or sets the walk booking associated with this record.
        /// </summary>
        public WalkBooking WalkBooking { get; set; } = null!;

        /// <summary>
        /// Gets or sets the monetary amount for the transaction.
        /// Calculated based on the walk duration at a standard rate.
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the current status of the payment confirmation.
        /// Valid values are Confirmed & Failed.
        /// </summary>
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Confirmed";

        /// <summary>
        /// Gets or sets the date and time when the payment confirmation was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
