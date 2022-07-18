using System.Web.Mvc;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using ControllerBuilder = System.Web.Mvc.ControllerBuilder;

namespace AllinaHealth.Framework.Pipelines.Mvc.GetRenderer
{
    public class AreaControllerRunner : ControllerRunner
    {
        private readonly ControllerInfo controllerInfo;

        public AreaControllerRunner(ControllerInfo controllerInfo)
            : base(controllerInfo.Controller, controllerInfo.Action)
        {
            this.controllerInfo = controllerInfo;
        }

        protected override IController CreateController()
        {
            var requestContext = PageContext.Current.RequestContext;
            requestContext.RouteData.Values["controller"] = controllerInfo.Controller;
            requestContext.RouteData.Values["action"] = controllerInfo.Action;

            if (!string.IsNullOrWhiteSpace(controllerInfo.Area))
            {
                requestContext.RouteData.DataTokens["area"] = controllerInfo.Area;
            }

            if (!string.IsNullOrWhiteSpace(controllerInfo.Namespace))
            {
                requestContext.RouteData.DataTokens["namespaces"] = new[] { controllerInfo.Namespace };
            }

            return ControllerBuilder.Current.GetControllerFactory().CreateController(requestContext, controllerInfo.Controller);
        }

        protected override void ReleaseController(Controller controller)
        {
            ControllerBuilder.Current.GetControllerFactory().ReleaseController(controller);
        }
    }
}