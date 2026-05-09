using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Page for owners to track walker location during an active walk.
/// Related to US15 - GPS Tracking During Walk
/// </summary>
public partial class OwnerTrackingPage : BasePage
{
    private readonly TrackingViewModel _viewModel;
    public OwnerTrackingPage(TrackingViewModel viewmodel)
    {
        InitializeComponent();
        _viewModel = viewmodel;
        BindingContext = viewmodel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopPolling();
    }
}