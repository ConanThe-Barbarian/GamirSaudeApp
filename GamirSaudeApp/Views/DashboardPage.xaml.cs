using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class DashboardPage : ContentPage
{
    // Apenas UM construtor é necessário
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Apenas UM método OnAppearing é necessário
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DashboardViewModel vm)
        {
            // A chamada deve corresponder ao nome do método + "Command"
            vm.AppearingCommand.Execute(null);
        }
    }
}