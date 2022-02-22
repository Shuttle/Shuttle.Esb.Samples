using Ninject;
using Shuttle.Core.Container;
using Shuttle.Core.Ninject;
using Shuttle.Core.WorkerService;
using Shuttle.DependencyInjection.EMail;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;

namespace Shuttle.DependencyInjection.Server
{
    public class Host : IServiceHost
    {
        private IServiceBus _bus;
        private StandardKernel _kernel;

        public void Stop()
        {
            _kernel.Dispose();
            _bus.Dispose();
        }

        public void Start()
        {
            _kernel = new StandardKernel();

            _kernel.Bind<IEMailService>().To<EMailService>();

            var container = new NinjectComponentContainer(_kernel);

            container.Register<IAzureStorageConfiguration, DefaultAzureStorageConfiguration>();
            container.RegisterServiceBus();

            _bus = container.Resolve<IServiceBus>().Start();
        }
    }
}