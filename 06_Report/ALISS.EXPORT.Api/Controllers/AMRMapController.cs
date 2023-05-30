using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using ALISS.EXPORT.Library.Model;
using ALISS.EXPORT.Library.DTO;
using System.Globalization;

namespace ALISS.EXPORT.Api.Controllers
{
    public class AMRMapController : ApiController
    {
        ALISSEntities db = new ALISSEntities(true);

        [HttpGet]
        [Route("api/AMRMap/Get")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("api/AMRMap/GetFile")]
        public IHttpActionResult GetFile()
        {
            //DateTime modelMonthFrom = Convert.ToDateTime(model.month_start_str);  //Convert.ToDateTime("01/01/2020");// Convert.ToDateTime(model.month_start_str); 
            //DateTime modelMonthTo = Convert.ToDateTime(model.month_end_str);  //Convert.ToDateTime("01/06/2020");//Convert.ToDateTime(model.month_end_str); 

            var log_start = new LogWriter("Start AMRMap/GetFile ...");

            DateTime modelMonthFrom = DateTime.ParseExact("2019/01/01", "yyyy/MM/dd", new CultureInfo("en-US"));
            DateTime modelMonthTo = DateTime.ParseExact("2020/01/01", "yyyy/MM/dd", new CultureInfo("en-US"));

            ReportDocument rpt = new ReportDocument();
            var query = new List<sp_GET_RPAntibiotrendAMRStrategy_Result>();
            //var strReportName = "rptAntibiotrendMap.rpt";
            var strReportName = "rptAntibiotrendMapAreaH.rpt";

            query = db.sp_GET_RPAntibiotrendAMRStrategy(modelMonthFrom, modelMonthTo).ToList();

            if (query.Count() > 0)
            {
                var strHeaderDateFromTo = string.Format("{0} - {1} {2}"
                                        , modelMonthFrom.ToString("MMMM", new CultureInfo("en-US"))
                                        , modelMonthTo.ToString("MMMM", new CultureInfo("en-US"))
                                        , modelMonthTo.ToString("yyyy")
                                        );

                var Targetpath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Report"), strReportName);
                rpt.Load(Targetpath);
                rpt.SetDataSource(query);
                rpt.SetParameterValue("paramStrHeader", strHeaderDateFromTo);

                // --------------- Export to PDF -------------------------------

                var stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
                // processing the stream.
                var sm = ReadFully(stream);
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(sm.ToArray())
                };
                result.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        //FileName = "AMR_MAP.xls"
                        FileName = "AMR_MAP.pdf"
                    };
                result.Content.Headers.ContentType =
                     new MediaTypeHeaderValue("application/pdf");

                rpt.Dispose();

                var response = ResponseMessage(result);
                var log_end = new LogWriter("End AMRMap/GetFile ...");
                return response;
            }
            else
            {
                var result = new HttpResponseMessage(HttpStatusCode.NotFound);
                var response = ResponseMessage(result);
                return response;
            }
          
        }

        //[HttpGet]
        //[Route("api/AMRGraph/ExportMap")]
        //public IHttpActionResult ExportMap()
        [HttpPost]
        [Route("api/AMRMap/ExportMap")]
        public IHttpActionResult ExportMap(AMRSearchMapDTO model)
        {
            //DateTime modelMonthFrom = Convert.ToDateTime(model.month_start_str);  //Convert.ToDateTime("01/01/2020");// Convert.ToDateTime(model.month_start_str); 
            //DateTime modelMonthTo = Convert.ToDateTime(model.month_end_str);  //Convert.ToDateTime("01/06/2020");//Convert.ToDateTime(model.month_end_str); 
           try
           {
                var log_start = new LogWriter("Start ExportMap ...");

                DateTime modelMonthFrom = DateTime.ParseExact(model.month_start_str, "yyyy/MM/dd", new CultureInfo("en-US"));
                DateTime modelMonthTo = DateTime.ParseExact(model.month_end_str, "yyyy/MM/dd", new CultureInfo("en-US"));

                ReportDocument rpt = new ReportDocument();
                var query = new List<sp_GET_RPAntibiotrendAMRStrategy_Result>();
                //var strReportName = "rptAntibiotrendMap.rpt";
                var strReportName = "rptAntibiotrendMapAreaH.rpt";


                query = db.sp_GET_RPAntibiotrendAMRStrategy(modelMonthFrom, modelMonthTo).ToList();
                if (query.Count() > 0)
                {
                    var strHeaderDateFromTo = string.Format("{0} - {1} {2}"
                                            , modelMonthFrom.ToString("MMMM", new CultureInfo("en-US"))
                                            , modelMonthTo.ToString("MMMM", new CultureInfo("en-US"))
                                            , modelMonthTo.ToString("yyyy")
                                            );

                    var Targetpath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Report"), strReportName);
                  
                    rpt.Load(Targetpath);
                    rpt.SetDataSource(query);
                    rpt.SetParameterValue("paramStrHeader", strHeaderDateFromTo);
                   

                    // --------------- Export to PDF -------------------------------
                    try
                    {
                        var stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
                        var sm = ReadFully(stream);
                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(sm.ToArray())
                        };
                        result.Content.Headers.ContentDisposition =
                       new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                       {
                            //FileName = "AMR_MAP.xls"
                            FileName = "AMR_MAP.pdf"
                       };
                        result.Content.Headers.ContentType =
                             new MediaTypeHeaderValue("application/pdf");
                
                        var response = ResponseMessage(result);
                        var log_resp = new LogWriter("Response : " + response.Response.StatusCode.ToString());
                        var log_end = new LogWriter("End AMRMap/GetFile ...");
                        return response;
                    }
                    catch(Exception ex)
                    {
                        var x = ex.Message;
                    }
                    finally
                    {
                        rpt.Dispose();
                    }

                }
                else
                {
                    var result = new HttpResponseMessage(HttpStatusCode.NotFound);
                    var log_data = new LogWriter("No data to export ...");
                    var log_end = new LogWriter("End AMRMap/GetFile ...");
                    var response = ResponseMessage(result);
                    return response;
                }

            }
            catch(Exception ex)
            {
                var logw = new LogWriter(ex.Message);
            }

            var result_e = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var log_x = new LogWriter("out of try-catch ...");
            var response_e = ResponseMessage(result_e);
            return response_e;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
     
    }
}
