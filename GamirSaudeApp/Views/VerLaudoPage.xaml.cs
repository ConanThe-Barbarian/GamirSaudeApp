using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views
{
    public partial class VerLaudoPage : ContentPage
    {
        public VerLaudoPage(VerLaudoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}