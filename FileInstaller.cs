using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;

namespace FileWatcher
{
    [RunInstaller(true)]
    public partial class FileInstaller : Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public FileInstaller()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "Service1";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
