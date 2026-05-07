using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// The owner dogs list page. Displays all the dogs on the owners profile
/// with options to edit or remove. Inherits from the BasePage to automatically
/// execute LoadCommand on appearance.
/// Related to US04 - Add Dog and US05 - Edit/Remove Dog
/// </summary>
public partial class OwnerDogsPage : BasePage
{
    public OwnerDogsPage(DogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}