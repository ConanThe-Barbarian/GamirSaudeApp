using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GamirSaudeApp.ViewModels
{
    // Recebe os dados do médico selecionado na lista anterior
    [QueryProperty(nameof(EspecialidadeNome), "Especialidade")]
    [QueryProperty(nameof(MedicoNome), "MedicoNome")]
    [QueryProperty(nameof(ValorConsulta), "Valor")]
    public partial class MedicoCalendarioViewModel : BaseViewModel
    {
        private readonly GamirApiService _gamirApiService;
        private readonly UserDataService _userDataService;

        [ObservableProperty]
        private string especialidadeNome;

        [ObservableProperty]
        private string medicoNome; // Nome do médico recebido

        [ObservableProperty]
        private string valorConsulta; // Valor recebido

        [ObservableProperty]
        private DateTime mesCorrente;

        public ObservableCollection<CalendarDay> DiasDoMes { get; } = new();
        public ObservableCollection<HorarioModel> HorariosDisponiveis { get; } = new();

        private HorarioModel horarioSelecionado;
        public HorarioModel HorarioSelecionado
        {
            get => horarioSelecionado;
            set
            {
                if (SetProperty(ref horarioSelecionado, value))
                {
                    if (value != null)
                    {
                        foreach (var h in HorariosDisponiveis) h.IsSelected = false;
                        value.IsSelected = true;
                        ShowAgendarButton = true;
                    }
                }
            }
        }

        [ObservableProperty]
        private bool showAgendarButton;

        [ObservableProperty]
        private bool isBusy;

        private CalendarDay _diaSelecionado;

        public MedicoCalendarioViewModel(GamirApiService gamirApiService, UserDataService userDataService)
        {
            _gamirApiService = gamirApiService;
            _userDataService = userDataService;
            MesCorrente = DateTime.Today;
        }

        // Chamado automaticamente quando a propriedade MedicoNome é preenchida pelo Shell
        partial void OnMedicoNomeChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                // Carrega o calendário imediatamente, pois já temos o médico
                _ = CarregarDiasDisponiveis();
            }
        }

        private async Task CarregarDiasDisponiveis()
        {
            ShowAgendarButton = false;
            HorariosDisponiveis.Clear();
            if (_diaSelecionado != null) _diaSelecionado.IsSelected = false;
            _diaSelecionado = null;
            HorarioSelecionado = null;

            IsBusy = true;

            List<DiaDisponivel>? listaDiasApi = null;
            try
            {
                // Nota: Aqui precisaríamos do ID real do médico. 
                // Como estamos no fluxo Mock visual, vou simular usando o nome.
                // No futuro, passaremos o IdMedico via QueryProperty também.
                int idMedicoSimulado = 89;

                var diasResult = await _gamirApiService.GetDiasDisponiveisAsync(idMedicoSimulado, MesCorrente.Month, MesCorrente.Year);
                listaDiasApi = diasResult?.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRO: {ex.Message}");
                listaDiasApi = new List<DiaDisponivel>();
            }
            finally
            {
                IsBusy = false;
            }

            await MainThread.InvokeOnMainThreadAsync(() => GerarCalendario(listaDiasApi));
        }

        private void GerarCalendario(List<DiaDisponivel>? diasDisponiveisApi)
        {
            DiasDoMes.Clear();
            try
            {
                var primeiroDiaDoMes = new DateTime(MesCorrente.Year, MesCorrente.Month, 1);
                int diasNoMes = DateTime.DaysInMonth(MesCorrente.Year, MesCorrente.Month);
                int diaDaSemanaInicio = (int)primeiroDiaDoMes.DayOfWeek;

                for (int i = 0; i < diaDaSemanaInicio; i++)
                {
                    DiasDoMes.Add(new CalendarDay { IsEmpty = true });
                }

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
            }
            catch (Exception) { /* Logs */ }
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
            HorarioSelecionado = null;

            // Simula busca de horários
            var listaHorarios = await _gamirApiService.GetHorariosDisponiveisAsync(new HorariosDisponiveisRequest { Data = dia.Date });
            if (listaHorarios != null)
            {
                foreach (var horario in listaHorarios)
                {
                    HorariosDisponiveis.Add(new HorarioModel { Hora = horario, IsSelected = false });
                }
            }
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
            if (IsBusy || HorarioSelecionado == null) return;
            IsBusy = true;

            // Lógica de Agendamento (Mock/Final)
            bool confirmar = await Shell.Current.DisplayAlert("Confirmar",
                $"Agendar com {MedicoNome}\nData: {_diaSelecionado.Date:dd/MM}\nHora: {HorarioSelecionado.Hora}\nValor: {ValorConsulta}?",
                "Sim", "Não");

            if (confirmar)
            {
                // await _gamirApiService.AgendarConsultaAsync(...);
                await Shell.Current.GoToAsync(nameof(AgendamentoSucessoPage));
            }

            IsBusy = false;
        }

        [RelayCommand] private async Task Voltar() => await Shell.Current.GoToAsync("..");

        // Nav Bar
        [RelayCommand] private async Task Home() => await Shell.Current.GoToAsync("//DashboardPage");
        [RelayCommand] private async Task Chat() { }
        [RelayCommand] private async Task Profile() => await Shell.Current.GoToAsync(nameof(ProfilePage));
        [RelayCommand] private async Task Calendar() => await Shell.Current.GoToAsync(nameof(HistoricoPage));
    }
}