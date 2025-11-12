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
        private readonly GamirApiService _gamirApiService;

        [ObservableProperty]
        private ObservableCollection<TipoExame> _tiposDeExame;

        [ObservableProperty]
        private TipoExame _tipoExameSelecionado;

        public AgendarExameViewModel(GamirApiService gamirApiService)
        {
            _gamirApiService = gamirApiService;
            TiposDeExame = new ObservableCollection<TipoExame>();
        }

        [RelayCommand]
        private async Task PageAppearing()
        {
            if (TiposDeExame.Any()) return; // Não carrega se já tiver dados

            var lista = await _gamirApiService.GetTiposExameAsync();
            if (lista != null)
            {
                foreach (var item in lista)
                {
                    TiposDeExame.Add(item);
                }
            }
        }

        // Comando para o botão "Ver Exames" que vamos criar na tela
        [RelayCommand]
        private async Task VerExamesEspecificos()
        {
            if (TipoExameSelecionado == null)
            {
                await Shell.Current.DisplayAlert("Atenção", "Por favor, selecione um tipo de exame.", "OK");
                return;
            }

            // Ação final: Navega para a próxima tela, passando o nome do tipo de exame como parâmetro
            await Shell.Current.GoToAsync($"{nameof(ExamesEspecificosPage)}?TipoExameNome={TipoExameSelecionado.Nome}");
        }
    }
}