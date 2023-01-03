namespace HomeSpeaker.Maui.Services
{
    public class Database
    {
        //readonly SQLiteAsyncConnection _database;
        bool initialized = false;
        List<StarredSong> starredSongs = new();
        private readonly string dbPath;

        public Database(string dbPath)
        {
            //_database = new SQLiteAsyncConnection(dbPath);
            try
            {
                if (File.Exists(dbPath))
                {
                    var text = File.ReadAllText(dbPath);
                    var deserialized = JsonSerializer.Deserialize<IEnumerable<StarredSong>>(text);
                    starredSongs.AddRange(deserialized);
                }
            }
            catch
            {
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
                starredSongs = new List<StarredSong>();
            }

            this.dbPath = dbPath;
        }

        //private async Task initIfNeededAsync()
        //{
        //    if (initialized)
        //        return;
        //    await _database.CreateTableAsync<StarredSong>();
        //    initialized = true;
        //}

        public async Task<List<StarredSong>> GetStarredSongsAsync()
        {
            return await Task.FromResult(starredSongs);
            //await initIfNeededAsync();
            //return await _database.Table<StarredSong>().ToListAsync();
        }

        public async Task<int> SaveStarredSongAsync(StarredSong song)
        {
            starredSongs.Add(song);
            await File.WriteAllTextAsync(dbPath, JsonSerializer.Serialize(starredSongs));
            return 1;

            //await initIfNeededAsync();
            //return await _database.InsertAsync(song);
        }

        public async Task<int> DeleteStarredSong(StarredSong song)
        {
            starredSongs.Remove(song);
            await File.WriteAllTextAsync(dbPath, JsonSerializer.Serialize(starredSongs));
            return 1;

            //await initIfNeededAsync();
            //return await _database.DeleteAsync(song);
        }

        public async Task<int> DeleteStarredSong(string path)
        {
            int removed = starredSongs.RemoveAll(s => s.Path == path);
            await File.WriteAllTextAsync(dbPath, JsonSerializer.Serialize(starredSongs));
            return removed;

            //await initIfNeededAsync();
            //return await _database.ExecuteAsync("delete from StarredSong where path = ?", path);
        }
    }
}
