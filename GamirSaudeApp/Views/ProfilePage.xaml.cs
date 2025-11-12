using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views
{
    public partial class ProfilePage : ContentPage
    {
        // Code-behind simples, apenas injeta e define o BindingContext
        public ProfilePage(ProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
        
        // O OnAppearing é tratado pelo Behavior no XAML
    }
}