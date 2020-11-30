using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileWatcher
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            FileInstallation fileinstallation = new FileInstallation();

            try
            {
                ServiceBase.Run(fileinstallation);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exceptions.txt"), true))
                {
                    sw.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Exception: {ex.Message}");
                }
            }
        }
    }
}
