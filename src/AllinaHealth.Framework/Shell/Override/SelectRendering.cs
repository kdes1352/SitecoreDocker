using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.Shell.Applications.Dialogs.SelectRendering;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;

namespace AllinaHealth.Framework.Shell.Override
{
    [UsedImplicitly]
    // ReSharper disable once UnusedMember.Global
    public class SelectRendering : SelectRenderingForm
    {
        protected Tabstrip Tabs;

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            var selectRenderingOption = SelectItemOptions.Parse<SelectRenderingOptions>();
            base.OnLoad(e);
            Tabs.OnChange += Tabs_OnChange;
            if (Context.ClientPage.IsEvent)
            {
                return;
            }

            IsOpenPropertiesChecked = Registry.GetBool("/Current_User/SelectRendering/IsOpenPropertiesChecked");
            if (selectRenderingOption.ShowOpenProperties)
            {
                OpenPropertiesBorder.Visible = true;
                OpenProperties.Checked = IsOpenPropertiesChecked;
            }

            if (selectRenderingOption.ShowPlaceholderName)
            {
                PlaceholderNameBorder.Visible = true;
                PlaceholderName.Value = selectRenderingOption.PlaceholderName;
            }

            if (selectRenderingOption.ShowTree)
            {
                if (Renderings.Parent is GridPanel parent)
                {
                    Tabs.Visible = false;
                    parent.SetExtensibleProperty(Tabs, "class", "scDisplayNone");
                }
            }
            else
            {
                TreeviewContainer.Class = string.Empty;
                if (TreeviewContainer.Parent is GridPanel gridPanel)
                {
                    TreeviewContainer.Visible = false;
                    gridPanel.SetExtensibleProperty(TreeviewContainer, "class", "scDisplayNone");
                    TreeSplitter.Visible = false;
                    gridPanel.SetExtensibleProperty(TreeSplitter, "class", "scDisplayNone");
                }

                gridPanel = Renderings.Parent as GridPanel;
                if (gridPanel != null)
                {
                    Renderings.Visible = false;
                    gridPanel.SetExtensibleProperty(Renderings, "class", "scDisplayNone");
                }

                foreach (var list in (
                             from i in selectRenderingOption.Items
                             group i by i.Parent.DisplayName
                             into g
                             orderby g.Key
                             select g).ToList())
                {
                    var tab = new Tab
                    {
                        Header = list.Key
                    };
                    var scrollbox = new Scrollbox
                    {
                        Class = "scScrollbox scFixSize scKeepFixSize",
                        Background = "white",
                        Padding = "0px",
                        Width = new Unit(100, UnitType.Percentage),
                        Height = new Unit(100, UnitType.Percentage),
                        InnerHtml = RenderPreviews(list)
                    };
                    tab.Controls.Add(scrollbox);
                    Tabs.Controls.Add(tab);
                }
            }

            SetOpenPropertiesState(selectRenderingOption.SelectedItem);
        }

        protected virtual string RenderEmptyPreview(Item item)
        {
            var htmlTextWriter = new HtmlTextWriter(new StringWriter());
            htmlTextWriter.Write("<table class='scEmptyPreview'>");
            htmlTextWriter.Write("<tbody>");
            htmlTextWriter.Write("<tr>");
            htmlTextWriter.Write("<td>");
            if (item == null)
            {
                htmlTextWriter.Write(Translate.Text("None available."));
            }
            else if (!IsItemRendering(item))
            {
                htmlTextWriter.Write(Translate.Text(Translate.Text("Please select a rendering item")));
            }
            else
            {
                htmlTextWriter.Write("<div class='scImageContainer'>");
                htmlTextWriter.Write("<span style='height:100%; width:1px; display:inline-block;'></span>");
                var icon = item.Appearance.Icon;
                var num = 48;
                var num1 = 48;
                if (!string.IsNullOrEmpty(item.Appearance.Thumbnail) && item.Appearance.Thumbnail != Settings.DefaultThumbnail)
                {
                    var thumbnailSrc = UIUtil.GetThumbnailSrc(item, 128, 128);
                    if (!string.IsNullOrEmpty(thumbnailSrc))
                    {
                        icon = thumbnailSrc;
                        num = 128;
                        num1 = 128;
                    }
                }

                (new ImageBuilder
                {
                    Align = "absmiddle",
                    Src = icon,
                    Width = num1,
                    Height = num
                }).Render(htmlTextWriter);
                htmlTextWriter.Write("</div>");
                htmlTextWriter.Write("<span class='scDisplayName'>");
                htmlTextWriter.Write(item.DisplayName);
                htmlTextWriter.Write("</span>");
            }

            htmlTextWriter.Write("</td>");
            htmlTextWriter.Write("</tr>");
            htmlTextWriter.Write("</tbody>");
            htmlTextWriter.Write("</table>");
            return htmlTextWriter.InnerWriter.ToString();
        }

        protected virtual string RenderPreviews(IEnumerable<Item> items)
        {
            Assert.ArgumentNotNull(items, "items");
            items =
                from item in items
                orderby item.DisplayName
                select item;
            var htmlTextWriter = new HtmlTextWriter(new StringWriter());
            var flag = false;
            foreach (var item1 in items)
            {
                RenderItemPreview(item1, htmlTextWriter);
                flag = true;
            }

            return flag ? htmlTextWriter.InnerWriter.ToString() : RenderEmptyPreview(null);
        }

        protected virtual void SetOpenPropertiesState(Item item)
        {
            if (item == null || !IsItemRendering(item))
            {
                OpenProperties.Disabled = true;
                OpenProperties.Checked = false;
                return;
            }

            var str = item["Open Properties After Add"];
            if (str == "-" || str != null && str.Length == 0)
            {
                OpenProperties.Disabled = false;
                OpenProperties.Checked = IsOpenPropertiesChecked;
                return;
            }

            if (str == "0")
            {
                if (!OpenProperties.Disabled)
                {
                    IsOpenPropertiesChecked = OpenProperties.Checked;
                }

                OpenProperties.Disabled = true;
                OpenProperties.Checked = false;
                return;
            }

            if (str != "1")
            {
                return;
            }

            if (!OpenProperties.Disabled)
            {
                IsOpenPropertiesChecked = OpenProperties.Checked;
            }

            OpenProperties.Disabled = true;
            OpenProperties.Checked = true;
        }

        [UsedImplicitly]
        protected virtual void Tabs_OnChange(object sender, EventArgs e)
        {
            InitializeFixsizeElements();
        }

        public static void InitializeFixsizeElements()
        {
            SheerResponse.Eval("\r\n                setTimeout(function() {\r\n                    if (Prototype.Browser.Gecko || Prototype.Browser.WebKit) scForm.browser.initializeFixsizeElements();\r\n                }, 100);");
        }
    }
}