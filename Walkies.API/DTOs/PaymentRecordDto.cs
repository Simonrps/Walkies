namespace Walkies.API.DTOs
{

    /// <summary>
    /// Data transfer object for returning payment record information.
    /// Contains the public facing data for a simulated payment
    /// confirmation. No sensitive payment information is  stored
    /// the confirmation is simulated and stored as a status record
    /// only as per NFR04. Related to US19 - Payment Confirmation Owner
    /// and US20 - Payment Confirmation Walker.
    /// </summary>
    public class PaymentRecordDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the payment record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the booking
        /// </summary>
        public int WalkBookingId { get; set; }

        /// <summary>
        /// Gets or sets the name of the owner
        /// </summary>
        public string OwnerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the walker.
        /// </summary>
        public string WalkerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the dog
        /// </summary>
        public string DogName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date of the walk
        /// </summary>
        public DateTime WalkDate { get; set; }

        /// <summary>
        /// Gets or sets the duration of the event in minutes.
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the payment amount in euros.
        /// Calculated based on the duration of the walk and a fixed rate.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the payment status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the payment record was made
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
