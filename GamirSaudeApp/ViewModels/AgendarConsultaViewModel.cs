using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GamirSaudeApp.ViewModels
{
    public partial class AgendarConsultaViewModel : BaseViewModel
    {
        // 1. DADOS (Mock)
        public ObservableCollection<string> Especialidades { get; } = new ObservableCollection<string>
        {
            "Cardiologia",
            "Dermatologia",
            "Ginecologia",
            "Ortopedia",
            "Pediatria",
            "Clínico Geral"
        };

        [ObservableProperty]
        private string especialidadeSelecionada;

        // --- NOVO COMANDO PARA CORRIGIR O ERRO ---
        [RelayCommand]
        private async Task PageAppearing()
        {
            // Por enquanto não faz nada, pois os dados estão fixos acima.
            // No futuro, aqui chamaremos: await _apiService.GetEspecialidades();
            await Task.CompletedTask;
        }

        // 2. COMANDOS PRINCIPAIS
        [RelayCommand]
        private async Task Voltar()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task VerProfissionais()
        {
            if (string.IsNullOrEmpty(EspecialidadeSelecionada))
            {
                await Shell.Current.DisplayAlert("Atenção", "Selecione uma especialidade primeiro.", "OK");
                return;
            }

            // NAVEGAÇÃO CORRIGIDA: Vai para a lista de cartões de médicos
            await Shell.Current.GoToAsync($"{nameof(MedicosListaPage)}?Especialidade={EspecialidadeSelecionada}");
        }

        [RelayCommand]
        private async Task FaleConosco()
        {
            await Shell.Current.DisplayAlert("Contato", "Abrindo WhatsApp do suporte...", "OK");
        }

        // 3. COMANDOS DA BARRA INFERIOR
        [RelayCommand]
        private async Task Home()
        {
            await Shell.Current.GoToAsync("//DashboardPage");
        }

        [RelayCommand]
        private async Task Chat() { /* Lógica futura */ }

        [RelayCommand]
        private async Task Profile() { /* Lógica futura */ }

        [RelayCommand]
        private async Task Calendar() { /* Lógica futura */ }
    }
}