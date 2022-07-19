using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AllinaHealth.Models.Extensions;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;

namespace AllinaHealth.Web.Controllers
{
    public class ContentController : Controller
    {
        public ActionResult ContentBlock()
        {
            return View("~/views/content/contentblock.cshtml");
        }

        public ActionResult GrayContentBlock()
        {
            return View("~/views/content/graycontentblock.cshtml");
        }

        public ActionResult ContentListWithButton()
        {
            var dictionary = new Dictionary<string, List<Item>>();
            var scList = RenderingContext.Current.Rendering.Item.GetSelectedItems("Selected Button");
            ProcessDictionary(dictionary, scList);
            return View("~/views/content/contentlistwithbutton.cshtml", dictionary);
        }

        private static void ProcessDictionary(IDictionary<string, List<Item>> dictionary, IReadOnlyCollection<Item> list)
        {
            for (var unicode = 65; unicode < 91; unicode++)
            {
                var character = (char)unicode;
                var key = character.ToString();

                var selectedItems = list.Where(fu => !string.IsNullOrEmpty(fu.GetFieldValue("Link Text")) && fu.GetFieldValue("Link Text").ToUpper().StartsWith(key)).OrderBy(fu => fu.GetFieldValue("Link Text").ToUpper()).ToList();

                dictionary.Add(key, selectedItems);
            }
        }

        public ActionResult ColorHeader()
        {
            return View("~/Views/Content/ColorHeader.cshtml");
        }

        public ActionResult HealthSource()
        {
            return View("~/Views/Content/HealthSource.cshtml");
        }

        public ActionResult Button()
        {
            return View("~/Views/Content/Button.cshtml");
        }

        public ActionResult FeedList()
        {
            return View("~/Views/Content/Button.cshtml");
        }

        public ActionResult TipBox()
        {
            var tList = new List<string> { "full width container" };
            return View("~/views/content/tipbox.cshtml", tList);
        }

        public ActionResult TipBoxForSplits()
        {
            var tList = new List<string> { "split container" };
            return View("~/views/content/tipbox.cshtml", tList);
        }

        public ActionResult BorderCalloutWithButtonOption()
        {
            return View("~/Views/Content/BorderedCalloutWithButtonOption.cshtml");
        }

        public ActionResult Testimonial()
        {
            return View("~/Views/Content/Testimonial.cshtml");
        }

        public ActionResult LinkLibrary()
        {
            return View("~/Views/Content/LinkLibrary.cshtml");
        }

        public ActionResult LinkSection()
        {
            return View("~/Views/Content/LinkSection.cshtml");
        }
    }
}