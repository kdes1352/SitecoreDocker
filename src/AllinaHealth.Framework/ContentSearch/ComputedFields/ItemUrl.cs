using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;
using Sitecore.Links;

namespace AllinaHealth.Framework.ContentSearch.ComputedFields
{
    public class ItemUrl : IComputedIndexField
    {
        public object ComputeFieldValue(IIndexable indexable)
        {
            if (!(indexable is SitecoreIndexableItem scIndexable))
            {
                return null;
            }

            var item = (Item)scIndexable;
            if (item == null)
            {
                return null;
            }

            var opts = LinkManager.GetDefaultUrlOptions();
            opts.Site = Sitecore.Sites.SiteContext.GetSite("website");
            opts.SiteResolving = true;
            var url = LinkManager.GetItemUrl(item, opts);
            if (url.StartsWith(":"))
            {
                url = url.Substring(1);
            }

            return url;
        }

        public string FieldName { get; set; }
        public string ReturnType { get; set; }
    }
}