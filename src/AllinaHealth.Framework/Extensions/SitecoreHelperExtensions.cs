using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.HtmlControls;

namespace AllinaHealth.Framework.Extensions
{
    public static class SitecoreHelperExtensions
    {
        public static HtmlString LinkAndOtherField(this SitecoreHelper sitecoreHelper, string linkFieldName, string otherFieldName, Item i = null, object linkParameters = null)
        {
            if (sitecoreHelper == null) throw new ArgumentNullException(nameof(sitecoreHelper));
            var result = string.Empty;
            if (i == null)
            {
                i = RenderingContext.Current.Rendering.Item;
            }

            if (i == null) return new HtmlString(result);
            if (linkParameters == null)
            {
                linkParameters = new { haschildren = true };
            }

            LinkField lf = i.Fields[linkFieldName];
            if (lf != null)
            {
                result = string.Format("{0}{1}{2}", sitecoreHelper.BeginField(linkFieldName, i, linkParameters), sitecoreHelper.Field(otherFieldName, i), sitecoreHelper.EndField());
            }

            return new HtmlString(result);
        }

        public static HtmlString LinkFieldWithString(this SitecoreHelper sitecoreHelper, string linkFieldName, string value, Item i = null, object linkParameters = null)
        {
            if (sitecoreHelper == null) throw new ArgumentNullException(nameof(sitecoreHelper));
            var result = string.Empty;
            if (i == null)
            {
                i = RenderingContext.Current.Rendering.Item;
            }

            if (i == null) return new HtmlString(result);
            if (linkParameters == null)
            {
                linkParameters = new { haschildren = true };
            }

            LinkField lf = i.Fields[linkFieldName];
            if (lf != null)
            {
                result = string.Format("{0}{1}{2}", sitecoreHelper.BeginField(linkFieldName, i, linkParameters), value, sitecoreHelper.EndField());
            }

            return new HtmlString(result);
        }

        public static HtmlString DatePickerControl(this SitecoreHelper helper, string id)
        {
            var datePicker = new DatePicker();
            datePicker.ID = id;
            datePicker.ClientIDMode = ClientIDMode.Static;
            datePicker.Width = new Unit(100);
            return new HtmlString(datePicker.RenderControlAsString());
        }

        public static HtmlString CanonicalUrl(this SitecoreHelper sitecoreHelper)
        {
            var urlOptions = LinkManager.GetDefaultUrlOptions();
            urlOptions.AlwaysIncludeServerUrl = true;

            var canonicalUrl = LinkManager.GetItemUrl(Context.Item, urlOptions);

            var canonicalQueryString = GetCanonicalQueryString(Context.RawUrl);

            if (!string.IsNullOrEmpty(canonicalQueryString))
            {
                canonicalUrl += canonicalQueryString;
            }

            return new HtmlString(canonicalUrl);
        }

        private static string GetCanonicalQueryString(string rawUrl)
        {
            string canonicalQueryString = null;

            if (rawUrl.IndexOf('?') <= 0)
            {
                return null;
            }

            var queryString = rawUrl.Substring(rawUrl.IndexOf('?'));
            if (queryString.Length > 1)
            {
                canonicalQueryString = queryString;
            }

            return canonicalQueryString;
        }

    }
}