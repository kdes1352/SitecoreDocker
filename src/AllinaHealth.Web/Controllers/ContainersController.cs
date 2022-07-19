using System.Web.Mvc;
using AllinaHealth.Models.Allina_Health.Datasources.Containers;
using Glass.Mapper.Sc.Web.Mvc;

namespace AllinaHealth.Web.Controllers
{
    public class ContainersController : GlassController
    {
        public ActionResult ContentBox()
        {
            var context = new MvcContext();
            var model = context.GetDataSourceItem<IContent_Box>();
            if (model == null)
            {
                return new EmptyResult();
            }
            return View("~/Views/Containers/ContentBox.cshtml", model);
        }
    }
}