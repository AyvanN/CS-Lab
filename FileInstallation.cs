using System;
using System.ServiceProcess;
using System.IO;
using System.Threading;
using System.Xml.Schema;
using System.Xml.Linq;

namespace FileWatcher
{
    public partial class FileInstallation : ServiceBase
    {
        FileLogger logger;

        public FileInstallation()
        {
            InitializeComponent();
        }

        protected override void OnStop()
        {
            if (!(logger is null))
            {
                logger.StopService();
            }
        }

        protected override void OnStart(string[] args)
        {
            CheckingFiles config;

            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XMLFile.xml")) &&
                File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XMLFile.xsd")))
            {
                XmlSchemaSet schema = new XmlSchemaSet();

                schema.Add(string.Empty, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XMLFile.xsd"));

                XDocument xdoc = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XMLFile.xml"));

                xdoc.Validate(schema, ValidationEventHandler);

                config = new CheckingFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XMLFile.xml"), typeof(Tools));
            }
            else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonFile.json")))
            {
                config = new CheckingFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonFile.json"), typeof(Tools));
            }
            else
            {
                throw new ArgumentNullException($"Config was not found");
            }

            Tools options = config.Parse<Tools>();

            logger = new FileLogger(options);

            Thread loggerThread = new Thread(new ThreadStart(logger.StartService));

            loggerThread.Start();
        }

        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (Enum.TryParse("Error", out XmlSeverityType type) && type == XmlSeverityType.Error)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
