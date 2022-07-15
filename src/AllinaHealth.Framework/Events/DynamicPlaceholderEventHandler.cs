using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using AllinaHealth.Models.Extensions;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Events;

namespace AllinaHealth.Framework.Events
{
    public class DynamicPlaceholderEventHandler
    {
        public const string DynamicKeyRegex = @"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";
        public const int GuidLength = 36;

        public void OnItemSaved(object sender, EventArgs args)
        {
            var item = Event.ExtractParameter(args, 0) as Item;
            if (item.IsNull())
            {
                return;
            }

            var device = Context.Device;
            if (device == null)
            {
                return;
            }

            var renderingReferences = item?.Visualization.GetRenderings(device, false);
            if (renderingReferences == null)
            {
                return;
            }

            foreach (var rr in renderingReferences)
            {
                var key = rr.Placeholder;
                var regex = new Regex(DynamicKeyRegex);
                var match = regex.Match(rr.Placeholder);

                if (!match.Success || match.Groups.Count <= 0) continue;
                //get the rendering reference unique id that we are contained in
                var parentRenderingId = key.Substring(key.Length - GuidLength, GuidLength);

                if (!Guid.TryParse(parentRenderingId, out var parentGuid))
                {
                    continue;
                }

                parentRenderingId = parentGuid.ToString().ToUpper();

                var hasRenderingReference =
                    renderingReferences.Any(r => r.UniqueId.ToUpper().Contains(parentRenderingId));

                //if this parent renderingReference is not in the current list of rendering references 
                //then the current rendering reference should be removed as it means that the parent
                //rendering reference has been removed by the user without first removing  the children
                if (!hasRenderingReference)
                {
                    //use an extension method to remove the orphaned rendering reference
                    //from the item's layout definition
                    RemoveRenderingReference(item, rr.UniqueId);
                }
            }
        }

        public static void RemoveRenderingReference(Item item, string renderingReferenceUid)
        {
            var doc = new XmlDocument();
            var finalLayoutXml = item[FieldIDs.FinalLayoutField];
            if (string.IsNullOrEmpty(finalLayoutXml))
            {
                return;
            }

            doc.LoadXml(finalLayoutXml);

            //remove the orphaned rendering reference from the layout definition
            var node = doc.SelectSingleNode(string.Format("//r[@uid='{0}']", renderingReferenceUid));

            if (node?.ParentNode == null)
            {
                return;
            }

            node.ParentNode.RemoveChild(node);

            //save layout definition back to the item
            using (new EditContext(item))
            {
                item[FieldIDs.FinalLayoutField] = doc.OuterXml;
            }
        }
    }
}