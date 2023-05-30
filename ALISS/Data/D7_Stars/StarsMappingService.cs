using ALISS.Data.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using ALISS.STARS.DTO;

namespace ALISS.Data.D7_StarsMapping
{
    public class StarsMappingService
    {
        private IConfiguration Configuration { get; }

        private ApiHelper _apiHelper;

        public StarsMappingService(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration["ApiClient:ApiUrl"]);
        }

        #region Mapping

        public async Task<List<STARSMappingListsDTO>> GetMappingListByParamAsync(STARSMappingSearchDTO mappingSearch)
        {
            List<STARSMappingListsDTO> objList = new List<STARSMappingListsDTO>();

            var searchJson = JsonSerializer.Serialize(mappingSearch);

            objList = await _apiHelper.GetDataListByParamsAsync<STARSMappingListsDTO>("stars_mapping_api/GetMappingList", searchJson);

            return objList;
        }

        public async Task<STARSMappingDataDTO> GetMappingDataAsync(string mp_Id)
        {
            STARSMappingDataDTO mapping = new STARSMappingDataDTO();

            mapping = await _apiHelper.GetDataByIdAsync<STARSMappingDataDTO>("stars_mapping_api/GetMappingDataById", mp_Id);

            return mapping;
        }

        public async Task<STARSMappingDataDTO> SaveMappingDataAsync(STARSMappingDataDTO model)
        {
            if (model.smp_id.Equals(Guid.Empty))
            {
                model.smp_id = Guid.NewGuid();
                model.smp_status = 'N';
                if(model.smp_version == 0)
                {
                    model.smp_version = 1;
                }                                
                model.smp_flagdelete = false;
            }

            if (model.smp_status.Equals('A'))
            {
                model.smp_status = 'A';
            }


            model.smp_updatedate = DateTime.Now;

            var mapping = await _apiHelper.PostDataAsync<STARSMappingDataDTO>("stars_mapping_api/Post_SaveMappingData", model);

            return mapping;
        }

        
        public async Task<STARSMappingDataDTO> CopyMappingDataAsync(STARSMappingDataDTO model)
        {

            var mapping = await _apiHelper.PostDataAsync<STARSMappingDataDTO>("stars_mapping_api/Post_CopyMappingData", model);

            return mapping;
        }

        public async Task<STARSMappingDataDTO> GetMappingDataByModelAsync(STARSMappingDataDTO model)
        {
            STARSMappingDataDTO objList = new STARSMappingDataDTO();
            objList = await _apiHelper.GetDataByModelAsync<STARSMappingDataDTO, STARSMappingDataDTO>("stars_mapping_api/Get_MappingDataByModel", model);
            return objList;
        }

        public async Task<STARSMappingDataDTO> GetchkDuplicateMappingApprovedAsync(STARSMappingDataDTO model)
        {
            STARSMappingDataDTO objList = new STARSMappingDataDTO();
            objList = await _apiHelper.GetDataByModelAsync<STARSMappingDataDTO, STARSMappingDataDTO>("stars_mapping_api/Get_chkDuplicateMappingApproved", model);
            return objList;
        }

        public async Task<STARSMappingDataDTO> GetMappingDataActiveWithModelAsync(STARSMappingDataDTO model)
        {
            STARSMappingDataDTO objList = new STARSMappingDataDTO();
            objList = await _apiHelper.GetDataByModelAsync<STARSMappingDataDTO, STARSMappingDataDTO>("stars_mapping_api/Get_MappingDataActiveWithModel", model);
            return objList;
        }

        #endregion

        #region WHONetMapping
        public async Task<List<STARSWHONetMappingListsDTO>> GetWHONetMappingListByModelAsync(STARSWHONetMappingSearch searchData)
        {
            List<STARSWHONetMappingListsDTO> objList = new List<STARSWHONetMappingListsDTO>();
            objList = await _apiHelper.GetDataListByModelAsync<STARSWHONetMappingListsDTO, STARSWHONetMappingSearch>("stars_mapping_api/Get_WHONetMappingListByModel", searchData);
            return objList;
        }

        public async Task<STARSWHONetMappingDataDTO> GetWHONetMappingDataAsync(string swm_Id)
        {
            STARSWHONetMappingDataDTO menu = new STARSWHONetMappingDataDTO();
            menu = await _apiHelper.GetDataByIdAsync<STARSWHONetMappingDataDTO>("stars_mapping_api/Get_WHONetMappingData", swm_Id);
            return menu;
        }

        public async Task<STARSWHONetMappingDataDTO> SaveWHONetMappingDataAsync(STARSWHONetMappingDataDTO model)
        {
            if (model.swm_id.Equals(Guid.Empty))
            {
                model.swm_id = Guid.NewGuid();
                model.swm_status = 'N';
                model.swm_flagdelete = false;
            }
            else
            {
                model.swm_status = 'E';
            }

            model.swm_updatedate = DateTime.Now;
            var whonetmapping = await _apiHelper.PostDataAsync<STARSWHONetMappingDataDTO>("stars_mapping_api/Post_SaveWHONetMappingData", model);


            return whonetmapping;
        }

