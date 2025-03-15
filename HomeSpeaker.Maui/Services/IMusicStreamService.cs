
namespace HomeSpeaker.Maui.Services
{
    public interface IMusicStreamService
    {
        Dictionary<string, string> Streams { get; }

        bool AddStream(string key, string value);
        bool RemoveStream(string key);
    }
}