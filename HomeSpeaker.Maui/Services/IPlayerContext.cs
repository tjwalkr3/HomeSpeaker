﻿using HomeSpeaker.Maui.Models;

namespace HomeSpeaker.Maui.Services
{
    public interface IPlayerContext
    {
        IMauiHomeSpeakerService? CurrentService { get; }
        List<IMauiHomeSpeakerService> Services { get; }
        List<SongModel> Songs { get; }

        Task AddService(string serverAddress);
        Task SetCurrentService(string serverAddress);
    }
}