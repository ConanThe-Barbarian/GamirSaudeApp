using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaudeApp.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly GamirApiService _apiService;

        [ObservableProperty] string nome;
        [ObservableProperty] string cpf;
        [ObservableProperty] string email;
        [ObservableProperty] string telefone;
        [ObservableProperty] string senha;
        [ObservableProperty] string confirmarSenha;
        [ObservableProperty] DateTime dataNascimento = DateTime.Today;
        [ObservableProperty] string sexo;
        public ObservableCollection<string> SexoOptions { get; }
        public RegisterViewModel(GamirApiService apiService)
        {
            _apiService = apiService;
            SexoOptions = new ObservableCollection<string> { "Masculino", "Feminino" }; // <-- INICIALIZAR
        }

        [RelayCommand]
        private async Task Register()
        {
            if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Cpf) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
            {
                await Shell.Current.DisplayAlert("Atenção", "Por favor, preencha todos os campos obrigatórios.", "OK");
                return;
            }

            if (Senha != ConfirmarSenha)
            {
                await Shell.Current.DisplayAlert("Erro", "As senhas não coincidem.", "OK");
                return;
            }

            IsBusy = true;
            var request = new RegisterRequest
            {
                Nome = this.Nome,
                Cpf = this.Cpf, // TODO: Considerar remover máscara do CPF antes de enviar
                Email = this.Email,
                Senha = this.Senha,
                Telefone = new string(this.Telefone?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>()),
                DataNascimento = this.DataNascimento, // <-- ADICIONAR
                // Envia apenas a primeira letra (M ou F)
                Sexo = this.Sexo?.Substring(0, 1)    // <-- ADICIONAR
            };

            var (Success, Message) = await _apiService.RegisterAsync(request);

            if (Success)
            {
                await Shell.Current.DisplayAlert("Sucesso", Message, "OK");
                // Navega de volta para a tela de login
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Falha no Cadastro", Message, "OK");
            }
            IsBusy = false;
        }
    }
}
