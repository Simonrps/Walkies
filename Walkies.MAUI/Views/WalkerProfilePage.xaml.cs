using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// The walker profile management page. Recieves a ProfileViewModel 
/// via dependency injection and loads profile on appearance
/// Related to US03 - Profile Management
/// </summary>
public partial class WalkerProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;

    public WalkerProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadProfileAsync();
    }
}