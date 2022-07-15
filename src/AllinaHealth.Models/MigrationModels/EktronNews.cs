using System;
using System.Linq;
using System.Xml.Linq;
using AllinaHealth.Models.Extensions;

namespace AllinaHealth.Models.MigrationModels
{
    public class EktronNews
    {
        private bool _isFromXml = false;
        public string ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Teaser { get; set; }
        public string DateCreated { get; set; }
        public string Header { get; set; }
        public string Location { get; set; }
        public string NewsContent { get; set; }
        public string NewsRegion { get; set; }
        public string Date { get; set; }
        public string PhotoSrc { get; set; }
        public string PhotoAlt { get; set; }
        public string PhotoCaption { get; set; }
        public string ContactInformation { get; set; }
        public string ContactEmail { get; set; }

        public DateTime DateValue => DateTime.TryParse(Date, out var dt) ? dt : new DateTime();

        public bool IsFromXml => _isFromXml;

        public void FromXmlString()
        {
            if (!Content.StartsWith("<root>"))
            {
                return;
            }

            var doc = XDocument.Parse(Content);
            var elements = doc.Descendants("News").ToList();

            Header = elements[0].Element("Header").ToStringWtihHtml();
            Location = elements[0].Element("Location").ToStringWtihHtml();
            NewsRegion = elements[0].Element("NewsRegion").ToStringWtihHtml();
            if (NewsRegion.Equals("choose regions", StringComparison.InvariantCultureIgnoreCase))
            {
                NewsRegion = string.Empty;
            }
            Date = elements[0].Element("Date").ToStringWtihHtml();
            NewsContent = elements[0].Element("NewsContent").ToStringWtihHtml();
            var photo = elements[0].Element("Photo");
            var img = photo?.Element("img");
            if (img != null)
            {
                PhotoSrc = elements[0]?.Element("Photo")?.Element("img")?.Attribute("src")?.Value;
                PhotoAlt = elements[0]?.Element("Photo")?.Element("img")?.Attribute("alt")?.Value;
            }
            PhotoCaption = elements[0].Element("PhotoCaption").ToStringWtihHtml();
            var contact = elements[0].Element("ContactInformation");
            if (contact != null)
            {
                var mailto = "mailto:";
                ContactInformation = contact.ToStringWtihHtml();
                var contactAnchor = contact.Element("a");
                if (contactAnchor != null)
                {
                    ContactEmail = contactAnchor.Attribute("href")?.Value.ToLower().Replace(mailto, string.Empty).Trim();
                }
                else
                {
                    contactAnchor = contact.Descendants().FirstOrDefault(e => e.Name == "a");
                    var href = contactAnchor?.Attribute("href");
                    if (href != null)
                    {
                        ContactEmail = href.Value.ToLower();

                        if (ContactEmail.StartsWith("<a") || ContactEmail.StartsWith("&lt;a"))
                        {
                            var pos = ContactEmail.IndexOf(mailto, StringComparison.Ordinal) + mailto.Length;
                            ContactEmail = ContactEmail.Substring(pos);
                            ContactEmail = ContactEmail.Substring(0, ContactEmail.IndexOf("\"", StringComparison.Ordinal));
                        }

                        ContactEmail = ContactEmail.Replace(mailto, string.Empty).Trim();
                    }
                }

                if (ContactEmail == "http://")
                {
                    ContactEmail = string.Empty;
                }
                else if (ContactEmail == "timotny.burke@allina.com")
                {
                    ContactEmail = "timothy.burke@allina.com";
                }
                else if (ContactEmail != null && ContactEmail.StartsWith("<a"))
                {
                    ContactEmail = ContactEmail.Substring(ContactEmail.IndexOf("\"" + 1, StringComparison.Ordinal));
                    ContactEmail = ContactEmail.Substring(0, ContactEmail.IndexOf("\"", StringComparison.Ordinal));
                }
            }
            _isFromXml = true;
        }
    }
}
