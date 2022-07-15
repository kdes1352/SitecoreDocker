using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace AllinaHealth.IOC
{
    public class Container
    {
        protected static readonly IWindsorContainer Windsor = new WindsorContainer();

        public static void Install(params IWindsorInstaller[] installers)
        {
            Windsor.Install(installers);
        }

        public static bool HasComponent(Type componentType)
        {
            return Windsor.Kernel.HasComponent(componentType);
        }

        public static T Resolve<T>()
        {
            return Windsor.Resolve<T>();
        }

        public static object Resolve(Type componentType)
        {
            return Windsor.Resolve(componentType);
        }

        public static void Release(object obj)
        {
            if (Windsor == null || obj == null) return;
            Windsor.Release(obj);
        }
    }
}