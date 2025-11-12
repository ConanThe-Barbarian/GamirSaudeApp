using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class AgendarConsultaPage : ContentPage
{
    public AgendarConsultaPage(AgendarConsultaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // --- MUDANÇA AQUI: O OnAppearing agora executa o comando ---
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AgendarConsultaViewModel viewModel)
        {
            if (viewModel.PageAppearingCommand.CanExecute(null))
            {
                viewModel.PageAppearingCommand.Execute(null);
            }
        }
    }
}