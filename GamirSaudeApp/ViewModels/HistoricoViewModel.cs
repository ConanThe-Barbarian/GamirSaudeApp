// GamirSaudeApp/ViewModels/HistoricoViewModel.cs

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models; // Para AgendamentoHistorico
using GamirSaudeApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls; // Para DisplayAlert
using System.Diagnostics; // Para Debug.WriteLine
using System; // Para Exception
using System.Collections.Generic; // Para IEnumerable

namespace GamirSaudeApp.ViewModels
{
    // GARANTIDO: Palavra-chave "partial" está presente
    public partial class HistoricoViewModel : BaseViewModel
    {
        private readonly GamirApiService _apiService;
        private readonly UserDataService _userDataService;

        [ObservableProperty]
        private ObservableCollection<AgendamentoHistorico> _agendamentos;

        public HistoricoViewModel(GamirApiService apiService, UserDataService userDataService)
        {
            _apiService = apiService;
            _userDataService = userDataService;
            Agendamentos = new ObservableCollection<AgendamentoHistorico>();
            Debug.WriteLine("--- HistoricoViewModel Construído ---"); // Log de construção
        }

        // GARANTIDO: Atributo [RelayCommand] está presente
        [RelayCommand]
        private async Task OnAppearing()
        {
            Debug.WriteLine("--- OnAppearing Executado ---"); // LOG 0
            await CarregarHistorico();
        }

        private async Task CarregarHistorico()
        {
            if (IsBusy)
            {
                Debug.WriteLine("CarregarHistorico: Abortado (IsBusy=true)");
                return;
            }
            IsBusy = true;
            Debug.WriteLine("--- Iniciando CarregarHistorico ---"); // LOG 1

            Agendamentos.Clear();

            // --- PONTO CRÍTICO 1: VERIFICAR O ID DO PACIENTE ---
            int pacienteId = _userDataService.IdPaciente;
            Debug.WriteLine($"Buscando histórico para IdPaciente: {pacienteId}"); // LOG 2

            if (pacienteId <= 0)
            {
                Debug.WriteLine("ERRO: IdPaciente inválido ou usuário não verificado."); // LOG 3
                await Application.Current.MainPage.DisplayAlert("Atenção", "Usuário não identificado ou conta não verificada. Não é possível carregar o histórico.", "OK");
                IsBusy = false;
                return; // Aborta se o ID for inválido
            }
            // --- FIM DA VERIFICAÇÃO ---

            IEnumerable<AgendamentoHistorico> lista = null; // Inicializa como null
            try
            {
                lista = await _apiService.GetHistoricoAgendamentosAsync(pacienteId);
                Debug.WriteLine($"Chamada API GetHistoricoAgendamentosAsync para IdPaciente {pacienteId} concluída."); // LOG 3.5
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRO NA CHAMADA API GetHistorico: {ex.Message}"); // LOG 4
                await Application.Current.MainPage.DisplayAlert("Erro de Rede", "Não foi possível buscar o histórico. Verifique sua conexão.", "OK");
            }


            if (lista != null)
            {
                Debug.WriteLine($"API retornou {lista.Count()} agendamentos."); // LOG 5
                // Usar Dispatcher para garantir que a atualização ocorra na thread da UI
                if (Application.Current.Dispatcher != null)
                {
                    Application.Current.Dispatcher.Dispatch(() =>
                    {
                        foreach (var item in lista.OrderByDescending(a => a.DataHoraMarcada))
                        {
                            Agendamentos.Add(item);
                        }
                        Debug.WriteLine($"Coleção 'Agendamentos' atualizada na UI com {Agendamentos.Count} itens."); // LOG 6
                    });
                }
                else
                {
                    // Fallback se dispatcher não estiver disponível (menos comum em MAUI padrão)
                    foreach (var item in lista.OrderByDescending(a => a.DataHoraMarcada))
                    {
                        Agendamentos.Add(item);
                    }
                    Debug.WriteLine($"Coleção 'Agendamentos' atualizada (sem dispatcher) com {Agendamentos.Count} itens.");
                }
            }
            else
            {
                Debug.WriteLine("API retornou NULL ou ocorreu erro na chamada."); // LOG 7
            }

            IsBusy = false;
            Debug.WriteLine("--- Finalizando CarregarHistorico ---"); // LOG 8
        }

        [RelayCommand]
        private async Task CancelarAgendamento(AgendamentoHistorico agendamento)
        {
            if (agendamento == null)
            {
                Debug.WriteLine("CancelarAgendamento: Abortado (agendamento nulo)");
                return;
            }
            if (!agendamento.PodeCancelar)
            {
                Debug.WriteLine($"CancelarAgendamento: Abortado (PodeCancelar=false para IdAgenda {agendamento.IdAgenda})");
                await Application.Current.MainPage.DisplayAlert("Atenção", "Este agendamento não pode mais ser cancelado (prazo expirado ou já realizado/cancelado).", "OK");
                return;
            }
            if (IsBusy)
            {
                Debug.WriteLine("CancelarAgendamento: Abortado (IsBusy=true)");
                return;
            }


            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                "Confirmar Cancelamento",
                $"Deseja realmente cancelar o agendamento com {agendamento.NomePrestador} em {agendamento.DataHoraMarcada:dd/MM/yyyy 'às' HH:mm}?",
                "Sim, Cancelar",
                "Não");

            if (!confirmar)
            {
                Debug.WriteLine($"Cancelamento IdAgenda {agendamento.IdAgenda} NÃO confirmado pelo usuário.");
                return;
            }

            IsBusy = true;
            Debug.WriteLine($"Iniciando cancelamento para IdAgenda: {agendamento.IdAgenda}, Usuário: {_userDataService.IdUserApp}");

            // Passando o IdUserApp corretamente
            var (success, message) = await _apiService.CancelarAgendamentoAsync(agendamento.IdAgenda, _userDataService.IdUserApp);

            Debug.WriteLine($"Resultado do cancelamento API: Success={success}, Message='{message}'");
            IsBusy = false;

            await Application.Current.MainPage.DisplayAlert(success ? "Sucesso" : "Erro", message, "OK");

            if (success)
            {
                Debug.WriteLine("Cancelamento bem-sucedido. Recarregando histórico...");
                await CarregarHistorico(); // Recarrega a lista para refletir a mudança
            }
            else
            {
                Debug.WriteLine("Falha no cancelamento reportada pela API.");
            }
        }
    }
}