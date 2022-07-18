using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Configuration;
using AllinaHealth.Framework.Contexts;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Mvc.Pipelines.MvcEvents.Exception;

namespace AllinaHealth.Framework.Pipelines.Mvc.Exception
{
    public class ProcessError : ExceptionProcessor
    {
        public override void Process(ExceptionArgs args)
        {
            var context = args.ExceptionContext;
            var httpContext = context.HttpContext;
            var exception = context.Exception;

            if (context.ExceptionHandled || httpContext == null || exception == null)
            {
                return;
            }

            var exceptionInfo = GetExceptionInfo(httpContext, exception);
            Log.Error("Exception for Site: " + Sitecore.Context.Site.Name + Environment.NewLine + exceptionInfo, this);

            var section = (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");
            if (section.Mode == CustomErrorsMode.Off || (section.Mode == CustomErrorsMode.RemoteOnly && httpContext.Request.IsLocal))
            {
                return;
            }

            var page500 = SiteContext.Current.Page500;
            if (page500 != null)
            {
                httpContext.Server.ClearError();
                httpContext.Response.Redirect(LinkManager.GetItemUrl(page500), true);
            }
            else
            {
                Log.Error("No 500 page found for site: " + Sitecore.Context.Site.Name, this);
            }
        }

        private static string GetExceptionInfo(HttpContextBase httpContext, System.Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine("URL: " + httpContext.Request.Url);
            sb.AppendLine("Source: " + exception.Source);
            sb.AppendLine("Message: " + exception.Message);
            sb.AppendLine("Stack Trace: " + exception.StackTrace);
            return sb.ToString();
        }
    }
}