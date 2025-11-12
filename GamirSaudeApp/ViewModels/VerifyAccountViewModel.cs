using System;
using System.Collections.Generic;
using System.Linq;
using GamirSaudeApp.Views;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Services;
using System.Threading.Tasks;

namespace GamirSaudeApp.ViewModels
{
    public partial class VerifyAccountViewModel : BaseViewModel
    {
        private readonly GamirApiService _apiService;
        private readonly UserDataService _userDataService;

        [ObservableProperty]
        private string verificationCode;

        public VerifyAccountViewModel(GamirApiService apiService, UserDataService userDataService)
        {
            _apiService = apiService;
            _userDataService = userDataService;
        }

        [RelayCommand]
        private async Task SendCode()
        {
            IsBusy = true;
            var (success, message) = await _apiService.SendVerificationCodeAsync(_userDataService.EmailUsuario);
            IsBusy = false;

            await Shell.Current.DisplayAlert(success ? "Sucesso" : "Atenção", message, "OK");
        }

        [RelayCommand]
        private async Task Verify()
        {
            if (string.IsNullOrWhiteSpace(VerificationCode) || VerificationCode.Length != 6)
            {
                await Shell.Current.DisplayAlert("Erro", "Por favor, insira um código válido de 6 dígitos.", "OK");
                return;
            }

            IsBusy = true;
            // CORREÇÃO: Captura os 3 valores retornados pelo serviço
            var (success, message, updatedUser) = await _apiService.VerifyAccountAsync(_userDataService.EmailUsuario, VerificationCode);
            IsBusy = false;

            await Shell.Current.DisplayAlert(success ? "Sucesso" : "Falha na Verificação", message, "OK");



            if (success && updatedUser != null) // Verifica se os dados do usuário vieram
            {
                // --- SINCRONIZAÇÃO COMPLETA DO UserDataService ---
                _userDataService.IdUserApp = updatedUser.IdUserApp;
                _userDataService.NomeUsuario = updatedUser.NomeUserApp;
                _userDataService.EmailUsuario = updatedUser.EmailUserApp;
                _userDataService.ContaVerificada = updatedUser.ContaVerificada; // Deve ser true
                _userDataService.IdPacienteGamir = updatedUser.IdPacienteGamir; // ATUALIZA O ID CRUCIAL

                // --- FIM DA SINCRONIZAÇÃO ---

                // Navega de volta para o Dashboard, forçando um refresh
                await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}?refresh=true");
            }
        }
    }
}