using Walkies.MAUI.ViewModels;

namespace Walkies.MAUI.Views;

/// <summary>
/// Page for Walkers to send and receive messages. Related to US18 - Owner Messaging
/// </summary>
public partial class WalkerMessagesPage : BasePage
{
    public WalkerMessagesPage(MessagingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}