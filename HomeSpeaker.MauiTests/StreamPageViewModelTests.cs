using HomeSpeaker.Maui.ViewModels;
using HomeSpeaker.Maui.Services;
using NSubstitute;
using System.Security.Cryptography.X509Certificates;
using HomeSpeaker.Maui.Models;
namespace HomeSpeaker.MauiTests;


public class StreamPageViewModelTests
{
    [Fact]
    public void TestCreateViewModel()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        var navigationService = Substitute.For<INavigationService>();
        var musicStreamService = Substitute.For<IMusicStreamService>();

        // Act
        StreamPageViewModel viewModel = new(context, navigationService, musicStreamService);
        
        // Assert
        Assert.NotNull(viewModel);
    }

    [Fact]
    public async Task TestSearchStreams_NoPromptReturnsNothing()
    {
        // Arrange
        List<StreamModel> listToReturn = [
            new() { Name = "KBAQ", Url = "https://kbaq.streamguys1.com/kbaq_mp3_128" }
        ];
        var context = Substitute.For<IPlayerContext>();
        var navigationService = Substitute.For<INavigationService>();
        var musicStreamService = Substitute.For<IMusicStreamService>();
        musicStreamService.Search(Arg.Any<string>()).Returns(Task.FromResult(listToReturn));

        // Act
        StreamPageViewModel viewModel = new(context, navigationService, musicStreamService);
        viewModel.SearchQuery = "";
        await viewModel.SearchStreamsCommand.ExecuteAsync(null);

        // Assert
        Assert.Empty(viewModel.AllStreamsList);
    }

    [Fact]
    public async Task TestSearchStreams_PromptReturnsMultipleStreams()
    {
        // Arrange
        List<StreamModel> listToReturn = [
            new() { Name = "KBAQ", Url = "https://kbaq.streamguys1.com/kbaq_mp3_128" },
            new() { Name = "Madrigals", Url = "https://streams.calmradio.com:14228/stream" }
        ];
        var context = Substitute.For<IPlayerContext>();
        var navigationService = Substitute.For<INavigationService>();
        var musicStreamService = Substitute.For<IMusicStreamService>();
        musicStreamService.Search(Arg.Any<string>()).Returns(Task.FromResult(listToReturn));

        // Act
        StreamPageViewModel viewModel = new(context, navigationService, musicStreamService);
        viewModel.SearchQuery = "KB";
        await viewModel.SearchStreamsCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(2, viewModel.AllStreamsList.Count);
        Assert.Equal("KBAQ", viewModel.AllStreamsList[0].Name);
        Assert.Equal("https://kbaq.streamguys1.com/kbaq_mp3_128", viewModel.AllStreamsList[0].Url);
        Assert.Equal("Madrigals", viewModel.AllStreamsList[1].Name);
        Assert.Equal("https://streams.calmradio.com:14228/stream", viewModel.AllStreamsList[1].Url);
    }
}

