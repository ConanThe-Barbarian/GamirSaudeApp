using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Services;
using GamirSaudeApp.Views;
using System.Threading.Tasks;

namespace GamirSaudeApp.ViewModels
{
    public partial class VerifyAccountViewModel : BaseViewModel
    {
        private readonly UserDataService _userDataService;
        private readonly GamirApiService _apiService;

        // --- PROPRIEDADES ---
        private string codigo;
        public string Codigo
        {
            get => codigo;
            set => SetProperty(ref codigo, value);
        }

        private string mensagemInfo;
        public string MensagemInfo
        {
            get => mensagemInfo;
            set => SetProperty(ref mensagemInfo, value);
        }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        // --- COMANDOS ---
        public IAsyncRelayCommand VerificarCodigoCommand { get; }
        public IAsyncRelayCommand ReenviarCodigoCommand { get; }
        public IAsyncRelayCommand VoltarCommand { get; }

        public VerifyAccountViewModel(UserDataService userDataService, GamirApiService apiService)
        {
            _userDataService = userDataService;
            _apiService = apiService;

            VerificarCodigoCommand = new AsyncRelayCommand(VerificarCodigo);
            ReenviarCodigoCommand = new AsyncRelayCommand(SolicitarCodigo); // Reenviar chama Solicitar
            VoltarCommand = new AsyncRelayCommand(Voltar);

            // Mensagem visual
            string cel = _userDataService.TelefoneUsuario ?? "seu número";
            MensagemInfo = $"Enviamos um código SMS para {cel}. Insira-o abaixo para validar.";

            // Opcional: Já dispara o SMS assim que abre a tela
            // _ = SolicitarCodigo();
        }

        private async Task SolicitarCodigo()
        {
            if (IsBusy) return;
            IsBusy = true;

            var cpf = _userDataService.CpfUsuario;
            var celular = _userDataService.TelefoneUsuario;

            // Validação de segurança
            if (string.IsNullOrEmpty(cpf) || string.IsNullOrEmpty(celular))
            {
                IsBusy = false;
                await Shell.Current.DisplayAlert("Erro", "Dados de contato não encontrados. Faça login novamente.", "OK");
                return;
            }

            var sucesso = await _apiService.SolicitarVerificacaoAsync(cpf, celular);

            IsBusy = false;

            if (sucesso)
                await Shell.Current.DisplayAlert("SMS Enviado", $"Código enviado para {celular}", "OK");
            else
                await Shell.Current.DisplayAlert("Erro", "Falha ao enviar SMS.", "OK");
        }

        private async Task VerificarCodigo()
        {
            if (string.IsNullOrWhiteSpace(Codigo))
            {
                await Shell.Current.DisplayAlert("Erro", "Digite o código de 6 dígitos.", "OK");
                return;
            }

            IsBusy = true;
            var cpf = _userDataService.CpfUsuario;
            var sucesso = await _apiService.ConfirmarVerificacaoAsync(cpf, Codigo);
            IsBusy = false;

            if (sucesso)
            {
                await Shell.Current.DisplayAlert("Parabéns!", "Conta verificada com sucesso.", "OK");
                _userDataService.ContaVerificada = true;
                await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erro", "Código inválido ou expirado.", "OK");
            }
        }

        private async Task Voltar() => await Shell.Current.GoToAsync("..");
    }
}