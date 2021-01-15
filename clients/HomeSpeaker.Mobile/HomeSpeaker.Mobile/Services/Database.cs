using HomeSpeaker.Mobile.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeSpeaker.Mobile.Services
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<StarredSong>().Wait();
        }

        public Task<List<StarredSong>> GetStarredSongsAsync()
        {
            return _database.Table<StarredSong>().ToListAsync();
        }

        public Task<int> SaveStarredSongAsync(StarredSong song)
        {
            return _database.InsertAsync(song);
        }

        public Task<int> DeleteStarredSong(StarredSong song)
        {
            return _database.DeleteAsync(song);
        }

        public Task<int> DeleteStarredSong(string path)
        {
            return _database.ExecuteAsync("delete from StarredSong where path = ?", path);
        }
    }
}
