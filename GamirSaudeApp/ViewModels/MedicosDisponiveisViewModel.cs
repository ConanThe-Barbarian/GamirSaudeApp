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
using System.Text.Json;

namespace GamirSaudeApp.ViewModels
{
    // Adicionado o QueryProperty para receber o IdProcedimento da tela anterior
    [QueryProperty(nameof(EspecialidadeNome), "EspecialidadeNome")]
    [QueryProperty(nameof(IdProcedimento), "IdProcedimento")]
    public partial class MedicosDisponiveisViewModel : BaseViewModel
    {
        private readonly GamirApiService _gamirApiService;
        private readonly UserDataService _userDataService;

        [ObservableProperty]
        string especialidadeNome;

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
        // ADICIONE ESTE MÉTODO ABAIXO
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

        // Propriedade para receber e armazenar o IdProcedimento
        [ObservableProperty]
        int idProcedimento;

        [ObservableProperty]
        private bool isBusy;

        private CalendarDay _diaSelecionado;

        public MedicosDisponiveisViewModel(GamirApiService gamirApiService, UserDataService userDataService)
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
            if (string.IsNullOrEmpty(EspecialidadeNome) || Medicos.Any()) return;

            var listaMedicos = await _gamirApiService.GetMedicosAsync(EspecialidadeNome);
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
            Debug.WriteLine($"Carregando dias (Consulta) para Médico: {MedicoSelecionado.Id}, Mês: {MesCorrente:MM/yyyy}");

            List<DiaDisponivel>? listaDiasApi = null;
            try
            {
                // Chama o método de CONSULTA (3 argumentos)
                var diasResult = await _gamirApiService.GetDiasDisponiveisAsync(MedicoSelecionado.Id, MesCorrente.Month, MesCorrente.Year);
                listaDiasApi = diasResult?.ToList(); // Converte para lista para usar no GerarCalendario
                Debug.WriteLine($"API retornou {listaDiasApi?.Count ?? 0} dias disponíveis para consulta.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRO ao buscar dias da API (Consulta): {ex.Message}");
                // Opcional: Mostrar alerta para o usuário
                await Shell.Current.DisplayAlert("Erro", "Não foi possível carregar a disponibilidade.", "OK");
                listaDiasApi = new List<DiaDisponivel>(); // Define como lista vazia em caso de erro
            }
            finally
            {
                IsBusy = false; // Finaliza o indicador de ocupado
            }

            // Chama GerarCalendario na thread principal, passando a lista (possivelmente vazia)
            await MainThread.InvokeOnMainThreadAsync(() => GerarCalendario(listaDiasApi));
        }

        // --- MÉTODO GerarCalendario CORRIGIDO ---
        private void GerarCalendario(List<DiaDisponivel>? diasDisponiveisApi)
        {
            DiasDoMes.Clear(); // Limpa a coleção antes de preencher

            try
            {
                var primeiroDiaDoMes = new DateTime(MesCorrente.Year, MesCorrente.Month, 1);
                int diasNoMes = DateTime.DaysInMonth(MesCorrente.Year, MesCorrente.Month);
                int diaDaSemanaInicio = (int)primeiroDiaDoMes.DayOfWeek; // 0=Domingo, 1=Segunda...

                // 1. Adiciona células VAZIAS para os dias antes do dia 1
                for (int i = 0; i < diaDaSemanaInicio; i++)
                {
                    DiasDoMes.Add(new CalendarDay { IsEmpty = true }); // CORREÇÃO: Cria objeto vazio
                }

                // 2. Adiciona os dias REAIS do mês
                for (int dia = 1; dia <= diasNoMes; dia++)
                {
                    var dataAtual = new DateTime(MesCorrente.Year, MesCorrente.Month, dia);
                    // Procura o status na lista da API para este dia específico
                    var diaApi = diasDisponiveisApi?.FirstOrDefault(d => d.Dia == dia);

                    DiasDoMes.Add(new CalendarDay
                    {
                        Date = dataAtual,
                        IsEmpty = false, // É um dia real
                        Status = diaApi?.Status ?? "Indisponivel", // Pega status da API ou define como Indisponivel
                        IsSelected = false // Estado inicial não selecionado
                    });
                }
                Debug.WriteLine($"Calendário gerado com {DiasDoMes.Count} células no total ({diasNoMes} dias reais).");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRO CRÍTICO ao gerar calendário: {ex.ToString()}");
                // Pode ser útil limpar o calendário em caso de erro grave
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
                // CORREÇÃO AQUI: Usando a propriedade IdProcedimento recebida via QueryProperty
                IdProcedimento = IdProcedimento
            };
            var listaHorarios = await _gamirApiService.GetHorariosDisponiveisAsync(request);
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

                if (_diaSelecionado != null)
                {
                    _diaSelecionado.IsSelected = false;
                    _diaSelecionado = null;
                }
                HorariosDisponiveis.Clear();
                HorarioSelecionado = null;
                ShowAgendarButton = false;

                await CarregarDiasDisponiveis();
            }
        }

        // --- COMANDO AGENDAR CONSULTA ---
        [RelayCommand]
        private async Task AgendarConsulta()
        {
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
                    // CORREÇÃO AQUI: Usando o valor recebido dinamicamente
                    IdProcedimento = IdProcedimento,
                    Minutos = 30 // Hardcoded por enquanto
                };

                // --- INÍCIO DA NOSSA LINHA DE DEPURAÇÃO ---
                string displayText = $@"
                        Médico: {MedicoSelecionado?.Apelido}
                        Data: {_diaSelecionado.Date:dd/MM/yyyy}
                        Horário: {HorarioSelecionado}
                        Especialidade: {EspecialidadeNome}
                        Duração Média: 30 minutos";

                bool continuar = await Shell.Current.DisplayAlert(
                    "Confirmação de Agendamento",
                    displayText.Trim(),
                    "Confirmar",
                    "Cancelar"
                );

                if (!continuar)
                {
                    IsBusy = false;
                    return;
                }
                // --- FIM DA ALTERAÇÃO ---
                // --- FIM DA NOSSA LINHA DE DEPURAÇÃO ---

                // Descomente e use a linha abaixo após o teste de depuração:
                var result = await _gamirApiService.AgendarConsultaAsync(request);
                //var result = true; // <--- REMOVA ISTO APÓS O TESTE DE DEPURAÇÃO!!!

                if (result)
                {
                    await Shell.Current.GoToAsync(nameof(AgendamentoSucessoPage));
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível agendar a consulta.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao agendar consulta: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}