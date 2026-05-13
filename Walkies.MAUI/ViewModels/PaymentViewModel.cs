using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Walkies.MAUI.Models;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// Viewmodel for the payment confirmation page. Handles loading payment records for owners and walkers.
    /// Related to US19 - Payment confirmation Owner and US20 - Payment confirmation Walker
    /// </summary>
    public partial class PaymentViewModel(ApiService apiService, AuthService authService) : BaseViewModel
    {
        private readonly ApiService _apiService = apiService;
        private readonly AuthService _authService = authService;

        /// <summary>
        /// Gets the list of payment rerods
        /// </summary>
        public ObservableCollection<PaymentModel> Payments { get; } = [];

        /// <summary>
        /// Gets or sets whether there are no payments to display
        /// </summary>
        [ObservableProperty]
        public partial bool NoPayments { get; set; }

        /// <summary>
        /// Gets or sets the current user is a walker
        /// </summary>
        [ObservableProperty]
        public partial bool IsWalker { get; set; }

        /// <summary>
        /// Loads payment records for the current user. Owners see payments
        /// by bookings, walkers see all their payments.
        /// Related to US19 - Payment Confirmation Owner
        /// Related to US20 - Payment Confirmation Walker
        /// </summary>
        [RelayCommand]
        public async Task LoadPaymentsAsync()
        {
            IsBusy = true;
            ClearError();
            NoPayments = false;
            Payments.Clear();

            try
            {
                var userId = await _authService.GetUserIdAsync();
                var role = await _authService.GetUserRoleAsync();
                IsWalker = role == "Walker";

                if (IsWalker)
                {
                    await LoadWalkerPaymentsAsync(userId);
                }
                else
                {
                    await LoadOwnerPaymentsAsync(userId);
                }
                if (Payments.Count == 0)
                {
                    NoPayments = true;
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
        /// Loads all payment records for the current owner.
        /// Related to US19 - Payment Confirmation Ownr
        /// </summary>
        private async Task LoadOwnerPaymentsAsync(int ownerId)
        {
            var bookings = await _apiService.GetBookingsAsync(ownerId);
            if (bookings == null || bookings.Count == 0)
            {
                NoPayments = true;
                return;
            }

            var completedBookings = bookings.Where(b => b.Status == "Completed").ToList();

            if (completedBookings.Count == 0)
            {
                NoPayments = true;
                return;
            }

            foreach (var booking in completedBookings)
            {
                var payment = await _apiService.GetPaymentByBookingAsync(booking.Id);
                if (payment != null)
                {
                    Payments.Add(payment);
                }
            }
        }

        /// <summary>
        /// Loads all payment records for the current walker.
        /// Related to US20 - Payment Confirmation Walker
        /// </summary>
        private async Task LoadWalkerPaymentsAsync(int walkerId)
        {
            var payments = await _apiService.GetPaymentsByWalkerAsync(walkerId);
            if (payments == null)
            {
                NoPayments = true;
                return;
            }
            foreach (var payment in payments)
            {
                Payments.Add(payment);
            }
        }

        /// <summary>
        /// Exposes loadpaymentsasync as the basepage loadcommand
        /// </summary>
        public override IAsyncRelayCommand? LoadCommand => LoadPaymentsCommand;
    }
}