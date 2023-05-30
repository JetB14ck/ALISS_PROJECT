using ALISS.Process.Batch.DataAccess;
using ALISS.Process.Batch.Model;
using ALISS.Process.Batch.SQLite;
using EFCore.BulkExtensions;
//using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Process.Batch
{
    public class P02_WHONET_NARST_Service : IDisposable
    {
        private string whonetPath;
        private string whonetMacroPath;
        private static int process_year;
        private TCMasterTemplate masterTemplate;

        public void Dispose()
        {

        }

        private string Get_Data_FolderName(TRProcessRequestLabData processLabData)
        {
            return $"P_{(processLabData.pcl_arh_code ?? "00")}_{(processLabData.pcl_prv_code ?? "00")}_{(processLabData.pcl_hos_code ?? "000000000")}_{(processLabData.pcl_lab_code ?? "000")}_{processLabData.pcl_year}";
        }

        private string Get_Data_Filename(TRProcessRequestLabData processLabData, int month)
        {
            return $"P_{(processLabData.pcl_arh_code ?? "00")}_{(processLabData.pcl_prv_code ?? "00")}_{(processLabData.pcl_hos_code ?? "000000000")}_{(processLabData.pcl_lab_code ?? "000")}_{processLabData.pcl_year}_{month.ToString("00")}.sqlite";
        }

        private string Get_Process_FolderName(TRProcessRequest processRequest)
        {
            return $"P_{processRequest.pcr_code}_{(processRequest.pcr_arh_code ?? "00")}_{(processRequest.pcr_prv_code ?? "00")}_{(processRequest.pcr_hos_code ?? "000000000")}_{(processRequest.pcr_lab_code ?? "000")}_{processRequest.pcr_year}_{processRequest.pcr_month_start}_{processRequest.pcr_month_end}";
        }

        private string Get_Process_File_Before(TRProcessRequest processRequest)
        {
            return $"{Get_Process_FolderName(processRequest)}-NARST-Before.zip";
        }

        private string Get_Process_File_After(TRProcessRequest processRequest)
        {
            return $"{Get_Process_FolderName(processRequest)}-NARST-After.zip";
        }

        private string Get_Process_File_Result(TRProcessRequest processRequest)
        {
            return $"{Get_Process_FolderName(processRequest)}-NARST-Result.xlsx";
        }

        private string Get_Process_FileZip_Result(TRProcessRequest processRequest)
        {
            return $"{Get_Process_FolderName(processRequest)}-NARST-Result.zip";
        }

        private string Get_Data_File_List(TRProcessRequest processRequest, List<TRProcessRequestLabData> processLabList)
        {
            string dataFilename = "";
            var str_list = new List<string>();

            foreach (var processLabData in processLabList)
            {
                var month_end = 0;
                if (processRequest.pcr_month_end == "03") month_end = 3;
                else if (processRequest.pcr_month_end == "06") month_end = 6;
                else if (processRequest.pcr_month_end == "09") month_end = 9;
                else if (processRequest.pcr_month_end == "12") month_end = 12;

                for (var i = 1; i <= month_end; i++)
                {
                    str_list.Add($"{Get_Data_FolderName(processLabData)}\\{Get_Data_Filename(processLabData, i)}");
                }
            }

            dataFilename = string.Join(",", str_list);

            return dataFilename;
        }

        private static string pcr_code_pattern()
        {
            return $"PCR_{DateTime.Now.ToString("yyyyMMdd")}";
        }

        public P02_WHONET_NARST_Service()
        {
            whonetPath = P00_BATCH_Service.GetConfigurationValue("WHONET:PARAM:PATH");
            process_year = Convert.ToInt32(P00_BATCH_Service.GetConfigurationValue("WHONET:PARAM:PROCESS_YEAR"));
            whonetMacroPath = P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:PARAM:MACRO_TEMPLATE_FOLDER");
            GET_MASTERTEMPLATE_ACTIVE();
        }

        #region WHONET:NARST:FILE_BEFORE:RUN_SERVICE
        public void FILE_BEFORE_SERVICE_AUTO()
        {
            Console.WriteLine($"FILE_BEFORE_SERVICE_AUTO : START");

            var processLabList = GET_LIST_ProcessRequestLabData(x => x.pcl_status == "W");
            foreach (var processLabData in processLabList)
            {
                Console.WriteLine($"FILE_BEFORE_SERVICE : START : {processLabData.pcl_hos_code}");

                FILE_BEFORE_SERVICE(processLabData);
            }

            Console.WriteLine($"FILE_BEFORE_SERVICE_AUTO : END");
        }

        public void FILE_BEFORE_SERVICE_MANUAL(string hos_list)
        {
            Console.WriteLine($"FILE_BEFORE_SERVICE_MANUAL : START");

            var hos_split = hos_list.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var hos_code in hos_split)
            {
                Console.WriteLine($"FILE_BEFORE_SERVICE : START : {hos_code}");

                var processLabList = GET_LIST_ProcessRequestLabData(x => x.pcl_hos_code == hos_code && x.pcl_year == process_year);
                var processLabData = new TRProcessRequestLabData();

                if (processLabList != null && processLabList.Count > 0)
                {
                    processLabData = processLabList.FirstOrDefault();
                }
                else
                {
                    processLabData = INSERT_ProcessRequestLabData(hos_code);
                }

                FILE_BEFORE_SERVICE(processLabData);
            }

            Console.WriteLine($"FILE_BEFORE_SERVICE_MANUAL : END");
        }

        private void FILE_BEFORE_SERVICE(TRProcessRequestLabData processLabData)
        {
            CREATE_NARST_FILE_BEFORE(processLabData);

            Console.WriteLine($"FILE_BEFORE_SERVICE : END");
        }
        #endregion

        #region WHONET:NARST:HOS:RUN_SERVICE
        public void HOS_SERVICE_AUTO()
        {
            Console.WriteLine($"HOS_SERVICE_AUTO : START");

            var processLabList = GET_LIST_ProcessRequestLabData(x => (x.pcl_HQ01 == "W" || x.pcl_HQ02 == "W" || x.pcl_HQ03 == "W" || x.pcl_HQ04 == "W") && (x.pcl_year == process_year));
            foreach (var hos_code in processLabList.Select(x => x.pcl_hos_code).Distinct())
            {
                Console.WriteLine($"HOS_SERVICE : START : {hos_code}");

                var hos_processLabList = processLabList.Where(x => x.pcl_hos_code == hos_code).ToList();

                HOS_SERVICE(hos_processLabList);
            }

            Console.WriteLine("HOS_SERVICE_AUTO : END");
        }

        public void HOS_SERVICE_MANUAL(string hos_list)
        {
            Console.WriteLine($"HOS_SERVICE_MANUAL : START");

            var hos_split = hos_list.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var hos_code in hos_split)
            {
                Console.WriteLine($"HOS_SERVICE : START : {hos_code}");

                var processLabList = GET_LIST_ProcessRequestLabData(x => x.pcl_hos_code == hos_code && x.pcl_year == process_year);

                if (processLabList != null && processLabList.Count > 0)
                {
                    HOS_SERVICE(processLabList);
                }
            }

            Console.WriteLine("HOS_SERVICE_MANUAL : END");
        }

        private void HOS_SERVICE(List<TRProcessRequestLabData> processLabList)
        {
            var processLabData = processLabList.FirstOrDefault();

            Console.WriteLine($"HOS_SERVICE : START : {processLabData.pcl_hos_code}");

            var month_start = "01";
            var month_end = "";

            if (processLabData.pcl_HQ04 == "W") month_end = "12";
            else if (processLabData.pcl_HQ03 == "W") month_end = "09";
            else if (processLabData.pcl_HQ02 == "W") month_end = "06";
            else if (processLabData.pcl_HQ01 == "W") month_end = "03";

            if (string.IsNullOrEmpty(month_end) == false) NARST_SERVICE(processLabList, processLabData, month_start, month_end);

            Console.WriteLine("HOS_SERVICE : END");
        }
        #endregion

        #region WHONET:NARST:PRV:RUN_SERVICE
        public void PRV_SERVICE_AUTO()
        {
            Console.WriteLine($"PRV_SERVICE_AUTO : START");

            var processLabList = GET_LIST_ProcessRequestLabData(x => (x.pcl_PQ01 == "W" || x.pcl_PQ02 == "W" || x.pcl_PQ03 == "W" || x.pcl_PQ04 == "W") && (x.pcl_year == process_year));
            foreach (var prv_code in processLabList.Select(x => x.pcl_prv_code).Distinct())
            {
                Console.WriteLine($"PRV_SERVICE : START : {prv_code}");

                var prv_processLabList = processLabList.Where(x => x.pcl_prv_code == prv_code).ToList();

                PRV_SERVICE(prv_processLabList);
            }

            Console.WriteLine("PRV_SERVICE_AUTO : END");
        }

        public void PRV_SERVICE_MANUAL(string prv_list)
        {
            Console.WriteLine($"PRV_SERVICE_MANUAL : START");

            var prv_split = prv_list.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var prv_code in prv_split)
            {
                Console.WriteLine($"PRV_SERVICE : START : {prv_code}");

                var processLabList = GET_LIST_ProcessRequestLabData(x => x.pcl_prv_code == prv_code && x.pcl_year == process_year);

                if (processLabList != null && processLabList.Count > 0)
                {
                    PRV_SERVICE(processLabList);
                }
            }

            Console.WriteLine("PRV_SERVICE_MANUAL : END");
        }

        private void PRV_SERVICE(List<TRProcessRequestLabData> processLabList)
        {
            Console.WriteLine($"PRV_SERVICE : START");

            var firstLabData = processLabList.FirstOrDefault();
            var processLabData = new TRProcessRequestLabData()
            {
                pcl_arh_code = firstLabData.pcl_arh_code,
                pcl_prv_code = firstLabData.pcl_prv_code,
                pcl_hos_code = null,
                pcl_lab_code = null,
                pcl_year = firstLabData.pcl_year,
            };

            var month_start = "01";
            var month_end = "";

            if (processLabList.Any(x => x.pcl_PQ04 == "W")) month_end = "12";
            else if (processLabList.Any(x => x.pcl_PQ03 == "W")) month_end = "09";
            else if (processLabList.Any(x => x.pcl_PQ02 == "W")) month_end = "06";
            else if (processLabList.Any(x => x.pcl_PQ01 == "W")) month_end = "03";

            if (string.IsNullOrEmpty(month_end) == false) NARST_SERVICE(processLabList, processLabData, month_start, month_end);

            Console.WriteLine("PRV_SERVICE : END");
        }
        #endregion

        #region WHONET:NARST:ARH:RUN_SERVICE
        public void ARH_SERVICE_AUTO()
        {
            Console.WriteLine($"ARH_SERVICE_AUTO : START");

            var processLabList = GET_LIST_ProcessRequestLabData(x => (x.pcl_AQ01 == "W" || x.pcl_AQ02 == "W" || x.pcl_AQ03 == "W" || x.pcl_AQ04 == "W") && (x.pcl_year == process_year));
            foreach (var arh_code in processLabList.Select(x => x.pcl_arh_code).Distinct())
            {
                Console.WriteLine($"ARH_SERVICE : START : {arh_code}");

                var arh_processLabList = processLabList.Where(x => x.pcl_arh_code == arh_code).ToList();

                ARH_SERVICE(arh_processLabList);
            }

            Console.WriteLine("ARH_SERVICE_AUTO : END");
        }

        public void ARH_SERVICE_MANUAL(string arh_list)
        {
            Console.WriteLine($"ARH_SERVICE_MANUAL : START");

            var arh_split = arh_list.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var arh_code in arh_split)
            {
                Console.WriteLine($"ARH_SERVICE : START : {arh_code}");

                var processLabList = GET_LIST_ProcessRequestLabData(x => x.pcl_arh_code == arh_code && x.pcl_year == process_year);

                if (processLabList != null && processLabList.Count > 0)
                {
                    ARH_SERVICE(processLabList);
                }
            }

            Console.WriteLine("ARH_SERVICE_MANUAL : END");
        }

        private void ARH_SERVICE(List<TRProcessRequestLabData> processLabList)
        {
            Console.WriteLine($"ARH_SERVICE : START");

            var firstLabData = processLabList.FirstOrDefault();
            var processLabData = new TRProcessRequestLabData()
            {
                pcl_arh_code = firstLabData.pcl_arh_code,
                pcl_prv_code = null,
                pcl_hos_code = null,
                pcl_lab_code = null,
                pcl_year = firstLabData.pcl_year,
            };

            var month_start = "01";
            var month_end = "";

            if (processLabList.Any(x => x.pcl_AQ04 == "W")) month_end = "12";
            else if (processLabList.Any(x => x.pcl_AQ03 == "W")) month_end = "09";
            else if (processLabList.Any(x => x.pcl_AQ02 == "W")) month_end = "06";
            else if (processLabList.Any(x => x.pcl_AQ01 == "W")) month_end = "03";

            if (string.IsNullOrEmpty(month_end) == false) NARST_SERVICE(processLabList, processLabData, month_start, month_end);

            Console.WriteLine("ARH_SERVICE : END");
        }
        #endregion

        #region WHONET:NARST:NATION:RUN_SERVICE
        public void NATION_SERVICE()
        {
            Console.WriteLine($"NATION_SERVICE : START");

            var processLabList = GET_LIST_ProcessRequestLabData(x => (x.pcl_NQ01 == "W" || x.pcl_NQ02 == "W" || x.pcl_NQ03 == "W" || x.pcl_NQ04 == "W") && (x.pcl_year == process_year));

            var firstLabData = processLabList.FirstOrDefault();
            var processLabData = new TRProcessRequestLabData()
            {
                pcl_arh_code = null,
                pcl_prv_code = null,
                pcl_hos_code = null,
                pcl_lab_code = null,
                pcl_year = firstLabData.pcl_year,
            };

            var month_start = "01";
            var month_end = "";

            if (processLabList.Any(x => x.pcl_NQ04 == "W")) month_end = "12";
            else if (processLabList.Any(x => x.pcl_NQ03 == "W")) month_end = "09";
            else if (processLabList.Any(x => x.pcl_NQ02 == "W")) month_end = "06";
            else if (processLabList.Any(x => x.pcl_NQ01 == "W")) month_end = "03";

            //if (processLabList.Any(x => x.pcl_NQ04 == "W")) month_end = "12";

            if (string.IsNullOrEmpty(month_end) == false) NARST_SERVICE(processLabList, processLabData, month_start, month_end);

            Console.WriteLine("NATION_SERVICE : END");
        }
        #endregion

        #region WHONET:NARST:NARST_SERVICE
        private void NARST_SERVICE(List<TRProcessRequestLabData> processLabList, TRProcessRequestLabData processLabData, string month_start, string month_end)
        {
            Console.WriteLine($"NARST_SERVICE : START");

            string pcr_prev_code = null;

            var prev_processRequest = GET_DATA_ProcessRequest(x => x.pcr_arh_code == processLabData.pcl_arh_code && x.pcr_prv_code == processLabData.pcl_prv_code && x.pcr_hos_code == processLabData.pcl_hos_code && x.pcr_lab_code == processLabData.pcl_lab_code && x.pcr_year == processLabData.pcl_year.ToString() && x.pcr_month_start == month_start && x.pcr_month_end == month_end);

            if (prev_processRequest != null)
            {
                pcr_prev_code = prev_processRequest.pcr_prev_code;
                UPDATE_ProcessRequest(prev_processRequest);
            }

            var processRequest = INSERT_ProcessRequest(processLabData, month_start, month_end, pcr_prev_code);

            pcr_prev_code = processRequest.pcr_code;

            foreach (var processLab in processLabList)
            {
                INSERT_ProcessRequestDetail(processLab, pcr_prev_code);
            }

            CREATE_NARST_MACRO_FILE(processRequest, processLabList);

            RUN_NARST_COMMAND(processRequest);

            INSERT_ProcessDataResult(processRequest);

            GENERATE_NARST_EXCEL_FILE(processRequest);

            Console.WriteLine("NARST_SERVICE : END");
        }
        #endregion

        #region WHONET:NARST:INSERT_RESULT_SERVICE
        public void INSERT_RESULT_SERVICE(string prc_list)
        {
            Console.WriteLine($"INSERT_RESULT_SERVICE : START");

            var prc_split = prc_list.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pcr_code in prc_split)
            {
                Console.WriteLine($"INSERT_RESULT_SERVICE : START : {pcr_code}");

                var processRequest = GET_DATA_ProcessRequest(x => x.pcr_code == pcr_code);

                INSERT_ProcessDataResult(processRequest);

                GENERATE_NARST_EXCEL_FILE(processRequest);
            }

            Console.WriteLine("INSERT_RESULT_SERVICE : END");
        }
        #endregion

        #region WHONET:NARST:GENERATE_EXCEL_SERVICE
        public void GENERATE_EXCEL_SERVICE(string prc_list)
        {
            Console.WriteLine($"GENERATE_EXCEL_SERVICE : START");

            var prc_split = prc_list.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pcr_code in prc_split)
            {
                Console.WriteLine($"GENERATE_EXCEL_SERVICE : START : {pcr_code}");

                var processRequest = GET_DATA_ProcessRequest(x => x.pcr_code == pcr_code);

                //INSERT_ProcessDataResult(processRequest);

                GENERATE_NARST_EXCEL_FILE(processRequest);
            }

            Console.WriteLine("GENERATE_EXCEL_SERVICE : END");
        }
        #endregion

        public List<TRProcessRequestLabData> GET_LIST_ProcessRequestLabData(Func<TRProcessRequestLabData, bool> query)
        {
            Console.WriteLine($"GET_LIST_ProcessRequestLabData : START");

            var processLabList = new List<TRProcessRequestLabData>();

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        processLabList = _db.TRProcessRequestLabDatas.Where(query).ToList();

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

            Console.WriteLine($"GET_LIST_ProcessRequestLabData : END");

            return processLabList;
        }

        private TRProcessRequestLabData INSERT_ProcessRequestLabData(string hos_code)
        {
            Console.WriteLine($"CREATE_ProcessRequestLabData : START : {hos_code}");

            var processLabData = new TRProcessRequestLabData();

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

                            processLabData = new TRProcessRequestLabData()
                            {
                                pcl_arh_code = hospital.hos_arh_code,
                                pcl_prv_code = hospital.hos_prv_code,
                                pcl_hos_code = hos_code,
                                pcl_lab_code = hospitalLab.lab_code,
                                pcl_year = process_year,
                                pcl_active = true,
                                pcl_status = "N",
                                pcl_createuser = "BATCH",
                                pcl_createdate = DateTime.Now
                            };
                            _db.TRProcessRequestLabDatas.Add(processLabData);

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

            Console.WriteLine($"CREATE_ProcessRequestLabData : END");

            return processLabData;
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

        private void CREATE_NARST_FILE_BEFORE(TRProcessRequestLabData processLabData)
        {
            Console.WriteLine($"CREATE_FILE_BEFORE : START");

            for (var month = 1; month <= 12; month++)
            {
                var monthDataList = GET_DATA_BEFORE(processLabData, month);

                Console.WriteLine($"CREATE_FILE_BEFORE : {processLabData.pcl_hos_code} : month {month} : year {processLabData.pcl_year}: {monthDataList.Count}");

               if (monthDataList != null && monthDataList.Count > 0)
                {
                    processLabData.pcl_status = "A";

                    CREATE_NARST_FILE_BEFORE_MONTH(processLabData, month, monthDataList);

                    if (month == 1) processLabData.pcl_M01_qty = monthDataList.Count;
                    else if (month == 2) processLabData.pcl_M02_qty = monthDataList.Count;
                    else if (month == 3) processLabData.pcl_M03_qty = monthDataList.Count;
                    else if (month == 4) processLabData.pcl_M04_qty = monthDataList.Count;
                    else if (month == 5) processLabData.pcl_M05_qty = monthDataList.Count;
                    else if (month == 6) processLabData.pcl_M06_qty = monthDataList.Count;
                    else if (month == 7) processLabData.pcl_M07_qty = monthDataList.Count;
                    else if (month == 8) processLabData.pcl_M08_qty = monthDataList.Count;
                    else if (month == 9) processLabData.pcl_M09_qty = monthDataList.Count;
                    else if (month == 10) processLabData.pcl_M10_qty = monthDataList.Count;
                    else if (month == 11) processLabData.pcl_M11_qty = monthDataList.Count;
                    else if (month == 12) processLabData.pcl_M12_qty = monthDataList.Count;

                    if (month == 1 || month == 2 || month == 3)
                    {
                        processLabData.pcl_HQ01 = "W";
                        processLabData.pcl_PQ01 = "W";
                        processLabData.pcl_AQ01 = "W";
                        processLabData.pcl_NQ01 = "W";
                    }
                    else if (month == 4 || month == 5 || month == 6)
                    {
                        processLabData.pcl_HQ01 = "A";
                        processLabData.pcl_PQ01 = "A";
                        processLabData.pcl_AQ01 = "A";
                        processLabData.pcl_NQ01 = "A";
                        processLabData.pcl_HQ02 = "W";
                        processLabData.pcl_PQ02 = "W";
                        processLabData.pcl_AQ02 = "W";
                        processLabData.pcl_NQ02 = "W";
                    }
                    else if (month == 7 || month == 8 || month == 9)
                    {
                        processLabData.pcl_HQ01 = "A";
                        processLabData.pcl_PQ01 = "A";
                        processLabData.pcl_AQ01 = "A";
                        processLabData.pcl_NQ01 = "A";
                        processLabData.pcl_HQ02 = "A";
                        processLabData.pcl_PQ02 = "A";
                        processLabData.pcl_AQ02 = "A";
                        processLabData.pcl_NQ02 = "A";
                        processLabData.pcl_HQ03 = "W";
                        processLabData.pcl_PQ03 = "W";
                        processLabData.pcl_AQ03 = "W";
                        processLabData.pcl_NQ03 = "W";
                    }
                    else if (month == 10 || month == 11 || month == 12)
                    {
                        processLabData.pcl_HQ01 = "A";
                        processLabData.pcl_PQ01 = "A";
                        processLabData.pcl_AQ01 = "A";
                        processLabData.pcl_NQ01 = "A";
                        processLabData.pcl_HQ02 = "A";
                        processLabData.pcl_PQ02 = "A";
                        processLabData.pcl_AQ02 = "A";
                        processLabData.pcl_NQ02 = "A";
                        processLabData.pcl_HQ03 = "A";
                        processLabData.pcl_PQ03 = "A";
                        processLabData.pcl_AQ03 = "A";
                        processLabData.pcl_NQ03 = "A";
                        processLabData.pcl_HQ04 = "W";
                        processLabData.pcl_PQ04 = "W";
                        processLabData.pcl_AQ04 = "W";
                        processLabData.pcl_NQ04 = "W";
                    }
                }
            }

            UPDATE_ProcessRequestLabData(processLabData);

            Console.WriteLine("CREATE_FILE_BEFORE : END");
        }

        private List<NARST_File> GET_DATA_BEFORE(TRProcessRequestLabData processLabData, int month)
        {
            Console.WriteLine("GET_DATA_BEFORE : Start");

            var narstDataList = new List<NARST_File>();
            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        var param1 = new SqlParameter("@lfu_id", DBNull.Value);
                        var param2 = new SqlParameter("@hos_code", processLabData.pcl_hos_code);
                        var param3 = new SqlParameter("@lab_code", processLabData.pcl_lab_code);
                        var param4 = new SqlParameter("@month", month);
                        var param5 = new SqlParameter("@year", process_year);
                        narstDataList = _db.NARST_Files.FromSqlRaw<NARST_File>($"sp_BATCH_PROC_Get_NARST_File @lfu_id, @hos_code, @lab_code, @month, @year", param1, param2, param3, param4, param5).ToList();

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

            return narstDataList;
        }

        private void CREATE_NARST_FILE_BEFORE_MONTH(TRProcessRequestLabData processLabData, int month, List<NARST_File> narstDataList)
        {
            Console.WriteLine($"CREATE_FILE_BEFORE_MONTH : Start : {month}");

            var narstDataTable = List_To_DataTable<NARST_File>(narstDataList);

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

                string dataFolderName = Path.Combine(whonetPath, "Data", Get_Data_FolderName(processLabData));
                P00_BATCH_Service.Create_Folder(dataFolderName);

                var dataFileName = Get_Data_Filename(processLabData, month);
                var dataFileFullName = Path.Combine(dataFolderName, dataFileName);
                P00_BATCH_Service.Delete_File_Exists(dataFileFullName);

                SQLiteDataAccess.CreateTable(dataFolderName, dataFileName, dataList.FirstOrDefault().Keys.ToList());

                SQLiteDataAccess.InsertData(dataFolderName, dataFileName, dataList);
            }

            Console.WriteLine("CREATE_FILE_BEFORE_MONTH : END");
        }

        private void UPDATE_ProcessRequestLabData(TRProcessRequestLabData processLabData)
        {
            Console.WriteLine($"UPDATE_ProcessRequestLabData : Start");

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        var processLabDataModel = _db.TRProcessRequestLabDatas.FirstOrDefault(x => x.pcl_arh_code == processLabData.pcl_arh_code && x.pcl_prv_code == processLabData.pcl_prv_code && x.pcl_hos_code == processLabData.pcl_hos_code && x.pcl_lab_code == processLabData.pcl_lab_code && x.pcl_year == processLabData.pcl_year);

                        processLabDataModel.pcl_M01_qty = processLabData.pcl_M01_qty;
                        processLabDataModel.pcl_M02_qty = processLabData.pcl_M02_qty;
                        processLabDataModel.pcl_M03_qty = processLabData.pcl_M03_qty;
                        processLabDataModel.pcl_M04_qty = processLabData.pcl_M04_qty;
                        processLabDataModel.pcl_M05_qty = processLabData.pcl_M05_qty;
                        processLabDataModel.pcl_M06_qty = processLabData.pcl_M06_qty;
                        processLabDataModel.pcl_M07_qty = processLabData.pcl_M07_qty;
                        processLabDataModel.pcl_M08_qty = processLabData.pcl_M08_qty;
                        processLabDataModel.pcl_M09_qty = processLabData.pcl_M09_qty;
                        processLabDataModel.pcl_M10_qty = processLabData.pcl_M10_qty;
                        processLabDataModel.pcl_M11_qty = processLabData.pcl_M11_qty;
                        processLabDataModel.pcl_M12_qty = processLabData.pcl_M12_qty;

                        processLabDataModel.pcl_HQ01 = processLabData.pcl_HQ01;
                        processLabDataModel.pcl_HQ02 = processLabData.pcl_HQ02;
                        processLabDataModel.pcl_HQ03 = processLabData.pcl_HQ03;
                        processLabDataModel.pcl_HQ04 = processLabData.pcl_HQ04;

                        processLabDataModel.pcl_PQ01 = processLabData.pcl_PQ01;
                        processLabDataModel.pcl_PQ02 = processLabData.pcl_PQ02;
                        processLabDataModel.pcl_PQ03 = processLabData.pcl_PQ03;
                        processLabDataModel.pcl_PQ04 = processLabData.pcl_PQ04;

                        processLabDataModel.pcl_AQ01 = processLabData.pcl_AQ01;
                        processLabDataModel.pcl_AQ02 = processLabData.pcl_AQ02;
                        processLabDataModel.pcl_AQ03 = processLabData.pcl_AQ03;
                        processLabDataModel.pcl_AQ04 = processLabData.pcl_AQ04;

                        processLabDataModel.pcl_updateuser = "BATCH";
                        processLabDataModel.pcl_updatedate = DateTime.Now;

                        _db.SaveChanges();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        Console.WriteLine("UPDATE_ProcessRequestLabData : " + ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                        _db.Dispose();
                    }
                }
            }

            Console.WriteLine($"UPDATE_ProcessRequestLabData : END");
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

        private TRProcessRequest INSERT_ProcessRequest(TRProcessRequestLabData processLabData, string month_start, string month_end, string pcr_prev_code)
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
                            pcr_arh_code = processLabData.pcl_arh_code,
                            pcr_prv_code = processLabData.pcl_prv_code,
                            pcr_hos_code = processLabData.pcl_hos_code,
                            pcr_lab_code = processLabData.pcl_lab_code,
                            pcr_type = "01",
                            pcr_month_start = month_start,
                            pcr_month_end = month_end,
                            pcr_year = processLabData.pcl_year.ToString(),
                            pcr_filetype = "application/vnd.ms-excel",
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

        private void INSERT_ProcessRequestDetail(TRProcessRequestLabData processLabData, string pcr_code_new)
        {
            Console.WriteLine($"INSERT_ProcessRequestDetail : Start : {processLabData.pcl_hos_code}_{processLabData.pcl_lab_code}");

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
                            pcd_hos_code = processLabData.pcl_hos_code,
                            pcd_lab_code = processLabData.pcl_lab_code,
                            pcd_M01_qty = processLabData.pcl_M01_qty,
                            pcd_M02_qty = processLabData.pcl_M02_qty,
                            pcd_M03_qty = processLabData.pcl_M03_qty,
                            pcd_M04_qty = processLabData.pcl_M04_qty,
                            pcd_M05_qty = processLabData.pcl_M05_qty,
                            pcd_M06_qty = processLabData.pcl_M06_qty,
                            pcd_M07_qty = processLabData.pcl_M07_qty,
                            pcd_M08_qty = processLabData.pcl_M08_qty,
                            pcd_M09_qty = processLabData.pcl_M09_qty,
                            pcd_M10_qty = processLabData.pcl_M10_qty,
                            pcd_M11_qty = processLabData.pcl_M11_qty,
                            pcd_M12_qty = processLabData.pcl_M12_qty,
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

        private void CREATE_NARST_MACRO_FILE(TRProcessRequest processRequest, List<TRProcessRequestLabData> processLabList)
        {
            Console.WriteLine($"CREATE_MACRO_FILE : Start");

            string processFolder = Get_Process_FolderName(processRequest);

            string processMacroFolderName = Path.Combine(whonetPath, "Macros", processFolder);
            P00_BATCH_Service.Create_Folder_Empty(processMacroFolderName);

            string processOutputFolderName = Path.Combine(whonetPath, "Output", processFolder);
            P00_BATCH_Service.Create_Folder_Empty(processOutputFolderName);

            var whonetMacroPathActive = $"Macro_NARST_{masterTemplate.mst_code}";
            string templateMacroFolderName = Path.Combine(whonetPath, "MacroTemplates", whonetMacroPathActive);
            DirectoryInfo templateMacroFolderInfo = new DirectoryInfo(templateMacroFolderName);
            FileInfo[] templateMacroFiles = templateMacroFolderInfo.GetFiles();

            var data_File_List = Get_Data_File_List(processRequest, processLabList);

            foreach (var macroFile in templateMacroFiles)
            {
                string newMacroName = $"P_{processRequest.pcr_code}_{macroFile.Name.Replace(macroFile.Extension, "")}";
                string newMacroFullName = Path.Combine(processMacroFolderName, $"{newMacroName}{macroFile.Extension}");

                P00_BATCH_Service.Copy_File(macroFile.FullName, newMacroFullName);

                string[] message = File.ReadAllLines(newMacroFullName);
                message[0] = $"Macro Name = {newMacroName}";
                File.WriteAllLines(newMacroFullName, message);
                string appendText = Environment.NewLine + $"Data file = {data_File_List}";
                File.AppendAllText(newMacroFullName, appendText, System.Text.Encoding.UTF8);
                appendText = Environment.NewLine + $"Output = SQLITE File (\"{processFolder}\\{processFolder}-{macroFile.Name.Replace(macroFile.Extension, ".db")}\")";
                File.AppendAllText(newMacroFullName, appendText, System.Text.Encoding.UTF8);
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

            var zip_NARST_Before = Get_Process_File_Before(processRequest);
            P00_BATCH_Service.Delete_File_Exists(zip_NARST_Before);
            ZipFile.CreateFromDirectory(processDataFolderName, zip_NARST_Before);

            P00_BATCH_Service.Delete_Folder(processDataFolderName);

            var backupPath_NARST_Before = P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:PARAM:PATH_BACKUP_FILE_BEFORE");
            if (string.IsNullOrEmpty(backupPath_NARST_Before) == false)
            {
                FileInfo fileInfo = new FileInfo(zip_NARST_Before);
                var backup_NARST_Before = Path.Combine(backupPath_NARST_Before, fileInfo.Name);
                P00_BATCH_Service.Copy_File(zip_NARST_Before, backup_NARST_Before);

                Console.WriteLine($"CREATE_MACRO_FILE : {backup_NARST_Before}");
            }

            Console.WriteLine($"CREATE_MACRO_FILE : END");
        }

        private void RUN_NARST_COMMAND(TRProcessRequest processRequest)
        {
            Console.WriteLine($"RUN_COMMAND : {processRequest.pcr_code}");

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

            Console.WriteLine($"RUN_COMMAND : END");
        }

        private void INSERT_ProcessDataResult(TRProcessRequest processRequest)
        {
            Console.WriteLine($"INSERT_ProcessDataResult : {processRequest.pcr_code}");

            string processFolder = Get_Process_FolderName(processRequest);
            string outputFolderName = Path.Combine(whonetPath, "Output", processFolder);

            DirectoryInfo directory = new DirectoryInfo(outputFolderName);
            FileInfo[] infos = directory.GetFiles();

            foreach (FileInfo file in infos)
            {
                Console.WriteLine("INSERT_ProcessDataResult : " + file.Name);

                List<TRProcessDataResult> processResultList = new List<TRProcessDataResult>();
                var detailCount = 1;

                string[] filenameSplit = file.Name.Split('-');
                string specimen = filenameSplit[5];
                string pathogen = file.Name.Replace($"{filenameSplit[0]}-{filenameSplit[1]}-{filenameSplit[2]}-{filenameSplit[3]}-{filenameSplit[4]}-{filenameSplit[5]}-", "").Replace(".db", "");

                var data = SQLiteDataAccess.LoadOutput(file.DirectoryName, file.Name, detailCount.ToString());

                detailCount++;
                foreach (var outputItem in data)
                {
                    processResultList.Add(new TRProcessDataResult()
                    {
                        pdr_pcr_code = processRequest.pcr_code,
                        pdr_arh_code = processRequest.pcr_arh_code,
                        pdr_prv_code = processRequest.pcr_prv_code,
                        pdr_hos_code = processRequest.pcr_hos_code,
                        pdr_lab_code = processRequest.pcr_lab_code,
                        pdr_specimen = specimen,
                        pdr_pathogen = pathogen,
                        pdr_row_idx = outputItem.ROW_IDX,
                        pdr_organisms = outputItem.ORGANISMS,
                        pdr_isolates = outputItem.ISOLATES,
                        pdr_drug_code = outputItem.DRUG_CODE,
                        pdr_drug_name = outputItem.DRUG_NAME,
                        pdr_class = outputItem.CLASS,
                        pdr_subclass = outputItem.SUBCLASS,
                        pdr_drug_code1 = outputItem.DRUG_CODE1,
                        pdr_methods = outputItem.METHODS,
                        pdr_bp_type = outputItem.BP_TYPE,
                        pdr_site_inf = outputItem.SITE_INF,
                        pdr_host = outputItem.HOST,
                        pdr_breakpoint = outputItem.BREAKPOINT,
                        pdr_num_tested = outputItem.NUM_TESTED,
                        pdr_perc_res = outputItem.PERC_RES,
                        pdr_perc_resx = outputItem.PERC_RESX,
                        pdr_perc_int = outputItem.PERC_INT,
                        pdr_perc_susc = outputItem.PERC_SUSC,
                        pdr_perc_suscx = outputItem.PERC_SUSCX,
                        pdr_perc_unk = outputItem.PERC_UNK,
                        pdr_res_ci = outputItem.RES_CI,
                        pdr_mic50 = outputItem.MIC50,
                        pdr_mic90 = outputItem.MIC90,
                        pdr_geom_mean = outputItem.GEOM_MEAN,
                        pdr_mic_range = outputItem.MIC_RANGE,
                        pdr_num_quant = outputItem.NUM_QUANT,
                        pdr_zone_6 = outputItem.ZONE_6,
                        pdr_zone_7 = outputItem.ZONE_7,
                        pdr_zone_8 = outputItem.ZONE_8,
                        pdr_zone_9 = outputItem.ZONE_9,
                        pdr_zone_10 = outputItem.ZONE_10,
                        pdr_zone_11 = outputItem.ZONE_11,
                        pdr_zone_12 = outputItem.ZONE_12,
                        pdr_zone_13 = outputItem.ZONE_13,
                        pdr_zone_14 = outputItem.ZONE_14,
                        pdr_zone_15 = outputItem.ZONE_15,
                        pdr_zone_16 = outputItem.ZONE_16,
                        pdr_zone_17 = outputItem.ZONE_17,
                        pdr_zone_18 = outputItem.ZONE_18,
                        pdr_zone_19 = outputItem.ZONE_19,
                        pdr_zone_20 = outputItem.ZONE_20,
                        pdr_zone_21 = outputItem.ZONE_21,
                        pdr_zone_22 = outputItem.ZONE_22,
                        pdr_zone_23 = outputItem.ZONE_23,
                        pdr_zone_24 = outputItem.ZONE_24,
                        pdr_zone_25 = outputItem.ZONE_25,
                        pdr_zone_26 = outputItem.ZONE_26,
                        pdr_zone_27 = outputItem.ZONE_27,
                        pdr_zone_28 = outputItem.ZONE_28,
                        pdr_zone_29 = outputItem.ZONE_29,
                        pdr_zone_30 = outputItem.ZONE_30,
                        pdr_zone_31 = outputItem.ZONE_31,
                        pdr_zone_32 = outputItem.ZONE_32,
                        pdr_zone_33 = outputItem.ZONE_33,
                        pdr_zone_34 = outputItem.ZONE_34,
                        pdr_zone_35 = outputItem.ZONE_35,
                        pdr_zone__35 = outputItem.ZONE__35,
                        pdr_mic_L_001 = outputItem.MIC_L_001,
                        pdr_mic__0015 = outputItem.MIC__0015,
                        pdr_mic__002 = outputItem.MIC__002,
                        pdr_mic__003 = outputItem.MIC__003,
                        pdr_mic__004 = outputItem.MIC__004,
                        pdr_mic__006 = outputItem.MIC__006,
                        pdr_mic__008 = outputItem.MIC__008,
                        pdr_mic__012 = outputItem.MIC__012,
                        pdr_mic__016 = outputItem.MIC__016,
                        pdr_mic__023 = outputItem.MIC__023,
                        pdr_mic__032 = outputItem.MIC__032,
                        pdr_mic__047 = outputItem.MIC__047,
                        pdr_mic__064 = outputItem.MIC__064,
                        pdr_mic__094 = outputItem.MIC__094,
                        pdr_mic__125 = outputItem.MIC__125,
                        pdr_mic__19 = outputItem.MIC__19,
                        pdr_mic__25 = outputItem.MIC__25,
                        pdr_mic__38 = outputItem.MIC__38,
                        pdr_mic__5 = outputItem.MIC__5,
                        pdr_mic__75 = outputItem.MIC__75,
                        pdr_mic_1 = outputItem.MIC_1,
                        pdr_mic_1_5 = outputItem.MIC_1_5,
                        pdr_mic_2 = outputItem.MIC_2,
                        pdr_mic_3 = outputItem.MIC_3,
                        pdr_mic_4 = outputItem.MIC_4,
                        pdr_mic_6 = outputItem.MIC_6,
                        pdr_mic_8 = outputItem.MIC_8,
                        pdr_mic_12 = outputItem.MIC_12,
                        pdr_mic_16 = outputItem.MIC_16,
                        pdr_mic_24 = outputItem.MIC_24,
                        pdr_mic_32 = outputItem.MIC_32,
                        pdr_mic_48 = outputItem.MIC_48,
                        pdr_mic_64 = outputItem.MIC_64,
                        pdr_mic_96 = outputItem.MIC_96,
                        pdr_mic_128 = outputItem.MIC_128,
                        pdr_mic_192 = outputItem.MIC_192,
                        pdr_mic_256 = outputItem.MIC_256,
                        pdr_mic_384 = outputItem.MIC_384,
                        pdr_mic_g256 = outputItem.MIC_G256,
                        pdr_createuser = "BATCH",
                        pdr_createdate = DateTime.Now
                    });
                }

                using (var _db = new ProcessContext())
                {
                    using (var trans = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            _db.BulkInsert(processResultList);
                            _db.SaveChanges();

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            Console.WriteLine("INSERT_ProcessDataResult BulkInsert : Error " + ex.Message);
                        }
                        finally
                        {
                            trans.Dispose();
                            _db.Dispose();

                            GC.Collect();
                            GC.WaitForPendingFinalizers();

                            Console.WriteLine("INSERT_ProcessDataResult BulkInsert : Success");
                        }
                    }
                }
            }

            var zip_NARST_After = Get_Process_File_After(processRequest);
            P00_BATCH_Service.Delete_File_Exists(zip_NARST_After);
            ZipFile.CreateFromDirectory(outputFolderName, zip_NARST_After);

            P00_BATCH_Service.Delete_Folder(outputFolderName);

            var backupPath_NARST_After = P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:PARAM:PATH_BACKUP_FILE_AFTER");
            if (string.IsNullOrEmpty(backupPath_NARST_After) == false)
            {
                FileInfo fileInfo = new FileInfo(zip_NARST_After);
                var backup_NARST_After = Path.Combine(backupPath_NARST_After, fileInfo.Name);
                P00_BATCH_Service.Copy_File(zip_NARST_After, backup_NARST_After);

                Console.WriteLine($"INSERT_ProcessDataResult : PATH_BACKUP_FILE_AFTER : {backup_NARST_After}");
            }

            Console.WriteLine($"INSERT_ProcessDataResult : END");
        }

        private void GENERATE_NARST_EXCEL_FILE(TRProcessRequest processRequest)
        {
            Console.WriteLine($"GENERATE_EXCEL_FILE : {processRequest.pcr_code}");

            List<TRProcessDataResult> processResultList = new List<TRProcessDataResult>();

            var hospitalCount = 0;

            var hospitalCountStr = "";
            var month_start = (new DateTime(Convert.ToInt32(processRequest.pcr_year), Convert.ToInt32(processRequest.pcr_month_start), 1)).ToString("MMM");
            var month_end = (new DateTime(Convert.ToInt32(processRequest.pcr_year), Convert.ToInt32(processRequest.pcr_month_end), 1)).ToString("MMM");
            var otherMessage = "";

            var currentDateTime = DateTime.Now;
            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        processResultList = _db.TRProcessDataResults.Where(x => x.pdr_pcr_code == processRequest.pcr_code).ToList();

                        hospitalCount = _db.TRProcessRequestDetails.Where(x => x.pcd_pcr_code == processRequest.pcr_code).ToList().Count;
                        if (hospitalCount == 0)
                        {
                            hospitalCountStr = $"? hospital";
                        }
                        else if (hospitalCount == 1)
                        {
                            hospitalCountStr = $"{hospitalCount} hospital";
                        }
                        else
                        {
                            hospitalCountStr = $"{hospitalCount} hospitals";
                        }

                        if (string.IsNullOrEmpty(processRequest.pcr_arh_code))
                        {
                            //otherMessage = $", NATION";
                        }
                        else if (string.IsNullOrEmpty(processRequest.pcr_prv_code))
                        {
                            otherMessage = $", RMSC {processRequest.pcr_arh_code}";
                        }
                        else if (string.IsNullOrEmpty(processRequest.pcr_hos_code))
                        {
                            var province = _db.TCProvinces.FirstOrDefault(x => x.prv_code == processRequest.pcr_prv_code);
                            otherMessage = $", (จังหวัด{province.prv_name})";
                        }
                        else
                        {
                            var hospital = _db.TRHospitals.FirstOrDefault(x => x.hos_code == processRequest.pcr_hos_code);
                            otherMessage = $", ({hospital.hos_name})";
                        }

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

            var excelTemplateActive = $"Template_NARST_{masterTemplate.mst_code}.xlsx";
            string excelFileTemplate = Path.Combine(whonetPath, "ExcelFile", excelTemplateActive);
            //string excelFileTemplate = Path.Combine(whonetPath, "ExcelFile", P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:PARAM:EXCEL_TEMPLATE_FILE"));
            string excelFileFolder = Path.Combine(whonetPath, "ExcelFile", Get_Process_FolderName(processRequest));
            P00_BATCH_Service.Create_Folder(excelFileFolder);
            string excelFileResult = Path.Combine(excelFileFolder, Get_Process_File_Result(processRequest));

            var sheetList = P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:PARAM:EXCEL_TEMPLATE_SHEET").Split(',', StringSplitOptions.RemoveEmptyEntries);

            var processColumnList = new List<TCProcessExcelColumn>();
            var processRowList = new List<TCProcessExcelRow>();
            var processTemplateList = new List<TCProcessExcelTemplate>();

            if (File.Exists(excelFileResult)) File.Delete(excelFileResult);
            File.Copy(excelFileTemplate, excelFileResult);

            FileInfo file = new FileInfo(excelFileResult);

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            ExcelWorksheet worksheet;

            using (ExcelPackage package = new ExcelPackage(file))
            {
                foreach (var sheet in sheetList)
                {

                    using (var _db = new ProcessContext())
                    {
                        using (var trans = _db.Database.BeginTransaction())
                        {
                            try
                            {
                                processColumnList = _db.TCProcessExcelColumnModel.Where(x => x.pec_sheet_name == sheet && x.pec_mst_code == masterTemplate.mst_code).ToList();
                                processRowList = _db.TCProcessExcelRowModel.Where(x => x.per_sheet_name == sheet && x.per_mst_code == masterTemplate.mst_code).ToList();
                                processTemplateList = _db.TCProcessExcelTemplateModel.Where(x => x.pet_sheet_name == sheet && x.pet_mst_code == masterTemplate.mst_code).ToList();

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

                    worksheet = package.Workbook.Worksheets[sheet];

                    //worksheet.Cells[2, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //worksheet.Cells[2, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 217, 241));
                    //var refColor = worksheet.Cells[2, 3].Style.Fill.BackgroundColor.Tint;

                    var currentRowMaxValue = 0;

                    var message = worksheet.Cells[1, 1].Value.ToString();

                    message = message.Replace("{hospitalCount}", hospitalCountStr);
                    message = message.Replace("{month_start}", month_start);
                    message = message.Replace("{month_end}", month_end);
                    message = message.Replace("{year}", processRequest.pcr_year);
                    message = message.Replace("{otherMessage}", otherMessage);

                    worksheet.Cells[1, 1].Value = message;

                    foreach (var itemTemplate in processTemplateList.OrderBy(x => x.pet_row_num).ThenBy(x => x.pet_col_num))
                    {
                        var valueRow = itemTemplate.pet_row_num.Value;
                        var valueCol = itemTemplate.pet_col_num.Value;

                        var processColumn = processColumnList.FirstOrDefault(x => x.pec_col_num == valueCol);
                        var processRow = processRowList.FirstOrDefault(x => x.per_row_num == valueRow);

                        if (processColumn != null && processRow != null)
                        {
                            var drug_code = processColumn.pec_ant_code;
                            var site_inf = itemTemplate.pet_site_inf;

                            var pathogen = processRow.per_macro_name;

                            if (processColumn.pec_col_name.Contains("(A)"))
                            {
                                pathogen = pathogen + "-Urine-Exclude";
                            }
                            else if (processColumn.pec_col_name.Contains("(U)"))
                            {
                                if (pathogen.Contains("-Urine") == false)
                                {
                                    pathogen = pathogen + "-Urine";
                                }
                            }

                            if (processColumn.pec_col_name.Contains(" MIC"))
                            {
                                pathogen = pathogen + "_MIC";
                            }

                            if (processColumn.pec_col_name.Contains("(parenteral)"))
                            {
                                site_inf = "Parenteral";
                            }

                            if (processColumn.pec_col_name.Contains("(Oral)"))
                            {
                                site_inf = "Oral";
                            }

                            if (pathogen.Contains("Nonmeningitis"))
                            {
                                site_inf = "Non-meningitis";
                            }
                            else if (pathogen.Contains("Meningitis"))
                            {
                                site_inf = "Meningitis";
                            }

                            var specimen = sheet;
                            if (sheet == "Stool")
                            {
                                specimen = "Stool and Rectal Swab";
                            }

                            if (itemTemplate.pet_d == true)
                            {
                                drug_code = "OXA";
                            }
                            else if (itemTemplate.pet_e == true)
                            {
                                if (pathogen.Contains("_MIC") == false)
                                {
                                    pathogen = pathogen + "_MIC";
                                }
                                if (drug_code.Contains("_NM") == false)
                                {
                                    drug_code = drug_code + "_NM";
                                }
                            }
                            else if (itemTemplate.pet_u == true)
                            {
                                if (pathogen.Contains("-Urine") == false)
                                {
                                    pathogen = pathogen + "-Urine";
                                }
                            }

                            if (itemTemplate.pet_f == true)
                            {
                                drug_code = "FOX";
                            }

                            var cellValueList = processResultList.Where(x => x.pdr_specimen == specimen && x.pdr_pathogen == pathogen && x.pdr_drug_code == drug_code && (string.IsNullOrEmpty(site_inf) || x.pdr_site_inf == site_inf)).ToList();
                            //if (processRow.per_row_num >= 122 && processColumn.pec_col_name.Contains("(Oral)"))
                            //{
                            //    var xxx = 10;
                            //}

                            if (cellValueList.Count > 0)
                            {
                                var cellValue = cellValueList.OrderBy(x => Convert.ToInt32(x.pdr_row_idx)).FirstOrDefault();

                                var perc_susc = Convert.ToDouble(cellValue.pdr_perc_susc);
                                if (itemTemplate.pet_i == true)
                                {
                                    perc_susc = Convert.ToDouble(cellValue.pdr_perc_int);
                                }
                                //var str_perc_susc = perc_susc.ToString("0.0");
                                //if (string.IsNullOrEmpty(itemTemplate.pet_cell_sup) == false)
                                //{
                                //    str_perc_susc = str_perc_susc + " " + itemTemplate.pet_cell_sup;
                                //}
                                if (itemTemplate.pet_d == true || itemTemplate.pet_e == true || itemTemplate.pet_f == true || itemTemplate.pet_h == true || itemTemplate.pet_i == true || itemTemplate.pet_u == true || itemTemplate.pet_wt == true)
                                {
                                    worksheet.Cells[valueRow, valueCol].Value = perc_susc.ToString("0.0 ") + itemTemplate.pet_cell_sup;
                                }
                                else
                                {
                                    worksheet.Cells[valueRow, valueCol].Value = perc_susc;
                                }
                                worksheet.Cells[(valueRow + 1), valueCol].Value = Convert.ToInt32(cellValue.pdr_num_tested);

                                worksheet.Cells[valueRow, valueCol].Style.Numberformat.Format = "0.0";
                                worksheet.Cells[(valueRow + 1), valueCol].Style.Numberformat.Format = "(##)";

                                worksheet.Cells[valueRow, valueCol, (valueRow + 1), valueCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[valueRow, valueCol, (valueRow + 1), valueCol].Style.Fill.BackgroundColor.SetColor(GET_COLOR_BACKGROUND(perc_susc));
                                worksheet.Cells[valueRow, valueCol, (valueRow + 1), valueCol].Style.Fill.BackgroundColor.Tint = 0;

                                if (sheet != "Stool")
                                {
                                    if (worksheet.Cells[valueRow, 3].Text == "")
                                    {
                                        worksheet.Cells[valueRow, 3].Value = Convert.ToInt32(cellValue.pdr_num_tested);
                                        currentRowMaxValue = Convert.ToInt32(cellValue.pdr_num_tested);
                                    }
                                    else if (currentRowMaxValue < Convert.ToInt32(cellValue.pdr_num_tested))
                                    {
                                        worksheet.Cells[valueRow, 3].Value = Convert.ToInt32(cellValue.pdr_num_tested);
                                        currentRowMaxValue = Convert.ToInt32(cellValue.pdr_num_tested);
                                    }
                                }
                                else
                                {
                                    if (worksheet.Cells[valueRow, 2].Text == "")
                                    {
                                        worksheet.Cells[valueRow, 2].Value = Convert.ToInt32(cellValue.pdr_num_tested);
                                        currentRowMaxValue = Convert.ToInt32(cellValue.pdr_num_tested);
                                    }
                                    else if (currentRowMaxValue < Convert.ToInt32(cellValue.pdr_num_tested))
                                    {
                                        worksheet.Cells[valueRow, 2].Value = Convert.ToInt32(cellValue.pdr_num_tested);
                                        currentRowMaxValue = Convert.ToInt32(cellValue.pdr_num_tested);
                                    }
                                }
                            }
                            else
                            {
                                if (processColumn.pec_col_name.Contains("(U)") && itemTemplate.pet_u == true)
                                {
                                    worksheet.Cells[valueRow, valueCol].Value = "-";
                                }
                                else
                                {
                                    worksheet.Cells[valueRow, valueCol].Value = "    -   " + itemTemplate.pet_cell_sup;
                                }
                            }
                        }
                    }

                    worksheet.Calculate();

                    //------- am add for set print format
                    worksheet.PrinterSettings.FitToPage = true;
                    worksheet.PrinterSettings.FitToWidth = 1;
                    worksheet.PrinterSettings.FitToHeight = 1;
                    //------- am add for set print format
                }

                package.Save();
            }

            var backupPath_NARST_Result = P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:PARAM:PATH_BACKUP_FILE_RESULT");

            var NARST_Result = Path.Combine(backupPath_NARST_Result, Get_Process_File_Result(processRequest));
            P00_BATCH_Service.Delete_File_Exists(NARST_Result);
            //ZipFile.CreateFromDirectory(excelFileFolder, NARST_Result);

            P00_BATCH_Service.Create_Folder(backupPath_NARST_Result);
            if (string.IsNullOrEmpty(backupPath_NARST_Result) == false)
            {
                FileInfo fileInfo = new FileInfo(NARST_Result);
                var backup_NARST_Result = Path.Combine(backupPath_NARST_Result, fileInfo.Name);
                P00_BATCH_Service.Copy_File(excelFileResult, backup_NARST_Result);

                Console.WriteLine($"GENERATE_EXCEL_FILE : {backup_NARST_Result}");
            }

            Console.WriteLine($"GENERATE_EXCEL_FILE : END");
        }

        private void GET_MASTERTEMPLATE_ACTIVE()
        {
            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        masterTemplate = _db.TCMasterTemplates.FirstOrDefault(x => x.mst_active == true);

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
        }

        private Color GET_COLOR_BACKGROUND(double value)
        {
            if (value > 90)
            {
                return Color.FromArgb(0, 255, 0);
            }
            else if (value > 80)
            {
                return Color.FromArgb(255, 255, 0);
            }
            else if (value > 70)
            {
                return Color.FromArgb(247, 150, 70);
            }
            else if (value > 50)
            {
                return Color.FromArgb(255, 0, 0);
            }
            else
            {
                return Color.FromArgb(217, 217, 217);
            }
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
