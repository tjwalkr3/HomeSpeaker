using MauiInsights;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace HomeSpeaker.Maui;

public static partial class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .AddApplicationInsights("InstrumentationKey=070db546-6b00-4f97-8b9e-5a4b4d4e97bf;IngestionEndpoint=https://westus2-2.in.applicationinsights.azure.com/;LiveEndpoint=https://westus2.livediagnostics.monitor.azure.com/")
            .AddCrashLogging()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        SetupSerilog();
        builder.Logging.AddSerilog();

        builder.Services.AddTransient<SampleDataService>();
        builder.Services.AddTransient<ListDetailDetailViewModel>();
        builder.Services.AddTransient<ListDetailDetailPage>();

        builder.Services.AddSingleton<IPlayerService, GrpcPlayerService>();
        builder.Services.AddSingleton<HomeSpeakerClientProvider>();

        builder.Services.AddSingleton<IStaredSongDb, StaredSongDb>(_ =>
        {
            var path = Path.Combine(FileSystem.Current.AppDataDirectory, "homespeaker_starredsongs.db3");
            return new StaredSongDb(path);
        });

        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<SettingsPage>();

        builder.Services.AddSingleton<ListDetailViewModel>();
        builder.Services.AddSingleton<ListDetailPage>();

        builder.Services.AddSingleton<WebViewViewModel>();
        builder.Services.AddSingleton<WebViewPage>();

        builder.Services.AddSingleton<BlankViewModel>();
        builder.Services.AddSingleton<BlankPage>();

        builder.Services.AddSingleton<StarredViewModel>();
        builder.Services.AddSingleton<StarredPage>();

        builder.Services.AddSingleton<FoldersViewModel>();
        builder.Services.AddSingleton<FoldersPage>();

        builder.Services.AddSingleton<StatusViewModel>();
        builder.Services.AddSingleton<StatusPage>();

        builder.Services.AddSingleton<StreamViewModel>();
        builder.Services.AddSingleton<StreamPage>();

        return builder.Build();
    }

    private static void SetupSerilog()
    {
        var flushInterval = new TimeSpan(0, 0, 1);
        var file = Path.Combine(FileSystem.AppDataDirectory, "MyApp.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .WriteTo.File(file, flushToDiskInterval: flushInterval, encoding: System.Text.Encoding.UTF8, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 22)
            .WriteTo.Seq("http://10.0.2.2:5341")
            .CreateLogger();
    }
}
