using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using System.Diagnostics;

namespace GamirSaudeApp.ViewModels
{
    // O "QueryProperty" conecta o objeto enviado pelo Shell à propriedade 'Laudo' abaixo
    [QueryProperty(nameof(Laudo), "Laudo")]
    public partial class VerLaudoViewModel : BaseViewModel
    {
        [ObservableProperty]
        private LaudoModel laudo;

        // --- PROPRIEDADES VISUAIS (BINDING) ---
        [ObservableProperty] private string nomeExame;
        [ObservableProperty] private string nomeMedico;
        [ObservableProperty] private string especialidade;
        [ObservableProperty] private DateTime dataExame;
        [ObservableProperty] private string status;
        [ObservableProperty] private Color corStatus;
        [ObservableProperty] private string urlPdf;

        // Quando o objeto 'Laudo' chega, este método roda automaticamente
        partial void OnLaudoChanged(LaudoModel value)
        {
            if (value is null) return;

            // Mapeia os dados do modelo para a tela
            NomeExame = value.NomeExame;
            NomeMedico = value.NomeMedico;
            Especialidade = value.Especialidade;
            DataExame = value.DataExame;
            Status = value.Status;
            UrlPdf = value.UrlPdf;

            // Converte a cor hexadecimal (#0098DA) para Color do MAUI
            if (!string.IsNullOrEmpty(value.CorStatus))
            {
                try { CorStatus = Color.FromArgb(value.CorStatus); }
                catch { CorStatus = Colors.Gray; }
            }
        }

        // --- COMANDOS ---

        [RelayCommand]
        private async Task Voltar()
        {
            await Shell.Current.GoToAsync("//MeusLaudosPage");
        }

        [RelayCommand]
        private async Task BaixarPdf()
        {
            if (string.IsNullOrEmpty(UrlPdf))
            {
                await Shell.Current.DisplayAlert("Indisponível", "O arquivo PDF deste laudo ainda não foi gerado.", "OK");
                return;
            }

            try
            {
                // Abre o navegador/visualizador de PDF padrão do celular
                await Launcher.OpenAsync(new Uri(UrlPdf));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao abrir PDF: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Não foi possível abrir o arquivo.", "OK");
            }
        }

        [RelayCommand]
        private async Task Compartilhar()
        {
            if (string.IsNullOrEmpty(UrlPdf)) return;

            try
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Title = $"Laudo - {NomeExame}",
                    Text = $"Olá, segue o meu laudo de {NomeExame} realizado em {DataExame:dd/MM/yyyy}.\nLink: {UrlPdf}",
                    Uri = UrlPdf
                });
            }
            catch { /* Ignora cancelamento */ }
        }
    }
}