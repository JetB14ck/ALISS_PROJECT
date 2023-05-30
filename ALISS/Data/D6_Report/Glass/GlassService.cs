using ALISS.GLASS.DTO;
using ALISS.Data.Client;
using ALISS.DropDownList.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Log4NetLibrary;

namespace ALISS.Data.D6_Report.Glass
{
    public class GlassService
    {
        private IConfiguration Configuration { get; }
        private ApiHelper _apiHelper;
        private string _reportPath;
        private static readonly ILogService log = new LogService(typeof(GlassService));

        public GlassService(IConfiguration configuration)
        {           
            _apiHelper = new ApiHelper(configuration["ApiClient:ApiUrl"]);
            //_reportPath = configuration["ReportPath"];
        }

        public async Task<List<GlassFileListDTO>> GetGlassPublicFileListAsync()
        {
            List<GlassFileListDTO> objList = new List<GlassFileListDTO>();

            objList = await _apiHelper.GetDataListAsync<GlassFileListDTO>("glassreport_api/GetGlassPublicFileList");

            return objList;
        }

        public async Task<List<GlassFileListDTO>> GetGlassPublicFileListModelAsync(GlassSearchDTO searchData)
        {
            List<GlassFileListDTO> objList = new List<GlassFileListDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<GlassFileListDTO, GlassSearchDTO>("glassreport_api/GetGlassPublicFileListModel", searchData);

            return objList;
        }

        public async Task<List<GlassFileListDTO>> GetGlassPublicFileListRegHealthModelAsync(GlassSearchDTO searchData)
        {
            List<GlassFileListDTO> objList = new List<GlassFileListDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<GlassFileListDTO, GlassSearchDTO>("glassreport_api/GetGlassPublicRegHealthFileListModel", searchData);

            return objList;
        }

        public async Task<List<GlassAnalyzeListDTO>> GetGlassAreaHealthAnalyzeModelAsync(GlassSearchDTO searchData)
        {
            List<GlassAnalyzeListDTO> objList = new List<GlassAnalyzeListDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<GlassAnalyzeListDTO, GlassSearchDTO>("glassreport_api/GetGlassAreaHealthAnalyzeModel", searchData);

            return objList;
        }
        public async Task<List<GlassAnalyzeListDTO>> GetGlassHospitalAnalyzeModelAsync(GlassSearchDTO searchData)
        {
            List<GlassAnalyzeListDTO> objList = new List<GlassAnalyzeListDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<GlassAnalyzeListDTO, GlassSearchDTO>("glassreport_api/GetGlassHospitalAnalyzeModel", searchData);

            return objList;
        }

        public async Task<List<GlassAnalyzeListDTO>> GetGlassProvinceAnalyzeModelAsync(GlassSearchDTO searchData)
        {
            List<GlassAnalyzeListDTO> objList = new List<GlassAnalyzeListDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<GlassAnalyzeListDTO, GlassSearchDTO>("glassreport_api/GetGlassProvinceAnalyzeModel", searchData);

            return objList;
        }
        public async Task<string> RequestAnalyseFileAsync(GlassAnalyzeListDTO selectedobj)
        {
            // -- Note --
            // สร้าง Report ใหม่ทุกครั้งที่กดปุ่ม Download
            var statuscode = "";
            try
            {
                log.MethodStart();
                var filepath = selectedobj.gls_analyze_file_path.Remove(0, 1) + "\\" + selectedobj.gls_analyze_file_name;
                var strDirectoryPath = selectedobj.gls_analyze_file_path.Remove(0, 1);
                // Get Report Path
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
                    //FileInfo OriginalfileExcel = new FileInfo(strFullPath);                    
                    statuscode = await _apiHelper.ExportDataAsync<GlassAnalyzeListDTO>("glassreport_api/GenerateAnalyzeFile", outputfileInfo, selectedobj);
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

        //public async Task<string> RequestHospitalAnalyseFileAsync(GlassHospitalAnalyzeDTO selectedobj)
        //{
        //    // -- Note --
        //    // สร้าง Report ใหม่ทุกครั้งที่กดปุ่ม Download
        //    var statuscode = "";
        //    try
        //    {
        //        log.MethodStart();
        //        var filepath = selectedobj.gls_analyze_file_path.Remove(0, 1) + "\\" + selectedobj.gls_analyze_file_name;
        //        var strDirectoryPath = selectedobj.gls_analyze_file_path.Remove(0, 1);
        //        // Get Report Path
        //        List<ParameterDTO> objParamList = new List<ParameterDTO>();
        //        var searchParam = new ParameterDTO() { prm_code_major = "RPT_PROCESS_PATH" };
        //        objParamList = await _apiHelper.GetDataListByModelAsync<ParameterDTO, ParameterDTO>("dropdownlist_api/GetParameterList", searchParam);

        //        if (objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH") != null)
        //        {
        //            _reportPath = objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH").prm_value;
        //            var outputfileInfo = new FileInfo(Path.Combine(_reportPath, filepath));
        //            if (!Directory.Exists(Path.Combine(_reportPath, strDirectoryPath)))
        //            {
        //                Directory.CreateDirectory(Path.Combine(_reportPath, strDirectoryPath));
        //            }
        //            //FileInfo OriginalfileExcel = new FileInfo(strFullPath);                    
        //            statuscode = await _apiHelper.ExportDataAsync<GlassHospitalAnalyzeDTO>("glassreport_api/GenerateHospitalAnalyzeFile", outputfileInfo, selectedobj);
        //        }
        //        else
        //        {
        //            statuscode = "ERR_PATH";
        //        }
        //        log.MethodFinish();
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //    }
        //    return statuscode;
        //}
        public async Task<string> DownloadPDFFileAsync(GlassAnalyzeListDTO selectedobj)
        {
            var statuscode = "";
            try
            {
                log.MethodStart();
                var filename = selectedobj.gls_analyze_file_path.Remove(0, 1) + "/" + selectedobj.gls_analyze_file_name;          
                var fileextension = Path.GetExtension(selectedobj.gls_analyze_file_name).Replace(".",""); //.xlsx   
                var extension = Path.GetExtension(selectedobj.gls_analyze_file_name);
                var PdfFileName = filename.Replace(extension, ".pdf") ; // GLASS/20200730_01_GLASS/2019_13_glass.pdf
                
                var lstfullname = filename.Split("/");
                //var strFullPath = "";

                List<ParameterDTO> objParamList = new List<ParameterDTO>();
                var searchParam = new ParameterDTO() { prm_code_major = "RPT_PROCESS_PATH" };
                objParamList = await _apiHelper.GetDataListByModelAsync<ParameterDTO, ParameterDTO>("dropdownlist_api/GetParameterList", searchParam);
                              
                if (objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH") != null)
                {
                    _reportPath = objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH").prm_value;
                    FileInfo outputfileInfo = new FileInfo(Path.Combine(_reportPath, PdfFileName));
                    statuscode = await _apiHelper.ExportToPdfDataAsync("exportpdf_api/PdfGenerator", outputfileInfo, lstfullname);                    
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
