using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace GamirSaudeApp.ViewModels
{
    [QueryProperty(nameof(ExameNome), "ExameNome")]
    [QueryProperty(nameof(IdProcedimento), "IdProcedimento")]
    [QueryProperty(nameof(MedicoNome), "MedicoNome")]
    [QueryProperty(nameof(ValorExame), "Valor")]
    public partial class ExameCalendarioViewModel : BaseViewModel
    {
        private readonly GamirApiService _gamirApiService;
        private readonly UserDataService _userDataService;

        // --- PROPRIEDADES EXPLÍCITAS ---

        private string exameNome;
        public string ExameNome
        {
            get => exameNome;
            set => SetProperty(ref exameNome, value);
        }

        private int idProcedimento;
        public int IdProcedimento
        {
            get => idProcedimento;
            set => SetProperty(ref idProcedimento, value);
        }

        private string medicoNome;
        public string MedicoNome
        {
            get => medicoNome;
            set
            {
                if (SetProperty(ref medicoNome, value))
                {
                    // Assim que o nome do médico chegar, carregamos o calendário
                    if (!string.IsNullOrEmpty(value)) _ = CarregarDiasDisponiveis();
                }
            }
        }

        private string valorExame;
        public string ValorExame
        {
            get => valorExame;
            set => SetProperty(ref valorExame, value);
        }

        private DateTime mesCorrente = DateTime.Today;
        public DateTime MesCorrente
        {
            get => mesCorrente;
            set => SetProperty(ref mesCorrente, value);
        }

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

        private bool showAgendarButton;
        public bool ShowAgendarButton
        {
            get => showAgendarButton;
            set => SetProperty(ref showAgendarButton, value);
        }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        // --- COLEÇÕES ---
        public ObservableCollection<CalendarDay> DiasDoMes { get; } = new();
        public ObservableCollection<HorarioModel> HorariosDisponiveis { get; } = new();

        private CalendarDay _diaSelecionado;

        // --- CONSTRUTOR ---
        public ExameCalendarioViewModel(GamirApiService gamirApiService, UserDataService userDataService)
        {
            _gamirApiService = gamirApiService;
            _userDataService = userDataService;
        }

        // --- COMANDOS ---

        // CORREÇÃO: Adicionado este comando para satisfazer a chamada do OnAppearing na View
        [RelayCommand]
        private async Task PageAppearing()
        {
            // O carregamento principal já é feito via OnMedicoNomeChanged, 
            // mas mantemos este método caso precise de lógica extra ao exibir a tela.
            await Task.CompletedTask;
        }

        private async Task CarregarDiasDisponiveis()
        {
            ShowAgendarButton = false;
            HorariosDisponiveis.Clear();
            IsBusy = true;

            List<DiaDisponivel>? listaDiasApi = null;
            try
            {
                var request = new DiasDisponiveisRequest
                {
                    IdMedico = 89,
                    Mes = MesCorrente.Month,
                    Ano = MesCorrente.Year,
                    IdProcedimento = IdProcedimento // <--- Agora existe (nullable)
                };

                var diasResult = await _gamirApiService.GetDiasDisponiveisExameAsync(request);
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

        private void GerarCalendario(List<DiaDisponivel>? diasApi)
        {
            DiasDoMes.Clear();
            try
            {
                var primeiroDia = new DateTime(MesCorrente.Year, MesCorrente.Month, 1);
                int diasNoMes = DateTime.DaysInMonth(MesCorrente.Year, MesCorrente.Month);
                int offset = (int)primeiroDia.DayOfWeek;

                for (int i = 0; i < offset; i++) DiasDoMes.Add(new CalendarDay { IsEmpty = true });

                for (int dia = 1; dia <= diasNoMes; dia++)
                {
                    var data = new DateTime(MesCorrente.Year, MesCorrente.Month, dia);

                    // CORREÇÃO: Comparar Data.Day
                    var apiData = diasApi?.FirstOrDefault(d => d.Data.Day == dia);

                    // CORREÇÃO: Mapear bool -> string
                    string status = (apiData != null && apiData.Disponivel) ? "Disponivel" : "Indisponivel";

                    DiasDoMes.Add(new CalendarDay
                    {
                        Date = data,
                        IsEmpty = false,
                        Status = status,
                        IsSelected = false
                    });
                }
            }
            catch { }
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

            // Adicione IdProcedimento ao request
            var request = new HorariosDisponiveisRequest
            {
                IdMedico = 89,
                Data = dia.Date,
                IdProcedimento = IdProcedimento
            };
            
            var lista = await _gamirApiService.GetHorariosDisponiveisExameAsync(request);

            if (lista != null)
            {
                foreach (var h in lista)
                    HorariosDisponiveis.Add(new HorarioModel { Hora = h, IsSelected = false });
            }
        }

        [RelayCommand]
        private async Task ChangeMonth(string dir)
        {
            if (int.TryParse(dir, out int val))
            {
                MesCorrente = MesCorrente.AddMonths(val);
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
            try
            {
                var horaSpan = TimeSpan.Parse(HorarioSelecionado.Hora, CultureInfo.InvariantCulture);
                var dataHora = _diaSelecionado.Date.Date.Add(horaSpan);

                // CORREÇÃO: Usando os nomes da Unified DTO
                var request = new AgendamentoRequest
                {
                    IdPacienteLegado = 88922, // Use IdPacienteLegado (pode pegar do _userDataService se tiver)
                    IdMedico = 89,            // Antes era IdPrestadorMedico
                    DataHorario = dataHora,   // Antes era DataHoraMarcada
                    IdProcedimento = IdProcedimento,
                    Minutos = 30,
                    Observacao = "Agendamento de Exame via App"
                };

                
                bool ok = await Shell.Current.DisplayAlert("Confirmar",
                    $"Agendar {ExameNome}\nCom: {MedicoNome}\nEm: {_diaSelecionado.Date:dd/MM} às {HorarioSelecionado.Hora}\nValor: {ValorExame}?",
                    "Sim", "Não");

                if (!ok) { IsBusy = false; return; }

                var result = await _gamirApiService.AgendarConsultaAsync(request);

                if (result) await Shell.Current.GoToAsync(nameof(AgendamentoSucessoPage));
                else await Shell.Current.DisplayAlert("Erro", "Falha ao agendar.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Comandos de Navegação
        [RelayCommand] private async Task Voltar() => await Shell.Current.GoToAsync("..");
        [RelayCommand] private async Task Home() => await Shell.Current.GoToAsync("//DashboardPage");
        [RelayCommand] private async Task Chat() { }
        [RelayCommand] private async Task Profile() => await Shell.Current.GoToAsync(nameof(ProfilePage));
        [RelayCommand] private async Task Calendar() => await Shell.Current.GoToAsync(nameof(HistoricoPage));
    }
}