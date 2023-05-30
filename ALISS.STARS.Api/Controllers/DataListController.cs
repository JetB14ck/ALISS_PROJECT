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

    public class DataListController : Controller
    {
        private readonly ISTARSMappingDataService _service;

        public DataListController(STARSMappingContext db, IMapper mapper)
        {
            _service = new STARSMappingDataService(db, mapper);
        }

        #region Mapping

        [Route("api/STARSMapping/GetList")]
        [HttpGet]
        public IEnumerable<STARSMappingListsDTO> GetList()
        {
            var objReturn = _service.GetList();
            return objReturn;
        }


        // GET: api/<controller>
        [Route("api/STARSMapping/GetMappingList/{param}")]
        [HttpGet]
        public IEnumerable<STARSMappingListsDTO> GetMappingList(string param)
        {

            var objReturn = _service.GetMappingList(param);
            return objReturn;
        }

        // GET: api/<controller>
        [Route("api/STARSMapping/GetMappingDataById/{id}")]
        [HttpGet]
        public STARSMappingDataDTO GetMappingDataById(string id)
        {
            var objReturn = _service.GetMappingDataById(id);
            return objReturn;
        }

        [HttpPost]
        [Route("api/STARSMapping/Post_SaveMappingData")]
        public STARSMappingDataDTO Post_SaveMappingData([FromBody] STARSMappingDataDTO model)
        {
            var objReturn = _service.SaveMappingData(model);

            return objReturn;
        }

        [HttpPost]
        [Route("api/STARSMapping/Post_CopyMappingData")]
        public STARSMappingDataDTO Post_CopyMappingData([FromBody] STARSMappingDataDTO model)
        {
            var objReturn = _service.CopyMappingData(model);

            return objReturn;
        }

        [HttpPost]
        [Route("api/STARSMapping/Get_MappingDataByModel")]
        public STARSMappingDataDTO Get_MappingDataByModel([FromBody] STARSMappingDataDTO model)
        {
            var objReturn = _service.GetMappingDataWithModel(model);

            return objReturn;
        }

        [HttpPost]
        [Route("api/STARSMapping/Get_chkDuplicateMappingApproved")]
        public STARSMappingDataDTO Get_chkDuplicateMappingApproved([FromBody] STARSMappingDataDTO model)
        {
            var objReturn = _service.chkDuplicateMappingApproved(model);

            return objReturn;
        }


        [HttpPost]
        [Route("api/STARSMapping/Get_MappingDataActiveWithModel")]
        public STARSMappingDataDTO Get_MappingDataActiveWithModel([FromBody] STARSMappingDataDTO model)
        {
            var objReturn = _service.GetMappingDataActiveWithModel(model);

            return objReturn;
        }
        #endregion


        #region WHONetMapping

        [HttpPost]
        [Route("api/STARSMapping/Post_SaveWHONetMappingData")]
        public STARSWHONetMappingDataDTO Post_SaveWHONetMappingData([FromBody] STARSWHONetMappingDataDTO model)
        {
            var objReturn = _service.SaveWHONetMappingData(model);

            return objReturn;
        }


        [HttpPost]
        [Route("api/STARSMapping/Get_WHONetMappingListByModel")]
        public IEnumerable<STARSWHONetMappingListsDTO> Get_WHONetMappingListByModel([FromBody] STARSWHONetMappingSearch searchModel)
        {
            var objReturn = _service.GetWHONetMappingListWithModel(searchModel);

            return objReturn;
        }


        [HttpGet]
        [Route("api/STARSMapping/Get_WHONetMappingData/{wnm_Id}")]
        public STARSWHONetMappingDataDTO Get_WHONetMappingData(string wnm_Id)
        {
            var objReturn = _service.GetWHONetMappingData(wnm_Id);

            return objReturn;
        }

        [HttpPost]
        [Route("api/STARSMapping/Get_WHONetMappingDataByModel")]
        public STARSWHONetMappingDataDTO Get_WHONetMappingDataByModel([FromBody] STARSWHONetMappingDataDTO model)
        {
            var objReturn = _service.GetWHONetMappingDataWithModel(model);

            return objReturn;
        }

        #endregion


        #region SpecimenMapping

        [HttpPost]
        [Route("api/STARSMapping/Post_SaveSpecimenMappingData")]
        public STARSSpecimenMappingDataDTO Post_SaveSpecimenMappingData([FromBody] STARSSpecimenMappingDataDTO model)
        {
            var objReturn = _service.SaveSpecimenMappingData(model);

            return objReturn;
        }


        [HttpPost]
        [Route("api/STARSMapping/Get_SpecimenMappingListByModel")]
        public IEnumerable<STARSSpecimenMappingListsDTO> Get_SpecimenHONetMappingListByModel([FromBody] STARSSpecimenMappingSearch searchModel)
        {
            var objReturn = _service.GetSpecimenMappingListWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/STARSMapping/Get_SpecimenMappingDataByModel")]
        public STARSSpecimenMappingDataDTO Get_SpecimenMappingDataByModel([FromBody] STARSSpecimenMappingDataDTO model)
        {
            var objReturn = _service.GetSpecimenMappingDataWithModel(model);

            return objReturn;
        }

        [HttpGet]
        [Route("api/STARSMapping/Get_SpecimenMappingData/{spm_Id}")]
        public STARSSpecimenMappingDataDTO Get_SpecimenMappingData(string spm_Id)
        {
            var objReturn = _service.GetSpecimenMappingData(spm_Id);

            return objReturn;
        }

        #endregion

        #region OrganismMapping
        [HttpPost]
        [Route("api/STARSMapping/Post_SaveOrganismMappingData")]
        public STARSOrganismMappingDataDTO Post_SaveOrganismMappingData([FromBody] STARSOrganismMappingDataDTO model)
        {
            var objReturn = _service.SaveOrganismMappingData(model);

            return objReturn;
        }


        [HttpPost]
        [Route("api/STARSMapping/Get_OrganismMappingListByModel")]
        public IEnumerable<STARSOrganismMappingListsDTO> Get_OrganismMappingListByModel([FromBody] STARSOrganismMappingSearch searchModel)
        {
            var objReturn = _service.GetOrganismMappingListWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/STARSMapping/Get_OrganismMappingDataByModel")]
        public STARSOrganismMappingDataDTO Get_OrganismMappingDataByModel([FromBody] STARSOrganismMappingDataDTO model)
        {
            var objReturn = _service.GetOrganismMappingDataWithModel(model);

            return objReturn;
        }


        [HttpGet]
        [Route("api/STARSMapping/Get_OrganismMappingData/{ogm_Id}")]
        public STARSOrganismMappingDataDTO Get_OrganismMappingData(string ogm_Id)
        {
            var objReturn = _service.GetOrganismMappingData(ogm_Id);

            return objReturn;
        }
        #endregion
    }
}
