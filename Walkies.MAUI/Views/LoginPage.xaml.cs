using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// The login page for users to log in
/// Recieves a LoginViewModel via dependency injection
/// and sets it as the BindingContext. Inherits from BasePage
/// to automatically execute LoadCommand on appearance.
/// Related to US02 - Login
/// </summary>
public partial class LoginPage : BasePage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}