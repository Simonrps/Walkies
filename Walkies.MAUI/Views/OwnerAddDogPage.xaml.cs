using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// The add/edit dog page. Operates in add mode by default and switches to
/// edit mode when an existing dog is passed via the editdog command. Inherits
/// from the BasePage to automatically execute LoadCommand on appearance
/// Relates to US04 - Add Dog and US05 - Edit/Remove Dog
/// </summary>
public partial class OwnerAddDogPage : BasePage
{
    public OwnerAddDogPage(DogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}