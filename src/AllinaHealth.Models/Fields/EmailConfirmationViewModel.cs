using System;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using Sitecore.ExperienceForms.Mvc.Models.Validation;

namespace AllinaHealth.Models.Fields
{
    [Serializable]
    public class EmailConfirmationViewModel : StringInputViewModel
    {
        public string ConfirmEmailLabel { get; set; }

        public string ConfirmEmailPlaceholder { get; set; }

        [LocalizedCompare("Value", ErrorMessage = "Emails do not match.")]
        public string ConfirmEmail { get; set; }

        protected override void InitItemProperties(Item item)
        {
            Assert.ArgumentNotNull(item, nameof(item));
            base.InitItemProperties(item);
            ConfirmEmailLabel = StringUtil.GetString(item.Fields["Confirm Email Label"]);
            ConfirmEmailPlaceholder = StringUtil.GetString(item.Fields["Confirm Email Placeholder"]);
        }

        protected override void UpdateItemFields(Item item)
        {
            Assert.ArgumentNotNull(item, nameof(item));
            base.UpdateItemFields(item);
            item.Fields["Confirm Email Label"]?.SetValue(ConfirmEmailLabel, true);
            item.Fields["Confirm Email Placeholder"]?.SetValue(ConfirmEmailPlaceholder, true);
        }
    }
}