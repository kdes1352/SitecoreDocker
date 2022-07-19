using System.Web;
using System.Web.Mvc;
using AllinaHealth.IOC;
using AllinaHealth.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Installer;
using Glass.Mapper.Sc;

[assembly: PreApplicationStartMethod(typeof(InitializeIOC), "Initialize")]

namespace AllinaHealth.Web
{
    // ReSharper disable once InconsistentNaming
    public class InitializeIOC
    {
        public static void Initialize()
        {
            Container.Install(FromAssembly.This());
        }
    }

    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient());

            //Glass stuff 
            container.Register(Component.For<ISitecoreService>().UsingFactoryMethod(k => (ISitecoreService)new SitecoreService(Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database)).LifestylePerWebRequest());
#pragma warning disable CS0618
            container.Register(Component.For<ISitecoreContext>().ImplementedBy<SitecoreContext>().LifestylePerWebRequest());
#pragma warning restore CS0618
        }
    }
}