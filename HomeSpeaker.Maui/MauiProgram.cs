namespace HomeSpeaker.Maui;

public static partial class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddTransient<SampleDataService>();
        builder.Services.AddTransient<ListDetailDetailViewModel>();
        builder.Services.AddTransient<ListDetailDetailPage>();

        builder.Services.AddSingleton<IPlayerService, GrpcPlayerService>();

        builder.Services.AddSingleton<GrpcClientProvider>();
        builder.Services.AddScoped(services =>
        {
            var provider = services.GetRequiredService<GrpcClientProvider>();
            return provider.Client;
        });

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
}
