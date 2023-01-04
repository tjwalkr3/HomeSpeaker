//using SQLite;

namespace HomeSpeaker.Maui.Models;

public class StarredSong : IEquatable<StarredSong>
{
    //[PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Path { get; set; }

    public override bool Equals(object obj)
    {
        return Equals(obj as StarredSong);
    }

    public bool Equals(StarredSong other)
    {
        return other is not null &&
               Id == other.Id &&
               Path == other.Path;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Path);
    }
}
