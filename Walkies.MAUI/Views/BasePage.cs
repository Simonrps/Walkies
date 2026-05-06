namespace Walkies.MAUI.Views
{
    /// <summary>
    /// Base page for all content pages in the app. Automatically
    /// invokes LoadCommand on the ViewModel when the page appears
    /// </summary>
    public abstract class BasePage : ContentPage
    {
        /// <summary>
        /// Checks for a loadcommand on the BindingContext and
        /// executes it when the page appears.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is ViewModels.BaseViewModel vm &&
                vm.LoadCommand?.CanExecute(null) == true)
            {
                await vm.LoadCommand.ExecuteAsync(null);
            }
        }
    }
}
