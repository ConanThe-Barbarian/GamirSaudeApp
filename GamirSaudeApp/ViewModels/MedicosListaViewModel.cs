using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Views;
using System.Collections.ObjectModel;

namespace GamirSaudeApp.ViewModels
{
    [QueryProperty(nameof(EspecialidadeNome), "Especialidade")]
    public partial class MedicosListaViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string especialidadeNome;

        // Reutilizamos a classe Medico, mas talvez precisemos garantir que ela tenha propriedade 'Valor'
        // Se Medico.cs não tiver Valor, podemos usar um DTO local ou adicionar lá.
        // Assumindo que vamos adicionar ou usar uma propriedade mockada aqui.
        public ObservableCollection<MedicoCard> MedicosEncontrados { get; } = new();

        partial void OnEspecialidadeNomeChanged(string value)
        {
            CarregarMedicosMock(value);
        }

        private void CarregarMedicosMock(string especialidade)
        {
            MedicosEncontrados.Clear();

            // MOCK: Dados simulados
            if (especialidade == "Cardiologia")
            {
                MedicosEncontrados.Add(new MedicoCard { Nome = "Dr. Oswaldo", Valor = 300.00m, Descricao = "Por ordem de chegada entre os agendados" });
                MedicosEncontrados.Add(new MedicoCard { Nome = "Dra. Ana Paula", Valor = 350.00m, Descricao = "Hora marcada" });
            }
            else
            {
                MedicosEncontrados.Add(new MedicoCard { Nome = "Dr. Genérico 1", Valor = 250.00m, Descricao = "Por ordem de chegada entre os agendados" });
                MedicosEncontrados.Add(new MedicoCard { Nome = "Dra. Genérica 2", Valor = 280.00m, Descricao = "Por ordem de chegada entre os agendados" });
            }
        }

        [RelayCommand]
        private async Task Voltar() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        private async Task SelecionarMedico(MedicoCard medico)
        {
            // Navega para a tela de Calendário (renomeada)
            // Passamos o nome e o valor formatado
            await Shell.Current.GoToAsync($"{nameof(MedicoCalendarioPage)}?MedicoNome={medico.Nome}&Valor={medico.ValorFormatado}&Especialidade={EspecialidadeNome}");
        }

        // Nav Bar
        [RelayCommand] private async Task Home() => await Shell.Current.GoToAsync("//DashboardPage");
        [RelayCommand] private async Task Chat() { }
        [RelayCommand] private async Task Profile() => await Shell.Current.GoToAsync(nameof(ProfilePage));
        [RelayCommand] private async Task Calendar() => await Shell.Current.GoToAsync(nameof(HistoricoPage));
    }

    // Classe auxiliar para o Card (pode mover para Models se preferir)
    public class MedicoCard
    {
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public string ValorFormatado => Valor.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
    }
}