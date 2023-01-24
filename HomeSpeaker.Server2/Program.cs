using HomeSpeaker.Server;
using HomeSpeaker.Server.Data;
using HomeSpeaker.Server2.Services;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Exceptions;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);
//builder.Logging.ClearProviders();
//builder.Logging.AddJsonConsole();
//builder.Logging.AddDebug();
builder.Services.AddApplicationInsightsTelemetry();

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
try
{
    var ping = new System.Net.NetworkInformation.Ping();
    var response = ping.Send("http://jaeger");
    if (response.Status == System.Net.NetworkInformation.IPStatus.Success)
    {
        builder.Services.AddOpenTelemetry()
            .WithTracing(b =>
            {
                b.AddConsoleExporter()
                .AddAspNetCoreInstrumentation()
                .AddJaegerExporter(options => options.AgentHost = "jaeger");
            })
            .WithMetrics(b =>
            {

            })
            .StartWithHost();
    }
}
catch { }

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
        .WriteTo.Console()
        .Enrich.WithExceptionDetails()
        .WriteTo.Seq("http://localhost:5341");
});

builder.Services.AddGrpc();

builder.Services.AddSingleton<IDataStore, OnDiskDataStore>();
builder.Services.AddSingleton<IFileSource>(_ => new DefaultFileSource(builder.Configuration[ConfigKeys.MediaFolder]));
builder.Services.AddSingleton<ITagParser, DefaultTagParser>();
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    builder.Services.AddSingleton<IMusicPlayer, WindowsMusicPlayer>();
else
    builder.Services.AddSingleton<IMusicPlayer, LinuxSoxMusicPlayer>();
builder.Services.AddSingleton<Mp3Library>();
builder.Services.AddHostedService<LifecycleEvents>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<HomeSpeakerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
