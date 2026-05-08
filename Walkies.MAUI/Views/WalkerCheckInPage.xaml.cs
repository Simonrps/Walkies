using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Page for walkers to manage their bookings and checkin/out
/// Related to US16 - Check In / Check Out
/// </summary>
public partial class WalkerCheckInPage : BasePage
{
    public WalkerCheckInPage(BookingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}