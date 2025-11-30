using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class MedicosExameListaPage : ContentPage
{
    public MedicosExameListaPage(MedicosExameListaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}