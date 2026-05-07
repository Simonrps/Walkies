using System.Net.Http.Headers;
using System.Net.Http.Json;
using Walkies.MAUI.Models;
using Walkies.MAUI.Utilities;

namespace Walkies.MAUI.Services
{
    /// <summary>
    /// Handles all HTTP communication with the API.
    /// Provides methods for each API endpoint used by the frontend
    /// </summary>
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initialised the ApiService and sets the base address for ApiConstants.
        /// </summary>
        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = ApiConstants.BaseUrl;
        }

        /// <summary>
        /// Sets the JWT token on the HTTP client for authenticated requests.
        /// </summary>
        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        /// Auth

        /// <summary>
        /// Sends a registration request to the API and returns the auth response.
        /// Related to US01 - Registration
        /// </summary>
        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"auth/register", request);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        /// <summary>
        /// Sends a login request to the API and returns the auth response.
        /// Related to US02 - Login
        /// </summary>
        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"auth/login", request);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        /// Users

        /// <summary>
        /// Retrieves a user profile by user id.
        /// Related to US03 - Profile Management
        /// </summary>
        public async Task<UserModel?> GetUserAsync(int userId) =>
            await _httpClient.GetFromJsonAsync<UserModel>($"users/{userId}");

        /// <summary>
        /// Retrieves a list of available dog walkers within a specified radius and date.
        /// Related to US08 - Owner Searches For Walkers
        /// </summary>
        public async Task<List<UserModel>?> GetWalkersAsync(
            double latitude, double longitude, double distanceKm, DateTime date) =>
            await _httpClient.GetFromJsonAsync<List<UserModel>>
            ($"users/walkers?latitude={latitude}&longitude={longitude}&distanceKm={distanceKm}&date={date:yyyy-MM-dd}");

        /// <summary>
        /// Sends updated profile data to the API for the specified user.
        /// Related to US03 - Profile Management
        /// </summary>
        public async Task<UserModel?> UpdateUserAsync(int userId, object request) =>
            await _httpClient.PutAsJsonAsync($"users/{userId}", request)
                .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<UserModel>().Result);

        /// Dogs

        /// <summary>
        /// Retrieves all dogs belonging to the specified user.
        /// Related to US04 - Add Dog
        /// </summary>
        public async Task<List<DogModel>?> GetDogsByOwnerAsync(int ownerId) =>
            await _httpClient.GetFromJsonAsync<List<DogModel>>($"dogs?ownerId={ownerId}");

        /// <summary>
        /// Sends a request to add a new dog to the owners profile.
        /// Related to US04 - Add Dog
        /// </summary>
        public async Task<DogModel?> AddDogAsync(object request) =>
            await _httpClient.PostAsJsonAsync($"dogs", request)
                .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<DogModel>().Result);

        /// <summary>
        /// Sends updated dog details to the API for the specified dog.
        /// Related to US05 - Edit/Remove Dog
        /// </summary>
        public async Task<DogModel?> UpdateDogAsync(int dogId, object request) =>
            await _httpClient.PutAsJsonAsync($"dogs/{dogId}", request)
                .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<DogModel>().Result);

        /// <summary>
        /// Sends to delete the specified dog from the owners profile.
        /// Related to US05 - Edit/Remove Dog
        /// </summary>
        public async Task<bool> DeleteDogAsync(int dogId)
        {
            var response = await _httpClient.DeleteAsync($"dogs/{dogId}");
            return response.IsSuccessStatusCode;
        }

        /// Walk Requests


        /// <summary>
        /// Retrieves open walk requests filtered by location and radius.
        /// Related to US07 - Walker Searches For Requests
        /// </summary>
        public async Task<List<WalkRequestModel>?> GetWalkRequestsAsync(double? latitude = null, double? longitude = null,
            double? distanceKm = null)
        {
            var url = $"walkrequests?";
            if (latitude.HasValue && longitude.HasValue && distanceKm.HasValue)
            {
                url += $"latitude={latitude.Value}&longitude={longitude.Value}&distanceKm={distanceKm.Value}";
            }
            return await _httpClient.GetFromJsonAsync<List<WalkRequestModel>>(url);
        }

        /// <summary>
        /// Sends a new walk request to the API.
        /// Related to US06 - Post walk Request
        /// </summary>
        public async Task<WalkRequestModel?> PostWalkRequestAsync(object request) =>
            await _httpClient.PostAsJsonAsync($"walkrequests", request)
                .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<WalkRequestModel>().Result);
        /// <summary>
        /// Sends a request to cancel a specified walk request.
        /// Related to US11 - Cancellation
        /// </summary>

        public async Task<bool> CancelWalkRequestAsync(int requestId)
        {
            var response = await _httpClient.DeleteAsync($"walkrequests/{requestId}");
            return response.IsSuccessStatusCode;
        }

        /// Bookings

        /// <summary>
        /// Sends a request for a walker to accept a walk request which creates a booking.
        /// Related to US09 - Accept of Decline Request
        /// </summary>
        public async Task<BookingModel?> CreateBookingAsync(object request) =>
            await _httpClient.PostAsJsonAsync($"bookings", request)
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<BookingModel>().Result);

        /// <summary>
        /// Sends a request for a walker to decline a walk request.
        /// Related to US09 - Accept of Decline Request
        /// </summary>
        public async Task<bool> DeclineBookingAsync(object request)
        {
            var response = await _httpClient.PostAsJsonAsync($"bookings/decline", request);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Retrieves bookings filetered by owner id in chronological order.
        /// Related to US13 - View Confirmed Bookings
        /// </summary>
        public async Task<List<BookingModel>?> GetBookingsAsync(int? ownerId = null)
        {
            var url = ownerId.HasValue ? $"bookings?ownerId={ownerId}"
                : $"bookings";
            return await _httpClient.GetFromJsonAsync<List<BookingModel>>(url);
        }

        /// <summary>
        /// Sends a check-in request for the specified booking. Records start time
        /// and activates the GPS tracking.
        /// Related to US16 - Check In / Check Out
        /// </summary>
        public async Task<BookingModel?> CheckInAsync(int bookingId) =>
            await _httpClient.PutAsJsonAsync($"bookings/{bookingId}/checkin", new { })
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<BookingModel>().Result);

        /// <summary>
        /// Sends a check-out request for the specified booking. Records end time
        /// and triggers payment confirmation.
        /// Related to US16 - Check In / Check Out
        /// </summary>
        public async Task<BookingModel?> CheckOutAsync(int bookingId) =>
            await _httpClient.PutAsJsonAsync($"bookings/{bookingId}/checkout", new { })
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<BookingModel>().Result);

        /// <summary>
        /// Sends a cancellation request for the specified booking.
        /// Related to US11 - Cancellation
        /// </summary>
        public async Task<BookingModel?> CancelBookingAsync(int bookingId) =>
            await _httpClient.PutAsJsonAsync($"bookings/{bookingId}/cancel", new { })
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<BookingModel>().Result);

        /// <summary>
        /// Sends an updated walker location to the API during active walk.
        /// Related to US15 - GPS Tracking During Walk
        /// </summary>
        public async Task<BookingModel?> UpdateLocationAsync(int bookingId, double latitude, double longitude) =>
            await _httpClient.PutAsJsonAsync($"bookings/{bookingId}/location", new { Latitude = latitude, Longitude = longitude })
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<BookingModel>().Result);

        /// Availability

        /// <summary>
        /// Retrieves all availability slots for the specified walker.
        /// Related to US12 - Walker Availability
        /// </summary>
        public async Task<List<AvailabilityModel>?> GetAvailabilityAsync(int walkerId) =>
            await _httpClient.GetFromJsonAsync<List<AvailabilityModel>>($"availability/{walkerId}");

        /// <summary>
        /// Sends a new availability slot to the API for the specified walker.
        /// Related to US12 - Walker Availability
        /// </summary>
        public async Task<AvailabilityModel?> SetAvailabilityAsync(object request) =>
            await _httpClient.PostAsJsonAsync($"availability", request)
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<AvailabilityModel>().Result);

        /// <summary>
        /// Sends a request to delete the specified availability slot
        /// If force is true the slot is deleted even if a booking exists
        /// Related to US12 - Walker Availability
        /// </summary>
        public async Task<bool> DeleteAvailabilityAsync(int slotId, bool force = false)
        {
            var response = await _httpClient.DeleteAsync($"availability/{slotId}?force={force}");
            return response.IsSuccessStatusCode;
        }

        /// Messages

        /// <summary>
        /// Retrieves all messages for the specified user.
        /// Related to US17 - Owner Messaging and US18 - Walker Messaging
        /// </summary>
        public async Task<List<MessageModel>?> GetMessagesAsync(int userId) =>
            await _httpClient.GetFromJsonAsync<List<MessageModel>>($"messages/{userId}");

        /// <summary>
        /// Sends a new message to the API.
        /// Related to US17 - Owner Messaging and US18 - Walker Messaging
        /// </summary>
        public async Task<MessageModel?> SendMessageAsync(object request) =>
            await _httpClient.PostAsJsonAsync($"messages", request)
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<MessageModel>().Result);

        /// Payments

        /// <summary>
        /// Retrieves the payment record for the specified booking
        /// Related to US19 - Payment Confirmation Owner
        /// </summary>
        public async Task<PaymentModel?> GetPaymentByBookingAsync(int bookingId) =>
            await _httpClient.GetFromJsonAsync<PaymentModel>($"payments/{bookingId}");

        /// <summary>
        /// Retrieves all payment records for the specified walker
        /// Related to US20 - Payment Confirmation Walker
        /// </summary>
        public async Task<PaymentModel?> GetPaymentsByWalkerAsync(int walkerId) =>
            await _httpClient.GetFromJsonAsync<PaymentModel>($"payments/walker/{walkerId}");
    }
}