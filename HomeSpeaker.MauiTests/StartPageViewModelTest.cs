using HomeSpeaker.Maui.ViewModels;
using HomeSpeaker.Maui.Services;
using NSubstitute;
namespace HomeSpeaker.MauiTests;

public class StartPageViewModelTests
{
    [Fact]
    public void TestCreateViewModel()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        context.Services.Returns([]);
        var navigationService = Substitute.For<INavigationService>();

        // Act
        var viewModel = new StartPageViewModel(context, navigationService);

        // Assert
        Assert.NotNull(viewModel);
    }

    [Fact]
    public void TestEventSubscription()
    {
        // Arrange
        int eventFiredCount = 0;
        PlayerContext context = new();
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new StartPageViewModel(context, navigationService);
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

    [Fact]
    public void TestNewServerValid()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        context.Services.Returns([]);
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new StartPageViewModel(context, navigationService);

        // Act
        viewModel.BaseUrl = "https://testURL.com:8085";

        // Assert
        Assert.True(viewModel.NewServerValid());
    }

    [Fact]
    public async Task TestAddServer()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        context.Services.Returns([]);
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new StartPageViewModel(context, navigationService);
        viewModel.BaseUrl = "https://testURL.com:8085";

        // Act
        await viewModel.AddNewServerCommand.ExecuteAsync(null);

        // Assert
        await context.Received().AddService(Arg.Is<string>(x => x == "https://testURL.com:8085"));
        Assert.Contains("https://testURL.com:8085", viewModel.Servers);
    }

    [Fact]
    public async Task StartControlling()
    {
        // Arrange
        var context = Substitute.For<IPlayerContext>();
        context.Services.Returns([]);
        var navigationService = Substitute.For<INavigationService>();
        var viewModel = new StartPageViewModel(context, navigationService);
        viewModel.BaseUrl = "https://testURL.com:8085";

        // Act
        await viewModel.AddNewServerCommand.ExecuteAsync(null);
        await viewModel.StartControlling("https://testURL.com:8085");

        // Assert
        await context.Received().SetCurrentService(Arg.Is<string>(x => x == "https://testURL.com:8085"));
        await navigationService.Received().GoToAsync(Arg.Is<string>(x => x == "///MainPage"));
    }
}
