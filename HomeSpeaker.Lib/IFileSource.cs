using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HomeSpeaker.Lib
{
    public interface IFileSource
    {
        IEnumerable<FileInfo> GetAllMp3s();
    }

    public class DefaultFileSource : IFileSource
    {
        private readonly string rootFolder;

        public DefaultFileSource(string rootFolder)
        {
            this.rootFolder = rootFolder;
        }

        public IEnumerable<FileInfo> GetAllMp3s()
        {
            return from f in Directory.GetFiles(rootFolder, "*.mp3", SearchOption.AllDirectories)
                   select new FileInfo(f);
        }
    }
}
