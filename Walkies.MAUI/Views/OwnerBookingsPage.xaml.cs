using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Page for owners to view confirmed bookings and upcoming walks.
/// Related to US13 - View Confirmed Bookings
/// </summary>
public partial class OwnerBookingsPage : BasePage
{
    public OwnerBookingsPage(BookingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}