// Local: GamirSaudeApp/Views/RedefinirSenhaPage.xaml.cs
using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class RedefinirSenhaPage : ContentPage
{
    public RedefinirSenhaPage(RedefinirSenhaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}