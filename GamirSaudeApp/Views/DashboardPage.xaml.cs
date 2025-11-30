using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class DashboardPage : ContentPage
{
    // CORREÇÃO: Recebe o ViewModel via Injeção de Dependência
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();

        // Conecta o ViewModel à tela
        BindingContext = viewModel;
    }

    // Se tiver lógica de OnAppearing, mantenha aqui
}