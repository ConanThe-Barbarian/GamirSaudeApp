using GamirSaudeApp.ViewModels;
using System;


namespace GamirSaudeApp.Views;

public partial class MedicosExamePage : ContentPage
{
    // Injeta o ViewModel específico para Exames
    public MedicosExamePage(MedicosExameViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Define o contexto de binding da página
    }

    // --- ADICIONE ESTE MÉTODO ---
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Garante que o BindingContext é o ViewModel correto
        if (BindingContext is MedicosExameViewModel viewModel)
        {
            // Dispara o comando para carregar os dados
            if (viewModel.PageAppearingCommand.CanExecute(null))
            {
                viewModel.PageAppearingCommand.Execute(null);
            }
        }
    }
}