using GamirSaudeApp.Handlers;
using GamirSaudeApp.Services;
using GamirSaudeApp.ViewModels;
using GamirSaudeApp.Views;
using CommunityToolkit.Maui;
using GamirSaudeApp.Views.Popups;
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
        builder.Services.AddTransient<MedicoCalendarioViewModel>();
        builder.Services.AddTransient<SucessoAgendamentoViewModel>();
        builder.Services.AddTransient<AgendarExameViewModel>();
        builder.Services.AddTransient<ExamesEspecificosViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<VerifyAccountViewModel>();
        builder.Services.AddTransient<HistoricoViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<ExameCalendarioViewModel>();
        builder.Services.AddTransient<EsqueciSenhaViewModel>();
        builder.Services.AddTransient<RedefinirSenhaViewModel>();
        builder.Services.AddTransient<EditarPerfilViewModel>();
        builder.Services.AddTransient<MedicosListaViewModel>();
        builder.Services.AddTransient<MedicosExameListaViewModel>();
        builder.Services.AddTransient<MeusLaudosViewModel>();
        builder.Services.AddTransient<VerLaudoViewModel>();

        // Registro das Views (Páginas)
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<DashboardPage>();
        builder.Services.AddTransient<AgendarConsultaPage>();
        builder.Services.AddTransient<MedicoCalendarioPage>();
        builder.Services.AddTransient<AgendamentoSucessoPage>();
        builder.Services.AddTransient<AgendarExamePage>();
        builder.Services.AddTransient<ExamesEspecificosPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<VerifyAccountPage>();
        builder.Services.AddTransient<HistoricoPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<ExameCalendarioPage>();
        builder.Services.AddTransient<EsqueciSenhaPage>();
        builder.Services.AddTransient<RedefinirSenhaPage>();
        builder.Services.AddTransient<EditarPerfilPage>();
        builder.Services.AddTransient<MedicosListaPage>();
        builder.Services.AddTransient<MedicosExameListaPage>();
        builder.Services.AddTransient<MeusLaudosPage>();
        builder.Services.AddTransient<VerLaudoPage>();
        // ----- FIM DO NOSSO CÓDIGO -----

        return builder.Build();
    }
}