using ALISS.ANTIBIOTREND.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.ANTIBIOTREND.Library
{
    public interface IAntibiotrendService
    {
        List<SP_AntimicrobialResistanceDTO> GetAMRWithModel(SP_AntimicrobialResistanceSearchDTO searchModel);
        List<NationHealthStrategyDTO> GetAMRNationStrategyWithModel(AMRStrategySearchDTO searchModel);
        List<AntibiotrendAMRStrategyDTO> GetAntibiotrendAMRStrategyWithModel(AMRStrategySearchDTO searchModel);
        List<SP_AntimicrobialResistanceDTO> GetAMRByOverallWithModel(SP_AntimicrobialResistanceSearchDTO searchModel);
        List<SP_AntimicrobialResistanceDTO> GetAMRByOverallByHospWithModel(SP_AntimicrobialResistanceHospSearchDTO searchModel);
        List<SP_AntimicrobialResistanceDTO> GetAMRByOverallByAreaHWithModel(SP_AntimicrobialResistanceAreaHSearchDTO searchModel);
        List<SP_AntimicrobialResistanceDTO> GetAMRByOverallByProvWithModel(SP_AntimicrobialResistanceProvinceSearchDTO searchModel);
        List<SP_AntimicrobialResistanceDTO> GetAMRBySpecimenWithModel(SP_AntimicrobialResistanceSearchDTO searchModel);
        List<SP_AntimicrobialResistanceDTO> GetAMRBySpecimenByHospWithModel(SP_AntimicrobialResistanceHospSearchDTO searchModel);
        List<SP_AntimicrobialResistanceDTO> GetAMRBySpecimenByAreaHWithModel(SP_AntimicrobialResistanceAreaHSearchDTO searchModel);
        List<SP_AntimicrobialResistanceDTO> GetAMRBySpecimenByProvWithModel(SP_AntimicrobialResistanceProvinceSearchDTO searchModel);
        List<SP_AntimicrobialResistanceDTO> GetAMRByWardWithModel(SP_AntimicrobialResistanceSearchDTO searchModel);
        List<SP_AntimicrobialResistanceDTO> GetAMRByWardByHospWithModel(SP_AntimicrobialResistanceHospSearchDTO searchModel);
        List<SP_AntimicrobialResistanceDTO> GetAMRByWardByAreaHWithModel(SP_AntimicrobialResistanceAreaHSearchDTO searchModel);
        List<SP_AntimicrobialResistanceDTO> GetAMRByWardByProvWithModel(SP_AntimicrobialResistanceProvinceSearchDTO searchModel);
        List<AntibioticNameDTO> GetAntibioticNames();
    }
}
