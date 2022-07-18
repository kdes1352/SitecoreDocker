using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using AllinaHealth.Framework.Pipelines.GetRenderingDatasource;
using AllinaHealth.Models.Extensions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Validators;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Text;

namespace AllinaHealth.Framework.Validation
{
    [Serializable]
    public class BrokenLinkValidator : StandardValidator
    {
        public BrokenLinkValidator()
        {
        }

        public BrokenLinkValidator(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string Name => "Broken Links";

        protected override ValidatorResult Evaluate()
        {
            var obj = GetItem();
            if (obj == null)
            {
                return ValidatorResult.Valid;
            }

            var brokenLinks = GetBrokenLinks(obj);
            if (brokenLinks.Length == 0)
            {
                return ValidatorResult.Valid;
            }

            var stringBuilder = new StringBuilder();
            foreach (var itemLink in brokenLinks)
            {
                stringBuilder.Append("\n");
                var sourceFieldId = itemLink.SourceFieldID;

                if (ID.IsNullOrEmpty(sourceFieldId))
                {
                    stringBuilder.Append(Translate.Text("Template or branch"));
                }
                else
                {
                    var field = obj.Fields[sourceFieldId];
                    stringBuilder.Append(field == null ? Translate.Text("[Unknown field]") : field.DisplayName);
                }
            }

            Text = GetText("This item contains broken links in:{0}", stringBuilder.ToString());
            return GetFailedResult(ValidatorResult.Warning);
        }

        protected override ValidatorResult GetMaxValidatorResult()
        {
            return GetFailedResult(ValidatorResult.Warning);
        }

        public static ItemLink[] GetBrokenLinks(Item obj, bool allVersions = false)
        {
            var brokenItemLinkList = new List<ItemLink>();
            var datasourceLocationField = obj.Fields["Datasource Location"];
            foreach (var link in obj.Links.GetBrokenLinks(allVersions))
            {
                if (datasourceLocationField != null && link.SourceFieldID == datasourceLocationField.ID)
                {
                    var ls = new ListString(datasourceLocationField.Value);
                    brokenItemLinkList.AddRange(from s in ls where !s.StartsWith(GetCodeDatasourceLocations.Token) && !s.StartsWith(GetSiteRootDatasourceLocations.Token) && !s.StartsWith("query:") where ID.IsID(s) select obj.Database.GetItem(new ID(s)) into item where item.IsNull() select link);
                }
                else
                {
                    brokenItemLinkList.Add(link);
                }
            }

            return brokenItemLinkList.ToArray();
        }
    }
}