using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Page for owners to send and receive messages. Related to US17 - Owner Messaging
/// </summary>
public partial class OwnerMessagesPage : BasePage
{
    public OwnerMessagesPage(MessagingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}