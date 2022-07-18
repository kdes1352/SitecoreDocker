using System;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;
using Sitecore;
using Sitecore.Data.Databases;
using Sitecore.Data.Events;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Layouts;
using Sitecore.SecurityModel;
using Sitecore.Shell.Applications.Dialogs;
using Sitecore.Shell.Applications.Dialogs.LayoutDetails;
using Sitecore.Shell.Applications.Layouts.DeviceEditor;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Web.UI;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Sitecore.Xml;
using Version = Sitecore.Data.Version;

namespace AllinaHealth.Framework.Shell.Override
{
    public class LayoutDetails : DialogForm
    {
        protected Border LayoutPanel;
        protected Border FinalLayoutPanel;
        protected Border FinalLayoutNoVersionWarningPanel;
        protected Tab SharedLayoutTab;

        // ReSharper disable once UnusedMember.Global
        protected Tab FinalLayoutTab;
        protected Tabstrip Tabs;
        protected Literal WarningTitle;

        public LayoutDetails()
        {
            DatabaseHelper = new DatabaseHelper();
        }

        protected DatabaseHelper DatabaseHelper { get; set; }

        public virtual string Layout
        {
            get => StringUtil.GetString(ServerProperties[nameof(Layout)]);
            set
            {
                Assert.ArgumentNotNull(value, nameof(value));
                ServerProperties[nameof(Layout)] = value;
            }
        }

        public virtual string FinalLayout
        {
            get
            {
                var layoutDelta = LayoutDelta;
                if (string.IsNullOrWhiteSpace(layoutDelta))
                    return Layout;
                return string.IsNullOrWhiteSpace(Layout) ? layoutDelta : XmlDeltas.ApplyDelta(Layout, layoutDelta);
            }
            set
            {
                Assert.ArgumentNotNull(value, nameof(value));
                if (!string.IsNullOrWhiteSpace(Layout))
                    LayoutDelta = XmlUtil.XmlStringsAreEqual(Layout, value) ? null : XmlDeltas.GetDelta(value, Layout);
                else
                    LayoutDelta = value;
            }
        }

        protected virtual string LayoutDelta
        {
            get => StringUtil.GetString(ServerProperties[nameof(LayoutDelta)]);
            set => ServerProperties[nameof(LayoutDelta)] = value;
        }

        protected bool VersionCreated
        {
            get => MainUtil.GetBool(ServerProperties[nameof(VersionCreated)], false);
            set => ServerProperties[nameof(VersionCreated)] = value;
        }

