using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class MedicoCalendarioPage : ContentPage
{
    public MedicoCalendarioPage(MedicoCalendarioViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}