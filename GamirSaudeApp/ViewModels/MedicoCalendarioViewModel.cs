using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using GamirSaudeApp.Views.Popups;
using CommunityToolkit.Maui.Views;

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

                    // CORREÇÃO 1: Usamos d.Data.Day em vez de d.Dia
                    var diaApi = diasDisponiveisApi?.FirstOrDefault(d => d.Data.Day == dia);

                    // CORREÇÃO 2: Mapeamos bool Disponivel para string Status
                    string status = (diaApi != null && diaApi.Disponivel) ? "Disponivel" : "Indisponivel";

                    DiasDoMes.Add(new CalendarDay
                    {
                        Date = dataAtual,
                        IsEmpty = false,
                        Status = status, // Agora usa a variável calculada
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
            // 1. Evita cliques duplos
            if (IsBusy || HorarioSelecionado == null) return;

            IsBusy = true;

            try
            {
                // 2. Preparação dos Dados (Data + Hora)
                // Tenta converter a string "09:00" para TimeSpan
                if (!TimeSpan.TryParse(HorarioSelecionado.Hora, out var horaSpan))
                {
                    // Fallback de segurança
                    horaSpan = TimeSpan.ParseExact(HorarioSelecionado.Hora, "hh\\:mm", null);
                }

                // Combina a data do dia selecionado com a hora escolhida
                var dataHora = _diaSelecionado.Date.Date.Add(horaSpan);

                // 3. Criar o Objeto de Requisição (DTO)
                var request = new AgendamentoRequest
                {
                    // ID do Paciente (Fixo 88922 conforme nosso Mock de verificação)
                    // No futuro: int.Parse(_userDataService.IdPacienteLegado)
                    IdPacienteLegado = 88922,

                    // ID do Médico (Fixo 89 para o Mock visual, no futuro virá da seleção anterior)
                    IdMedico = 89,

                    // IMPORTANTE: Null identifica que é uma CONSULTA MÉDICA, não exame
                    IdProcedimento = null,

                    DataHorario = dataHora,
                    Observacao = "Consulta marcada via App Gamir Saúde",
                    Minutos = 30 // Duração padrão
                };

                // 4. Exibir o POPUP PERSONALIZADO
                // Montamos a mensagem bonita para o usuário ler
                string mensagemConfirmacao = $"Médico: {MedicoNome}\n" +
                                             $"Especialidade: {EspecialidadeNome}\n" +
                                             $"Data: {_diaSelecionado.Date:dd/MM/yyyy}\n" +
                                             $"Horário: {HorarioSelecionado.Hora}\n" +
                                             $"Valor: {ValorConsulta}";

                var popup = new ConfirmacaoAgendamentoPopup(mensagemConfirmacao);

                // Espera o usuário clicar em "Sim" ou "Não"
                var resultado = await Shell.Current.CurrentPage.ShowPopupAsync(popup);

                // 5. Verificar a resposta do Popup
                if (resultado is bool confirmado && confirmado)
                {
                    // --- O USUÁRIO DISSE SIM ---

                    // Chama a API de verdade
                    var sucesso = await _gamirApiService.AgendarConsultaAsync(request);

                    if (sucesso)
                    {
                        // 1. Mostra o Popup de Sucesso
                        var popupSucesso = new AgendamentoSucessoPopup();
                        await Shell.Current.CurrentPage.ShowPopupAsync(popupSucesso);

                        // 2. Quando o popup fechar (usuário clicar em OK), redireciona para o Início
                        // O "//" reseta a pilha de navegação, voltando para a raiz
                        await Shell.Current.GoToAsync("//DashboardPage");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Ops", "Não foi possível agendar. Tente outro horário.", "OK");
                    }
                }

                else
                {
                    // --- O USUÁRIO DISSE NÃO (CANCELOU) ---
                    // Apenas liberamos o botão para clicar de novo se quiser
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRO CRÍTICO AO AGENDAR: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Falha de comunicação com o servidor. Verifique sua internet.", "OK");
            }
            finally
            {
                // Garante que o indicador de carregamento pare (apenas se não navegou)
                // Se navegou para SucessoPage, o IsBusy não importa tanto pois a tela mudou.
                if (IsBusy) IsBusy = false;
            }
        }
        

        [RelayCommand] private async Task Voltar() => await Shell.Current.GoToAsync("..");

        // Nav Bar
        [RelayCommand] private async Task Home() => await Shell.Current.GoToAsync("//DashboardPage");
        [RelayCommand] private async Task Chat() { }
        [RelayCommand] private async Task Profile() => await Shell.Current.GoToAsync(nameof(ProfilePage));
        [RelayCommand] private async Task Calendar() => await Shell.Current.GoToAsync(nameof(HistoricoPage));
    }
}