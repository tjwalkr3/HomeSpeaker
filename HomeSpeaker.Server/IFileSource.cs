using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HomeSpeaker.Server
{
    public interface IFileSource
    {
        IEnumerable<FileInfo> GetAllMp3s();
        string RootFolder{get;}
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
            return from f in Directory.GetFiles(rootFolder, "*.mp3", SearchOption.AllDirectories)
                   select new FileInfo(f);
        }
    }
}
