﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Shared;
using System.Collections.ObjectModel;
namespace HomeSpeaker.Maui.ViewModels;

public partial class StartPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _baseUrl = "https://localhost:7238";

    public ObservableCollection<string> Servers { get; } = [];

    private readonly IPlayerContext _context;
    private readonly INavigationService _navigationService;

    public StartPageViewModel(IPlayerContext playerContext, INavigationService navigationService)
    {
        _context = playerContext ?? throw new ArgumentNullException(nameof(playerContext));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        LoadServers();
    }

    private void LoadServers()
    {
        Servers.Clear();
        foreach (var server in _context.Services)
        {
            Servers.Add(server.ServerAddress);
        }
    }

    public bool NewServerValid()
    {
        Uri? uriResult;
        if (Uri.TryCreate(BaseUrl, UriKind.Absolute, out uriResult) && uriResult is not null &&
           (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnBaseUrlChanged()
    {
        AddNewServerCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(NewServerValid))]
    public async Task AddNewServer()
    {
        await _context.AddService(BaseUrl);
        Servers.Add(BaseUrl);
        StartControllingCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    public async Task StartControlling(string baseURL)
    {
        await _context.SetCurrentService(baseURL);
        await _navigationService.GoToAsync("///MainPage");
    }

    private void ResetValue()
    {
        BaseUrl = string.Empty;
    }
}
