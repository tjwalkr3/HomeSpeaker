using Grpc.Net.Client;
using static HomeSpeaker.Shared.HomeSpeaker;

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


		builder.Services.AddTransient<SampleDataService>();
		builder.Services.AddTransient<ListDetailDetailViewModel>();
		builder.Services.AddTransient<ListDetailDetailPage>();
		AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
		builder.Services.AddScoped(services =>
		{
			var baseUri = new Uri(builder.Configuration["ServerAddress"] ?? "http://192.168.1.110");
			var httpHandler = new HttpClientHandler();
			// Return `true` to allow certificates that are untrusted/invalid
			httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

			//var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpHandler = httpHandler });
			var channel = GrpcChannel.ForAddress(baseUri);
			return new HomeSpeakerClient(channel);
		});
		builder.Services.AddSingleton(_ => new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "homespeaker_starredsongs.db3")));

		builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddSingleton<MainPage>();

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
