using System.Linq;
using System.Xml.Linq;
using AllinaHealth.Models.Extensions;

namespace AllinaHealth.Models.MigrationModels
{
    public class EktronGoogleMap : AEktronMigrationModel<EktronGoogleMap>
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string ZoomLevel { get; set; }
        public string Pin { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string HideAddress { get; set; }
        public string HideMap { get; set; }
        public EktronGroupLinks GroupLinks { get; set; }

        public EktronGoogleMap()
        {

        }

        public override EktronGoogleMap FromXmlString(EktronInfo ei)
        {
            var doc = XDocument.Parse(ei.Data);

            var elements = doc.Descendants("root").ToList();

            return new EktronGoogleMap
            {
                Address1 = elements[0].Element("Address1").ToStringWtihHtml(),
                Address2 = elements[0].Element("Address2").ToStringWtihHtml(),
                City = elements[0].Element("City").ToStringWtihHtml(),
                State = elements[0].Element("State").ToStringWtihHtml(),
                ZipCode = elements[0].Element("ZipCode").ToStringWtihHtml(),
                Phone = elements[0].Element("Phone").ToStringWtihHtml(),
                Fax = elements[0].Element("Fax").ToStringWtihHtml(),
                Height = elements[0].Element("Height").ToStringWtihHtml(),
                Width = elements[0].Element("Width").ToStringWtihHtml(),
                ZoomLevel = elements[0].Element("ZoomLevel").ToStringWtihHtml(),
                Pin = elements[0].Element("Pin").ToStringWtihHtml(),
                Longitude = elements[0].Element("Longitude").ToStringWtihHtml(),
                HideAddress = elements[0].Element("HideAddress").ToStringWtihHtml(),
                HideMap = elements[0].Element("HideMap").ToStringWtihHtml(),
                GroupLinks = new EktronGroupLinks
                {
                    ContactUsLinkName = elements[0]?.Element("GroupLinks")?.Element("ContactUsLinkName").ToStringWtihHtml(),
                    ContactUsLink = elements[0]?.Element("GroupLinks")?.Element("ContactUsLink").ToStringWtihHtml()
                }
            };
        }
    }

    public class EktronGroupLinks
    {
        public string ContactUsLinkName { get; set; }
        public string ContactUsLink { get; set; }
    }
}
