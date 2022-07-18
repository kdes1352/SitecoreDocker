using System;
using System.Web.UI.HtmlControls;
using AllinaHealth.Framework.Pipelines.GetContentEditorWarnings;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentManager.Galleries;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XmlControls;
using Version = Sitecore.Data.Version;

namespace AllinaHealth.Framework.Shell.Applications.ContentManager.Galleries
{
    public class VersionForm : GalleryForm
    {
        /// <summary></summary>
        protected GalleryMenu Options;

        /// <summary></summary>
        protected Scrollbox Versions;

        /// <summary>Handles the message.</summary>
        /// <param name="message">The message.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, nameof(message));
            if (message.Name == "event:click")
                return;
            Invoke(message, true);
        }

        /// <summary>Raises the load event.</summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
        /// <remarks>
        /// This method notifies the server control that it should perform actions common to each HTTP
        /// request for the page it is associated with, such as setting up a database query. At this
        /// stage in the page lifecycle, server controls in the hierarchy are created and initialized,
        /// view state is restored, and form controls reflect client-side data. Use the IsPostBack
        /// property to determine whether the page is being loaded in response to a client postback,
        /// or if it is being loaded and accessed for the first time.
        /// </remarks>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, nameof(e));
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
                return;
            var currentItem = GetCurrentItem();
            if (currentItem != null)
            {
                if (currentItem.IsFallback)
                {
                    var htmlGenericControl = new HtmlGenericControl("div");
                    htmlGenericControl.InnerText = Translate.Text("No version exists in the current language. You see a fallback version from '{0}' language.", currentItem.OriginalLanguage);
                    htmlGenericControl.Attributes["class"] = "versionNumSelected";
                    Context.ClientPage.AddControl(Versions, htmlGenericControl);
                }
                else
                {
                    var versions = currentItem.Versions.GetVersions();
                    for (var index = versions.Length - 1; index >= 0; --index)
                    {
                        var obj = versions[index];
                        var control = ControlFactory.GetControl("Gallery.Versions.Option") as XmlControl;
                        Assert.IsNotNull(control, typeof(XmlControl), "Xml Control \"{0}\" not found", "Gallery.Versions.Option");
                        Context.ClientPage.AddControl(Versions, control);
                        var culture = Context.User.Profile.Culture;
                        var str1 = obj.Statistics.Updated == DateTime.MinValue ? Translate.Text("[Not set]") : DateUtil.FormatShortDateTime(DateUtil.ToServerTime(obj.Statistics.Updated), culture);
                        //string str2 = obj.Statistics.UpdatedBy.Length == 0 ? "-" : obj.Statistics.UpdatedBy; // LINE TO UPDATE WITH CALL FROM IsLocked class
                        var userName = IsLocked.GetUserName(obj.Statistics.UpdatedBy) != "" ? IsLocked.GetUserName(obj.Statistics.UpdatedBy) : obj.Statistics.UpdatedBy;
                        var str2 = obj.Statistics.UpdatedBy.Length == 0 ? "-" : userName;
                        var str3 = obj.Version + ".";
                        var str4 = obj.Version.Number != currentItem.Version.Number ? "<div class=\"versionNum\">" + str3 + "</div>" : "<div class=\"versionNumSelected\">" + str3 + "</div>";
                        if (control == null)
                        {
                            continue;
                        }

                        control["Number"] = str4;
                        control["Header"] = Translate.Text("Modified <b>{0}</b> by <b>{1}</b>.", str1, str2);
                        control["Click"] = $"item:load(id={currentItem.ID},language={currentItem.Language},version={obj.Version.Number})";
                    }
                }
            }

            var obj1 = Client.CoreDatabase.GetItem("/sitecore/content/Applications/Content Editor/Menues/Versions");
            if (obj1 == null || !obj1.HasChildren)
                return;
            var queryString = WebUtil.GetQueryString("id");
            Options.AddFromDataSource(obj1, queryString, new CommandContext(currentItem));
        }

        /// <summary>Gets the current item.</summary>
        /// <returns>The current item.</returns>
        private static Item GetCurrentItem()
        {
            var queryString1 = WebUtil.GetQueryString("db");
            var queryString2 = WebUtil.GetQueryString("id");
            var index1 = Language.Parse(WebUtil.GetQueryString("la"));
            var index2 = Version.Parse(WebUtil.GetQueryString("vs"));
            var database = Sitecore.Configuration.Factory.GetDatabase(queryString1);
            Assert.IsNotNull(database, queryString1);
            return database.Items[queryString2, index1, index2];
        }
    }
}