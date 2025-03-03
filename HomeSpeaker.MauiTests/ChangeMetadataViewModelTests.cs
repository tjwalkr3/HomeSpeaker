using HomeSpeaker.Maui.ViewModels;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Maui.Models;
using NSubstitute;
using HomeSpeaker.Shared;
namespace HomeSpeaker.MauiTests;

public class ChangeMetadataViewModelTests
{
    [Fact]
    public void TestCreateViewModel()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        context.Songs.Returns([]);
        var navigationService = Substitute.For<INavigationService>();

        // Act
        var viewModel = new ChangeMetadataViewModel(context, navigationService);

        // Assert
        Assert.NotNull(viewModel);
    }

    [Fact]
    public void TestLoadSongs()
    {
        // Arrange
        IPlayerContext context = Substitute.For<IPlayerContext>();
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
        var viewModel = new ChangeMetadataViewModel(context, navigationService);

        // Act
        viewModel.LoadSongs();

        // Assert
        Assert.Equal(2, viewModel.AllSongsList.Count);
        Assert.Equal("Song 1", viewModel.AllSongsList[0].Name);
        Assert.Equal("Song 2", viewModel.AllSongsList[1].Name);
    }

    [Fact]
    public async Task TestUpdateMetadata()
    {
        // Arrange
        IPlayerContext context = Substitute.For<IPlayerContext>();
        var song1 = new SongModel()
        {
            SongId = 1,
            Name = "Song 1",
            Path = "Path 1",
            Album = "Album 1",
            Artist = "Artist 1",
        };
        context.CurrentService.Returns(Substitute.For<IMauiHomeSpeakerService>());
        context.Songs.Returns(new List<SongModel> { song1 });
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new ChangeMetadataViewModel(context, navigationService);
        viewModel.SelectedSong = song1;
        viewModel.SongName = "New Song Name";
        viewModel.Artist = "New Artist";
        viewModel.Album = "New Album";

        // Act
        await viewModel.UpdateMetadataCommand.ExecuteAsync(null);

        // Assert
        await context.CurrentService.Received()!.UpdateMetadataAsync(Arg.Is<SongMessage>(x =>
            x.SongId == 1 &&
            x.Name == "New Song Name" &&
            x.Artist == "New Artist" &&
            x.Album == "New Album"
        ));
    }

    [Fact]
    public async Task TestNavigateToMainPage()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        context.Songs.Returns([]);
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new ChangeMetadataViewModel(context, navigationService);

        // Act
        await viewModel.NavigateToMainPageCommand.ExecuteAsync(null);

        // Assert
        await navigationService.Received()
            .GoToAsync(Arg.Is<string>(x => x == "///MainPage"));
    }
}
