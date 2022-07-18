using System.Linq;
using System.Text;
using AllinaHealth.Framework.Validation;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Pipelines.Save;

namespace AllinaHealth.Framework.Pipelines.Save
{
    public class CheckLinks
    {
        public void Process(SaveArgs args)
        {
            if (!args.HasSheerUI)
                return;
            if (args.Result == "no" || args.Result == "undefined")
            {
                args.AbortPipeline();
            }
            else
            {
                var num = 0;
                if (args.Parameters["LinkIndex"] == null)
                    args.Parameters["LinkIndex"] = "0";
                else
                    num = MainUtil.GetInt(args.Parameters["LinkIndex"], 0);
                for (var index = 0; index < args.Items.Length; ++index)
                {
                    if (index < num)
                    {
                        continue;
                    }

                    ++num;
                    var saveItem = args.Items[index];
                    var obj = Context.ContentDatabase.Items[saveItem.ID, saveItem.Language, saveItem.Version];
                    if (obj == null)
                    {
                        continue;
                    }

                    obj.Editing.BeginEdit();
                    foreach (var field1 in saveItem.Fields)
                    {
                        var field2 = obj.Fields[field1.ID];
                        if (field2 != null)
                            field2.Value = string.IsNullOrEmpty(field1.Value) ? null : field1.Value;
                    }

                    var brokenLinks = BrokenLinkValidator.GetBrokenLinks(obj);
                    if (brokenLinks.Length != 0)
                    {
                        ShowDialog(obj, brokenLinks);
                        args.WaitForPostBack();
                        break;
                    }

                    obj.Editing.CancelEdit();
                }

                args.Parameters["LinkIndex"] = num.ToString();
            }
        }

        private static void ShowDialog(Item item, ItemLink[] links)
        {
            var stringBuilder = new StringBuilder(Translate.Text("The item \"{0}\" contains broken links in these fields:\n\n", item.DisplayName));
            var flag = false;
            if (links.Any())
            {
                stringBuilder.Append("<table style='word-break:break-all;'>");
                stringBuilder.Append("<tbody>");
                foreach (var link in links)
                {
                    if (!link.SourceFieldID.IsNull)
                    {
                        stringBuilder.Append("<tr>");
                        stringBuilder.Append("<td style='width:70px;vertical-align:top;padding-bottom:5px;padding-right:5px;'>");
                        stringBuilder.Append(item.Fields.Contains(link.SourceFieldID) ? item.Fields[link.SourceFieldID].DisplayName : Translate.Text("[Unknown field: {0}]", link.SourceFieldID.ToString()));
                        stringBuilder.Append("</td>");
                        stringBuilder.Append("<td style='vertical-align:top;padding-bottom:5px;'>");
                        if (!string.IsNullOrEmpty(link.TargetPath) && !ID.IsID(link.TargetPath))
                            stringBuilder.Append(link.TargetPath);
                        stringBuilder.Append("</td>");
                        stringBuilder.Append("</tr>");
                    }
                    else
                        flag = true;
                }

                stringBuilder.Append("</tbody></table>");
            }

            if (flag)
            {
                stringBuilder.Append("<br />");
                stringBuilder.Append(Translate.Text("The template or branch for this item is missing."));
            }

            stringBuilder.Append("<br />");
            stringBuilder.Append(Translate.Text("Do you want to save anyway?"));
            Context.ClientPage.ClientResponse.Confirm(stringBuilder.ToString());
        }
    }
}