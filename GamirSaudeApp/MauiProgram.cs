using GamirSaudeApp.Handlers;
using GamirSaudeApp.Services;
using GamirSaudeApp.ViewModels;
using GamirSaudeApp.Views;
using CommunityToolkit.Maui;

namespace GamirSaudeApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ----- INÍCIO DO NOSSO CÓDIGO -----

        CustomHandlers.Apply();

        // Registro de Serviços
        builder.Services.AddSingleton<GamirApiService>();
        builder.Services.AddSingleton<UserDataService>(); // <-- Posição correta

        // Registro dos ViewModels
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<DashboardViewModel>();
        builder.Services.AddTransient<AgendarConsultaViewModel>();
        builder.Services.AddTransient<MedicosDisponiveisViewModel>();
        builder.Services.AddTransient<SucessoAgendamentoViewModel>();
        builder.Services.AddTransient<AgendarExameViewModel>();
        builder.Services.AddTransient<ExamesEspecificosViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<VerifyAccountViewModel>();
        builder.Services.AddTransient<HistoricoViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<MedicosExameViewModel>();
        builder.Services.AddTransient<EsqueciSenhaViewModel>();
        builder.Services.AddTransient<RedefinirSenhaViewModel>();

        // Registro das Views (Páginas)
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<DashboardPage>();
        builder.Services.AddTransient<AgendarConsultaPage>();
        builder.Services.AddTransient<MedicosDisponiveisPage>();
        builder.Services.AddTransient<AgendamentoSucessoPage>();
        builder.Services.AddTransient<AgendarExamePage>();
        builder.Services.AddTransient<ExamesEspecificosPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<VerifyAccountPage>();
        builder.Services.AddTransient<HistoricoPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<MedicosExamePage>();
        builder.Services.AddTransient<EsqueciSenhaPage>();
        builder.Services.AddTransient<RedefinirSenhaPage>();
        // ----- FIM DO NOSSO CÓDIGO -----

        return builder.Build();
    }
}