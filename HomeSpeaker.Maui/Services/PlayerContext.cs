using HomeSpeaker.Maui.Models;

namespace HomeSpeaker.Maui.Services;

public class PlayerContext : IPlayerContext
{
    public List<MauiHomeSpeakerService> Services { get; private set; } = [];
    public IMauiHomeSpeakerService? CurrentService { get; private set; }
    public List<SongModel> Songs { get; private set; } = [];

    public async Task AddService(string serverAddress)
    {
        MauiHomeSpeakerService newService = new(serverAddress);
        Services.Add(newService);
        CurrentService = newService;
        await SyncSongs();
    }

    public async Task SetCurrentService(string serverAddress)
    {
        MauiHomeSpeakerService? service = Services.FirstOrDefault(s => s.ServerAddress == serverAddress);
        if (service is null)
        {
            throw new Exception("Service not found");
        }
        CurrentService = service;
        await SyncSongs();
    }

    private async Task SyncSongs()
    {
        if (CurrentService is null) return;
        Songs = (await CurrentService.GetAllSongsAsync()).ToList();
    }
}
