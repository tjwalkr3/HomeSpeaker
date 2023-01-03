using Grpc.Core;
using HomeSpeaker.Shared;
using static HomeSpeaker.Shared.HomeSpeaker;

namespace HomeSpeaker.Maui.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly HomeSpeakerClient client;

    public MainViewModel(HomeSpeakerClient client)
    {
        this.client = client;
    }

    [RelayCommand]
    public async Task ViewModelLoading()
    {
        try
        {
            var getSongsReply = client.GetSongs(new GetSongsRequest { });
            await foreach (var reply in getSongsReply.ResponseStream.ReadAllAsync())
            {
                var newSongs = reply.Songs.Select(s => s.ToSong());
                foreach (var song in newSongs)
                {
                    songs.Add(song);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.ToString();
        }
    }

    [ObservableProperty]
    private string errorMessage;

    [ObservableProperty]
    private ObservableCollection<Song> songs = new();
}
