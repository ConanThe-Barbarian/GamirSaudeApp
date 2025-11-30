using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Views; // Se precisar navegar
using System.Collections.ObjectModel;

namespace GamirSaudeApp.ViewModels
{
    public partial class MeusLaudosViewModel : BaseViewModel
    {
        // Datas para o filtro
        [ObservableProperty]
        private DateTime dataInicio = DateTime.Today.AddMonths(-1); // Padrão: últimos 30 dias

        [ObservableProperty]
        private DateTime dataFim = DateTime.Today;

        // Lista completa (banco de dados simulado)
        private List<LaudoModel> _todosLaudos;

        // Lista exibida na tela
        public ObservableCollection<LaudoModel> LaudosExibidos { get; } = new();

        public MeusLaudosViewModel()
        {
            CarregarMocks();
            FiltrarLaudos(); // Carrega a lista inicial
        }

        private void CarregarMocks()
        {
            _todosLaudos = new List<LaudoModel>
            {
                new LaudoModel {
                    NomeExame = "HEMOGRAMA COMPLETO",
                    DataExame = DateTime.Today.AddDays(-2),
                    Status = "Liberado",
                    NomeMedico = "Dr. João Silva",
                    Especialidade = "Clínico Geral"
                },
                new LaudoModel {
                    NomeExame = "RAIO X TÓRAX",
                    DataExame = DateTime.Today.AddDays(-5),
                    Status = "Liberado",
                    NomeMedico = "Dra. Maria Souza",
                    Especialidade = "Radiologia"
                },
                new LaudoModel {
                    NomeExame = "RESSONÂNCIA MAGNÉTICA",
                    DataExame = DateTime.Today,
                    Status = "Pendente",
                    NomeMedico = "Dr. Pedro Santos",
                    Especialidade = "Neurologia"
                }
            };
        }

        [RelayCommand]
        private void FiltrarLaudos()
        {
            if (DataInicio > DataFim)
            {
                Shell.Current.DisplayAlert("Atenção", "A data inicial não pode ser maior que a final.", "OK");
                return;
            }

            IsBusy = true;

            // Filtra a lista mockada
            var filtrados = _todosLaudos
                .Where(l => l.DataExame.Date >= DataInicio.Date && l.DataExame.Date <= DataFim.Date)
                .OrderByDescending(l => l.DataExame) // Mais recentes primeiro
                .ToList();

            LaudosExibidos.Clear();
            foreach (var laudo in filtrados)
            {
                LaudosExibidos.Add(laudo);
            }

            IsBusy = false;
        }

        [RelayCommand]
        private async Task VerLaudo(LaudoModel laudo)
        {
            if (!laudo.IsLiberado) return;

            // NAVEGA PARA A NOVA TELA DE DETALHES
            // Passamos os dados via URL
            var navigationParameter = new Dictionary<string, object>
            {
                { "LaudoSelecionado", laudo }
            };

            // Usando passagem de objeto direta (requer registro correto no Shell)
            // Ou passando propriedade a propriedade na URL se preferir simplicidade:
            await Shell.Current.GoToAsync($"{nameof(VerLaudoPage)}?NomeExame={laudo.NomeExame}&NomeMedico={laudo.NomeMedico}&Especialidade={laudo.Especialidade}&Data={laudo.DataExame:dd/MM/yyyy}");
        }
        [RelayCommand] private async Task Voltar() => await Shell.Current.GoToAsync("..");

        // Barra Inferior
        [RelayCommand] private async Task Home() => await Shell.Current.GoToAsync("//DashboardPage");
        [RelayCommand] private async Task Chat() { }
        [RelayCommand] private async Task Profile() => await Shell.Current.GoToAsync(nameof(ProfilePage));
        [RelayCommand] private async Task Calendar() => await Shell.Current.GoToAsync(nameof(HistoricoPage));
    }
}