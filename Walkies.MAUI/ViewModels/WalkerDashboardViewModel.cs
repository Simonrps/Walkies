using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// ViewModel for the walker dashboard page. Loads the current users
    /// first name and provides navigation to walker specifc features.
    /// </summary>
    public partial class WalkerDashboardViewModel(AuthService authService) : BaseViewModel
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
        /// Loads the current walkers first name from secure storage.
        /// </summary>
        [RelayCommand]
        public async Task FetchAsync()
        {
            var firstName = await _authService.GetUserFirstNameAsync();
            FirstName = firstName ?? "Walker";
        }
    }
}