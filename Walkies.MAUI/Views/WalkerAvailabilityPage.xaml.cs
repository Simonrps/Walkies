using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Page for walkers to manage their availability slots.
/// Related to US12 - Walker Availability
/// </summary>
public partial class WalkerAvailabilityPage : BasePage
{
    public WalkerAvailabilityPage(AvailabilityViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}