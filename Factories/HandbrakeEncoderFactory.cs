using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandbrakeBatchEncoder.Services;
using Ninject.Syntax;

namespace HandbrakeBatchEncoder.Factories
{
    public class HandbrakeEncoderFactory : IEncoderFactory
    {
        private readonly IResolutionRoot kernel;

        public HandbrakeEncoderFactory(IResolutionRoot kernel)
        {
            this.kernel = kernel;
        }

        public IEncoder GetEncoder(EncoderSettings settings)
        {
            return new HandbrakeEncoder();
        }
    }
}
