using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AllinaHealth.Models.Global.Folders;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.Sites;

namespace AllinaHealth.Models.Extensions
{
    public static class ItemExtensions
    {
        private const string StandardValues = "__Standard Values";

        public static bool IsNull(this Item item)
        {
            return item == null || item.Version.Number == 0;
        }

        public static string GetFieldValue(this Item item, string fieldName, string nullValue = "")
        {
            if (item == null)
                return nullValue;

            if (item.Fields[fieldName] == null)
                return nullValue;

            return (string.IsNullOrWhiteSpace(item.Fields[fieldName].Value)) ? nullValue : item.Fields[fieldName].Value;
        }

        public static string GetFieldValue(this Item item, ID fieldId, string nullValue = "")
        {
            if (item == null)
                return nullValue;

            if (item.Fields[fieldId] == null)
                return nullValue;

            return (string.IsNullOrWhiteSpace(item.Fields[fieldId].Value)) ? nullValue : item.Fields[fieldId].Value;
        }

        public static string TryMultipleGetFieldValue(this Item i, params string[] fieldNames)
        {
            foreach (var value in fieldNames.Select(fieldName => i.GetFieldValue(fieldName)).Where(value => !string.IsNullOrEmpty(value)))
            {
                return value;
            }
            return string.Empty;
        }

        public static int GetFieldInteger(this Item i, string fieldName, int nullValue = 0)
        {
            var value = i.GetFieldNullableInteger(fieldName);
            return value ?? nullValue;
        }

        public static int? GetFieldNullableInteger(this Item i, string fieldName)
        {
            var f = i?.Fields[fieldName];
            if (f == null) return null;

            if (int.TryParse(f.Value, out var value))
            {
                return value;
            }
            return null;
        }

        public static SiteContext GetSiteContext(this Item i)
        {
            var list = Factory.GetSiteInfoList().OrderByDescending(x => x.RootPath.Length).Where(x => x.Domain != "sitecore" && !x.VirtualFolder.StartsWith("/sitecore")).ToList();
            var si = list.FirstOrDefault(x => i.Paths.FullPath.StartsWith(x.RootPath + x.StartItem, StringComparison.InvariantCultureIgnoreCase));
            return si != null ? SiteContext.GetSite(si.Name) : null;
        }

        public static string GetMediaUrl(this Item i, string fieldName, bool includeWidthInUrl = false)
        {
            var url = string.Empty;
            if (i == null)
            {
                return url;
            }

            ImageField imgField = i.Fields[fieldName];
            if (imgField == null)
            {
                return url;
            }

            MediaItem mi = imgField.MediaItem;
            if (mi == null) return url;
            url = MediaManager.GetMediaUrl(mi);

            if (includeWidthInUrl)
            {
                url += " " + imgField.Width + "w";
            }

            return url;
        }

        public static DateTime? GetNullableDate(this Item item, string fieldName)
        {
            DateField df = item?.Fields[fieldName];
            return df?.DateTime;
        }

        public static DateTime GetDate(this Item item, string fieldName)
        {
            if (item == null) return DateTime.MinValue;

            DateField df = item.Fields[fieldName];
            return df?.DateTime ?? DateTime.MinValue;
        }

        public static string GetLinkFieldUrl(this Item i, string fieldName)
        {
            if (i.IsNull()) return string.Empty;
            var lf = (LinkField)i.Fields[fieldName];
            if (lf == null) return string.Empty;
            if (lf.TargetItem == null) return lf.Url;
            return lf.IsMediaLink ? MediaManager.GetMediaUrl(lf.TargetItem) : LinkManager.GetItemUrl(lf.TargetItem);
        }

        public static Item GetLinkFieldTargetItem(this Item i, string fieldName)
        {
            if (i.IsNull()) return null;
            var lf = (LinkField)i.Fields[fieldName];
            return lf?.TargetItem;
        }

        public static Item GetFirstChildWithBaseTemplate(this Item item, string templateId)
        {
            return item?.GetChildren().FirstOrDefault(e => HasBaseTemplate(e, templateId));
        }

        public static Item GetFirstChildWithTemplate(this Item item, string templateId)
        {
            return item?.GetChildren().FirstOrDefault(e => e.TemplateID.ToString() == templateId);
        }

        public static bool GetCheckBoxFieldValue(this Item i, string fieldName, bool defaultReturnValue)
        {
            var returnValue = GetCheckBoxFieldNullableValue(i, fieldName);
            return returnValue ?? defaultReturnValue;
        }

        public static bool? GetCheckBoxFieldNullableValue(this Item i, string fieldName)
        {
            bool? returnValue = null;
            CheckboxField cbf = i?.Fields[fieldName];
            if (cbf != null)
            {
                returnValue = cbf.Checked;
            }
            return returnValue;
        }

        public static Item GetInternalLinkFieldItem(this Item i, string internalLinkFieldName)
        {
            var f = i?.Fields[internalLinkFieldName];

            if (f == null) return null;

            var ilf = (InternalLinkField)f;
            if (ilf?.TargetItem != null)
            {
                return ilf.TargetItem;
            }

            return ID.TryParse(f.Value, out var id) ? i.Database.GetItem(id) : null;
        }

