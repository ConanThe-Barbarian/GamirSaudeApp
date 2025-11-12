// GamirSaudeApp/Views/HistoricoPage.xaml.cs

using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views
{
    public partial class HistoricoPage : ContentPage
    {
        public HistoricoPage(HistoricoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        // --- RESTAURADO: O OnAppearing chama o comando manualmente ---
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is HistoricoViewModel vm)
            {
                // Verifica se o comando pode ser executado antes de chamá-lo
                if (vm.AppearingCommand.CanExecute(null))
                {
                    vm.AppearingCommand.Execute(null);
                }
            }
        }
        // --- FIM DA RESTAURAÇÃO ---
    }
}