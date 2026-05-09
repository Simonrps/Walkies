using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Page displaying available dog walkers on a map
/// Relates to US14 - Map Display of walkers
/// </summary>
public partial class OwnerMapPage : BasePage
{
    public OwnerMapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}