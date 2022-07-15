using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AllinaHealth.Models.MigrationModels
{
    public class EktronRelatedLink
    {
        public string ID { get; set; }
        public string Text { get; set; }
        public RelatedLink Link { get; set; }
    }


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", ElementName = "root", IsNullable = false)]
    public class RelatedLink
    {

        private RelatedLinkRelatedLinks _relatedLinksField;

        /// <remarks/>
        public RelatedLinkRelatedLinks RelatedLinks
        {
            get => _relatedLinksField;
            set => _relatedLinksField = value;
        }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class RelatedLinkRelatedLinks
    {

        private RelatedLinkRelatedLinksLink _linkField;

        /// <remarks/>
        public RelatedLinkRelatedLinksLink Link
        {
            get => _linkField;
            set => _linkField = value;
        }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class RelatedLinkRelatedLinksLink
    {

        private RelatedLinkRelatedLinksLinkA _aField;

        /// <remarks/>
        public RelatedLinkRelatedLinksLinkA a
        {
            get => _aField;
            set => _aField = value;
        }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class RelatedLinkRelatedLinksLinkA
    {

        private string _hrefField;

        private string _titleField;

        private string _valueField;

        /// <remarks/>
        [XmlAttribute]
        public string Href
        {
            get => _hrefField;
            set => _hrefField = value;
        }

        /// <remarks/>
        [XmlAttribute]
        public string Title
        {
            get => _titleField;
            set => _titleField = value;
        }

        /// <remarks/>
        [XmlText]
        public string Value
        {
            get => _valueField;
            set => _valueField = value;
        }
    }
}