using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandbrakeBatchEncoder.Services;

namespace HandbrakeBatchEncoder.Factories
{
    public interface IEncoderFactory
    {
        IEncoder GetEncoder(EncoderSettings settings);
    }
}
