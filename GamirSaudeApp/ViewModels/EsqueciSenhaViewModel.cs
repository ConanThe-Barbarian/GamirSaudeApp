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
    public partial class EsqueciSenhaViewModel : BaseViewModel
    {
        private readonly GamirApiService _apiService;

        [ObservableProperty]
        private string email;

        public EsqueciSenhaViewModel(GamirApiService apiService)
        {
            _apiService = apiService;
            Title = "Recuperar Senha";
        }

        [RelayCommand]
        private async Task SolicitarCodigo()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await Shell.Current.DisplayAlert("Atenção", "Por favor, digite seu e-mail.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var (success, message) = await _apiService.EsqueciSenhaAsync(Email);

                // --- ALTERAÇÃO AQUI ---
                if (success)
                {
                    // E-mail ENCONTRADO. API enviou o código.
                    await Shell.Current.DisplayAlert("Verifique seu E-mail", message, "OK");

                    // Navega para a próxima etapa, passando o e-mail
                    await Shell.Current.GoToAsync($"{nameof(RedefinirSenhaPage)}?Email={Email}");
                }
                else
                {
                    // E-mail NÃO ENCONTRADO. API retornou erro.
                    await Shell.Current.DisplayAlert("Erro", message, "OK");
                    // Não navega
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao solicitar código: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Não foi possível conectar ao servidor.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}