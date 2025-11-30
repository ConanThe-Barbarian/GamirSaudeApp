using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using GamirSaudeApp.Views.Popups;
using System.Diagnostics;

namespace GamirSaudeApp.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly UserDataService _userDataService;

        // --- PROPRIEDADES EXPLÍCITAS ---

        private string nomeUsuario;
        public string NomeUsuario
        {
            get => nomeUsuario;
            set => SetProperty(ref nomeUsuario, value);
        }

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

        // --- COMANDOS EXPLÍCITOS (Sem chance de erro) ---
        public IRelayCommand OnAppearingCommand { get; } // Comando Síncrono
        public IAsyncRelayCommand AlterarDadosCommand { get; }
        public IAsyncRelayCommand NotificacoesCommand { get; }
        public IAsyncRelayCommand VerificarContaCommand { get; }
        public IAsyncRelayCommand SuporteCommand { get; }
        public IAsyncRelayCommand LogoutCommand { get; }
        public IAsyncRelayCommand VoltarCommand { get; }
        public IAsyncRelayCommand HomeCommand { get; }
        public IAsyncRelayCommand ChatCommand { get; }
        public IAsyncRelayCommand CalendarCommand { get; }

        // --- CONSTRUTOR ---
        public ProfileViewModel(UserDataService userDataService)
        {
            _userDataService = userDataService;

            // Inicialização Manual dos Comandos
            OnAppearingCommand = new RelayCommand(OnAppearing);
            AlterarDadosCommand = new AsyncRelayCommand(AlterarDados);
            NotificacoesCommand = new AsyncRelayCommand(Notificacoes);
            VerificarContaCommand = new AsyncRelayCommand(VerificarConta);
            SuporteCommand = new AsyncRelayCommand(Suporte);
            LogoutCommand = new AsyncRelayCommand(Logout);
            VoltarCommand = new AsyncRelayCommand(Voltar);
            HomeCommand = new AsyncRelayCommand(Home);
            ChatCommand = new AsyncRelayCommand(Chat);
            CalendarCommand = new AsyncRelayCommand(Calendar);

            // Carga inicial (Segurança)
            CarregarDados();
        }

        // --- LÓGICA ---

        private void OnAppearing()
        {
            CarregarDados();
        }

        private void CarregarDados()
        {
            NomeUsuario = _userDataService.NomeUsuario ?? "Usuário";

            if (!string.IsNullOrEmpty(_userDataService.FotoPerfil))
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(_userDataService.FotoPerfil);
                    FotoUsuario = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                    TemFoto = true;
                    NaoTemFoto = false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erro ao converter foto: {ex.Message}");
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

        private async Task AlterarDados() => await Shell.Current.GoToAsync(nameof(EditarPerfilPage));

        private async Task Notificacoes() => await Shell.Current.DisplayAlert("Notificações", "Sem novas notificações.", "OK");

        private async Task VerificarConta()
        {
            if (_userDataService.ContaVerificada)
                await Shell.Current.DisplayAlert("Verificado", "Sua conta já está verificada!", "OK");
            else
                await Shell.Current.GoToAsync(nameof(VerifyAccountPage));
        }

        private async Task Suporte() => await Shell.Current.DisplayAlert("Suporte", "Contato: suporte@gamir.com.br", "OK");

        private async Task Logout()
        {
            var popup = new LogoutPopup();
            var resultado = await Shell.Current.ShowPopupAsync(popup);

            if (resultado is bool confirmou && confirmou)
            {
                _userDataService.LimparDados();
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }

        private async Task Voltar() => await Shell.Current.GoToAsync("..");
        private async Task Home() => await Shell.Current.GoToAsync("//DashboardPage");
        private async Task Chat() { }
        private async Task Calendar() => await Shell.Current.GoToAsync(nameof(HistoricoPage));
    }
}