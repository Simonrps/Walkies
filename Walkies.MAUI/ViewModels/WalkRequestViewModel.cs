using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Walkies.MAUI.Models;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// ViewModel for the walk request page. Handles loading the owners
    ///  dogs, posting a new walk request and displaying the owners
    ///  existing open requests. Related to US06 - Post Walk Request
    /// </summary>
    public partial class WalkRequestViewModel(ApiService apiService, AuthService authService) : BaseViewModel
    {
        private readonly ApiService _apiService = apiService;
        private readonly AuthService _authService = authService;

        /// <summary>
        /// Gets the list of dogs belonging to the owner
        /// </summary>
        public ObservableCollection<DogModel> Dogs { get; } = [];

        /// <summary>
        /// Gets the list of open walk requests for the owner
        /// </summary>
        public ObservableCollection<WalkRequestModel> WalkRequests { get; } = [];

        /// <summary>
        /// Gets or sets the selected dog
        /// </summary>
        [ObservableProperty]
        public partial DogModel? SelectedDog { get; set; }

        /// <summary>
        /// Gets or sets the requested date for the walk
        /// </summary>
        [ObservableProperty]
        public partial DateTime RequestedDate { get; set; } = DateTime.Now.AddDays(1);

        /// <summary>
        /// Gets or sets the duration of the walk in minutes
        /// </summary>
        [ObservableProperty]
        public partial int Duration { get; set; } = 30;

        /// <summary>
        /// Gets or sets the location for the walk
        /// </summary>
        [ObservableProperty]
        public partial string Location { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the walk request was successfully posted
        /// </summary>
        [ObservableProperty]
        public partial bool IsPosted { get; set; }

        /// <summary>
        /// Gets the available duration options in minutes
        /// </summary>
        public List<int> DurationOptions { get; } = [15, 30, 45, 60, 90, 120];

        /// <summary>
        /// Load the onwers dogs and open walk requests when the view appears
        /// </summary>
        [RelayCommand]
        public async Task LoadWalkRequestPageAsync()
        {
            IsBusy = true;
            ClearError();
            IsPosted = false;

            try
            {
                var ownerId = await _authService.GetUserIdAsync();

                var dogs = await _apiService.GetDogsByOwnerAsync(ownerId);
                Dogs.Clear();
                if (dogs is not null)
                    foreach (var dog in dogs)
                    {
                        Dogs.Add(dog);
                    }
                if (Dogs.Count > 0)
                    SelectedDog = Dogs[0];

                var requests = await _apiService.GetWalkRequestsAsync();
                WalkRequests.Clear();
                if (requests is not null)
                    foreach (var r in requests.Where(r => r.OwnerId == ownerId))
                    {
                        WalkRequests.Add(r);
                    }
            }
            catch (Exception ex)
            {
                SetError($"An error occurred while loading walk requests: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Validates inpt and posts a new walk request to the API
        /// Related to US06 - Post Walk Request
        /// </summary>
        [RelayCommand]
        private async Task PostWalkRequestAsync()
        {
            if (SelectedDog is null)
            {
                SetError("Please select a dog for the walk request.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Location))
            {
                SetError("Please enter a location for the walk request.");
                return;
            }
            if(RequestedDate <= DateTime.UtcNow)
            {
                SetError("Walk date must be in the future.");
                return;
            }

            IsBusy = true;
            ClearError();
            IsPosted = false;

            try
            {
                var ownerId = await _authService.GetUserIdAsync();

                var request = new
                {
                    OwnerId = ownerId,
                    DogId = SelectedDog.Id,
                    RequestedDate,
                    DurationMinutes = Duration,
                    Location,
                    Latitude = 54.9966,
                    Longitude = -7.3086
                };

                var response = await _apiService.PostWalkRequestAsync(request);

                if (response == null)
                {
                    SetError("Failed to post walk request. Please try again.");
                    return;
                }

                IsPosted = true;
                Location = string.Empty;
                RequestedDate = DateTime.Today.AddDays(1);
                Duration = 30;
                SelectedDog = Dogs.Count > 0 ? Dogs[0] : null;
                await LoadWalkRequestPageAsync();
            }
            catch (Exception ex)
            {
                SetError($"An error occurred while posting the walk request: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Cancels a walk request after confirmation. Related to US011 - Cancel Walk Request
        /// </summary>
        [RelayCommand]
        private async Task CancelWalkRequestAsync(WalkRequestModel request)
        {
            var confirmed = await Shell.Current.DisplayAlertAsync(
                "Cancel Request",
                $"Are you sure you want to cancel the walk request for {request.DogName} on {request.RequestedDate:g}?",
                "Cancel Request",
                "Keep Request"
                );

            if (!confirmed)
            {
                return;
            }
            IsBusy = true;
            ClearError();

            try
            {
                if (!await _apiService.CancelWalkRequestAsync(request.Id))
                {
                    SetError("Failed to cancel walk request. Please try again.");
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
        /// Exposes LoadWalkRequestPageAsync as the BasePage load command 
        /// </summary>
        public override IAsyncRelayCommand? LoadCommand => LoadWalkRequestPageCommand;
    }
}
