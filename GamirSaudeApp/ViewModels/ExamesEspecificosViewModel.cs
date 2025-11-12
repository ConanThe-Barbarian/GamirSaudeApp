using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GamirSaudeApp.ViewModels
{
    [QueryProperty(nameof(TipoExameNome), "TipoExameNome")]
    public partial class ExamesEspecificosViewModel : BaseViewModel
    {
        private readonly GamirApiService _gamirApiService;

        [ObservableProperty]
        private string tipoExameNome;

        [ObservableProperty]
        private ObservableCollection<Especialidade> examesEspecificos; // Reutilizando o modelo 'Especialidade'

        [ObservableProperty]
        private Especialidade exameSelecionado;

        [ObservableProperty]
        private string title;

        private int idProcedimentoSelecionado;

        public ExamesEspecificosViewModel(GamirApiService gamirApiService)
        {
            _gamirApiService = gamirApiService;
            ExamesEspecificos = new ObservableCollection<Especialidade>();
            Title = "Selecione o Exame";
        }

        [RelayCommand]
        private async Task PageAppearing()
        {
            if (ExamesEspecificos.Count > 0 || string.IsNullOrEmpty(TipoExameNome)) return;

            var lista = await _gamirApiService.GetExamesEspecificosAsync(TipoExameNome);
            if (lista != null)
            {
                foreach (var item in lista)
                {
                    ExamesEspecificos.Add(item);
                }
            }
        }

        partial void OnExameSelecionadoChanged(Especialidade value)
        {
            if (value != null)
            {
                idProcedimentoSelecionado = value.IdProcedimento;
            }
        }

        [RelayCommand]
        private async Task VerProfissionaisDeExame(Especialidade exame) // <-- 1. Aceita o parâmetro
        {
            if (exame == null) // <-- 2. Verifica o parâmetro recebido
            {
                await Shell.Current.DisplayAlert("Atenção", "Por favor, selecione um exame.", "OK");
                return;
            }

            // 3. Define as propriedades com base no parâmetro
            ExameSelecionado = exame;
            idProcedimentoSelecionado = exame.IdProcedimento;

            // NAVEGA PARA A NOVA PÁGINA DE MÉDICOS DE EXAME
            await Shell.Current.GoToAsync($"{nameof(MedicosExamePage)}?ExameNome={exame.Nome}&IdProcedimento={exame.IdProcedimento}");
        }
    }
}