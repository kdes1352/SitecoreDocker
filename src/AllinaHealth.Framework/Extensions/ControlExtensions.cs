using System.Web.UI;

namespace AllinaHealth.Framework.Extensions
{
    public static class ControlExtensions
    {
        public static string RenderControlAsString(this Control c)
        {
            using (var sw = new System.IO.StringWriter())
            {
                var htw = new HtmlTextWriter(sw);
                c.RenderControl(htw);
                return sw.ToString();
            }
        }
    }
}