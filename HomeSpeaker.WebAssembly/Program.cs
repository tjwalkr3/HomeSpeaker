using HomeSpeaker.WebAssembly;
using HomeSpeaker.WebAssembly.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Fast.Components.FluentUI;
using MudBlazor.Services;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<HomeSpeakerService>();
builder.Services.AddFluentUIComponents();
builder.Services.AddMudServices();

try
{
    Console.WriteLine($"Trying to setup otel tracing @ {builder.Configuration["OtlpExporter"]}");
    builder.Services.AddOpenTelemetry()
        .WithTracing(b =>
        {
            b.SetResourceBuilder(
                ResourceBuilder.CreateDefault().AddService("HomeSpeaker.WebAssembly"))
            .AddSource("BlazorUI")
            //.AddOtlpExporter(opts => opts.Endpoint = new Uri(builder.Configuration["OtlpExporter"]));
            .AddZipkinExporter(o =>
            {
                o.Endpoint = new Uri(builder.Configuration["OtlpExporter"]);
                o.ExportProcessorType = OpenTelemetry.ExportProcessorType.Simple;
            });
        })
        .WithMetrics(b =>
        {

        });
}
catch (Exception ex)
{
    Console.WriteLine("!!! Trouble contacting jaeger: " + ex.ToString());
}

await builder.Build().RunAsync();
