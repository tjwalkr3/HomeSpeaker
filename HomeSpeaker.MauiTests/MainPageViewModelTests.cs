using HomeSpeaker.Maui.ViewModels;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Maui.Models;
using NSubstitute;
using HomeSpeaker.Shared;
namespace HomeSpeaker.MauiTests;

public class MainPageViewModelTest
{
    [Fact]
    public void TestCreateViewModel()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        context.Songs.Returns([]);
        var navigationService = Substitute.For<INavigationService>();

        // Act
        var viewModel = new MainPageViewModel(context, navigationService);

        // Assert
        Assert.NotNull(viewModel);
    }

    [Fact]
    public async Task TestPlaySong()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        context.Songs.Returns([]);
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new MainPageViewModel(context, navigationService);

        // Act
        await viewModel.PlaySongCommand.ExecuteAsync(1);

        // Assert
        await context.Received().CurrentService!.PlaySongAsync(1);
    }

    [Fact]
    public async Task TestNavigateToStart()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        context.Songs.Returns([]);
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new MainPageViewModel(context, navigationService);

        // Act
        await viewModel.NavigateToStartCommand.ExecuteAsync(null);

        // Assert
        await navigationService.Received()
            .GoToAsync(Arg.Is<string>(x => x == "///StartPage"));
    }

    [Fact]
    public async Task TestNavigateToEditor()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        context.Songs.Returns([]);
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new MainPageViewModel(context, navigationService);

        // Act
        await viewModel.NavigateToEditorCommand.ExecuteAsync(null);

        // Assert
        await navigationService.Received()
            .GoToAsync(Arg.Is<string>(x => x == "///ChangeMetadata"));
    }

    [Fact]
    public async Task TestNavigateToPlaylist()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        context.Songs.Returns([]);
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new MainPageViewModel(context, navigationService);

        // Act
        await viewModel.NavigateToPlaylistCommand.ExecuteAsync(null);

        // Assert
        await navigationService.Received()
            .GoToAsync(Arg.Is<string>(x => x == "///PlaylistPage"));
    }

    [Fact]
    public void TestLoadSongs()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        var song1 = new SongModel()
        {
            SongId = 1,
            Name = "Song 1",
            Path = "Path 1",
            Album = "Album 1",
            Artist = "Artist 1",
        };
        var song2 = new SongModel()
        {
            SongId = 2,
            Name = "Song 2",
            Path = "Path 2",
            Album = "Album 2",
            Artist = "Artist 2",
        };
        context.Songs.Returns(new List<SongModel> { song1, song2 });
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new MainPageViewModel(context, navigationService);

        // Act
        viewModel.LoadSongs();

        // Assert
        Assert.Equal(2, viewModel.AllSongsList.Count);
        Assert.Equal("Song 1", viewModel.AllSongsList[0].Name);
        Assert.Equal("Song 2", viewModel.AllSongsList[1].Name);
    }
}
