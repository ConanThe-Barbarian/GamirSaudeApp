using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class VerifyAccountPage : ContentPage
{
    public VerifyAccountPage(VerifyAccountViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}