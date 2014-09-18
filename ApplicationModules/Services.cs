using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandbrakeBatchEncoder.Factories;
using HandbrakeBatchEncoder.Services;
using Ninject.Modules;

namespace HandbrakeBatchEncoder.ApplicationModules
{
    public class Services : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileWatcher>().To<FileWatcher>()
                                .InSingletonScope()
                                .WithConstructorArgument("watchFolder", Properties.Settings.Default.WatchFolder)
                                .WithConstructorArgument("watchedExtensions", new[] { ".avi", ".mpg", ".mpeg", ".mp4", ".m4v" });
            Bind<IEncoderFactory>().To<HandbrakeEncoderFactory>()
                                   .InSingletonScope();
        }
    }
}