        public async Task<STARSWHONetMappingDataDTO> GetWHONetMappingDataByModelAsync(STARSWHONetMappingDataDTO model)
        {
            STARSWHONetMappingDataDTO objList = new STARSWHONetMappingDataDTO();
            objList = await _apiHelper.GetDataByModelAsync<STARSWHONetMappingDataDTO, STARSWHONetMappingDataDTO>("stars_mapping_api/Get_WHONetMappingDataByModel", model);
            return objList;
        }
        #endregion

        #region SpecimenMapping
        public async Task<List<STARSSpecimenMappingListsDTO>> GetSpecimenMappingListByModelAsync(STARSSpecimenMappingSearch searchData)
        {
            List<STARSSpecimenMappingListsDTO> objList = new List<STARSSpecimenMappingListsDTO>();
            objList = await _apiHelper.GetDataListByModelAsync<STARSSpecimenMappingListsDTO, STARSSpecimenMappingSearch>("stars_mapping_api/Get_SpecimenMappingListByModel", searchData);
            return objList;
        }

        public async Task<STARSSpecimenMappingDataDTO> GetSpecimenMappingDataByModelAsync(STARSSpecimenMappingDataDTO model)
        {
            STARSSpecimenMappingDataDTO objList = new STARSSpecimenMappingDataDTO();
            objList = await _apiHelper.GetDataByModelAsync<STARSSpecimenMappingDataDTO, STARSSpecimenMappingDataDTO>("stars_mapping_api/Get_SpecimenMappingDataByModel", model);
            return objList;
        }

        public async Task<STARSSpecimenMappingDataDTO> GetSpecimenMappingDataAsync(string ssm_Id)
        {
            STARSSpecimenMappingDataDTO SpecimenMapping = new STARSSpecimenMappingDataDTO();
            SpecimenMapping = await _apiHelper.GetDataByIdAsync<STARSSpecimenMappingDataDTO>("stars_mapping_api/Get_SpecimenMappingData", ssm_Id);
            return SpecimenMapping;
        }

        public async Task<STARSSpecimenMappingDataDTO> SaveSpecimenMappingDataAsync(STARSSpecimenMappingDataDTO model)
        {
            if (model.ssm_id.Equals(Guid.Empty))
            {
                model.ssm_id = Guid.NewGuid();
                model.ssm_status = 'N';
                model.ssm_flagdelete = false;
            }
            else
            {
                model.ssm_status = 'E';
            }

            model.ssm_updatedate = DateTime.Now;
            var specimenmapping = await _apiHelper.PostDataAsync<STARSSpecimenMappingDataDTO>("stars_mapping_api/Post_SaveSpecimenMappingData", model);
            return specimenmapping;
        }
        #endregion

        #region OrganismMapping
        public async Task<List<STARSOrganismMappingListsDTO>> GetOrganismMappingListByModelAsync(STARSOrganismMappingSearch searchData)
        {
            List<STARSOrganismMappingListsDTO> objList = new List<STARSOrganismMappingListsDTO>();
            objList = await _apiHelper.GetDataListByModelAsync<STARSOrganismMappingListsDTO, STARSOrganismMappingSearch>("stars_mapping_api/Get_OrganismMappingListByModel", searchData);
            return objList;
        }

        public async Task<STARSOrganismMappingDataDTO> GetOrganismMappingDataByModelAsync(STARSOrganismMappingDataDTO model)
        {
            STARSOrganismMappingDataDTO objList = new STARSOrganismMappingDataDTO();
            objList = await _apiHelper.GetDataByModelAsync<STARSOrganismMappingDataDTO, STARSOrganismMappingDataDTO>("stars_mapping_api/Get_OrganismMappingDataByModel", model);
            return objList;
        }

        public async Task<STARSOrganismMappingDataDTO> GetOrganismMappingDataAsync(string som_Id)
        {
            STARSOrganismMappingDataDTO OrganismMapping = new STARSOrganismMappingDataDTO();
            OrganismMapping = await _apiHelper.GetDataByIdAsync<STARSOrganismMappingDataDTO>("stars_mapping_api/Get_OrganismMappingData", som_Id);
            return OrganismMapping;
        }

        public async Task<STARSOrganismMappingDataDTO> SaveOrganismMappingDataAsync(STARSOrganismMappingDataDTO model)
        {
            if (model.som_id.Equals(Guid.Empty))
            {
                model.som_id = Guid.NewGuid();
                model.som_status = 'N';
                model.som_flagdelete = false;
            }
            else
            {
                model.som_status = 'E';
            }



            model.som_updatedate = DateTime.Now;
            var organismmapping = await _apiHelper.PostDataAsync<STARSOrganismMappingDataDTO>("stars_mapping_api/Post_SaveOrganismMappingData", model);
            return organismmapping;
        }
        #endregion
    }
}
