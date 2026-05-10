using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Page for owners to view payment confirmations
/// Related to US9 - Payment Confirmation Owner
/// </summary>
public partial class OwnerPaymentPage : BasePage
{
    public OwnerPaymentPage(PaymentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}