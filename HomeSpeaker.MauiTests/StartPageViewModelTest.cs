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
}
