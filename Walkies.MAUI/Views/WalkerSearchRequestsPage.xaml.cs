using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Page for walkers to search for open walk requests within
/// a specified distance. Related to US07 - Walker Searches For Requests
/// </summary>
public partial class WalkerSearchRequestsPage : BasePage
{
    public WalkerSearchRequestsPage(WalkerSearchViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}