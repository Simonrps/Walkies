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

        [ObservableProperty]
        public partial string FirstName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string LastName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Password { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string SelectedRole { get; set; } = "Owner";

        public List<string> Roles { get; set; } = ["Owner", "Walker"];

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
        }

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
                    await Shell.Current.GoToAsync("//owner/dashboard");
                }
                else
                {
                    await Shell.Current.GoToAsync("//walker/dashboard");
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

        [RelayCommand]
        private static async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}
