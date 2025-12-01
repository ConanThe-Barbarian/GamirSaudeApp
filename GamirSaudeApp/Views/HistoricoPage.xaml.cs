using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views
{
    public partial class HistoricoPage : ContentPage
    {
        // Alteração: Recebemos a ViewModel pronta via construtor (Injeção de Dependência)
        public HistoricoPage(HistoricoViewModel viewModel)
        {
            InitializeComponent();

            // O MAUI já criou a viewModel com o ApiService e UserDataService dentro dela
            BindingContext = viewModel;
        }
    }
}