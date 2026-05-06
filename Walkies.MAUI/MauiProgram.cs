using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Walkies.MAUI.Services;
using Walkies.MAUI.ViewModels;
using Walkies.MAUI.Views;

namespace Walkies.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
#if ANDROID || IOS || MACCATALYST
            .UseMauiMaps()
#endif
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services
        builder.Services.AddHttpClient<ApiService>();
        builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<ApiService>();

        // ViewModels
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<LoginViewModel>();

        // Views
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<LoginPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}