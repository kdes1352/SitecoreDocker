using AllinaHealth.Models.Extensions;
using Sitecore.Data.Items;

namespace AllinaHealth.Models.Forms
{
    public class EmailTemplate
    {
        public string Subject { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string MessageRichText { get; set; }
        public string MessageText { get; set; }

        public EmailTemplate(Item i)
        {
            Subject = i.GetFieldValue("Subject");
            To = i.GetFieldValue("To");
            From = i.GetFieldValue("From");
            Cc = i.GetFieldValue("Cc");
            Bcc = i.GetFieldValue("Bcc");
            MessageRichText = i.GetFieldValue("Message Rich Text");
            MessageText = i.GetFieldValue("Message Text");
        }
    }
}