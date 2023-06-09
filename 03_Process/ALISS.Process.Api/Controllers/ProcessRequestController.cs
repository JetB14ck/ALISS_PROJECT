﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALISS.Process.DTO;
using ALISS.Process.Library;
using ALISS.Process.Library.DataAccess;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ALISS.Process.Api.Controllers
{
    //[Route("api/[controller]")]
    public class ProcessRequestController : Controller
    {
        private readonly IProcessRequestService _service;

        public ProcessRequestController(ProcessContext db, IMapper mapper)
        {
            _service = new ProcessRequestService(db, mapper);
        }

        [HttpGet]
        [Route("api/ProcessRequest/Get_List")]
        public IEnumerable<ProcessRequestDTO> Get_List()
        {
            var objReturn = _service.GetList();

            return objReturn;
        }

        [HttpGet]
        [Route("api/ProcessRequest/Get_List/{param}")]
        public IEnumerable<ProcessRequestDTO> Get_List(string param)
        {
            var objReturn = _service.GetListWithParam(param);

            return objReturn;
        }

        [HttpPost]
        [Route("api/ProcessRequest/Get_ListByModel")]
        public IEnumerable<ProcessRequestDTO> Get_ListByModel([FromBody]ProcessRequestDTO searchModel)
        {
            var objReturn = _service.GetListWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/ProcessRequest/Get_DetailListByModel")]
        public IEnumerable<ProcessRequestCheckDetailDTO> Get_DetailListByModel([FromBody]ProcessRequestDTO searchModel)
        {
            var objReturn = _service.GetDetailListWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/ProcessRequest/Get_CheckDetailListByModel")]
        public IEnumerable<ProcessRequestCheckDetailDTO> Get_CheckDetailListByModel([FromBody]ProcessRequestDTO searchModel)
        {
            var objReturn = _service.GetCheckDetailListWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/ProcessRequest/Get_CheckDetailListByParam")]
        public IEnumerable<ProcessRequestCheckDetailDTO> Get_CheckDetailListByParam([FromBody] ProcessRequestDTO searchModel)
        {
            var objReturn = _service.GetCheckDetailListWithParam(searchModel);

            return objReturn;
        }

        [HttpGet]
        [Route("api/ProcessRequest/Get_Data/{pcr_code}")]
        public ProcessRequestDTO Get_Data(string pcr_code)
        {
            var objReturn = _service.GetData(pcr_code);

            return objReturn;
        }

        [HttpGet]
        [Route("api/ProcessRequest/Save_DataToPublic/{pcr_code}")]
        public ProcessRequestDTO Save_DataToPublic(string pcr_code)
        {
            var objReturn = _service.SaveDataToPublic(pcr_code);

            return objReturn;
        }

        [HttpPost]
        [Route("api/ProcessRequest/Get_DataWithModel")]
        public ProcessRequestDTO Get_DataWithModel([FromBody]ProcessRequestDTO searchModel)
        {
            var objReturn = _service.GetDataWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/ProcessRequest/Post_SaveData")]
        public ProcessRequestDTO Post_SaveData([FromBody]ProcessRequestDTO model)
        {
            var objReturn = _service.SaveData(model);

            return objReturn;
        }

        [HttpPost]
        [Route("api/ProcessRequest/Post_SaveDetailData")]
        public IEnumerable<ProcessRequestDetailDTO> Post_SaveDetailData([FromBody]List<ProcessRequestDetailDTO> model)
        {
            var objReturn = _service.SaveDetailData(model);

            return objReturn;
        }

    }
}
