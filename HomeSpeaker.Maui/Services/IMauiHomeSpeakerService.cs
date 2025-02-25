using HomeSpeaker.Maui.Models;
using HomeSpeaker.Shared;

namespace HomeSpeaker.Maui.Services
{
    public interface IMauiHomeSpeakerService
    {
        IEnumerable<SongMessage> Songs { get; }

        event EventHandler QueueChanged;
        event EventHandler<string>? StatusChanged;

        Task AddToPlaylistAsync(string playlistName, string songPath);
        Task ClearQueueAsync();
        Task EnqueueFolderAsync(SongGroup songs);
        Task EnqueueFolderAsync(string folder);
        Task EnqueueSongAsync(int songId);
        Task<IEnumerable<SongModel>> GetAllSongsAsync();
        Task<IEnumerable<string>> GetFolders();
        Task<IEnumerable<Playlist>> GetPlaylistsAsync();
        Task<IEnumerable<SongModel>> GetPlayQueueAsync();
        Task<Dictionary<string, List<SongModel>>> GetSongGroups();
        Task<IEnumerable<SongModel>> GetSongsInFolder(string folder);
        Task<GetStatusReply> GetStatusAsync();
        Task<int> GetVolumeAsync();
        Task PlayFolderAsync(string folder);
        Task PlayPlaylistAsync(string playlistName);
        Task PlaySongAsync(int songId);
        Task PlayStreamAsync(string streamUri);
        Task RemoveFromPlaylistAsync(string playlistName, string songPath);
        Task ResumePlayAsync();
        Task SetVolumeAsync(int volume0to100);
        Task ShuffleQueueAsync();
        Task SkipToNextAsync();
        Task StopPlayingAsync();
        Task ToggleBrightness();
        Task UpdateQueueAsync(List<SongModel> songs);
    }
}