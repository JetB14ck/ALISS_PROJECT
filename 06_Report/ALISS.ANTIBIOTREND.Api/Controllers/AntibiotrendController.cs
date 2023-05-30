using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALISS.ANTIBIOTREND.DTO;
using ALISS.ANTIBIOTREND.Library;
using ALISS.ANTIBIOTREND.Library.DataAccess;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALISS.ANTIBIOTREND.Api.Controllers
{
    
    [ApiController]
    public class AntibiotrendController : ControllerBase
    {
        private readonly IAntibiotrendService _service;
        private readonly IWebHostEnvironment _host;

        public AntibiotrendController(AntibiotrendContext db, IMapper mapper, IWebHostEnvironment host)
        {
            _service = new AntibiotrendService(db, mapper);
            _host = host;
        }

        [HttpGet]
        [Route("api/Antibiotrend/Get")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("api/Antibiotrend/GetTest")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetTest()
        {
            var objReturn = _service.GetAMRWithModel(new SP_AntimicrobialResistanceSearchDTO());
            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMRModel")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetAMR([FromBody]SP_AntimicrobialResistanceSearchDTO searchModel)
        {
            var objReturn = _service.GetAMRWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetNationHealthStrategyModel")]
        public IEnumerable<NationHealthStrategyDTO> GetAMRNationHealthStrategy([FromBody]AMRStrategySearchDTO searchModel)
        {
            var objReturn = _service.GetAMRNationStrategyWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMRStrategyModel")]
        public IEnumerable<AntibiotrendAMRStrategyDTO> GetAMRStrategy([FromBody]AMRStrategySearchDTO searchModel)
        {
            var objReturn = _service.GetAntibiotrendAMRStrategyWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMROverallModel")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetAMROverall([FromBody]SP_AntimicrobialResistanceSearchDTO searchModel)
        {
            var objReturn = _service.GetAMRByOverallWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMROverallByAreaHModel")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetAMROverallByAreaH([FromBody]SP_AntimicrobialResistanceAreaHSearchDTO searchModel)
        {
            var objReturn = _service.GetAMRByOverallByAreaHWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMROverallByHospModel")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetAMROverallByHosp([FromBody]SP_AntimicrobialResistanceHospSearchDTO searchModel)
        {
            var objReturn = _service.GetAMRByOverallByHospWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMROverallByProvModel")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetAMROverallByProvince([FromBody]SP_AntimicrobialResistanceProvinceSearchDTO searchModel)
        {
            var objReturn = _service.GetAMRByOverallByProvWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMRSpecimenModel")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetAMRSpecimen([FromBody]SP_AntimicrobialResistanceSearchDTO searchModel)
        {
            var objReturn = _service.GetAMRBySpecimenWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMRSpecimenByAreaHModel")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetAMRSpecimenByAreaH([FromBody]SP_AntimicrobialResistanceAreaHSearchDTO searchModel)
        {
            var objReturn = _service.GetAMRBySpecimenByAreaHWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMRSpecimenByHospModel")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetAMRSpecimenByHosp([FromBody]SP_AntimicrobialResistanceHospSearchDTO searchModel)
        {
            var objReturn = _service.GetAMRBySpecimenByHospWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMRSpecimenByProvModel")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetAMRSpecimenByProvince([FromBody]SP_AntimicrobialResistanceProvinceSearchDTO searchModel)
        {
            var objReturn = _service.GetAMRBySpecimenByProvWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMRWardTypeModel")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetAMRWardType([FromBody]SP_AntimicrobialResistanceSearchDTO searchModel)
        {
            var objReturn = _service.GetAMRByWardWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMRWardTypeByAreaHModel")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetAMRWardTypeByAreaH([FromBody]SP_AntimicrobialResistanceAreaHSearchDTO searchModel)
        {
            var objReturn = _service.GetAMRByWardByAreaHWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMRWardTypeByHospModel")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetAMRWardTypeByHosp([FromBody]SP_AntimicrobialResistanceHospSearchDTO searchModel)
        {
            var objReturn = _service.GetAMRByWardByHospWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAMRWardTypeByProvModel")]
        public IEnumerable<SP_AntimicrobialResistanceDTO> GetAMRWardTypeByProv([FromBody]SP_AntimicrobialResistanceProvinceSearchDTO searchModel)
        {
            var objReturn = _service.GetAMRByWardByProvWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/Antibiotrend/GetAntibioticNameModel")]
        public IEnumerable<AntibioticNameDTO> GetAntibioticName([FromBody]AntibioticNameDTO searchModel)
        {
            var objReturn = _service.GetAntibioticNames();

            return objReturn;
        }
    }
}