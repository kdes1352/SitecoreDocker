using System.Web.Mvc;

namespace AllinaHealth.Web.Controllers
{
    public class HeaderController : Controller
    {
        public ActionResult ImageBanner()
        {
            return View("~/Views/Header/ImageBanner.cshtml");
        }
    }
}