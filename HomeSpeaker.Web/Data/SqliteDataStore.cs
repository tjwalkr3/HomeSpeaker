using HomeSpeaker.Lib;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeSpeaker.Web.Data
{

    public class SqliteDataStore : IDataStore
    {
        private readonly ApplicationDbContext dbContext;

        public SqliteDataStore(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddOrUpdateAsync(Song song)
        {
            var existingSong = dbContext.Songs.Find(song.SongId);
            if (existingSong == null)
            {
                dbContext.Songs.Add(song);
                await dbContext.SaveChangesAsync();
            }
        }

        public IEnumerable<Album> GetAlbums()
        {
            foreach (var album in from s in dbContext.Songs
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
            foreach (var artist in from s in dbContext.Songs
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
            return dbContext.Songs.AsEnumerable();
        }
    }
}
