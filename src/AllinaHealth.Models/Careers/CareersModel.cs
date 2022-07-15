using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AllinaHealth.Models.Careers
{
    [Serializable]
    public class CareersModel
    {
        public CareersModel()
        {
            // ReSharper disable once LocalVariableHidesMember
            // ReSharper disable once InconsistentNaming
            // ReSharper disable once UnusedVariable
            var NewsList = new List<Sitecore.Data.Items.Item>();
        }

        [XmlElement(ElementName = "NewsList", Namespace = "urn:com.workday/bsvc")]
        public List<Sitecore.Data.Items.Item> NewsList { get; set; }
    }
}
