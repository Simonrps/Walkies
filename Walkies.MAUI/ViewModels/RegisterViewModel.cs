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
    public partial class RegisterViewModel(AuthService authService) : BaseViewModel
    {
        private readonly AuthService _authService = authService;

        /// <summary>
        /// gets or sets the users first name
        /// </summary>
        [ObservableProperty]
        public partial string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating if the first name field has
        /// a valid input. Used for UI validation feedback.
        /// </summary>
        [ObservableProperty]
        public partial bool IsFirstNameValid { get; set; } = true;

        /// <summary>
        /// gets or sets the users last name
        /// </summary>
        [ObservableProperty]
        public partial string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating if the last name field has
        /// a valid input. Used for UI validation feedback.
        /// </summary>
        [ObservableProperty]
        public partial bool IsLastNameValid { get; set; } = true;
        /// <summary>
        /// gets or sets the users email
        /// </summary>
        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating if the email field has
        /// a valid input. Used for UI validation feedback.
        /// </summary>
        [ObservableProperty]
        public partial bool IsEmailValid { get; set; } = true;
        /// <summary>
        /// gets or sets the users password
        /// </summary>
        [ObservableProperty]
        public partial string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating if the password field has
        /// a valid input. Used for UI validation feedback.
        /// </summary>
        [ObservableProperty]
        public partial bool IsPasswordValid { get; set; } = true;
        /// <summary>
        /// gets or sets the selected role
        /// </summary>
        [ObservableProperty]
        public partial string SelectedRole { get; set; } = "Owner";

        /// <summary>
        /// gets the list of available roles for selection
        /// </summary>
        public List<string> Roles { get; set; } = ["Owner", "Walker"];

        /// <summary>
        /// Validates user input and submits a registration request
        /// vai AuthService. On success, navigates to the appropriate
        /// dashboard based on the users role.
        /// </summary>
        [RelayCommand]
        private async Task RegisterAsync()
        {
            // validates user input and sets validation properties for UI feedback
            IsFirstNameValid = !string.IsNullOrWhiteSpace(FirstName);
            IsLastNameValid = !string.IsNullOrWhiteSpace(LastName);
            IsEmailValid = !string.IsNullOrWhiteSpace(Email) && Email.Contains('@');
            IsPasswordValid = !string.IsNullOrWhiteSpace(Password) && Password.Length >= 8;

            // if any validation fails, set an error message and return early
            if (!IsFirstNameValid || !IsLastNameValid || !IsEmailValid || !IsPasswordValid)
            {
                SetError("Please correct the highlighted fields.");
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

        /// <summary>
        /// Clears input fields and resets validaiton state. called on
        /// every page appearance via LoadCommand to ensure previously
        /// entered credentials are not retained after logout
        /// </summary>
        private void ClearFields()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            SelectedRole = "Owner";
            IsFirstNameValid = true;
            IsLastNameValid = true;
            IsEmailValid = true;
            IsPasswordValid = true;
            ClearError();
        }

        /// <summary>
        /// exposes ClearFieldsCommand as the basepage loadcommand so fields are 
        /// cleared every time the registration page appears
        /// </summary>
        public override IAsyncRelayCommand? LoadCommand =>
            new AsyncRelayCommand(() =>
            {
                ClearFields();
                return Task.CompletedTask;
            });
    }
}