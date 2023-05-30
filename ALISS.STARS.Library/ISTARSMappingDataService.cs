using System;
using System.Collections.Generic;
using ALISS.STARS.DTO;

namespace ALISS.STARS.Library
{
    public interface ISTARSMappingDataService
    {

        #region Mapping

        List<STARSMappingListsDTO> GetList();
        List<STARSMappingListsDTO> GetMappingList(string Param);
        STARSMappingDataDTO GetMappingDataById(string mp_id);
        STARSMappingDataDTO SaveMappingData(STARSMappingDataDTO model);
        STARSMappingDataDTO CopyMappingData(STARSMappingDataDTO objParam);
        STARSMappingDataDTO GetMappingDataWithModel(STARSMappingDataDTO model);
        STARSMappingDataDTO chkDuplicateMappingApproved(STARSMappingDataDTO model);
        STARSMappingDataDTO GetMappingDataActiveWithModel(STARSMappingDataDTO model);
        #endregion

        #region WHONetMapping

        List<STARSWHONetMappingListsDTO> GetWHONetMappingListWithModel(STARSWHONetMappingSearch searchModel);
        STARSWHONetMappingDataDTO GetWHONetMappingData(string wnm_id);
        STARSWHONetMappingDataDTO SaveWHONetMappingData(STARSWHONetMappingDataDTO model);
        STARSWHONetMappingDataDTO GetWHONetMappingDataWithModel(STARSWHONetMappingDataDTO model);
        #endregion


        #region SpecimenMapping

        List<STARSSpecimenMappingListsDTO> GetSpecimenMappingListWithModel(STARSSpecimenMappingSearch searchModel);
        STARSSpecimenMappingDataDTO GetSpecimenMappingData(string spm_id);
        STARSSpecimenMappingDataDTO GetSpecimenMappingDataWithModel(STARSSpecimenMappingDataDTO model);
        STARSSpecimenMappingDataDTO SaveSpecimenMappingData(STARSSpecimenMappingDataDTO model);

        #endregion

        #region OrganismMapping
        List<STARSOrganismMappingListsDTO> GetOrganismMappingListWithModel(STARSOrganismMappingSearch searchModel);
        STARSOrganismMappingDataDTO GetOrganismMappingData(string ogm_id);
        STARSOrganismMappingDataDTO GetOrganismMappingDataWithModel(STARSOrganismMappingDataDTO model);
        STARSOrganismMappingDataDTO SaveOrganismMappingData(STARSOrganismMappingDataDTO model);

        #endregion
    }
}
