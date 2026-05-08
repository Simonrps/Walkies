using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Page for owners to search for available dog walkers by distance
///  and date. Related to US08 - Owner Searches for Walkers
/// </summary>
public partial class OwnerSearchWalkersPage : BasePage
{
    public OwnerSearchWalkersPage(OwnerSearchViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}