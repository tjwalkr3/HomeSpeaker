using System.Collections.Generic;

namespace HomeSpeaker.Shared;

public record Playlist(string Name, IEnumerable<Song> Songs);
