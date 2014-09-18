using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandbrakeBatchEncoder.Services;

namespace HandbrakeBatchEncoder.Factories
{
    public class HandbrakeEncoderFactory : IEncoderFactory
    {
        public IEncoder GetEncoder(EncoderSettings settings)
        {
            return new HandbrakeEncoder();
        }
    }
}
