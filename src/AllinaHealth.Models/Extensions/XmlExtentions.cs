using System.Xml.Linq;

namespace AllinaHealth.Models.Extensions
{
    public static class XmlExtentions
    {
        public static string ToStringWtihHtml(this XElement x)
        {
            return x == null
                ? string.Empty
                : x.ToString().Replace("<" + x.Name + ">", string.Empty).Replace("</" + x.Name + ">", string.Empty);
        }
    }
}