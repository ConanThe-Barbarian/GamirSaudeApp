using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class EditarPerfilPage : ContentPage
{
    public EditarPerfilPage(EditarPerfilViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}