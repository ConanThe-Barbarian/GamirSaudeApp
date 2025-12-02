using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GamirSaudeApp.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly GamirApiService _apiService;
        private readonly UserDataService _userDataService;

        [ObservableProperty]
        private string nomeUsuario;

        [ObservableProperty]
        private ImageSource fotoPerfilSource;

        // NOVA PROPRIEDADE: PONTOS
        [ObservableProperty]
        private int pontosFidelidade;

        public ObservableCollection<DashboardBannerItem> Banners { get; } = new();

        public DashboardViewModel(GamirApiService apiService, UserDataService userDataService)
        {
            _apiService = apiService;
            _userDataService = userDataService;

            CarregarDadosUsuario();
            _ = CarregarDashboard();
        }

        private void CarregarDadosUsuario()
        {
            // 1. Nome
            string nomeCompleto = _userDataService.NomeUsuario ?? "Paciente";
            var partesNome = nomeCompleto.Split(' ');
            NomeUsuario = partesNome.Length > 0 ? partesNome[0] : nomeCompleto;

            // 2. Pontos Fidelidade (Mock - Futuramente virá da API)
            pontosFidelidade = 1250;

            // 3. Foto
            string fotoString = _userDataService.FotoPerfil;
            if (string.IsNullOrEmpty(fotoString))
            {
                FotoPerfilSource = "profile_icon.png";
            }
            else
            {
                try
                {
                    if (!fotoString.StartsWith("http"))
                    {
                        byte[] imageBytes = Convert.FromBase64String(fotoString);
                        FotoPerfilSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                    }
                    else
                    {
                        FotoPerfilSource = ImageSource.FromUri(new Uri(fotoString));
                    }
                }
                catch
                {
                    FotoPerfilSource = "profile_icon.png";
                }
            }
        }

        private async Task CarregarDashboard()
        {
            try
            {
                Banners.Clear();

                var proximo = await ObterProximoAgendamento();

                if (proximo != null)
                {
                    Banners.Add(new DashboardBannerItem
                    {
                        Titulo = "Sua Próxima Consulta",
                        Descricao = $"{proximo.NomePrestador} - {proximo.Especialidade}",
                        DetalhePrincipal = proximo.DataHoraMarcada.ToString("dd/MM 'às' HH:mm"),
                        Icone = "calendar_icon.png",
                        CorFundo = Color.FromArgb("#0098DA"),
                        IsAgendamento = true
                    });
                }
                else
                {
                    Banners.Add(new DashboardBannerItem
                    {
                        Titulo = "Sem Agendamentos",
                        Descricao = "Não há consultas futuras.",
                        DetalhePrincipal = "Agendar Agora",
                        Icone = "calendar_icon.png",
                        CorFundo = Color.FromArgb("#95A5A6"),
                        IsAgendamento = false
                    });
                }

                Banners.Add(new DashboardBannerItem
                {
                    Titulo = "Preços Populares",
                    Descricao = "Qualidade acessível.",
                    DetalhePrincipal = "A partir de R$ 75,00",
                    Icone = "home_icon.png",
                    CorFundo = Color.FromArgb("#27AE60"),
                    IsAgendamento = false
                });

                Banners.Add(new DashboardBannerItem
                {
                    Titulo = "Aviso Importante",
                    Descricao = "Dra. Maristela e Dra. Karina",
                    DetalhePrincipal = "Agende por Telefone",
                    Icone = "chat_icon.png",
                    CorFundo = Color.FromArgb("#E67E22"),
                    IsAgendamento = false
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro Dashboard: {ex.Message}");
            }
        }

        private async Task<AgendamentoHistorico> ObterProximoAgendamento()
        {
            try
            {
                int idPaciente = 88922;
                var historico = await _apiService.GetHistoricoAgendamentosAsync(idPaciente);

                if (historico != null)
                {
                    return historico
                        .Where(a => a.DataHoraMarcada > DateTime.Now && !a.Desativado)
                        .OrderBy(a => a.DataHoraMarcada)
                        .FirstOrDefault();
                }
            }
            catch { }
            return null;
        }

        // COMANDOS
        [RelayCommand] private async Task NovaConsulta() => await Shell.Current.GoToAsync(nameof(AgendarConsultaPage));
        [RelayCommand] private async Task NovoExame() => await Shell.Current.GoToAsync(nameof(AgendarExamePage));
        [RelayCommand] private async Task MeusAgendamentos() => await Shell.Current.GoToAsync(nameof(HistoricoPage));
        [RelayCommand] private async Task MeusLaudos() => await Shell.Current.GoToAsync(nameof(MeusLaudosPage));
        [RelayCommand] private async Task VerDetalhesAgendamento() => await Shell.Current.GoToAsync(nameof(HistoricoPage));
        [RelayCommand] private async Task IrParaPerfil() => await Shell.Current.GoToAsync(nameof(ProfilePage));

        // BARRA INFERIOR
        [RelayCommand] private async Task Home() { }
        [RelayCommand] private async Task Chat() => await Shell.Current.DisplayAlert("Contato", "Ligue para: (21) 9999-9999", "OK");
        [RelayCommand] private async Task Profile() => await Shell.Current.GoToAsync(nameof(ProfilePage));
        [RelayCommand] private async Task Calendar() => await Shell.Current.GoToAsync(nameof(HistoricoPage));
    }
}