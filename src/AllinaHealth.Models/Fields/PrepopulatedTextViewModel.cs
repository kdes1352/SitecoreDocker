using System;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Mvc.Models.Fields;

namespace AllinaHealth.Models.Fields
{
    [Serializable]
    public class PrepopulatedTextViewModel : StringInputViewModel
    {
        public string QueryStringParameter { get; set; }

        protected override void InitItemProperties(Item item)
        {
            Assert.ArgumentNotNull(item, nameof(item));
            base.InitItemProperties(item);
            QueryStringParameter = StringUtil.GetString(item.Fields["Query String Parameter"]);
        }

        protected override void UpdateItemFields(Item item)
        {
            Assert.ArgumentNotNull(item, nameof(item));
            base.UpdateItemFields(item);
            item.Fields["Query String Parameter"]?.SetValue(QueryStringParameter, true);
        }
    }
}