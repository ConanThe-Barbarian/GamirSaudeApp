using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;
public partial class MedicosListaPage : ContentPage
{
    public MedicosListaPage() { InitializeComponent(); BindingContext = new MedicosListaViewModel(); }
}