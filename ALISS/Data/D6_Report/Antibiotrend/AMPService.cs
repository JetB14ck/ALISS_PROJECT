using ALISS.ANTIBIOTREND.DTO;
using ALISS.Data.Client;
using ALISS.DropDownList.DTO;
//using ALISS.EXPORT.Api.DTO;
using ALISS.EXPORT.Library.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Log4NetLibrary;

namespace ALISS.Data.D6_Report.Antibiotrend
{
    public class AMPService
    {
        private IConfiguration Configuration { get; }
        private string _reportPath;
        private ApiHelper _apiHelper;
        private static readonly ILogService log = new LogService(typeof(AMPService));
        public AMPService(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration["ApiClient:ApiUrl"]);
            _reportPath = configuration["ReportPath"];
        }

        public async Task<List<SP_AntimicrobialResistanceDTO>> GetAMRModelAsync(SP_AntimicrobialResistanceSearchDTO searchData)
        {
            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<SP_AntimicrobialResistanceDTO, SP_AntimicrobialResistanceSearchDTO>("antibiotrend_api/GetAMRModel", searchData);

            return objList;
        }
        public async Task<List<SP_AntimicrobialResistanceDTO>> GetAMROverallModelAsync(SP_AntimicrobialResistanceSearchDTO searchData)
        {
            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<SP_AntimicrobialResistanceDTO, SP_AntimicrobialResistanceSearchDTO>("antibiotrend_api/GetAMROverallModel", searchData);

            return objList;
        }
        public async Task<List<SP_AntimicrobialResistanceDTO>> GetAMROverallByAreaHealthModelAsync(SP_AntimicrobialResistanceAreaHSearchDTO searchData)
        {
            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<SP_AntimicrobialResistanceDTO, SP_AntimicrobialResistanceAreaHSearchDTO>("antibiotrend_api/GetAMROverallByAreaHModel", searchData);

            return objList;
        }
        public async Task<List<SP_AntimicrobialResistanceDTO>> GetAMROverallByHospModelAsync(SP_AntimicrobialResistanceHospSearchDTO searchData)
        {
            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<SP_AntimicrobialResistanceDTO, SP_AntimicrobialResistanceHospSearchDTO>("antibiotrend_api/GetAMROverallByHospModel", searchData);

            return objList;
        }
        public async Task<List<SP_AntimicrobialResistanceDTO>> GetAMROverallByProvinceModelAsync(SP_AntimicrobialResistanceProvinceSearchDTO searchData)
        {
            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<SP_AntimicrobialResistanceDTO, SP_AntimicrobialResistanceProvinceSearchDTO>("antibiotrend_api/GetAMROverallByProvModel", searchData);

            return objList;
        }
        public async Task<List<SP_AntimicrobialResistanceDTO>> GetAMRSpecimenModelAsync(SP_AntimicrobialResistanceSearchDTO searchData)
        {
            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<SP_AntimicrobialResistanceDTO, SP_AntimicrobialResistanceSearchDTO>("antibiotrend_api/GetAMRSpecimenModel", searchData);

            return objList;
        }
        public async Task<List<SP_AntimicrobialResistanceDTO>> GetAMRSpecimenByAreaHModelAsync(SP_AntimicrobialResistanceAreaHSearchDTO searchData)
        {
            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<SP_AntimicrobialResistanceDTO, SP_AntimicrobialResistanceAreaHSearchDTO>("antibiotrend_api/GetAMRSpecimenByAreaHModel", searchData);

            return objList;
        }
        public async Task<List<SP_AntimicrobialResistanceDTO>> GetAMRSpecimenByHospModelAsync(SP_AntimicrobialResistanceHospSearchDTO searchData)
        {
            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<SP_AntimicrobialResistanceDTO, SP_AntimicrobialResistanceHospSearchDTO>("antibiotrend_api/GetAMRSpecimenByHospModel", searchData);

            return objList;
        }
        public async Task<List<SP_AntimicrobialResistanceDTO>> GetAMRSpecimenByProvinceModelAsync(SP_AntimicrobialResistanceProvinceSearchDTO searchData)
        {
            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<SP_AntimicrobialResistanceDTO, SP_AntimicrobialResistanceProvinceSearchDTO>("antibiotrend_api/GetAMRSpecimenByProvModel", searchData);

            return objList;
        }
        public async Task<List<SP_AntimicrobialResistanceDTO>> GetAMRWardTypeModelAsync(SP_AntimicrobialResistanceSearchDTO searchData)
        {
            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<SP_AntimicrobialResistanceDTO, SP_AntimicrobialResistanceSearchDTO>("antibiotrend_api/GetAMRWardTypeModel", searchData);

            return objList;
        }
        public async Task<List<SP_AntimicrobialResistanceDTO>> GetAMRWardTypeByAreaHModelAsync(SP_AntimicrobialResistanceAreaHSearchDTO searchData)
        {
            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<SP_AntimicrobialResistanceDTO, SP_AntimicrobialResistanceAreaHSearchDTO>("antibiotrend_api/GetAMRWardTypeByAreaHModel", searchData);

            return objList;
        }
        public async Task<List<SP_AntimicrobialResistanceDTO>> GetAMRWardTypeByHospModelAsync(SP_AntimicrobialResistanceHospSearchDTO searchData)
        {
            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<SP_AntimicrobialResistanceDTO, SP_AntimicrobialResistanceHospSearchDTO>("antibiotrend_api/GetAMRWardTypeByHospModel", searchData);

            return objList;
        }
        public async Task<List<SP_AntimicrobialResistanceDTO>> GetAMRWardTypeByProvinceModelAsync(SP_AntimicrobialResistanceProvinceSearchDTO searchData)
        {
            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<SP_AntimicrobialResistanceDTO, SP_AntimicrobialResistanceProvinceSearchDTO>("antibiotrend_api/GetAMRWardTypeByProvModel", searchData);

            return objList;
        }

