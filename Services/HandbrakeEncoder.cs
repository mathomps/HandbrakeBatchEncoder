using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HandbrakeBatchEncoder.Properties;

namespace HandbrakeBatchEncoder.Services
{
    public class HandbrakeEncoder : IEncoder
    {
        private readonly BackgroundWorker encoderWorker;

        public HandbrakeEncoder()
        {
            encoderWorker = new BackgroundWorker();
            encoderWorker.RunWorkerCompleted += EncodingCompleted;
            encoderWorker.DoWork += BackgroundEncoder;
        }

        void BackgroundEncoder(object sender, DoWorkEventArgs e)
        {
            var settings = e.Argument as EncoderSettings;
            if (settings == null) return;

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

            encoderWorker.ReportProgress(100);
        }


        // IEncoder Implementation

        public void Encode(EncoderSettings settings)
        {
            if (IsBusy)
            {
                var busyEvent = new EncodeCompleteEventArgs(false, "Encoder is currently busy!");
                Completed(this, busyEvent);
            }

            encoderWorker.RunWorkerAsync(settings);

        }

        public bool IsBusy
        {
            get { return encoderWorker.IsBusy; }
        }

        public event EncodeComplete Completed;


        void EncodingCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var evt = Completed;
            if (evt != null)
            {
                Completed(this, new EncodeCompleteEventArgs(true,null));
            }
        }

    }
}
