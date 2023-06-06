
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
using ALISS.STARS.Report.Api.Helpers;

namespace ALISS.EXPORT.Api.Controllers
{
    public class BoxNoBarcodeController : ApiController
    {
        [HttpPost]
        [Route("api/STARSReport/ExportBoxNoBarcode")]
        public IHttpActionResult ExportBoxNoBarcode([FromBody] BoxNoBarcodeDTO model)
        {
            ReportDocument rpt = new ReportDocument();
            string strReportName = "rptBoxNoBarcode.rpt";
            //var Targetpath = Path.Combine(_hostEnvironment.ContentRootPath, "Report", strReportName);
            var Targetpath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Report"), strReportName);
            rpt.Load(Targetpath);
            rpt.SetParameterValue("hos_name", model.hos_name ?? "");
            rpt.SetParameterValue("arh_name", model.arh_name ?? "");
            rpt.SetParameterValue("lfu_boxno", model.lfu_boxno ?? "");
            rpt.SetParameterValue("code128", string.Format("*{0}*", model.lfu_boxno));
            rpt.SetParameterValue("send_date", model.send_date ?? "");

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
                    FileName = string.Format("{0}.pdf", model.lfu_boxno)
                };
            result.Content.Headers.ContentType =
                 new MediaTypeHeaderValue("application/pdf");

            rpt.Dispose();

            var response = ResponseMessage(result);
            return response;
        }
    }
}
