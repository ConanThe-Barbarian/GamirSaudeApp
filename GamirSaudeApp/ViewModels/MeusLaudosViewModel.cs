using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GamirSaudeApp.ViewModels
{
    public partial class MeusLaudosViewModel : BaseViewModel
    {
        private readonly GamirApiService _apiService;
        private readonly UserDataService _userDataService;
        private List<LaudoModel> _todosLaudosCache = new();

        // LISTA VINCULADA AO XAML (O nome deve ser exato!)
        public ObservableCollection<LaudoModel> LaudosExibidos { get; } = new();

        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private bool isEmpty;
        [ObservableProperty] private DateTime dataInicio = DateTime.Today.AddMonths(-3);
        [ObservableProperty] private DateTime dataFim = DateTime.Today;

        public MeusLaudosViewModel(GamirApiService apiService, UserDataService userDataService)
        {
            _apiService = apiService;
            _userDataService = userDataService;
            _ = InicializarDados();
        }

        private async Task InicializarDados()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                // ID fixo para teste (garante que dados venham se a API estiver rodando)
                int idPaciente = 88922;

                var resultado = await _apiService.GetMeusLaudosAsync(idPaciente);

                _todosLaudosCache = resultado?.ToList() ?? new List<LaudoModel>();

                AplicarFiltro();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro: {ex.Message}");
                // Opcional: Mostrar alerta se falhar
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task FiltrarLaudos()
        {
            IsBusy = true;
            await Task.Delay(200);
            AplicarFiltro();
            IsBusy = false;
        }

        private void AplicarFiltro()
        {
            LaudosExibidos.Clear();
            var dataFimAjustada = DataFim.Date.AddDays(1).AddTicks(-1);

            var filtrados = _todosLaudosCache
                .Where(l => l.DataExame >= DataInicio.Date && l.DataExame <= dataFimAjustada)
                .OrderByDescending(l => l.DataExame).ToList();

            foreach (var item in filtrados) LaudosExibidos.Add(item);

            // Define se mostra a mensagem de "Nenhum laudo"
            IsEmpty = !LaudosExibidos.Any();
        }

        [RelayCommand]
        private async Task AbrirLaudo(LaudoModel laudo)
        {
            if (laudo == null) return;
            var navParam = new Dictionary<string, object> { { "Laudo", laudo } };
            // Certifique-se que VerLaudoPage está registrada no AppShell
            await Shell.Current.GoToAsync(nameof(VerLaudoPage), navParam);
        }

        // COMANDOS DA BARRA INFERIOR
        [RelayCommand] private async Task Voltar() => await Shell.Current.GoToAsync("..");
        [RelayCommand] private async Task Home() => await Shell.Current.GoToAsync("//DashboardPage");
        [RelayCommand] private async Task Chat() => await Shell.Current.DisplayAlert("Em breve", "Chat indisponível.", "OK");
        [RelayCommand] private async Task Profile() => await Shell.Current.GoToAsync(nameof(ProfilePage));
        [RelayCommand] private async Task Calendar() => await Shell.Current.GoToAsync(nameof(HistoricoPage));
    }
}