using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALISS.Master.DTO;
using ALISS.Master.Library.DataAccess;
using ALISS.Master.NoticeMessage.Library;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ALISS.Master.Api.Controllers
{
    //[Route("api/[controller]")]
    public class WHONET_Antibiotics_Active_ListController : Controller
    {
        private readonly IWHONETService _service;

        public WHONET_Antibiotics_Active_ListController(MasterContext db, IMapper mapper)
        {
            _service = new WHONETService(db, mapper);
        }

        // GET: api/<controller>

        [HttpGet]
        [Route("api/WHONET/WHONET_Antibiotics_Active_List/")]
        public IEnumerable<TCWHONET_AntibioticsDTO> Get()
        {
            var objList = _service.Get_TCWHONET_Antibiotics_Acitve_List();
            return objList;
        }

        [HttpPost]
        [Route("api/WHONET/WHONET_Antibiotics_List/")]
        public IEnumerable<TCWHONET_AntibioticsDTO> Get_TCWHONET_Antibiotics_List([FromBody] TCWHONET_AntibioticsDTO searchModel)
        {
            var objList = _service.Get_TCWHONET_Antibiotic_List(searchModel);
            return objList;
        }

        [HttpPost]
        [Route("api/WHONET/WHONET_AntibioticSpecies_List/")]
        public IEnumerable<TCWHONET_AntibioticSpeciesDTO> Get_TCWHONET_AntibioticSpecies_List([FromBody] TCWHONET_AntibioticSpeciesDTO searchModel)
        {
            var objList = _service.Get_TCWHONET_AntibioticSpecies_List(searchModel);
            return objList;
        }

        [HttpPost]
        [Route("api/WHONET/WHONET_Departments_List/")]
        public IEnumerable<TCWHONET_DepartmentsDTO> Get_TCWHONET_Departments_List([FromBody] TCWHONET_DepartmentsDTO searchModel)
        {
            var objList = _service.Get_TCWHONET_Departments_List(searchModel);
            return objList;
        }

        [HttpPost]
        [Route("api/WHONET/WHONET_Specimen_List/")]
        public IEnumerable<TCWHONET_SpecimenDTO> Get_TCWHONET_Specimen_List([FromBody] TCWHONET_SpecimenDTO searchModel)
        {
            var objList = _service.Get_TCWHONET_Specimen_List(searchModel);
            return objList;
        }

        [HttpPost]
        [Route("api/WHONET/WHONET_Column_List/")]
        public IEnumerable<TCWHONETColumnDTO> Get_TCWHONET_Column_List([FromBody] TCWHONETColumnDTO searchModel)
        {
            var objList = _service.Get_TCWHONET_Column_List(searchModel);
            return objList;
        }

        [HttpPost]
        [Route("api/WHONET/WHONET_Organism_List/")]
        public IEnumerable<TCWHONET_OrganismDTO> Get_TCWHONET_Organism_List([FromBody] TCWHONET_OrganismDTO searchModel)
        {
            var objList = _service.Get_TCWHONET_Organism_List(searchModel);
            return objList;
        }

        [HttpPost]
        [Route("api/WHONET/WHONET_Macro_List/")]
        public IEnumerable<TCWHONET_MacroDTO> Get_TCWHONET_Macro_List([FromBody] TCWHONET_MacroDTO searchModel)
        {
            var objList = _service.Get_TCWHONET_Macro_List(searchModel);
            return objList;
        }

        [HttpPost]
        [Route("api/WHONET/Post_WhonetAntibiotics_SaveListData")]
        public TCWHONET_AntibioticsDTO Post_SaveData([FromBody] List<TCWHONET_AntibioticsDTO> models)
        {
            var objReturn = _service.SaveWHONET_AntibioticsListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_AntibioticSpecies_SaveListData")]
        public TCWHONET_AntibioticSpeciesDTO Post_SaveData([FromBody] List<TCWHONET_AntibioticSpeciesDTO> models)
        {
            var objReturn = _service.SaveWHONET_AntibioticSpeciesListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_Departments_SaveListData")]
        public TCWHONET_DepartmentsDTO Post_SaveData([FromBody] List<TCWHONET_DepartmentsDTO> models)
        {
            var objReturn = _service.SaveWHONET_DepartmentsListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_Antibiotic_SaveListData")]
        public TCAntibioticDTO Post_SaveData([FromBody] List<TCAntibioticDTO> models)
        {
            var objReturn = _service.SaveAntibioticListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_Specimen_SaveListData")]
        public TCWHONET_SpecimenDTO Post_SaveData([FromBody] List<TCWHONET_SpecimenDTO> models)
        {
            var objReturn = _service.SaveWHONET_SpecimenListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_Column_SaveListData")]
        public TCWHONETColumnDTO Post_SaveData([FromBody] List<TCWHONETColumnDTO> models)
        {
            var objReturn = _service.SaveWHONET_ColumnListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_Organism_SaveListData")]
        public TCWHONET_OrganismDTO Post_SaveData([FromBody] List<TCWHONET_OrganismDTO> models)
        {
            var objReturn = _service.SaveWHONET_OrganismListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_Macro_SaveListData")]
        public TCWHONET_MacroDTO Post_SaveData([FromBody] List<TCWHONET_MacroDTO> models)
        {
            var objReturn = _service.SaveWHONET_MacroListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_WhonetAntibiotics_DeleteListData")]
        public TCWHONET_AntibioticsDTO Post_DeleteData([FromBody] List<TCWHONET_AntibioticsDTO> models)
        {
            var objReturn = _service.DeleteWHONET_AntibioticsListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_AntibioticSpecies_DeleteListData")]
        public TCWHONET_AntibioticSpeciesDTO Post_DeleteData([FromBody] List<TCWHONET_AntibioticSpeciesDTO> models)
        {
            var objReturn = _service.DeleteWHONET_AntibioticSpeciesListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_Departments_DeleteListData")]
        public TCWHONET_DepartmentsDTO Post_DeleteData([FromBody] List<TCWHONET_DepartmentsDTO> models)
        {
            var objReturn = _service.DeleteWHONET_DepartmentsListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_Column_DeleteListData")]
        public TCWHONETColumnDTO Post_DeleteData([FromBody] List<TCWHONETColumnDTO> models)
        {
            var objReturn = _service.DeleteWHONET_ColumnListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_Antibiotic_DeleteListData")]
        public TCAntibioticDTO Post_DeleteData([FromBody] List<TCAntibioticDTO> models)
        {
            var objReturn = _service.DeleteAntibioticListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_Specimen_DeleteListData")]
        public TCWHONET_SpecimenDTO Post_DeleteData([FromBody] List<TCWHONET_SpecimenDTO> models)
        {
            var objReturn = _service.DeleteWHONET_SpecimenListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_Organism_DeleteListData")]
        public TCWHONET_OrganismDTO Post_DeleteData([FromBody] List<TCWHONET_OrganismDTO> models)
        {
            var objReturn = _service.DeleteWHONET_OrganismListData(models);

            return objReturn;
        }

        [HttpPost]
        [Route("api/WHONET/Post_Macro_DeleteListData")]
        public TCWHONET_MacroDTO Post_DeleteData([FromBody] List<TCWHONET_MacroDTO> models)
        {
            var objReturn = _service.DeleteWHONET_MacroListData(models);

            return objReturn;
        }
    }
}
