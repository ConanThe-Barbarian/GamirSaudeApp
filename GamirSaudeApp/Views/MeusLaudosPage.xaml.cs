using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class MeusLaudosPage : ContentPage
{
    public MeusLaudosPage()
    {
        InitializeComponent();
        BindingContext = new MeusLaudosViewModel();
    }
}