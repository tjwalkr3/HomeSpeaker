namespace HomeSpeaker.Maui.ViewModels;

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
