using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Views; // Incluímos a View, mas usaremos a rota absoluta
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace GamirSaudeApp.ViewModels
{
    // A página de sucesso é simples, herda de BaseViewModel para consistência
    public partial class SucessoAgendamentoViewModel : BaseViewModel
    {
        public SucessoAgendamentoViewModel()
        {
        }

        // Em ViewModels/AgendamentoSucessoViewModel.cs

        [RelayCommand]
        private async Task Voltar()
        {
            // Tentativa A: Rota absoluta para a primeira aba (assumindo que sua dashboard é a raiz)
            await Shell.Current.GoToAsync("//DashboardPage");

            // Se o problema persistir, use a navegação para voltar para a página anterior (mais seguro)
            // await Shell.Current.GoToAsync(".."); 
        }
    }
}