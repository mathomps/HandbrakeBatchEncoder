using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HandbrakeBatchEncoder.Factories;
using HandbrakeBatchEncoder.Properties;
using HandbrakeBatchEncoder.Services;

namespace HandbrakeBatchEncoder
{
    public partial class MainForm : Form
    {
        private readonly IEncoderFactory encoderFactory;
        private readonly IFileWatcher fileWatcher;

        private delegate void UpdateEncodeQueueListDelegate();

        private readonly List<string> encodeQueueFiles = new List<string>();
        //private readonly BackgroundWorker _encoderWorker = new BackgroundWorker { WorkerReportsProgress = true };
        //private readonly HandbrakeEncoder _encoder = new HandbrakeEncoder();
        private IEncoder encoder;

        public MainForm(IEncoderFactory encoderFactory,
                        IFileWatcher fileWatcher)
        {
            InitializeComponent();

            this.encoderFactory = encoderFactory;
            this.fileWatcher = fileWatcher;
        }

       


        private void MainForm_Load(object sender, EventArgs e)
        {
            // Setup watch on folder for file creations
            fileWatcher.FileFound += NewFileFound;
            fileWatcher.Watch();
            
            // Update source path label
            uxWatchFolderNameLabel.Text = fileWatcher.WatchedFolderName;

            //// Hook up Background Worker Events
            //// _encoderWorker.ProgressChanged += _encoderWorker_ProgressChanged;
            //_encoderWorker.RunWorkerCompleted += _encoderWorker_RunWorkerCompleted;
            //_encoderWorker.DoWork += OldHandbrakeEncoder.EncodeFile;
            
        }



        /// <summary>
        /// Checks if the HandbrakeEncoder is available to immediately start
        /// encoding the next file. If it is not, nothing is done until the
        /// encoder is no longer busy.
        /// </summary>
        private void CheckAndQueueEncoderTask()
        {
            UpdateEncodeQueueList();

            if (encoder.IsBusy || encodeQueueFiles.Count == 0)
            {
                return;
            }

            // Start encoding the next file in the queue
            var sourceFile = encodeQueueFiles[0];
            var destinationFile = Path.Combine(Settings.Default.OutputFolder,
                                                  Path.GetFileNameWithoutExtension(sourceFile));
            destinationFile = Path.ChangeExtension(destinationFile, Settings.Default.OutputFileExtension);

            var settings = new EncoderSettings
                               {
                                   SourceFilename = sourceFile,
                                   DestinationFilename = destinationFile
                               };
            //_encoder.SourceFilename = sourceFile;
            //_encoder.DestinationFilename = destinationFile;
            //_encoderWorker.RunWorkerAsync(settings);
            encoder.Encode(settings);

            //UpdateEncodeQueueList();

            //_encodeQueueFiles.RemoveAt(0);
        }

        /// <summary>
        /// Updates the pending list of files to be encoded in the UI.
        /// </summary>
        private void UpdateEncodeQueueList()
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateEncodeQueueListDelegate(UpdateEncodeQueueList));
                return;
            }

            uxEncodeQueueListView.Items.Clear();
            foreach (var file in encodeQueueFiles)
            {
                uxEncodeQueueListView.Items.Add(Path.GetFileName(file));
            }

            uxNowEncodingFile.Text = encodeQueueFiles.Count > 0 ? Path.GetFileName(encodeQueueFiles[0]) : "";

        }

        /// <summary>
        /// Adds a new file to the Encoder queue.
        /// </summary>
        /// <param name="filename"></param>
        private void AddFileToQueue(string filename)
        {
            if (encodeQueueFiles.Contains(filename)) return;
            encodeQueueFiles.Add(filename);
        }


        void NewFileFound(object sender, FileFoundArgs args)
        {
            AddFileToQueue(args.Filename);
            CheckAndQueueEncoderTask();
        }


        void _encoderWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Pop the completed job off the queue
            if (encodeQueueFiles.Count > 0)
            {
                encodeQueueFiles.RemoveAt(0);
            }

            // See if there's another item in the queue to encode
            CheckAndQueueEncoderTask();
        }


    }
}