        public static Item[] GetReferrers(this Item item, bool includeStandardValues = false)
        {
            if (item == null)
                return Array.Empty<Item>();
            var links = Globals.LinkDatabase.GetReferrers(item);
            if (links == null)
                return Array.Empty<Item>();
            var linkedItems = links.Select(i => i.GetSourceItem()).Where(i => i != null);
            if (!includeStandardValues)
                linkedItems = linkedItems.Where(i => !i.Name.Equals(StandardValues, StringComparison.InvariantCultureIgnoreCase));
            return linkedItems.ToArray();
        }

        public static Item[] GetReferences(this Item item, bool includeStandardValues = false)
        {
            if (item == null)
                return Array.Empty<Item>();
            var links = Globals.LinkDatabase.GetReferences(item);
            if (links == null)
                return Array.Empty<Item>();
            var linkedItems = links.Select(i => i.GetSourceItem()).Where(i => i != null);
            if (!includeStandardValues)
                linkedItems = linkedItems.Where(i => !i.Name.Equals(StandardValues, StringComparison.InvariantCultureIgnoreCase));
            return linkedItems.ToArray();
        }

        public static List<Item> GetSelectedItems(this Item i, string multiListFieldName)
        {
            if (i == null) return new List<Item>();

            MultilistField mlf = i.Fields[multiListFieldName];
            return mlf != null ? mlf.GetItems().ToList() : new List<Item>();
        }


        public static bool HasBaseTemplate(this Item item, string baseTemplateNameOrId)
        {
            if (IsNull(item) || item.TemplateID.IsNull) return false;

            var template = item.Template;

            if (template == null) return false;

            return IsMatchOnTemplateNameOrId(baseTemplateNameOrId, template) || HasBaseTemplateHelper(item.Template.BaseTemplates, baseTemplateNameOrId);
        }

        private static bool HasBaseTemplateHelper(IEnumerable<TemplateItem> baseTemplates, string baseTemplateNameOrId)
        {
            var templateFound = false;
            foreach (var baseTemplateItem in baseTemplates.Where(baseTemplateItem => !IsNull(baseTemplateItem) && !baseTemplateItem.ID.IsNull))
            {
                templateFound = IsMatchOnTemplateNameOrId(baseTemplateNameOrId, baseTemplateItem) || HasBaseTemplateHelper(baseTemplateItem.BaseTemplates, baseTemplateNameOrId);

                if (templateFound)
                {
                    break;
                }
            }
            return templateFound;
        }

        private static bool IsMatchOnTemplateNameOrId(string baseTemplateNameOrId, TemplateItem baseTemplateItem)
        {
            return baseTemplateItem.ID.Guid.ToString("B").Equals(baseTemplateNameOrId, StringComparison.OrdinalIgnoreCase) ||
                   baseTemplateItem.FullName.Equals(baseTemplateNameOrId, StringComparison.OrdinalIgnoreCase) ||
                   baseTemplateItem.Name.Equals(baseTemplateNameOrId, StringComparison.OrdinalIgnoreCase);
        }

        public static ItemList GetChildrenUsingTemplate(this Item currentItem, string baseTemplateNameOrId)
        {
            var children = currentItem.Children;
            var childList = new ItemList();
            if (string.IsNullOrWhiteSpace(baseTemplateNameOrId))
            {
                return childList;
            }
            foreach (Item child in children)
            {
                if (child.HasBaseTemplate(baseTemplateNameOrId))
                {
                    childList.Add(child);
                }
            }
            return childList;
        }

        public static List<Item> GetChildrenSafe(this Item i)
        {
            return i != null ? i.GetChildren().Where(e => e.TemplateID != IData_FolderConstants.TemplateId).ToList() : new List<Item>();
        }

        public static Item GetFirstParentOfTemplate(this Item currentItem, string baseTemplateNameOrId)
        {
            if (currentItem == null)
            {
                return null;
            }
            if (IsMatchOnTemplateNameOrId(baseTemplateNameOrId, currentItem.Template))
            {
                return currentItem;
            }
            var parent = currentItem.Parent;
            while (parent != null && IsMatchOnTemplateNameOrId(baseTemplateNameOrId, parent.Template) == false)
            {
                parent = parent.Parent;
            }
            return parent;
        }

        public static Item GetFirstParentWithBaseTemplate(this Item currentItem, string baseTemplateNameOrId)
        {
            if (currentItem == null)
            {
                return null;
            }
            if (currentItem.HasBaseTemplate(baseTemplateNameOrId))
            {
                return currentItem;
            }
            var parent = currentItem.Parent;
            while (parent != null && !parent.HasBaseTemplate(baseTemplateNameOrId))
            {
                parent = parent.Parent;
            }
            return parent;
        }

        public static int RenderingsInDynamicPlaceholder(this Item item, string placeholder)
        {
            var regex = new Regex($"/{placeholder}" + @"_\w{8}(?:-\w{4}){3}-\w{12}$", RegexOptions.IgnoreCase);

            var renderingReferences = item.Visualization.GetRenderings(Context.Device, true);
            var renderingsInPlaceholder = renderingReferences.Where(r => regex.IsMatch(r.Placeholder));
            return renderingsInPlaceholder.Count();
        }

        public static bool InExperienceEditorOrHasValue(this Item i, string fieldName)
        {
            return (Context.PageMode.IsExperienceEditor || !string.IsNullOrEmpty(i.GetFieldValue(fieldName)));
        }
    }
}
