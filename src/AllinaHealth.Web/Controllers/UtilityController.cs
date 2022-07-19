using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using AllinaHealth.Framework.Extensions;
using AllinaHealth.Models.Extensions;
using AllinaHealth.Models.MigrationModels;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OfficeOpenXml;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.ContentSearch.Maintenance;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.Resources.Media;
using Sitecore.SecurityModel;
using Sitecore.Text;

namespace AllinaHealth.Web.Controllers
{
    public class UtilityController : Controller
    {
        private const string HomePath = "/sitecore/content/Allina Health/Home";
        private readonly WebClient _webClient;
        private readonly Database _db;
        private readonly TemplateItem _template;
        private readonly Dictionary<string, int> _images;
        private readonly StringBuilder _sb;
        private readonly List<string> _imagesForAction;

        private void CreateMenus(IReadOnlyCollection<EktronMenu> list, List<EktronMenu> parents, Item menuFolder, IReadOnlyDictionary<string, string> dict)
        {
            var menuFolderTemplateId = new TemplateID(new ID("{628C49FA-3CD5-48E8-90D7-2D145DB7A845}"));
            var menuTemplateId = new TemplateID(new ID("{ED0AAB07-2690-4CEA-B95E-B81CAC7DCF52}"));
            var menu1TemplateId = new TemplateID(new ID("{A506856E-6A50-4B7F-AC34-C8D580A4EED8}"));
            var menu2TemplateId = new TemplateID(new ID("{B74ACC24-E4E4-440A-8D49-1A2411949DD2}"));
            var menu3TemplateId = new TemplateID(new ID("{93E44D2D-3B98-4136-BD26-E073F3EE334A}"));
            var menuFolderTemplateItem = _db.GetTemplate(new ID("{628C49FA-3CD5-48E8-90D7-2D145DB7A845}"));
            var menuTemplateItem = _db.GetTemplate(new ID("{ED0AAB07-2690-4CEA-B95E-B81CAC7DCF52}"));
            var menu1TemplateItem = _db.GetTemplate(new ID("{A506856E-6A50-4B7F-AC34-C8D580A4EED8}"));
            var menu2TemplateItem = _db.GetTemplate(new ID("{B74ACC24-E4E4-440A-8D49-1A2411949DD2}"));
            var menu3TemplateItem = _db.GetTemplate(new ID("{93E44D2D-3B98-4136-BD26-E073F3EE334A}"));

            foreach (var p in parents)
            {
                if (string.IsNullOrEmpty(p.Name))
                {
                    continue;
                }

                var itemName = ItemUtil.ProposeValidItemName(p.Name.Replace("&amp;", "and")).ToLower();
                var pItem = menuFolder.GetChildren().FirstOrDefault(e => e.Name == itemName);
                if (pItem.IsNull())
                {
                    pItem = menuFolder.Add(itemName, menuFolderTemplateId);
                }

                pItem.ChangeTemplate(menuFolderTemplateItem);

                foreach (var m in list.Where(e => e.ParentID == p.ID))
                {
                    if (string.IsNullOrEmpty(m.Name))
                    {
                        continue;
                    }

                    var itemName2 = ItemUtil.ProposeValidItemName(m.Name.Replace("&amp;", "and")).ToLower();
                    var mItem = pItem.GetChildren().FirstOrDefault(e => e.Name == itemName2);
                    if (mItem.IsNull())
                    {
                        mItem = pItem.Add(itemName2, menuTemplateId);
                    }

                    mItem.ChangeTemplate(menuTemplateItem);

                    using (new EditContext(mItem))
                    {
                        mItem.Fields["Menu Link Text"].SetValue(HttpUtility.HtmlDecode(m.Name), true);
                        SetLinkField(mItem, "Menu Link", m);
                        if (dict.ContainsKey(m.ID))
                        {
                            mItem.Fields[FieldIDs.Sortorder].SetValue(dict[m.ID], true);
                        }
                    }

                    foreach (var m1 in list.Where(e => e.ParentID == m.ID))
                    {
                        if (string.IsNullOrEmpty(m1.Name))
                        {
                            continue;
                        }

                        var itemName3 = ItemUtil.ProposeValidItemName(m1.Name.Replace("&amp;", "and")).ToLower();
                        var m1Item = mItem.GetChildren().FirstOrDefault(e => e.Name == itemName3);
                        if (m1Item.IsNull())
                        {
                            m1Item = mItem.Add(itemName3, menu1TemplateId);
                        }

                        m1Item.ChangeTemplate(menu1TemplateItem);

                        using (new EditContext(m1Item))
                        {
                            m1Item.Fields["Menu Item Link Text"].SetValue(HttpUtility.HtmlDecode(m1.Name), true);
                            SetLinkField(m1Item, "Menu Item Link", m1);
                            if (dict.ContainsKey(m1.ID))
                            {
                                m1Item.Fields[FieldIDs.Sortorder].SetValue(dict[m1.ID], true);
                            }
                        }

                        foreach (var m2 in list.Where(e => e.ParentID == m1.ID))
                        {
                            if (string.IsNullOrEmpty(m2.Name))
                            {
                                continue;
                            }

                            var itemName4 = ItemUtil.ProposeValidItemName(m2.Name.Replace("&amp;", "and")).ToLower();
                            var m2Item = m1Item.GetChildren().FirstOrDefault(e => e.Name == itemName4);
                            if (m2Item.IsNull())
                            {
                                m2Item = m1Item.Add(itemName4, menu2TemplateId);
                            }

                            m2Item.ChangeTemplate(menu2TemplateItem);

                            using (new EditContext(m2Item))
                            {
                                m2Item.Fields["Menu Item Link Text"].SetValue(HttpUtility.HtmlDecode(m2.Name), true);
                                SetLinkField(m2Item, "Menu Item Link", m2);
                                if (dict.ContainsKey(m2.ID))
                                {
                                    m2Item.Fields[FieldIDs.Sortorder].SetValue(dict[m2.ID], true);
                                }
                            }

                            foreach (var m3 in list.Where(e => e.ParentID == m2.ID))
                            {
                                if (string.IsNullOrEmpty(m3.Name))
                                {
                                    continue;
                                }

                                var itemName5 = ItemUtil.ProposeValidItemName(m3.Name.Replace("&amp;", "and")).ToLower();
                                var m3Item = m2Item.GetChildren().FirstOrDefault(e => e.Name == itemName5);
                                if (m3Item.IsNull())
                                {
                                    m3Item = m2Item.Add(itemName5, menu3TemplateId);
                                }

                                m3Item.ChangeTemplate(menu3TemplateItem);

                                using (new EditContext(m3Item))
                                {
                                    m3Item.Fields["Menu Item Link Text"].SetValue(HttpUtility.HtmlDecode(m3.Name), true);
                                    SetLinkField(m3Item, "Menu Item Link", m3);
                                    if (dict.ContainsKey(m3.ID))
                                    {
                                        m3Item.Fields[FieldIDs.Sortorder].SetValue(dict[m3.ID], true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private Item FindMenuItem(string url)
        {
            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.LastIndexOf("/", StringComparison.Ordinal));
            }

            var path = HomePath + "/" + url.Replace("-", " ");
            return _db.GetItem(path);
        }

        public void CreateRedirect(string sourceUrl, string targetUrl, bool isDocument = false)
        {
            var folder = _db.GetItem(new ID("{4B76E95C-7B28-4F53-B588-E7D99D5611D9}"));
            if (isDocument)
            {
                folder = _db.GetItem(new ID("{1F80E8AB-19F8-4B7B-A2A4-3EE1417EA49C}"));
            }

            var templateId = new TemplateID(new ID("{B5967A68-7F70-42D3-9874-0E4D001DBC20}"));
            const string status301 = "{3184B308-C050-4A16-9F82-D77190A28F0F}";

            if (targetUrl.EndsWith("/"))
            {
                targetUrl = targetUrl.Substring(0, targetUrl.LastIndexOf("/", StringComparison.Ordinal));
            }

            var existingTargetItem = _db.GetItem(HomePath + targetUrl.Replace("-", " "));

            if (existingTargetItem == null && ID.IsID(targetUrl))
            {
                existingTargetItem = _db.GetItem(new ID(targetUrl));
            }

            var name = sourceUrl.TrimEnd('/');
            name = name.Substring(name.LastIndexOf("/", StringComparison.Ordinal) + 1);
            if (isDocument && existingTargetItem != null)
            {
                name = existingTargetItem.Name;
            }

            name = ItemUtil.ProposeValidItemName(name.Replace("-", "_").Replace(" ", "_")).ToLower().Trim();
            var count = name.Length;
            if (name.Length >= 3)
            {
                count = 3;
            }

            var itemBucketPath = string.Join("/", name.Substring(0, count).ToArray());

            var newItem = _db.GetItem(folder.Paths.FullPath + "/" + itemBucketPath + "/" + name) ?? folder.Add(name, templateId);

            using (new EditContext(newItem))
            {
                var sUrl = sourceUrl;
                if (!sUrl.StartsWith("/"))
                {
                    sUrl = "/" + sUrl;
                }

                newItem.Fields["Requested Url"].SetValue(sUrl, true);
                newItem.Fields["Response Status Code"].SetValue(status301, true);
                if (existingTargetItem != null)
                {
                    newItem.Fields["Redirect To Item"].SetValue(existingTargetItem.ID.ToString(), true);
                    newItem.Fields["Redirect To Url"].SetValue(string.Empty, true);
                }
                else
                {
                    newItem.Fields["Redirect To Item"].SetValue(string.Empty, true);
                    newItem.Fields["Redirect To Url"].SetValue(targetUrl, true);
                }
            }
        }

        private Item CreateFolderStructure(Item i, string url, string templateId)
        {
            if (!url.StartsWith("/"))
            {
                url = "/" + url;
            }

            var item = _db.GetItem(i.Paths.FullPath + url);

            if (item != null || string.IsNullOrEmpty(url))
            {
                return item;
            }

            var parentPath = i.Paths.FullPath + url.Substring(0, url.LastIndexOf("/", StringComparison.Ordinal));
            var parent = _db.GetItem(parentPath) ?? CreateFolderStructure(i, url.Substring(0, url.LastIndexOf("/", StringComparison.Ordinal)), templateId);

            if (parent == null)
            {
                return null;
            }

            var lastIndex = url.LastIndexOf("/", StringComparison.Ordinal);
            var name = url.Substring(lastIndex + 1);

            name = ItemUtil.ProposeValidItemName(name).ToLower();
            item = _db.GetItem(parent.Paths.FullPath + "/" + name) ?? parent.Add(name, new TemplateID(new ID(templateId)));

            return item;
        }

        private void CreatePages(List<string> urlList)
        {
            foreach (var url in urlList)
            {
                CreatePage(url);
            }
        }

        private ID CreateImageInSitecore(string rawUrl, EktronDocument doc = null)
        {
            try
            {
                GetImageUrlAndPath(rawUrl, out var url, out var path, out var fileName);

                if (doc != null)
                {
                    var docPath = "/" + string.Join("/", GetDocumentPath(doc.Url));
                    path = "/files" + docPath;
                    var pos = docPath.LastIndexOf("/", StringComparison.Ordinal);
                    if (pos >= 0)
                    {
                        fileName = docPath.Substring(pos + 1);
                    }

                    var extension = doc.Handle;
                    if (!string.IsNullOrEmpty(extension))
                    {
                        var posPeriod = extension.LastIndexOf(".", StringComparison.Ordinal);
                        if (posPeriod > 0)
                        {
                            extension = extension.Substring(posPeriod);
                            fileName = fileName + extension;
                        }
                        else
                        {
                            _sb.AppendFormat("no ext: {0}, {1}<br/>", doc.Url, doc.Handle);
                        }
                    }
                    else
                    {
                        extension = doc.Url;
                        var posPeriod = extension.LastIndexOf(".", StringComparison.Ordinal);
                        if (posPeriod > 0)
                        {
                            extension = extension.Substring(posPeriod);
                            fileName = fileName + extension;
                        }
                        else
                        {
                            _sb.AppendFormat("no ext: {0}, {1}<br/>", doc.Url, doc.Handle);
                        }
                    }
                }

                var item = _db.GetItem("/sitecore/media library/Allina Health" + path);
                if (item != null)
                {
                    return item.ID;
                }

                var webRequest = WebRequest.Create(url);
                using (var webResponse = webRequest.GetResponse())
                {
                    using (var stream = webResponse.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                stream.CopyTo(memoryStream);

                                var mediaCreator = new MediaCreator();
                                var options = new MediaCreatorOptions
                                {
                                    Versioned = false,
                                    IncludeExtensionInItemName = false,
                                    Database = Factory.GetDatabase("master"),
                                    Destination = "/sitecore/media library/Allina Health" + path
                                };

                                using (new SecurityDisabler())
                                {
                                    Item mi;
                                    try
                                    {
                                        mi = mediaCreator.CreateFromStream(memoryStream, fileName, options);
                                    }
                                    catch (Exception)
                                    {
                                        mi = mediaCreator.CreateFromStream(memoryStream, fileName.Replace("-", " "), options);
                                    }

                                    if (mi != null)
                                    {
                                        if (mi.Fields["Alt"] == null)
                                        {
                                            return mi.ID;
                                        }

                                        using (new EditContext(mi))
                                        {
                                            mi.Fields["Alt"].SetValue(path.Substring(path.LastIndexOf("/", StringComparison.Ordinal) + 1), true);
                                        }

                                        return mi.ID;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (WebException we)
            {
                if (we.Response is HttpWebResponse errorResponse)
                {
                    _sb.Append("media asset: " + rawUrl + ", status code: " + errorResponse.StatusCode + "<br/>");
                }
            }
            catch (Exception e)
            {
                _sb.Append("media asset: " + rawUrl + ", message: " + e.Message + "<br/>");
            }

            return ID.Null;
        }

        private Item CreatePage(string url)
        {
            try
            {
                var htmlNonUtf8 = _webClient.DownloadString(url);
                var htmlBytes = Encoding.Default.GetBytes(htmlNonUtf8);
                var html = Encoding.UTF8.GetString(htmlBytes);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                html = html.Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

                SaveImages(html, doc);

                var metaTitle = doc.DocumentNode.SelectNodes("//title").FirstOrDefault();
                var metaTags = doc.DocumentNode.SelectNodes("//meta");
                var title = metaTitle?.InnerText;
                var description = GetMetaTagValue(metaTags, "description");
                var ogTitle = GetMetaTagValue(metaTags, "og:title");
                var ogDescription = GetMetaTagValue(metaTags, "og:description");

                return CreatePageInSitecore(url, title, description, ogTitle, ogDescription);
            }
            catch (Exception)
            {
                _sb.Append("Error migrating page: " + url + "<br/>");
            }

            return null;
        }

        private Item CreatePageInSitecore(string baseUrl, string title, string description, string ogTitle, string ogDescription)
        {
            if (baseUrl.EndsWith("/"))
            {
                baseUrl = baseUrl.Substring(0, baseUrl.LastIndexOf("/", StringComparison.Ordinal));
            }

            var url = baseUrl.Replace("https://www.allinahealth.org", string.Empty);

            var item = _db.GetItem(HomePath + url.Replace("-", " "));

            if (item == null)
            {
                var parentPath = HomePath + url.Substring(0, url.LastIndexOf("/", StringComparison.Ordinal)).Replace("-", " ");
                var parent = _db.GetItem(parentPath);

                if (parent == null)
                {
                    var parentUrl = baseUrl.Substring(0, baseUrl.LastIndexOf("/", StringComparison.Ordinal));
                    parent = CreatePage(parentUrl);
                }

                if (parent != null)
                {
                    var lastIndex = url.LastIndexOf("/", StringComparison.Ordinal);
                    var name = url.Substring(lastIndex + 1);
                    name = name.Replace("-", " ");
                    item = parent.Add(name, _template);
                }
            }

            UpdateItem(item, title, description, ogTitle, ogDescription);

            return item;
        }

        private void CreateRelatedCallsToAction(IEnumerable<HsgMigrationModel> list)
        {
            var articleActions = new Dictionary<int, Relatedarticleaction>();
            foreach (var b in from a in list where a.RelatedArticleActions != null from b in a.RelatedArticleActions where b != null where !articleActions.ContainsKey(b.Id) select b)
            {
                articleActions.Add(b.Id, b);
            }

            var relatedActionFolder = _db.GetItem(new ID("{C941DB02-4CEF-4C76-9E41-751A5B525F6E}"));
            var relatedActionTemplate = _db.GetTemplate(new ID("{EE6ED02F-FFBA-4EB5-9CE3-FB741A22E34A}"));
            using (new SecurityDisabler())
            {
                foreach (var r in articleActions.Values)
                {
                    if (string.IsNullOrEmpty(r.Title))
                    {
                        //_sb.AppendFormat("{0}<br/>", r.Id);
                        continue;
                    }

                    var name = ItemUtil.ProposeValidItemName(r.Title).ToLower();
                    var rItem = relatedActionFolder.GetChildren().FirstOrDefault(e => e.Name == name) ?? relatedActionFolder.Add(name, relatedActionTemplate);

                    using (new EditContext(rItem))
                    {
                        rItem.Fields["Action Header"].SetValue(r.Title, true);
                        rItem.Fields["Action Button Text"].SetValue(r.ActionTitle, true);

                        var url = r.ActionUrl;
                        if (url.EndsWith("/"))
                        {
                            url = url.Substring(0, url.LastIndexOf("/", StringComparison.Ordinal));
                        }

                        SetLinkField(rItem, "Action Button URL", null, url);
                    }
                }
            }
        }

        public string Ekt()
        {
            var d = new DirectoryInfo("C:\\ComponentDump2\\ComponentDump");
            var list = new List<EktronInfo>();
            var dictComp = new Dictionary<string, int>();
            var dictUrl = new Dictionary<string, int>();

            foreach (var f in d.GetFiles())
            {

                var fr = f.OpenText();

                try
                {
                    var isFirstLine = true;
                    string line;
                    var ektInf = new EktronInfo();
                    var sb = new StringBuilder();
                    while ((line = fr.ReadLine()) != null)
                    {
                        if (isFirstLine)
                        {
                            var infoArray = line.Split('|');
                            if (infoArray.Length == 3)
                            {
                                ektInf.WidgetName = infoArray[0];
                                ektInf.Url = infoArray[1];
                                ektInf.Title = infoArray[2];

                                if (dictComp.ContainsKey(infoArray[0]))
                                {
                                    dictComp[infoArray[0]] += 1;
                                }
                                else
                                {
                                    dictComp.Add(infoArray[0], 1);
                                }

                                if (dictUrl.ContainsKey(infoArray[1]))
                                {
                                    dictUrl[infoArray[1]] += 1;
                                }
                                else
                                {
                                    dictUrl.Add(infoArray[1], 1);
                                }
                            }

                            isFirstLine = false;
                        }
                        else
                        {
                            sb.Append(line);
                        }
                    }

                    ektInf.Data = sb.ToString().Trim();
                    list.Add(ektInf);
                }
                catch (Exception)
                {
                    // ignored
                }
                finally
                {
                    fr.Close();
                }
            }

            ProcessEktronData(list);

            //_sb.Append("<h1>Components</h1>");
            //foreach (var k in dictComp.Keys)
            //{
            //    _sb.AppendFormat("{0} {1}<br/>", k, dictComp[k]);
            //}

            //_sb.Append("<h1>Urls</h1>");
            //foreach (var k in dictUrl.Keys)
            //{
            //    _sb.AppendFormat("{0} {1}<br/>", k, dictUrl[k]);
            //}

            return _sb.ToString();
        }

        public string FixNavTitle()
        {
            var home = _db.GetItem(HomePath);
            if (home == null)
            {
                return ":)";
            }

            var list = new List<Item> { home };
            list.AddRange(home.Axes.GetDescendants());
            using (new SecurityDisabler())
            {
                foreach (var i in list.Where(i => !i.IsNull()))
                {
                    using (new EditContext(i))
                    {
                        i.Fields["Page Title"]?.SetValue(i.GetFieldValue("OG Title"), true);
                    }
                }
            }

            return ":)";
        }

        private static string FixPath(string path)
        {
            path = path.Replace(" - ", " ").Replace("®", string.Empty).Replace("?", string.Empty).Replace("_", " ").Replace("=", string.Empty).Replace(".", " ").Replace("™", string.Empty).Trim();
            return path.Replace("-", " ").Replace(",", string.Empty).Replace("'", string.Empty).Replace("&", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace("!", string.Empty).Replace("’", string.Empty).Replace(((char)39).ToString(), string.Empty).Replace("#39;", string.Empty).Replace("é", "e").Replace(" amp; ", " ").ToLower();
        }

        // ReSharper disable once UnusedMember.Local
        private string GetAuthor(HsgMigrationModel a)
        {
            var authorFolder = _db.GetItem(new ID("{9C895D94-32A8-4F40-9298-8FDEA7197850}"));
            var authorTemplate = _db.GetTemplate(new ID("{738E64D0-657A-4970-B077-601460877759}"));
            if (authorFolder == null)
            {
                return string.Empty;
            }

            var authorName = ItemUtil.ProposeValidItemName(a.AuthorName).ToLower();
            var authorItem = authorFolder.GetChildren().FirstOrDefault(e => e.Name == authorName) ?? authorFolder.Add(authorName, authorTemplate);

            using (new EditContext(authorItem))
            {
                if (!string.IsNullOrEmpty(a.AuthorImageUrl))
                {
                    var imgId = CreateImageInSitecore(a.AuthorImageUrl);
                    if (!imgId.IsNull)
                    {

                        var imgField = (ImageField)authorItem.Fields["Image"];
                        imgField.Clear();
                        imgField.MediaID = imgId;
                    }
                }

                authorItem.Fields["Name"].SetValue(a.AuthorName, true);
                if (!string.IsNullOrEmpty(a.AuthorUrl))
                {
                    SetLinkField(authorItem, "URL", null, a.AuthorUrl);
                }
            }

            return authorItem.ID.ToString();

        }

        private static IEnumerable<string> GetDocumentPath(string rawUrl)
        {
            rawUrl = HttpUtility.UrlDecode(rawUrl.ToLower());
            rawUrl = rawUrl.Replace("/uploadedfiles/content", string.Empty).Replace("_", " ").Replace("-", " ").Replace("   ", " ").Replace("  ", " ").Replace("(", string.Empty).Replace(")", string.Empty);
            var lastPeriod = rawUrl.LastIndexOf(".", StringComparison.Ordinal);
            if (lastPeriod > 0)
            {
                rawUrl = rawUrl.Substring(0, lastPeriod);
            }

            if (!rawUrl.StartsWith("/"))
            {
                rawUrl = "/" + rawUrl;
            }

            if (rawUrl.StartsWith("/pages/"))
            {
                rawUrl = rawUrl.Replace("/pages/", "/");
            }

            var pathArray = rawUrl.Split('/');
            return (from p in pathArray where !string.IsNullOrEmpty(p) select ItemUtil.ProposeValidItemName(p.Trim())).ToList();
        }

        public List<EktronAtoZ> GetEktronAtoZ()
        {
            var list = new List<EktronAtoZ>();
            using (var pkg = new ExcelPackage())
            {
                using (var stream = System.IO.File.OpenRead(@"C:\ComponentDump2\Ektron_Taxonomy_2.xlsx"))
                {
                    pkg.Load(stream);
                }

                var ws = pkg.Workbook.Worksheets.First();
                for (var rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var c1 = ws.Cells[rowNum, 1].FirstOrDefault();
                    var c3 = ws.Cells[rowNum, 3].FirstOrDefault();
                    var c5 = ws.Cells[rowNum, 5].FirstOrDefault();
                    var c7 = ws.Cells[rowNum, 7].FirstOrDefault();
                    var c8 = ws.Cells[rowNum, 8].FirstOrDefault();
                    var c23 = ws.Cells[rowNum, 23].FirstOrDefault();
                    var c24 = ws.Cells[rowNum, 24].FirstOrDefault();

                    list.Add(new EktronAtoZ
                    {
                        ID = c1 != null ? c1.Text : string.Empty,
                        Name = c3 != null ? c3.Text : string.Empty,
                        ParentID = c5 != null ? c5.Text : string.Empty,
                        Path = c7 != null ? c7.Text : string.Empty,
                        DisplayOrder = c8 != null ? c8.Text : string.Empty,
                        IdPath = c23 != null ? c23.Text : string.Empty,
                        ItemID = c24 != null ? c24.Text : string.Empty
                    });
                }
            }

            return list;
        }

        public List<EktronDocument> GetEktronDocuments(bool isDms = true)
        {
            var list = new List<EktronDocument>();
            using (var pkg = new ExcelPackage())
            {
                if (isDms)
                {
                    using (var stream = System.IO.File.OpenRead(@"C:\ComponentDump2\DMS_Documents.xlsx"))
                    {
                        pkg.Load(stream);
                    }
                }
                else
                {
                    using (var stream = System.IO.File.OpenRead(@"C:\ComponentDump2\Ektron_documents.xlsx"))
                    {
                        pkg.Load(stream);
                    }
                }

                var ws = pkg.Workbook.Worksheets.First();
                for (var rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var c3 = ws.Cells[rowNum, 3].FirstOrDefault();
                    var c4 = ws.Cells[rowNum, 4].FirstOrDefault();

                    if (isDms)
                    {
                        list.Add(new EktronDocument
                        {
                            Url = c3 != null ? c3.Text : string.Empty,
                            Handle = c4 != null ? c4.Text : string.Empty
                        });
                    }
                    else
                    {
                        var url = c3 != null ? c3.Text : string.Empty;
                        list.Add(new EktronDocument
                        {
                            Url = url,
                            Handle = url.Substring(url.LastIndexOf("/", StringComparison.Ordinal) + 1)
                        });
                    }
                }
            }

            return list;
        }

        public List<EktronMenu> GetEktronMenus(out Dictionary<string, string> dict)
        {
            var list = new List<EktronMenu>();
            using (var pkg = new ExcelPackage())
            {
                using (var stream = System.IO.File.OpenRead(@"C:\ComponentDump2\Ektron_Menus_2.xlsx"))
                {
                    pkg.Load(stream);
                }

                var ws = pkg.Workbook.Worksheets.First();
                for (var rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var idCell = ws.Cells[rowNum, 1].FirstOrDefault();
                    var nameCell = ws.Cells[rowNum, 2].FirstOrDefault();
                    var typeCell = ws.Cells[rowNum, 11].FirstOrDefault();
                    var linkCell = ws.Cells[rowNum, 12].FirstOrDefault();
                    var parentIdCell = ws.Cells[rowNum, 14].FirstOrDefault();
                    var ancestorIdCell = ws.Cells[rowNum, 15].FirstOrDefault();
                    var id = idCell != null ? idCell.Text : string.Empty;
                    var name = nameCell != null ? nameCell.Text : string.Empty;
                    var menuType = typeCell != null ? typeCell.Text : string.Empty;
                    var link = linkCell != null ? linkCell.Text.ToLower().Trim() : string.Empty;
                    var parentId = parentIdCell != null ? parentIdCell.Text : string.Empty;
                    var ancestorId = ancestorIdCell != null ? ancestorIdCell.Text : string.Empty;

                    list.Add(new EktronMenu
                    {
                        ID = id,
                        Name = name,
                        Link = link,
                        MenuType = menuType,
                        ParentID = parentId,
                        AncestorID = ancestorId
                    });
                }
            }

            using (var pkg = new ExcelPackage())
            {
                using (var stream = System.IO.File.OpenRead(@"C:\ComponentDump2\menu_order.xlsx"))
                {
                    pkg.Load(stream);
                }

                dict = new Dictionary<string, string>();

                var ws = pkg.Workbook.Worksheets.First();
                for (var rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var idCell = ws.Cells[rowNum, 2].FirstOrDefault();
                    var orderCell = ws.Cells[rowNum, 7].FirstOrDefault();
                    var id = idCell != null ? idCell.Text : string.Empty;
                    var order = orderCell != null ? orderCell.Text : string.Empty;

                    if (!string.IsNullOrEmpty(id) && !dict.ContainsKey(id))
                    {
                        dict.Add(id, order);
                    }
                }
            }

            return list;
        }

        public List<EktronNews> GetEktronNews()
        {
            var list = new List<EktronNews>();
            using (var pkg = new ExcelPackage())
            {
                using (var stream = System.IO.File.OpenRead(@"C:\ComponentDump2\News_release_data.xlsx"))
                {
                    pkg.Load(stream);
                }

                var ws = pkg.Workbook.Worksheets.First();
                for (var rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var c1 = ws.Cells[rowNum, 1].FirstOrDefault();
                    var c3 = ws.Cells[rowNum, 3].FirstOrDefault();
                    var c4 = ws.Cells[rowNum, 4].FirstOrDefault();
                    var c6 = ws.Cells[rowNum, 6].FirstOrDefault();
                    var c18 = ws.Cells[rowNum, 18].FirstOrDefault();

                    var n = new EktronNews
                    {
                        ID = c1 != null ? c1.Text : string.Empty,
                        Title = c3 != null ? c3.Text : string.Empty,
                        Content = c4 != null ? c4.Text : string.Empty,
                        DateCreated = c6 != null ? c6.Text : string.Empty,
                        Teaser = c18 != null ? c18.Text : string.Empty
                    };

                    n.FromXmlString();
                    list.Add(n);
                }
            }

            return list;
        }

        public List<EktronNewsMetaData> GetEktronNewsMetaData()
        {
            var list = new List<EktronNewsMetaData>();
            using (var pkg = new ExcelPackage())
            {
                using (var stream = System.IO.File.OpenRead(@"C:\ComponentDump2\HSGMetaData.xlsx"))
                {
                    pkg.Load(stream);
                }

                var ws = pkg.Workbook.Worksheets.First();
                for (var rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var c1 = ws.Cells[rowNum, 1].FirstOrDefault();
                    var c2 = ws.Cells[rowNum, 2].FirstOrDefault();
                    var c4 = ws.Cells[rowNum, 4].FirstOrDefault();

                    if (c1 != null && c1.Text == "12")
                    {
                        list.Add(new EktronNewsMetaData
                        {
                            ID = c2 != null ? c2.Text : string.Empty,
                            Tags = c4 != null ? c4.Text : string.Empty
                        });
                    }

                }
            }

            return list;
        }

        public List<EktronPageID> GetEktronPageIDs()
        {
            var list = new List<EktronPageID>();
            var fi = new FileInfo("C:\\ComponentDump2\\page_ids.txt");
            try
            {
                using (var stream = new StreamReader(fi.FullName, Encoding.UTF8))
                {

                    string line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        if (!line.Contains("::"))
                        {
                            continue;
                        }

                        var arr = line.Split(':');
                        if (arr.Length == 3)
                        {
                            list.Add(new EktronPageID { Path = arr[0], ID = arr[2] });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _sb.Append("GetExtronPageIDs: " + e.Message);
            }

            return list;
        }

        public List<EktronRelatedLink> GetEktronRelatedLinks()
        {
            var list = new List<EktronRelatedLink>();
            using (var pkg = new ExcelPackage())
            {
                using (var stream = System.IO.File.OpenRead(@"C:\ComponentDump2\Related_Links_prod.xlsx"))
                {
                    pkg.Load(stream);
                }

                var ws = pkg.Workbook.Worksheets.First();
                for (var rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var c1 = ws.Cells[rowNum, 1].FirstOrDefault();
                    var c2 = ws.Cells[rowNum, 2].FirstOrDefault();
                    var c3 = ws.Cells[rowNum, 3].FirstOrDefault();

                    list.Add(new EktronRelatedLink
                    {
                        ID = c1 != null ? c1.Text : string.Empty,
                        Text = c2 != null ? c2.Text : string.Empty,
                        Link = c3 != null ? GetRelatedLink(c3.Text) : null
                    });
                }
            }

            return list;
        }

        public List<HsgMetaData> GetHsgMetaData()
        {
            var list = new List<HsgMetaData>();
            using (var pkg = new ExcelPackage())
            {
                using (var stream = System.IO.File.OpenRead(@"C:\ComponentDump2\HSGMetaData.xlsx"))
                {
                    pkg.Load(stream);
                }

                var ws = pkg.Workbook.Worksheets.First();
                for (var rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var c1 = ws.Cells[rowNum, 1].FirstOrDefault();
                    var c2 = ws.Cells[rowNum, 2].FirstOrDefault();
                    var c4 = ws.Cells[rowNum, 4].FirstOrDefault();
                    var c7 = ws.Cells[rowNum, 7].FirstOrDefault();

                    if (c1 != null && (c1.Text == "149" || c1.Text == "152"))
                    {
                        list.Add(new HsgMetaData
                        {
                            MetaTypeID = c1.Text,
                            ContentID = c2 != null ? c2.Text : string.Empty,
                            MetaValue = c4 != null ? c4.Text : string.Empty,
                            MetaName = c7 != null ? c7.Text : string.Empty
                        });
                    }
                }
            }

            return list;
        }

        private static void GetImageUrlAndPath(string rawUrl, out string url, out string path, out string fileName)
        {
            rawUrl = rawUrl.Trim();
            url = string.Empty;
            path = string.Empty;
            fileName = string.Empty;

            if (rawUrl.StartsWith("//"))
            {
                url = "https:" + rawUrl;
            }
            else if (rawUrl.StartsWith("/") || rawUrl.StartsWith("https://www.allinahealth.org//"))
            {
                url = "https://www.allinahealth.org" + rawUrl.Replace("https://www.allinahealth.org//", "/");
            }
            else if (rawUrl.StartsWith("../") || rawUrl.StartsWith("/.."))
            {
                while (rawUrl.Contains("../"))
                {
                    rawUrl = rawUrl.Replace("../", string.Empty);
                }

                if (!rawUrl.StartsWith("/"))
                {
                    rawUrl = "/" + rawUrl;
                }

                if (!rawUrl.StartsWith("/"))
                {
                    rawUrl += "/" + rawUrl;
                }

                url = "https://www.allinahealth.org" + rawUrl;
            }
            else if (rawUrl.StartsWith("https://www.allinahealth.org/"))
            {
                url = rawUrl;
            }

            if (!url.StartsWith("https://www.allinahealth.org") && !url.StartsWith("https://content.wellclicks.com"))
            {
                return;
            }

            while (url.Contains("?"))
            {
                var position = url.LastIndexOf("?", StringComparison.Ordinal);
                url = url.Substring(0, position);
            }

            url = url.ToLower();

            if (url.EndsWith("\""))
            {
                url = url.Substring(0, url.Length - 1);
            }

            path = url.Replace("https://www.allinahealth.org", string.Empty);
            path = path.Replace("https://content.wellclicks.com", string.Empty);
            path = path.Replace("/uploadedimages", string.Empty);
            path = path.Replace("/uploadedfiles", string.Empty);
            path = path.Replace("™", string.Empty);
            fileName = url.Substring(url.LastIndexOf("/", StringComparison.Ordinal) + 1);
            fileName = fileName.Replace("(", string.Empty).Replace(")", string.Empty).Replace(",", string.Empty).Trim();
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            var lastIndexOfDot = path.LastIndexOf(".", StringComparison.Ordinal);
            if (lastIndexOfDot > 0)
            {
                path = path.Substring(0, lastIndexOfDot);
            }

            path = path.Replace("’", string.Empty).Replace("®", string.Empty).Replace("_", " ").Replace("=", string.Empty).Replace("-", " ").Replace("(", string.Empty).Replace(")", string.Empty).Replace(".", " ").Replace(",", string.Empty).Replace("'", string.Empty).Trim();

            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            var pathArray = path.Split('/');
            for (var i = 0; i < pathArray.Length; i++)
            {
                pathArray[i] = pathArray[i].Trim();
            }

            path = string.Join("/", pathArray);
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
        }

        private Item GetMediaItem(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            var reUrls = new Regex("<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);
            var results = reUrls.Matches(s).Cast<Match>().Select(match => match.Groups[1].Value).ToArray();
            foreach (var r in results)
            {
                GetImageUrlAndPath(r, out _, out var path, out _);

                var item = _db.GetItem("/sitecore/media library/Allina Health" + path);
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        private Item GetMediaItem2(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            GetImageUrlAndPath(s, out _, out var path, out _);

            var item = _db.GetItem("/sitecore/media library/Allina Health" + path);
            return item;
        }

        private static string GetMetaTagValue(HtmlNodeCollection nodes, string id, string attrName = "content")
        {
            id = id.ToLower();
            var n = nodes.FirstOrDefault(e => e.Attributes.FirstOrDefault(x => x.Value.ToLower() == id) != null);
            var attr = n?.Attributes.FirstOrDefault(e => e.Name == attrName);
            return attr != null ? attr.Value : string.Empty;
        }

        private RelatedLink GetRelatedLink(string xml)
        {
            try
            {
                if (xml.StartsWith("<root>"))
                {
                    var serializer = new XmlSerializer(typeof(RelatedLink));
                    using (TextReader reader = new StringReader(xml))
                    {
                        var result = (RelatedLink)serializer.Deserialize(reader);
                        return result;
                    }
                }

                _sb.AppendFormat("{0}<br /><br />", HttpUtility.HtmlEncode(xml));
            }
            catch (XmlException)
            {
                _sb.AppendFormat("XmlException: {0}<br /><br />", HttpUtility.HtmlEncode(xml));
            }
            catch (Exception)
            {
                _sb.AppendFormat("{0}<br /><br />", HttpUtility.HtmlEncode(xml));
            }

            return null;
        }

        private static string GetTagValue(HtmlNode n, string attrName)
        {
            var attr = n.Attributes.FirstOrDefault(e => e.Name == attrName);
            return attr != null ? attr.Value : string.Empty;
        }

        public string Hsg()
        {
            var jsonNonUtf8 = _webClient.DownloadString("https://api.wellclicks.com/HealthySetGo/ListArticlesByCategory?apiKey=3854eedad78f40c79297821a5a1c5e36&apiPassword=Uk2haxF2kRr/GnzbJG7vxg==&category=&count=1200");
            var jsonBytes = Encoding.Default.GetBytes(jsonNonUtf8);
            var json = Encoding.UTF8.GetString(jsonBytes);
            var list = JsonConvert.DeserializeObject<List<HsgMigrationModel>>(json);

            var metaDatas = GetHsgMetaData();
            var hsgHome = _db.GetItem(new ID("{887BAFA9-E997-46CD-80BA-6D3B60158B09}"));
            var categoryList = hsgHome.GetChildrenSafe();

            CreateRelatedCallsToAction(list);

            using (new SecurityDisabler())
            {
                foreach (var a in list)
                {
                    var categoryItem = categoryList.FirstOrDefault(e => e.Name.ToLower() == a.Category.ToLower());
                    if (categoryItem == null)
                    {
                        continue;
                    }

                    var articleName = ItemUtil.ProposeValidItemName(a.Title).ToLower();
                    var article = categoryItem.GetChildren().FirstOrDefault(e => e.Name == articleName);
                    if (article == null)
                    {
                        continue;
                        //isNew = true;
                        //article = categoryItem.Add(articleName, articleTemplate);
                    }

                    var metaData = metaDatas.Where(e => e.ContentID == a.EktronId.ToString()).ToList();

                    using (new EditContext(article))
                    {
                        if (metaData.Count <= 0)
                        {
                            continue;
                        }

                        var desc = metaData.FirstOrDefault(e => e.MetaTypeID == "149")?.MetaValue;
                        var title = metaData.FirstOrDefault(e => e.MetaTypeID == "152")?.MetaValue;
                        desc = HttpUtility.HtmlDecode(desc);
                        title = HttpUtility.HtmlDecode(title);

                        article.Fields["Page Title"].SetValue(a.Title, true);
                        article.Fields["OG Title"].SetValue(a.Title, true);
                        if (!string.IsNullOrEmpty(desc))
                        {
                            article.Fields["Meta Description"].SetValue(desc, true);
                            article.Fields["OG Description"].SetValue(desc, true);
                        }

                        if (!string.IsNullOrEmpty(title))
                        {
                            article.Fields["Browser Title"].SetValue(title, true);
                        }
                    }
                }
            }

            PrintImagesForAction();

            return _sb.ToString();
        }

        public string ImportAtoZ()
        {
            var list = GetEktronAtoZ();
            var pages = GetEktronPageIDs();

            var dict = new Dictionary<string, EktronAtoZ>();
            foreach (var a in list)
            {
                if (!dict.ContainsKey(a.ID))
                {
                    dict.Add(a.ID, a);
                }
                else if (dict[a.ID].ItemID != a.ItemID && a.ID == "3714")
                {
                    dict[a.ID] = a;
                }
            }

            var atozRefFolder = _db.GetItem(new ID("{4D5A0CB1-9724-4157-9865-119EE8D20F26}"));
            var atozRefTemplate = _db.GetTemplate(new ID("{263EB8CC-B73B-4AD4-8CBC-1DB89272BAD5}"));
            using (new SecurityDisabler())
            {
                foreach (var a in dict.Values.OrderBy(e => e.Path))
                {
                    var p = pages.FirstOrDefault(e => e.ID == a.ItemID);
                    if (p == null)
                    {
                        continue;
                    }

                    {
                        var path = p.Path.Replace("Pages/", HomePath + "/").Replace("-", " ").Replace("&", "and");
                        var i = _db.GetItem(path);
                        if (i == null)
                        {
                            path = FixPath(p.Path.Replace("Pages/", HomePath + "/"));
                            i = _db.GetItem(path);
                        }

                        if (i == null)
                        {
                            continue;
                        }

                        var folderPath = a.Path.ToLower().Replace("\\a to z", string.Empty);
                        var pos = folderPath.LastIndexOf("\\", StringComparison.Ordinal);
                        if (pos <= 0)
                        {
                            _sb.AppendFormat("{0}<br/>", folderPath);
                            continue;
                        }

                        folderPath = folderPath.Substring(0, pos).Replace("\\", "/");
                        var f = CreateFolderStructure(atozRefFolder, folderPath, "{7CB9F2D2-7485-457C-8585-1AF05BA1C019}");
                        var n = ItemUtil.ProposeValidItemName(a.Name).ToLower();
                        var atozRef = f.GetChildren().FirstOrDefault(e => e.Name == n) ?? f.Add(n, atozRefTemplate);

                        using (new EditContext(atozRef))
                        {
                            SetLinkField(atozRef, "Link", null, i.ID.ToString());
                            atozRef.Fields["Link Text"].SetValue(a.Name, true);
                        }
                    }
                }
            }

            var atozFolder = _db.GetItem(new ID("{C2904675-8885-4006-B210-4A336DD26771}"));
            var atozTemplate = _db.GetTemplate(new ID("{D85D1D2A-5AD0-4CE6-9514-E5D94FEA5E99}"));
            if (atozFolder == null)
            {
                return _sb.ToString();
            }

            using (new SecurityDisabler())
            {
                foreach (Item refFolder in atozRefFolder.GetChildren())
                {
                    var atozItem = atozFolder.GetChildren().FirstOrDefault(e => e.Name == refFolder.Name) ?? atozFolder.Add(refFolder.Name, atozTemplate);

                    var refList = refFolder.Axes.GetDescendants().Where(e => e.TemplateID == atozRefTemplate.ID).ToList().Select(e => e.ID.ToString());
                    using (new EditContext(atozItem))
                    {
                        atozItem.Fields["Links"].SetValue(string.Join("|", refList), true);
                    }
                }
            }

            return _sb.ToString();
        }

        public string ImportReatedLinks()
        {
            _sb.Append("<h2>Related Links 2</h2>");
            var folder = _db.GetItem(new ID("{0A4BC59F-D776-4402-AF94-CC4F67EDD217}"));
            var templateId = new TemplateID(new ID("{9089153D-F8C3-40EB-995A-6681B79E59F6}"));
            var list = GetEktronRelatedLinks();
            var urls = new List<string>();


            using (new SecurityDisabler())
            {
                foreach (var r in list)
                {
                    if (r.Link?.RelatedLinks?.Link?.a == null)
                    {
                        continue;
                    }

                    var a = r.Link.RelatedLinks.Link.a;
                    var href = a.Href;
                    if (href.EndsWith("/"))
                    {
                        href = href.Substring(0, href.LastIndexOf("/", StringComparison.Ordinal));
                    }

                    var name = string.Empty;
                    try
                    {
                        name = ItemUtil.ProposeValidItemName(r.Text).ToLower();
                    }
                    catch (Exception)
                    {
                        _sb.AppendFormat("Item Name: {0} ----- {1}<br />", a.Title, r.Text);
                    }

                    if (name.Length >= 100)
                    {
                        name = name.Substring(0, 100);
                    }

                    name = name.Trim();

                    var newItem = _db.GetItem(folder.Paths.FullPath + "/" + name.Substring(0, 1) + "/" + name);

                    if (newItem == null)
                    {
                        try
                        {
                            newItem = folder.Add(name, templateId);
                        }
                        catch (DuplicateItemNameException)
                        {
                            _sb.AppendFormat("Duplicate Name: {0}<br />", name);
                        }
                    }

                    if (newItem == null)
                    {
                        continue;
                    }

                    var linkedItem = FindMenuItem(href);
                    if (linkedItem != null)
                    {
                        using (new EditContext(newItem))
                        {
                            SetLinkField(newItem, "Link", null, linkedItem.ID.ToString());
                            newItem.Fields["Icon"].SetValue("{AFC8F47F-2B59-42F1-9D2A-5651F68515D6}", true);
                            newItem.Fields["Link Text"].SetValue(a.Title, true);
                        }
                    }
                    else if (href.StartsWith("http"))
                    {
                        using (new EditContext(newItem))
                        {
                            SetLinkField(newItem, "Link", new EktronMenu { Link = href });
                            if (newItem.Fields["Icon"] != null)
                            {
                                newItem.Fields["Icon"].SetValue("{AFC8F47F-2B59-42F1-9D2A-5651F68515D6}", true);
                                newItem.Fields["Link Text"].SetValue(a.Title, true);
                            }
                            else
                            {
                                _sb.AppendFormat("FIELDS: {0}<br/>", newItem.Paths.Path);
                            }
                        }
                    }
                    else
                    {
                        urls.Add(href);
                    }
                }
            }

            foreach (var u in urls.OrderBy(e => e))
            {
                _sb.AppendFormat("{0}<br/>", u);
            }

            return _sb.ToString();
        }

        public string Migrate()
        {
            var xml = _webClient.DownloadString("https://www.allinahealth.org/sitemap.xml");
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var manager = new XmlNamespaceManager(xmlDoc.NameTable);
            if (xmlDoc.DocumentElement != null) manager.AddNamespace("s", xmlDoc.DocumentElement.NamespaceURI);

            var list = new List<string> { "https://www.allinahealth.org/" };
            list.AddRange(from XmlNode node in xmlDoc.SelectNodes("/s:urlset/s:url/s:loc", manager) select node.InnerText);
            list = list.OrderBy(e => e).ToList();

            IndexCustodian.PauseIndexing();

            CreatePages(list);
            ProcessImages();

            IndexCustodian.ResumeIndexing();

            Log.Error(_sb.ToString().Replace("<br/>", Environment.NewLine), this);

            _sb.Append("<br/>:)");
            return _sb.ToString();
        }

        public string News()
        {
            var folder = _db.GetItem(new ID("{C2EA4ED1-C45E-4B40-B59D-8B038433C616}"));
            var templateId = new TemplateID(new ID("{75FBD114-524E-4B57-B5CE-445C97D7C320}"));
            var list = GetEktronNews().Where(e => e.IsFromXml);
            var listMetaData = GetEktronNewsMetaData();
            var regions = new Dictionary<string, string>();
            var categories = new Dictionary<string, string>();
            var contacts = new Dictionary<string, string>();

            var ektronNewsEnumerable = list as EktronNews[] ?? list.ToArray();
            foreach (var n in ektronNewsEnumerable)
            {
                if (!string.IsNullOrEmpty(n.NewsRegion) && !regions.ContainsKey(n.NewsRegion))
                {
                    regions.Add(n.NewsRegion, string.Empty);
                }

                foreach (var tag in listMetaData.Where(e => e.ID == n.ID).Select(e => e.Tags))
                {
                    foreach (var c in tag.Split(';'))
                    {
                        if (string.IsNullOrEmpty(c))
                        {
                            continue;
                        }

                        var temp = c.Replace("Abbot Northwestern Hospital WestHealth", "Abbott Northwestern Hospital WestHealth");
                        if (!categories.ContainsKey(temp))
                        {
                            categories.Add(temp, string.Empty);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(n.ContactEmail) && !contacts.ContainsKey(n.ContactEmail))
                {
                    contacts.Add(n.ContactEmail, n.ContactInformation);
                }
            }

            using (new SecurityDisabler())
            {
                regions = CreateNewsRegions(regions);
                categories = CreateNewsCategories(categories);
                contacts = CreateNewsContacts(contacts);

                foreach (var n in ektronNewsEnumerable)
                {
                    var name = ItemUtil.ProposeValidItemName(n.Title.Replace("'", string.Empty)).ToLower();
                    var parentItem = folder.GetChildrenSafe().FirstOrDefault(e => e.Name.Contains(n.DateValue.Year.ToString()));
                    if (parentItem == null)
                    {
                        _sb.AppendFormat("bad date: {0}<br/>", n.Title);
                        continue;
                    }

                    var item = _db.GetItem(parentItem.Paths.FullPath + "/" + name) ?? parentItem.Add(name, templateId);

                    using (new EditContext(item))
                    {
                        item.Fields["Page Title"].SetValue(n.Header, true);
                        //Removed Location & Date fields below, commented-out code can be removed if/when everything is good to go
                        //item.Fields["Location"].SetValue(n.Location, true);
                        //item.Fields["Article Date"].SetValue(DateUtil.ToIsoDate(n.DateValue), true);
                        item.Fields["Content"].SetValue(ReplaceImageSrc(n.NewsContent.StripHtmlClassAndStyleAttributes()), true);

                        if (!string.IsNullOrEmpty(n.ContactEmail))
                        {
                            item.Fields["Contact"].SetValue(contacts[n.ContactEmail], true);
                        }
                        else
                        {
                            _sb.AppendFormat("no contact: {0}<br/>", item.Paths.FullPath);
                        }

                        if (!string.IsNullOrEmpty(n.NewsRegion))
                        {
                            item.Fields["Region"].SetValue(regions[n.NewsRegion], true);
                        }

                        var itemCategories = new ListString();
                        foreach (var tag in listMetaData.Where(e => e.ID == n.ID).Select(e => e.Tags))
                        {
                            foreach (var c in tag.Split(';'))
                            {
                                if (!string.IsNullOrEmpty(c) && categories.ContainsKey(c))
                                {
                                    itemCategories.Add(categories[c]);
                                }
                            }
                        }

                        item.Fields["Categories"].SetValue(itemCategories.ToString(), true);
                    }
                }
            }

            PrintImagesForAction();
            return _sb.ToString();
        }

        private Dictionary<string, string> CreateNewsCategories(Dictionary<string, string> dict)
        {
            var result = new Dictionary<string, string>();
            var folder = _db.GetItem(new ID("{35A4FD8B-9EA2-4323-84E9-2A5CF08E8DE3}"));
            var templateId = new TemplateID(new ID("{542160DB-1D3E-461E-888D-485F68B27621}"));
            foreach (var d in dict.Keys)
            {
                var name = ItemUtil.ProposeValidItemName(d).ToLower();
                var item = _db.GetItem(folder.Paths.FullPath + "/" + name) ?? folder.Add(name, templateId);
                result.Add(d, item.ID.ToString());
                using (new EditContext(item))
                {
                    item.Fields["Name"].SetValue(d, true);
                }
            }

            return result;
        }

        private Dictionary<string, string> CreateNewsRegions(Dictionary<string, string> dict)
        {
            var result = new Dictionary<string, string>();
            var folder = _db.GetItem(new ID("{D2E32860-9833-40FB-BC35-7DA0B2331E50}"));
            var templateId = new TemplateID(new ID("{A29A5D0B-A01B-4236-A934-6A27546CF8B3}"));
            foreach (var d in dict.Keys)
            {
                var name = ItemUtil.ProposeValidItemName(d).ToLower();
                var item = _db.GetItem(folder.Paths.FullPath + "/" + name) ?? folder.Add(name, templateId);
                result.Add(d, item.ID.ToString());
                using (new EditContext(item))
                {
                    item.Fields["Name"].SetValue(d, true);
                }
            }

            return result;
        }

        private Dictionary<string, string> CreateNewsContacts(Dictionary<string, string> dict)
        {
            var result = new Dictionary<string, string>();
            var folder = _db.GetItem(new ID("{ED81E3BF-43C9-4CFC-B704-D4979EFAA639}"));
            var templateId = new TemplateID(new ID("{9A29D874-88F3-400A-95A9-010812CB736F}"));
            foreach (var d in dict.Keys)
            {
                if (d == "http://")
                {
                    continue;
                }

                var proposedName = d.Substring(0, d.IndexOf("@", StringComparison.Ordinal)).Replace(".", " ");
                var name = ItemUtil.ProposeValidItemName(proposedName).ToLower();
                var item = _db.GetItem(folder.Paths.FullPath + "/" + name) ?? folder.Add(name, templateId);
                result.Add(d, item.ID.ToString());
                using (new EditContext(item))
                {
                    item.Fields["Name"].SetValue(name, true);
                    item.Fields["Email"].SetValue(d, true);
                }
            }

            return result;
        }

        private void PrintImagesForAction()
        {
            _sb.Append("<h2>Images</h2>");
            foreach (var img in _imagesForAction.OrderBy(e => e))
            {
                _sb.AppendFormat("{0}<br/>", img);
            }

            _sb.Append("<br/><br/>");
        }

        // ReSharper disable once UnusedMember.Local
        private void ProcessAccordion(List<EktronAccordion> list)
        {
            var folder = _db.GetItem(new ID("{3A3CDA3D-E72E-4227-ADA6-52AC5077400F}"));
            var templateId = new TemplateID(new ID("{70C07EF9-32EC-4257-B79A-A13B38E6EB75}"));
            if (folder.IsNull())
            {
                return;
            }

            using (new SecurityDisabler())
            {
                foreach (var ekt in list)
                {
                    var folderItem = CreateFolderStructure(folder, ekt.Info.Url, "{2C2FAF76-66A5-4852-95BF-70FA032E9560}");
                    var count = 1;
                    foreach (var cg in ekt.ContentGroup)
                    {
                        var proposedName = cg.LinkText;
                        if (string.IsNullOrEmpty(proposedName))
                        {
                            proposedName = "accordion entry " + count;
                            count++;
                        }

                        var name = ItemUtil.ProposeValidItemName(proposedName).ToLower();
                        var newItem = _db.GetItem(folderItem.Paths.FullPath + "/" + name) ?? folderItem.Add(name, templateId);

                        using (new EditContext(newItem))
                        {
                            newItem.Fields["Heading"].SetValue(proposedName, true);
                            newItem.Fields["Description"].SetValue(ReplaceImageSrc(cg.Content.StripHtmlClassAndStyleAttributes()), true);
                        }
                    }
                }
            }
        }

        private void ProcessContentBlocks(List<EktronInfo> list)
        {
            var folder = _db.GetItem(new ID("{6F8563A0-1C38-4619-B556-A92AAE3F9E55}"));
            var templateId = new TemplateID(new ID("{CB678537-26D1-4543-8875-9B7D05D694CC}"));
            if (folder.IsNull())
            {
                return;
            }

            using (new SecurityDisabler())
            {
                foreach (var ei in list)
                {
                    var folderItem = CreateFolderStructure(folder, ei.Url, "{5545F215-F734-4CFC-A66B-D207E2DA23AC}");
                    var name = ItemUtil.ProposeValidItemName(ei.Title).ToLower();
                    var newItem = _db.GetItem(folderItem.Paths.FullPath + "/" + name) ?? folderItem.Add(name, templateId);

                    using (new EditContext(newItem))
                    {
                        var value = ReplaceImageSrc(ei.Data.StripHtmlClassAndStyleAttributes());
                        newItem.Fields["Content"].SetValue(value, true);
                    }
                }
            }
        }

        private void ProcessEktronData(IEnumerable<EktronInfo> list)
        {
            var cbList = list.Where(e => e.WidgetName == "ContentBlock.ascx").ToList();
            ProcessContentBlocks(cbList);

            //var tempList = list.Where(e => e.WidgetName == "allina-google-map.ascx").ToList();
            //var gmList = tempList.GetList<EktronGoogleMap>();
            //ProcessGoogleMaps(gmList);
            //_sb.Append("gmList count: " + gmList.Count.ToString() + ", temp: " + tempList.Where(e => !string.IsNullOrEmpty(e.Data)).Count().ToString() + "<br />");

            //tempList = list.Where(e => e.WidgetName == "allina-accordion-content.ascx").ToList();
            //var acList = tempList.GetList<EktronAccordion>();
            //ProcessAccordion(acList);
            //_sb.Append("acList count: " + acList.Count.ToString() + ", temp: " + tempList.Where(e => !string.IsNullOrEmpty(e.Data)).Count().ToString() + "<br />");

            //tempList = list.Where(e => e.WidgetName == "allina-health-information.ascx").ToList();
            //var hiList = tempList.GetList<EktronHealthInfo>();
            //ProcessHealthInfo(hiList);
            //_sb.Append("hiList count: " + hiList.Count.ToString() + ", temp: " + tempList.Where(e => !string.IsNullOrEmpty(e.Data)).Count().ToString() + "<br />");

            //var tempList = list.Where(e => e.WidgetName == "allina-recipes.ascx").ToList();
            //var rList = tempList.GetList<EktronRecipe>();
            //ProcessRecipes(rList);
            //_sb.Append("rList count: " + rList.Count.ToString() + ", temp: " + tempList.Where(e => !string.IsNullOrEmpty(e.Data)).Count().ToString() + "<br />");

            //tempList = list.Where(e => e.WidgetName == "allina-photogallery.ascx").ToList();
            //var pList = tempList.GetList<EktronPhotoGallery>();
            //_sb.Append("pList count: " + pList.Count.ToString() + ", temp: " + tempList.Where(e => !string.IsNullOrEmpty(e.Data)).Count().ToString() + "<br />");

        }

        // ReSharper disable once UnusedMember.Local
        private void ProcessGoogleMaps(List<EktronGoogleMap> list)
        {
            var folder = _db.GetItem(new ID("{816213FD-FF01-4770-BE81-20B009918E10}"));
            var templateId = new TemplateID(new ID("{7795C3E3-ACA9-4A40-BDD8-88CCD83DF046}"));
            if (folder.IsNull())
            {
                return;
            }

            using (new SecurityDisabler())
            {
                foreach (var ekt in list)
                {
                    var folderItem = CreateFolderStructure(folder, ekt.Info.Url, "{FA2D766C-BE4C-4FFF-938A-BB0BCABE440A}");
                    var name = ItemUtil.ProposeValidItemName(ekt.Info.Title).ToLower();
                    var newItem = _db.GetItem(folderItem.Paths.FullPath + "/" + name) ?? folderItem.Add(name, templateId);

                    using (new EditContext(newItem))
                    {
                        newItem.Fields["Location Name"].SetValue(ekt.Info.Title, true);
                        newItem.Fields["Address Line 1"].SetValue(ekt.Address1, true);
                        newItem.Fields["Address Line 2"].SetValue(ekt.Address2, true);
                        newItem.Fields["City"].SetValue(ekt.City, true);
                        newItem.Fields["State"].SetValue(ekt.State, true);
                        newItem.Fields["Zip"].SetValue(ekt.ZipCode, true);
                        newItem.Fields["Phone Number"].SetValue(ekt.Phone, true);
                        newItem.Fields["Latitude"].SetValue(ekt.Latitude, true);
                        newItem.Fields["Longitude"].SetValue(ekt.Longitude, true);
                        newItem.Fields["Hide Address"].SetValue(ekt.HideAddress == "Y" ? "1" : string.Empty, true);
                        newItem.Fields["Hide Map"].SetValue(ekt.HideMap == "Y" ? "1" : string.Empty, true);
                    }
                }
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void ProcessHealthInfo(List<EktronHealthInfo> list)
        {
            var folder = _db.GetItem(new ID("{48EDB459-EEDF-4FEE-843C-AF42AE828182}"));
            var templateId = new TemplateID(new ID("{27270A6F-A01D-4F98-9AF3-31759499F047}"));
            if (folder.IsNull())
            {
                return;
            }

            using (new SecurityDisabler())
            {
                foreach (var ekt in list)
                {
                    var folderItem = CreateFolderStructure(folder, ekt.Info.Url, "{3B27175E-153C-4994-812C-DED61492369C}");
                    var name = ItemUtil.ProposeValidItemName(ekt.Info.Title).ToLower();
                    var newItem = _db.GetItem(folderItem.Paths.FullPath + "/" + name) ?? folderItem.Add(name, templateId);

                    using (new EditContext(newItem))
                    {
                        newItem.Fields["Source"].SetValue(ReplaceImageSrc(ekt.Source.StripHtmlClassAndStyleAttributes()), true);
                        newItem.Fields["Reviewed By"].SetValue(ReplaceImageSrc(ekt.ReviewedBy.StripHtmlClassAndStyleAttributes()), true);

                        if (DateTime.TryParse(ekt.FirstPublished, out var dt))
                        {
                            newItem.Fields["First Published"].SetValue(DateUtil.ToIsoDate(dt), true);
                        }

                        if (DateTime.TryParse(ekt.LastReviewed, out dt))
                        {
                            newItem.Fields["Last Reviewed"].SetValue(DateUtil.ToIsoDate(dt), true);
                        }
                    }
                }
            }
        }

        private void ProcessImages()
        {
            foreach (var img in _images.Keys)
            {
                CreateImageInSitecore(img);
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void ProcessRecipes(List<EktronRecipe> list)
        {
            var recipesPage = _db.GetItem(new ID("{7A2EAA3F-5100-4C9D-A0DB-86E3C7848322}"));
            var templateItem = _db.GetTemplate(new ID("{AC49F5B2-D7CD-4A2D-9CAA-29ECE9DD04C2}"));
            var parentTemplateId = new ID("{7F902F0E-6371-4A08-A9D2-F881F8BA878C}");
            var parentTemplateItem = _db.GetTemplate(parentTemplateId);

            if (recipesPage.IsNull())
            {
                return;
            }

            using (new SecurityDisabler())
            {
                foreach (var ekt in list)
                {
                    var path = HomePath + ekt.Info.Url.ToLower().Substring(5);
                    path = path.Replace("-", " ").Replace(",", string.Empty).Replace("'", string.Empty).Replace("&", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace("!", string.Empty).Replace("’", string.Empty).Replace(((char)39).ToString(), string.Empty).Replace("#39;", string.Empty).Replace(" amp; ", " ").Replace("é", "e").Replace("grandmas apple crisp", "grandma apple crisp");

                    var item = _db.GetItem(path);
                    if (item.IsNull())
                    {
                        var parentPath = path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal));
                        var parentItem = _db.GetItem(parentPath);
                        if (parentItem != null)
                        {
                            if (parentItem.TemplateID != parentTemplateId)
                            {
                                parentItem.ChangeTemplate(parentTemplateItem);
                            }

                            var itemName = path.Substring(path.LastIndexOf("/", StringComparison.Ordinal));
                            itemName = ItemUtil.ProposeValidItemName(itemName).ToLower();
                            item = parentItem.Add(itemName, templateItem);
                        }
                    }

                    if (item != null)
                    {
                        item.ChangeTemplate(templateItem);
                        var parent = item.Parent;
                        if (parent != null && parent.TemplateID != parentTemplateId)
                        {
                            parent.ChangeTemplate(parentTemplateItem);
                        }

                        using (new EditContext(item))
                        {
                            item.Fields["Page Title"].SetValue(ekt.RecipesName, true);

                            var content = "<p>" + ekt.RecipesDescription + "</p>";
                            content += "<h2>Ingredients</h2><p>" + ekt.RecipesIngredients + "</p>";
                            content += "<h2>Directions</h2><p>" + ekt.RecipesDirections + "</p>";
                            content += ekt.RecipesServings;
                            item.Fields["Content"].SetValue(content, true);
                            item.Fields["Heart Smart"].SetValue(ekt.RecipesContains.Any(e => e == "H") ? "1" : string.Empty, true);
                            item.Fields["Low Sodium"].SetValue(ekt.RecipesContains.Any(e => e == "L") ? "1" : string.Empty, true);
                            item.Fields["Gluten Free"].SetValue(ekt.RecipesContains.Any(e => e == "G") ? "1" : string.Empty, true);

                            if (ekt.OptionGallery == null)
                            {
                                continue;
                            }

                            var mediaItem = GetMediaItem(ekt.OptionGallery.NutritionImages);
                            if (mediaItem != null)
                            {
                                var imgField = (ImageField)item.Fields["Nutrition Facts"];
                                imgField.Clear();
                                imgField.MediaID = mediaItem.ID;
                            }

                            mediaItem = GetMediaItem2(ekt.OptionGallery.Photo);
                            if (mediaItem != null)
                            {
                                var imgField = (ImageField)item.Fields["Image"];
                                imgField.Clear();
                                imgField.MediaID = mediaItem.ID;
                            }

                            item.Fields["Caption"].SetValue(ekt.OptionGallery.PhotoCaption, true);
                        }
                    }
                    else
                    {
                        _sb.AppendFormat("{0}<br/>", path);
                    }
                }
            }
        }

        public string FixRteLinks()
        {
            var urlList = new List<Tuple<string, string>>();
            var foundUrlList = new List<string>();

            //content blocks
            FixRteLinks("{6F8563A0-1C38-4619-B556-A92AAE3F9E55}", "{CB678537-26D1-4543-8875-9B7D05D694CC}", "Content", ref urlList, ref foundUrlList);

            //color headers
            FixRteLinks("{E3D1C283-22AF-4842-B7B1-5C2DED75DD89}", "{91DB8132-42D7-42B2-91AB-1E9C269506FD}", "Content", ref urlList, ref foundUrlList);

            //accordions
            FixRteLinks("{3A3CDA3D-E72E-4227-ADA6-52AC5077400F}", "{70C07EF9-32EC-4257-B79A-A13B38E6EB75}", "Description", ref urlList, ref foundUrlList);

            //stacked images
            FixRteLinks("{9D01721E-52EA-4217-A845-EC355ADD198E}", "{690E182C-97C9-40B3-8A4B-BE2F9CBD9614}", "Text", ref urlList, ref foundUrlList);

            //recipes
            FixRteLinks("{7A2EAA3F-5100-4C9D-A0DB-86E3C7848322}", "{AC49F5B2-D7CD-4A2D-9CAA-29ECE9DD04C2}", "Content", ref urlList, ref foundUrlList);
            FixRteLinks("{7A2EAA3F-5100-4C9D-A0DB-86E3C7848322}", "{AC49F5B2-D7CD-4A2D-9CAA-29ECE9DD04C2}", "Source", ref urlList, ref foundUrlList);
            FixRteLinks("{7A2EAA3F-5100-4C9D-A0DB-86E3C7848322}", "{AC49F5B2-D7CD-4A2D-9CAA-29ECE9DD04C2}", "Reviewed By", ref urlList, ref foundUrlList);

            //news releases
            FixRteLinks("{302A8D22-D656-4501-AF34-5C9FC137ABF7}", "{75FBD114-524E-4B57-B5CE-445C97D7C320}", "Content", ref urlList, ref foundUrlList);
            FixRteLinks("{6D758332-7097-4F8E-BAFF-2129F2673D99}", "{413D4796-0B7A-4CA4-B3CA-D637947F2FFE}", "Description", ref urlList, ref foundUrlList);

            _sb.Append("<h3>fixed " + foundUrlList.Count + " links</h3>");
            //foreach (var r in foundUrlList.OrderBy(e => e))
            //{
            //    _sb.AppendFormat("{0}<br/>", r);
            //}

            _sb.Append("<h3>bad</h3><table>");
            foreach (var r in urlList.OrderBy(e => e.Item1))
            {
                _sb.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", r.Item1, r.Item2);
                //_sb.AppendFormat("{0} === {1}<br/>", r.Item1, r.Item2);
            }

            _sb.Append("</table>");

            return _sb.ToString();
        }

        private void FixRteLinks(string rootItemId, string templateId, string fieldName, ref List<Tuple<string, string>> urlList, ref List<string> foundUrlList)
        {
            var rootItem = _db.GetItem(new ID(rootItemId));
            var children = rootItem.Axes.GetDescendants().Where(e => e.TemplateID.ToString() == templateId).ToList();
            var aUrls = new Regex("<a.+?href=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);

            using (new SecurityDisabler())
            {
                foreach (var c in children)
                {
                    var s = c.GetFieldValue(fieldName);

                    if (string.IsNullOrWhiteSpace(s))
                    {
                        continue;
                    }

                    var results = aUrls.Matches(s).Cast<Match>().Select(match => match.Groups[1].Value).ToArray();
                    foreach (var r in results)
                    {
                        var rLower = r.ToLower().Trim();
                        if (rLower.StartsWith("http") || rLower.StartsWith("~/link.aspx") || rLower.StartsWith("/uploadedfiles/") || rLower.StartsWith("mailto:") || rLower.StartsWith("#") || rLower.StartsWith("-/media/"))
                        {
                            continue;
                        }

                        var path = HomePath + rLower.Replace("-", " ");
                        if (path.EndsWith("/"))
                        {
                            path = path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal));
                        }

                        var item = _db.GetItem(path);
                        if (!item.IsNull())
                        {
                            s = s.Replace("href=\"" + r + "\"", "href=\"~/link.aspx?_id=" + item.ID.ToShortID() + "&amp;_z=z\"");
                            foundUrlList.Add(r);
                            continue;
                        }

                        urlList.Add(new Tuple<string, string>(r, c.Paths.FullPath.Replace("/sitecore/content/Allina Health/Data", string.Empty).Replace("/sitecore/content/Allina Health/Home", string.Empty)));
                    }

                    using (new EditContext(c))
                    {
                        if (c.Fields[fieldName] != null)
                        {
                            c.Fields[fieldName].SetValue(s, true);
                        }
                    }
                }
            }
        }

        private string ReplaceImageSrc(string s)
        {
            var reUrls = new Regex("<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);
            var results = reUrls.Matches(s).Cast<Match>().Select(match => match.Groups[1].Value).ToArray();
            foreach (var r in results)
            {
                GetImageUrlAndPath(r, out _, out var path, out _);

                var item = _db.GetItem("/sitecore/media library/Allina Health" + path);
                if (item != null)
                {
                    _imagesForAction.Add(item.Paths.FullPath);
                    s = s.Replace("src=\"" + r + "\"", "src=\"/-/media/" + item.ID.ToShortID() + ".ashx\"");
                }
                else
                {
                    var imgId = CreateImageInSitecore(r);
                    if (imgId != ID.Null)
                    {
                        var img = _db.GetItem(imgId);
                        _imagesForAction.Add(img.Paths.FullPath);
                        s = s.Replace("src=\"" + r + "\"", "src=\"/-/media/" + imgId.ToShortID() + ".ashx\"");
                    }
                    else
                    {
                        _sb.AppendFormat("{0}<br />", r);
                    }
                }
            }

            return s;
        }

        private void SaveImages(string html, HtmlDocument doc)
        {
            // image tags
            var imgTags = doc.DocumentNode.SelectNodes("//img");
            if (imgTags != null)
            {
                foreach (var img in imgTags)
                {
                    var key = GetTagValue(img, "src");
                    if (string.IsNullOrEmpty(key))
                    {
                        continue;
                    }

                    if (!_images.ContainsKey(key))
                    {
                        _images.Add(key, 1);
                    }
                    else
                    {
                        _images[key] += 1;
                    }
                }
            }

            // background url
            const string strRegex = @"url\s*[(][\s('""|&quot;)]*(?<Url>[\w- ./?%&=]*)[\s('""|&quot;)]*[)]";
            const RegexOptions options = RegexOptions.IgnoreCase;
            var reUrls = new Regex(strRegex, options);
            var results = reUrls.Matches(html).Cast<Match>().Select(match => match.Groups["Url"].Value).ToArray();
            foreach (var key in results)
            {
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                if (!_images.ContainsKey(key))
                {
                    _images.Add(key, 1);
                }
                else
                {
                    _images[key] += 1;
                }
            }

            // get picture source srcset
            var pictureSrcset = doc.DocumentNode.SelectNodes("//source");
            if (pictureSrcset == null)
            {
                return;
            }

            {
                foreach (var img in pictureSrcset)
                {
                    var keys = new List<string>();
                    var val = GetTagValue(img, "srcset");
                    var array = val.Split(',');
                    if (array.Length > 1)
                    {
                        keys.AddRange(from k in array select k.Split(' ') into array2 where array2.Length == 2 select array2[0]);
                    }
                    else if (array.Length > 0)
                    {
                        keys.Add(array[0]);
                    }

                    foreach (var key in keys.Where(key => !string.IsNullOrEmpty(key)))
                    {
                        if (!_images.ContainsKey(key))
                        {
                            _images.Add(key, 1);
                        }
                        else
                        {
                            _images[key] += 1;
                        }
                    }
                }
            }
        }

        private void SetLinkField(BaseItem i, string fieldName, EktronMenu m, string urlOrId = "")
        {
            var link = m != null ? m.Link : urlOrId;
            if (string.IsNullOrEmpty(link))
            {
                return;
            }

            var lf = (LinkField)i.Fields[fieldName];
            if (lf == null)
            {
                return;
            }

            lf.Clear();
            if (link.StartsWith("http"))
            {
                lf.Url = link;
                lf.LinkType = "external";
            }
            else
            {
                if (ID.TryParse(urlOrId, out var id))
                {
                    lf.TargetID = id;
                    lf.LinkType = "internal";
                }
                else
                {
                    var pageItem = FindMenuItem(link);
                    if (pageItem == null)
                    {
                        return;
                    }

                    lf.TargetID = pageItem.ID;
                    lf.LinkType = "internal";
                    //else
                    //{
                    //    _sb.AppendFormat("NAME: {0}, LINK: {1}<br/>", name, link);
                    //}
                }
            }
        }

        public string TestDocumentsCount()
        {
            var list = GetEktronDocuments();
            list.AddRange(GetEktronDocuments(false));
            return $"Count: {list.Count}";
        }

        public string TestDocuments()
        {
            var count = 0;
            var list = GetEktronDocuments();
            list.AddRange(GetEktronDocuments(false));
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var d in list)
            {
                var path = d.Url;
                if (!path.StartsWith("/"))
                {
                    path = "/" + path;
                }

                var url = "http://local.allinahealth.org" + path;

                try
                {
                    var webRequest = WebRequest.Create(url);
                    using (webRequest.GetResponse())
                    {
                        count++;
                    }
                }
                catch (WebException we)
                {
                    var errorResponse = we.Response as HttpWebResponse;
                    _sb.Append(url + ", status code: " + errorResponse?.StatusCode + "<br/>");
                }
                catch (Exception e)
                {
                    _sb.Append(url + ", message: " + e.Message + "<br/>");
                }
            }

            _sb.AppendFormat("Passed Count: {0}/{1}<br/>", count, list.Count);

            return _sb.ToString();
        }

        public string TestImage()
        {
            CreateImageInSitecore("/uploadedimages/content/careers/student-nurses.jpg");
            CreateImageInSitecore("/uploadedImages/Content/Business_units/United_Hospital/Hospital_services/Brain_aneurysm/IMRI223x216.jpg");
            CreateImageInSitecore("/uploadedImages/Content/Business_units/Phillips_Eye_Institute/Patient_and_visitor_information/team300x250.jpg");

            _sb.Append(":)");
            return _sb.ToString();
        }

        public string TestUrls()
        {
            var folder = _db.GetItem("/sitecore/content/Allina Health/Data/Content Blocks");
            if (folder == null) return _sb.ToString();
            var list = folder.Axes.GetDescendants().Where(e => e.TemplateID.ToString() == "{CB678537-26D1-4543-8875-9B7D05D694CC}").ToList();
            var urls = (from i in list select i.GetFieldValue("Content") into content let reUrls = new Regex("<a.+?href=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase) from r in reUrls.Matches(content).Cast<Match>().Select(match => match.Groups[1].Value).ToArray() select r).ToList();
            foreach (var r in urls.OrderByDescending(e => e))
            {
                _sb.AppendFormat("{0}<br/>", r);
            }

            return _sb.ToString();
        }

        private static void UpdateItem(Item i, string title, string description, string ogTitle, string ogDescription)
        {
            if (i == null)
            {
                return;
            }

            using (new SecurityDisabler())
            {
                using (new EditContext(i))
                {
                    i.Fields["Browser Title"].SetValue(title, true);
                    i.Fields["Meta Description"].SetValue(description, true);
                    i.Fields["OG Title"].SetValue(ogTitle, true);
                    i.Fields["Page Title"].SetValue(ogTitle, true);
                    i.Fields["OG Description"].SetValue(ogDescription, true);
                }
            }
        }

        public string UploadAtoZ()
        {
            return _sb.ToString();
        }

        public string UploadDocuments()
        {
            var list = GetEktronDocuments();
            list.AddRange(GetEktronDocuments(false));
            return _sb.ToString();
        }

        public string UploadMenus()
        {
            var menuFolder = _db.GetItem(new ID("{4177E490-9EB4-488B-8A35-15BD827B2FE2}")); // menu folder
            if (menuFolder.IsNull())
            {
                return "No menu folder found.";
            }

            var list = GetEktronMenus(out var dict);
            if (list.Count <= 0)
            {
                return _sb.ToString();
            }

            var parents = list.Where(e => e.ParentID == "0" || string.IsNullOrEmpty(e.ParentID)).ToList();
            CreateMenus(list, parents, menuFolder, dict); // initial create

            foreach (var p in parents)
            {
                if (string.IsNullOrEmpty(p.Name))
                {
                    continue;
                }

                var itemName = ItemUtil.ProposeValidItemName(p.Name.Replace("&amp;", "and")).ToLower(); // fix stand alone menus
                var pItem = menuFolder.GetChildren().FirstOrDefault(e => e.Name == itemName);
                if (pItem != null && ((itemName.Contains("standalone") && !itemName.Contains("hct and service")) || itemName.Contains("business unit locations") || itemName.Contains("business unit services")))
                {
                    CreateMenus(list, list.Where(e => e.ParentID == p.ID).ToList(), pItem, dict);
                }

                //if (itemName == "main navigation" && pItem != null)
                //{
                //    var learnList = list.Where(e => e.ParentID == p.ID);
                //    foreach (var l in learnList)
                //    {
                //        var learnName = ItemUtil.ProposeValidItemName(l.Name).ToLower();
                //        if (learnName == "learn")
                //        {
                //            CreateMenus(list, new List<EktronMenu>() { l }, pItem, dict);
                //        }
                //    }
                //}
            }

            return _sb.ToString();
        }

        public string UploadRedirects()
        {
            using (var pkg = new ExcelPackage())
            {
                using (var stream = System.IO.File.OpenRead(@"C:\ComponentDump2\301 redirects.xlsx"))
                {
                    pkg.Load(stream);
                }

                var ws = pkg.Workbook.Worksheets.First();

                using (new SecurityDisabler())
                {
                    for (var rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var sourceUrl = ws.Cells[rowNum, 2].First().Text;
                        var targetUrl = ws.Cells[rowNum, 3].First().Text;

                        CreateRedirect(sourceUrl, targetUrl);
                    }
                }
            }

            return _sb.ToString();
        }

        public UtilityController()
        {
            _webClient = new WebClient();
            _db = Factory.GetDatabase("master");
            _template = _db.GetTemplate(new ID("{B280AA13-8D91-40D1-850B-F0B7E60577B6}"));
            _images = new Dictionary<string, int>();
            _sb = new StringBuilder();
            _imagesForAction = new List<string>();
        }

    }
}