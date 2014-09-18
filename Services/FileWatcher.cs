using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace HandbrakeBatchEncoder.Services
{
    public class FileWatcher : IFileWatcher
    {
        private readonly string[] watchedExtensions;
        private readonly FileSystemWatcher fsWatcher;

        public FileWatcher(string watchFolder, string[] watchedExtensions)
        {
            this.watchedExtensions = watchedExtensions;
            WatchedFolderName = watchFolder;
            fsWatcher = new FileSystemWatcher(watchFolder);
            fsWatcher.Created += OnFileCreated;
        }

        public void Watch()
        {
            fsWatcher.EnableRaisingEvents = true;

            ScanFolderForExistingFiles();

        }

        public void Unwatch()
        {
            fsWatcher.EnableRaisingEvents = false;
        }

        public string WatchedFolderName { get; private set; }


        void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            if (IsFileWatched(e.Name))
            {
                OnMatchedFileCreated(e.FullPath);
            }
           
        }

        private void OnMatchedFileCreated(string filename)
        {
            if (FileFound != null)
            {
                FileFound(this, new FileFoundArgs(filename));
            }
        }


        public event FileFound FileFound;

        private bool IsFileWatched(string filename)
        {
            var extension = Path.GetExtension(filename);
            if (string.IsNullOrEmpty(extension)) return false;
            return watchedExtensions.Contains(extension.ToLowerInvariant());
        }

        private void ScanFolderForExistingFiles()
        {
            var di = new DirectoryInfo(WatchedFolderName);

            foreach (var fi in di.GetFiles())
            {
                if (IsFileWatched(fi.Name))
                {
                    OnMatchedFileCreated(fi.FullName);
                }
            }
        }

    }
}
