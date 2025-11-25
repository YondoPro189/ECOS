using Microsoft.Extensions.Logging;
using ECOSApp.Mobile.Services;

namespace ECOSApp.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registrar servicios
            builder.Services.AddSingleton<ApiService>();

            // Registrar páginas
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<EquipoDetallePage>();
            builder.Services.AddTransient<VotacionPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}