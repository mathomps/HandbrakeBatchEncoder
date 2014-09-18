using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandbrakeBatchEncoder.Services
{
    public interface IFileWatcher
    {
        void Watch();
        void Unwatch();
        string WatchedFolderName { get; }
        event FileFound FileFound;
    }

    public delegate void FileFound(object sender, FileFoundArgs args);

    public class FileFoundArgs
    {
        public FileFoundArgs(string filename)
        {
            Filename = filename;
        }
        public string Filename { get; set; }
    }
}
