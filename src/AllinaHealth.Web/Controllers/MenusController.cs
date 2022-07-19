using System.Web.Mvc;
using AllinaHealth.Models.ViewModels.Menus;

namespace AllinaHealth.Web.Controllers
{
    public class MenusController : Controller
    {
        public ActionResult MenuBlock()
        {
            var model = new MenuTreeModel(Sitecore.Mvc.Presentation.RenderingContext.Current.Rendering.Item);
            return View("~/views/Menus/Menus.cshtml", model);
        }
    }
}