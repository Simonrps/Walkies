using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Walker dashboard page. Loads walker data on appearance and
/// serves as the central navigation hub for walker features.
/// Inherits from ContentPage and manually executes LoadCommand on appearance
/// </summary>
public partial class WalkerDashboardPage : BasePage
{
    public WalkerDashboardPage(WalkerDashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}