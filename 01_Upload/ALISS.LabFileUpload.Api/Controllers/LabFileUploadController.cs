using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ALISS.LabFileUpload.DTO;
using ALISS.LabFileUpload.Library;
using ALISS.LabFileUpload.Library.DataAccess;
using AutoMapper;


namespace ALISS.LabFileUpload.Api.Controllers
{
    public class LabFileUploadController : Controller
    {
        private readonly ILabFileUploadService _service;

        public LabFileUploadController(LabFileUploadContext db, IMapper mapper)
        {
            _service = new LabFileUploadService(db, mapper);
        }

        [Route("api/LabFileUpload/GetLabFileUploadDataById/{id}")]
        [HttpGet]
        public LabFileUploadDataDTO GetLabFileUploadDataById(string id)
        {
            var objReturn = _service.GetLabFileUploadDataById(id);
            return objReturn;
        }


        [HttpPost]
        [Route("api/LabFileUpload/Post_SaveLabFileUploadData")]
        public LabFileUploadDataDTO Post_SaveLabFileUploadData([FromBody]LabFileUploadDataDTO model)
        {
            var objReturn = _service.SaveLabFileUploadData(model);

            return objReturn;
        }

        [HttpPost]
        [Route("api/LabFileUpload/Get_LabFileUploadListByModel")]
        public IEnumerable<LabFileUploadDataDTO> Get_LabFileUploadListByModel([FromBody]LabFileUploadSearchDTO searchModel)
        {
            var objReturn = _service.GetLabFileUploadListWithModel(searchModel);

            return objReturn;
        }

        [Route("api/LabFileUpload/GetLabFileSummaryHeaderBylfuId/{param}")]
        [HttpGet]
        public IEnumerable<LabFileSummaryHeaderListDTO> GetLabFileSummaryHeaderBylfuId(string param)
        {

            var objReturn = _service.GetLabFileSummaryHeaderBylfuId(param);
            return objReturn;
        }

        [Route("api/LabFileUpload/GetLabFileSummaryDetailBylfuId/{param}")]
        [HttpGet]
        public IEnumerable<LabFileSummaryDetailListDTO> GetLabFileSummaryDetailBylfuId(string param)
        {

            var objReturn = _service.GetLabFileSummaryDetailBylfuId(param);
            return objReturn;
        }
        
        [Route("api/LabFileUpload/GetLabFileSummaryDetailListBylfuIdAsync/{param}")]
        [HttpGet]
        public IEnumerable<LabFileSummaryDetailListDTO> GetLabFileSummaryDetailListBylfuIdAsync(string param)
        {

            var objReturn = _service.GetLabFileSummaryDetailListBylfuId(param);
            return objReturn;
        }

        [Route("api/LabFileUpload/GetLabFileErrorHeaderBylfuId/{param}")]
        [HttpGet]
        public IEnumerable<LabFileErrorHeaderListDTO> GetLabFileErrorHeaderBylfuId(string param)
        {

            var objReturn = _service.GetLabFileErrorHeaderListBylfuId(param);
            return objReturn;
        }

        [Route("api/LabFileUpload/GetLabFileErrorDetailBylfuId/{param}")]
        [HttpGet]
        public IEnumerable<LabFileErrorDetailListDTO> GetLabFileErrordetailBylfuId(string param)
        {

            var objReturn = _service.GetLabFileErrorDetailListBylfuId(param);
            return objReturn;
        }

        [Route("api/LabFileUpload/GetLabFileLabAlertSummaryBylfuId/{param}")]
        [HttpGet]
        public IEnumerable<LabFileLabAlertSummaryListDTO> GetLabFileLabAlertSummaryBylfuId(string param)
        {

            var objReturn = _service.GetLabFileLabAlertSummaryBylfuId(param);
            return objReturn;
        }


        [Route("api/LabFileUpload/GetLabFileExportMappingError")]
        [HttpPost]
        public List<LabFileExportMappingErrorDTO> GetLabFileExportMappingError([FromBody] string[] lfu_ids)
        {
            var objReturn = _service.GetLabFileExportMappingError(lfu_ids);
            return objReturn;
        }

        [Route("api/LabFileUpload/GenerateBoxNo/{format}")]
        [HttpPost]
        public List<LabFileUploadDataDTO> GenerateBoxNo(string format, [FromBody] List<LabFileUploadDataDTO> list)
        {
            var objReturn = _service.GenerateBoxNo(list, format);
            return objReturn;
        }

    }
}