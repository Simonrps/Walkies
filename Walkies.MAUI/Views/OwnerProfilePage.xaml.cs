using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// The profile management page for owners. Receives ProfileViewModel
/// via dependency injection and loads profile on appearance. Inherits
/// from the BasePage to automatically execute LoadCommand on appearance.
/// Related to US03 - Profile Management
/// </summary>
public partial class OwnerProfilePage : BasePage
{
    public OwnerProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}