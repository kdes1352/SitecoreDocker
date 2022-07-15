using Sitecore.Data.Comparers;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace AllinaHealth.Models.Comparers
{
    public abstract class NewsBlogPublishDateComparer : Comparer
    {
        public int BaseItemCompare(Item item1, Item item2)
        {
            ItemComparer itemComp = new ItemComparer();
            return itemComp.Compare(item1, item2);
        }
    }

    public class NewsBlogPublishDateComparerAsc : NewsBlogPublishDateComparer
    {
        protected override int DoCompare(Item item1, Item item2)
        {
            if (null == item1 && null == item2)
            {
                return 0;
            }
            DateField pubDateField1 = item1?.Fields["Publish Date"];
            DateField pubDateField2 = item2.Fields["Publish Date"];
            var pubDate1 = pubDateField1?.DateTime;
            var pubDate2 = pubDateField2?.DateTime;
            if (pubDate1.HasValue && pubDate2.HasValue)
            {
                return pubDate1.Equals(pubDate2) ? BaseItemCompare(item1, item2) : pubDate1.Value.CompareTo(pubDate2.Value);
            }
            if (pubDate1.HasValue)
            {
                return -1;
            }
            return pubDate2.HasValue ? 1 : BaseItemCompare(item1, item2);
            //No publish dates, fall back on item comparer
        }
    }

    public class NewsBlogPublishDateComparerDesc : NewsBlogPublishDateComparer
    {
        protected override int DoCompare(Item item1, Item item2)
        {
            if (null == item1 && null == item2)
            {
                return 0;
            }
            DateField pubDateField1 = item1?.Fields["Publish Date"];
            DateField pubDateField2 = item2.Fields["Publish Date"];
            var pubDate1 = pubDateField1?.DateTime;
            var pubDate2 = pubDateField2?.DateTime;
            if (pubDate1.HasValue && pubDate2.HasValue)
            {
                return pubDate2.Equals(pubDate1) ? BaseItemCompare(item2, item1) : pubDate2.Value.CompareTo(pubDate1.Value);
            }
            if (pubDate2.HasValue)
            {
                return -1;
            }
            return pubDate1.HasValue ? 1 : BaseItemCompare(item2, item1);
            //No publish dates, fall back on item comparer
        }
    }
}
