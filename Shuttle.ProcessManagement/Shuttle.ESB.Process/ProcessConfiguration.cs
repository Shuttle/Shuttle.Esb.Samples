namespace Shuttle.ESB.Process
{
    public class ProcessConfiguration : IProcessConfiguration
    {
        public string ProviderName { get; set; }
        public string ConnectionString { get; set; }
    }
}