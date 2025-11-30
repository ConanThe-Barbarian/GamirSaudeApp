using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamirSaudeApp.Models;
using GamirSaudeApp.Services;
using System.Diagnostics;
using GamirSaudeApp.Views;

namespace GamirSaudeApp.ViewModels
{
    public partial class EditarPerfilViewModel : BaseViewModel
    {
        private readonly UserDataService _userDataService;
        private readonly GamirApiService _apiService;

        // Propriedades de Leitura
        [ObservableProperty] private string nome;
        [ObservableProperty] private string cpf;
        [ObservableProperty] private string sexo;
        [ObservableProperty] private string email;
        [ObservableProperty] private DateTime dataNascimento;

        // Propriedades Editáveis
        [ObservableProperty] private string celular;

        // Propriedades de Imagem
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FotoPerfilSource))]
        [NotifyPropertyChangedFor(nameof(TemFoto))]
        [NotifyPropertyChangedFor(nameof(NaoTemFoto))]
        private string fotoPerfil;

        public ImageSource FotoPerfilSource
        {
            get
            {
                if (string.IsNullOrEmpty(FotoPerfil)) return null;
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(FotoPerfil);
                    return ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
                catch { return null; }
            }
        }

        public bool TemFoto => !string.IsNullOrEmpty(FotoPerfil);
        public bool NaoTemFoto => string.IsNullOrEmpty(FotoPerfil);

        [ObservableProperty] private bool isBusy;

        public EditarPerfilViewModel(UserDataService userDataService, GamirApiService apiService)
        {
            _userDataService = userDataService;
            _apiService = apiService;

            // O carregamento agora será feito pelo comando PageAppearing
        }

        // --- COMANDO QUE FALTAVA (Corrigindo erro XFC0045) ---
        [RelayCommand]
        private async Task PageAppearing()
        {
            await CarregarDadosReais();
        }

        private async Task CarregarDadosReais()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                // 1. Carrega dados locais (Cache)
                Nome = _userDataService.NomeUsuario;
                Celular = _userDataService.TelefoneUsuario;
                FotoPerfil = _userDataService.FotoPerfil;

                // 2. Tenta atualizar via API
                int idUsuario = _userDataService.IdUserApp;
                if (idUsuario > 0)
                {
                    var perfil = await _apiService.GetUserProfileAsync(idUsuario);
                    if (perfil != null)
                    {
                        Nome = perfil.Nome;
                        Cpf = perfil.Cpf;
                        Email = perfil.Email;
                        Celular = perfil.Telefone;
                        Sexo = perfil.Sexo;
                        DataNascimento = perfil.DataNascimento ?? DateTime.MinValue;

                        // Atualiza visual e cofre
                        FotoPerfil = perfil.FotoPerfil;
                        _userDataService.FotoPerfil = perfil.FotoPerfil;
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task AlterarFoto()
        {
            string action = await Shell.Current.DisplayActionSheet("Alterar Foto", "Cancelar", null, "Tirar Foto", "Escolher da Galeria");
            if (action == "Tirar Foto") await TirarFotoAsync();
            else if (action == "Escolher da Galeria") await EscolherFotoAsync();
        }

        private async Task TirarFotoAsync()
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null) await ProcessarFoto(photo);
            }
        }

        private async Task EscolherFotoAsync()
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo != null) await ProcessarFoto(photo);
        }

        private async Task ProcessarFoto(FileResult photo)
        {
            IsBusy = true;
            try
            {
                using Stream stream = await photo.OpenReadAsync();
                using MemoryStream ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                byte[] bytes = ms.ToArray();

                // Limite de tamanho (2MB)
                if (bytes.Length > 2 * 1024 * 1024)
                {
                    await Shell.Current.DisplayAlert("Atenção", "Imagem muito grande. Escolha uma menor.", "OK");
                    return;
                }

                FotoPerfil = Convert.ToBase64String(bytes);
            }
            catch { await Shell.Current.DisplayAlert("Erro", "Falha ao processar imagem.", "OK"); }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task AtualizarPerfil()
        {
            if (string.IsNullOrWhiteSpace(Celular))
            {
                await Shell.Current.DisplayAlert("Erro", "Celular é obrigatório.", "OK");
                return;
            }

            IsBusy = true;
            var request = new UpdateProfileRequest
            {
                IdUserApp = _userDataService.IdUserApp,
                Telefone = this.Celular,
                FotoPerfil = this.FotoPerfil
            };

            bool sucesso = await _apiService.UpdateProfileAsync(request);
            IsBusy = false;

            if (sucesso)
            {
                // Atualiza o serviço global
                _userDataService.TelefoneUsuario = Celular;
                _userDataService.FotoPerfil = FotoPerfil;

                await Shell.Current.DisplayAlert("Sucesso", "Perfil atualizado!", "OK");
                await Voltar();
            }
            else
            {
                await Shell.Current.DisplayAlert("Erro", "Falha ao salvar.", "OK");
            }
        }

        [RelayCommand] private async Task Voltar() => await Shell.Current.GoToAsync(nameof(ProfilePage));
    }
}