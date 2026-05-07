using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Walkies.MAUI.Models;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// ViewModel for the owner search walkers page. Allows an owner
    /// to search for walkers by disstance and date.
    /// Related to US08 - Owner searches for walkers
    /// </summary>
    public partial class OwnerSearchViewModel(ApiService apiService, AuthService authService) : BaseViewModel
    {
        private readonly ApiService _apiService = apiService;
        private readonly AuthService _authService = authService;

        /// <summary>
        /// Gets the list of walker returned by the search
        /// </summary>
        public  ObservableCollection<UserModel> Walkers { get; } = [];

        /// <summary>
        /// Gets or sets the search radius in kilometers
        /// </summary>
        [ObservableProperty]
        public partial double DistanceKm { get; set; } = 10;

        /// <summary>
        /// Gets or sets the date to search for available walkers.
        /// </summary>
        [ObservableProperty]
        public partial DateTime SearchDate { get; set; } = DateTime.Today.AddDays(1);

        /// <summary>
        /// Gets or sets whether the search returned no results.
        /// </summary>
        [ObservableProperty]
        public partial bool NoResults { get; set; }

        /// <summary>
        /// Gets or sets the available distance options
        /// </summary>
        public List<double> DistanceOptions { get; set; } = [5, 10, 20, 50];

        /// <summary>
        /// Searches for available walkers within the specified distance and date.
        /// Related to US08 - Owner searches for walkers
        /// </summary>
        [RelayCommand]
        private async Task SearchAsync()
        {
            if (DistanceKm <= 0)
            {
                SetError("Please select a search distance.");
                return;
            }

            IsBusy = true;
            ClearError();
            NoResults = false;
            Walkers.Clear();

            try
            {
                var ownerId = await _authService.GetUserIdAsync();
                var owner = await _apiService.GetUserAsync(ownerId);

                if (owner == null)
                {
                    SetError("Could not load your profile. Please try again");
                    return;
                }
                var latitude = owner.Latitude ?? 54.9966;
                var longitude = owner.Longitude ?? -7.3086;
                var walkers = await _apiService.GetWalkersAsync(latitude, longitude, DistanceKm, SearchDate);
                if (walkers == null || walkers.Count == 0)
                {
                    NoResults = true;
                    return;
                }
                foreach (var walker in walkers)
                {
                    Walkers.Add(walker);
                }
            }
            catch (Exception ex)
            {
                SetError($"An error occurred while searching for walkers: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
