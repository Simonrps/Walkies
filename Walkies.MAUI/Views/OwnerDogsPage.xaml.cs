using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// The owner dogs list page. Displays all the dogs on the owners profile
/// with options to edit or remove. Related to US04 - Add Dog and
/// US05 - Edit/Remove Dog
/// </summary>
public partial class OwnerDogsPage : ContentPage
{
    private readonly DogViewModel _viewModel;

    public OwnerDogsPage(DogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    
    /// <summary>
    /// Loads the owners dogs each time the page appears
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDogsCommand.ExecuteAsync(null);
    }
}