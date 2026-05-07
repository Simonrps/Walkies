using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Walk request page for dog owners. Allows owners to post new
/// walk requests and view existing open requests
/// Related to US06 - Post Walk Request
/// </summary>
public partial class OwnerWalkRequestPage : BasePage
{
    public OwnerWalkRequestPage(WalkRequestViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}