using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// ViewModel for the profile management pages.
    /// Handles loading and updating user profile data.
    /// Related to US03 - Profile Management
    /// </summary>
    public partial class ProfileViewModel(ApiService apiService, AuthService authService) : BaseViewModel
    {
        private readonly ApiService _apiService = apiService;
        private readonly AuthService _authService = authService;

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
        /// gets or sets the users phone number
        /// </summary>
        [ObservableProperty]
        public partial string? Phone { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the users address
        /// </summary>
        [ObservableProperty]
        public partial string? Address { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the users role (Walker or Owner)
        /// </summary>
        [ObservableProperty]
        public partial string Role { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the saved value of the profile
        /// </summary>
        [ObservableProperty]
        public partial bool IsSaved { get; set; }

        /// <summary>
        /// Loads the current users profile from the api
        /// and populates the viewmodel properties.
        /// </summary>
        [RelayCommand]
        public async Task LoadProfileAsync()
        {
            IsBusy = true;
            ClearError();
            IsSaved = false;

            try
            {
                var userId = await _authService.GetUserIdAsync();
                var profile = await _apiService.GetUserAsync(userId);

                if (profile == null)
                {
                    SetError("Could not load profile.");
                    return;
                }

                FirstName = profile.FirstName;
                LastName = profile.LastName;
                Email = profile.Email;
                Phone = profile.Phone;
                Address = profile.Address;
                Role = profile.Role;
            }
            catch (Exception ex)
            {
                SetError($"An error occurred while loading your profile: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Validates input and submits updated profile data to the api.
        /// Displays confirmation on success or error message on failure.
        /// </summary>
        [RelayCommand]
        public async Task SaveProfileAsync()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                SetError("First name & last name are required.");
                return;
            }

            IsBusy = true;
            ClearError();
            IsSaved = false;

            try
            {
                var userId = await _authService.GetUserIdAsync();
                var request = new
                {
                    FirstName,
                    LastName,
                    Phone,
                    Address
                };

                var response = await _apiService.UpdateUserAsync(userId, request);

                if (response == null)
                {
                    SetError("Failed to save profile. Please try again.");
                }

                IsSaved = true;
            }
            catch (Exception ex)
            {
                SetError($"An error occurred while saving your profile: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Exposes the LoadProfileAsync method as the BasePage LoadCommand
        /// </summary>
        public override IAsyncRelayCommand? LoadCommand => LoadProfileCommand;
    }
}
