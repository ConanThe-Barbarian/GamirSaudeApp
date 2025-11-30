using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;
public partial class VerLaudoPage : ContentPage
{
    public VerLaudoPage() { InitializeComponent(); BindingContext = new VerLaudoViewModel(); }
}