using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class ExamesEspecificosPage : ContentPage
{
    // 1. O construtor é atualizado para receber o ViewModel
    public ExamesEspecificosPage(ExamesEspecificosViewModel viewModel)
    {
        InitializeComponent();

        // 2. A "COLA": O cérebro da página é conectado
        BindingContext = viewModel;
    }

    // 3. O GATILHO: Executa a ação quando a página aparece
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ExamesEspecificosViewModel viewModel)
        {
            if (viewModel.PageAppearingCommand.CanExecute(null))
            {
                viewModel.PageAppearingCommand.Execute(null);
            }
        }
    }
}