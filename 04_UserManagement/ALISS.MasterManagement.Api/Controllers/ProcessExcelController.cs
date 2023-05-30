using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALISS.MasterManagement;
using ALISS.MasterManagement.DTO;
using ALISS.MasterManagement.Library;
using ALISS.MasterManagement.Library.DataAccess;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ALISS.Master.Api.Controllers
{
    //[Route("api/[controller]")]
    public class ProcessExcelController : Controller
    {
        private readonly ProcessExcelService _service;

        public ProcessExcelController(MasterManagementContext db, IMapper mapper)
        {
            _service = new ProcessExcelService(db, mapper);
        }

        [HttpPost]
        [Route("api/MasterTemplate/Get_ProcessExcelColumn_DataList/")]
        public IEnumerable<TCProcessExcelColumnDTO> Get_ProcessExcelColumn_DataList([FromBody] TCProcessExcelColumnDTO searchModel)
        {
            var objReturn = _service.Get_ProcessExcelColumn(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/MasterTemplate/Get_ProcessExcelRow_DataList/")]
        public IEnumerable<TCProcessExcelRowDTO> Get_ProcessExcelRow_DataList([FromBody] TCProcessExcelRowDTO searchModel)
        {
            var objReturn = _service.Get_ProcessExcelRow(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/MasterTemplate/Get_ProcessExcelTemplate_DataList/")]
        public IEnumerable<TCProcessExcelTemplateDTO> Get_ProcessExcelTemplate_DataList([FromBody] TCProcessExcelTemplateDTO searchModel)
        {
            var objReturn = _service.Get_ProcessExcelTemplate(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/MasterTemplate/Post_ProcessExcelColumn_SaveData")]
        public TCProcessExcelColumnDTO Post_SaveData([FromBody] TCProcessExcelColumnDTO model)
        {
            var objReturn = _service.SaveProcessExcelColumnData(model);

            return objReturn;
        }

        [HttpPost]
        [Route("api/MasterTemplate/Post_ProcessExcelRow_SaveData")]
        public TCProcessExcelRowDTO Post_SaveData([FromBody] TCProcessExcelRowDTO model)
        {
            var objReturn = _service.SaveProcessExcelRowData(model);

            return objReturn;
        }

        [HttpPost]
        [Route("api/MasterTemplate/Post_ProcessExcelTemplate_SaveData")]
        public TCProcessExcelTemplateDTO Post_SaveData([FromBody] TCProcessExcelTemplateDTO model)
        {
            var objReturn = _service.SaveProcessExcelTemplateData(model);

            return objReturn;
        }

        [HttpPost]
        [Route("api/MasterTemplate/Post_ProcessExcelColumn_SaveListData")]
        public TCProcessExcelColumnDTO Post_SaveData([FromBody] List<TCProcessExcelColumnDTO> models)
        {
            var objReturn = _service.SaveProcessExcelColumnListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/MasterTemplate/Post_ProcessExcelRow_SaveListData")]
        public TCProcessExcelRowDTO Post_SaveData([FromBody] List<TCProcessExcelRowDTO> models)
        {
            var objReturn = _service.SaveProcessExcelRowListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/MasterTemplate/Post_ProcessExcelTemplate_SaveListData")]
        public TCProcessExcelTemplateDTO Post_SaveData([FromBody] List<TCProcessExcelTemplateDTO> models)
        {
            var objReturn = _service.SaveProcessExcelTemplateListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/MasterTemplate/Post_ProcessExcelColumn_DeleteListData")]
        public TCProcessExcelColumnDTO Post_DeleteData([FromBody] List<TCProcessExcelColumnDTO> models)
        {
            var objReturn = _service.DeleteProcessExcelColumnListData(models);

            return objReturn;
        }
        [HttpPost]
        [Route("api/MasterTemplate/Post_ProcessExcelRow_DeleteListData")]
        public TCProcessExcelRowDTO Post_DeleteData([FromBody] List<TCProcessExcelRowDTO> models)
        {
            var objReturn = _service.DeleteProcessExcelRowListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/MasterTemplate/Post_ProcessExcelTemplate_DeleteListData")]
        public TCProcessExcelTemplateDTO Post_DeleteData([FromBody] List<TCProcessExcelTemplateDTO> models)
        {
            var objReturn = _service.DeleteProcessExcelTemplateListData(models);

            return objReturn;
        }
    }
}
