using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using GamirSaudeApp.Views.Popups;
using CommunityToolkit.Maui.Views;

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
                // 1. Prepara dados
                if (!TimeSpan.TryParse(HorarioSelecionado.Hora, out var horaSpan))
                    horaSpan = TimeSpan.ParseExact(HorarioSelecionado.Hora, "hh\\:mm", null);

                var dataHora = _diaSelecionado.Date.Date.Add(horaSpan);

                var request = new AgendamentoRequest
                {
                    IdPacienteLegado = 88922,
                    IdMedico = 89, // Mock
                    IdProcedimento = IdProcedimento, // <--- Aqui está o ID do Exame
                    DataHorario = dataHora,
                    Minutos = 30,
                    Observacao = "Exame agendado via App"
                };

                // 2. Popup de Confirmação (Estilo novo)
                string msg = $"Exame: {ExameNome}\n" +
                             $"Médico: {MedicoNome}\n" +
                             $"Data: {_diaSelecionado.Date:dd/MM/yyyy}\n" +
                             $"Horário: {HorarioSelecionado.Hora}\n" +
                             $"Valor: {ValorExame}";

                var popup = new ConfirmacaoAgendamentoPopup(msg);
                var resultado = await Shell.Current.CurrentPage.ShowPopupAsync(popup);

                if (resultado is bool confirmado && confirmado)
                {
                    // 3. Chamada API
                    var sucesso = await _gamirApiService.AgendarConsultaAsync(request);

                    if (sucesso)
                    {
                        // 4. Popup de Sucesso + Redirecionar
                        var popupSucesso = new AgendamentoSucessoPopup();
                        await Shell.Current.CurrentPage.ShowPopupAsync(popupSucesso);

                        await Shell.Current.GoToAsync("//DashboardPage");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erro", "Falha ao agendar.", "OK");
                    }
                }
                else
                {
                    IsBusy = false; // Cancelou
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
            }
            finally
            {
                if (IsBusy) IsBusy = false;
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