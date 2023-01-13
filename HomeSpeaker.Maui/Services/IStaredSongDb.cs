namespace HomeSpeaker.Maui.Services
{
    public interface IStaredSongDb
    {
        Task<int> DeleteStarredSong(StarredSong song);
        Task<int> DeleteStarredSong(string path);
        Task<List<StarredSong>> GetStarredSongsAsync();
        Task<int> SaveStarredSongAsync(StarredSong song);
    }
}