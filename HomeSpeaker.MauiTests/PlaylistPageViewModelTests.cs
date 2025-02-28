using HomeSpeaker.Maui.ViewModels;
using HomeSpeaker.Maui.Services;
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
}
