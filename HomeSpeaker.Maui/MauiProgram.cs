namespace HomeSpeaker.Maui;

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

        builder.Services.AddSingleton<MainViewModel>();

        builder.Services.AddSingleton<MainPage>();

		builder.Services.AddTransient<SampleDataService>();
		builder.Services.AddTransient<ListDetailDetailViewModel>();
		builder.Services.AddTransient<ListDetailDetailPage>();

        builder.Services.AddSingleton<ListDetailViewModel>();

        builder.Services.AddSingleton<ListDetailPage>();

        builder.Services.AddSingleton<WebViewViewModel>();

        builder.Services.AddSingleton<WebViewPage>();

        builder.Services.AddSingleton<BlankViewModel>();

        builder.Services.AddSingleton<BlankPage>();

		return builder.Build();
	}
}
