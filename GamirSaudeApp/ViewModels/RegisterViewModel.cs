using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models; // Para RegisterRequest
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System.Collections.ObjectModel;

namespace GamirSaudeApp.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly GamirApiService _gamirApiService;

        // 1. PROPRIEDADES
        [ObservableProperty] private string nome;
        [ObservableProperty] private string email;
        [ObservableProperty] private string cpf;
        [ObservableProperty] private string celular;
        [ObservableProperty] private string sexoSelecionado;
        [ObservableProperty] private DateTime dataNascimento = DateTime.Today;
        [ObservableProperty] private string senha;
        [ObservableProperty] private string confirmarSenha;

        public ObservableCollection<string> ListaSexos { get; } = new() { "Masculino", "Feminino", "Outro" };

        public RegisterViewModel(GamirApiService gamirApiService)
        {
            _gamirApiService = gamirApiService;
        }

        // 2. COMANDOS

        [RelayCommand]
        private async Task VoltarParaLogin()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Cadastrar()
        {
            // A. Validações Básicas
            if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Cpf) || string.IsNullOrWhiteSpace(Senha))
            {
                await Shell.Current.DisplayAlert("Erro", "Preencha os campos obrigatórios.", "OK");
                return;
            }

            if (Senha != ConfirmarSenha)
            {
                await Shell.Current.DisplayAlert("Erro", "As senhas não coincidem.", "OK");
                return;
            }

            IsBusy = true;

            // B. Preparação dos Dados (Limpeza)
            var cpfLimpo = Cpf.Replace(".", "").Replace("-", "").Trim();
            var celLimpo = Celular?.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim();
            var sexoSigla = SexoSelecionado?.Substring(0, 1); // Pega "M", "F" ou "O"

            var request = new RegisterRequest
            {
                Nome = this.Nome,
                Email = this.Email,
                Cpf = cpfLimpo,
                Telefone = celLimpo,
                DataNascimento = this.DataNascimento,
                Sexo = sexoSigla,
                Senha = this.Senha
            };

            // C. Chamada à API
            var (success, message) = await _gamirApiService.RegisterAsync(request);

            IsBusy = false;

            // D. Resultado
            if (success)
            {
                await Shell.Current.DisplayAlert("Sucesso", message, "OK");
                await VoltarParaLogin(); // Volta para o usuário fazer login
            }
            else
            {
                await Shell.Current.DisplayAlert("Erro", message, "OK");
            }
        }
    }
}