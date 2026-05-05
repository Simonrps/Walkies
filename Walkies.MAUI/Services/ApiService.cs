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

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request) =>
            await _httpClient.PostAsJsonAsync($"auth/register", request)
                .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<AuthResponse>().Result);

        public async Task<AuthResponse?> LoginAsync(LoginRequest request) =>
            await _httpClient.PostAsJsonAsync($"auth/login", request)
                .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<AuthResponse>().Result);

        /// Users

        public async Task<UserModel?> GetUserAsync(int userId) =>
            await _httpClient.GetFromJsonAsync<UserModel>($"users/{userId}");

        public async Task<List<UserModel>?> GetWalkersAsync(double latitude, double longitude, double distanceKm, DateTime date) =>
            await _httpClient.GetFromJsonAsync<List<UserModel>>
            ($"users/walkers?latitude={latitude}&longitude={longitude}&distanceKm={distanceKm}&date={date:yyyy-MM-dd}");

        /// Dogs

        public async Task<List<DogModel>?> GetDogsByOwnerAsync(int ownerId) =>
            await _httpClient.GetFromJsonAsync<List<DogModel>>($"dogs?ownerId={ownerId}");

        public async Task<DogModel?> AddDogAsync(object request) =>
            await _httpClient.PostAsJsonAsync($"dogs", request)
                .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<DogModel>().Result);

        public async Task<DogModel?> UpdateDogAsync(int dogId, object request) =>
            await _httpClient.PutAsJsonAsync($"dogs/{dogId}", request)
                .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<DogModel>().Result);

        public async Task<bool> DeleteDogAsync(int dogId)
        {
            var response = await _httpClient.DeleteAsync($"dogs/{dogId}");
            return response.IsSuccessStatusCode;
        }

        /// Walk Requests

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

        public async Task<WalkRequestModel?> PostWalkRequestAsync(object request) =>
            await _httpClient.PostAsJsonAsync($"walkrequests", request)
                .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<WalkRequestModel>().Result);

        public async Task<bool> CancelWalkRequestAsync(int requestId)
        {
            var response = await _httpClient.DeleteAsync($"walkrequests/{requestId}");
            return response.IsSuccessStatusCode;
        }

        /// Bookings

        public async Task<BookingModel?> CreateBookingAsync(object request) =>
            await _httpClient.PostAsJsonAsync($"bookings", request)
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<BookingModel>().Result);

        public async Task<bool> DeclineBookingAsync(object request)
        {
            var response = await _httpClient.PostAsJsonAsync($"bookings/decline", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<BookingModel>?> GetBookingsAsync(int? ownerId = null)
        {
            var url = ownerId.HasValue ? $"bookings?ownerId={ownerId}"
                : $"bookings";
            return await _httpClient.GetFromJsonAsync<List<BookingModel>>(url);
        }

        public async Task<BookingModel?> CheckInAsync(int bookingId) =>
            await _httpClient.PutAsJsonAsync($"bookings/{bookingId}/checkin", new { })
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<BookingModel>().Result);

        public async Task<BookingModel?> CheckOutAsync(int bookingId) =>
            await _httpClient.PutAsJsonAsync($"bookings/{bookingId}/checkout", new { })
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<BookingModel>().Result);

        public async Task<BookingModel?> CancelBookingAsync(int bookingId) =>
            await _httpClient.PutAsJsonAsync($"bookings/{bookingId}/cancel", new { })
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<BookingModel>().Result);

        public async Task<BookingModel?> UpdateLocationAsync(int bookingId, double latitude, double longitude) =>
            await _httpClient.PutAsJsonAsync($"bookings/{bookingId}/location", new { Latitude = latitude, Longitude = longitude })
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<BookingModel>().Result);

        /// Availability

        public async Task<List<AvailabilityModel>?> GetAvailabilityAsync(int walkerId) =>
            await _httpClient.GetFromJsonAsync<List<AvailabilityModel>>($"availability/{walkerId}");

        public async Task<AvailabilityModel?> SetAvailabilityAsync(object request) =>
            await _httpClient.PostAsJsonAsync($"availability", request)
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<AvailabilityModel>().Result);

        public async Task<bool> DeleteAvailabilityAsync(int slotId, bool force = false)
        {
            var response = await _httpClient.DeleteAsync($"availability/{slotId}?force={force}");
            return response.IsSuccessStatusCode;
        }

        /// Messages

        public async Task<List<MessageModel>?> GetMessagesAsync(int userId) =>
            await _httpClient.GetFromJsonAsync<List<MessageModel>>($"messages/{userId}");

        public async Task<MessageModel?> SendMessageAsync(object request) =>
            await _httpClient.PostAsJsonAsync($"messages", request)
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<MessageModel>().Result);

        /// Payments

        public async Task<PaymentModel?> GetPaymentByBookingAsync(int bookingId) =>
            await _httpClient.GetFromJsonAsync<PaymentModel>($"payments/{bookingId}");

        public async Task<PaymentModel?> GetPaymentsByWalkerAsync(int walkerId) =>
            await _httpClient.GetFromJsonAsync<PaymentModel>($"payments/walker/{walkerId}");
    }
}