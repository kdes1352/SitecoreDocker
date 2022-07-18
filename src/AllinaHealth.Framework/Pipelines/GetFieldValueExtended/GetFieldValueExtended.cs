using System.Text.RegularExpressions;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Pipelines.RenderField;

namespace AllinaHealth.Framework.Pipelines.GetFieldValueExtended
{
    public class GetFieldValueExtended
    {
        private const string AltToken = "{{alt}}";
        private const string TitleToken = "{{title}}";

        public void Process(RenderFieldArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Item, "args.Item");
            Assert.ArgumentNotNull(args.Item.Database, "args.Item.Database");
            Assert.ArgumentNotNull(args.GetField(), "args.GetField()");

            if (!args.Item.Database.Name.Equals("web"))
            {
                return;
            }

            if (args.GetField().Value.Contains(AltToken) || args.GetField().Value.Contains(TitleToken))
            {
                args.Result.FirstPart = ReplaceTokens(args.GetField().Value);
            }
        }

        private static string ReplaceTokens(string fieldValue)
        {
            if (fieldValue.Contains("img") == false) return fieldValue;

            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(fieldValue);

            foreach (var img in document.DocumentNode.Descendants("img"))
            {
                var imgSrc = img.Attributes["src"];
                if (imgSrc == null) continue;

                // Remove query string from src (if it exists)
                var mediaUrl = CleanImageSource(imgSrc.Value);

                // Get media item ID based on media url
                if (!DynamicLink.TryParse(mediaUrl, out var dynamicLink)) continue;
                MediaItem mediaItem = Sitecore.Context.Database.GetItem(dynamicLink.ItemId,
                    dynamicLink.Language ?? Sitecore.Context.Language);

                // Replace the tokens
                if (img.Attributes["alt"] != null && img.Attributes["alt"].Value.Trim() == AltToken)
                {
                    img.Attributes["alt"].Value = mediaItem.Alt;
                }

                if (img.Attributes["title"] != null && img.Attributes["title"].Value.Trim() == TitleToken)
                {
                    img.Attributes["title"].Value = mediaItem.Title;
                }
            }

            return document.DocumentNode.OuterHtml;
        }

        private static string CleanImageSource(string imgSrcValue)
        {
            // First match the SHA1
            var shaMatcher = new Regex("\b[0-9a-f]{5,40}\b");
            var match = shaMatcher.Match(imgSrcValue);

            return match.Success ? string.Format("/~/media/{0}.ashx", match.Value) : imgSrcValue;
        }
    }
}