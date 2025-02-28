using HomeSpeaker.Maui.ViewModels;
using HomeSpeaker.Maui.Services;
using NSubstitute;
namespace HomeSpeaker.MauiTests;

public class StartPageViewModelTests
{
    [Fact]
    public void CreateViewModel()
    {
        // Arrange
        PlayerContext context = new();

        // Act
        var viewModel = new StartPageViewModel(context);

        // Assert
        Assert.NotNull(viewModel);
    }

    [Fact]
    public void TestEventSubscription()
    {
        // Arrange
        int eventFiredCount = 0;
        PlayerContext context = new();
        var viewModel = new StartPageViewModel(context);
        viewModel.BaseUrl = "";

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(viewModel.BaseUrl))
            {
                Assert.Equal("https://testURL.com:8085", viewModel.BaseUrl);
                eventFiredCount++;
            }
        };

        // Act
        viewModel.BaseUrl = "https://testURL.com:8085";

        // Assert
        Assert.Equal(1, eventFiredCount);
    }
}
