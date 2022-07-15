using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Sitecore.Diagnostics;

namespace AllinaHealth.IOC
{
    public abstract class BaseWindsorInstaller : IWindsorInstaller
    {
        public abstract void Install(IWindsorContainer container, IConfigurationStore store);

        protected virtual void LoadServices(IWindsorContainer container, Assembly assembly)
        {
            Log.Info("Loading services from assembly: " + assembly.FullName, this);
            try
            {

                LoadServicesFromFromAssembly(container, assembly);
                LoadTransientServicesFromFromAssembly(container, assembly);
            }
            catch (System.Exception ex)
            {
                Log.Error("Error loading assembly: " + assembly.FullName, ex, this);
                throw;
            }
        }

        protected virtual void LoadServicesFromFromAssembly(IWindsorContainer container, Assembly assembly)
        {
            var serviceTypeRegistrations = Classes.FromAssembly(assembly)
                .BasedOn<IServiceType>()
                .WithService
                .DefaultInterfaces()
                .LifestylePerWebRequest();
            container.Register(serviceTypeRegistrations);
        }

        protected virtual void LoadTransientServicesFromFromAssembly(IWindsorContainer container, Assembly assembly)
        {
            container.Register(Classes.FromAssembly(assembly).BasedOn<IServiceTypeTransient>().WithService.DefaultInterfaces().LifestyleTransient());
        }
    }
}
