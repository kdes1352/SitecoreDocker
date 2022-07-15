using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AllinaHealth.Models.Extensions;

namespace AllinaHealth.Models.MigrationModels
{
    public class EktronAccordion : AEktronMigrationModel<EktronAccordion>
    {
        public string Header { get; set; }
        public string Summary { get; set; }
        public List<EktronContentGroup> ContentGroup { get; set; }

        public EktronAccordion()
        {

        }

        public override EktronAccordion FromXmlString(EktronInfo ei)
        {
            var doc = XDocument.Parse(ei.Data);

            var elements = doc.Descendants("AccordionContents").ToList();

            return new EktronAccordion
            {
                Header = elements[0].Element("Header").ToStringWtihHtml(),
                Summary = elements[0].Element("Summary").ToStringWtihHtml(),
                ContentGroup = elements[0].Elements("ContentGroup").Select(e => new EktronContentGroup
                {
                    LinkText = e.Element("LinkText").ToStringWtihHtml(),
                    Content = e.Element("Content").ToStringWtihHtml()
                }).ToList()
            };
        }
    }

    public class EktronContentGroup
    {
        public string LinkText { get; set; }
        public string Content { get; set; }
    }
}