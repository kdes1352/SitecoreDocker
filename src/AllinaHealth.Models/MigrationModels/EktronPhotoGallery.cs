using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AllinaHealth.Models.Extensions;

namespace AllinaHealth.Models.MigrationModels
{
    public class EktronPhotoGallery : AEktronMigrationModel<EktronPhotoGallery>
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public List<EktronGallery> Gallery { get; set; }

        public EktronPhotoGallery()
        {

        }

        public override EktronPhotoGallery FromXmlString(EktronInfo ei)
        {
            var doc = XDocument.Parse(ei.Data);

            var elements = doc.Descendants("PhotoGallery").ToList();

            return new EktronPhotoGallery
            {
                Title = elements[0].Element("Title").ToStringWtihHtml(),
                Summary = elements[0].Element("Summary").ToStringWtihHtml(),
                Gallery = elements[0].Elements("Gallery").Select(e => new EktronGallery
                {
                    PhotoThumbnail = e.Element("PhotoThumbnail").ToStringWtihHtml(),
                    PhotoLarge = e.Element("PhotoLarge").ToStringWtihHtml(),
                    Caption = e.Element("Caption").ToStringWtihHtml()
                }).ToList()
            };
        }
    }

    public class EktronGallery
    {
        public string PhotoThumbnail { get; set; }
        public string PhotoLarge { get; set; }
        public string Caption { get; set; }
    }
}