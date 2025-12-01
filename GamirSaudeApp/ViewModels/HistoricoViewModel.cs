using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GamirSaudeApp.ViewModels
{
    public partial class HistoricoViewModel : BaseViewModel
    {
        private readonly GamirApiService _apiService;
        private readonly UserDataService _userDataService;

        public ObservableCollection<AgendamentoHistorico> AgendamentosExibidos { get; } = new();

        // Listas em memória para alternar abas sem recarregar API toda hora
        private List<AgendamentoHistorico> _todosPendentes = new();
        private List<AgendamentoHistorico> _todosFechados = new();

        [ObservableProperty]
        private bool isPendentesSelected = true;

        [ObservableProperty]
        private bool isFechadosSelected = false;

        [ObservableProperty]
        private bool isBusy;

        public HistoricoViewModel(GamirApiService apiService, UserDataService userDataService)
        {
            _apiService = apiService;
            _userDataService = userDataService;

            // Carrega os dados ao iniciar
            _ = CarregarHistorico();
        }

        private async Task CarregarHistorico()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // Busca o ID do paciente (Mock 88922 ou do serviço)
                int idPaciente = 88922;
                // Se tiver salvo no UserDataService: int.Parse(_userDataService.IdPacienteLegado ?? "0");

                var dadosApi = await _apiService.GetHistoricoAgendamentosAsync(idPaciente);

                _todosPendentes.Clear();
                _todosFechados.Clear();

                if (dadosApi != null)
                {
                    foreach (var item in dadosApi)
                    {
                        // A API (DTO) já diz se está desativado (Fechado) ou não
                        if (item.Desativado)
                            _todosFechados.Add(item);
                        else
                            _todosPendentes.Add(item);
                    }
                }

                // Atualiza a tela com a aba atual
                if (IsPendentesSelected) MostrarPendentes();
                else MostrarFechados();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar histórico: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Não foi possível carregar seus agendamentos.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void MostrarPendentes()
        {
            IsPendentesSelected = true;
            IsFechadosSelected = false;
            AgendamentosExibidos.Clear();
            foreach (var item in _todosPendentes) AgendamentosExibidos.Add(item);
        }

        [RelayCommand]
        private void MostrarFechados()
        {
            IsPendentesSelected = false;
            IsFechadosSelected = true;
            AgendamentosExibidos.Clear();
            foreach (var item in _todosFechados) AgendamentosExibidos.Add(item);
        }

        [RelayCommand]
        private async Task Voltar() => await Shell.Current.GoToAsync("..");

        // Comandos de ação (Placeholder)
        [RelayCommand] private async Task Confirmar(AgendamentoHistorico agendamento) => await Shell.Current.DisplayAlert("Info", "Funcionalidade de Check-in em breve.", "OK");
        [RelayCommand] private async Task Remarcar(AgendamentoHistorico agendamento) => await Shell.Current.DisplayAlert("Info", "Entre em contato para remarcar.", "OK");
        [RelayCommand] private async Task Cancelar(AgendamentoHistorico agendamento) => await Shell.Current.DisplayAlert("Info", "Entre em contato para cancelar.", "OK");
    }
}