using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Walkies.MAUI.Models;
using Walkies.MAUI.Services;

namespace Walkies.MAUI.ViewModels
{
    /// <summary>
    /// ViewModel for the dog management pages. Handles loading, adding,
    /// editing and removing dogs from the owners profile.
    /// Related to US04 - Add Dog and US05 - Edit/Remove Dog
    /// </summary>
    public partial class DogViewModel(ApiService apiService, AuthService authService) : BaseViewModel
    {
        private readonly ApiService _apiService = apiService;
        private readonly AuthService _authService = authService;

        /// <summary>
        /// Gets the list of dogs belonging to the logged in owner
        /// </summary>
        public ObservableCollection<DogModel> Dogs { get; } = [];

        /// <summary>
        /// Gets or sets the dog name
        /// </summary>
        [ObservableProperty]
        public partial string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating if the dogs name field has
        /// a valid input. Used for UI validation feedback.
        /// </summary>
        [ObservableProperty]
        public partial bool IsNameValid { get; set; } = true;

        /// <summary>
        /// gets or sets the dog breed
        /// </summary>
        [ObservableProperty]
        public partial string Breed { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating if the breed field has
        /// a valid input. Used for UI validation feedback.
        /// </summary>
        [ObservableProperty]
        public partial bool IsBreedValid { get; set; } = true;

        /// <summary>
        /// gets or sets the dog age
        /// </summary>
        [ObservableProperty]
        public partial int Age { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the age field has
        /// a valid input. Used for UI validation feedback.
        /// </summary>
        [ObservableProperty]
        public partial bool IsAgeValid { get; set; } = true;

        /// <summary>
        /// gets or sets the notes related to the dog
        /// </summary>
        [ObservableProperty]
        public partial string? Notes { get; set; }

        /// <summary>
        /// gets or sets the id of the dog being edited
        /// </summary>
        [ObservableProperty]
        public partial int? EditingDogId { get; set; }

        /// <summary>
        /// gets or sets a value indicating whether the form is in editing mode
        /// </summary>
        [ObservableProperty]
        public partial bool IsEditMode { get; set; }

        /// <summary>
        /// gets or sets a value indicating whether the dog has been saved successfully
        /// </summary>
        [ObservableProperty]
        public partial bool IsSaved { get; set; }

        /// <summary>
        /// Loads all the dogs belonging to the logged in owner from the API
        /// and populates the Dogs collection. Related to US04 - Add Dog
        /// </summary>
        [RelayCommand]
        public async Task LoadDogsAsync()
        {
            IsBusy = true;
            ClearError();

            try
            {
                var ownerId = await _authService.GetUserIdAsync();
                var dogs = await _apiService.GetDogsByOwnerAsync(ownerId);

                Dogs.Clear();
                if (dogs is not null)
                {
                    foreach (var dog in dogs)
                    {
                        Dogs.Add(dog);
                    }
                }
            }
            catch (Exception ex)
            {
                SetError("An error occurred: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Populates the form fields with the selected dog's information
        /// and sets the viewModel into edit mode. Related to US05 - Edit/Remove Dog
        /// </summary>
        [RelayCommand]
        public async Task EditDog(DogModel dog)
        {
            EditingDogId = dog.Id;
            Name = dog.Name;
            Breed = dog.Breed;
            Age = dog.Age;
            Notes = dog.Notes;
            IsEditMode = true;
            IsSaved = true;
            ClearError();
        }

        /// <summary>
        /// Validates input and either adds a new dog or updates an
        /// existing dog depending on whether the viewModel is in edit mode.
        /// Related to US04 - Add Dog and US05 - Edit/Remove Dog
        /// </summary>
        [RelayCommand]
        private async Task SaveDogAsync()
        {
            // validates user input and sets validation properties for UI feedback
            IsNameValid = !string.IsNullOrWhiteSpace(Name);
            IsBreedValid = !string.IsNullOrWhiteSpace(Breed);

            // if any validation fails, set an error message and return early
            if (!IsNameValid || !IsBreedValid)
            {
                SetError("Please correct the highlighted fields.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Name))
            {
                SetError("Dog name is required.");
                return;
            }

            IsBusy = true;
            ClearError();
            IsSaved = false;

            try
            {
                var ownerId = await _authService.GetUserIdAsync();

                if (IsEditMode && EditingDogId.HasValue)
                {
                    var request = new { Name, Breed, Age, Notes };
                    var response = await _apiService.UpdateDogAsync(EditingDogId.Value, request);

                    if (response == null)
                    {
                        SetError("Failed to update the dog. Please try again.");
                        return;
                    }
                }
                else
                {
                    var request = new { Name, Breed, Age, Notes, OwnerId = ownerId };
                    var response = await _apiService.AddDogAsync(request);

                    if (response == null)
                    {
                        SetError("Failed to add the dog. Please try again.");
                        return;
                    }
                }

                IsSaved = true;
                ResetForm();
                await LoadDogsAsync();
            }
            catch (Exception ex)
            {
                SetError($"An error occurred while saving the dog: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Displays a confirmation prompt and removes the selected dog
        /// from the owners profile if confirmed.
        /// Related to US05 - Edit/Remove Dog
        /// </summary>
        [RelayCommand]
        private async Task RemoveDogAsync(DogModel dog)
        {
            var confirmed = await Shell.Current.DisplayAlertAsync(
                "Remove Dog",
                $"Are you sure you want to remove {dog.Name} from your profile?",
                "Remove",
                "Cancel");

            if (!confirmed)
                return;

            IsBusy = true;
            ClearError();

            try
            {
                if (!await _apiService.DeleteDogAsync(dog.Id))
                {
                    SetError("Failed to remove the dog. Please try again.");
                    return;
                }
                Dogs.Remove(dog);
            }
            catch (Exception ex)
            {
                SetError($"An error occurred while removing the dog: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Navigates to the add dog page.  Related to US04 - Add Dog
        /// </summary>
        [RelayCommand]
        private static async Task NavigateToAddDogAsync()
        {
            await Shell.Current.GoToAsync("owner/adddog");
        }

        /// <summary>
        /// Resets the form fields and clears edit mode
        /// </summary>
        private void ResetForm()
        {
            Name = string.Empty;
            Breed = string.Empty;
            Age = 0;
            Notes = null;
            EditingDogId = null;
            IsEditMode = false;
        }

        /// <summary>
        /// Exposes the LoadDogsAsync method as the BasePage LoadCommand
        /// </summary>
        public override IAsyncRelayCommand? LoadCommand => LoadDogsCommand;
    }
}