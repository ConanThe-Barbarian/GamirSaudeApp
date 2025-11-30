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
        [ObservableProperty]
        private string tipoExameNome;

        public ObservableCollection<TipoExame> ExamesEspecificos { get; } = new();

        // Chamado automaticamente quando a propriedade TipoExameNome muda (ao navegar)
        partial void OnTipoExameNomeChanged(string value)
        {
            CarregarExamesMock(value);
        }

        private void CarregarExamesMock(string tipo)
        {
            ExamesEspecificos.Clear();

            // Simula dados diferentes dependendo do tipo
            if (tipo == "RAIO X")
            {
                ExamesEspecificos.Add(new TipoExame { Nome = "RX ANTEBRAÇO", PrazoResultado = "2 dias úteis", Valor = 150.00m });
                ExamesEspecificos.Add(new TipoExame { Nome = "RX TÓRAX", PrazoResultado = "1 dia útil", Valor = 120.00m });
                ExamesEspecificos.Add(new TipoExame { Nome = "RX COTOVELO", PrazoResultado = "2 dias úteis", Valor = 150.00m });
            }
            else if (tipo == "ECO/DOPPLER")
            {
                ExamesEspecificos.Add(new TipoExame { Nome = "ECOCARDIOGRAMA", PrazoResultado = "Na hora", Valor = 350.00m });
                ExamesEspecificos.Add(new TipoExame { Nome = "DOPPLER DE CARÓTIDAS", PrazoResultado = "Na hora", Valor = 400.00m });
            }
            else
            {
                // Genérico
                ExamesEspecificos.Add(new TipoExame { Nome = $"EXAME DE {tipo} 1", PrazoResultado = "3 dias úteis", Valor = 200.00m });
                ExamesEspecificos.Add(new TipoExame { Nome = $"EXAME DE {tipo} 2", PrazoResultado = "5 dias úteis", Valor = 500.00m });
            }
        }

        [RelayCommand]
        private async Task Voltar() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        private async Task SelecionarExame(TipoExame exame)
        {
            // Agora navega para a LISTA, passando também o valor
            await Shell.Current.GoToAsync($"{nameof(MedicosExameListaPage)}?ExameNome={exame.Nome}&IdProcedimento={exame.Id}&Valor={exame.ValorFormatado}");
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
