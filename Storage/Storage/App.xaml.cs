using System.Windows;
using Autofac;
using Storage.Bootstrap;
using Storage.Util.Interface;

namespace Storage
{
    public partial class App : Application, IDispatcherProperty, IExitable
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacModule());

            using (var container = builder.Build().BeginLifetimeScope()) {
                container.Resolve<IBootstrapper>().Start();
            }
        }
    }
}
