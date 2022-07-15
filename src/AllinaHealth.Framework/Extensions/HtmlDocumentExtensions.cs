using System;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace AllinaHealth.Framework.Extensions
{
    public static class HtmlDocumentExtensions
    {
        public static string GetAllInnerTexts(this HtmlDocument doc)
        {
            if (doc == null)
            {
                return string.Empty;
            }

            var nodes = doc.DocumentNode.DescendantsAndSelf();
            var list = (from n in nodes where !n.HasChildNodes && !HasAncestorType(n, HtmlNodeType.Comment) && !HasAncestor(n, "script") && !HasAncestor(n, "style") && !HasAncestor(n, "button") select n.InnerText.Replace(Environment.NewLine, " ").Trim() into value where !string.IsNullOrEmpty(value) select value).ToList();
            //return RemoveWhitespace(string.Join(" ", list).Trim().ToLowerInvariant());
            return RemoveWhitespace(string.Join(" ", list).Trim());
        }

        public static string GetAllInnerTexts(this HtmlNode node, bool toLower = false)
        {
            if (node == null)
            {
                return string.Empty;
            }

            var nodes = node.Descendants();
            var list = (from n in nodes where !n.HasChildNodes && !HasAncestorType(n, HtmlNodeType.Comment) && !HasAncestor(n, "script") && !HasAncestor(n, "style") && !HasAncestor(n, "button") select n.InnerText.Replace(Environment.NewLine, " ").Trim() into value where !string.IsNullOrEmpty(value) select value).ToList();

            return RemoveWhitespace(toLower ? string.Join(" ", list).Trim().ToLowerInvariant() : string.Join(" ", list).Trim());
        }

        private static bool HasAncestor(HtmlNode n, string name)
        {
            if (n == null)
            {
                return false;
            }

            return n.Name == name || HasAncestor(n.ParentNode, name);
        }

        private static bool HasAncestorType(HtmlNode n, HtmlNodeType nodeType)
        {
            if (n == null)
            {
                return false;
            }

            return n.NodeType == nodeType || HasAncestorType(n.ParentNode, nodeType);
        }

        private static string RemoveWhitespace(string inputStr)
        {
            const int n = 5;
            var tmpbuilder = new StringBuilder(inputStr.Length);
            for (var i = 0; i < n; ++i)
            {
                var inspaces = false;
                tmpbuilder.Length = 0;
                foreach (var c in inputStr)
                {
                    if (inspaces)
                    {
                        if (c == ' ') continue;
                        inspaces = false;
                        tmpbuilder.Append(c);
                    }
                    else if (c == ' ')
                    {
                        inspaces = true;
                        tmpbuilder.Append(' ');
                    }
                    else
                    {
                        tmpbuilder.Append(c);
                    }
                }
            }

            return tmpbuilder.ToString();
        }
    }
}