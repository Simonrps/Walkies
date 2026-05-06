using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// The profile management page for owners. Receives ProfileViewModel
/// via dependency injection and loads profile on appearance
/// Related to US03 - Profile Management
/// </summary>
public partial class OwnerProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;

    public OwnerProfilePage(ProfileViewModel viewModel)
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