using System.IO;
using System.Web.UI;
using AllinaHealth.Models.Extensions;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.Shell.Applications.ContentEditor.Pipelines.RenderContentEditorHeader;

namespace AllinaHealth.Framework.Pipelines.RenderContentEditorHeader
{
    public class AddLinks
    {
        public AddLinks()
        {
        }

        public void Process(RenderContentEditorHeaderArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            var item = args.Item;

            if (item.IsNull())
            {
                return;
            }

            if (item.Visualization.Layout != null)
            {
                using (var htmlTextWriter = new HtmlTextWriter(new StringWriter()))
                {
                    var options = LinkManager.GetDefaultUrlOptions();
                    options.AlwaysIncludeServerUrl = true;
                    options.SiteResolving = true;
                    var url = LinkManager.GetItemUrl(item, options);

                    var hasPreview = !url.Contains(".dx-dev.") && !url.Contains("local."); // Sitecore.Configuration.Factory.GetDatabase("preview", false) != null;

                    var padding = "0px";
                    if (!hasPreview)
                    {
                        padding = "9px";
                    }

                    htmlTextWriter.Write("<div class=\"scEditorHeaderTitlePanel\" style=\"margin-left: 20px; padding-top: " + padding + ";\">");

                    if (hasPreview)
                    {
                        htmlTextWriter.Write($"<strong><a href=\"{url}\" target=\"_blank\">Preview Url</a></strong><br />");
                    }

                    url = ModifyUrlForEnvironment(url, hasPreview);

                    htmlTextWriter.Write($"<strong><a href=\"{url}\" target=\"_blank\">Live Url</a></strong>");

                    htmlTextWriter.Write("</div>");
                    args.EditorFormatter.AddLiteralControl(args.Parent, htmlTextWriter.InnerWriter.ToString());
                }
            }
            else if (item.Paths.IsMediaItem && !string.IsNullOrEmpty(item.GetFieldValue("Extension")))
            {
                using (var htmlTextWriter = new HtmlTextWriter(new StringWriter()))
                {
                    var mediaOptions = new MediaUrlOptions { AlwaysIncludeServerUrl = true };
                    var url = MediaManager.GetMediaUrl(item, mediaOptions);

                    var hasPreview = !url.Contains(".dx-dev.") && !url.Contains("local."); // Sitecore.Configuration.Factory.GetDatabase("preview", false) != null;

                    url = ModifyUrlForEnvironment(url, hasPreview);
                    if (url.Contains("sitecore/shell/"))
                    {
                        url = url.Replace("sitecore/shell/", "");
                    }

                    htmlTextWriter.Write("<div class=\"scEditorHeaderTitlePanel\" style=\"margin-left: 20px; padding-top: 9px;\">");
                    htmlTextWriter.Write($"<strong><a href=\"{url}\" target=\"_blank\">Media Url</a></strong>");
                    htmlTextWriter.Write("</div>");
                    args.EditorFormatter.AddLiteralControl(args.Parent, htmlTextWriter.InnerWriter.ToString());
                }
            }
        }

        private static string ModifyUrlForEnvironment(string url, bool hasPreview)
        {
            if (url.Contains("dx-cm-stg."))
            {
                url = url.Replace("dx-cm-stg.", "sc.dx-stg.");
            }

            if (url.Contains("cm.") && hasPreview)
            {
                url = url.Replace("cm.", "www.");
            }

            return url;
        }
    }
}