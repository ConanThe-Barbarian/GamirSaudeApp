using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class AgendarExamePage : ContentPage
{
    // 1. O construtor agora recebe o ViewModel via Injeção de Dependência
    public AgendarExamePage(AgendarExameViewModel viewModel)
    {
        InitializeComponent();

        // 2. A "COLA": Definimos o cérebro (BindingContext) da nossa página
        BindingContext = viewModel;
    }

    // 3. O GATILHO: Este método é chamado sempre que a página vai aparece    }
}