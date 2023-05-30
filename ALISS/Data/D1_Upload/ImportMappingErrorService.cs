using ALISS.Data.Client;
using ALISS.Data.D0_Master;
using ALISS.LabFileUpload.DTO;
using ALISS.Mapping.DTO;
using ALISS.DropDownList.DTO;
using ALISS.MasterManagement.DTO;
using DbfDataReader;
using ExcelDataReader;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using ALISS.Helpers;
using Newtonsoft.Json;

namespace ALISS.Data.D1_Upload
{
    public class ImportMappingErrorService
    {
        //private readonly IWebHostEnvironment _environment;
        //public FileUploadService(IWebHostEnvironment env)
        //{
        //    _environment = env;
        //}
        private IConfiguration configuration { get; }

        private ApiHelper _apiHelper;

        public ImportMappingErrorService(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration["ApiClient:ApiUrl"]);
        }

        private string _Path;


        public async Task<string> GetPath()
        {
            var ErrorMessage = new List<LabFileUploadErrorMessageDTO>();

            string path = "";
            List<ParameterDTO> objParamList = new List<ParameterDTO>();
            var searchModel = new ParameterDTO() { prm_code_major = "LAB_MAPPING_ERROR_PATH" };

            objParamList = await _apiHelper.GetDataListByModelAsync<ParameterDTO, ParameterDTO>("dropdownlist_api/GetParameterList", searchModel);

            if (objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH") != null)
            {
                path = objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH").prm_value;
            }
            return path;
        }

        public async Task<List<TRImportMappingLogErrorMessageDTO>> ValidateLabFileAsync(string path, string fileName)
        {

            var ErrorMessage = new List<TRImportMappingLogErrorMessageDTO>();
            int row = 1;
            try
            {
                path = Path.Combine(path, fileName);
                #region ReadExcel
                if (Path.GetExtension(fileName) == ".xls" || Path.GetExtension(fileName) == ".xlsx")
                {
                    ExcelDataSetConfiguration option = new ExcelDataSetConfiguration();

                    using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            DataSet result = new DataSet();

                            //First row is header
                            result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            }
                                );


                            //Rename column by mapping
                            string columns = "[{\"OriginalName\":\"รหัสโรงพยาบาล\",\"NewName\":\"hos_code\"},{\"OriginalName\":\"โรงพยาบาล\",\"NewName\":\"hos_name\"},{\"OriginalName\":\"เขตสุขภาพ\",\"NewName\":\"hos_arh_code\"},{\"OriginalName\":\"File Type\",\"NewName\":\"lfu_FileType\"},{\"OriginalName\":\"Program\",\"NewName\":\"lfu_Program\"},{\"OriginalName\":\"File Name\",\"NewName\":\"lfu_FileName\"},{\"OriginalName\":\"Field\",\"NewName\":\"feh_field\"},{\"OriginalName\":\"Message\",\"NewName\":\"feh_message\"},{\"OriginalName\":\"Local Value\",\"NewName\":\"fed_localvalue\"},{\"OriginalName\":\"Local Descr\",\"NewName\":\"fed_localdescr\"},{\"OriginalName\":\"feh_lfu_id\",\"NewName\":\"feh_lfu_id\"},{\"OriginalName\":\"lfu_mp_id\",\"NewName\":\"lfu_mp_id\"},{\"OriginalName\":\"Whonet Code\",\"NewName\":\"whonet_code\"}]";
                            List<MappingColumn> names = JsonConvert.DeserializeObject<List<MappingColumn>>(columns);
                            DataTable source = result.Tables[0];

                            // Validate file
                            if (names.Count != source.Columns.Count)
                            {
                                ErrorMessage.Add(new TRImportMappingLogErrorMessageDTO
                                {
                                    lfu_status = 'E',
                                    lfu_Err_type = 'E',
                                    lfu_Err_no = 1,
                                    lfu_Err_Column = "All",
                                    lfu_Err_Message = "รูปแบบไฟล์ไม่ถูกต้อง กรุณาตรวจสอบ!"
                                });
                                return ErrorMessage;
                            }

                            int error_no = 1;
                            foreach ( DataColumn col in source.Columns) {
                                if (!names.Where(x => x.OriginalName == col.ColumnName).Any())
                                {
                                    ErrorMessage.Add(new TRImportMappingLogErrorMessageDTO
                                    {
                                        lfu_status = 'E',
                                        lfu_Err_type = 'E',
                                        lfu_Err_no = error_no,
                                        lfu_Err_Column = col.ColumnName,
                                        lfu_Err_Message = string.Format("ไม่พบ column {0} กรุณาตรวจสอบ!", col.ColumnName)
                                    });
                                    error_no++;
                                }
                            }
                            if (ErrorMessage.Any())
                            {
                                return ErrorMessage;
                            }

                            // Rename
                            var dtResult = DataTableHelper.RenameColumn(result.Tables[0], names).Select("whonet_code<>''").CopyToDataTable();

                            //Save Temp to table
                            List<TempImportMappingLogDTO> objReturn = new List<TempImportMappingLogDTO>();
                            List<TempImportMappingLogDTO> models = new List<TempImportMappingLogDTO>();
                            models = DataTableHelper.ConvertDataTable<TempImportMappingLogDTO>(dtResult);
                            objReturn = await _apiHelper.PostDataAsync("mapping_api/Post_SaveTempImportMappingLogData", models);

                            ErrorMessage.Add(new TRImportMappingLogErrorMessageDTO
                            {
                                lfu_status = 'I',
                                lfu_Err_type = 'I',
                                lfu_Err_no = 1,
                                lfu_Err_Column = "Who_code",
                                lfu_Err_Message = dtResult.Rows.Count.ToString()
                            });

                            ErrorMessage.Add(new TRImportMappingLogErrorMessageDTO
                            {
                                lfu_status = 'I',
                                lfu_Err_type = 'I',
                                lfu_Err_no = 1,
                                lfu_Err_Column = "Total",
                                lfu_Err_Message = source.Rows.Count.ToString()
                            });
                        }
                    }
                }
                #endregion


                var chkError = ErrorMessage.FirstOrDefault(x => x.lfu_status == 'E');
                if (chkError != null)
                {
                    File.Delete(path);
                }
                else
                {
                    ErrorMessage.Add(new TRImportMappingLogErrorMessageDTO
                    {
                        lfu_status = 'I',
                        lfu_Err_type = 'P',
                        lfu_Err_no = 1,
                        lfu_Err_Column = "path",
                        lfu_Err_Message = path
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Add(new TRImportMappingLogErrorMessageDTO
                {
                    lfu_status = 'E',
                    lfu_Err_type = 'E',
                    lfu_Err_no = 1,
                    lfu_Err_Column = "",
                    lfu_Err_Message = ex.Message
                });
            }

            return ErrorMessage;
        }


        public async Task<TRImportMappingLogDTO> UploadFileAsync(TRImportMappingLogDTO model)
        {
            TRImportMappingLogDTO objReturn = new TRImportMappingLogDTO();
            objReturn = await _apiHelper.PostDataAsync("mapping_api/Post_SaveTRImportMappingLogData", model);

            return objReturn;
        }
    }
}