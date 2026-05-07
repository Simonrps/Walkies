using Walkies.MAUI.Models;

namespace Walkies.MAUI.Services
{
    /// <summary>
    /// Handles authentication for the MAUI application. Manages login, registration
    /// and JWT token storage using SecureStorgae.
    /// Related to US01 - Registration and US02 - Login.
    /// </summary>
    public class AuthService(ApiService apiService, ISecureStorageService secureStorage)
    {
        private readonly ApiService _apiService = apiService;
        private readonly ISecureStorageService _secureStorage = secureStorage;
        private const string TokenKey = "auth-token";
        private const string UserIdKey = "user-id";
        private const string UserRoleKey = "user-role";
        private const string UserFirstNameKey = "user-first-name";
        private const string UserLastNameKey = "user-last-name";
        private const string UserEmailKey = "user-email";

        /// <summary>
        /// Registers a new user and stores the returned JWT token
        /// Related to US01 - Registration.
        /// </summary>
        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            var response = await _apiService.RegisterAsync(request);
            if (response != null)
                await StoreAuthResponseAsync(response);
            return response;
        }

        /// <summary>
        /// Logs in existing user and stores the returned JWT token
        /// Related to US02 - Login.
        /// </summary>
        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var response = await _apiService.LoginAsync(request);
            if (response != null)
                await StoreAuthResponseAsync(response);
            return response;
        }

        /// <summary>
        /// Logs out the current user by clearing all stored credentials
        /// </summary>
        public void Logout()
        {
            _secureStorage.Remove(TokenKey);
            _secureStorage.Remove(UserIdKey);
            _secureStorage.Remove(UserRoleKey);
            _secureStorage.Remove(UserFirstNameKey);
            _secureStorage.Remove(UserLastNameKey);
            _secureStorage.Remove(UserEmailKey);
        }

        /// <summary>
        /// Returns true if a valid JWT token is stored on the device
        /// </summary>
        public async Task<bool> IsLoggedInAsync()
        {
            var token = await _secureStorage.GetAsync(TokenKey);
            return !string.IsNullOrEmpty(token);
        }

        /// <summary>
        /// Retrieves the stored JWT token for secure storage
        /// </summary>
        public async Task<string?> GetTokenAsync() =>
            await _secureStorage.GetAsync(TokenKey);

        /// <summary>
        /// Retrieves the stored user ID for secure storage
        /// </summary>
        public async Task<int> GetUserIdAsync()
        {
            var id = await _secureStorage.GetAsync(UserIdKey);
            return int.TryParse(id, out var userId) ? userId : 0;
        }

        /// <summary>
        /// Retrieves the stored user role for secure storage
        /// </summary>
        public async Task<string?> GetUserRoleAsync() =>
            await _secureStorage.GetAsync(UserRoleKey);

        /// <summary>
        /// Retrieves the stored user first name for secure storage
        /// </summary>
        public async Task<string?> GetUserFirstNameAsync() =>
            await _secureStorage.GetAsync(UserFirstNameKey);

        /// <summary>
        /// Stores the authentication response in secure storage and
        /// sets the JWT token on the API service
        /// </summary>
        private async Task StoreAuthResponseAsync(AuthResponse response)
        {
            await _secureStorage.SetAsync(TokenKey, response.Token);
            await _secureStorage.SetAsync(UserIdKey, response.UserId.ToString());
            await _secureStorage.SetAsync(UserRoleKey, response.Role);
            await _secureStorage.SetAsync(UserFirstNameKey, response.FirstName);
            await _secureStorage.SetAsync(UserLastNameKey, response.LastName);
            await _secureStorage.SetAsync(UserEmailKey, response.Email);
            _apiService.SetAuthToken(response.Token);
        }
    }
}