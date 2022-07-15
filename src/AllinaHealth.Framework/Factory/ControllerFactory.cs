using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AllinaHealth.IOC;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Helpers;

namespace AllinaHealth.Framework.Factory
{
    public class ControllerFactory : DefaultControllerFactory
    {
        readonly IControllerFactory _innerFactory;

        public ControllerFactory(IControllerFactory innerFactory)
        {
            _innerFactory = innerFactory;
        }

        public override void ReleaseController(IController controller)
        {
            Container.Release(controller);
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            Assert.ArgumentNotNull(requestContext, "requestContext");
            Assert.ArgumentNotNull(controllerName, "controllerName");
            Type controllerType = null;

            if (TypeHelper.LooksLikeTypeName(controllerName))
            {
                controllerType = TypeHelper.GetType(controllerName);
            }

            if (controllerType == null)
            {
                controllerType = GetControllerType(
                    requestContext,
                    controllerName);
            }

            if (controllerType != null && Container.HasComponent(controllerType))
            {
                return Container.Resolve(controllerType) as IController;
            }

            Assert.IsNotNull(_innerFactory, "_innerFactory");
            return _innerFactory.CreateController(requestContext, controllerName);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404, string.Format("The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
            }

            return (IController)Container.Resolve(controllerType);
        }
    }
}