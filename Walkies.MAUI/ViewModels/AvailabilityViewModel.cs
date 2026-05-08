using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Walkies.MAUI.Models;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// Viewmodel for the walker availability page. Handles loading
    /// adding and removing availability slots.
    /// Related to US12 - Walker Availability
    /// </summary>
    public partial class AvailabilityViewModel(ApiService apiService, AuthService autService) : BaseViewModel
    {
        private readonly ApiService _apiService = apiService;
        private readonly AuthService _authService = autService;

        /// <summary>
        /// Gets the list of availability slots for the walker
        /// </summary>
        public ObservableCollection<AvailabilityModel> Slots { get; } = [];

        /// <summary>
        /// Gets or sets the available from date
        /// </summary>
        [ObservableProperty]
        public partial DateTime AvailableFrom { get; set; } = DateTime.Today;

        /// <summary>
        /// Gets or sets the available to date
        /// </summary>
        [ObservableProperty]
        public partial DateTime AvailableTo { get; set; } = DateTime.Today;

        /// <summary>
        /// Gets or sets whether there are no slots to display
        /// </summary>
        [ObservableProperty]
        public partial bool NoSlots { get; set; }

        /// <summary>
        /// Gets or sets whether a slot was successfully added
        /// </summary>
        [ObservableProperty]
        public partial bool IsAdded { get; set; }

        /// <summary>
        /// Loads the walkers existing availability slots from the API
        /// Related to US12 - Walker Availability
        /// </summary>
        [RelayCommand]
        public async Task LoadAvailabilityAsync()
        {
            IsBusy = false;
            ClearError();
            NoSlots = false;
            IsAdded = false;
            Slots.Clear();

            try
            {
                var walkerId = await _authService.GetUserIdAsync();
                var slots = await _apiService.GetAvailabilityAsync(walkerId);

                if (slots == null || slots.Count == 0)
                {
                    NoSlots = true;
                    return;
                }
                foreach (var slot in slots)
                {
                    Slots.Add(slot);
                }
            }
            catch (Exception ex)
            {
                SetError("An error occurred: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Validates input and adds a new availability slot
        /// Related to US12 - Walker Availability
        /// </summary>
        [RelayCommand]
        private async Task AddSlotAsync()
        {
            if (AvailableFrom >= AvailableTo)
            {
                SetError("Available From date must be before Available To date.");
                return;
            }
            if (AvailableFrom < DateTime.Today)
            {
                SetError("Available From date cannot be in the past.");
                return;
            }

            IsBusy = true;
            ClearError();
            IsAdded = false;

            try
            {
                var walkerId = await _authService.GetUserIdAsync();
                var request = new
                {
                    WalkerId = walkerId,
                    AvailableFrom,
                    AvailableTo
                };

                var result = await _apiService.SetAvailabilityAsync(request);
                if (result == null)
                {
                    SetError("Failed to add availability slot. Please try again");
                    return;
                }

                IsAdded = true;
                AvailableFrom = DateTime.Today;
                AvailableTo = DateTime.Today.AddDays(7);
                await LoadAvailabilityAsync();
            }
            catch (Exception ex)
            {
                SetError("An error occurred: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Removes an availability slot after confirmation
        /// Related to US12 - Walker Availability
        /// </summary>
        [RelayCommand]
        private async Task RemoveSlotAsync(AvailabilityModel slot)
        {
            var confirmed = await Shell.Current.DisplayAlertAsync(
                "Remove Slot",
                $"Remove availability from {slot.AvailableFrom:dd MMM yyyy} to {slot.AvailableTo:dd MMM yyyy}?",
                "Remove",
                "Cancel");
            if (!confirmed)
                return;

            IsBusy = true;
            ClearError();

            try
            {
                var success = await _apiService.DeleteAvailabilityAsync(slot.Id);
                if (!success)
                {
                    var forcedConfirmed = await Shell.Current.DisplayAlertAsync(
                        "Confirmed Booking Exists",
                        "This slot has a confirmed booking. Are you sure you want to remove it?",
                        "Remove Anyway",
                        "Keep");
                    if (!forcedConfirmed)
                        return;
                    success = await _apiService.DeleteAvailabilityAsync(slot.Id, forcedConfirmed);
                    if (!success)
                    {
                        SetError("Failed to remove availability slot. Please try again.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                SetError("An error occurred: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Exposes loadavailabilityasync as the basepage loadcommand
        /// </summary>
        public override IAsyncRelayCommand? LoadCommand => LoadAvailabilityCommand;
    }
}