using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System.Diagnostics;

namespace GamirSaudeApp.ViewModels
{
    [QueryProperty(nameof(Email), "Email")]
    public partial class RedefinirSenhaViewModel : BaseViewModel
    {
        private readonly GamirApiService _apiService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string codigo;

        [ObservableProperty]
        private string novaSenha;

        [ObservableProperty]
        private string confirmarNovaSenha;

        public RedefinirSenhaViewModel(GamirApiService apiService)
        {
            _apiService = apiService;
            Title = "Redefinir Senha";
        }

        [RelayCommand]
        private async Task RedefinirSenha()
        {
            if (string.IsNullOrWhiteSpace(Codigo) || string.IsNullOrWhiteSpace(NovaSenha) || string.IsNullOrWhiteSpace(ConfirmarNovaSenha))
            {
                await Shell.Current.DisplayAlert("Atenção", "Preencha todos os campos.", "OK");
                return;
            }

            // --- VALIDAÇÃO DE 6 CARACTERES ADICIONADA ---
            // Verifica o tamanho antes de qualquer outra coisa
            if (NovaSenha.Length < 6)
            {
                // Exibe a mensagem de erro específica que você solicitou
                await Shell.Current.DisplayAlert("Atenção", "A senha deve ter no mínimo 6 caracteres.", "OK");
                return; // Para a execução
            }
            // --- FIM DA VALIDAÇÃO ---

            if (NovaSenha != ConfirmarNovaSenha)
            {
                await Shell.Current.DisplayAlert("Atenção", "As senhas não conferem.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var (success, message) = await _apiService.RedefinirSenhaAsync(Email, Codigo, NovaSenha);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Sucesso", message, "OK");
                    // Envia o usuário de volta para a tela de Login
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", message, "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao redefinir senha: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Não foi possível conectar ao servidor.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}