using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => "Brightness API.");

app.MapGet("/set", (int brightness, ILogger<Program> logger) =>
{
    logger.LogInformation("Setting brightness to {brightness}", brightness);
    Process.Start("sudo", $"bash -c \"echo {brightness} > /sys/class/backlight/10-0045/brightness");
});

app.Run();

public partial class Program { }