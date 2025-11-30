using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System.Diagnostics;

namespace GamirSaudeApp.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly UserDataService _userDataService;
        private readonly GamirApiService _apiService;

        // --- PROPRIEDADES ---
        private string nomeUsuario;
        public string NomeUsuario
        {
            get => nomeUsuario;
            set => SetProperty(ref nomeUsuario, value);
        }

        private int pontosFidelidade;
        public int PontosFidelidade
        {
            get => pontosFidelidade;
            set => SetProperty(ref pontosFidelidade, value);
        }

        private bool isContaVerificada;
        public bool IsContaVerificada
        {
            get => isContaVerificada;
            set => SetProperty(ref isContaVerificada, value);
        }

        private bool isContaNaoVerificada;
        public bool IsContaNaoVerificada
        {
            get => isContaNaoVerificada;
            set => SetProperty(ref isContaNaoVerificada, value);
        }

        // --- IMAGEM ---
        private ImageSource fotoUsuario;
        public ImageSource FotoUsuario
        {
            get => fotoUsuario;
            set => SetProperty(ref fotoUsuario, value);
        }

        private bool temFoto;
        public bool TemFoto
        {
            get => temFoto;
            set => SetProperty(ref temFoto, value);
        }

        private bool naoTemFoto;
        public bool NaoTemFoto
        {
            get => naoTemFoto;
            set => SetProperty(ref naoTemFoto, value);
        }

        private int _totalAgendamentosRealizados = 0;

        public DashboardViewModel(UserDataService userDataService, GamirApiService apiService)
        {
            _userDataService = userDataService;
            _apiService = apiService;
        }

        // COMANDO DISPARADO AO ABRIR A TELA
        [RelayCommand]
        private async Task PageAppearing()
        {
            await CarregarDadosUsuario();
        }

        private async Task CarregarDadosUsuario()
        {
            // 1. Dados Básicos do Cofre
            NomeUsuario = _userDataService.NomeUsuario ?? "Visitante";
            IsContaVerificada = _userDataService.ContaVerificada;
            IsContaNaoVerificada = !IsContaVerificada;
            PontosFidelidade = IsContaVerificada ? 150 : 0;

            // 2. SEGURANÇA: Se não tem foto no cofre, tenta buscar na API
            if (string.IsNullOrEmpty(_userDataService.FotoPerfil) && _userDataService.IdUserApp > 0)
            {
                try
                {
                    var perfil = await _apiService.GetUserProfileAsync(_userDataService.IdUserApp);
                    if (perfil != null && !string.IsNullOrEmpty(perfil.FotoPerfil))
                    {
                        _userDataService.FotoPerfil = perfil.FotoPerfil; // Atualiza cofre
                    }
                }
                catch { } // Falha silenciosa, mantém sem foto
            }

            // 3. Processa a imagem para exibição
            AtualizarImagemVisual();
        }

        private void AtualizarImagemVisual()
        {
            if (!string.IsNullOrEmpty(_userDataService.FotoPerfil))
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(_userDataService.FotoPerfil);
                    FotoUsuario = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                    TemFoto = true;
                    NaoTemFoto = false;
                }
                catch
                {
                    TemFoto = false;
                    NaoTemFoto = true;
                }
            }
            else
            {
                TemFoto = false;
                NaoTemFoto = true;
            }
        }

        private async Task<bool> PodeAgendar()
        {
            if (IsContaVerificada) return true;
            if (_totalAgendamentosRealizados >= 1)
            {
                await Shell.Current.DisplayAlert("Limite", "Verifique sua conta.", "OK");
                return false;
            }
            return true;
        }

        // --- NAVEGAÇÃO ---
        [RelayCommand]
        private async Task NavigateToAgendarConsulta()
        {
            if (await PodeAgendar()) await Shell.Current.GoToAsync(nameof(AgendarConsultaPage));
        }

        [RelayCommand]
        private async Task NavigateToAgendarExame()
        {
            if (await PodeAgendar()) await Shell.Current.GoToAsync(nameof(AgendarExamePage));
        }

        [RelayCommand] private async Task NavigateToMeusAgendamentos() => await Shell.Current.GoToAsync(nameof(HistoricoPage));

        [RelayCommand]
        private async Task NavigateToMeusLaudos()
        {
            if (!IsContaVerificada) { await Shell.Current.DisplayAlert("Restrito", "Verifique sua conta.", "OK"); return; }
            await Shell.Current.GoToAsync(nameof(MeusLaudosPage));
        }

        [RelayCommand] private async Task NavigateToProfile() => await Shell.Current.GoToAsync(nameof(ProfilePage));

        [RelayCommand] private async Task Home() { }
        [RelayCommand] private async Task Chat() { await Shell.Current.DisplayAlert("Em Breve", "Chat em desenvolvimento.", "OK"); }
        [RelayCommand] private async Task Calendar() => await Shell.Current.GoToAsync(nameof(HistoricoPage));
    }
}