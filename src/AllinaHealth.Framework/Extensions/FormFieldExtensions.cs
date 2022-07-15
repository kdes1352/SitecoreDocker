using Sitecore.ExperienceForms.Mvc.Models.Fields;

namespace AllinaHealth.Framework.Extensions
{
    public static class FormFieldExtensions
    {
        public static string IsFormFieldRequired(this StringInputViewModel model)
        {

            if (model.Required)
            {

            }

            return string.Empty;
        }
    }
}