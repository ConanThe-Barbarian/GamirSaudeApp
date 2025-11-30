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
    public partial class HistoricoViewModel : BaseViewModel
    {
        public ObservableCollection<AgendamentoHistorico> AgendamentosExibidos { get; } = new();

        private List<AgendamentoHistorico> _todosPendentes;
        private List<AgendamentoHistorico> _todosFechados;

        [ObservableProperty]
        private bool isPendentesSelected = true;

        [ObservableProperty]
        private bool isFechadosSelected = false;

        public HistoricoViewModel()
        {
            CarregarDadosMock();
            MostrarPendentes();
        }

        private void CarregarDadosMock()
        {
            _todosPendentes = new List<AgendamentoHistorico>
            {
                // Exemplo 1: Exame
                new AgendamentoHistorico {
                    NomePrestador = "Dr. Oswaldo",
                    Especialidade = "Cardiologista",
                    Procedimento = "ECO/DOPPLER COLORIDO", // <--- Procedimento
                    Valor = 150.00m,                       // <--- Valor
                    DataHoraMarcada = DateTime.Now.AddDays(2),
                    Desativado = false
                },
                // Exemplo 2: Consulta
                new AgendamentoHistorico {
                    NomePrestador = "Dra. Ana Paula",
                    Especialidade = "Dermatologista",
                    Procedimento = "CONSULTA MÉDICA",      // <--- Procedimento
                    Valor = 300.00m,                       // <--- Valor
                    DataHoraMarcada = DateTime.Now.AddDays(5),
                    Desativado = false
                },
                 // Exemplo 3: Revisão (Gratis?)
                new AgendamentoHistorico {
                    NomePrestador = "Dr. Ricardo",
                    Especialidade = "Ortopedista",
                    Procedimento = "REVISÃO DE CONSULTA",  // <--- Procedimento
                    Valor = 0.00m,                         // <--- Valor Zero
                    DataHoraMarcada = DateTime.Now.AddDays(8),
                    Desativado = false
                }
            };

            _todosFechados = new List<AgendamentoHistorico>
            {
                new AgendamentoHistorico {
                    NomePrestador = "Dr Tomaz",
                    Especialidade = "Cardiologista",
                    Procedimento = "ELETROCARDIOGRAMA",
                    Valor = 100.00m,
                    DataHoraMarcada = DateTime.Now.AddDays(-10),
                    Desativado = false
                },
                new AgendamentoHistorico {
                    NomePrestador = "Dra Karina",
                    Especialidade = "Ginecologista",
                    Procedimento = "CONSULTA DE ROTINA",
                    Valor = 250.00m,
                    DataHoraMarcada = DateTime.Now.AddDays(-20),
                    Desativado = true
                }
            };
        }

        [RelayCommand]
        private void MostrarPendentes()
        {
            IsPendentesSelected = true;
            IsFechadosSelected = false;
            AtualizarLista(_todosPendentes);
        }

        [RelayCommand]
        private void MostrarFechados()
        {
            IsPendentesSelected = false;
            IsFechadosSelected = true;
            AtualizarLista(_todosFechados);
        }

        private void AtualizarLista(List<AgendamentoHistorico> lista)
        {
            AgendamentosExibidos.Clear();
            foreach (var item in lista)
            {
                AgendamentosExibidos.Add(item);
            }
        }

        [RelayCommand]
        private async Task Voltar() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        private async Task Confirmar(AgendamentoHistorico agendamento) => await Shell.Current.DisplayAlert("Ação", $"Confirmar {agendamento.NomePrestador}", "OK");

        [RelayCommand]
        private async Task Remarcar(AgendamentoHistorico agendamento) => await Shell.Current.DisplayAlert("Ação", $"Remarcar {agendamento.NomePrestador}", "OK");

        [RelayCommand]
        private async Task Cancelar(AgendamentoHistorico agendamento) => await Shell.Current.DisplayAlert("Ação", $"Cancelar {agendamento.NomePrestador}", "OK");
    }
}