using ALISS.DropDownList.DTO;
using ALISS.GLASS.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.GLASS.Library
{
    public interface IGlassService
    {
        List<GlassFileListDTO> GetGlassPublicFileListData();
        List<GlassFileListDTO> GetGlassPublicFileListDataModel(GlassSearchDTO searchModel);
        //List<GlassFileListDTO> GetGlassPublicRegHealthFileListDataModel(GlassFileListNationSearchDTO searchModel);
        List<GlassInfectOriginOverviewDTO> GetGlassInfectOverviewModel(GlassAnalyzeListDTO searchModel);
        List<GlassPathogenNSDTO> GetGlassPathogenNSModel(GlassAnalyzeListDTO searchModel);
        List<GlassInfectSpecimenDTO> GlassInfectSpecimenModel(GlassAnalyzeListDTO searchModel);
        List<GlassInfectPathAntiCombineDTO> GetGlassInfectPathAntiCombineModel(GlassAnalyzeListDTO searchModel);
        List<ParameterDTO> GetGlassReportPath(ParameterDTO searchModel);
        List<GlassAnalyzeListDTO> GetGlassAreaHealthAnalyzeModel(GlassSearchDTO searchModel);
        List<GlassAnalyzeListDTO> GetGlassHospitalAnalyzeModel(GlassSearchDTO searchModel);
        List<GlassAnalyzeListDTO> GetGlassProvinceAnalyzeModel(GlassSearchDTO searchModel);
    }
}
