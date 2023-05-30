using ALISS.Process.Batch.DataAccess;
using ALISS.Process.Batch.DTO;
using ALISS.Process.Batch.Model;
using ALISS.Process.Batch.SQLite;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Process.Batch
{
    public class P03_WHONET_GLASS_Service : IDisposable
    {
        private static string whonetPath;
        private string whonetMacroPath;
        private static int process_year;
        private static List<TRProcessRequestDTO> processRequestList = new List<TRProcessRequestDTO>();

        public void Dispose()
        {

        }

        private string Get_Process_FolderName(TRProcessRequest processRequest)
        {
            return $"P_{processRequest.pcr_code}_{(processRequest.pcr_arh_code ?? "00")}_{(processRequest.pcr_prv_code ?? "00")}_{(processRequest.pcr_hos_code ?? "000000000")}_{(processRequest.pcr_lab_code ?? "000")}_{processRequest.pcr_year}_{processRequest.pcr_month_start}_{processRequest.pcr_month_end}";
        }
        private static string pcr_code_pattern()
        {
            return "PCR_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00");
        }

        private string Get_Process_File_Before(TRProcessRequest processRequest)
        {
            return $"{Get_Process_FolderName(processRequest)}-GLASS-Before.zip";
        }

        private string Get_Process_File_After(TRProcessRequest processRequest)
        {
            return $"{Get_Process_FolderName(processRequest)}-GLASS-After.zip";
        }

        private string Get_Process_File_Result(TRProcessRequest processRequest)
        {
            return $"{Get_Process_FolderName(processRequest)}-GLASS-Result.zip";
        }

        private string Get_Data_FolderName(TRProcessRequestHISData processHISData)
        {
            return $"P_GLASS_{(processHISData.pch_arh_code ?? "00")}_{(processHISData.pch_prv_code ?? "00")}_{(processHISData.pch_hos_code ?? "000000000")}_{(processHISData.pch_lab_code ?? "000")}_{processHISData.pch_year}";
        }

        private string Get_Data_Filename(TRProcessRequestHISData processHISData, int month)
        {
            return $"P_GLASS_{(processHISData.pch_arh_code ?? "00")}_{(processHISData.pch_prv_code ?? "00")}_{(processHISData.pch_hos_code ?? "000000000")}_{(processHISData.pch_lab_code ?? "000")}_{processHISData.pch_year}_{month.ToString("00")}.sqlite";
        }

        private string Get_Data_File_List(TRProcessRequest processRequest, List<TRProcessRequestHISData> processHISList)
        {
            string dataFilename = "";
            var str_list = new List<string>();

            foreach (var processHISData in processHISList)
            {
                var month_end = 0;
                if (processRequest.pcr_month_end == "03") month_end = 3;
                else if (processRequest.pcr_month_end == "06") month_end = 6;
                else if (processRequest.pcr_month_end == "09") month_end = 9;
                else if (processRequest.pcr_month_end == "12") month_end = 12;

                for (var i = 1; i <= month_end; i++)
                {
                    str_list.Add($"{Get_Data_FolderName(processHISData)}\\{Get_Data_Filename(processHISData, i)}");
                }
            }

            dataFilename = string.Join(",", str_list);

            return dataFilename;
        }

        public P03_WHONET_GLASS_Service()
        {
            whonetPath = P00_BATCH_Service.GetConfigurationValue("WHONET:PARAM:PATH");
            process_year = Convert.ToInt32(P00_BATCH_Service.GetConfigurationValue("WHONET:PARAM:PROCESS_YEAR"));
            whonetMacroPath = P00_BATCH_Service.GetConfigurationValue("WHONET:GLASS:PARAM:MACRO_TEMPLATE_FOLDER");
        }

        #region WHONET:GLASS:FILE_BEFORE:RUN_SERVICE
        public void FILE_BEFORE_SERVICE_AUTO()
        {
            Console.WriteLine($"FILE_BEFORE_SERVICE_AUTO : START");

            var processHISList = GET_LIST_ProcessRequestHISData(x => x.pch_status == "W");
            foreach (var processHISData in processHISList)
            {
                Console.WriteLine($"FILE_BEFORE_SERVICE : START : {processHISData.pch_hos_code}");

                FILE_BEFORE_SERVICE(processHISData);
            }

            Console.WriteLine($"FILE_BEFORE_SERVICE_AUTO : END");
        }

        public void FILE_BEFORE_SERVICE_MANUAL(string processList)
        {
            Console.WriteLine($"FILE_BEFORE_SERVICE_MANUAL : START");

            var process_split = processList.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var process in process_split)
            {
                var processInfo = process.Split('|', StringSplitOptions.RemoveEmptyEntries);

                var hos_code = processInfo[0];
                var pcr_month_start = "01";
                var pcr_month_end = processInfo[1];
                var pcr_year = Convert.ToInt32(processInfo[2]);

                Console.WriteLine($"FILE_BEFORE_SERVICE : START : {hos_code} : {pcr_month_start} : {pcr_month_end} : {pcr_year}");

                var processHISList = GET_LIST_ProcessRequestHISData(x => x.pch_hos_code == hos_code && x.pch_year == pcr_year);
                var processHISData = new TRProcessRequestHISData();

                if (processHISList != null && processHISList.Count > 0)
                {
                    processHISData = processHISList.FirstOrDefault();
                }
                else
                {
                    processHISData = INSERT_ProcessRequestHISData(hos_code, pcr_year);
                }

                FILE_BEFORE_SERVICE(processHISData);
            }

            Console.WriteLine($"FILE_BEFORE_SERVICE_MANUAL : END");
        }

        private void FILE_BEFORE_SERVICE(TRProcessRequestHISData processHISData)
        {
            CREATE_GLASS_FILE_BEFORE(processHISData);

            Console.WriteLine($"FILE_BEFORE_SERVICE : END");
        }
        #endregion

        #region WHONET:GLASS:RUN_SERVICE
        public void RUN_GLASS_SERVICE_AUTO()
        {
            Console.WriteLine($"RUN_GLASS_SERVICE_AUTO : START");

            var processRequestList = GET_LIST_ProcessRequest(x => x.pcr_type == "02" && (x.pcr_status == "N" || x.pcr_status == "R"));
            List<TRProcessRequestHISData> processRequestHISDataList = new List<TRProcessRequestHISData>();
            foreach (var processRequest in processRequestList)
            {
                Console.WriteLine($"RUN_GLASS_SERVICE_AUTO : START : {processRequest.pcr_month_end + "|" + processRequest.pcr_year}");

                var processRequestDetailList = GET_LIST_ProcessRequestDetail(x => x.pcd_pcr_code == processRequest.pcr_code);

                foreach (var processRequestDetail in processRequestDetailList)
                {
                    processRequestHISDataList.AddRange(GET_LIST_ProcessRequestHISData(x => x.pch_year == Convert.ToInt32(processRequest.pcr_year) && x.pch_hos_code == processRequest.pcr_hos_code));
                }

                var processRequestHISData = new TRProcessRequestHISData();
                if (string.IsNullOrEmpty(processRequest.pcr_arh_code) == false)
                {
                    processRequestHISData = new TRProcessRequestHISData()
                    {
                        pch_arh_code = processRequest.pcr_arh_code,
                        pch_prv_code = processRequest.pcr_prv_code,
                        pch_hos_code = processRequest.pcr_hos_code,
                        pch_lab_code = processRequest.pcr_lab_code,
                        pch_year = Convert.ToInt32(processRequest.pcr_year)
                    };
                }

                GLASS_SERVICE(processRequestHISDataList, processRequestHISData, processRequest.pcr_month_start, processRequest.pcr_month_end);
            }

            Console.WriteLine($"RUN_GLASS_SERVICE_AUTO : END");
        }

        public void RUN_GLASS_SERVICE_MANUAL(string processList)
        {
            Console.WriteLine($"RUN_GLASS_SERVICE_MANUAL : START");

            var process_split = processList.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var process in process_split)
            {
                Console.WriteLine($"RUN_GLASS_SERVICE_MANUAL : START : {process}");
                var processInfo = process.Split('|', StringSplitOptions.RemoveEmptyEntries);

                var hos_code = processInfo[0];
                var pcr_month_start = "01";
                var pcr_month_end = processInfo[1];
                var pcr_year = Convert.ToInt32(processInfo[2]);

                var processHISList = new List<TRProcessRequestHISData>();
                var processHISData = new TRProcessRequestHISData();

                if (processInfo[0] == "000000000")
                {
                    processHISList = GET_LIST_ProcessRequestHISData(x => x.pch_year == pcr_year);
                }
                else
                {
                    processHISList = GET_LIST_ProcessRequestHISData(x => x.pch_hos_code == hos_code && x.pch_year == pcr_year);

                    processHISData = processHISList.FirstOrDefault();
                }

                if (processHISList != null && processHISList.Count > 0)
                {
                    GLASS_SERVICE(processHISList, processHISData, pcr_month_start, pcr_month_end);
                }
            }

            Console.WriteLine($"RUN_GLASS_SERVICE_MANUAL : END");
        }
 
        private void GLASS_SERVICE(List<TRProcessRequestHISData> processHISList, TRProcessRequestHISData processHISData, string month_start, string month_end)
        {
            Console.WriteLine($"GLASS_SERVICE : START");

            string pcr_prev_code = null;

            var prev_processRequest = GET_DATA_ProcessRequest(x => x.pcr_arh_code == processHISData.pch_arh_code && x.pcr_prv_code == processHISData.pch_prv_code && x.pcr_hos_code == processHISData.pch_hos_code && x.pcr_lab_code == processHISData.pch_lab_code && x.pcr_year == processHISData.pch_year.ToString() && x.pcr_month_start == month_start && x.pcr_month_end == month_end);

            if (prev_processRequest != null)
            {
                pcr_prev_code = prev_processRequest.pcr_prev_code;
                UPDATE_ProcessRequest(prev_processRequest);
            }

            var processRequest = INSERT_ProcessRequest(processHISData, month_start, month_end, pcr_prev_code);

            pcr_prev_code = processRequest.pcr_code;

            foreach (var processLab in processHISList)
            {
                INSERT_ProcessRequestDetail(processLab, pcr_prev_code);
            }

            CREATE_GLASS_MACRO_FILE(processRequest, processHISList);

            GLASS_Run_WHONET(processRequest);

            GLASS_Insert_Result(processRequest);

            Console.WriteLine("GLASS_SERVICE : END");
        }
       #endregion

        public List<TRProcessRequest> GET_LIST_ProcessRequest(Func<TRProcessRequest, bool> query)
        {
            Console.WriteLine($"GET_LIST_ProcesRequest : START");

            var processHISList = new List<TRProcessRequest>();

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        processHISList = _db.TRProcessRequests.Where(query).ToList();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        // error handling
                        trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                        _db.Dispose();
                    }
                }
            }

            Console.WriteLine($"GET_LIST_ProcesRequest : END");

            return processHISList;
        }

        public List<TRProcessRequestDetail> GET_LIST_ProcessRequestDetail(Func<TRProcessRequestDetail, bool> query)
        {
            Console.WriteLine($"GET_LIST_ProcessRequestDetail : START");

            var processHISList = new List<TRProcessRequestDetail>();

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        processHISList = _db.TRProcessRequestDetails.Where(query).ToList();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        // error handling
                        trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                        _db.Dispose();
                    }
                }
            }

            Console.WriteLine($"GET_LIST_ProcessRequestDetail : END");

            return processHISList;
        }

        private TRProcessRequestHISData INSERT_ProcessRequestHISData(string hos_code, int pcr_year)
        {
            Console.WriteLine($"CREATE_ProcessRequestHISData : START : {hos_code} : {pcr_year}");

            var processHISData = new TRProcessRequestHISData();

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        var hospital = _db.TRHospitals.FirstOrDefault(x => x.hos_code == hos_code);

                        if (hospital != null)
                        {
                            var hospitalLab = _db.TRHospitalLabs.FirstOrDefault(x => x.lab_hos_code == hos_code);

                            processHISData = new TRProcessRequestHISData()
                            {
                                pch_arh_code = hospital.hos_arh_code,
                                pch_prv_code = hospital.hos_prv_code,
                                pch_hos_code = hos_code,
                                pch_lab_code = hospitalLab.lab_code,
                                pch_year = pcr_year,
                                pch_active = true,
                                pch_status = "N",
                                pch_createuser = "BATCH",
                                pch_createdate = DateTime.Now
                            };
                            _db.TRProcessRequestHISDatas.Add(processHISData);

                            _db.SaveChanges();
                        }

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        // error handling
                        trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                        _db.Dispose();
                    }
                }
            }

            Console.WriteLine($"CREATE_ProcessRequestHISData : END");

            return processHISData;
        }

        private TRProcessRequest GET_DATA_ProcessRequest(Func<TRProcessRequest, bool> query)
        {
            Console.WriteLine($"GET_DATA_ProcessRequest : Start");

            var processRequest = new TRProcessRequest();

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        processRequest = _db.TRProcessRequests.FirstOrDefault(query);

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                        _db.Dispose();
                    }
                }
            }

            Console.WriteLine("GET_DATA_ProcessRequest : End");

            return processRequest;
        }

        private void CREATE_GLASS_FILE_BEFORE(TRProcessRequestHISData processHISData)
        {
            Console.WriteLine($"CREATE_FILE_BEFORE : START");

            for (var month = 1; month <= 12; month++)
            {
                var monthDataList = GET_DATA_BEFORE(processHISData, month);

                Console.WriteLine($"CREATE_FILE_BEFORE : {processHISData.pch_hos_code} : month {month} : year {processHISData.pch_year}: {monthDataList.Count}");

                if (monthDataList != null && monthDataList.Count > 0)
                {
                    processHISData.pch_status = "A";

                    CREATE_GLASS_FILE_BEFORE_MONTH(processHISData, month, monthDataList);

                    if (month == 1) processHISData.pch_M01_qty = monthDataList.Count;
                    else if (month == 2) processHISData.pch_M02_qty = monthDataList.Count;
                    else if (month == 3) processHISData.pch_M03_qty = monthDataList.Count;
                    else if (month == 4) processHISData.pch_M04_qty = monthDataList.Count;
                    else if (month == 5) processHISData.pch_M05_qty = monthDataList.Count;
                    else if (month == 6) processHISData.pch_M06_qty = monthDataList.Count;
                    else if (month == 7) processHISData.pch_M07_qty = monthDataList.Count;
                    else if (month == 8) processHISData.pch_M08_qty = monthDataList.Count;
                    else if (month == 9) processHISData.pch_M09_qty = monthDataList.Count;
                    else if (month == 10) processHISData.pch_M10_qty = monthDataList.Count;
                    else if (month == 11) processHISData.pch_M11_qty = monthDataList.Count;
                    else if (month == 12) processHISData.pch_M12_qty = monthDataList.Count;
                }
            }

            UPDATE_ProcessRequestHISData(processHISData);

            Console.WriteLine("CREATE_FILE_BEFORE : END");
        }

        private List<GLASS_File> GET_DATA_BEFORE(TRProcessRequestHISData processHISData, int month)
        {
            Console.WriteLine("GET_DATA_BEFORE : Start");

            var glassDataList = new List<GLASS_File>();
            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        var param1 = new SqlParameter("@hfu_id", DBNull.Value);
                        var param2 = new SqlParameter("@COUNTRY_A", "THA");
                        var param3 = new SqlParameter("@LABORATORY", "G20");
                        var param4 = new SqlParameter("@ORIGIN", "");
                        var param5 = new SqlParameter("@hos_code", processHISData.pch_hos_code);
                        var param6 = new SqlParameter("@lab_code", processHISData.pch_lab_code);
                        var param7 = new SqlParameter("@month", month);
                        var param8 = new SqlParameter("@year", processHISData.pch_year);
                        glassDataList = _db.GLASS_Files.FromSqlRaw<GLASS_File>($"sp_BATCH_PROC_Get_GLASS_File @hfu_id, @COUNTRY_A, @LABORATORY, @ORIGIN, @hos_code, @lab_code, @month, @year", param1, param2, param3, param4, param5, param6, param7, param8).ToList();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        var te = ex.Message;
                        // error handling
                        trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                        _db.Dispose();
                    }
                }
            }

            Console.WriteLine("GET_DATA_BEFORE : END");

            return glassDataList;
        }

        private void CREATE_GLASS_FILE_BEFORE_MONTH(TRProcessRequestHISData processHISData, int month, List<GLASS_File> glassDataList)
        {
            Console.WriteLine($"CREATE_FILE_BEFORE_MONTH : Start : {month}");

            var narstDataTable = List_To_DataTable<GLASS_File>(glassDataList);

            var dataList = new List<Dictionary<string, string>>();
            if (narstDataTable.Rows.Count > 0)
            {
                foreach (DataRow row in narstDataTable.Rows)
                {
                    var dataRow = new Dictionary<string, string>();
                    string[] columnNames = narstDataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).Where(x => x.Contains("_STR") == false && x.Contains("_cdate") == false).ToArray();
                    string[] columnNames_NE = narstDataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).Where(x => x.Contains("_NE")).ToArray();
                    foreach (var col in columnNames)
                    {
                        if (string.IsNullOrEmpty(row[col].ToString()))
                        {
                            dataRow.Add(col, null);
                        }
                        else
                        {
                            dataRow.Add(col, row[col].ToString());
                        }
                    }

                    dataList.Add(dataRow);
                }

                string dataFolderName = Path.Combine(whonetPath, "Data", Get_Data_FolderName(processHISData));
                P00_BATCH_Service.Create_Folder(dataFolderName);

                var dataFileName = Get_Data_Filename(processHISData, month);
                var dataFileFullName = Path.Combine(dataFolderName, dataFileName);
                P00_BATCH_Service.Delete_File_Exists(dataFileFullName);

                SQLiteDataAccess.CreateTable(dataFolderName, dataFileName, dataList.FirstOrDefault().Keys.ToList());

                SQLiteDataAccess.InsertData(dataFolderName, dataFileName, dataList);
            }

            Console.WriteLine("CREATE_FILE_BEFORE_MONTH : END");
        }

        private void UPDATE_ProcessRequestHISData(TRProcessRequestHISData processHISData)
        {
            Console.WriteLine($"UPDATE_ProcessRequestHISData : Start");

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        var processHISDataModel = _db.TRProcessRequestHISDatas.FirstOrDefault(x => x.pch_arh_code == processHISData.pch_arh_code && x.pch_prv_code == processHISData.pch_prv_code && x.pch_hos_code == processHISData.pch_hos_code && x.pch_lab_code == processHISData.pch_lab_code && x.pch_year == processHISData.pch_year);

                        processHISDataModel.pch_M01_qty = processHISData.pch_M01_qty;
                        processHISDataModel.pch_M02_qty = processHISData.pch_M02_qty;
                        processHISDataModel.pch_M03_qty = processHISData.pch_M03_qty;
                        processHISDataModel.pch_M04_qty = processHISData.pch_M04_qty;
                        processHISDataModel.pch_M05_qty = processHISData.pch_M05_qty;
                        processHISDataModel.pch_M06_qty = processHISData.pch_M06_qty;
                        processHISDataModel.pch_M07_qty = processHISData.pch_M07_qty;
                        processHISDataModel.pch_M08_qty = processHISData.pch_M08_qty;
                        processHISDataModel.pch_M09_qty = processHISData.pch_M09_qty;
                        processHISDataModel.pch_M10_qty = processHISData.pch_M10_qty;
                        processHISDataModel.pch_M11_qty = processHISData.pch_M11_qty;
                        processHISDataModel.pch_M12_qty = processHISData.pch_M12_qty;

                        processHISDataModel.pch_updateuser = "BATCH";
                        processHISDataModel.pch_updatedate = DateTime.Now;

                        _db.SaveChanges();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        Console.WriteLine("UPDATE_ProcessRequestHISData : " + ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                        _db.Dispose();
                    }
                }
            }

            Console.WriteLine($"UPDATE_ProcessRequestHISData : END");
        }

        private void UPDATE_ProcessRequest(TRProcessRequest processRequest)
        {
            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        Console.WriteLine($"UPDATE_ProcessRequest : {processRequest.pcr_code} : Inactive");

                        var update_processRequest = _db.TRProcessRequests.FirstOrDefault(x => x.pcr_code == processRequest.pcr_code);

                        update_processRequest.pcr_status = "I";
                        update_processRequest.pcr_active = false;

                        _db.SaveChanges();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        Console.WriteLine("Create_ProcessRequest : " + ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                        _db.Dispose();
                    }
                }
            }

            Console.WriteLine($"UPDATE_ProcessRequest : END");
        }

        private TRProcessRequest INSERT_ProcessRequest(TRProcessRequestHISData processHISData, string month_start, string month_end, string pcr_prev_code)
        {
            Console.WriteLine($"INSERT_ProcessRequest : Start");

            var new_processRequest = new TRProcessRequest();
            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        var pcr_code_new = "";

                        Console.WriteLine("Create_Process : " + pcr_code_pattern());

                        new_processRequest = new TRProcessRequest()
                        {
                            pcr_arh_code = processHISData.pch_arh_code,
                            pcr_prv_code = processHISData.pch_prv_code,
                            pcr_hos_code = processHISData.pch_hos_code,
                            pcr_lab_code = processHISData.pch_lab_code,
                            pcr_type = "02",
                            pcr_month_start = month_start,
                            pcr_month_end = month_end,
                            pcr_year = processHISData.pch_year.ToString(),
                            pcr_filetype = "application/zip",
                            pcr_status = "C",
                            pcr_active = true,
                            pcr_createuser = "BATCH",
                            pcr_createdate = DateTime.Now
                        };

                        var lastProcessRequestList = _db.TRProcessRequests.Where(x => x.pcr_code.Contains(pcr_code_pattern())).ToList();
                        if (lastProcessRequestList != null && lastProcessRequestList.Count > 0)
                        {
                            var lastProcessRequest = lastProcessRequestList.OrderByDescending(x => x.pcr_code).FirstOrDefault();
                            var pcr_code_last = lastProcessRequest.pcr_code;
                            var code_running_next = (Convert.ToInt32(pcr_code_last.Replace(pcr_code_pattern(), "")) + 1).ToString("0000");

                            pcr_code_new = pcr_code_pattern() + code_running_next;
                        }
                        else
                        {
                            pcr_code_new = pcr_code_pattern() + "0001";
                        }

                        Console.WriteLine("INSERT_ProcessRequest : CREATE NEW " + pcr_code_new);

                        new_processRequest.pcr_code = pcr_code_new;
                        new_processRequest.pcr_file_before = Get_Process_File_Before(new_processRequest);
                        new_processRequest.pcr_file_after = Get_Process_File_After(new_processRequest);
                        new_processRequest.pcr_filename = Get_Process_File_Result(new_processRequest);
                        new_processRequest.pcr_filepath = Get_Process_FolderName(new_processRequest);
                        new_processRequest.pcr_prev_code = pcr_prev_code;

                        _db.TRProcessRequests.Add(new_processRequest);

                        _db.SaveChanges();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        Console.WriteLine("INSERT_ProcessRequest : " + ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                        _db.Dispose();
                    }
                }
            }

            Console.WriteLine($"INSERT_ProcessRequest : END");

            return new_processRequest;
        }

        private void INSERT_ProcessRequestDetail(TRProcessRequestHISData processHISData, string pcr_code_new)
        {
            Console.WriteLine($"INSERT_ProcessRequestDetail : Start : {processHISData.pch_hos_code}_{processHISData.pch_lab_code}");

            var processRequestNew = new TRProcessRequest();
            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        var processRequestDetail = new TRProcessRequestDetail()
                        {
                            pcd_pcr_code = pcr_code_new,
                            pcd_hos_code = processHISData.pch_hos_code,
                            pcd_lab_code = processHISData.pch_lab_code,
                            pcd_M01_qty = processHISData.pch_M01_qty,
                            pcd_M02_qty = processHISData.pch_M02_qty,
                            pcd_M03_qty = processHISData.pch_M03_qty,
                            pcd_M04_qty = processHISData.pch_M04_qty,
                            pcd_M05_qty = processHISData.pch_M05_qty,
                            pcd_M06_qty = processHISData.pch_M06_qty,
                            pcd_M07_qty = processHISData.pch_M07_qty,
                            pcd_M08_qty = processHISData.pch_M08_qty,
                            pcd_M09_qty = processHISData.pch_M09_qty,
                            pcd_M10_qty = processHISData.pch_M10_qty,
                            pcd_M11_qty = processHISData.pch_M11_qty,
                            pcd_M12_qty = processHISData.pch_M12_qty,
                            pcd_status = "C",
                            pcd_active = true,
                            pcd_createuser = "BATCH",
                            pcd_createdate = DateTime.Now
                        };

                        _db.TRProcessRequestDetails.Add(processRequestDetail);

                        _db.SaveChanges();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        Console.WriteLine("INSERT_ProcessRequestDetail : " + ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                        _db.Dispose();
                    }
                }
            }

            Console.WriteLine($"INSERT_ProcessRequestDetail : END");
        }
        
        private void CREATE_GLASS_MACRO_FILE(TRProcessRequest processRequest, List<TRProcessRequestHISData> processHISList)
        {
            Console.WriteLine($"CREATE_MACRO_FILE : Start");

            string processFolder = Get_Process_FolderName(processRequest);

            string processMacroFolderName = Path.Combine(whonetPath, "Macros", processFolder);
            P00_BATCH_Service.Create_Folder_Empty(processMacroFolderName);

            string processOutputFolderName = Path.Combine(whonetPath, "Output", processFolder);
            P00_BATCH_Service.Create_Folder_Empty(processOutputFolderName);

            string templateMacroFolderName = Path.Combine(whonetPath, "MacroTemplates", whonetMacroPath);
            DirectoryInfo templateMacroFolderInfo = new DirectoryInfo(templateMacroFolderName);
            FileInfo[] templateMacroFiles = templateMacroFolderInfo.GetFiles("*.mcr");

            var data_File_List = Get_Data_File_List(processRequest, processHISList);

            foreach (var macroFile in templateMacroFiles)
            {
                string newMacroName = $"P_{processRequest.pcr_code}_{macroFile.Name.Replace(macroFile.Extension, "")}";
                string newMacroFullName = Path.Combine(processMacroFolderName, $"{newMacroName}{macroFile.Extension}");

                P00_BATCH_Service.Copy_File(macroFile.FullName, newMacroFullName);

                string[] message = File.ReadAllLines(newMacroFullName);

                File.WriteAllText(newMacroFullName, String.Empty);

                var sb = new StringBuilder();

                foreach (var line in message)
                {
                    if (line.Contains("{LabFile}"))
                    {
                        sb.AppendLine(line.Replace("{LabFile}", "LABWHO.GLS"));
                    }
                    else if (line.Contains("{DataFiles}"))
                    {
                        sb.AppendLine(line.Replace("{DataFiles}", $"Data file = " + data_File_List));
                    }
                    else if (line.Contains("{SQLiteFileFullPath}"))
                    {
                        sb.AppendLine(line.Replace("{SQLiteFileFullPath}", $"\"{processFolder}\\{processFolder}-{macroFile.Name.Replace(macroFile.Extension, ".db")}\""));
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }

                    File.AppendAllText(newMacroFullName, line, System.Text.Encoding.UTF8);
                }

                using (StreamWriter file = new StreamWriter(newMacroFullName))
                {
                    file.WriteLine(sb.ToString()); // "sb" is the StringBuilder
                }
            }

            string processDataFolderName = Path.Combine(whonetPath, "Data", processFolder);
            P00_BATCH_Service.Create_Folder_Empty(processDataFolderName);

            var data_File_Split = data_File_List.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var data_File in data_File_Split)
            {
                string processDataFileDesc = Path.Combine(processDataFolderName, data_File.Split('\\', StringSplitOptions.RemoveEmptyEntries)[1]);
                string processDataFile = Path.Combine(whonetPath, "Data", data_File);
                if (File.Exists(processDataFile))
                {
                    P00_BATCH_Service.Copy_File(processDataFile, processDataFileDesc);
                }
            }

            var zip_GLASS_Before = Get_Process_File_Before(processRequest);
            P00_BATCH_Service.Delete_File_Exists(zip_GLASS_Before);
            ZipFile.CreateFromDirectory(processDataFolderName, zip_GLASS_Before);

            P00_BATCH_Service.Delete_Folder(processDataFolderName);

            var backupPath_GLASS_Before = P00_BATCH_Service.GetConfigurationValue("WHONET:GLASS:PARAM:PATH_BACKUP_FILE_BEFORE");
            P00_BATCH_Service.Create_Folder(backupPath_GLASS_Before);
            if (string.IsNullOrEmpty(backupPath_GLASS_Before) == false)
            {
                FileInfo fileInfo = new FileInfo(zip_GLASS_Before);
                var backup_GLASS_Before = Path.Combine(backupPath_GLASS_Before, fileInfo.Name);
                P00_BATCH_Service.Copy_File(zip_GLASS_Before, backup_GLASS_Before);

                Console.WriteLine($"CREATE_MACRO_FILE : {backup_GLASS_Before}");
            }

            Console.WriteLine($"CREATE_MACRO_FILE : END");
        }

        private void GLASS_Run_WHONET(TRProcessRequest processRequest)
        {
            Console.WriteLine($"GLASS_Run_WHONET : {processRequest.pcr_code}");

            string processFolder = Get_Process_FolderName(processRequest);
            string macroFolderName = Path.Combine(whonetPath, "Macros", processFolder);

            ProcessStartInfo processInfo;
            System.Diagnostics.Process process;

            int exitCode;
            string output;
            string error;
            string command;

            string[] macroTemplateFiles = Directory.GetFiles(macroFolderName, "*.mcr");
            foreach (var macroFile in macroTemplateFiles)
            {
                FileInfo macroInfo = new FileInfo(macroFile);

                command = Path.Combine(whonetPath, "WHONET.EXE") + $" \"{processFolder}\\{macroInfo.Name}\"";
                Console.WriteLine(command);
                processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                // *** Redirect the output ***
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardOutput = true;

                process = System.Diagnostics.Process.Start(processInfo);
                process.WaitForExit();

                // *** Read the streams ***
                // Warning: This approach can lead to deadlocks, see Edit #2
                output = process.StandardOutput.ReadToEnd();
                error = process.StandardError.ReadToEnd();

                exitCode = process.ExitCode;

                Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
                Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
                Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
                process.Close();
            }

            Console.WriteLine($"GLASS_Run_WHONET : END");
        }

        private void GLASS_Insert_Result(TRProcessRequest processRequest)
        {
            Console.WriteLine($"GLASS_Insert_Result : {processRequest.pcr_code}");

            string processFolder = Get_Process_FolderName(processRequest);
            string outputFolderName = Path.Combine(whonetPath, "Output", processFolder);
            string outputFolderTXTName = Path.Combine(whonetPath, "Output", $"{processFolder}-TXT");
            P00_BATCH_Service.Create_Folder_Empty(outputFolderTXTName);

            DirectoryInfo directory = new DirectoryInfo(outputFolderName);
            FileInfo[] risInfos = directory.GetFiles("*RIS*");

            StringBuilder stringRIS = new StringBuilder();
            stringRIS.Append("COUNTRY");
            stringRIS.Append("\t");
            stringRIS.Append("YEAR");
            stringRIS.Append("\t");
            stringRIS.Append("SPECIMEN");
            stringRIS.Append("\t");
            stringRIS.Append("PATHOGEN");
            stringRIS.Append("\t");
            stringRIS.Append("GENDER");
            stringRIS.Append("\t");
            stringRIS.Append("ORIGIN");
            stringRIS.Append("\t");
            stringRIS.Append("AGEGROUP");
            stringRIS.Append("\t");
            stringRIS.Append("ANTIBIOTIC");
            stringRIS.Append("\t");
            stringRIS.Append("RESISTANT");
            stringRIS.Append("\t");
            stringRIS.Append("INTERMEDIATE");
            stringRIS.Append("\t");
            stringRIS.Append("NONSUSCEPTIBLE");
            stringRIS.Append("\t");
            stringRIS.Append("SUSCEPTIBLE");
            stringRIS.Append("\t");
            stringRIS.Append("UNKNOWN_NO_AST");
            stringRIS.Append("\t");
            stringRIS.Append("UNKNOWN_NO_BREAKPOINTS");
            stringRIS.Append("\t");
            stringRIS.Append("BATCHID");

            foreach (FileInfo file in risInfos)
            {
                Console.WriteLine("GLASS_Insert_Result : " + file.Name);

                List<TRProcessGLASS_RIS> processGLASSList = new List<TRProcessGLASS_RIS>();
                var detailCount = 1;

                string[] filenameSplit = file.Name.Split('-');
                string specimen = filenameSplit[filenameSplit.Length - 1].Replace(".db", "").ToUpper();
                //string pathogen = file.Name.Replace($"{filenameSplit[0]}-{filenameSplit[1]}-{filenameSplit[2]}-{filenameSplit[3]}-{filenameSplit[4]}-{filenameSplit[5]}-", "").Replace(".db", "");

                var data = SQLiteDataAccess.LoadOutput_RIS(file.DirectoryName, file.Name, detailCount.ToString());

                detailCount++;
                foreach (var outputItem in data)
                {
                    processGLASSList.Add(new TRProcessGLASS_RIS()
                    {
                        pcg_pcr_code = processRequest.pcr_code,
                        pcg_country = "THA",
                        pcg_year = processRequest.pcr_year,
                        pcg_specimen = specimen,
                        pcg_pathogen = "TEST",
                        pcg_gender = outputItem.CODE1.ToUpper(),
                        pcg_origin = outputItem.CODE3,
                        pcg_agegroup = outputItem.CODE2,
                        pcg_antibiotic = outputItem.DRUG_CODE,
                        pcg_resistant = outputItem.NUM_RES.ToString(),
                        pcg_intermediate = outputItem.NUM_INT.ToString(),
                        pcg_nonsusceptible = "0",
                        pcg_susceptible = outputItem.NUM_SUSC.ToString(),
                        pcg_unknown_no_ast = (outputItem.NUM_TESTED - outputItem.NUM_RES - outputItem.NUM_INT - outputItem.NUM_INT).ToString(),
                        pcg_unknown_no_breakpoints = "0",
                        pcg_batchid = "DS1"
                    });
                    stringRIS.AppendLine();
                    stringRIS.Append("THA");
                    stringRIS.Append("\t");
                    stringRIS.Append(processRequest.pcr_year);
                    stringRIS.Append("\t");
                    stringRIS.Append(specimen);
                    stringRIS.Append("\t");
                    stringRIS.Append("TEST");
                    stringRIS.Append("\t");
                    stringRIS.Append(outputItem.CODE1.ToUpper());
                    stringRIS.Append("\t");
                    stringRIS.Append(outputItem.CODE3);
                    stringRIS.Append("\t");
                    stringRIS.Append(outputItem.CODE2);
                    stringRIS.Append("\t");
                    stringRIS.Append(outputItem.DRUG_CODE);
                    stringRIS.Append("\t");
                    stringRIS.Append(outputItem.NUM_RES.ToString());
                    stringRIS.Append("\t");
                    stringRIS.Append(outputItem.NUM_INT.ToString());
                    stringRIS.Append("\t");
                    stringRIS.Append("0");
                    stringRIS.Append("\t");
                    stringRIS.Append(outputItem.NUM_SUSC.ToString());
                    stringRIS.Append("\t");
                    stringRIS.Append((outputItem.NUM_TESTED - outputItem.NUM_RES - outputItem.NUM_INT - outputItem.NUM_INT).ToString());
                    stringRIS.Append("\t");
                    stringRIS.Append("0");
                    stringRIS.Append("\t");
                    stringRIS.Append("DS1");
                }

                using (var _db = new ProcessContext())
                {
                    using (var trans = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            _db.BulkInsert(processGLASSList);

                            foreach(var item in processGLASSList)
                            {
                                _db.TRProcessGLASS_RISs.Add(item);
                            }

                            _db.SaveChanges();

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            Console.WriteLine("GLASS_Insert_Result BulkInsert : Error " + ex.Message);
                        }
                        finally
                        {
                            trans.Dispose();
                            _db.Dispose();

                            GC.Collect();
                            GC.WaitForPendingFinalizers();

                            Console.WriteLine("GLASS_Insert_Result BulkInsert : Success");
                        }
                    }
                }
            }

            FileInfo[] samplesInfos = directory.GetFiles("*Samples*");

            StringBuilder stringSample = new StringBuilder();
            stringSample.Append("COUNTRY");
            stringSample.Append("\t");
            stringSample.Append("YEAR");
            stringSample.Append("\t");
            stringSample.Append("SPECIMEN");
            stringSample.Append("\t");
            stringSample.Append("GENDER");
            stringSample.Append("\t");
            stringSample.Append("ORIGIN");
            stringSample.Append("\t");
            stringSample.Append("AGEGROUP");
            stringSample.Append("\t");
            stringSample.Append("NUMSAMPLEDPATIENTS");
            stringSample.Append("\t");
            stringSample.Append("BATCHID");
            stringSample.AppendLine();

            foreach (FileInfo file in samplesInfos)
            {
                Console.WriteLine("GLASS_Insert_Result : " + file.Name);

                List<TRProcessGLASS_Sample> processGLASSList = new List<TRProcessGLASS_Sample>();
                var detailCount = 1;

                string[] filenameSplit = file.Name.Split('-');
                string specimen = filenameSplit[filenameSplit.Length - 1].Replace(".db", "").ToUpper();
                //string pathogen = file.Name.Replace($"{filenameSplit[0]}-{filenameSplit[1]}-{filenameSplit[2]}-{filenameSplit[3]}-{filenameSplit[4]}-{filenameSplit[5]}-", "").Replace(".db", "");

                var data = SQLiteDataAccess.LoadOutput_Sample(file.DirectoryName, file.Name, detailCount.ToString());

                detailCount++;
                foreach (var outputItem in data)
                {
                    processGLASSList.Add(new TRProcessGLASS_Sample()
                    {
                        pcg_pcr_code = processRequest.pcr_code,
                        pcg_country = "THA",
                        pcg_year = processRequest.pcr_year,
                        pcg_specimen = specimen,
                        pcg_gender = outputItem.CODE1.ToUpper(),
                        pcg_origin = outputItem.CODE3,
                        pcg_agegroup = outputItem.CODE2,
                        pcg_numsampledpatients = outputItem.NUMPATIENT.ToString(),
                        pcg_batchid = "DS1"
                    });

                    stringSample.AppendLine();
                    stringSample.Append("THA");
                    stringSample.Append("\t");
                    stringSample.Append(processRequest.pcr_year);
                    stringSample.Append("\t");
                    stringSample.Append(specimen);
                    stringSample.Append("\t");
                    stringSample.Append(outputItem.CODE1.ToUpper());
                    stringSample.Append("\t");
                    stringSample.Append(outputItem.CODE3);
                    stringSample.Append("\t");
                    stringSample.Append(outputItem.CODE2);
                    stringSample.Append("\t");
                    stringSample.Append(outputItem.NUMPATIENT.ToString());
                    stringSample.Append("\t");
                    stringSample.Append("DS1");
                }

                using (var _db = new ProcessContext())
                {
                    using (var trans = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            _db.BulkInsert(processGLASSList);

                            _db.SaveChanges();

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            Console.WriteLine("GLASS_Insert_Result BulkInsert : Error " + ex.Message);
                        }
                        finally
                        {
                            trans.Dispose();
                            _db.Dispose();

                            GC.Collect();
                            GC.WaitForPendingFinalizers();

                            Console.WriteLine("GLASS_Insert_Result BulkInsert : Success");
                        }
                    }
                }
            }

            var zip_GLASS_After = Get_Process_File_After(processRequest);
            P00_BATCH_Service.Delete_File_Exists(zip_GLASS_After);
            ZipFile.CreateFromDirectory(outputFolderName, zip_GLASS_After);

            P00_BATCH_Service.Delete_Folder(outputFolderName);

            var backupPath_GLASS_After = P00_BATCH_Service.GetConfigurationValue("WHONET:GLASS:PARAM:PATH_BACKUP_FILE_AFTER");
            P00_BATCH_Service.Create_Folder(backupPath_GLASS_After);
            if (string.IsNullOrEmpty(backupPath_GLASS_After) == false)
            {
                FileInfo fileInfo = new FileInfo(zip_GLASS_After);
                var backup_GLASS_After = Path.Combine(backupPath_GLASS_After, fileInfo.Name);
                P00_BATCH_Service.Copy_File(zip_GLASS_After, backup_GLASS_After);

                Console.WriteLine($"GLASS_Insert_Result : PATH_BACKUP_FILE_AFTER : {backup_GLASS_After}");
            }

            //Create file {outputFolderTXTName}\\{processFolder}-GLASS-RIS.txt
            var risFilename = $"{outputFolderTXTName}\\{processFolder}-GLASS-RIS.txt";

            using (StreamWriter file = new StreamWriter(risFilename))
            {
                file.WriteLine(stringRIS.ToString()); // "sb" is the StringBuilder
            }

            //Create file {outputFolderTXTName}\\{processFolder}-GLASS-Samples.txt
            var sampleFilename = Path.Combine(outputFolderTXTName, $"{processFolder}-GLASS-Samples.txt");

            using (StreamWriter file = new StreamWriter(sampleFilename))
            {
                file.WriteLine(stringSample.ToString()); // "sb" is the StringBuilder
            }

            var zip_GLASS_Result = Get_Process_File_Result(processRequest);
            P00_BATCH_Service.Delete_File_Exists(zip_GLASS_Result);
            ZipFile.CreateFromDirectory(outputFolderTXTName, zip_GLASS_Result);

            P00_BATCH_Service.Delete_Folder(outputFolderTXTName);

            var backupPath_GLASS_Result = P00_BATCH_Service.GetConfigurationValue("WHONET:GLASS:PARAM:PATH_BACKUP_FILE_RESULT");
            P00_BATCH_Service.Create_Folder(backupPath_GLASS_Result);
            if (string.IsNullOrEmpty(backupPath_GLASS_Result) == false)
            {
                FileInfo fileInfo = new FileInfo(zip_GLASS_Result);
                var backup_NARST_Result = Path.Combine(backupPath_GLASS_Result, fileInfo.Name);
                P00_BATCH_Service.Copy_File(zip_GLASS_Result, backup_NARST_Result);

                Console.WriteLine($"GENERATE_EXCEL_FILE : {backup_NARST_Result}");
            }

            Console.WriteLine($"GLASS_Insert_Result : END");
        }

        public List<TRProcessRequestHISData> GET_LIST_ProcessRequestHISData(Func<TRProcessRequestHISData, bool> query)
        {
            Console.WriteLine($"GET_LIST_ProcessRequestHISData : START");

            var processHISList = new List<TRProcessRequestHISData>();

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        processHISList = _db.TRProcessRequestHISDatas.Where(query).ToList();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        // error handling
                        trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                        _db.Dispose();
                    }
                }
            }

            Console.WriteLine($"GET_LIST_ProcessRequestHISData : END");

            return processHISList;
        }

        private DataTable List_To_DataTable<T>(List<T> data)
        {
            //Logger.Info(string.Format(this.GetType().Name + " : " + MethodBase.GetCurrentMethod().Name + " : Start :"));
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();

            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }

                dataTable.Rows.Add(values);
            }
            //Logger.Info(string.Format(this.GetType().Name + " : " + MethodBase.GetCurrentMethod().Name + " : Finish :"));
            return dataTable;
        }
    }
}
