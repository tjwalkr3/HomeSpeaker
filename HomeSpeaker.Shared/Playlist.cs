﻿using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HomeSpeaker.Shared;

public partial class Playlist : ObservableObject
{
    public string Name { get; }
    public IEnumerable<Song> Songs { get; }

    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    private bool _isExpanded2;

    public Playlist(string name, IEnumerable<Song> songs)
    {
        Name = name;
        Songs = songs;
    }
}

