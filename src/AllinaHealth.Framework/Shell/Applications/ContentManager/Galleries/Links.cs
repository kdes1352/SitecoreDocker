using System;
using System.Linq;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Shell.Applications.ContentManager.Galleries.Links;

namespace AllinaHealth.Framework.Shell.Applications.ContentManager.Galleries
{
    public class Links : GalleryLinksForm
    {
        protected override ItemLink[] GetReferences(Item item)
        {
            Assert.ArgumentNotNull(item, nameof(item));

            var list = item.Links.GetAllLinks(true, true);

            var grouping = list.GroupBy(e => e.TargetItemID);

            var returnList = grouping.Select(g => g.FirstOrDefault(e => e.TargetItemVersion.Number == g.Max(f => f.TargetItemVersion.Number))).Select(x => new Tuple<ItemLink, Item>(x, Context.ContentDatabase.GetItem(x?.TargetItemID))).ToList();

            return returnList.OrderBy(e => e.Item2.DisplayName).Select(e => e.Item1).ToArray();
        }

        protected override ItemLink[] GetRefererers(Item item)
        {
            Assert.ArgumentNotNull(item, nameof(item));
            var linkDatabase = Globals.LinkDatabase;
            Assert.IsNotNull(linkDatabase, "Link database cannot be null");

            var list = linkDatabase.GetItemReferrers(item, true);

            //Below: Old "Items that refer to the selected item" Links section, where only 1 thing shows up
            //var grouping = list.GroupBy(e => e.SourceFieldID);

            //Below: New "Items that refer to the selected item" Links section, all articles show up (alphabetized, see '.Displayname')
            var grouping = list.GroupBy(e => e.SourceItemID); //SourceFieldID

            var returnList = grouping.Select(g => g.FirstOrDefault(e => e.SourceItemVersion.Number == g.Max(f => f.SourceItemVersion.Number))).Select(x => new Tuple<ItemLink, Item>(x, Context.ContentDatabase.GetItem(x?.SourceItemID))).ToList();

            return returnList.OrderBy(e => e.Item2.DisplayName).Select(e => e.Item1).ToArray();
        }
    }
}