using ALISS.Data.Client;
using ALISS.DropDownList.DTO;
using ALISS.HISUpload.DTO;
using ExcelDataReader;
using Microsoft.AspNetCore.Components.Forms;
//using ExcelNumberFormat;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ALISS.Data.D5_HISData
{
    public class FileUploadManageService
    {
        private IConfiguration configuration { get; }

        private ApiHelper _apiHelper;
        private string _Path;
        public FileUploadManageService(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration["ApiClient:ApiUrl"]);
        }
        public async Task<List<HISUploadErrorMessageDTO>> ValidateAndUploadFileSPFileAsync(string path, string fileName
                                                                                            , HISFileTemplateDTO HISTemplateActive
                                                                                            , string fileType)
        {
            List<HISUploadErrorMessageDTO> ErrorMessage = new List<HISUploadErrorMessageDTO>();
            int row = 1;
            try
            {
               
                List<ParameterDTO> objParamList = new List<ParameterDTO>();
                var searchModel = new ParameterDTO() { prm_code_major = "UPLOAD_PATH" };

                const bool FIRSTROW_IS_HEADER = true;
                const int colRefNo = 0;
                const int colHNNo = 1;
                const int colLabNo = 2;
                const int colSpecDate = 3;
                var formateDate = HISTemplateActive.hft_date_format;
                var COL_REF_NO = HISTemplateActive.hft_field1; // "Ref No";
                var COL_HN_NO = HISTemplateActive.hft_field2; //"HN";
                var COL_LAB_NO = HISTemplateActive.hft_field3; // "Lab";
                var COL_DATE = HISTemplateActive.hft_field4; //"Date";
                DataSet result = new DataSet();

                path = Path.Combine(path, fileName);
                if (Path.GetExtension(fileName) == ".xls" || Path.GetExtension(fileName) == ".xlsx")
                {
                    ExcelDataSetConfiguration option = new ExcelDataSetConfiguration();
                    using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            
                            //First row is header
                            if (FIRSTROW_IS_HEADER == true)
                            {

                                result = reader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                    {
                                        UseHeaderRow = true
                                    }
                                }
                                );
                            }
                            else
                            {
                                result = reader.AsDataSet();
                            }

                            var dtResult = result.Tables[0];

                            foreach (DataRow rows in dtResult.Rows)
                            {
                                DateTime temp = new DateTime();
                                formateDate = Regex.Replace(formateDate, "d", "d", RegexOptions.IgnoreCase);
                                formateDate = Regex.Replace(formateDate, "m", "M", RegexOptions.IgnoreCase);
                                formateDate = Regex.Replace(formateDate, "y", "y", RegexOptions.IgnoreCase);                            

                                if (rows[COL_DATE].GetType() != typeof(DateTime))
                                {
                                    DateTime.TryParseExact(rows[COL_DATE].ToString(), formateDate, System.Globalization.CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out temp);
                                    if (temp == DateTime.MinValue)
                                    {
                                        if (temp == DateTime.MinValue)
                                        {
                                            ErrorMessage.Add(new HISUploadErrorMessageDTO
                                            {
                                                hfu_status = 'E',
                                                hfu_Err_type = "E",
                                                hfu_Err_no = 1,
                                                hfu_Err_Column = "",
                                                hfu_Err_Message = "Column Date ต้องอยู่ในรูปแบบ " + formateDate
                                            }); ;
                                            return ErrorMessage;
                                        }
                                    }
                                 }

                                
                                //var strFileFormatDate = reader.GetNumberFormatString(colSpecDate);
                                //if ((strFileFormatDate.ToLower() == formateDate.ToLower())
                                //       || (strFileFormatDate.ToLower() == "d/m/yyyy"))
                                //{
                                //    //collect format
                                //}
                                //else
                                //{
                                //    ErrorMessage.Add(new HISUploadErrorMessageDTO
                                //    {
                                //        hfu_status = 'E',
                                //        hfu_Err_type = "E",
                                //        hfu_Err_no = 1,
                                //        hfu_Err_Column = "",
                                //        hfu_Err_Message = "Column Date ต้องอยู่ในรูปแบบ " + formateDate
                                //    }); ;
                                //    return ErrorMessage;
                                //}
                            }

                            //var dtResult = result.Tables[0];
                            ErrorMessage.Add(new HISUploadErrorMessageDTO
                            {
                                hfu_status = 'I',
                                hfu_Err_type = "I",
                                hfu_Err_no = 1,
                                hfu_Err_Column = "Total",
                                hfu_Err_Message = dtResult.Rows.Count.ToString()
                            });

                        }
                    }
                }
                   

                //----------  Validate Data in File -------------
                var dataTable = result.Tables[0];        
                if (dataTable is not null && dataTable.Rows.Count > 0)
                {
                    // Check column Exist
                    //Boolean columnExists = result.Tables[0].Columns.Contains(COL_REF_NO) 
                    //                    && result.Tables[0].Columns.Contains(COL_HN_NO)
                    //                    && result.Tables[0].Columns.Contains(COL_LAB_NO)
                    //                    && result.Tables[0].Columns.Contains(COL_DATE);
                    if (!result.Tables[0].Columns.Contains(COL_REF_NO))
                    {
                        ErrorMessage.Add(new HISUploadErrorMessageDTO
                        {
                            hfu_status = 'E',
                            hfu_Err_type = "C",
                            hfu_Err_no = 1,
                            hfu_Err_Column = COL_REF_NO,
                            hfu_Err_Message = "ไม่พบ Column " + COL_REF_NO
                        });
                    }
                    else if (!result.Tables[0].Columns.Contains(COL_HN_NO))
                    {
                        ErrorMessage.Add(new HISUploadErrorMessageDTO
                        {
                            hfu_status = 'E',
                            hfu_Err_type = "C",
                            hfu_Err_no = 1,
                            hfu_Err_Column = COL_HN_NO,
                            hfu_Err_Message = "ไม่พบ Column " + COL_HN_NO
                        });
                    }
                    else if (!result.Tables[0].Columns.Contains(COL_LAB_NO))
                    {
                        ErrorMessage.Add(new HISUploadErrorMessageDTO
                        {
                            hfu_status = 'E',
                            hfu_Err_type = "C",
                            hfu_Err_no = 1,
                            hfu_Err_Column = COL_LAB_NO,
                            hfu_Err_Message = "ไม่พบ Column " + COL_LAB_NO
                        });
                    }
                    else if (!result.Tables[0].Columns.Contains(COL_DATE))
                    {
                        ErrorMessage.Add(new HISUploadErrorMessageDTO
                        {
                            hfu_status = 'E',
                            hfu_Err_type = "C",
                            hfu_Err_no = 1,
                            hfu_Err_Column = COL_DATE,
                            hfu_Err_Message = "ไม่พบ Column " + COL_DATE
                        });
                    }                 

                    for (var i = 0; i < dataTable.Rows.Count; i++)
                    {
                        for (var j = 0; j < 4; j++)
                        {

                            var data = dataTable.Rows[i][j];

                            // Check column not null
                            if (string.IsNullOrEmpty(data.ToString()))
                            {
                                string columnError = "";

                                if (fileType == "HIS")
                                {
                                    if (j == colHNNo) { columnError = COL_HN_NO; }
                                    else if (j == colLabNo) { columnError = COL_LAB_NO; }
                                    else if (j == colSpecDate) { columnError = COL_DATE; }
                                }
                                else
                                {
                                    if (j == colRefNo) { columnError = COL_REF_NO; }
                                    else if (j == colHNNo) { columnError = COL_HN_NO; }
                                    else if (j == colLabNo) { columnError = COL_LAB_NO; }
                                    else if (j == colSpecDate) { columnError = COL_DATE; }
                                }

                                if (columnError == "")
                                {
                                    continue;
                                }
                                var chkErrExist = ErrorMessage.FirstOrDefault(e => e.hfu_status == 'W' && e.hfu_Err_no == i + 2);                       

                                if (chkErrExist == null)
                                {
                                    ErrorMessage.Add(new HISUploadErrorMessageDTO
                                    {
                                        hfu_status = 'W',
                                        hfu_Err_type = columnError,
                                        hfu_Err_no = i + 2,
                                        hfu_Err_Column = "Required",
                                        hfu_Err_Message = "Field is required"
                                    });
                                }

                                else
                                {
                                    chkErrExist.hfu_Err_type += ',' + columnError;
                                }

                            }

                        }
                    }

                    var chkError = ErrorMessage.FirstOrDefault(x => x.hfu_status == 'E');
                    if (chkError != null)
                    {
                        File.Delete(path);
                    }
                    else
                    {
                        ErrorMessage.Add(new HISUploadErrorMessageDTO
                        {
                            hfu_status = 'I',
                            hfu_Err_type = "P",
                            hfu_Err_no = 1,
                            hfu_Err_Column = "path",
                            hfu_Err_Message = path
                        });
                    }

                }
                                
            }
            catch (Exception ex)
            {
                //create log
            }
            return ErrorMessage;
        }

        public async Task<HISUploadDataDTO> SaveFileUploadAsync(IBrowserFile fileEntry, HISUploadDataDTO model)
        {
            HISUploadDataDTO objReturn = new HISUploadDataDTO();

            if (model.hfu_id == 0)
            {              
                model.hfu_status = 'N';
                model.hfu_delete_flag = false;
            }
            else
            {
                model.hfu_status = 'E';
            }

            //model.hfu_updatedate = DateTime.Now;
            objReturn = await _apiHelper.PostDataAsync<HISUploadDataDTO>("his_api/Post_SaveSPFileUploadData", model);

            return objReturn;
        }

        public async Task<HISFileUploadSummaryDTO> SaveFileUploadSumaryAsync(List<HISFileUploadSummaryDTO> models)
        {
            HISFileUploadSummaryDTO objReturn = new HISFileUploadSummaryDTO();

            objReturn = await _apiHelper.PostListofDataAsync<HISFileUploadSummaryDTO>("his_api/Post_SaveFileUploadSummary", models);

            return objReturn;
        }

        public async Task<string> GetPath()
        {
            //var ErrorMessage = new List<LabFileUploadErrorMessageDTO>();

            string path = "";
            List<ParameterDTO> objParamList = new List<ParameterDTO>();
            var searchModel = new ParameterDTO() { prm_code_major = "UPLOAD_PATH" };

            objParamList = await _apiHelper.GetDataListByModelAsync<ParameterDTO, ParameterDTO>("dropdownlist_api/GetParameterList", searchModel);

            if (objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH") != null)
            {
                path = objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH").prm_value;
            }
            else
            {
                //ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                //{
                //    lfu_status = 'E',
                //    lfu_Err_type = 'E',
                //    lfu_Err_no = 1,
                //    lfu_Err_Column = "",
                //    lfu_Err_Message = "ไม่พบ Config PATH กรุณาติดต่อผู้ดูแลระบบ "
                //});

                //return ErrorMessage;
            }

            return path;
        }
    }
}
