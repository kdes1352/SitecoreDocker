using AllinaHealth.Framework.Factory;
using Sitecore.Pipelines;

namespace AllinaHealth.Framework.Pipelines.Initialize
{
    public class InitializeControllerFactory : Sitecore.Mvc.Pipelines.Loader.InitializeControllerFactory
    {
        protected override void SetControllerFactory(PipelineArgs args)
        {
            //Container.Install(FromAssembly.Named("AllinaHealth.Web"));

            var current = System.Web.Mvc.ControllerBuilder.Current.GetControllerFactory();
            var controllerFactory = new ControllerFactory(current);
            System.Web.Mvc.ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }
    }
}