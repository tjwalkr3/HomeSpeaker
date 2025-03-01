﻿using HomeSpeaker.Maui.ViewModels;
using HomeSpeaker.Maui.Services;
using NSubstitute;
namespace HomeSpeaker.MauiTests;

public class ChangeMetadataViewModelTests
{
    [Fact]
    public void CreateViewModel()
    {
        // Arrange
        PlayerContext context = new();

        // Act
        var viewModel = new ChangeMetadataViewModel(context);

        // Assert
        Assert.NotNull(viewModel);
    }
}
