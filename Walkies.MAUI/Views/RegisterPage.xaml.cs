using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Registration page for new users to create an account.
/// Recieves a RegisterViewModel via dependency injection
/// and sets it as the BindingContext.
/// Related to US01 - Registration
/// </summary>
public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}