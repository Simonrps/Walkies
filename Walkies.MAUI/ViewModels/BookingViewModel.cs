using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Walkies.MAUI.Models;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// ViewModel for the booking management pages. Handles loading bookings,
    /// cancellation, check-in and check-out functionality
    /// Related to US09 - Accept or Decline Request,
    /// US10 - Booking Status Update, US11 - Cancellation,
    /// US13 - View Confirmed Bookings, US16 - Check In / Check Out
    /// </summary>
    public partial class BookingViewModel(ApiService apiService, AuthService authService) : BaseViewModel
    {
        private readonly ApiService _apiService = apiService;
        private readonly AuthService _authService = authService;
        private const string ErrorPrefix = "An error occurred: ";

        /// <summary>
        /// Gets the list of bookings for the current user
        /// </summary>
        public ObservableCollection<BookingModel> Bookings { get; } = [];

        /// <summary>
        /// Gets or sets the selected booking
        /// </summary>
        [ObservableProperty]
        public partial BookingModel? SelectedBooking { get; set; }

        /// <summary>
        /// Gets or sets whether there are no bookings to display
        /// </summary>
        [ObservableProperty]
        public partial bool NoBookings { get; set; }

        /// <summary>
        /// Gets or sets whether the current user is a walker
        /// </summary>
        [ObservableProperty]
        public partial bool IsWalker { get; set; }

        /// <summary>
        /// Loads bookings for the current user on page load.
        /// Related to US13 - View Confirmed Bookings
        /// </summary>
        [RelayCommand]
        public async Task LoadBookingsAsync()
        {
            IsBusy = true;
            ClearError();
            NoBookings = false;
            Bookings.Clear();

            try
            {
                var userId = await _authService.GetUserIdAsync();
                var role = await _authService.GetUserRoleAsync();
                IsWalker = role == "Walker";

                List<BookingModel>? bookings;

                if (IsWalker)
                {
                    bookings = await _apiService.GetBookingsAsync();
                }
                else
                {
                    bookings = await _apiService.GetBookingsAsync(userId);
                }

                if (bookings == null || bookings.Count == 0)
                {
                    NoBookings = true;
                    return;
                }
                var filtered = IsWalker ? bookings.Where(b => b.WalkerId == userId) : bookings;

                foreach (var booking in filtered)
                {
                    Bookings.Add(booking);
                }
            }
            catch (Exception ex)
            {
                SetError(ErrorPrefix + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Cancels a booking after confirmation.
        /// Related to US11 - Cancellation
        /// </summary>
        [RelayCommand]
        private async Task CancelBookingAsync(BookingModel booking)
        {
            var confirmed = await Shell.Current.DisplayAlertAsync(
                "Cancel Booking",
                $"Are you sure you want to cancel the booking for {booking.DogName} on {booking.ScheduledDate:dd MMM yyyy}?",
                "Cancel Booking",
                "Keep Booking");

            if (!confirmed)
            {
                return;
            }
            IsBusy = true;
            ClearError();

            try
            {
                var result = await _apiService.CancelBookingAsync(booking.Id);
                if (result == null)
                {
                    SetError("Failed to cancel the booking. Please try again.");
                    return;
                }

                Bookings.Remove(booking);
            }
            catch (Exception ex)
            {
                SetError(ErrorPrefix + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Checks in for a confirmed booking.
        /// Related to US16 - Check In / Check Out
        /// </summary>
        [RelayCommand]
        private async Task CheckInAsync(BookingModel booking)
        {
            IsBusy = true;
            ClearError();

            try
            {
                var result = await _apiService.CheckInAsync(booking.Id);
                if (result == null)
                {
                    SetError("Failed to check in. Please try again.");
                    return;
                }

                var index = Bookings.IndexOf(booking);
                if (index >= 0)
                {
                    Bookings[index] = result;
                }
            }
            catch (Exception ex)
            {
                SetError(ErrorPrefix + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Checks out of an active booking.
        /// Related to US16 - Check In / Check Out
        /// </summary>
        [RelayCommand]
        private async Task CheckOutAsync(BookingModel booking)
        {
            IsBusy = true;
            ClearError();

            try
            {
                var result = await _apiService.CheckOutAsync(booking.Id);
                if (result == null)
                {
                    SetError("Failed to check out. Please try again.");
                    return;
                }

                var index = Bookings.IndexOf(booking);
                if (index >= 0)
                {
                    Bookings[index] = result;
                }
            }
            catch (Exception ex)
            {
                SetError(ErrorPrefix + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Exposes LoadBookingsAsync as the basepage loadcommand
        /// </summary>
        public override IAsyncRelayCommand? LoadCommand => LoadBookingsCommand;
    }
}
