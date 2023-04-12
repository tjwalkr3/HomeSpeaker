using System.Collections.Generic;

namespace HomeSpeaker.Shared;

public record Playlist(string Name, string Description, IEnumerable<Song> Songs);
