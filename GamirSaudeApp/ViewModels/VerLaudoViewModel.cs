using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Views;

namespace GamirSaudeApp.ViewModels
{
    [QueryProperty(nameof(NomeExame), "NomeExame")]
    [QueryProperty(nameof(NomeMedico), "NomeMedico")]
    [QueryProperty(nameof(Especialidade), "Especialidade")]
    [QueryProperty(nameof(DataExame), "Data")]
    public partial class VerLaudoViewModel : BaseViewModel
    {
        [ObservableProperty] string nomeExame;
        [ObservableProperty] string nomeMedico;
        [ObservableProperty] string especialidade;
        [ObservableProperty] string dataExame;

        [RelayCommand]
        private async Task Voltar() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        private async Task BaixarPdf()
        {
            // Lógica de download real entraria aqui
            await Shell.Current.DisplayAlert("Download", "Iniciando download do PDF...", "OK");
        }

        [RelayCommand]
        private async Task Compartilhar()
        {
            // Lógica de compartilhamento nativo
            await Shell.Current.DisplayAlert("Compartilhar", "Abrindo opções de compartilhamento...", "OK");
            // await Share.RequestAsync(new ShareTextRequest { Text = "Confira meu laudo...", Title = "Compartilhar Laudo" });
        }
    }
}
