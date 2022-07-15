using static System.FormattableString;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;

namespace AllinaHealth.Framework.FormActions
{
    /// <summary>
    /// Executes a submit action for logging the form submit status.
    /// </summary>
    /// <seealso cref="Sitecore.ExperienceForms.Processing.Actions.SubmitActionBase{TParametersData}" />
    public class TestEmail : SubmitActionBase<string>
    {
        // ReSharper disable once InvalidXmlDocComment
        /// Initializes a new instance of the <see cref="LogSubmit"/> class.
        // ReSharper disable once InvalidXmlDocComment
        /// </summary>
        /// <param name="submitActionData">The submit action data.</param>
        public TestEmail(ISubmitActionData submitActionData) : base(submitActionData)
        {
        }

        /// <summary>
        /// Tries to convert the specified <paramref name="value" /> to an instance of the specified target type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="target">The target object.</param>
        /// <returns>
        /// true if <paramref name="value" /> was converted successfully; otherwise, false.
        /// </returns>
        protected override bool TryParse(string value, out string target)
        {
            target = string.Empty;
            return true;
        }

        /// <summary>
        /// Executes the action with the specified <paramref name="data" />.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="formSubmitContext">The form submit context.</param>
        /// <returns>
        ///   <c>true</c> if the action is executed correctly; otherwise <c>false</c>
        /// </returns>
        protected override bool Execute(string data, FormSubmitContext formSubmitContext)
        {
            Assert.ArgumentNotNull(formSubmitContext, nameof(formSubmitContext));

            if (!formSubmitContext.HasErrors)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("<p>Test email from Site Core form</p>");

                var emailMessage = new MailMessage("allina.webmaster@allina.com", "keven.swanson@allina.com", "Test Email From Site Core Form", sb.ToString());
                emailMessage.IsBodyHtml = true;
                MainUtil.SendMail(emailMessage);
            }
            else
            {
                Logger.Warn(Invariant($"Form {formSubmitContext.FormId} submitted with errors: {string.Join(", ", formSubmitContext.Errors.Select(t => t.ErrorMessage))}."), this);
            }

            return true;
        }
    }
}