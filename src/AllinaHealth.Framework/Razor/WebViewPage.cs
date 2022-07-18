using AllinaHealth.Framework.Contexts;
using AllinaHealth.Models.Contexts;

namespace AllinaHealth.Framework.Razor
{
    public abstract class WebViewPage : System.Web.Mvc.WebViewPage
    {
        // ReSharper disable once InconsistentNaming
        public SiteContextModel AH => SiteContext.Current;
    }

    public abstract class WebViewPage<T> : System.Web.Mvc.WebViewPage<T>
    {
        // ReSharper disable once InconsistentNaming
        public SiteContextModel AH => SiteContext.Current;
    }
}