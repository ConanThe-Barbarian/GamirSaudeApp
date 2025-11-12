using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace GamirSaudeApp.ViewModels
{
    // Recebe os parâmetros da tela anterior (ExamesEspecificos)
    [QueryProperty(nameof(ExameNome), "ExameNome")]
    [QueryProperty(nameof(IdProcedimento), "IdProcedimento")]
    public partial class MedicosExameViewModel : BaseViewModel
    {
        private readonly GamirApiService _gamirApiService;
        private readonly UserDataService _userDataService;

        [ObservableProperty]
        string exameNome;

        [ObservableProperty]
        int idProcedimento;

        [ObservableProperty]
        private ObservableCollection<Medico> medicos;

        [ObservableProperty]
        private Medico medicoSelecionado;

        [ObservableProperty]
        private DateTime mesCorrente;

        [ObservableProperty]
        private ObservableCollection<CalendarDay> diasDoMes;

        [ObservableProperty]
        private ObservableCollection<string> horariosDisponiveis;

        [ObservableProperty]
        private string horarioSelecionado;
        partial void OnHorarioSelecionadoChanged(string value)
        {
            // Se um valor válido for selecionado (não nulo), mostra o botão
            if (!string.IsNullOrEmpty(value))
            {
                ShowAgendarButton = true;
            }
        }

        [ObservableProperty]
        private bool showAgendarButton;

        [ObservableProperty]
        private bool isBusy;

        private CalendarDay _diaSelecionado;

        public MedicosExameViewModel(GamirApiService gamirApiService, UserDataService userDataService)
        {
            _gamirApiService = gamirApiService;
            _userDataService = userDataService;
            Medicos = new ObservableCollection<Medico>();
            DiasDoMes = new ObservableCollection<CalendarDay>();
            HorariosDisponiveis = new ObservableCollection<string>();
            MesCorrente = DateTime.Today;
        }

        [RelayCommand]
        private async Task PageAppearing() 
        {
            if (IdProcedimento == 0 || Medicos.Any()) return;

            // CHAMA O NOVO MÉTODO DE SERVIÇO
            var listaMedicos = await _gamirApiService.GetMedicosPorProcedimentoAsync(IdProcedimento);
            if (listaMedicos != null && listaMedicos.Any())
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Medicos.Clear();
                    foreach (var medico in listaMedicos) Medicos.Add(medico);
                });
            }
        }

        async partial void OnMedicoSelecionadoChanged(Medico value)
        {
            await CarregarDiasDisponiveis();
        }

        private async Task CarregarDiasDisponiveis()
        {
            ShowAgendarButton = false;
            HorariosDisponiveis.Clear();
            if (_diaSelecionado != null) _diaSelecionado.IsSelected = false;
            _diaSelecionado = null; // Garante que _diaSelecionado seja resetado

            if (MedicoSelecionado == null)
            {
                // Limpa o calendário se nenhum médico estiver selecionado
                await MainThread.InvokeOnMainThreadAsync(() => DiasDoMes.Clear());
                return;
            }

            IsBusy = true; // Inicia o indicador de ocupado
            Debug.WriteLine($"Carregando dias (Exame) para Medico: {MedicoSelecionado.Id}, Proc: {IdProcedimento}, Mês: {MesCorrente:MM/yyyy}");


            List<DiaDisponivel>? listaDiasApi = null;
            try
            {
                // USA O IdProcedimento na requisição
                var request = new DiasDisponiveisRequest
                {
                    IdMedico = MedicoSelecionado.Id,
                    Mes = MesCorrente.Month,
                    Ano = MesCorrente.Year,
                    IdProcedimento = IdProcedimento // <-- ESSENCIAL PARA O EXAME
                };
                // CHAMA O NOVO MÉTODO DE SERVIÇO DE EXAME
                var diasResult = await _gamirApiService.GetDiasDisponiveisExameAsync(request);
                listaDiasApi = diasResult?.ToList();
                Debug.WriteLine($"API retornou {listaDiasApi?.Count ?? 0} dias disponíveis para exame.");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRO ao buscar dias da API (Exame): {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Não foi possível carregar a disponibilidade.", "OK");
                listaDiasApi = new List<DiaDisponivel>();
            }
            finally
            {
                IsBusy = false; // Finaliza o indicador de ocupado
            }

            // Chama GerarCalendario na thread principal
            await MainThread.InvokeOnMainThreadAsync(() => GerarCalendario(listaDiasApi));
        }

        // --- MÉTODO GerarCalendario CORRIGIDO (Idêntico ao do outro ViewModel) ---
        private void GerarCalendario(List<DiaDisponivel>? diasDisponiveisApi)
        {
            DiasDoMes.Clear();

            try
            {
                var primeiroDiaDoMes = new DateTime(MesCorrente.Year, MesCorrente.Month, 1);
                int diasNoMes = DateTime.DaysInMonth(MesCorrente.Year, MesCorrente.Month);
                int diaDaSemanaInicio = (int)primeiroDiaDoMes.DayOfWeek;

                // 1. Adiciona células VAZIAS
                for (int i = 0; i < diaDaSemanaInicio; i++)
                {
                    DiasDoMes.Add(new CalendarDay { IsEmpty = true }); // CORREÇÃO
                }

                // 2. Adiciona os dias REAIS
                for (int dia = 1; dia <= diasNoMes; dia++)
                {
                    var dataAtual = new DateTime(MesCorrente.Year, MesCorrente.Month, dia);
                    var diaApi = diasDisponiveisApi?.FirstOrDefault(d => d.Dia == dia);

                    DiasDoMes.Add(new CalendarDay
                    {
                        Date = dataAtual,
                        IsEmpty = false,
                        Status = diaApi?.Status ?? "Indisponivel",
                        IsSelected = false
                    });
                }
                Debug.WriteLine($"Calendário (Exame) gerado com {DiasDoMes.Count} células ({diasNoMes} dias reais).");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRO CRÍTICO ao gerar calendário (Exame): {ex.ToString()}");
                DiasDoMes.Clear();
                Shell.Current.DisplayAlert("Erro", "Ocorreu um erro inesperado ao montar o calendário.", "OK");
            }
        }

        [RelayCommand]
        private async Task SelecionarDia(CalendarDay dia)
        {
            if (dia == null || dia.IsEmpty || dia.Status == "Indisponivel") return;

            if (_diaSelecionado != null) _diaSelecionado.IsSelected = false;
            dia.IsSelected = true;
            _diaSelecionado = dia;

            HorariosDisponiveis.Clear();
            ShowAgendarButton = false;

            var request = new HorariosDisponiveisRequest
            {
                IdMedico = MedicoSelecionado.Id,
                Data = dia.Date,
                IdProcedimento = IdProcedimento // Usa o ID do Exame
            };

            // CHAMA O NOVO MÉTODO DE SERVIÇO
            var listaHorarios = await _gamirApiService.GetHorariosDisponiveisExameAsync(request);
            if (listaHorarios != null)
            {
                foreach (var horario in listaHorarios) HorariosDisponiveis.Add(horario);
            }
        }

        [RelayCommand]
        private void SelecionarHorario(string horario)
        {
            HorarioSelecionado = horario;
            ShowAgendarButton = true;
        }

        [RelayCommand]
        private async Task ChangeMonth(string direction)
        {
            if (int.TryParse(direction, out int monthsToAdd))
            {
                MesCorrente = MesCorrente.AddMonths(monthsToAdd);
                if (_diaSelecionado != null) { _diaSelecionado.IsSelected = false; _diaSelecionado = null; }
                HorariosDisponiveis.Clear();
                HorarioSelecionado = null;
                ShowAgendarButton = false;
                await CarregarDiasDisponiveis();
            }
        }

        [RelayCommand]
        private async Task AgendarConsulta()
        {
            // A lógica de agendamento (AgendarConsultaAsync) é a mesma
            // pois o endpoint /api/agendamento já aceita o IdProcedimento

            if (IsBusy || MedicoSelecionado == null || _diaSelecionado == null || string.IsNullOrEmpty(HorarioSelecionado)) return;
            IsBusy = true;
            try
            {
                var horaSelecionada = TimeSpan.Parse(HorarioSelecionado, CultureInfo.InvariantCulture);
                var dataHora = _diaSelecionado.Date.Date.Add(horaSelecionada);

                var request = new AgendamentoRequest
                {
                    IdPaciente = _userDataService.IdPaciente,
                    IdPrestadorMedico = MedicoSelecionado.Id,
                    DataHoraMarcada = dataHora,
                    IdProcedimento = IdProcedimento, // ID do Exame
                    Minutos = 30
                };

                // (Opcional: remover o DisplayAlert de confirmação)
                bool continuar = await Shell.Current.DisplayAlert("Confirmar Agendamento", $"Confirmar exame com {MedicoSelecionado.Nome} em {_diaSelecionado.Date:dd/MM/yyyy} às {HorarioSelecionado}?", "Confirmar", "Cancelar");
                if (!continuar) { IsBusy = false; return; }

                var result = await _gamirApiService.AgendarConsultaAsync(request);

                if (result)
                {
                    await Shell.Current.GoToAsync(nameof(AgendamentoSucessoPage));
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível agendar o exame.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao agendar exame: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}