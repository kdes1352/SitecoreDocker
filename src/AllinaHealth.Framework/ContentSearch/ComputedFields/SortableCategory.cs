using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Diagnostics;

namespace AllinaHealth.Framework.ContentSearch.ComputedFields
{
    public class SortableCategory : IComputedIndexField
    {
        public object ComputeFieldValue(IIndexable indexable)
        {
            Assert.ArgumentNotNull(indexable, "indexable");
            var item = (indexable as SitecoreIndexableItem)?.Item;
            if (item != null && item.Fields["Category"] != null)
            {
                return (string.IsNullOrWhiteSpace(item.Fields["Category"].Value)) ? null : item.Fields["Category"].Value;
            }

            return null;
        }

        public string FieldName { get; set; }
        public string ReturnType { get; set; }
    }
}