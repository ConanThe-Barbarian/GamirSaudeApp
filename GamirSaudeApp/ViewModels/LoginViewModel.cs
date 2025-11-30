using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System.Threading.Tasks;

namespace GamirSaudeApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly GamirApiService _gamirApiService;
        private readonly UserDataService _userDataService;

        // 1. PROPRIEDADES VISUAIS
        [ObservableProperty]
        private string cpf;

        [ObservableProperty]
        private string senha;

        [ObservableProperty]
        private bool isPasswordHidden = true;

        [ObservableProperty]
        private string eyeIconSource = "eye_off_icon.png";

        // 2. CONSTRUTOR
        public LoginViewModel(GamirApiService gamirApiService, UserDataService userDataService)
        {
            _gamirApiService = gamirApiService;
            _userDataService = userDataService;
        }

        // 3. COMANDOS

        [RelayCommand]
        private void TogglePassword()
        {
            IsPasswordHidden = !IsPasswordHidden;
            EyeIconSource = IsPasswordHidden ? "eye_off_icon.png" : "eye_icon.png";
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Cpf) || string.IsNullOrWhiteSpace(Senha))
            {
                await Shell.Current.DisplayAlert("Atenção", "Por favor, preencha CPF e Senha.", "OK");
                return;
            }

            IsBusy = true;

            // Chama a API Real
            var response = await _gamirApiService.LoginAsync(Cpf, Senha);

            IsBusy = false;

            if (response != null)
            {
                // SUCESSO: Salva os dados na memória do App (Singleton)
                _userDataService.IdUserApp = response.Id;
                _userDataService.NomeUsuario = response.Nome;
                _userDataService.EmailUsuario = response.Email;
                _userDataService.ContaVerificada = response.ContaVerificada;
                _userDataService.CpfUsuario = Cpf;

                // --- CORREÇÃO CRÍTICA ---
                // Salva a foto recebida no login para as outras telas usarem
                _userDataService.FotoPerfil = response.FotoPerfil;

                // Navega para o Dashboard (Resetando a pilha para não voltar ao login)
                await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erro", "CPF ou Senha inválidos, ou erro de conexão.", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }

        [RelayCommand]
        private async Task ForgotPassword()
        {
            await Shell.Current.GoToAsync(nameof(EsqueciSenhaPage));
        }
    }
}