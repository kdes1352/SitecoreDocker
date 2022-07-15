using System;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Events;

namespace AllinaHealth.Framework.Events
{
    public class LowercaseNameForCreatedItem
    {
        public const string StartPath = "/sitecore/content/allina health";
        public const string DataFolderTemplateId = "{91B5BDC3-510F-47B8-BCFE-BB5914649C1B}";

        public LowercaseNameForCreatedItem()
        {

        }

        public void OnItemCreated(object sender, EventArgs args)
        {
            if (args == null)
            {
                return;
            }

            var icArgs = Event.ExtractParameter(args, 0) as ItemCreatedEventArgs;

            var i = icArgs?.Item;

            if (i == null)
            {
                return;
            }

            if (i.Database.Name != "master")
            {
                return;
            }

            if (!i.Paths.IsContentItem || i.TemplateID.ToString() == DataFolderTemplateId)
            {
                return;
            }

            using (new EditContext(i))
            {
                i.Name = i.Name.ToLower();
            }
        }
    }
}