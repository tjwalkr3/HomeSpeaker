using HomeSpeaker.Maui.ViewModels;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Shared;
using NSubstitute;
namespace HomeSpeaker.MauiTests;

public class PlaylistPageViewModelTest
{
    [Fact]
    public void CreateViewModel()
    {
        // Arrange
        PlayerContext context = new();

        // Act
        var viewModel = new PlaylistPageViewModel(context);

        // Assert
        Assert.NotNull(viewModel);
    }

    [Fact]
    public async Task GetAllPlaylists_ShouldReturnCorrectPlaylists()
    {
        // Arrange
        var mockService = Substitute.For<IMauiHomeSpeakerService>();
        var song1 = new Song() {
            SongId = 1, 
            Name = "Song 1", 
            Path = "Path 1", 
            Album = "Album 1", 
            Artist = "Artist 1", 
        };
        var song2 = new Song()
        {
            SongId = 2,
            Name = "Song 2",
            Path = "Path 2",
            Album = "Album 2",
            Artist = "Artist 2",
        };
        var mockPlaylists = new List<Playlist>
        {
            new Playlist("Playlist 1", new List<Song> {song1}),
            new Playlist ("Playlist 2", new List <Song> {song2})
        };
        mockService.GetPlaylistsAsync().Returns(Task.FromResult((IEnumerable<Playlist>)mockPlaylists));

        var context = Substitute.For<IPlayerContext>();
        context.CurrentService.Returns(mockService);
        var viewModel = new PlaylistPageViewModel(context);

        // Act
        await viewModel.GetAllPlaylists();

        // Assert
        Assert.Equal(2, viewModel.Playlists.Count);
        Assert.Equal("Playlist 1", viewModel.Playlists[0].Name);
        Assert.Equal("Song 1", viewModel.Playlists[0].Songs.FirstOrDefault()!.Name);
        Assert.Equal("Playlist 2", viewModel.Playlists[1].Name);
        Assert.Equal("Song 2", viewModel.Playlists[1].Songs.FirstOrDefault()!.Name);
    }
}