        public async Task<List<AntibioticNameDTO>> GetAntibioticNameAsync()
        {
            List<AntibioticNameDTO> objList = new List<AntibioticNameDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<AntibioticNameDTO, AntibioticNameDTO>("antibiotrend_api/GetAntibioticNameModel",new AntibioticNameDTO());

            return objList;
        }

        public async Task<string> ExportGraphFileDataAsync(AMRGraphSearchDTO selectedobj, string strDataLevelBy, string filename)
        {
            string statuscode = "";
            try
            {
                log.MethodStart();
                 //var filename = string.Format("{0}-{1}_{2}_{3}"
                 //                           , selectedobj.start_year
                 //                           , selectedobj.end_year
                 //                           , "AMRGraph.pdf");

                var strDirectoryPath = "ANTIBIOTREND" + "/" + DateTime.Today.ToString("yyyyMMdd");
                var filepath = strDirectoryPath + "/" + filename;
                

                List<ParameterDTO> objParamList = new List<ParameterDTO>();
                var searchParam = new ParameterDTO() { prm_code_major = "RPT_PROCESS_PATH" };
                objParamList = await _apiHelper.GetDataListByModelAsync<ParameterDTO, ParameterDTO>("dropdownlist_api/GetParameterList", searchParam);

                if (objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH") != null)
                {
                    _reportPath = objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH").prm_value;
                    var outputfileInfo = new FileInfo(Path.Combine(_reportPath, filepath));
                    if (!Directory.Exists(Path.Combine(_reportPath, strDirectoryPath)))
                    {
                        Directory.CreateDirectory(Path.Combine(_reportPath, strDirectoryPath));
                    }   
                    
                    if (strDataLevelBy == "N") //ระดับประเทศ
                    { 
                        statuscode = await _apiHelper.ExportDataAsync<AMRGraphSearchDTO>("exportgraph_api/ExportGraph", outputfileInfo, selectedobj);
                    }
                    else if (strDataLevelBy == "H") // ระดับโรงพยาบาล
                    {
                        statuscode = await _apiHelper.ExportDataAsync<AMRGraphSearchDTO>("exportgraph_api/ExportGraphByHosp", outputfileInfo, selectedobj);
                    }
                    else if (strDataLevelBy == "A") // ระดับเขต
                    {
                        statuscode = await _apiHelper.ExportDataAsync<AMRGraphSearchDTO>("exportgraph_api/ExportGraphByAreaH", outputfileInfo, selectedobj);
                    }
                    else if (strDataLevelBy == "P") // ระดับจังหวัด
                    {
                        statuscode = await _apiHelper.ExportDataAsync<AMRGraphSearchDTO>("exportgraph_api/ExportGraphByProv", outputfileInfo, selectedobj);
                    }
                    
                }
                else
                {
                    statuscode = "ERR_PATH";
                }
                       
                log.MethodFinish();
               
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
             return statuscode;
        }
        public async Task<List<NationHealthStrategyDTO>> GetAMRNationHealthStrategyModelAsync(AMRStrategySearchDTO searchData)
        {
            List<NationHealthStrategyDTO> objList = new List<NationHealthStrategyDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<NationHealthStrategyDTO, AMRStrategySearchDTO>("antibiotrend_api/GetNationHealthStrategyModel", searchData);

            return objList;
        }

        public async Task<List<AntibiotrendAMRStrategyDTO>> GetAMRStrategyModelAsync(AMRStrategySearchDTO searchData)
        {
            List<AntibiotrendAMRStrategyDTO> objList = new List<AntibiotrendAMRStrategyDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<AntibiotrendAMRStrategyDTO, AMRStrategySearchDTO>("antibiotrend_api/GetAMRStrategyModel", searchData);

            return objList;
        }
        public async Task<string> ExportMapDataAsync(AMRSearchMapDTO selectedobj)
        {
            string statuscode = "";
            try
            {
                log.MethodStart();
                var filename = string.Format("{0}_{1}-{2}_{3}"
                                            , DateTime.Today.ToString("yyyyMMdd")
                                            , selectedobj.month_start.Value.ToString("MMMyy", new CultureInfo("en-US"))
                                            , selectedobj.month_end.Value.ToString("MMMyy", new CultureInfo("en-US"))
                                            , "AMPMap.pdf"
                                            );
                //var filename = "AMPMAP.pdf";
           
                var strDirectoryPath = "ANTIBIOTREND" + "/" + DateTime.Today.ToString("yyyyMMdd");
                var filepath = strDirectoryPath + "/" + filename;
              
          
                List<ParameterDTO> objParamList = new List<ParameterDTO>();
                var searchParam = new ParameterDTO() { prm_code_major = "RPT_PROCESS_PATH" };
                objParamList = await _apiHelper.GetDataListByModelAsync<ParameterDTO, ParameterDTO>("dropdownlist_api/GetParameterList", searchParam);

                if (objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH") != null)
                {
                    _reportPath = objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH").prm_value;
                    var outputfileInfo = new FileInfo(Path.Combine(_reportPath, filepath));
                    if (!Directory.Exists(Path.Combine(_reportPath, strDirectoryPath)))
                    {
                        Directory.CreateDirectory(Path.Combine(_reportPath, strDirectoryPath));
                    }
                    statuscode = await _apiHelper.ExportDataAsync<AMRSearchMapDTO>("exportmap_api/ExportMap", outputfileInfo, selectedobj);
                }
                else
                {
                    statuscode = "ERR_PATH";
                }
                log.MethodFinish();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return statuscode;
        }
    }
}
