using CommunityToolkit.Mvvm.ComponentModel;

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
        private bool _isBusy;

        /// <summary>
        /// Gets a value indicating whether the ViewModel is not busy
        /// </summary>
        public bool IsNotBusy => !IsBusy;

        /// <summary>
        /// Gets or sets the surrent status message displayed to the user
        /// </summary>
        [ObservableProperty]
        private string _statusMessage = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether an error has occurred
        /// </summary>
        [ObservableProperty]
        private bool _hasError;

        /// <summary>
        /// Gets or sets the error message to display to the user
        /// </summary>
        [ObservableProperty]
        private string _errorMessage = string.Empty;

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
