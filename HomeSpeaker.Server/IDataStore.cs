using HomeSpeaker.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeSpeaker.Server
{
    public interface IDataStore
    {
        Task AddOrUpdateAsync(Song song);
        IEnumerable<Artist> GetArtists();
        IEnumerable<Album> GetAlbums();
        IEnumerable<Song> GetSongs();
    }
}
