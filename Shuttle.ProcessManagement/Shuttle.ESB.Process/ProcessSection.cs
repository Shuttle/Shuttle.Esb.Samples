using System.Configuration;
using Shuttle.ESB.Core;

namespace Shuttle.ESB.Process
{
    public class ProcessSection : ConfigurationSection
    {
        [ConfigurationProperty("connectionStringName", IsRequired = false, DefaultValue = "Process")]
        public string ConnectionStringName
        {
            get { return (string) this["connectionStringName"]; }
        }

        public static ProcessSection Open(string file)
        {
            return ShuttleConfigurationSection.Open<ProcessSection>("process", file);
        }

        public static ProcessConfiguration Configuration()
        {
            var section = ShuttleConfigurationSection.Open<ProcessSection>("process");
            var configuration = new ProcessConfiguration();

            var connectionStringName = "Process";

            if (section != null)
            {
                connectionStringName = section.ConnectionStringName;
            }

            var settings = ConfigurationManager.ConnectionStrings[connectionStringName];

            configuration.ConnectionString = settings.ConnectionString;
            configuration.ProviderName = settings.ProviderName;

            return configuration;
        }
    }
}