﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Diagnostics;
using uno;
using uno.util;
using unoidl.com.sun.star.beans;
using unoidl.com.sun.star.frame;
using unoidl.com.sun.star.lang;
using unoidl.com.sun.star.text;
using ALISS.EXPORT.Library.Model;
using System.Net.Http.Headers;

namespace ALISS.EXPORT.Api.Controllers
{
    public class ConvertPDFController : ApiController
    {
        ALISSEntities db = new ALISSEntities(true);

        [HttpGet]
        [Route("api/ConvertPDF/Get")]
        public IEnumerable<string> Get()
        {
            return new string[] { "EXPORT1", "EXPORT2" };
        }

        [HttpGet]
        [Route("api/ConvertPDF/TestPDF")]
        public IHttpActionResult TestPDF()
        {

            var statuscode = "";
            //string inputFile = @"C:\ALISS_ProcessFile\GLASS\test.txt";
            //string outputFile = @"C:\ALISS_ProcessFile\GLASS\test.pdf";
            //string inputFile = @"C:\ALISS_ProcessFile\GLASS\test.xlsm";
            //string outputFile = @"C:\ALISS_ProcessFile\GLASS\test.pdf";

            string strReportPath = "";
            var query = new List<sp_GET_TCParameter_Result>();
            query = db.sp_GET_TCParameter("TEST_PATH").ToList();

            if (query.FirstOrDefault(x => x.prm_code_minor == "PATH") != null)
            {
                strReportPath = query.FirstOrDefault(x => x.prm_code_minor == "PATH").prm_value;
            }

            string inputFile = Path.Combine(strReportPath, "P_PCR_202103200001_01_50_001071300_NPH_2020_01_12-NARST-Result.xlsx");
            string outputFile = Path.Combine(strReportPath, "P_PCR_202103200001_01_50_001071300_NPH_2020_01_12-NARST-Result.pdf");

            if (ConvertExtensionToFilterType(Path.GetExtension(inputFile)) == null)
                throw new InvalidProgramException("Unknown file type for OpenOffice. File = " + inputFile);

            StartOpenOffice();

            //Get a ComponentContext
            var xLocalContext =
                Bootstrap.bootstrap();
            //Get MultiServiceFactory
            var xRemoteFactory =
                (XMultiServiceFactory)
                xLocalContext.getServiceManager();
            //Get a CompontLoader
            var aLoader =
                (XComponentLoader)xRemoteFactory.createInstance("com.sun.star.frame.Desktop");
            //Load the sourcefile

            XComponent xComponent = null;
            System.Web.Http.Results.ResponseMessageResult response = null;
            try
            {
                xComponent = InitDocument(aLoader,
                                        PathConverter(inputFile), "_blank");
                //Wait for loading
                while (xComponent == null)
                {
                    Thread.Sleep(1000);
                }

                // save/export the document
                SaveDocument(xComponent, inputFile, PathConverter(outputFile));

                var dataBytes = File.ReadAllBytes(outputFile);
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(dataBytes.ToArray())
                };
                result.Content.Headers.ContentDisposition =
                   new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                   {
                       FileName = "FilePdf.pdf"
                   };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                response = ResponseMessage(result);
                //return response;

                //statuscode = "OK";
            }
            catch
            {
                //statuscode = "Error";
                var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response = ResponseMessage(result);
            }
            finally
            {
                if (xComponent != null) xComponent.dispose();
            }
           
            //return "Gen PDF status : " + statuscode;
            return response;
        }

