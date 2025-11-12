
using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();

        // ESTA LINHA É A "COLA" QUE LIGA A VIEW AO VIEWMODEL. ELA É ESSENCIAL.
        BindingContext = viewModel;
    }
}