using System;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using AllinaHealth.Framework.Contexts;
using AllinaHealth.Framework.Pipelines.GetContentEditorWarnings;
using AllinaHealth.Models.Constants;
using AllinaHealth.Models.ViewModels.Reporting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Sitecore.Configuration;
using Sitecore.Data;

namespace AllinaHealth.Web.Controllers
{
    public class ReportingController : Controller
    {
        private Database _db;

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedMember.Local
        private Database DB => _db ?? (_db = Factory.GetDatabase("master"));

        public ActionResult ModifiedItems()
        {
            var model = new ModifiedItemsViewModel
            {
                UseModifiedDate = true
            };
            return View("~/Views/Reporting/ModifiedItems.cshtml", model);
        }

        [HttpPost]
        public ActionResult ModifiedItems(FormCollection collection)
        {
            var model = new ModifiedItemsViewModel
            {
                UseModifiedDate = true
            };
            GetList(model, collection);

            if (collection["isDownload"] == "true")
            {
                ExportToExcel(model, "Modified Items", "Modified Items");
            }

            return View("~/Views/Reporting/ModifiedItems.cshtml", model);
        }

        public ActionResult CreatedItems()
        {
            var model = new ModifiedItemsViewModel
            {
                UseModifiedDate = false
            };
            return View("~/Views/Reporting/ModifiedItems.cshtml", model);
        }

        [HttpPost]
        public ActionResult CreatedItems(FormCollection collection)
        {
            var model = new ModifiedItemsViewModel
            {
                UseModifiedDate = false
            };
            GetList(model, collection);

            if (collection["isDownload"] == "true")
            {
                ExportToExcel(model, "Created Items", "Created Items");
            }

            return View("~/Views/Reporting/ModifiedItems.cshtml", model);
        }

        private static void GetList(ModifiedItemsViewModel model, NameValueCollection collection)
        {
            if (!DateTime.TryParse(collection["startDate"], out var startDate) || !DateTime.TryParse(collection["endDate"], out var endDate))
            {
                return;
            }

            model.Sort = collection["sort"];
            model.StartDate = startDate;
            model.EndDate = endDate;
            model.List = model.UseModifiedDate ? SiteContext.Current.HomeItem.Axes.GetDescendants().Where(e => e.Statistics.Updated >= startDate && e.Statistics.Updated <= endDate).ToList() : SiteContext.Current.HomeItem.Axes.GetDescendants().Where(e => e.Statistics.Created >= startDate && e.Statistics.Created <= endDate).ToList();

            switch (model.Sort)
            {
                case "item":
                    model.List = model.List.OrderBy(e => e.Paths.FullPath).ToList();
                    break;
                case "term" when model.UseModifiedDate:
                    model.List = model.List.OrderByDescending(e => e.Statistics.Updated).ToList();
                    break;
                case "term":
                    model.List = model.List.OrderByDescending(e => e.Statistics.Created).ToList();
                    break;
            }
        }

        private void ExportToExcel(ModifiedItemsViewModel model, string sheetName, string fileName)
        {
            var dt = new DataTable();
            dt.Columns.Add("Item");
            if (model.UseModifiedDate)
            {
                dt.Columns.Add("Modified");
                dt.Columns.Add("Modified By");
            }
            else
            {
                dt.Columns.Add("Created");
                dt.Columns.Add("Created By");
            }

            foreach (var entity in model.List)
            {
                if (entity == null)
                {
                    continue;
                }

                var dr = dt.NewRow();
                dr["Item"] = entity.Paths.FullPath;

                if (model.UseModifiedDate)
                {
                    dr["Modified"] = entity.Statistics.Updated.ToString(DateFormat.Default);
                    var name = IsLocked.GetUserName(entity.Statistics.UpdatedBy);
                    dr["Modified By"] = !string.IsNullOrEmpty(name) ? name : entity.Statistics.UpdatedBy;
                }
                else
                {
                    dr["Created"] = entity.Statistics.Created.ToString(DateFormat.Default);
                    var name = IsLocked.GetUserName(entity.Statistics.CreatedBy);
                    dr["Created By"] = !string.IsNullOrEmpty(name) ? name : entity.Statistics.CreatedBy;
                }

                dt.Rows.Add(dr);
            }

            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add(sheetName);
                ws.Cells["A1"].LoadFromDataTable(dt, true);
                using (var rng = ws.Cells["A1:Z1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", $"attachment;  filename={fileName} {DateTime.Today.ToString("MM.dd.yyyy")}.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
        }
    }
}