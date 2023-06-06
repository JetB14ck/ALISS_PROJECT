using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALISS.STARS.DTO;
using ALISS.STARS.Library;
using ALISS.STARS.Library.DataAccess;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


namespace ALISS.STARS.Api.Controllers
{

    public class ReceiveSampleController : Controller
    {
        private readonly IReceiveSampleDataService _service;

        public ReceiveSampleController(STARSContext db, IMapper mapper)
        {
            _service = new ReceiveSampleDataService(db, mapper);
        }

        #region Receive Sample

        [Route("api/ReceiveSample/GetList")]
        [HttpGet]
        public IEnumerable<ReceiveSampleListsDTO> GetList()
        {
            var objReturn = _service.GetList();
            return objReturn;
        }


        [Route("api/ReceiveSample/GetStarsResultList/{param}")]
        [HttpGet]
        public IEnumerable<ReceiveSampleListsDTO> GetStarsResultList(string param)
        {

            var objReturn = _service.GetStarsResultList(param);
            return objReturn;
        }

        [HttpPost]
        [Route("api/ReceiveSample/Post_SaveReceiveSampleData/{format}")]
        public List<ReceiveSampleListsDTO> Post_SaveReceiveSampleData(string format, [FromBody] List<ReceiveSampleListsDTO> models)
        {
            var objReturn = _service.SaveReceiveSampleData(models, format);

            return objReturn;
        }

        //// GET: api/<controller>
        //[Route("api/STARSMapping/GetMappingDataById/{id}")]
        //[HttpGet]
        //public STARSMappingDataDTO GetMappingDataById(string id)
        //{
        //    var objReturn = _service.GetMappingDataById(id);
        //    return objReturn;
        //}

        //[HttpPost]
        //[Route("api/STARSMapping/Post_SaveMappingData")]
        //public STARSMappingDataDTO Post_SaveMappingData([FromBody] STARSMappingDataDTO model)
        //{
        //    var objReturn = _service.SaveMappingData(model);

        //    return objReturn;
        //}

        //[HttpPost]
        //[Route("api/STARSMapping/Post_CopyMappingData")]
        //public STARSMappingDataDTO Post_CopyMappingData([FromBody] STARSMappingDataDTO model)
        //{
        //    var objReturn = _service.CopyMappingData(model);

        //    return objReturn;
        //}

        //[HttpPost]
        //[Route("api/STARSMapping/Get_MappingDataByModel")]
        //public STARSMappingDataDTO Get_MappingDataByModel([FromBody] STARSMappingDataDTO model)
        //{
        //    var objReturn = _service.GetMappingDataWithModel(model);

        //    return objReturn;
        //}

        //[HttpPost]
        //[Route("api/STARSMapping/Get_chkDuplicateMappingApproved")]
        //public STARSMappingDataDTO Get_chkDuplicateMappingApproved([FromBody] STARSMappingDataDTO model)
        //{
        //    var objReturn = _service.chkDuplicateMappingApproved(model);

        //    return objReturn;
        //}


        //[HttpPost]
        //[Route("api/STARSMapping/Get_MappingDataActiveWithModel")]
        //public STARSMappingDataDTO Get_MappingDataActiveWithModel([FromBody] STARSMappingDataDTO model)
        //{
        //    var objReturn = _service.GetMappingDataActiveWithModel(model);

        //    return objReturn;
        //}
        #endregion
    }
}
