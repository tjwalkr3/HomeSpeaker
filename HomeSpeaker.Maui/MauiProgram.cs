using Microsoft.Extensions.Logging;
using HomeSpeaker.Maui.Views;
using HomeSpeaker.Maui.ViewModels;
using HomeSpeaker.Maui.Services;

namespace HomeSpeaker.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .RegisterServices()
                .RegisterViewModels()
                .RegisterViews()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<StartPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ChangeMetadata>();
            builder.Services.AddTransient<PlaylistPage>();
            return builder;
        }

        private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<StartPageViewModel>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<ChangeMetadataViewModel>();
            builder.Services.AddTransient<PlaylistPageViewModel>();
            return builder;
        }

        private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IPlayerContext, PlayerContext>();
            return builder;
        }
    }
}
