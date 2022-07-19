using System;
using System.Web.Mvc;

namespace AllinaHealth.Web.Controllers
{
    public class GlobalController : Controller
    {
        public string WhichServer()
        {
            return Sitecore.Configuration.Settings.InstanceName;
        }

        public ActionResult ThrowError()
        {
            throw new Exception("Testing Error");
        }
    }
}