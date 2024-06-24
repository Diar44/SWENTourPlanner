using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using log4net;
using log4net.Config;

namespace SWENTourPlanner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            string log4netConfigPath = "..\\..\\..\\log4net.config";

            var log4netConfig = new FileInfo(log4netConfigPath);
            XmlConfigurator.ConfigureAndWatch(log4netConfig);
        }
    }

}
