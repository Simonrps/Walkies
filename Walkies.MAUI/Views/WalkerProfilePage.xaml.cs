using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

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