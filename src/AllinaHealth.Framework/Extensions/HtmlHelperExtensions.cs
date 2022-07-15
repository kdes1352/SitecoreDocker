using System;
using System.Web.Mvc;
using AllinaHealth.Framework.Controls;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace AllinaHealth.Framework.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcEditFrame ItemEditFrame(this HtmlHelper htmlHelper, string buttonsFolder, Item i = null, string cssClass = "")
        {
            if (i == null)
            {
                i = RenderingContext.Current.Rendering.Item;
            }

            EditFrame frame = null;
            try
            {
                frame = new EditFrame { DataSource = i.ID.ToString(), Buttons = buttonsFolder, CssClass = cssClass };
            }
            catch (NullReferenceException)
            {
            }

            return new MvcEditFrame(frame, htmlHelper.ViewContext.Writer);
        }
    }
}