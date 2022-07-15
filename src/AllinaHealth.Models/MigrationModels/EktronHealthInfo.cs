using System.Linq;
using System.Xml.Linq;
using AllinaHealth.Models.Extensions;

namespace AllinaHealth.Models.MigrationModels
{
    public class EktronHealthInfo : AEktronMigrationModel<EktronHealthInfo>
    {
        public string Content { get; set; }
        public string Source { get; set; }
        public string ReviewedBy { get; set; }
        public string FirstPublished { get; set; }
        public string LastReviewed { get; set; }

        public EktronHealthInfo()
        {

        }

        public override EktronHealthInfo FromXmlString(EktronInfo ei)
        {
            var doc = XDocument.Parse(ei.Data);

            var elements = doc.Descendants("HealthInformation").ToList();

            return new EktronHealthInfo
            {
                Content = elements[0].Element("Content").ToStringWtihHtml(),
                Source = elements[0]?.Element("Citation")?.Element("Source").ToStringWtihHtml(),
                ReviewedBy = elements[0]?.Element("Citation")?.Element("ReviewedBy").ToStringWtihHtml(),
                FirstPublished = elements[0]?.Element("Citation")?.Element("FirstPublished").ToStringWtihHtml(),
                LastReviewed = elements[0]?.Element("Citation")?.Element("LastReviewed").ToStringWtihHtml()
            };
        }
    }
}