        private TabType ActiveTab
        {
            get
            {
                switch (Tabs.Active)
                {
                    case 0:
                        return TabType.Final;
                    case 1:
                        return TabType.Shared;
                    default:
                        return TabType.Unknown;
                }
            }
        }

        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, nameof(message));
            if (message.Name == "item:addversion")
            {
                var currentItem = GetCurrentItem();
                Dispatcher.Dispatch(message, currentItem);
            }
            else
                base.HandleMessage(message);
        }

        // ReSharper disable once UnusedMember.Global
        protected void CopyDevice(string deviceId)
        {
            Assert.ArgumentNotNullOrEmpty(deviceId, nameof(deviceId));
            Context.ClientPage.Start(this, "CopyDevicePipeline", new NameValueCollection
            {
                {
                    "deviceid",
                    deviceId
                }
            });
        }

        // ReSharper disable once UnusedMember.Global
        protected void CopyDevicePipeline(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            if (args.IsPostBack)
            {
                var sessionResult = WebUtil.GetSessionString("SC_CopyDeviceToValue");

                if (string.IsNullOrEmpty(sessionResult) || sessionResult == "undefined")
                    return;

                var strArray = sessionResult.Split('^');

                var str = StringUtil.GetString(args.Parameters["deviceid"]);

                var devices = new ListString(strArray[0]);
                var langs = new ListString(strArray[1]);
                var itemPath = strArray[2];
                var sourceDevice = GetDoc().SelectSingleNode("/r/d[@id='" + str + "']");

                if (sourceDevice != null)
                {
                    foreach (var lId in langs)
                    {
                        var l = Client.GetItemNotNull(lId);
                        var iso = l["Regional Iso Code"];
                        if (string.IsNullOrEmpty(iso))
                        {
                            iso = l["Iso"];
                        }

                        var langResult = LanguageManager.GetLanguage(iso);

                        //Sitecore.Data.Version version = Sitecore.Data.Version.Parse(WebUtil.GetQueryString("ve"));

                        var itemNotNull = Client.GetItemNotNull(itemPath, langResult);
                        CopyDevice(sourceDevice, devices, itemNotNull);
                    }

                }

                Refresh();
            }
            else
            {
                WebUtil.SetSessionValue("SC_DEVICEEDITOR", GetDoc().OuterXml);
                var urlString = new UrlString(UIUtil.GetUri("control:CopyDeviceTo"))
                {
                    ["de"] = StringUtil.GetString(args.Parameters["deviceid"]),
                    ["fo"] = WebUtil.GetQueryString("id")
                };
                SheerResponse.ShowModalDialog(urlString.ToString(), "1200px", "700px", string.Empty, true);
                args.WaitForPostBack();
            }
        }

        protected string GetActiveLayout()
        {
            return ActiveTab == TabType.Final ? FinalLayout : Layout;
        }

        protected string GetDialogResult()
        {
            return new LayoutDetailsDialogResult
            {
                Layout = Layout,
                FinalLayout = FinalLayout,
                VersionCreated = VersionCreated
            }.ToString();
        }

        protected void SetActiveLayout(string value)
        {
            if (ActiveTab == TabType.Final)
                FinalLayout = value;
            else
                Layout = value;
        }

        // ReSharper disable once UnusedMember.Global
        protected void EditPlaceholder(string deviceId, string uniqueId)
        {
            Assert.ArgumentNotNull(deviceId, nameof(deviceId));
            Assert.ArgumentNotNullOrEmpty(uniqueId, nameof(uniqueId));
            Context.ClientPage.Start(this, "EditPlaceholderPipeline", new NameValueCollection
            {
                {
                    "deviceid",
                    deviceId
                },
                {
                    "uniqueid",
                    uniqueId
                }
            });
        }

        // ReSharper disable once UnusedMember.Global
        protected void EditPlaceholderPipeline(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            var layoutDefinition = LayoutDefinition.Parse(GetDoc().OuterXml);
            var placeholder = layoutDefinition.GetDevice(args.Parameters["deviceid"]).GetPlaceholder(args.Parameters["uniqueid"]);
            if (placeholder == null)
                return;
            if (args.IsPostBack)
            {
                if (string.IsNullOrEmpty(args.Result) || args.Result == "undefined")
                    return;
                var dialogResult = SelectPlaceholderSettingsOptions.ParseDialogResult(args.Result, Client.ContentDatabase, out var placeholderKey);
                if (dialogResult != null)
                    placeholder.MetaDataItemId = dialogResult.ID.ToString();
                if (!string.IsNullOrEmpty(placeholderKey))
                    placeholder.Key = placeholderKey;
                SetActiveLayout(layoutDefinition.ToXml());
                Refresh();
            }
            else
            {
                var itemByPathOrId = DatabaseHelper.GetItemByPathOrId(Client.ContentDatabase, placeholder.MetaDataItemId);
                var placeholderSettingsOptions = new SelectPlaceholderSettingsOptions
                {
                    TemplateForCreating = null,
                    PlaceholderKey = placeholder.Key,
                    CurrentSettingsItem = itemByPathOrId,
                    SelectedItem = itemByPathOrId,
                    IsPlaceholderKeyEditable = true
                };
                SheerResponse.ShowModalDialog(placeholderSettingsOptions.ToUrlString().ToString(), "460px", "460px", string.Empty, true);
                args.WaitForPostBack();
            }
        }

        // ReSharper disable once UnusedMember.Global
        protected void EditRendering(string deviceId, string index)
        {
            Assert.ArgumentNotNull(deviceId, nameof(deviceId));
            Assert.ArgumentNotNull(index, nameof(index));
            Context.ClientPage.Start(this, "EditRenderingPipeline", new NameValueCollection
            {
                {
                    "deviceid",
                    deviceId
                },
                {
                    nameof(index),
                    index
                }
            });
        }

        // ReSharper disable once UnusedMember.Global
        protected void EditRenderingPipeline(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            var renderingParameters = new RenderingParameters
            {
                Args = args,
                DeviceId = StringUtil.GetString(args.Parameters["deviceid"]),
                SelectedIndex = MainUtil.GetInt(StringUtil.GetString(args.Parameters["index"]), 0),
                Item = UIUtil.GetItemFromQueryString(Client.ContentDatabase)
            };
            if (!args.IsPostBack)
                WebUtil.SetSessionValue("SC_DEVICEEDITOR", GetDoc().OuterXml);
            if (!renderingParameters.Show())
                return;
            var doc = XmlUtil.LoadXml(WebUtil.GetSessionString("SC_DEVICEEDITOR"));
            WebUtil.SetSessionValue("SC_DEVICEEDITOR", null);
            SetActiveLayout(GetLayoutValue(doc));
            Refresh();
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.CanRunApplication("Content Editor/Ribbons/Chunks/Layout");
            Assert.ArgumentNotNull(e, nameof(e));
            base.OnLoad(e);
            Tabs.OnChange += (sender, args) => Refresh();
            if (!Context.ClientPage.IsEvent)
            {
                if (!Context.User.IsAdministrator)
                {
                    SharedLayoutTab.Visible = false;
                }

                Tabs.Active = 0;
                var currentItem = GetCurrentItem();
                Assert.IsNotNull(currentItem, "Item not found");
                Layout = LayoutField.GetFieldValue(currentItem.Fields[FieldIDs.LayoutField]);
                var field = currentItem.Fields[FieldIDs.FinalLayoutField];
                LayoutDelta = currentItem.Name == "__Standard Values" ? field.GetStandardValue() : field.GetValue(false, false) ?? field.GetInheritedValue(false) ?? field.GetValue(false, false, true, false, false);
                ToggleVisibilityOfControlsOnFinalLayoutTab(currentItem);
                Refresh();
            }

            var site = Context.Site;
            if (site == null)
                return;
            site.Notifications.ItemSaved += ItemSavedNotification;
        }

        protected void ToggleVisibilityOfControlsOnFinalLayoutTab(Item item)
        {
            var flag = item.Versions.Count > 0;
            FinalLayoutPanel.Visible = flag;
            FinalLayoutNoVersionWarningPanel.Visible = !flag;
            if (flag)
                return;
            WarningTitle.Text = string.Format(Translate.Text("The current item does not have a version in \"{0}\"."), item.Language.GetDisplayName());
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull(args, nameof(args));
            SheerResponse.SetDialogValue(GetDialogResult());
            base.OnOK(sender, args);
        }

        // ReSharper disable once UnusedMember.Global
        protected void OpenDevice(string deviceId)
        {
            Assert.ArgumentNotNullOrEmpty(deviceId, nameof(deviceId));
            Context.ClientPage.Start(this, "OpenDevicePipeline", new NameValueCollection
            {
                {
                    "deviceid",
                    deviceId
                }
            });
        }

        // ReSharper disable once UnusedMember.Global
        protected void OpenDevicePipeline(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            if (args.IsPostBack)
            {
                if (string.IsNullOrEmpty(args.Result) || args.Result == "undefined")
                    return;
                var doc = XmlUtil.LoadXml(WebUtil.GetSessionString("SC_DEVICEEDITOR"));
                WebUtil.SetSessionValue("SC_DEVICEEDITOR", null);
                SetActiveLayout(doc != null ? GetLayoutValue(doc) : string.Empty);
                Refresh();
            }
            else
            {
                WebUtil.SetSessionValue("SC_DEVICEEDITOR", GetDoc().OuterXml);
                var urlString = new UrlString(UIUtil.GetUri("control:DeviceEditor"));
                urlString.Append("de", StringUtil.GetString(args.Parameters["deviceid"]));
                urlString.Append("id", WebUtil.GetQueryString("id"));
                urlString.Append("vs", WebUtil.GetQueryString("vs"));
                urlString.Append("la", WebUtil.GetQueryString("la"));
                Context.ClientPage.ClientResponse.ShowModalDialog(new ModalDialogOptions(urlString.ToString())
                {
                    Response = true,
                    Width = "700"
                });
                args.WaitForPostBack();
            }
        }

        private void CopyDevice(XmlNode sourceDevice, ListString devices, Item item)
        {
            Assert.ArgumentNotNull(sourceDevice, nameof(sourceDevice));
            Assert.ArgumentNotNull(devices, nameof(devices));
            Assert.ArgumentNotNull(item, nameof(item));
            var layoutField = GetLayoutField(item);
            var data = ((LayoutField)layoutField).Data;
            CopyDevices(data, devices, sourceDevice);
            using (new SecurityDisabler())
            {
                item.Editing.BeginEdit();

                var delta = XmlDeltas.GetDelta(data.OuterXml, item.Fields[FieldIDs.LayoutField].Value);
                if (string.IsNullOrWhiteSpace(delta))
                {
                    delta = data.OuterXml;
                }

                layoutField.SetValue(delta, true);
                item.Editing.EndEdit();
            }
        }

        private void ItemSavedNotification(object sender, ItemSavedEventArgs args)
        {
            VersionCreated = true;
            ToggleVisibilityOfControlsOnFinalLayoutTab(args.Item);
            SheerResponse.SetDialogValue(GetDialogResult());
        }

        private static void CopyDevices(XmlDocument doc, ListString devices, XmlNode sourceDevice)
        {
            Assert.ArgumentNotNull(doc, nameof(doc));
            Assert.ArgumentNotNull(devices, nameof(devices));
            Assert.ArgumentNotNull(sourceDevice, nameof(sourceDevice));
            var xmlNode1 = doc.ImportNode(sourceDevice, true);
            foreach (var device in devices)
            {
                if (doc.DocumentElement == null) continue;
                var node = doc.DocumentElement.SelectSingleNode("d[@id='" + device + "']");
                if (node != null)
                    XmlUtil.RemoveNode(node);
                var xmlNode2 = xmlNode1.CloneNode(true);
                XmlUtil.SetAttribute("id", device, xmlNode2);
                doc.DocumentElement.AppendChild(xmlNode2);
            }
        }

        private static Item GetCurrentItem()
        {
            return Client.ContentDatabase.GetItem(WebUtil.GetQueryString("id"), Language.Parse(WebUtil.GetQueryString("la")), Version.Parse(WebUtil.GetQueryString("vs")));
        }

        private static string GetLayoutValue(XmlDocument doc)
        {
            Assert.ArgumentNotNull(doc, nameof(doc));
            var xmlNodeList = doc.SelectNodes("/r/d");
            if (xmlNodeList == null || xmlNodeList.Count == 0)
                return string.Empty;
            return xmlNodeList.Cast<XmlNode>().Any(node => node.ChildNodes.Count > 0 || XmlUtil.GetAttribute("l", node).Length > 0) ? doc.OuterXml : string.Empty;
        }

        // ReSharper disable once UnusedMember.Local
        private void CopyDevice(XmlNode sourceDevice, ListString devices)
        {
            Assert.ArgumentNotNull(sourceDevice, nameof(sourceDevice));
            Assert.ArgumentNotNull(devices, nameof(devices));
            var doc = XmlUtil.LoadXml(GetActiveLayout());
            CopyDevices(doc, devices, sourceDevice);
            SetActiveLayout(doc.OuterXml);
        }

        private XmlDocument GetDoc()
        {
            var xmlDocument = new XmlDocument();
            var activeLayout = GetActiveLayout();
            xmlDocument.LoadXml(activeLayout.Length > 0 ? activeLayout : "<r/>");
            return xmlDocument;
        }

        private void Refresh()
        {
            RenderLayoutGridBuilder(GetActiveLayout(), ActiveTab == TabType.Final ? FinalLayoutPanel : (Control)LayoutPanel);
        }

        private static void RenderLayoutGridBuilder(string layoutValue, Control renderingContainer)
        {
            var str = renderingContainer.ID + "LayoutGrid";
            var layoutGridBuilder = new LayoutGridBuilder
            {
                ID = str,
                Value = layoutValue,
                EditRenderingClick = "EditRendering(\"$Device\", \"$Index\")",
                EditPlaceholderClick = "EditPlaceholder(\"$Device\", \"$UniqueID\")",
                OpenDeviceClick = "OpenDevice(\"$Device\")",
                CopyToClick = "CopyDevice(\"$Device\")"
            };
            renderingContainer.Controls.Clear();
            layoutGridBuilder.BuildGrid(renderingContainer);
            if (!Context.ClientPage.IsEvent)
                return;
            SheerResponse.SetOuterHtml(renderingContainer.ID, renderingContainer);
            SheerResponse.Eval("if (!scForm.browser.isIE) { scForm.browser.initializeFixsizeElements(); }");
        }

        private Field GetLayoutField(Item item)
        {
            return ActiveTab == TabType.Final ? item.Fields[FieldIDs.FinalLayoutField] : item.Fields[FieldIDs.LayoutField];
        }

        private enum TabType
        {
            Shared,
            Final,
            Unknown,
        }
    }
}