using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HomeSpeaker.Shared;

public partial class Playlist : ObservableObject
{
    public string Name { get; }
    public ObservableCollection<Song> Songs { get; }

    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    private bool _isExpanded2;

    public void AddSong(Song song)
    {
        Songs.Add(song);
    }

    public void RemoveSong(Song song)
    {
        Songs.Remove(song);
    }

    public Playlist(string name, IEnumerable<Song> songs)
    {
        Name = name;
        Songs = new ObservableCollection<Song>(songs);
    }
}

