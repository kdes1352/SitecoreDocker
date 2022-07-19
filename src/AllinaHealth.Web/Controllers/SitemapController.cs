using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Sitecore.Configuration;

namespace AllinaHealth.Web.Controllers
{
    public class SitemapController : Controller
    {
        // GET: Sitemap
        public ActionResult Index()
        {
            try
            {
                //Pull from Site Config
                //Url ~ should copy sitemap
                //2nd Download true/false
                //3rd Name prefix
                //string sUrls = "http://www.allinahealth.org/allinahealth_sitemap.xml~false~|http://wellness.allinahealth.org/home/sitemap.xml~true~wellclicks";
                const string sDomain = "http://www.allinahealth.org";

                var sUrls = Settings.GetSetting("SiteMap.HostName");

                var ns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
                XElement sitemapindex;
                var siteMapDocument = new XDocument(sitemapindex = new XElement(ns + "sitemapindex"));
                var webClient = new WebClient();

                var urlList = sUrls.Split('|').ToArray();
                foreach (var url in urlList)
                {
                    var parts = url.Split('~');
                    if (parts.Length != 3)
                    {
                        continue;
                    }

                    var urlParts = parts[0].Split('/');
                    var fileName = (urlParts.Length > 0 ? urlParts[urlParts.Length - 1] : "sitemap.xml");

                    var download = Convert.ToBoolean(parts[1]);
                    var filePrefix = parts[2];

                    if (download)
                    {
                        fileName = filePrefix + "_" + fileName;

                        var xml = webClient.DownloadString(parts[0]);
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml);
                        xmlDoc.Save(Server.MapPath("~/") + fileName);
                    }

                    sitemapindex.Add(new XElement
                    (ns + "sitemap",
                        new XElement(ns + "loc", sDomain + "/" + fileName),
                        new XElement(ns + "lastmod", DateTime.Now)
                    ));
                }

                return Content(siteMapDocument.ToString(), "text/xml");
            }
            catch (Exception ex)
            {
                return Content("Error:" + ex.Message);
            }
        }
    }
}