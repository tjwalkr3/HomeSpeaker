namespace HomeSpeaker.Server
{
    public interface IFileSource
    {
        IEnumerable<FileInfo> GetAllMp3s();
        void SoftDelete(string path);

        string RootFolder { get; }
    }

    public class DefaultFileSource : IFileSource
    {
        private readonly string rootFolder;
        string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public DefaultFileSource(string rootFolder)
        {
            this.rootFolder = rootFolder;
        }

        public string RootFolder => rootFolder;

        public IEnumerable<FileInfo> GetAllMp3s()
        {
            var musicFolder = rootFolder.Replace("~", userProfile);

            if (!Directory.Exists(musicFolder))
            {
                Directory.CreateDirectory(musicFolder);
            }

            var files = from f in Directory.GetFiles(musicFolder, "*.mp3", SearchOption.AllDirectories)
                        select new FileInfo(f);
            return files;
        }

        public void SoftDelete(string path)
        {
            File.Move(path, Path.Combine(userProfile, "DeletedMusic", Path.GetFileName(path)));
        }
    }
}
