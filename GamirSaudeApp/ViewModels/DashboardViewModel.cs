using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Views;
using GamirSaudeApp.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls; // Adicionado para DisplayAlert

namespace GamirSaudeApp.ViewModels
{
    [QueryProperty(nameof(NeedsRefresh), "refresh")]
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly UserDataService _userDataService;
        [ObservableProperty] string nomeUsuario;
        [ObservableProperty] int pontosFidelidade;
        [ObservableProperty] bool contaNaoVerificada;
        [ObservableProperty] bool needsRefresh;

        public DashboardViewModel(UserDataService userDataService)
        {
            _userDataService = userDataService;
            AtualizarDadosUsuario();
        }

        private void AtualizarDadosUsuario()
        {
            NomeUsuario = _userDataService.NomeUsuario;
            PontosFidelidade = 3200;
            ContaNaoVerificada = !_userDataService.ContaVerificada;
        }

        [RelayCommand]
        private void OnAppearing()
        {
            AtualizarDadosUsuario();
        }

        partial void OnNeedsRefreshChanged(bool value)
        {
            if (value)
            {
                AtualizarDadosUsuario();
                NeedsRefresh = false;
            }
        }

        // -----------------------------------------------------------
        // REFORÇO DE ACESSO: GATILHO AGENDAR CONSULTA
        // -----------------------------------------------------------
        [RelayCommand]
        private async Task AgendarConsulta()
        {
            if (!_userDataService.ContaVerificada)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Acesso Bloqueado",
                    "Sua conta precisa ser verificada para realizar agendamentos. Por favor, verifique seu e-mail para obter o ID de Paciente da clínica.",
                    "OK");
                return;
            }

            // Se verificado, prossegue com a navegação
            await Shell.Current.GoToAsync(nameof(AgendarConsultaPage));
        }

        // -----------------------------------------------------------
        // REFORÇO DE ACESSO: GATILHO AGENDAR EXAME
        // -----------------------------------------------------------
        [RelayCommand]
        private async Task AgendarExame()
        {
            if (!_userDataService.ContaVerificada)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Acesso Bloqueado",
                    "Sua conta precisa ser verificada para realizar agendamentos. Por favor, verifique seu e-mail para obter o ID de Paciente da clínica.",
                    "OK");
                return;
            }

            // Se verificado, prossegue com a navegação
            await Shell.Current.GoToAsync(nameof(AgendarExamePage));
        }


        [RelayCommand]
        private async Task VerHistorico()
        {
            await Shell.Current.GoToAsync(nameof(HistoricoPage));
        }

        // --- FIM DO COMANDO DE LOGOUT ---

        // --- NOVO COMANDO PARA NAVEGAR PARA O PERFIL ---
        [RelayCommand]
        private async Task GoToProfile()
        {
            await Shell.Current.GoToAsync(nameof(ProfilePage));
        }
        // --- FIM DO NOVO COMANDO ---
    }
}

    