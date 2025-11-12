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
        private readonly GamirApiService _gamirApiService;

        [ObservableProperty]
        private ObservableCollection<Especialidade> especialidades;

        [ObservableProperty]
        private Especialidade especialidadeSelecionada;

        // NOVO: Propriedade para guardar o ID do procedimento assim que a especialidade é selecionada
        private int idProcedimentoSelecionado;

        public AgendarConsultaViewModel(GamirApiService gamirApiService)
        {
            _gamirApiService = gamirApiService;
            Especialidades = new ObservableCollection<Especialidade>();
        }

        public async Task InitializeAsync()
        {
            await PageAppearing();
        }

        [RelayCommand]
        private async Task PageAppearing()
        {
            if (Especialidades.Count > 0) return; // Não carrega se já tiver dados

            var lista = await _gamirApiService.GetEspecialidadesAsync();

            if (lista != null)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Especialidades.Clear();
                    foreach (var item in lista)
                    {
                        Especialidades.Add(item);
                    }
                });
            }
        }

        // NOVO MÉTODO PARCIAL: Captura o IdProcedimento da Especialidade Selecionada
        partial void OnEspecialidadeSelecionadaChanged(Especialidade value)
        {
            if (value != null)
            {
                // Guarda o ID para ser enviado na navegação
                idProcedimentoSelecionado = value.IdProcedimento;
            }
        }

        [RelayCommand]
        private async Task VerProfissionais()
        {
            if (EspecialidadeSelecionada == null)
            {
                await Shell.Current.DisplayAlert("Atenção", "Por favor, selecione uma especialidade.", "OK");
                return;
            }

            // CORREÇÃO AQUI: Navega, passando o NOME da especialidade E o ID do Procedimento como parâmetros
            await Shell.Current.GoToAsync($"{nameof(MedicosDisponiveisPage)}?EspecialidadeNome={EspecialidadeSelecionada.Nome}&IdProcedimento={idProcedimentoSelecionado}");
        }
    }
}