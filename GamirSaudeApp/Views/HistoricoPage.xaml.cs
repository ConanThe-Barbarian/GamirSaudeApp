// GamirSaudeApp/Views/HistoricoPage.xaml.cs

using GamirSaudeApp.ViewModels;

namespace GamirSaudeApp.Views;

public partial class HistoricoPage : ContentPage
{
    public HistoricoPage()
    {
        InitializeComponent();
        BindingContext = new HistoricoViewModel();
    }
}