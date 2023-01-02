namespace HomeSpeaker.Server
{
    public interface IFileSource
    {
        IEnumerable<FileInfo> GetAllMp3s();
        string RootFolder { get; }
    }

    public class DefaultFileSource : IFileSource
    {
        private readonly string rootFolder;

        public DefaultFileSource(string rootFolder)
        {
            this.rootFolder = rootFolder;
        }

        public string RootFolder => rootFolder;

        public IEnumerable<FileInfo> GetAllMp3s()
        {
            var musicFolder = rootFolder.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            if (!Directory.Exists(musicFolder))
            {
                Directory.CreateDirectory(musicFolder);
            }

            var files = from f in Directory.GetFiles(musicFolder, "*.mp3", SearchOption.AllDirectories)
                        select new FileInfo(f);
            return files;
        }
    }
}
