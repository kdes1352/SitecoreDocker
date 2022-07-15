using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using AllinaHealth.Models.Forms;
using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using Sitecore.Globalization;

namespace AllinaHealth.Framework.FormActions
{
    public class SendEmailAction : SubmitActionBase<SendMailActionData>
    {
        private readonly string _keywordPrefix = Translate.TextByLanguage("Forms.Actions.SendMail.KeywordPrefix", Context.Language, "{");
        private readonly string _keywordSuffix = Translate.TextByLanguage("Forms.Actions.SendMail.KeywordSuffix", Context.Language, "}");

        public SendEmailAction(ISubmitActionData submitActionData) : base(submitActionData)
        {
        }

        protected override bool Execute(SendMailActionData data, FormSubmitContext formSubmitContext)
        {
            Assert.ArgumentNotNull(formSubmitContext, nameof(formSubmitContext));
            if (data == null || !(data.ReferenceId != Guid.Empty))
                return false;
            var item = Context.Database.GetItem(new ID(data.ReferenceId));
            if (item == null)
                return false;

            try
            {
                var emailTemplate = new EmailTemplate(item);
                var emailMessage =
                    new MailMessage
                    {
                        // Subject
                        Subject = ReplaceKeywords(emailTemplate.Subject, formSubmitContext),
                        // From
                        From = new MailAddress(ReplaceKeywords(emailTemplate.From, formSubmitContext))
                    };
                // To
                FillMailAddressCollection(SplitEmails(ReplaceKeywords(emailTemplate.To, formSubmitContext)), emailMessage.To);
                // CC
                FillMailAddressCollection(SplitEmails(ReplaceKeywords(emailTemplate.Cc, formSubmitContext)), emailMessage.CC);
                // BCC
                FillMailAddressCollection(SplitEmails(ReplaceKeywords(emailTemplate.Bcc, formSubmitContext)), emailMessage.Bcc);
                // Text
                if (!string.IsNullOrEmpty(emailTemplate.MessageRichText))
                {
                    emailMessage.Body = ReplaceKeywords(emailTemplate.MessageRichText, formSubmitContext);
                    emailMessage.IsBodyHtml = true;
                }
                else
                {
                    emailMessage.Body = ReplaceKeywords(emailTemplate.MessageText, formSubmitContext);
                    emailMessage.IsBodyHtml = false;
                }

                MainUtil.SendMail(emailMessage);
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"[SendMail Action] Error sending e-mail based on template {data.ReferenceId}", e, this);
                formSubmitContext.Abort();
                return false;
            }
        }

        protected void FillMailAddressCollection(string[] emails, MailAddressCollection collection)
        {
            foreach (var email in emails)
                collection.Add(email);
        }

        protected string[] SplitEmails(string text)
        {
            return text
                .Split(Translate.TextByLanguage("Forms.Actions.SendMail.EmailListSeparator", Context.Language, ",")
                    .ToCharArray()).Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToArray();
        }

        protected string ReplaceKeywords(string original, FormSubmitContext formSubmitContext)
        {
            var returnString = original;
            foreach (var viewModel in formSubmitContext.Fields)
            {
                var type = viewModel.GetType();
                string valueToReplace;

                // InputViewModel<string> types
                if (type.IsSubclassOf(typeof(InputViewModel<string>)))
                {
                    var field = (InputViewModel<string>)viewModel;
                    valueToReplace = field.Value;
                }
                // InputViewModel<List<string>> types
                else if (type.IsSubclassOf(typeof(InputViewModel<List<string>>)))
                {
                    var field = (InputViewModel<List<string>>)viewModel;
                    //valueToReplace =
                    //    string.Join(
                    //        Translate.TextByLanguage("Forms.Actions.SendMail.ListSeparator", Context.Language, ", "),
                    //        field.Value);
                    valueToReplace =
                        string.Join(
                            Translate.TextByLanguage("Forms.Actions.SendMail.ListSeparator", Context.Language, ", "),
                            (field.Value ?? new List<string>()));
                }
                // InputViewModel<bool> types
                else if (type.IsSubclassOf(typeof(InputViewModel<bool>)))
                {
                    var field = (InputViewModel<bool>)viewModel;
                    valueToReplace = field.Value
                        ? Translate.Text("Forms.Actions.SendMail.CheckboxCheckedText")
                        : Translate.Text("Forms.Actions.SendMail.CheckboxUncheckedText");
                }
                // InputViewModel<DateTime?> types
                else if (type.IsSubclassOf(typeof(InputViewModel<DateTime?>)))
                {
                    var field = (InputViewModel<DateTime?>)viewModel;
                    valueToReplace = field.Value?.ToString(Translate.Text("Forms.Actions.SendMail.DateFormatMask")) ??
                                     Translate.Text("Forms.Actions.SendMail.CheckboxUncheckedText");
                }
                // InputViewModel<DateTime> types
                else if (type.IsSubclassOf(typeof(InputViewModel<DateTime>)))
                {
                    var field = (InputViewModel<DateTime>)viewModel;
                    valueToReplace = field.Value.ToString(Translate.Text("Forms.Actions.SendMail.DateFormatMask"));
                }
                // InputViewModel<double?> types
                else if (type.IsSubclassOf(typeof(InputViewModel<double?>)))
                {
                    var field = (InputViewModel<double?>)viewModel;
                    valueToReplace = field.Value?.ToString(Translate.Text("Forms.Actions.SendMail.DoubleFormatMask"));
                }
                else
                {
                    valueToReplace =
                        string.Format(Translate.Text("Forms.Actions.SendMail.UnsupportedFieldTypeErrorMessage"),
                            type.FullName);
                }

                returnString = returnString.Replace($"{_keywordPrefix}{viewModel.Name}{_keywordSuffix}", valueToReplace);
            }

            return returnString;
        }
    }
}