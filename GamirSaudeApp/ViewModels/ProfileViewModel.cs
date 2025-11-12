using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models; // Para UserProfile
using GamirSaudeApp.Services;
using System.Diagnostics;
using GamirSaudeApp.Views;

namespace GamirSaudeApp.ViewModels
{
    // GARANTIDO: Palavra-chave "partial"
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly GamirApiService _apiService;
        private readonly UserDataService _userDataService;

        // Propriedades observáveis para exibir na tela
        [ObservableProperty] private string nome;
        [ObservableProperty] private string cpf;
        [ObservableProperty] private string email;
        [ObservableProperty] private string telefone;
        [ObservableProperty] private string dataNascimento;
        [ObservableProperty] private string sexo;
        [ObservableProperty] private bool contaVerificada;
        [ObservableProperty] private string statusConta;

        public ProfileViewModel(GamirApiService apiService, UserDataService userDataService)
        {
            _apiService = apiService;
            _userDataService = userDataService;
            Debug.WriteLine("--- ProfileViewModel Construído ---");
        }

        // GARANTIDO: Atributo [RelayCommand]
        [RelayCommand]
        private async Task OnAppearing()
        {
            Debug.WriteLine("--- ProfileViewModel OnAppearing Executado ---");
            await LoadProfileData();
        }

        // --- COMANDO LOGOUT (MOVIDO PARA CÁ) ---
        [RelayCommand]
        private async Task Logout()
        {
            Debug.WriteLine("--- ProfileViewModel.LogoutCommand Executado ---");
            _userDataService.ClearData();
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        // --- COMANDO VERIFICAR CONTA (MOVIDO PARA CÁ) ---
        [RelayCommand]
        private async Task VerificarConta()
        {
            Debug.WriteLine("--- ProfileViewModel.VerificarContaCommand Executado ---");
            // Navega para a página de inserção do código
            await Shell.Current.GoToAsync(nameof(VerifyAccountPage));
        }

        private async Task LoadProfileData()
        {
            if (IsBusy) return;
            IsBusy = true;
            Debug.WriteLine($"Carregando perfil para IdUserApp: {_userDataService.IdUserApp}");

            if (_userDataService.IdUserApp <= 0)
            {
                Debug.WriteLine("ERRO: IdUserApp inválido no UserDataService.");
                await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível identificar o usuário.", "OK");
                IsBusy = false;
                return;
            }

            UserProfile profile = null;
            try
            {
                profile = await _apiService.GetUserProfileAsync(_userDataService.IdUserApp);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRO NA CHAMADA API GetUserProfile: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erro de Rede", "Não foi possível buscar os dados do perfil.", "OK");
            }


            if (profile != null)
            {
                Debug.WriteLine($"Perfil carregado: {profile.NomeUsuario}");
                // Atualiza as propriedades observáveis
                Nome = profile.NomeUsuario; // <-- CORRIGIDO
                Cpf = FormatCpf(profile.CpfUsuario);
                Email = profile.EmailUsuario; // <-- CORRIGIDO
                Telefone = FormatPhone(profile.TelefoneUsuario); DataNascimento = profile.DataNascimentoDisplay;
                Sexo = profile.SexoDisplay;
                ContaVerificada = profile.ContaVerificada;
                StatusConta = profile.ContaVerificada ? "Verificada" : "Não Verificada";
            }
            else
            {
                Debug.WriteLine("API retornou perfil NULO ou ocorreu erro.");
                // Limpar campos ou mostrar mensagem de erro adicional
                Nome = "Erro ao carregar";
                // ... limpar outros campos
            }

            IsBusy = false;
            Debug.WriteLine("--- Carregamento do Perfil Finalizado ---");
        }

        // --- MÉTODOS AUXILIARES DE FORMATAÇÃO ---
        private string FormatCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return "Não informado";
            // Remove non-digits just in case
            var digits = new string(cpf.Where(char.IsDigit).ToArray());
            if (digits.Length == 11)
            {
                return $"{digits.Substring(0, 3)}.{digits.Substring(3, 3)}.{digits.Substring(6, 3)}-{digits.Substring(9)}";
            }
            return cpf; // Retorna original se não tiver 11 dígitos
        }

        private string FormatPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return "Não informado";
            var digits = new string(phone.Where(char.IsDigit).ToArray());
            if (digits.Length == 11) // Celular (XX) XXXXX-XXXX
            {
                return $"({digits.Substring(0, 2)}) {digits.Substring(2, 5)}-{digits.Substring(7)}";
            }
            if (digits.Length == 10) // Fixo (XX) XXXX-XXXX
            {
                return $"({digits.Substring(0, 2)}) {digits.Substring(2, 4)}-{digits.Substring(6)}";
            }
            return phone; // Retorna original se não for 10 ou 11 dígitos
        }


    }
}