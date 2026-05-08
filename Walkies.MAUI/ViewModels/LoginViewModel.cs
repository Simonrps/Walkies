using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Walkies.MAUI.Models;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// ViewModel for the login page. Handles user credentials and 
    /// communicates with AuthService to authenticate the user
    /// Relates to US02 - Login
    /// </summary>
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        /// <summary>
        /// gets or sets the users email
        /// </summary>
        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the users password
        /// </summary>
        [ObservableProperty]
        public partial string Password { get; set; } = string.Empty;

        public LoginViewModel(AuthService authService) => _authService = authService;

        /// <summary>
        /// Validates user credentials and submits a login request via AuthService.
        /// On success navigates to the appropriate dashboard based on user role.
        /// </summary>
        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                SetError("Email and password are required.");
                return;
            }

            IsBusy = true;
            ClearError();

            try
            {
                var request = new LoginRequest
                {
                    Email = Email,
                    Password = Password
                };

                var response = await _authService.LoginAsync(request);

                if (response == null)
                {
                    SetError("Login failed. Please check your credentials and try again.");
                    return;
                }

                if (response.Role == "Owner")
                {
                    await Shell.Current.GoToAsync("owner/dashboard");
                }
                else
                {
                    await Shell.Current.GoToAsync("walker/dashboard");
                }
            }
            catch (Exception ex)
            {
                SetError($"An error occurred: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Navigates the user to the registration page
        /// </summary>
        [RelayCommand]
        private static async Task NavigateToRegisterAsync()
        {
            await Shell.Current.GoToAsync("///register");
        }
    }
}