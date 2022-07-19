using System.Web.Mvc;
using AllinaHealth.Models.Extensions;

namespace AllinaHealth.Web.Controllers
{
    public class RedirectController : Controller
    {
        public ActionResult Index()
        {
            var url = Sitecore.Context.Item.GetLinkFieldUrl("Redirect");
            if (!string.IsNullOrEmpty(url) && Sitecore.Context.PageMode.IsNormal)
            {
                Response.RedirectPermanent(url);
            }

            return View("~/Views/Redirect/Index.cshtml");
        }
    }
}