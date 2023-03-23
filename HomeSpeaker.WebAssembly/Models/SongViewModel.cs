namespace HomeSpeaker.WebAssembly.Models;

public class SongViewModel
{
    public int SongId { get; set; }
    public string Name { get; set; }
    private string path;
    public string Path
    {
        get => path;
        set
        {
            path = value;
            if (path.Contains('\\'))
                Folder = System.IO.Path.GetDirectoryName(path.Replace('\\', '/'));
            else
                Folder = System.IO.Path.GetDirectoryName(path);
        }
    }
    public string Album { get; set; }
    public string Artist { get; set; }
    public string Folder { get; set; }
}

public partial class SongGroup : List<SongViewModel>
{
    public string FolderName { get; set; }
    public string FolderPath { get; set; }

    public SongGroup(string name, List<SongViewModel> songs) : base(songs)
    {
        var parts = name.Split('/', '\\');
        FolderName = parts.Last();
        FolderPath = name;
    }
}

public static class ViewModelExtensions
{
    public static SongViewModel ToSongViewModel(this SongMessage song)
    {
        return new SongViewModel
        {
            SongId = song?.SongId ?? -1,
            Name = song?.Name ?? "[ Null Song Response ??? ]",
            Album = song?.Album,
            Artist = song?.Artist,
            Path = song?.Path
        };
    }
    //public async static IAsyncEnumerable<T> ReadAllAsync<T>(this IAsyncStreamReader<T> streamReader, CancellationToken cancellationToken = default)
    //{
    //    if (streamReader == null)
    //    {
    //        throw new System.ArgumentNullException(nameof(streamReader));
    //    }

    //    while (await streamReader.MoveNext(cancellationToken))
    //    {
    //        yield return streamReader.Current;
    //    }
    //}
}