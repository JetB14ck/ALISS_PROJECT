﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using ALISS.HISUpload.DTO;
using ALISS.HISUpload.Library;
using ALISS.HISUpload.Library.DataAccess;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALISS.HISUpload.Api.Controllers
{
    [ApiController]
    public class HISUploadController : Controller
    {
        private readonly IHISFileUploadService _service;
        public HISUploadController(HISFileUploadContext db, IMapper mapper)
        {
            _service = new HISFileUploadService(db, mapper);
        }

        [HttpGet]
        [Route("api/HISUpload/Get")]
        public IEnumerable<string> Get()
        {
            return new string[] { "HIS", "Upload" };
        }

        [HttpPost]
        [Route("api/HISUpload/GetHISFileUploadList")]
        public IEnumerable<HISUploadDataDTO> GetHISFileUploadList([FromBody] HISUploadDataSearchDTO searchModel)
        {
            var objReturn = _service.GetHISFileUploadListWithModel(searchModel);
            return objReturn;
        }

        [Route("api/HISUpload/GetHISFileUploadDataById/{id}")]
        [HttpGet]
        public HISUploadDataDTO GetHISFileUploadDataById(int id)
        {
            var objReturn = _service.GetHISFileUploadDataById(id);
            return objReturn;
        }

        [Route("api/HISUpload/GetHISFileUploadSummaryById/{id}")]
        [HttpGet]
        public IEnumerable<HISFileUploadSummaryDTO> GetHISFileUploadSummaryById(int id)
        {
            var objReturn = _service.GetHISFileUploadSummaryByUploadId(id);
            return objReturn;
        }

        [HttpPost]
        [Route("api/HISUpload/GetHISTemplateActive")]
        public IEnumerable<HISFileTemplateDTO> GetHISTemplateActive([FromBody] HISFileTemplateDTO searchModel)
        {
            var objReturn = _service.GetHISFileTemplate_Actice_WithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/HISUpload/Post_SaveSPFileUploadData")]
        public HISUploadDataDTO Post_SaveSPFileUploadData([FromBody] HISUploadDataDTO model)
        {
            var objReturn = _service.SaveLabFileUploadData(model);

            return objReturn;
        }
        [HttpPost]
        [Route("api/HISUpload/Post_SaveFileUploadSummary")]
        public HISFileUploadSummaryDTO Post_SaveFileUploadSummary(List<HISFileUploadSummaryDTO> model)
        {
            var objReturn = _service.SaveFileUploadSummary(model);

            return objReturn;
        }

        [HttpPost]
        [Route("api/HISUpload/GetLabDataWithHIS")]
        public IEnumerable<LabDataWithHISDTO> GetLabDataWithHIS([FromBody] LabDataWithHISSearchDTO searchModel)
        {
            var objReturn = _service.GetLabDataWithHIS(searchModel);
            return objReturn;
        }

        [HttpPost]
        [Route("api/HISUpload/GetLabDataWithHISDetail")]
        public IEnumerable<STGLabFileDataDetailDTO> GetLabDataWithHISDetail([FromBody] LabDataWithHISSearchDTO searchModel)
        {
            var objReturn = _service.GetLabDataWithHISDetail(searchModel);
            return objReturn;
        }

        [HttpPost]
        [Route("api/HISUpload/GetHISDetail")]
        public IEnumerable<HISDetailDTO> GetHISUploadDetail([FromBody] LabDataWithHISSearchDTO searchModel)
        {
            var objReturn = _service.GetSTGHISUploadDetail(searchModel);
            return objReturn;
        }

        [HttpGet]
        [Route("api/HISUpload/ExportLabDataWithHISFile/{searchModel}")]
        public IActionResult ExportLabDataWithHISFile(string searchModel)
        {
            //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            //var searchModelData = JsonSerializer.Deserialize<LabDataWithHISSearchDTO>(searchModel);
            var searchModelData = JsonSerializer.Deserialize<TRSPWithHISSearchDTO>(searchModel);
            try
            {
                //var fileResult = _service.ExportLabDataWithHIS(searchModelData);
                var fileResult = _service.ExportHIS_WithStore(searchModelData);

                return File(fileResult, "application/vnd.ms-excel", string.Format("{0}_{1}.xlsx", searchModelData.hos_code, DateTime.Today.ToString("yyyyMMdd")));

                //var contentLength = fileResult.Length;
                //var statuscode = HttpStatusCode.OK;

                //response = new HttpResponseMessage(statuscode);
                //response.Content = new ByteArrayContent(fileResult);
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //response.Content.Headers.ContentLength = contentLength;

                //ContentDispositionHeaderValue contentDisposition = null;

                //if (ContentDispositionHeaderValue.TryParse($"attachment; filename={string.Format("{0}_{1}.xlsx", searchModelData.hos_code, DateTime.Today.ToString("yyyyMMdd"))}", out contentDisposition))
                //{
                //    response.Content.Headers.ContentDisposition = contentDisposition;
                //}
            }
            catch (Exception ex)
            {

            }

            return new EmptyResult();
        }
    }
}