using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandbrakeBatchEncoder.Services
{
    public class HandbrakeEncoder : IEncoder
    {
        private readonly BackgroundWorker encoderWorker;

        public HandbrakeEncoder()
        {
            encoderWorker = new BackgroundWorker();
            encoderWorker.RunWorkerCompleted += EncodingCompleted;
        }

      

        public void Encode(EncoderSettings settings)
        {
            
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
                Completed(this, new EncodeCompleteEventArgs());
            }

            throw new NotImplementedException();
        }

    }
}
