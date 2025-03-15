using System.Text.Json.Serialization;
namespace HomeSpeaker.Maui.Models;

public class StreamModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("url_resolved")]
    public string Url { get; set; } = string.Empty;
}