using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class MedicosDisponiveisPage : ContentPage
{
    public MedicosDisponiveisPage(MedicosDisponiveisViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // --- MUDANÇA AQUI: O OnAppearing agora executa o comando ---
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MedicosDisponiveisViewModel viewModel)
        {
            if (viewModel.PageAppearingCommand.CanExecute(null))
            {
                viewModel.PageAppearingCommand.Execute(null);
            }
        }
    }
}