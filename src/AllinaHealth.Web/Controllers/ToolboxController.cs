using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AllinaHealth.Models.Extensions;
using AllinaHealth.Models.ViewModels.Toolbox;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;

namespace AllinaHealth.Web.Controllers
{
    public class ToolboxController : Controller
    {
        public ActionResult NavigationTile()
        {
            return View("~/Views/Toolbox/NavigationTile.cshtml");
        }

        public ActionResult ContentBoxSignUp()
        {
            return View("~/Views/Toolbox/ContentBoxSignUp.cshtml");
        }

        public ActionResult IconLink()
        {
            return View("~/Views/Toolbox/IconLink.cshtml");
        }

        public ActionResult Teaser()
        {
            return View("~/Views/Toolbox/Teaser.cshtml");
        }

        public ActionResult RelatedLink()
        {
            return View("~/Views/Toolbox/RelatedLink.cshtml");
        }

        public ActionResult AccordionEntry()
        {
            return View("~/Views/Toolbox/AccordionEntry.cshtml");
        }

        public ActionResult PhotoWithCaption()
        {
            return View("~/Views/Toolbox/PhotoWithCaption.cshtml");
        }

        public ActionResult StackedImage()
        {
            return View("~/Views/Toolbox/StackedImage.cshtml");
        }

        public ActionResult StackedShadowButton()
        {
            return View("~/Views/Toolbox/StackedShadowButton.cshtml");
        }

        public ActionResult YoutubeVideo()
        {
            return View("~/Views/Toolbox/YoutubeVideo.cshtml");
        }

        public ActionResult PageAlert()
        {
            return View("~/Views/Toolbox/PageAlert.cshtml");
        }

        public ActionResult MapBoxMap()
        {
            return View("~/Views/Toolbox/MapBoxMap.cshtml");
        }

        public ActionResult AtoZ()
        {
            var dictionary = new Dictionary<string, List<Item>>();
            var scList = RenderingContext.Current.Rendering.Item.GetSelectedItems("Links");
            ProcessDictionary(dictionary, scList);
            return View("~/Views/Toolbox/AtoZ.cshtml", dictionary);
        }

        public ActionResult PhotoGallery()
        {
            return View("~/Views/Toolbox/PhotoGallery.cshtml");
        }

        public ActionResult Slide()
        {
            return View("~/Views/Toolbox/Slide.cshtml");
        }

        public ActionResult RichTextSlide()
        {
            return View("~/Views/Toolbox/RichTextSlide.cshtml");
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

        // ReSharper disable once InconsistentNaming
        public ActionResult EDWaitTime()
        {
            return View("~/Views/Toolbox/EDWaitTimes.cshtml");
        }


        public ActionResult TileButton()
        {
            return View("~/Views/Toolbox/TileButton.cshtml");
        }

        public ActionResult TwitterFeed()
        {
            return View("~/Views/Toolbox/TwitterFeed.cshtml");
        }

        public ActionResult QandA()
        {
            var model = new QandATreeModel(RenderingContext.Current.Rendering.Item);
            return View("~/Views/Toolbox/QandA.cshtml", model);
        }
    }
}