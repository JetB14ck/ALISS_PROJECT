
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Http.Results;
using RouteAttribute = System.Web.Http.RouteAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using ALISS.STARS.Report.DTO;
using ALISS.STARS.DTO;
using ALISS.STARS.Report.Api.Helpers;
using ALISS.START.Report.Api.Models;
using System.Reflection;

namespace ALISS.EXPORT.Api.Controllers
{
    public class ReceiveSampleController : ApiController
    {
        [HttpPost]
        [Route("api/STARSReport/printLogbook")]
        public IHttpActionResult PrintLogbook([FromBody] List<ReceiveLogbookModel> models)
        {
            ReportDocument rpt = new ReportDocument();
            string strReportName = "rptReceiveLogbook.rpt";
            var Targetpath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Report"), strReportName);
            rpt.Load(Targetpath);
            rpt.SetDataSource(models);

            // --------------- Export to PDF -------------------------------

            var stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
            // processing the stream.
            var sm = ReportHelpers.ReadFully(stream);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(sm.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = string.Format("ReceiveLogbook_{0}.pdf", DateTime.Now.ToString("yyyyMMdd"))
                };
            result.Content.Headers.ContentType =
                 new MediaTypeHeaderValue("application/pdf");

            rpt.Dispose();

            var response = ResponseMessage(result);
            return response;
        }
        [HttpPost]
        [Route("api/STARSReport/printBarcode")]
        public IHttpActionResult PrintBarcode([FromBody] List<ReceiveLogbookModel> models)
        {
            int col = 1;
            int count_of_col = 9;
            List<ReceiveBarcodeModel> lists = new List<ReceiveBarcodeModel>();
            ReceiveBarcodeModel obj = new ReceiveBarcodeModel();
            foreach (var model in models)
            {
                PropertyInfo property = obj.GetType().GetProperty(string.Format("srr_starsno_{0}",col));
                if (property != null && property.CanWrite)
                {
                    property.SetValue(obj, model.srr_starsno);
                }
                if (col == count_of_col)
                {
                    lists.Add(obj);
                    obj = new ReceiveBarcodeModel();
                    col = 0;
                }
                col++;
            }
            if (obj != new ReceiveBarcodeModel())
            {
                lists.Add(obj);
            }

            ReportDocument rpt = new ReportDocument();
            string strReportName = "rptReceiveBarcode.rpt";
            var Targetpath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Report"), strReportName);
            rpt.Load(Targetpath);
            rpt.SetDataSource(lists);

            // --------------- Export to PDF -------------------------------

            var stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
            // processing the stream.
            var sm = ReportHelpers.ReadFully(stream);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(sm.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = string.Format("ReceiveLogbook_{0}.pdf", DateTime.Now.ToString("yyyyMMdd"))
                };
            result.Content.Headers.ContentType =
                 new MediaTypeHeaderValue("application/pdf");

            rpt.Dispose();

            var response = ResponseMessage(result);
            return response;
        }
    }
}
