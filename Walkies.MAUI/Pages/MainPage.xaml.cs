using Walkies.MAUI.Models;
using Walkies.MAUI.PageModels;

namespace Walkies.MAUI.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}