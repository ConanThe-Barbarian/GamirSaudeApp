using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaudeApp.ViewModels
{
    public partial class AgendarExameViewModel : BaseViewModel
    {
        // Mock dos Grupos de Exame
        public ObservableCollection<string> GruposExame { get; } = new ObservableCollection<string>
        {
            "RAIO X",
            "ULTRASSONOGRAFIA",
            "ECO/DOPPLER",
            "TOMOGRAFIA",
            "RESSONÂNCIA MAGNÉTICA"
        };

        [ObservableProperty]
        private string grupoSelecionado;

        [RelayCommand]
        private async Task Voltar() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        private async Task VerExamesDisponiveis()
        {
            if (string.IsNullOrEmpty(GrupoSelecionado))
            {
                await Shell.Current.DisplayAlert("Atenção", "Selecione um tipo de exame.", "OK");
                return;
            }

            // Navega para a lista detalhada, passando o nome do grupo
            await Shell.Current.GoToAsync($"{nameof(ExamesEspecificosPage)}?TipoExameNome={GrupoSelecionado}");
        }

        // --- BARRA INFERIOR ---
        [RelayCommand]
        private async Task Home() => await Shell.Current.GoToAsync("//DashboardPage");

        [RelayCommand]
        private async Task Chat() { /* ... */ }

        [RelayCommand]
        private async Task Profile() => await Shell.Current.GoToAsync(nameof(ProfilePage));

        [RelayCommand]
        private async Task Calendar() => await Shell.Current.GoToAsync(nameof(HistoricoPage));
    }
}