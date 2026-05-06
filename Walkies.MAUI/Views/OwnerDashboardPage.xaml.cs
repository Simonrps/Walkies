using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Owner dashboard page. Loads owner data on appearance and serves
/// as the central navigation hub for the owner feeatures. Inherits 
/// from the BasePage to automatically execute LoadCommand on appearance.
/// </summary>
public partial class OwnerDashboardPage : BasePage
{
    public OwnerDashboardPage(OwnerDashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}