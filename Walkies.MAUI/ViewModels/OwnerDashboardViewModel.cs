using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// ViewModel for the owner dashboard page. Loads the current users
    /// first name and provides navigation to owner specifc features.
    /// </summary>
    public partial class OwnerDashboardViewModel(AuthService authService) : BaseViewModel
    {
        private readonly AuthService _authService = authService;

        /// <summary>
        /// Gets or sets the first name of the current user.
        /// </summary>
        [ObservableProperty]
        public partial string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Exposes the LoadAsync method as the BasePage LoadCommand
        /// </summary>
        public override IAsyncRelayCommand? LoadCommand => FetchCommand;

        /// <summary>
        /// Loads the current owners first name from secure storage.
        /// </summary>
        [RelayCommand]
        public async Task FetchAsync()
        {
            var firstName = await _authService.GetUserFirstNameAsync();
            FirstName = firstName ?? "Owner";
        }

        /// <summary>
        /// Clears stored credentials and navigates back to registration page
        /// </summary>
        protected override async Task LogoutAsync()
        {
            _authService.Logout();
            await base.LogoutAsync();
        }

        /// <summary>
        /// Navigates to the owner's profile page.
        /// </summary>
        [RelayCommand]
        private static async Task NavigateToProfileAsync()
        {
            await Shell.Current.GoToAsync("owner/profile");
        }

        /// <summary>
        /// Navigates to the owner's dog page.
        /// </summary>
        [RelayCommand]
        private static async Task NavigateToMyDogsAsync()
        {
            await Shell.Current.GoToAsync("owner/dogs");
        }

        /// <summary>
        /// Navigates to walk request page.
        /// </summary>
        [RelayCommand]
        private static async Task NavigateToPostWalkRequestAsync()
        {
            await Shell.Current.GoToAsync("owner/walkrequest");
        }

        /// <summary>
        /// Navigates to the search walkers page
        /// </summary>
        [RelayCommand]
        private static async Task NavigateToSearchWalkersAsync()
        {
            await Shell.Current.GoToAsync("owner/searchwalkers");
        }

        /// <summary>
        /// Navigates to the owners booking page
        /// </summary>
        [RelayCommand]
        private static async Task NavigateToBookingsAsync()
        {
            await Shell.Current.GoToAsync("owner/bookings");
        }

        /// <summary>
        /// Navigates to the map page
        /// </summary>
        [RelayCommand]
        private static async Task NavigateToMapAsync()
        {
            await Shell.Current.GoToAsync("owner/map");
        }

        /// <summary>
        /// Navigates to the walker tracking page
        /// </summary>
        [RelayCommand]
        private static async Task NavigateToTrackingAsync()
        {
            await Shell.Current.GoToAsync("owner/tracking");
        }

        /// <summary>
        /// Navigates to the owner messaging page
        /// </summary>
        [RelayCommand]
        private static async Task NavigateToMessagesAsync()
        {
            await Shell.Current.GoToAsync("owner/messages");
        }
    }
}