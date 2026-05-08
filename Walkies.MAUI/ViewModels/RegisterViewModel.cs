using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Walkies.MAUI.Models;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// ViewModel for the registration page. Handles user input
    /// and communicates with AuthService to register new user account
    /// Related to US01 - Registration
    /// </summary>
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        /// <summary>
        /// gets or sets the users first name
        /// </summary>
        [ObservableProperty]
        public partial string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the users last name
        /// </summary>
        [ObservableProperty]
        public partial string LastName { get; set; } = string.Empty;

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

        /// <summary>
        /// gets or sets the selected role
        /// </summary>
        [ObservableProperty]
        public partial string SelectedRole { get; set; } = "Owner";

        /// <summary>
        /// gets the list of available roles for selection
        /// </summary>
        public List<string> Roles { get; set; } = ["Owner", "Walker"];

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Validates user input and submits a registration request
        /// vai AuthService. On success, navigates to the appropriate
        /// dashboard based on the users role.
        /// </summary>
        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                SetError("All fields are required.");
                return;
            }

            IsBusy = true;
            ClearError();

            try
            {
                var request = new RegisterRequest
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Password = Password,
                    Role = SelectedRole
                };

                var response = await _authService.RegisterAsync(request);

                if (response == null)
                {
                    SetError("Registration failed. Please try again.");
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
        /// Navigates the user to the login page
        /// </summary>
        [RelayCommand]
        private static async Task GoToLoginAsync()
        {
            // Absolute navigation is required when targeting a Shell element route (ShellContent Route="login").
            await Shell.Current.GoToAsync("///login");
        }
    }
}