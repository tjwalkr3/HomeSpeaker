using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => "Brightness API.");

app.MapGet("/get", () => File.ReadAllText("/sys/class/backlight/10-0045/brightness"));

app.MapGet("/set", (int brightness, ILogger<Program> logger) =>
{
    if(brightness < 0 || brightness > 255)
    {
        logger.LogWarning("Brightness of {brightness} is outside of 0-255 range.", brightness);
        return "Brightness must be 0-255";
    }

    logger.LogInformation("Setting brightness to {brightness}", brightness);
    Process.Start("sudo", $"bash -c \"echo {brightness} > /sys/class/backlight/10-0045/brightness");
    return $"Set brightness to {brightness}";
});

app.Run();

public partial class Program { }