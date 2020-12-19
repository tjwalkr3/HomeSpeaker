using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using HomeSpeaker.Shared;

namespace HomeSpeaker.Server.Data
{

    public class OnDiskDataStore : IDataStore
    {
        const string filePath = "songs.json";
        public OnDiskDataStore()
        {
            if(File.Exists(filePath))
                songs = JsonSerializer.Deserialize<List<Song>>(File.ReadAllText(filePath));
            else 
                songs = new();
        }

        private List<Song> songs;

        public async Task AddOrUpdateAsync(Song song)
        {
            var existingSong = songs.FirstOrDefault(s => s.Path == song.Path);
            if (existingSong == null)
            {
                song.SongId = songs.Count;
                songs.Add(song);
                await serializeSongs();
            }
        }

        private async Task serializeSongs()
        {
            var jsonString = JsonSerializer.Serialize(songs);
            File.WriteAllText(filePath, jsonString);
        }

        public IEnumerable<Album> GetAlbums()
        {
            foreach (var album in from s in songs
                                   group s by s.Album into albums
                                   orderby albums.Key
                                   select new { AlbumName = albums.Key, Songs = albums })
            {
                yield return new Album
                {
                    Name = album.AlbumName,
                    Songs = album.Songs.AsQueryable()
                };
            }
        }

        public IEnumerable<Artist> GetArtists()
        {
            foreach (var artist in from s in songs
                                  group s by s.Artist into artists
                                  orderby artists.Key
                                  select new { ArtistName = artists.Key, Songs = artists })
            {
                yield return new Artist
                {
                    Name = artist.ArtistName,
                    Songs = artist.Songs.AsQueryable()
                };
            }
        }

        public IEnumerable<Song> GetSongs()
        {
            return songs.AsEnumerable();
        }
    }
}
