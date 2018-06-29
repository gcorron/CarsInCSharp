using Caliburn.Micro;
using Corron.Cars.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Corron.Cars
{
    class Bootstrapper:BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ViewModels.ShellViewModel>();
        }
    }
}
