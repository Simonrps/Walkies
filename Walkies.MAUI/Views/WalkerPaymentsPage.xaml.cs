using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Page for walkers to view payment confirmations
/// Related to US20 - Payment Confirmation Walker
/// </summary>
public partial class WalkerPaymentsPage : BasePage
{
    public WalkerPaymentsPage(PaymentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}