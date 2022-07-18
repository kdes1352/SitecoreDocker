using System;
using System.IO;
using System.Web;
using System.Web.UI;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.Layouts.DeviceEditor;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;

namespace AllinaHealth.Framework.Shell.Override
{
    public class CopyDeviceTo : CopyDeviceToForm
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RenderDevices();
            RenderLanguages();
        }

        private void RenderDevices()
        {
            var listString = new ListString(Registry.GetValue("/Current_User/DeviceEditor/CopyDevices/TargetDevices"));
            var itemNotNull = Client.GetItemNotNull(ItemIDs.DevicesRoot);
            var htmlTextWriter = new HtmlTextWriter(new StringWriter());

            htmlTextWriter.Write("<span class=\"title\">Target Devices:</span>");

            foreach (Item child in itemNotNull.Children)
            {
                var str = "de_" + child.ID.ToShortID();
                htmlTextWriter.Write("<div style=\"padding:2px\">");
                htmlTextWriter.Write("<input type=\"checkbox\" id=\"" + str + "\" name=\"" + str + "\"");
                if (listString.Contains(child.ID.ToString()))
                    htmlTextWriter.Write(" checked=\"checked\"");
                htmlTextWriter.Write(" />");
                htmlTextWriter.Write("<label for=\"" + str + "\">");
                htmlTextWriter.Write(child.GetUIDisplayName());
                htmlTextWriter.Write("</label>");
                htmlTextWriter.Write("</div>");
            }

            Devices.InnerHtml = htmlTextWriter.InnerWriter.ToString();
        }

        private void RenderLanguages()
        {
            var listString = new ListString(Registry.GetValue("/Current_User/DeviceEditor/CopyDevices/TargetLanguages"));
            var itemNotNull = Client.GetItemNotNull(ItemIDs.LanguageRoot);
            var htmlTextWriter = new HtmlTextWriter(new StringWriter());

            htmlTextWriter.Write("<span class=\"title\">Target Languages:</span>");

            foreach (Item child in itemNotNull.Children)
            {
                var str = "la_" + child.ID.ToShortID();
                htmlTextWriter.Write("<div style=\"padding:2px\">");
                htmlTextWriter.Write("<input type=\"checkbox\" id=\"" + str + "\" name=\"" + str + "\"");
                if (listString.Contains(child.ID.ToString()))
                    htmlTextWriter.Write(" checked=\"checked\"");
                htmlTextWriter.Write(" />");
                htmlTextWriter.Write("<label for=\"" + str + "\">");
                htmlTextWriter.Write(child.GetUIDisplayName());
                htmlTextWriter.Write("</label>");
                htmlTextWriter.Write("</div>");
            }

            Devices.InnerHtml += htmlTextWriter.InnerWriter.ToString();
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull(args, nameof(args));
            var selectionItem = Treeview.GetSelectionItem();

            if (selectionItem == null)
            {
                SheerResponse.Alert("Select an item.", Array.Empty<string>());
            }

            if (selectionItem == null)
            {
                SheerResponse.Alert("The target item could not be found.", Array.Empty<string>());
            }
            else
            {
                var allowCopy = true;

                var listStringDev = new ListString();
                foreach (string key in HttpContext.Current.Request.Form.Keys)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("de_", StringComparison.InvariantCulture))
                        listStringDev.Add(ShortID.Decode(StringUtil.Mid(key, 3)));
                }

                if (listStringDev.Count == 0)
                {
                    allowCopy = false;
                    SheerResponse.Alert("Please select one or more devices.", Array.Empty<string>());
                }
                else
                {
                    Registry.SetValue("/Current_User/DeviceEditor/CopyDevices/TargetDevices", listStringDev.ToString());
                }

                var listStringLang = new ListString();
                foreach (string key in HttpContext.Current.Request.Form.Keys)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("la_", StringComparison.InvariantCulture))
                        listStringLang.Add(ShortID.Decode(StringUtil.Mid(key, 3)));
                }

                if (listStringLang.Count == 0)
                {
                    allowCopy = false;
                    SheerResponse.Alert("Please select one or more languages.", Array.Empty<string>());
                }
                else
                {
                    Registry.SetValue("/Current_User/DeviceEditor/CopyDevices/TargetLanguages", listStringLang.ToString());
                }

                if (!allowCopy)
                {
                    return;
                }

                var response = listStringDev + "^" + listStringLang + "^" + selectionItem.ID;
                WebUtil.SetSessionValue("SC_CopyDeviceToValue", response);
                SheerResponse.SetDialogValue(listStringDev + "^" + selectionItem.ID);
                base.OnOK(sender, args);
            }
        }
    }
}