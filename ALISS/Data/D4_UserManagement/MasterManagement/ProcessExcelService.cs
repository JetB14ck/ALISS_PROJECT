using ALISS.Data.Client;
using ALISS.DropDownList.DTO;
using ALISS.Master.DTO;
using ALISS.MasterManagement.DTO;
using ExcelDataReader;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ALISS.Data.D4_UserManagement.MasterManagement
{
    public class ProcessExcelService
    {
        private IConfiguration Configuration { get; }

        private ApiHelper _apiHelper;

        public ProcessExcelService(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration["ApiClient:ApiUrl"]);
            Configuration = configuration;
        }

        public async Task<List<TCProcessExcelColumnDTO>> Get_TCProcessExcelColumn_DataList_Async(TCProcessExcelColumnDTO param)
        {
            List<TCProcessExcelColumnDTO> objList = new List<TCProcessExcelColumnDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<TCProcessExcelColumnDTO, TCProcessExcelColumnDTO>("mastertemplate_api/Get_ProcessExcelColumn_DataList", param);

            return objList;
        }

        public async Task<List<TCProcessExcelRowDTO>> Get_TCProcessExcelRow_DataList_Async(TCProcessExcelRowDTO param)
        {
            List<TCProcessExcelRowDTO> objList = new List<TCProcessExcelRowDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<TCProcessExcelRowDTO, TCProcessExcelRowDTO>("mastertemplate_api/Get_ProcessExcelRow_DataList", param);

            return objList;
        }

        public async Task<List<TCProcessExcelTemplateDTO>> Get_TCProcessExcelTemplate_DataList_Async(TCProcessExcelTemplateDTO param)
        {
            List<TCProcessExcelTemplateDTO> objList = new List<TCProcessExcelTemplateDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<TCProcessExcelTemplateDTO, TCProcessExcelTemplateDTO>("mastertemplate_api/Get_ProcessExcelTemplate_DataList", param);

            return objList;
        }

        public async Task<TCProcessExcelColumnDTO> SaveProcessExcelColumnDataAsync(TCProcessExcelColumnDTO model)
        {
            var _object = await _apiHelper.PostDataAsync<TCProcessExcelColumnDTO>("mastertemplate_api/Post_ProcessExcelColumn_SaveData", model);

            return _object;
        }

        public async Task<TCProcessExcelRowDTO> SaveProcessExcelRowDataAsync(TCProcessExcelRowDTO model)
        {
            var _object = await _apiHelper.PostDataAsync<TCProcessExcelRowDTO>("mastertemplate_api/Post_ProcessExcelRow_SaveData", model);

            return _object;
        }

        public async Task<TCProcessExcelTemplateDTO> SaveProcessExcelTemplateDataAsync(TCProcessExcelTemplateDTO model)
        {
            var _object = await _apiHelper.PostDataAsync<TCProcessExcelTemplateDTO>("mastertemplate_api/Post_ProcessExcelTemplate_SaveData", model);

            return _object;
        }

        public async Task<TCProcessExcelColumnDTO> SaveProcessExcelColumnListDataAsync(List<TCProcessExcelColumnDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCProcessExcelColumnDTO>("mastertemplate_api/Post_ProcessExcelColumn_SaveListData", models);

            return _object;
        }
        public async Task<TCProcessExcelColumnDTO> DeleteProcessExcelColumnListDataAsync(List<TCProcessExcelColumnDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCProcessExcelColumnDTO>("mastertemplate_api/Post_ProcessExcelColumn_DeleteListData", models);

            return _object;
        }

        public async Task<TCProcessExcelRowDTO> SaveProcessExcelRowListDataAsync(List<TCProcessExcelRowDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCProcessExcelRowDTO>("mastertemplate_api/Post_ProcessExcelRow_SaveListData", models);

            return _object;
        }
        public async Task<TCProcessExcelRowDTO> DeleteProcessExcelRowListDataAsync(List<TCProcessExcelRowDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCProcessExcelRowDTO>("mastertemplate_api/Post_ProcessExcelRow_DeleteListData", models);

            return _object;
        }

        public async Task<TCProcessExcelTemplateDTO> SaveProcessExcelTemplateListDataAsync(List<TCProcessExcelTemplateDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCProcessExcelTemplateDTO>("mastertemplate_api/Post_ProcessExcelTemplate_SaveListData", models);

            return _object;
        }
        public async Task<TCProcessExcelTemplateDTO> DeleteProcessExcelTemplateListDataAsync(List<TCProcessExcelTemplateDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCProcessExcelTemplateDTO>("mastertemplate_api/Post_ProcessExcelTemplate_DeleteListData", models);

            return _object;
        }
        public async Task<List<TCProcessExcelErrorDTO>> ValidateProcessExcelFileAsync(string path, MasterTemplateDTO masterTemplate, string mst_code, string createUser, bool bReplace)
        {
            int? startColIndex = null;
            int? endColIndex = null;
            int startRowIndex = 3;
            //int? endRowIndex = null;
            int groupHeaderRowIndex = 1;
            int headerRowIndex = 2;
            DateTime createDate = DateTime.Now;

            var ErrorMessage = new List<TCProcessExcelErrorDTO>();

            List<TCProcessExcelColumnDTO> cols = new List<TCProcessExcelColumnDTO>();
            List<TCProcessExcelRowDTO> rows = new List<TCProcessExcelRowDTO>();
            List<TCProcessExcelTemplateDTO> templates = new List<TCProcessExcelTemplateDTO>();

            try
            {
                //path = Path.Combine(path, fileName);
                #region ReadExcel
                if (Path.GetExtension(path) == ".xlsm")
                {
                    ExcelDataSetConfiguration option = new ExcelDataSetConfiguration();

                    using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            DataSet result = new DataSet();
                            result = reader.AsDataSet();
                            foreach (DataTable dataTable in result.Tables)
                            {
                                if (dataTable.TableName != "Initial" && dataTable.TableName != "MapUrine")
                                {
                                    string sheetName = dataTable.TableName;

                                    #region Column Data
                                    int colGroupNum = 0;
                                    int colNum = 0;
                                    string colGroupName = string.Empty;
                                    string colName = string.Empty;

                                    bool isReaded = false;
                                    for (int i = 0; (!string.IsNullOrEmpty(dataTable.Rows[headerRowIndex][i].ToString()) && isReaded) || !isReaded; i++)
                                    {
                                        string ant_code = string.Empty;
                                        if (dataTable.Rows[groupHeaderRowIndex][i].ToString() == "TOTAL ISOLATES")
                                        {
                                            startColIndex = startColIndex ?? i + 1;
                                            isReaded = true;
                                            continue;
                                        }

                                        if (!string.IsNullOrEmpty(dataTable.Rows[headerRowIndex][i].ToString()))
                                        {
                                            if (!string.IsNullOrEmpty(dataTable.Rows[groupHeaderRowIndex][i].ToString()))
                                            {
                                                colGroupName = dataTable.Rows[groupHeaderRowIndex][i].ToString();
                                                colGroupNum++;
                                            }

                                            if (string.IsNullOrEmpty(ant_code) && isReaded)
                                            {
                                                List<string> antList = new List<string>();
                                                for (int r = headerRowIndex + 1; r <= dataTable.Rows.Count - 1; r++)
                                                {
                                                    if (!string.IsNullOrEmpty(dataTable.Rows[r][i].ToString()) && dataTable.Rows[r][i].ToString() != "R" && r % 2 == 1)
                                                        antList.Add(dataTable.Rows[r][i].ToString());
                                                }
                                                if (antList.Count() > 0)
                                                {
                                                    ant_code = antList.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key.ToString();
                                                }                                              
                                            }

                                            colNum = i + 1;
                                            endColIndex = colNum;
                                            colName = dataTable.Rows[headerRowIndex][i].ToString();
                                            if (!string.IsNullOrEmpty(ant_code))
                                            {
                                                cols.Add(new TCProcessExcelColumnDTO
                                                {
                                                    pec_mst_code = mst_code,
                                                    pec_sheet_name = sheetName,
                                                    pec_ant_code = ant_code,
                                                    pec_col_num = colNum,
                                                    pec_col_name = colName.ToString(),
                                                    pec_col_group_num = colGroupNum,
                                                    pec_col_group_name = colGroupName,
                                                    pec_MIC = colName.Contains("BY MIC") ? true : null,
                                                    pec_Urine = colName.Contains("(U)") ? true : null,
                                                    pec_createuser = createUser,
                                                    pec_createdate = createDate
                                                });
                                            }
                                            
                                        }
                                    }
                                    #endregion

                                    #region Row Data
                                    string rowGroupName = string.Empty;
                                    int rowGroupNum = 0;
                                    int rowNum = 0;
                                    if (sheetName == "Stool")
                                    {
                                        rowGroupNum = 1;
                                        rowGroupName = string.Empty;
                                        for (int i = startRowIndex; !string.IsNullOrEmpty(dataTable.Rows[i][0].ToString()) || !string.IsNullOrEmpty(dataTable.Rows[i - 1][0].ToString()); i++)
                                        {
                                            string rowName = dataTable.Rows[i][0].ToString();
                                            if (!string.IsNullOrEmpty(rowName))
                                            {
                                                string macro_name = string.Empty;

                                                if (string.IsNullOrEmpty(macro_name))
                                                {
                                                    List<string> macro_nameList = new List<string>();
                                                    for (int r = (int)startColIndex; r <= endColIndex; r++)
                                                    {
                                                        if (!string.IsNullOrEmpty(dataTable.Rows[i + 1][r].ToString()) && dataTable.Rows[i + 1][r].ToString() != "R")
                                                            macro_nameList.Add(dataTable.Rows[i + 1][r].ToString());
                                                    }
                                                    if (macro_nameList.Count() > 0)
                                                    {
                                                        macro_name = macro_nameList.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key.ToString();
                                                    }                                                                                                     
                                                }

                                                rowNum = i + 1;
                                                //endRowIndex = rowNum;
                                                if (!string.IsNullOrEmpty(macro_name))
                                                {
                                                    rows.Add(new TCProcessExcelRowDTO
                                                    {
                                                        per_mst_code = mst_code,
                                                        per_sheet_name = sheetName,
                                                        per_row_num = rowNum,
                                                        per_row_name = rowName,
                                                        per_row_group_num = rowGroupNum,
                                                        per_row_group_name = rowGroupName,
                                                        per_macro_name = macro_name,
                                                        per_createuser = createUser,
                                                        per_createdate = createDate
                                                    });
                                                }
                                                    
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = startRowIndex; !string.IsNullOrEmpty(dataTable.Rows[i][1].ToString()) || !string.IsNullOrEmpty(dataTable.Rows[i - 1][1].ToString()); i++)
                                        {
                                            string rowName = dataTable.Rows[i][1].ToString();
                                            if (!string.IsNullOrEmpty(rowName))
                                            {
                                                string macro_name = string.Empty;

                                                if (!string.IsNullOrEmpty(dataTable.Rows[i][0].ToString()) && dataTable.Rows[i][0].ToString() != rowGroupName)
                                                {
                                                    rowGroupName = dataTable.Rows[i][0].ToString();
                                                    rowGroupNum++;
                                                }

                                                if (string.IsNullOrEmpty(macro_name))
                                                {
                                                    List<string> macro_nameList = new List<string>();
                                                    for (int r = (int)startColIndex; r <= endColIndex; r++)
                                                    {
                                                        if((i+1) >= dataTable.Rows.Count)
                                                        {
                                                            break;
                                                        }
                                                        if (!string.IsNullOrEmpty(dataTable.Rows[i + 1][r].ToString()) && dataTable.Rows[i + 1][r].ToString() != "R")
                                                            macro_nameList.Add(dataTable.Rows[i + 1][r].ToString());
                                                    }
                                                    if(macro_nameList.Count() > 0)
                                                    {
                                                        macro_name = macro_nameList.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key.ToString();
                                                    }
                                                   
                                                }

                                                rowNum = i + 1;
                                                //endRowIndex = rowNum;
                                                if (!string.IsNullOrEmpty(macro_name))
                                                {
                                                    rows.Add(new TCProcessExcelRowDTO
                                                    {
                                                        per_mst_code = mst_code,
                                                        per_sheet_name = sheetName,
                                                        per_row_num = rowNum,
                                                        per_row_name = rowName,
                                                        per_row_group_num = rowGroupNum,
                                                        per_row_group_name = rowGroupName,
                                                        per_macro_name = macro_name,
                                                        per_createuser = createUser,
                                                        per_createdate = createDate
                                                    });
                                                }
                                                  
                                            }
                                        }
                                    }
                                    #endregion
                                }

                                #region Row Template
                                int sheetIndex = 0;
                                if (dataTable.TableName == "Initial")
                                {
                                    for (int i = 0; dataTable.Rows[0][i].ToString() != "Format" && i <= dataTable.Columns.Count - 1; i++)
                                    {
                                        if (!string.IsNullOrEmpty(dataTable.Rows[0][i].ToString()))
                                        {
                                            sheetIndex++;
                                            string sheetName = dataTable.Rows[0][i].ToString() == "All" ? "All Specimen" : dataTable.Rows[0][i].ToString();
                                            for (int r = 1; r <= dataTable.Rows.Count - 1; r++)
                                            {
                                                string rowNum = dataTable.Rows[r][sheetIndex * 4 - 4].ToString();
                                                string colNum = dataTable.Rows[r][sheetIndex * 4 - 3].ToString();
                                                string cellUp = dataTable.Rows[r][sheetIndex * 4 - 2].ToString();
                                                if (!string.IsNullOrEmpty(colNum) && !string.IsNullOrEmpty(rowNum))
                                                {
                                                    templates.Add(new TCProcessExcelTemplateDTO
                                                    {
                                                        pet_sheet_name = sheetName,
                                                        pet_mst_code = mst_code,
                                                        pet_row_num = Convert.ToInt32(rowNum),
                                                        pet_col_num = Convert.ToInt32(colNum),
                                                        pet_cell_sup = cellUp,
                                                        pet_a = cellUp == "a" ? true : null,
                                                        pet_b = cellUp == "b" ? true : null,
                                                        pet_c = cellUp == "c" ? true : null,
                                                        pet_d = cellUp == "d" ? true : null,
                                                        pet_e = cellUp == "e" ? true : null,
                                                        pet_f = cellUp == "f" ? true : null,
                                                        pet_h = cellUp == "h" ? true : null,
                                                        pet_i = cellUp == "i" ? true : null,
                                                        pet_u = cellUp == "u" ? true : null,
                                                        pet_wt = cellUp == "wt" ? true : null,
                                                        pet_r = cellUp == "r" ? true : null,
                                                        pet_site_inf = null,
                                                        pet_fix_value = null,
                                                        pet_merge = null,
                                                        pet_createdate = createDate,
                                                        pet_createuser = createUser
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }

                        }
                    }
                }
                #endregion

                #region Save Data        

                // Delete Old Data
                if (bReplace)
                {
                    var delresultColumn = await DeleteProcessExcelColumnListDataAsync(cols);
                    var delresultRpw = await DeleteProcessExcelRowListDataAsync(rows);
                    var delresultTemplate = await DeleteProcessExcelTemplateListDataAsync(templates);
                }

                var resultColumn = await SaveProcessExcelColumnListDataAsync(cols);
                var resultRpw = await SaveProcessExcelRowListDataAsync(rows);
                var resultTemplate = await SaveProcessExcelTemplateListDataAsync(templates);
                #endregion


                //var chkError = ErrorMessage.FirstOrDefault(x => x.tcp_status == 'E');
                //if (chkError != null)
                //{
                //    File.Delete(path);
                //}
                //else
                //{
                //    ErrorMessage.Add(new TCProcessExcelErrorDTO
                //    {
                //        tcp_status = 'E',
                //        tcp_Err_type = 'E',
                //        tcp_Err_No = "",
                //        tcp_Err_SheetName = "Total",
                //        tcp_Err_Message = ""
                //    });
                //}
            }
            catch (Exception ex)
            {
                ErrorMessage.Add(new TCProcessExcelErrorDTO
                {
                    tcp_status = 'E',
                    tcp_Err_type = 'E',
                    tcp_Err_No = "",
                    tcp_Err_SheetName = "Total",
                    tcp_Err_Message = ex.Message
                });
            }

            return ErrorMessage;
        }
        public async Task<string> GetPath()
        {
            var ErrorMessage = new List<TCProcessExcelErrorDTO>();

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
            }

            return path;
        }
    }
}
