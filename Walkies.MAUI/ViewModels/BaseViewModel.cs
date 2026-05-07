using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels on the MAUI application.
    /// Provides common properties for loading state and error
    /// handling using CommunityToolkit.Mvvm's ObservableObject
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether the ViewModel
        /// is currently loading data
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        public partial bool IsBusy { get; set; }

        /// <summary>
        /// Gets a value indicating whether the ViewModel is not busy
        /// </summary>
        public bool IsNotBusy => !IsBusy;

        /// <summary>
        /// Gets or sets the surrent status message displayed to the user
        /// </summary>
        [ObservableProperty]
        public partial string StatusMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether an error has occurred
        /// </summary>
        [ObservableProperty]
        public partial bool HasError { get; set; }

        /// <summary>
        /// Gets or sets the error message to display to the user
        /// </summary>
        [ObservableProperty]
        public partial string ErrorMessage { get; set; } = string.Empty;


        /// <summary>
        /// Load command executed automatically by BasePage on appearance.
        /// Override in derived ViewModels  that require data loading on page load.
        /// </summary>
        public virtual IAsyncRelayCommand? LoadCommand => null;

        /// <summary>
        /// Logs the current user out and navigates back to the registration page.
        /// Override in derived ViewModels to add token clearing before navigation
        /// </summary>
        [RelayCommand]
        protected virtual async Task LogoutAsync()
        {
            await Shell.Current.GoToAsync("///register");
        }

        /// <summary>
        /// Sets the error state with the provided message
        /// </summary>
        protected void SetError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        /// <summary>
        /// Clears the error state
        /// </summary>
        protected void ClearError()
        {
            ErrorMessage = string.Empty;
            HasError = false;
        }
    }
}