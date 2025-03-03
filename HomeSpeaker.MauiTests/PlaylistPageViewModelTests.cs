using HomeSpeaker.Maui.ViewModels;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Shared;
using NSubstitute;
namespace HomeSpeaker.MauiTests;

public class PlaylistPageViewModelTest
{
    [Fact]
    public void TestCreateViewModel()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        context.Songs.Returns([]);
        var navigationService = Substitute.For<INavigationService>();

        // Act
        var viewModel = new PlaylistPageViewModel(context, navigationService);

        // Assert
        Assert.NotNull(viewModel);
    }

    [Fact]
    public async Task TestGetAllPlaylists()
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
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new PlaylistPageViewModel(context, navigationService);

        // Act
        await viewModel.GetAllPlaylistsCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(2, viewModel.Playlists.Count);
        Assert.Equal("Playlist 1", viewModel.Playlists[0].Name);
        Assert.Equal("Song 1", viewModel.Playlists[0].Songs.FirstOrDefault()!.Name);
        Assert.Equal("Playlist 2", viewModel.Playlists[1].Name);
        Assert.Equal("Song 2", viewModel.Playlists[1].Songs.FirstOrDefault()!.Name);
    }

    [Fact]
    public async Task TestPlayPlaylist()
    {
        // Arrange
        var mockService = Substitute.For<IMauiHomeSpeakerService>();
        var song1 = new Song()
        {
            SongId = 1,
            Name = "Song 1",
            Path = "Path 1",
            Album = "Album 1",
            Artist = "Artist 1",
        };
        var playlist = new Playlist("Playlist 1", [song1]);
        var context = Substitute.For<IPlayerContext>();
        context.CurrentService.Returns(mockService);
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new PlaylistPageViewModel(context, navigationService);

        // Act
        await viewModel.PlayPlaylistCommand.ExecuteAsync(playlist);

        // Assert
        await mockService.Received().PlayPlaylistAsync("Playlist 1");
    }

    [Fact]
    public void TestToggleExpand()
    {
        // Arrange
        var mockService = Substitute.For<IMauiHomeSpeakerService>();
        var playlist = new Playlist("Playlist 1", []);
        var context = Substitute.For<IPlayerContext>();
        context.CurrentService.Returns(mockService);
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new PlaylistPageViewModel(context, navigationService);

        // Act
        viewModel.ToggleExpandCommand.Execute(playlist);

        // Assert
        Assert.True(playlist.IsExpanded);
    }

    [Fact]
    public async Task TestNavigateToMainPage()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        context.Songs.Returns([]);
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new PlaylistPageViewModel(context, navigationService);

        // Act
        await viewModel.NavigateToMainPageCommand.ExecuteAsync(null);

        // Assert
        await navigationService.Received()
            .GoToAsync(Arg.Is<string>(x => x == "///MainPage"));
    }
}
