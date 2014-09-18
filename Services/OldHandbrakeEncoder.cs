using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using HandbrakeBatchEncoder.Properties;

namespace HandbrakeBatchEncoder.Services
{

    public static class OldHandbrakeEncoder
    {
       

        public static void EncodeFile(object sender, DoWorkEventArgs e)
        {
            BackgroundEncodeFile(sender as BackgroundWorker, e);
        }


        private static void BackgroundEncodeFile(BackgroundWorker worker, DoWorkEventArgs e)
        {
            var settings = e.Argument as EncoderSettings;
            if (worker == null || settings == null) return;

            // Make sure source file exists, but destination file doesn't (don't overwrite existing file)
            if (!File.Exists(settings.SourceFilename) || (File.Exists(settings.DestinationFilename)))
                return;

            // Wait for file to become readable...
            while (true)
            {
                try
                {
                    var fs = new FileStream(settings.SourceFilename, FileMode.Open, FileAccess.Read, FileShare.None);
                    fs.Close();
                    break;
                }
                catch (FileNotFoundException)
                {
                    throw;
                }
                catch (IOException)
                {
                    Thread.Sleep(1000);
                }
            }

            // Encode the file
            var handbrake = new Process();
            var tempDestinationFilename = Path.ChangeExtension(settings.DestinationFilename, ".tmp");
            var filenameArgs = string.Format(" -i \"{0}\" -o \"{1}\"", settings.SourceFilename, tempDestinationFilename);
            handbrake.StartInfo.FileName = Settings.Default.HandbrakeCliPath;
            handbrake.StartInfo.WorkingDirectory = Path.GetDirectoryName(Settings.Default.HandbrakeCliPath);
            handbrake.StartInfo.Arguments = Settings.Default.EncodeSettings + filenameArgs;
            //startInfo.UseShellExecute = false;
            //startInfo.RedirectStandardError = true;

            handbrake.Start();
            if (!handbrake.HasExited)
            {
                handbrake.PriorityClass = ProcessPriorityClass.Idle;
                handbrake.WaitForExit();
            }
            else
            {
                //var error = handbrake.StandardError.ReadToEnd();
            }

            // Rename the output file to .avi (or original file extension)
            if (File.Exists(tempDestinationFilename))
            {
                File.Move(tempDestinationFilename, settings.DestinationFilename);

                var competedSourceFolder = Path.Combine(Settings.Default.WatchFolder, Settings.Default.CompletedInputFolder);

                // Move the source file to a Completed subfolder
                if (!Directory.Exists(competedSourceFolder))
                {
                    // Folder doesn't exist - try to create it
                    try
                    {
                        Directory.CreateDirectory(competedSourceFolder);
                    }
                    catch (IOException)
                    {
                    }
                }

                if (Directory.Exists(competedSourceFolder))
                {
                    File.Move(settings.SourceFilename, Path.Combine(competedSourceFolder, Path.GetFileName(settings.SourceFilename)));
                }
            }

            worker.ReportProgress(100);
        }

    }
}
