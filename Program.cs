using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ninject;

namespace HandbrakeBatchEncoder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IKernel kernel = new StandardKernel(new ApplicationModules.Services());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(kernel.Get<MainForm>());
        }
    }
}
