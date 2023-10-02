using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", (int brightness, ILogger logger) =>
{
    logger.LogInformation("Setting brightness to {brightness}", brightness);
    Process.Start("sudo", $"bash -c \"echo {brightness} > /sys/class/backlight/10-0045/brightness");
});

app.Run();
