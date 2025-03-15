using HomeSpeaker.Maui.ViewModels;
using HomeSpeaker.Maui.Services;
using NSubstitute;
using System.Security.Cryptography.X509Certificates;
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
        musicStreamService.Streams.Returns([]);

        // Act
        StreamPageViewModel viewModel = new(context, navigationService, musicStreamService);
        
        // Assert
        Assert.NotNull(viewModel);
    }

    [Fact]
    public void TestSearchStreams_NoPromptReturnsAll()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        var navigationService = Substitute.For<INavigationService>();
        var musicStreamService = Substitute.For<IMusicStreamService>();
        musicStreamService.Streams.Returns(
            new Dictionary<string, string> 
            { 
                { "KBAQ", "https://kbaq.streamguys1.com/kbaq_mp3_128" } 
            });

        // Act
        StreamPageViewModel viewModel = new(context, navigationService, musicStreamService);

        // Assert
        Assert.Single(viewModel.FilteredStreamsList);
        Assert.Equal("KBAQ", viewModel.FilteredStreamsList[0].Name);
        Assert.Equal("https://kbaq.streamguys1.com/kbaq_mp3_128", viewModel.FilteredStreamsList[0].Url);
    }

    [Fact]
    public void TestSearchStreams_PromptReturnsNothing()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        var navigationService = Substitute.For<INavigationService>();
        var musicStreamService = Substitute.For<IMusicStreamService>();
        musicStreamService.Streams.Returns(
            new Dictionary<string, string>
            {
                { "KBAQ", "https://kbaq.streamguys1.com/kbaq_mp3_128" }
            });

        // Act
        StreamPageViewModel viewModel = new(context, navigationService, musicStreamService);
        viewModel.SearchQuery = "12";

        // Assert
        Assert.Empty(viewModel.FilteredStreamsList);
    }

    [Fact]
    public void TestSearchStreams_PromptReturnsCorrectStream()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        var navigationService = Substitute.For<INavigationService>();
        var musicStreamService = Substitute.For<IMusicStreamService>();
        musicStreamService.Streams.Returns(
            new Dictionary<string, string>
            {
                { "KBAQ", "https://kbaq.streamguys1.com/kbaq_mp3_128" },
                { "Madrigals", "https://streams.calmradio.com:14228/stream" }
            });

        // Act
        StreamPageViewModel viewModel = new(context, navigationService, musicStreamService);
        viewModel.SearchQuery = "KB";

        // Assert
        Assert.Single(viewModel.FilteredStreamsList);
        Assert.Equal("KBAQ", viewModel.FilteredStreamsList[0].Name);
        Assert.Equal("https://kbaq.streamguys1.com/kbaq_mp3_128", viewModel.FilteredStreamsList[0].Url);
    }
}

