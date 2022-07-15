using System;
using System.IO;
using System.Web.UI;
using Sitecore.Globalization;

namespace AllinaHealth.Framework.Analytics
{
    public class TrackingFieldDetailsPage : Sitecore.Shell.Applications.Analytics.TrackingField.TrackingFieldDetailsPage
    {
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);
            }
            catch (Exception)
            {
                var htmlTextWriter = new HtmlTextWriter(new StringWriter());
                TrackingValue.Value = "__#!$No value$!#__";

                if (string.IsNullOrEmpty(htmlTextWriter.InnerWriter.ToString()))
                {
                    htmlTextWriter.Write("<div style=\"padding:16px 8px 16px 8px;\" align=\"center\">");
                    htmlTextWriter.Write(Translate.Text("Nothing has been set."));
                    htmlTextWriter.Write("</div>");
                }

                Tracking.InnerHtml = htmlTextWriter.InnerWriter.ToString();
            }
        }
    }
}