        [HttpPost]
        [Route("api/ConvertPDF/PdfGenerator")]        
        public IHttpActionResult PdfGenerator([FromBody]string[] lstfullpath)
        {        
            var statuscode = "";
            var strFullPath = string.Join("\\", lstfullpath);
            var strReportPath = @"C:\ALISS_ProcessFile";

            var query = new List<sp_GET_TCParameter_Result>();         
            query = db.sp_GET_TCParameter("RPT_PROCESS_PATH").ToList();

            if (query.FirstOrDefault(x => x.prm_code_minor == "PATH") != null)
            {
                strReportPath = query.FirstOrDefault(x => x.prm_code_minor == "PATH").prm_value;
            }
         
            var inputFile = Path.Combine(strReportPath, strFullPath);
            var outputFile = Path.Combine(strReportPath, strFullPath.Replace(Path.GetExtension(inputFile), ".pdf"));
            //string outputFile = @"D:\TestDownload\pdf\test.pdf";

            if (ConvertExtensionToFilterType(Path.GetExtension(inputFile)) == null)
                throw new InvalidProgramException("Unknown file type for OpenOffice. File = " + inputFile);

            StartOpenOffice();

            //Get a ComponentContext
            var xLocalContext =
                Bootstrap.bootstrap();
            //Get MultiServiceFactory
            var xRemoteFactory =
                (XMultiServiceFactory)
                xLocalContext.getServiceManager();
            //Get a CompontLoader
            var aLoader =
                (XComponentLoader)xRemoteFactory.createInstance("com.sun.star.frame.Desktop");
            //Load the sourcefile

            XComponent xComponent = null;
            System.Web.Http.Results.ResponseMessageResult response = null;
            try
            {
                xComponent = InitDocument(aLoader,
                                        PathConverter(inputFile), "_blank");
                //Wait for loading
                while (xComponent == null)
                {
                    Thread.Sleep(1000);
                }

                // save/export the document
                SaveDocument(xComponent, inputFile, PathConverter(outputFile));

                var dataBytes = File.ReadAllBytes(outputFile);
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(dataBytes.ToArray())
                };
                result.Content.Headers.ContentDisposition =
                   new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                   {
                       FileName = "FilePdf.pdf"
                   };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                response = ResponseMessage(result);

                //statuscode = "OK";
            }
            catch
            {
                var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response = ResponseMessage(result);
            }
            finally
            {
                if (xComponent != null) xComponent.dispose();
            }
            //return statuscode;
            return response;
        }

        private static void StartOpenOffice()
        {
            var ps = Process.GetProcessesByName("soffice.exe");
            if (ps.Length != 0)
                throw new InvalidProgramException("OpenOffice not found.  Is OpenOffice installed?");
            if (ps.Length > 0)
                return;
            var p = new Process
            {
                StartInfo =
                        {
                            Arguments = "-headless -nofirststartwizard",
                            FileName = "soffice.exe",
                            CreateNoWindow = true,
                            Verb = "runas"
                        }
            };
            var result = p.Start();

            if (result == false)
                throw new InvalidProgramException("OpenOffice failed to start.");
        }

        private static XComponent InitDocument(XComponentLoader aLoader, string file, string target)
        {
            var openProps = new PropertyValue[1];
            openProps[0] = new PropertyValue { Name = "Hidden", Value = new Any(true) };

            var xComponent = aLoader.loadComponentFromURL(
                file, target, 0,
                openProps);

            return xComponent;
        }

        private static void SaveDocument(XComponent xComponent, string sourceFile, string destinationFile)
        {
            var propertyValues = new PropertyValue[2];
            // Setting the flag for overwriting
            propertyValues[1] = new PropertyValue { Name = "Overwrite", Value = new Any(true) };
            //// Setting the filter name
            propertyValues[0] = new PropertyValue
            {
                Name = "FilterName",
                Value = new Any(ConvertExtensionToFilterType(Path.GetExtension(sourceFile)))
            };
            ((XStorable)xComponent).storeToURL(destinationFile, propertyValues);
        }

        private static string PathConverter(string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new NullReferenceException("Null or empty path passed to OpenOffice");

            return String.Format("file:///{0}", file.Replace(@"\", "/"));
        }

        public static string ConvertExtensionToFilterType(string extension)
        {
            switch (extension)
            {
                case ".doc":
                case ".docx":
                case ".txt":
                case ".rtf":
                case ".html":
                case ".htm":
                case ".xml":
                case ".odt":
                case ".wps":
                case ".wpd":
                    return "writer_pdf_Export";
                case ".xls":
                case ".xlsb":
                case ".xlsx":
                case ".xlsm":
                case ".ods":
                    return "calc_pdf_Export";
                case ".ppt":
                case ".pptx":
                case ".odp":
                    return "impress_pdf_Export";

                default:
                    return null;
            }
        }

        //[HttpGet]
        //[Route("api/ConvertPDF/DownloadFile")]
        //public IHttpActionResult DownloadFile()
        //{
        //    var stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
        //    // processing the stream.
        //    var sm = ReadFully(stream);
        //    var result = new HttpResponseMessage(HttpStatusCode.OK)
        //    {
        //        Content = new ByteArrayContent(sm.ToArray())
        //    };
        //    result.Content.Headers.ContentDisposition =
        //        new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
        //        {
                      
        //                FileName = "AMR_MAP.pdf"
        //        };
        //    result.Content.Headers.ContentType =
        //         new MediaTypeHeaderValue("application/pdf");
          

        //    var response = ResponseMessage(result);
        //    var log_end = new LogWriter("End DownloadFile ...");
        //    return response;
        //}
    }

   
}
