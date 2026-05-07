using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Walkies.MAUI.Models;
using Walkies.MAUI.Services;


namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// ViewModel for the Walker Search requests page. Allows a walker
    /// to search for open walk requests within a specified distance.
    /// Related to US07 - Walker Searches For Requests
    /// </summary>
    public partial class WalkerSearchViewModel(ApiService apiService, AuthService authService) : BaseViewModel
    {
        private readonly ApiService _apiService = apiService;
        private readonly AuthService _authService = authService;

        /// <summary>
        /// Gets the list of open walk requests returned from the search
        /// </summary>
        public ObservableCollection<WalkRequestModel> WalkRequests { get; } = [];

        /// <summary>
        /// Gets or sets the search radius in kilometres
        /// </summary>
        public partial double DistanceKm { get; set; } = 10;

        /// <summary>
        /// Gets or sets whether the seach has been run with no results
        /// </summary>
        [ObservableProperty]
        public partial bool NoResults { get; set; }

        /// <summary>
        /// Gets the available distance options
        /// </summary>
        public List<double> DistanceOptions { get; } = [1, 5, 10, 20, 50];

        /// <summary>
        /// Searches for open walk requests within the specified distance
        /// of the walkers registered location.
        /// Related to US07 - Walker Searches For Requests
        /// </summary>
        [RelayCommand]
        public async Task SearchAsync()
        {
            if (DistanceKm <= 0)
            {
                SetError("Please select a search distance");
                return;
            }
            IsBusy = true;
            ClearError();
            NoResults = false;
            WalkRequests.Clear();

            try
            {
                var walkerId = await _authService.GetUserIdAsync();
                var walker = await _apiService.GetUserAsync(walkerId);

                if (walker == null)
                {
                    SetError("Could not load your profile please try again");
                    return;
                }
                var latitude = walker.Latitude ?? 54.9966;
                var longitude = walker.Longitude ?? -7.3086;

                var requests = await _apiService.GetWalkRequestsAsync(latitude, longitude, DistanceKm);

                if (requests == null || requests.Count == 0)
                {
                    NoResults = true;
                    return;
                }
                foreach (var request in requests)
                {
                    WalkRequests.Add(request);
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
        /// Accepts a walk request by creating a booking.
        /// Relates to US09 - Accept or decline request
        /// </summary>
        [RelayCommand]
        private async Task AcceptRequestAsync(WalkRequestModel request)
        {
            var confirmed = await Shell.Current.DisplayAlertAsync(
                "Accept Walk Request",
                $"Accept the walk request for {request.DogName} on {request.RequestedDate:dd MMM yyyy}?",
                "Accept",
                "Cancel");

            if (!confirmed)
                return;

            IsBusy = true;
            ClearError();
            try
            {
                var walkerId = await _authService.GetUserIdAsync();
                var booking = await _apiService.CreateBookingAsync(new
                {
                    WalkRequestId = request.Id,
                    WalkerId = walkerId
                });

                if (booking == null)
                {
                    SetError("Failed to accept the walk request, it may already have been accepted");
                    return;
                }

                WalkRequests.Remove(request);
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
        /// Exposes SearchAsync as the basepage loadcommand
        /// </summary>
        public override IAsyncRelayCommand? LoadCommand => SearchCommand;
    }
}
