
using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class EsqueciSenhaPage : ContentPage
{
    public EsqueciSenhaPage(EsqueciSenhaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}