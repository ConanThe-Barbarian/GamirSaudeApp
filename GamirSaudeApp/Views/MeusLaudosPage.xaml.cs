using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views
{
    public partial class MeusLaudosPage : ContentPage
    {      
        public MeusLaudosPage(MeusLaudosViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}