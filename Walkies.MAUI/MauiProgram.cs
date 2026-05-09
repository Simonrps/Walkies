using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
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
            .UseSkiaSharp()
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

        // ViewModels
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<DogViewModel>();
        builder.Services.AddTransient<OwnerDashboardViewModel>();
        builder.Services.AddTransient<WalkerDashboardViewModel>();
        builder.Services.AddTransient<WalkRequestViewModel>();
        builder.Services.AddTransient<WalkerSearchViewModel>();
        builder.Services.AddTransient<OwnerSearchViewModel>();
        builder.Services.AddTransient<BookingViewModel>();
        builder.Services.AddTransient<AvailabilityViewModel>();

        // Views
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<WalkerProfilePage>();
        builder.Services.AddTransient<OwnerDogsPage>();
        builder.Services.AddTransient<OwnerAddDogPage>();
        builder.Services.AddTransient<OwnerDashboardPage>();
        builder.Services.AddTransient<WalkerDashboardPage>();
        builder.Services.AddTransient<OwnerProfilePage>();
        builder.Services.AddTransient<OwnerWalkRequestPage>();
        builder.Services.AddTransient<WalkerSearchRequestsPage>();
        builder.Services.AddTransient<OwnerSearchWalkersPage>();
        builder.Services.AddTransient<OwnerBookingsPage>();
        builder.Services.AddTransient<WalkerCheckInPage>();
        builder.Services.AddTransient<WalkerAvailabilityPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}