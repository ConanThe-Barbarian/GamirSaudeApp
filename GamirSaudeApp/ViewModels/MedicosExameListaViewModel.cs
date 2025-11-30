using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Maui.Controls; // Necessário para QueryProperty e Shell

namespace GamirSaudeApp.ViewModels
{
    // QueryProperties conectam a URL de navegação às propriedades desta classe
    [QueryProperty(nameof(ExameNome), "ExameNome")]
    [QueryProperty(nameof(IdProcedimento), "IdProcedimento")]
    [QueryProperty(nameof(ValorExame), "Valor")]
    public partial class MedicosExameListaViewModel : BaseViewModel
    {
        // =========================================================================
        // PROPRIEDADES
        // =========================================================================

        // 1. Exame Nome (Com gatilho para carregar a lista)
        private string exameNome;
        public string ExameNome
        {
            get => exameNome;
            set
            {
                if (SetProperty(ref exameNome, value))
                {
                    // Assim que o nome chega, carregamos os médicos
                    CarregarMedicosMock();
                }
            }
        }

        // 2. ID do Procedimento
        private int idProcedimento;
        public int IdProcedimento
        {
            get => idProcedimento;
            set => SetProperty(ref idProcedimento, value);
        }

        // 3. Valor do Exame
        private string valorExame;
        public string ValorExame
        {
            get => valorExame;
            set => SetProperty(ref valorExame, value);
        }

        // 4. Lista de Médicos (Visualização)
        public ObservableCollection<MedicoExameCard> MedicosEncontrados { get; } = new();


        // =========================================================================
        // MÉTODOS
        // =========================================================================

        public MedicosExameListaViewModel()
        {
            // Construtor vazio
        }

        private void CarregarMedicosMock()
        {
            if (string.IsNullOrEmpty(ExameNome)) return;

            MedicosEncontrados.Clear();

            // Dados de Teste (Simulando a API)
            MedicosEncontrados.Add(new MedicoExameCard { Nome = "Clínica Radiológica Centro", Local = "Centro - Rio de Janeiro", Disponibilidade = "A partir de amanhã" });
            MedicosEncontrados.Add(new MedicoExameCard { Nome = "Dr. Especialista Imagem", Local = "Copacabana", Disponibilidade = "Próxima semana" });
            MedicosEncontrados.Add(new MedicoExameCard { Nome = "Hospital Gamir (Unidade 1)", Local = "Barra da Tijuca", Disponibilidade = "Hoje" });

            Debug.WriteLine($"Médicos mock carregados: {MedicosEncontrados.Count}");
        }


        // =========================================================================
        // COMANDOS
        // =========================================================================

        [RelayCommand]
        private async Task Voltar()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task SelecionarMedico(MedicoExameCard medico)
        {
            if (medico == null) return;

            // Monta a URL de navegação com todos os parâmetros necessários
            // As propriedades ExameNome, ValorExame e IdProcedimento são usadas aqui
            string rota = $"{nameof(ExameCalendarioPage)}?MedicoNome={medico.Nome}&Valor={ValorExame}&ExameNome={ExameNome}&IdProcedimento={IdProcedimento}";

            await Shell.Current.GoToAsync(rota);
        }

        // --- Barra Inferior ---
        [RelayCommand] private async Task Home() => await Shell.Current.GoToAsync("//DashboardPage");
        [RelayCommand] private async Task Chat() { /* Futuro */ }
        [RelayCommand] private async Task Profile() => await Shell.Current.GoToAsync(nameof(ProfilePage));
        [RelayCommand] private async Task Calendar() => await Shell.Current.GoToAsync(nameof(HistoricoPage));
    }

    // Classe de Modelo auxiliar para o Card
    public class MedicoExameCard
    {
        public string Nome { get; set; }
        public string Local { get; set; }
        public string Disponibilidade { get; set; }
    }
}