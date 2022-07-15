using System;
using System.Text;
using Sitecore.Data.Items;

namespace AllinaHealth.Models.Extensions
{
    public static class NewsBlogItemExtensions
    {
        public static string GetNewsSourceStr(this Item item)
        {
            if (null == item)
            {
                return "";
            }

            //Build the source string
            var srcStr = "";
            Sitecore.Data.Fields.ReferenceField srcRefField = item.Fields["Source"];
            var sourceItem = srcRefField?.TargetItem;
            if (sourceItem == null) return srcStr;
            var srcName = sourceItem.Fields["Source Name"].Value;
            var sourceDateObj = item.GetDate("Source Date");
            var srcDateStr = sourceDateObj.ToString("MMMM dd, yyyy");
            srcStr = $"[{srcName}, {srcDateStr}]";
            return srcStr;
        }

        public static string GetAbbreviatedMetaDescription(this Item item, int numberWords = 25)
        {
            if (null == item)
            {
                return "";
            }

            var parts = item.GetFieldValue("Meta Description")
                .GetWords(numberWords, null, StringSplitOptions.RemoveEmptyEntries);
            var builder = new StringBuilder();
            var hasFirstWord = false;
            foreach (var w in parts)
            {
                if (hasFirstWord)
                {
                    builder.Append(" ");
                }

                builder.Append(w);
                hasFirstWord = true;
            }

            if (parts.Length == numberWords)
            {
                builder.Append(" [...]");
            }

            return builder.ToString();
        }

        public static string[] GetWords(
            this string input,
            int count = -1,
            string[] wordDelimiter = null,
            StringSplitOptions options = StringSplitOptions.None)
        {
            if (string.IsNullOrEmpty(input)) return new string[] { };

            if (count < 0)
                return input.Split(wordDelimiter, options);

            var words = input.Split(wordDelimiter, count + 1, options);
            if (words.Length <= count)
                return words; // not so many words found

            // remove last "word" since that contains the rest of the string
            Array.Resize(ref words, words.Length - 1);

            return words;
        }
    }
}