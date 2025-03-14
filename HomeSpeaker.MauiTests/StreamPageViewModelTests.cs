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
        context.Songs.Returns([]);
        var navigationService = Substitute.For<INavigationService>();

        // Act
        StreamPageViewModel viewModel = new(context, navigationService);
        
        // Assert
        Assert.NotNull(viewModel);
    }

    [Fact]
    public void TestGetStreams()
    {

    }
}

