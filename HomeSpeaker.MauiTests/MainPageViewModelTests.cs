using HomeSpeaker.Maui.ViewModels;
using HomeSpeaker.Maui.Services;
using NSubstitute;
namespace HomeSpeaker.MauiTests;

public class MainPageViewModelTest
{
    [Fact]
    public void CreateViewModel()
    {
        // Arrange
        PlayerContext context = new();

        // Act
        var viewModel = new MainPageViewModel(context);

        // Assert
        Assert.NotNull(viewModel);
    }
}
