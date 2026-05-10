using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Walkies.MAUI.Models;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// Viewmodel for the messaging pages. Handles loading and sending messages between users.
    /// Related to US17 - Owner Messaging and US18 Walker Messaging
    /// </summary>
    public partial class MessagingViewModel(ApiService apiService, AuthService authService) : BaseViewModel
    {
        private readonly ApiService _apiService = apiService;
        private readonly AuthService _authService = authService;

        /// <summary>
        /// Gets the list of messages for the current user
        /// </summary>
        public ObservableCollection<MessageModel> Messages { get; } = [];

        /// <summary>
        /// Gets the list of contacts derived from bookings
        /// </summary>
        public ObservableCollection<ContactModel> Contacts { get; } = [];

        /// <summary>
        /// Gets or sets the selected contact
        /// </summary>
        [ObservableProperty]
        public partial ContactModel? SelectedContact { get; set; }

        /// <summary>
        /// Gets or sets the recipient userID
        /// </summary>
        [ObservableProperty]
        public partial int RecipientId { get; set; }

        /// <summary>
        /// Gets or sets the available to date
        /// </summary>
        [ObservableProperty]
        public partial string RecipientName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the message being composed
        /// </summary>
        [ObservableProperty]
        public partial string NewMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether there are no messages to display
        /// </summary>
        [ObservableProperty]
        public partial bool NoMessages { get; set; }

        /// <summary>
        /// Gets or sets the current user id
        /// </summary>
        private int CurrentUserId { get; set; }

        /// <summary>
        /// Loads all messages for the current user.
        /// Related to US17 - Owner Messaging and US18 - Walker Messaging
        /// </summary>
        [RelayCommand]
        public async Task LoadMessagesAsync()
        {
            IsBusy = true;
            ClearError();
            NoMessages = false;
            Messages.Clear();
            Contacts.Clear();

            try
            {
                CurrentUserId = await _authService.GetUserIdAsync();
                var role = await _authService.GetUserRoleAsync();
                var bookings = await _apiService.GetBookingsAsync(
                    role == "Owner" ? CurrentUserId : null);

                if (bookings != null)
                {
                    var contacts = role == "Owner"
                        ? bookings
                            .Where(b => b.WalkerId > 0)
                            .GroupBy(b => b.WalkerId)
                            .Select(g => new ContactModel
                            {
                                Id = g.First().WalkerId,
                                Name = g.First().WalkerName
                            })
                        : bookings
                            .Where(b => b.WalkerId == CurrentUserId && b.OwnerId > 0)
                            .GroupBy(b => b.OwnerId)
                            .Select(g => new ContactModel
                            { 
                                Id = g.First().OwnerId,
                                Name = g.First().OwnerName
                            });
                    foreach (var contact in contacts)
                    {
                        Contacts.Add(contact);
                    }
                }

                var messages = await _apiService.GetMessagesAsync(CurrentUserId);

                if (messages == null || messages.Count == 0)
                {
                    NoMessages = true;
                    return;
                }

                foreach (var message in messages)
                {
                    Messages.Add(message);
                }
            }
            catch (Exception ex)
            {
                SetError($"An error occured: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Validates and sends a new message to the recipient.
        /// Related to US17 - Owner Messaging and US18 - Walker Messaging
        /// </summary>
        [RelayCommand]
        public async Task SendMessagesAsync()
        {
            if (string.IsNullOrWhiteSpace(NewMessage))
            {
                SetError("Please enter a message before sending.");
                return;
            }

            if (RecipientId <= 0)
            {
                SetError($"Please select a recipient. Current RecipientId: {RecipientId}");
                return;
            }

            IsBusy = true;
            ClearError();

            try
            {
                var senderId = await _authService.GetUserIdAsync();
                var request = new
                {
                    SenderId = senderId,
                    RecipientId,
                    Content = NewMessage
                };

                var result = await _apiService.SendMessageAsync(request);
                if (result == null)
                {
                    SetError("Failed to send message. Please try again.");
                    return;
                }

                Messages.Add(result);
                NewMessage = string.Empty;
                NoMessages = false;
            }
            catch (Exception ex)
            {
                SetError($"An error occured: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Sets the recipient Id to the selected contact
        /// </summary>
        partial void OnSelectedContactChanged(ContactModel? value)
        {
            if (value != null)
            {
                RecipientId = value.Id;
            }
        }

        /// <summary>
        /// Exposes LoadmessagesCommand as the basepage loadcommand
        /// </summary>
        public override IAsyncRelayCommand? LoadCommand => LoadMessagesCommand;
    }
}