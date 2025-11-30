using System;
using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class ExameCalendarioPage : ContentPage
{
    // O ViewModel agora é injetado automaticamente pelo MAUI
    public ExameCalendarioPage(ExameCalendarioViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Garante que o carregamento inicial aconteça
        if (BindingContext is ExameCalendarioViewModel viewModel)
        {
            if (viewModel.PageAppearingCommand.CanExecute(null))
            {
                viewModel.PageAppearingCommand.Execute(null);
            }
        }
    }
}