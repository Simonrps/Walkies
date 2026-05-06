using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// The walker profile management page. Recieves a ProfileViewModel 
/// via dependency injection and loads profile on appearance. Inherits
/// from the BasePage to automatically execute LoadCommand on appearance.
/// Related to US03 - Profile Management
/// </summary>
public partial class WalkerProfilePage : BasePage
{
    public WalkerProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}