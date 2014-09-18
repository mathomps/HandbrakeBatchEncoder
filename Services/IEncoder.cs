using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandbrakeBatchEncoder.Services
{
    public interface IEncoder
    {
        void Encode(EncoderSettings settings);
        bool IsBusy { get; }
        event EncodeComplete Completed;
    }

    public delegate void EncodeComplete(object sender, EncodeCompleteEventArgs args);

    public class EncodeCompleteEventArgs : EventArgs
    {
        public EncodeCompleteEventArgs(bool successful, string errorMessage)
        {
            Successful = successful;
            ErrorMessage = errorMessage;
        }
        public bool Successful { get; set; }
        public string ErrorMessage { get; set; }
    }

}
