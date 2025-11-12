using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System.Threading.Tasks;
using System.Linq; // <-- CORREÇÃO 1: ADICIONADA A DIRETIVA ESSENCIAL
using System;     // Adicionado para Exception

namespace GamirSaudeApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly GamirApiService _apiService;
        private readonly UserDataService _userDataService;

        [ObservableProperty] private string cpf;
        [ObservableProperty] private string senha;

        public LoginViewModel(GamirApiService apiService, UserDataService userDataService)
        {
            _apiService = apiService;
            _userDataService = userDataService;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Cpf) || string.IsNullOrWhiteSpace(Senha))
            {
                await Shell.Current.DisplayAlert("Atenção", "Por favor, preencha o CPF e a senha.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                // <-- CORREÇÃO 2: UTILIZANDO O MÉTODO DE APOIO
                var cpfNumerico = GetCpfNumerico();
                var loginResponse = await _apiService.LoginAsync(cpfNumerico, Senha);

                if (loginResponse?.Usuario != null)
                {
                    _userDataService.IdUserApp = loginResponse.Usuario.IdUserApp;
                    _userDataService.NomeUsuario = loginResponse.Usuario.NomeUserApp;
                    _userDataService.EmailUsuario = loginResponse.Usuario.EmailUserApp;
                    _userDataService.ContaVerificada = loginResponse.Usuario.ContaVerificada;
                    _userDataService.IdPacienteGamir = loginResponse.Usuario.IdPacienteGamir;

                    await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro de Login", loginResponse?.Message ?? "CPF ou senha inválidos.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Falha de Comunicação", $"Ocorreu um erro: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToRegister()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }

        [RelayCommand]
        private async Task ForgotPassword()
        {
            // Navega para a nova página de recuperação
            await Shell.Current.GoToAsync(nameof(EsqueciSenhaPage));
        }

        private string GetCpfNumerico()
        {
            return new string(Cpf?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
        }
    }
}