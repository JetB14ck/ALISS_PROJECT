using ALISS.Process.Batch.DTO;
using ALISS.Process.Batch.Model;
using ALISS.Process.Batch.DataAccess;
using ALISS.Process.Batch.SQLite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using OfficeOpenXml.Style;
using OfficeOpenXml.Drawing;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using EFCore.BulkExtensions;
using System.Net;
//using EFCore.BulkExtensions;

namespace ALISS.Process.Batch
{
    class Program
    {
        private static IConfiguration _iconfiguration;
        private static List<TRProcessRequestDTO> processRequestList = new List<TRProcessRequestDTO>();
        private static List<TRSTGProcessFileUpload> fileUploadList = new List<TRSTGProcessFileUpload>();
        private static string whonetPath;

        private static string Get_DataFilename(string pfu_hos_code, string pfu_id)
        {
            return $"D_{pfu_hos_code}_{pfu_id}.sqlite";
        }

        private static string Get_DataFilename_Month(string pfu_hos_code, string year, string month)
        {
            return $"M_{pfu_hos_code}_{year}_{month}.sqlite";
        }

        private static string Get_ProcessName(string arh_code, string prv_code, string hos_code, string year, string month_start, string month_end)
        {
            var prefix = "";
            if (string.IsNullOrEmpty(hos_code) == false) prefix = "P_HOS_" + hos_code;
            else if (string.IsNullOrEmpty(prv_code) == false) prefix = "P_PRV_" + prv_code;
            else if (string.IsNullOrEmpty(arh_code) == false) prefix = "P_ARH_" + arh_code;
            else if (string.IsNullOrEmpty(arh_code)) prefix = "P_NATION_";
            return prefix + "_" + year + "_" + month_start + "_" + month_end;
        }

        private static string pcr_code_pattern()
        {
            return "PCR_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00");
        }

        private static string ouputFilename(string newmacroName)
        {
            return $"O_{newmacroName}.db";
        }

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            //var test = P00_Configuration_Service.GetConfigurationValue("WHONET:NARST:PARAM:PROCESS_YEAR");

            #region WHONET:ACTIVE MASTER TEMPLATE

            Console.WriteLine($"ACTIVE_MASTER_TEMPLATE : START");
            activeMasterTemplate();

            Console.WriteLine($"ACTIVE_MASTER_TEMPLATE : END");
            #endregion

            #region WHONET:LAB_ALERT
            if (P00_BATCH_Service.GetConfigurationValue("WHONET:LAB_ALERT:RUN_SERVICE") == "Y")
            {
                var file_list = P00_BATCH_Service.GetConfigurationValue("WHONET:LAB_ALERT:FILE_LIST");

                if (string.IsNullOrEmpty(file_list))
                {
                    using (var service = new P01_LabAlert_Service())
                    {
                        service.LabAlert_SERVICE_AUTO();
                    }
                }
                else
                {
                    using (var service = new P01_LabAlert_Service())
                    {
                        service.LabAlert_SERVICE_MANUAL(file_list);
                    }
                }
            }
            #endregion

            #region WHONET:NARST:FILE_BEFORE
            if (P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:FILE_BEFORE:RUN_SERVICE") == "Y")
            {
                var hos_list = P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:FILE_BEFORE:HOS_LIST");

                if (string.IsNullOrEmpty(hos_list))
                {
                    using (var service = new P02_WHONET_NARST_Service())
                    {
                        service.FILE_BEFORE_SERVICE_AUTO();
                    }
                }
                else
                {
                    using (var service = new P02_WHONET_NARST_Service())
                    {
                        service.FILE_BEFORE_SERVICE_MANUAL(hos_list);
                    }
                }
            }
            #endregion

            #region WHONET:NARST:HOS
            if (P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:HOS:RUN_SERVICE") == "Y")
            {
                var hos_list = P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:HOS:HOS_LIST");

                if (string.IsNullOrEmpty(hos_list))
                {
                    using (var service = new P02_WHONET_NARST_Service())
                    {
                        service.HOS_SERVICE_AUTO();
                    }
                }
                else
                {
                    using (var service = new P02_WHONET_NARST_Service())
                    {
                        service.HOS_SERVICE_MANUAL(hos_list);
                    }
                }
            }
            #endregion

            #region WHONET:NARST:PRV
            if (P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:PRV:RUN_SERVICE") == "Y")
            {
                var prv_list = P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:PRV:PRV_LIST");

                if (string.IsNullOrEmpty(prv_list))
                {
                    using (var service = new P02_WHONET_NARST_Service())
                    {
                        service.PRV_SERVICE_AUTO();
                    }
                }
                else
                {
                    using (var service = new P02_WHONET_NARST_Service())
                    {
                        service.PRV_SERVICE_MANUAL(prv_list);
                    }
                }
            }
            #endregion

            #region WHONET:NARST:ARH
            if (P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:ARH:RUN_SERVICE") == "Y")
            {
                var arh_list = P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:ARH:ARH_LIST");

                if (string.IsNullOrEmpty(arh_list))
                {
                    using (var service = new P02_WHONET_NARST_Service())
                    {
                        service.ARH_SERVICE_AUTO();
                    }
                }
                else
                {
                    using (var service = new P02_WHONET_NARST_Service())
                    {
                        service.ARH_SERVICE_MANUAL(arh_list);
                    }
                }
            }
            #endregion

            #region WHONET:NARST:NATION
            if (P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:NATION:RUN_SERVICE") == "Y")
            {
                using (var service = new P02_WHONET_NARST_Service())
                {
                    service.NATION_SERVICE();
                }
            }
            #endregion

            #region WHONET:NARST:INSERT_RESULT
            if (P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:INSERT_RESULT_AND_GENERATE_EXCEL:INSERT_RESULT") == "Y")
            {
                var pcr_list = P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:INSERT_RESULT_AND_GENERATE_EXCEL:PCR_LIST");

                if (string.IsNullOrEmpty(pcr_list) == false)
                {
                    using (var service = new P02_WHONET_NARST_Service())
                    {
                        service.INSERT_RESULT_SERVICE(pcr_list);
                    }
                }
            }
            #endregion

            #region WHONET:NARST:GENERATE_EXCEL
            if (P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:INSERT_RESULT_AND_GENERATE_EXCEL:GENERATE_EXCEL") == "Y")
            {
                var pcr_list = P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:INSERT_RESULT_AND_GENERATE_EXCEL:PCR_LIST");

                if (string.IsNullOrEmpty(pcr_list) == false)
                {
                    using (var service = new P02_WHONET_NARST_Service())
                    {
                        service.GENERATE_EXCEL_SERVICE(pcr_list);
                    }
                }
            }
            #endregion

            #region WHONET:GLASS
            if (P00_BATCH_Service.GetConfigurationValue("WHONET:GLASS:RUN_SERVICE") == "Y")
            {
                var process_list = P00_BATCH_Service.GetConfigurationValue("WHONET:GLASS:PROCESS_LIST");

                if (string.IsNullOrEmpty(process_list))
                {
                    using (var service = new P03_WHONET_GLASS_Service())
                    {
                        service.RUN_GLASS_SERVICE_AUTO();
                    }
                }
                else
                {
                    using (var service = new P03_WHONET_GLASS_Service())
                    {
                        service.FILE_BEFORE_SERVICE_MANUAL(process_list);

                        service.RUN_GLASS_SERVICE_MANUAL(process_list);
                    }
                }
            }
            #endregion
        }

        private static TRProcessRequest Get_ProcessRequest(string pcr_code)
        {
            Console.WriteLine($"Get_ProcessRequest : Start : {pcr_code}");

            var processRequest = new TRProcessRequest();

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        processRequest = _db.TRProcessRequests.FirstOrDefault(x => x.pcr_code == pcr_code);

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

            Console.WriteLine("Get_ProcessRequest : End");

            return processRequest;
        }

        private static void Get_Process_List()
        {
            Console.WriteLine("p01_Get_Process_List : Start");

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        processRequestList = _db.TRProcessRequestDTOs.FromSqlRaw<TRProcessRequestDTO>("SELECT * FROM TRProcessRequest WHERE pcr_status = 'A'").ToList();

                        foreach (var item in processRequestList)
                        {
                            item.processRequestDetails = _db.TRProcessRequestDetails.Where(x => x.pcd_pcr_code == item.pcr_code).ToList();
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

            Console.WriteLine("p01_Get_Process_List : End");
        }

        private static void Copy_DB()
        {
            Console.WriteLine("p02_Copy_DB : Start");

            foreach (var item in processRequestList)
            {
                using (var _db = new ProcessContext())
                {
                    using (var trans = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            var month_start = new DateTime(Convert.ToInt32(item.pcr_year), Convert.ToInt32(item.pcr_month_start), 1);
                            var month_end = new DateTime(Convert.ToInt32(item.pcr_year), Convert.ToInt32(item.pcr_month_end), 1).AddMonths(1);
                            var objResult = _db.Database.ExecuteSqlRaw("sp_Copy_TRProcessData {0}, {1}, {2}", item.pcr_code, month_start, month_end);

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

            Console.WriteLine("p02_Copy_DB : End");
        }

        private static void Get_File_List()
        {
            Console.WriteLine("Get_File_List : Start");

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        fileUploadList = _db.TRSTGProcessFileUploads.FromSqlRaw<TRSTGProcessFileUpload>("SELECT * FROM TRSTGProcessFileUpload WHERE pfu_StartDatePeriod >= '2020-01-01'").AsNoTracking().ToList();

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

            Console.WriteLine("Get_File_List : End");
        }

        private static void Create_F_SQLite_File()
        {
            Console.WriteLine("Create_F_SQLite_File : Start");

            var processDataList = new List<V_TRSTGProcessData>();
            var whonetFieldList = new Dictionary<string, string>();

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        var tmpWhonetColumnList = _db.TCWHONETColumns.FromSqlRaw<TCWHONETColumn>("sp_GET_TCWHONETColumn_Active @wnc_mst_code = NULL").ToList();
                        tmpWhonetColumnList.ForEach(x =>
                        {
                            whonetFieldList.Add(x.wnc_code, x.wnc_name);
                        });

                        var tmpWhonetAntibioticColumnList = _db.TCWHONET_AntibioticsModel.FromSqlRaw<TCWHONET_Antibiotics>("sp_GET_TCWHONET_Antibiotics_Active").ToList();
                        tmpWhonetAntibioticColumnList.ForEach(x =>
                        {
                            whonetFieldList.Add(x.who_ant_code, x.who_ant_name);
                        });

                        //var tmpWhonetFieldList = processDataDetailList.Select(x => x.pdd_whonetfield).Distinct().ToList();
                        //tmpWhonetFieldList.ForEach(x =>
                        //{
                        //    if (whonetFieldList.ContainsKey(x) == false && whonetFieldList.ContainsValue(x) == false)
                        //    {
                        //        whonetFieldList.Add(x, x);
                        //    }
                        //});

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

            foreach (var fileUpload in fileUploadList)
            {
                Console.WriteLine("Create_F_SQLite_File : " + fileUpload.pfu_hos_code);

                using (var _db = new ProcessContext())
                {
                    using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        try
                        {
                            processDataList = _db.V_TRSTGProcessDatas.Where(x => x.pdh_lfu_id == fileUpload.pfu_id).ToList();

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

                string dataFilename = Get_DataFilename(fileUpload.pfu_hos_code, fileUpload.pfu_id.ToString().ToUpper());

                string filepath = Path.Combine(whonetPath, "Data", fileUpload.pfu_hos_code);
                if (Directory.Exists(filepath) == false) Directory.CreateDirectory(filepath);

                var datafilefullname = Path.Combine(filepath, dataFilename);
                if (File.Exists(datafilefullname) == false) File.Delete(datafilefullname);

                SQLiteDataAccess.CreateTable(filepath, dataFilename, whonetFieldList.Keys.ToList());

                var dataDicList = new List<Dictionary<string, string>>();

                foreach (var itemData in processDataList.OrderBy(x => x.pdh_cdate).OrderBy(x => x.pdh_labno).Select(x => x.pdd_ldh_id).Distinct())
                {
                    var dataList = new Dictionary<string, string>();

                    var dataDetailList = processDataList.Where(x => x.pdd_ldh_id == itemData).ToList();

                    var pdh_labno = "";

                    foreach (var itemDataDetail in dataDetailList)
                    {
                        pdh_labno = itemDataDetail.pdh_labno;

                        if (whonetFieldList.Any(x => x.Key == itemDataDetail.pdd_whonetfield || x.Value == itemDataDetail.pdd_whonetfield))
                        {
                            var whonetColumn = whonetFieldList.FirstOrDefault(x => x.Key == itemDataDetail.pdd_whonetfield || x.Value == itemDataDetail.pdd_whonetfield).Key;
                            if (dataList.ContainsKey(whonetColumn) == false)
                            {
                                var fieldValue = itemDataDetail.pdd_mappingvalue ?? itemDataDetail.pdd_originalvalue;
                                if (whonetColumn.Contains("DATE"))
                                {
                                    string format1 = "M/d/yyyy H:mm:ss";
                                    string format2 = "M/d/yyyy H:mm:ss tt";
                                    string format3 = "M/d/yyyy H:mm:sss";
                                    string format4 = "M/d/yyyy H:mm:sss tt";
                                    string format5 = "M/d/yyyy";
                                    DateTime tmpFieldValue;
                                    if (DateTime.TryParseExact(fieldValue, format1, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                    {
                                        fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                    }
                                    else if (DateTime.TryParseExact(fieldValue, format2, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                    {
                                        fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                    }
                                    else if (DateTime.TryParseExact(fieldValue, format3, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                    {
                                        fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                    }
                                    else if (DateTime.TryParseExact(fieldValue, format4, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                    {
                                        fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                    }
                                    else if (DateTime.TryParseExact(fieldValue.Split(' ')[0], format5, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                    {
                                        fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                    }
                                    else
                                    {
                                        fieldValue = null;
                                    }
                                }

                                dataList.Add(whonetColumn, fieldValue);
                            }
                        }
                    }

                    if (dataList.Any(x => x.Key == "COUNTRY_A") == false)
                    {
                        dataList.Add("COUNTRY_A", "THA");
                    }
                    if (dataList.Any(x => x.Key == "LABORATORY") == false)
                    {
                        dataList.Add("LABORATORY", "H20");
                    }
                    if (dataList.Any(x => x.Key == "ORIGIN") == false)
                    {
                        dataList.Add("ORIGIN", "");
                    }
                    if (dataList.Any(x => x.Key == "INSTITUT") == false)
                    {
                        dataList.Add("INSTITUT", (fileUpload.pfu_hos_code + "." + fileUpload.pfu_lab));
                    }
                    if (dataList.Any(x => x.Key == "PATIENT_ID" || x.Value == "PATIENT_ID") == false)
                    {
                        dataList.Add("PATIENT_ID", pdh_labno);
                    }

                    dataDicList.Add(dataList);
                }

                SQLiteDataAccess.InsertData(filepath, dataFilename, dataDicList);

                filepath = Path.Combine(whonetPath, "Data");
                var copyfilename = Path.Combine(filepath, dataFilename);
                if (File.Exists(copyfilename)) File.Delete(copyfilename);
                File.Copy(datafilefullname, copyfilename);
            }

            Console.WriteLine("Create_F_SQLite_File : End");
        }

        private static void Create_M_SQLite_File()
        {
            Console.WriteLine("Create_M_SQLite_File : Start");

            var processDataList = new List<V_TRSTGProcessData>();
            var dataTable = new DataTable();

            List<string> hos_codeList = fileUploadList.Select(x => x.pfu_hos_code).Distinct().ToList();

            foreach (var hos_code in hos_codeList)
            {
                Console.WriteLine("Create_M_SQLite_File : " + hos_code);

                var minDate = fileUploadList.Where(x => x.pfu_hos_code == hos_code).OrderBy(x => x.pfu_StartDatePeriod).FirstOrDefault().pfu_StartDatePeriod.Value;
                minDate = new DateTime(minDate.Year, minDate.Month, 1);

                var maxDate = fileUploadList.Where(x => x.pfu_hos_code == hos_code).OrderByDescending(x => x.pfu_EndDatePeriod).FirstOrDefault().pfu_EndDatePeriod.Value;
                maxDate = new DateTime(maxDate.Year, maxDate.Month, 1);

                var monthDiff = (maxDate - minDate);

                var tmpDataumnList = new List<NARST_File>();

                for (var month = minDate; month <= maxDate; month = month.AddMonths(1))
                {
                    Console.WriteLine("Create_M_SQLite_File : " + hos_code + " " + month.Year + " " + month.Month.ToString("00"));

                    using (var _db = new ProcessContext())
                    {
                        using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            try
                            {
                                var param1 = new SqlParameter("@lfu_id", DBNull.Value);
                                var param2 = new SqlParameter("@hos_code", hos_code);
                                var param3 = new SqlParameter("@month", month.Month);
                                var param4 = new SqlParameter("@year", month.Year);
                                tmpDataumnList = _db.NARST_Files.FromSqlRaw<NARST_File>($"sp_BATCH_PROC_Get_NARST_File @lfu_id, @hos_code, @month, @year", param1, param2, param3, param4).ToList();

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

                            Console.WriteLine("Create_M_SQLite_File : " + tmpDataumnList.Count);
                        }
                    }

                    if (tmpDataumnList.Count > 0)
                    {
                        dataTable = ListToDataTable<NARST_File>(tmpDataumnList);

                        var dataList = new List<Dictionary<string, string>>();
                        if (dataTable.Rows.Count > 0)
                        {
                            foreach (DataRow row in dataTable.Rows)
                            {
                                var dataRow = new Dictionary<string, string>();
                                string[] columnNames = dataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).Where(x => x.Contains("_STR") == false && x.Contains("_cdate") == false && x.Contains("_NE") == false).ToArray();
                                string[] columnNames_NE = dataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).Where(x => x.Contains("_NE")).ToArray();
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
                                foreach (var col in columnNames_NE)
                                {
                                    var col_NM = col.Replace("_NE", "_NM");

                                    string val;
                                    if (dataRow.TryGetValue(col_NM, out val))
                                    {
                                        if (string.IsNullOrEmpty(row[col].ToString()) == false)
                                        {
                                            dataRow[col_NM] = row[col].ToString();
                                        }
                                    }
                                }
                                dataList.Add(dataRow);
                            }
                        }

                        Console.WriteLine("ListToDataTable : Success");

                        string dataFilename = Get_DataFilename_Month(hos_code, month.Year.ToString(), month.Month.ToString("00"));

                        Console.WriteLine("Create_M_SQLite_File : " + dataFilename);

                        string filepath = Path.Combine(whonetPath, "Data", ("M_" + hos_code));
                        if (Directory.Exists(filepath) == false) Directory.CreateDirectory(filepath);

                        var datafilefullname = Path.Combine(filepath, dataFilename);
                        if (File.Exists(datafilefullname)) File.Delete(datafilefullname);

                        Console.WriteLine("Create_M_SQLite_File : CreateTable");
                        SQLiteDataAccess.CreateTable(filepath, dataFilename, dataList.FirstOrDefault().Keys.ToList());

                        Console.WriteLine("Create_M_SQLite_File : InsertData");
                        SQLiteDataAccess.InsertData(filepath, dataFilename, dataList);

                        filepath = Path.Combine(whonetPath, "Data");
                        var copyfilename = Path.Combine(filepath, dataFilename);
                        if (File.Exists(copyfilename)) File.Delete(copyfilename);
                        File.Copy(datafilefullname, copyfilename);
                    }
                }
            }

            Console.WriteLine("Create_M_SQLite_File : End");
        }

        private static DataTable ListToDataTable<T>(List<T> data)
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

        private static void Run_WHONET_NARST_ARH()
        {
            Console.WriteLine("Run_WHONET_NARST_ARH : Start");

            List<string> arh_codeList = fileUploadList.Select(x => x.pfu_arh_code).Distinct().ToList();

            foreach (var arh_code in arh_codeList)
            {
                Console.WriteLine("Run_WHONET_NARST_ARH : " + arh_code);

                var arh_fileUploadList = fileUploadList.Where(x => x.pfu_arh_code == arh_code).ToList();

                var minDate = arh_fileUploadList.OrderBy(x => x.pfu_StartDatePeriod).FirstOrDefault().pfu_StartDatePeriod.Value;
                minDate = new DateTime(minDate.Year, minDate.Month, 1);

                var maxDate = arh_fileUploadList.OrderByDescending(x => x.pfu_EndDatePeriod).FirstOrDefault().pfu_EndDatePeriod.Value;
                maxDate = new DateTime(maxDate.Year, maxDate.Month, 1);

                var m03 = new DateTime(minDate.Year, 3, 31);
                var m06 = new DateTime(minDate.Year, 6, 30);
                var m09 = new DateTime(minDate.Year, 9, 30);
                var m12 = new DateTime(minDate.Year, 12, 31);

                var arh_file = arh_fileUploadList.FirstOrDefault();

                string dataFilename03 = string.Join(",", arh_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "01")).Distinct().ToList());
                dataFilename03 = dataFilename03 + "," + string.Join(",", arh_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "02")).Distinct().ToList());
                dataFilename03 = dataFilename03 + "," + string.Join(",", arh_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "03")).Distinct().ToList());
                string dataFilename06 = dataFilename03 + "," + string.Join(",", arh_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "04")).Distinct().ToList());
                dataFilename06 = dataFilename06 + "," + string.Join(",", arh_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "05")).Distinct().ToList());
                dataFilename06 = dataFilename06 + "," + string.Join(",", arh_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "06")).Distinct().ToList());
                string dataFilename09 = dataFilename06 + string.Join(",", arh_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "07")).Distinct().ToList());
                dataFilename09 = dataFilename09 + string.Join(",", arh_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "08")).Distinct().ToList());
                dataFilename09 = dataFilename09 + string.Join(",", arh_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "09")).Distinct().ToList());
                string dataFilename12 = dataFilename09 + string.Join(",", arh_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "10")).Distinct().ToList());
                dataFilename12 = dataFilename12 + string.Join(",", arh_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "11")).Distinct().ToList());
                dataFilename12 = dataFilename12 + string.Join(",", arh_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "12")).Distinct().ToList());

                if (maxDate <= m03)
                {
                    Run_WHONET_NARST(arh_file.pfu_arh_code, null, null, null, minDate.Year.ToString(), "01", "03", dataFilename03);
                }
                else if (maxDate <= m06)
                {
                    if (minDate <= m03)
                    {
                        Run_WHONET_NARST(arh_file.pfu_arh_code, null, null, null, minDate.Year.ToString(), "01", "03", dataFilename03);
                    }
                    Run_WHONET_NARST(arh_file.pfu_arh_code, null, null, null, minDate.Year.ToString(), "01", "06", dataFilename06);
                }
                else if (maxDate <= m09)
                {
                    if (minDate <= m03)
                    {
                        Run_WHONET_NARST(arh_file.pfu_arh_code, null, null, null, minDate.Year.ToString(), "01", "03", dataFilename03);
                    }
                    if (minDate <= m06)
                    {
                        Run_WHONET_NARST(arh_file.pfu_arh_code, null, null, null, minDate.Year.ToString(), "01", "06", dataFilename06);
                    }
                    Run_WHONET_NARST(arh_file.pfu_arh_code, null, null, null, minDate.Year.ToString(), "01", "09", dataFilename09);
                }
                else if (maxDate <= m12)
                {
                    if (minDate <= m03)
                    {
                        Run_WHONET_NARST(arh_file.pfu_arh_code, null, null, null, minDate.Year.ToString(), "01", "03", dataFilename03);
                    }
                    if (minDate <= m06)
                    {
                        Run_WHONET_NARST(arh_file.pfu_arh_code, null, null, null, minDate.Year.ToString(), "01", "06", dataFilename06);
                    }
                    if (minDate <= m09)
                    {
                        Run_WHONET_NARST(arh_file.pfu_arh_code, null, null, null, minDate.Year.ToString(), "01", "09", dataFilename09);
                    }
                    Run_WHONET_NARST(arh_file.pfu_arh_code, null, null, null, minDate.Year.ToString(), "01", "12", dataFilename12);
                }

            }

            Console.WriteLine("Run_WHONET_NARST_ARH : END");
        }

        private static void Run_WHONET_NARST_PRV()
        {
            Console.WriteLine("Run_WHONET_NARST_PRV : Start");

            List<string> prv_codeList = fileUploadList.Select(x => x.pfu_prv_code).Distinct().ToList();

            foreach (var prv_code in prv_codeList)
            {
                Console.WriteLine("Run_WHONET_NARST_PRV : " + prv_code);

                var prv_fileUploadList = fileUploadList.Where(x => x.pfu_prv_code == prv_code).ToList();

                var minDate = prv_fileUploadList.OrderBy(x => x.pfu_StartDatePeriod).FirstOrDefault().pfu_StartDatePeriod.Value;
                minDate = new DateTime(minDate.Year, minDate.Month, 1);

                var maxDate = prv_fileUploadList.OrderByDescending(x => x.pfu_EndDatePeriod).FirstOrDefault().pfu_EndDatePeriod.Value;
                maxDate = new DateTime(maxDate.Year, maxDate.Month, 1);

                var m03 = new DateTime(minDate.Year, 3, 31);
                var m06 = new DateTime(minDate.Year, 6, 30);
                var m09 = new DateTime(minDate.Year, 9, 30);
                var m12 = new DateTime(minDate.Year, 12, 31);

                var prv_file = prv_fileUploadList.FirstOrDefault();

                string dataFilename03 = string.Join(",", prv_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "01")).Distinct().ToList());
                dataFilename03 = dataFilename03 + "," + string.Join(",", prv_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "02")).Distinct().ToList());
                dataFilename03 = dataFilename03 + "," + string.Join(",", prv_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "03")).Distinct().ToList());
                string dataFilename06 = dataFilename03 + "," + string.Join(",", prv_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "04")).Distinct().ToList());
                dataFilename06 = dataFilename06 + "," + string.Join(",", prv_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "05")).Distinct().ToList());
                dataFilename06 = dataFilename06 + "," + string.Join(",", prv_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "06")).Distinct().ToList());
                string dataFilename09 = dataFilename06 + string.Join(",", prv_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "07")).Distinct().ToList());
                dataFilename09 = dataFilename09 + string.Join(",", prv_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "08")).Distinct().ToList());
                dataFilename09 = dataFilename09 + string.Join(",", prv_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "09")).Distinct().ToList());
                string dataFilename12 = dataFilename09 + string.Join(",", prv_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "10")).Distinct().ToList());
                dataFilename12 = dataFilename12 + string.Join(",", prv_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "11")).Distinct().ToList());
                dataFilename12 = dataFilename12 + string.Join(",", prv_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "12")).Distinct().ToList());

                if (maxDate <= m03)
                {
                    Run_WHONET_NARST(prv_file.pfu_arh_code, prv_file.pfu_prv_code, null, null, minDate.Year.ToString(), "01", "03", dataFilename03);
                }
                else if (maxDate <= m06)
                {
                    if (minDate <= m03)
                    {
                        Run_WHONET_NARST(prv_file.pfu_arh_code, prv_file.pfu_prv_code, null, null, minDate.Year.ToString(), "01", "03", dataFilename03);
                    }
                    Run_WHONET_NARST(prv_file.pfu_arh_code, prv_file.pfu_prv_code, null, null, minDate.Year.ToString(), "01", "06", dataFilename06);
                }
                else if (maxDate <= m09)
                {
                    if (minDate <= m03)
                    {
                        Run_WHONET_NARST(prv_file.pfu_arh_code, prv_file.pfu_prv_code, null, null, minDate.Year.ToString(), "01", "03", dataFilename03);
                    }
                    if (minDate <= m06)
                    {
                        Run_WHONET_NARST(prv_file.pfu_arh_code, prv_file.pfu_prv_code, null, null, minDate.Year.ToString(), "01", "06", dataFilename06);
                    }
                    Run_WHONET_NARST(prv_file.pfu_arh_code, prv_file.pfu_prv_code, null, null, minDate.Year.ToString(), "01", "09", dataFilename09);
                }
                else if (maxDate <= m12)
                {
                    if (minDate <= m03)
                    {
                        Run_WHONET_NARST(prv_file.pfu_arh_code, prv_file.pfu_prv_code, null, null, minDate.Year.ToString(), "01", "03", dataFilename03);
                    }
                    if (minDate <= m06)
                    {
                        Run_WHONET_NARST(prv_file.pfu_arh_code, prv_file.pfu_prv_code, null, null, minDate.Year.ToString(), "01", "06", dataFilename06);
                    }
                    if (minDate <= m09)
                    {
                        Run_WHONET_NARST(prv_file.pfu_arh_code, prv_file.pfu_prv_code, null, null, minDate.Year.ToString(), "01", "09", dataFilename09);
                    }
                    Run_WHONET_NARST(prv_file.pfu_arh_code, prv_file.pfu_prv_code, null, null, minDate.Year.ToString(), "01", "12", dataFilename12);
                }
            }

            Console.WriteLine("Run_WHONET_NARST_PRV : END");
        }

        private static void Run_WHONET_NARST_HOS()
        {
            Console.WriteLine("Run_WHONET_NARST_HOS : Start");

            List<string> hos_codeList = fileUploadList.Select(x => x.pfu_hos_code).Distinct().ToList();

            foreach (var hos_code in hos_codeList)
            {
                Console.WriteLine("Run_WHONET_NARST_HOS : " + hos_code);

                var hos_fileUploadList = fileUploadList.Where(x => x.pfu_hos_code == hos_code).ToList();

                var minDate = hos_fileUploadList.OrderBy(x => x.pfu_StartDatePeriod).FirstOrDefault().pfu_StartDatePeriod.Value;
                minDate = new DateTime(minDate.Year, minDate.Month, 1);

                var maxDate = hos_fileUploadList.OrderByDescending(x => x.pfu_EndDatePeriod).FirstOrDefault().pfu_EndDatePeriod.Value;
                maxDate = new DateTime(maxDate.Year, maxDate.Month, 1);

                var m03 = new DateTime(minDate.Year, 3, 31);
                var m06 = new DateTime(minDate.Year, 6, 30);
                var m09 = new DateTime(minDate.Year, 9, 30);
                var m12 = new DateTime(minDate.Year, 12, 31);

                var hos_file = hos_fileUploadList.FirstOrDefault();

                string dataFilename03 = string.Join(",", hos_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "01")).Distinct().ToList());
                dataFilename03 = dataFilename03 + "," + string.Join(",", hos_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "02")).Distinct().ToList());
                dataFilename03 = dataFilename03 + "," + string.Join(",", hos_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "03")).Distinct().ToList());
                string dataFilename06 = dataFilename03 + "," + string.Join(",", hos_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "04")).Distinct().ToList());
                dataFilename06 = dataFilename06 + "," + string.Join(",", hos_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "05")).Distinct().ToList());
                dataFilename06 = dataFilename06 + "," + string.Join(",", hos_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "06")).Distinct().ToList());
                string dataFilename09 = dataFilename06 + string.Join(",", hos_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "07")).Distinct().ToList());
                dataFilename09 = dataFilename09 + string.Join(",", hos_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "08")).Distinct().ToList());
                dataFilename09 = dataFilename09 + string.Join(",", hos_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "09")).Distinct().ToList());
                string dataFilename12 = dataFilename09 + string.Join(",", hos_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "10")).Distinct().ToList());
                dataFilename12 = dataFilename12 + string.Join(",", hos_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "11")).Distinct().ToList());
                dataFilename12 = dataFilename12 + string.Join(",", hos_fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "12")).Distinct().ToList());

                if (maxDate <= m03)
                {
                    Run_WHONET_NARST(hos_file.pfu_arh_code, hos_file.pfu_prv_code, hos_file.pfu_hos_code, hos_file.pfu_lab, minDate.Year.ToString(), "01", "03", dataFilename03);
                }
                else if (maxDate <= m06)
                {
                    if (minDate <= m03)
                    {
                        Run_WHONET_NARST(hos_file.pfu_arh_code, hos_file.pfu_prv_code, hos_file.pfu_hos_code, hos_file.pfu_lab, minDate.Year.ToString(), "01", "03", dataFilename03);
                    }
                    Run_WHONET_NARST(hos_file.pfu_arh_code, hos_file.pfu_prv_code, hos_file.pfu_hos_code, hos_file.pfu_lab, minDate.Year.ToString(), "01", "06", dataFilename06);
                }
                else if (maxDate <= m09)
                {
                    if (minDate <= m03)
                    {
                        Run_WHONET_NARST(hos_file.pfu_arh_code, hos_file.pfu_prv_code, hos_file.pfu_hos_code, hos_file.pfu_lab, minDate.Year.ToString(), "01", "03", dataFilename03);
                    }
                    if (minDate <= m06)
                    {
                        Run_WHONET_NARST(hos_file.pfu_arh_code, hos_file.pfu_prv_code, hos_file.pfu_hos_code, hos_file.pfu_lab, minDate.Year.ToString(), "01", "06", dataFilename06);
                    }
                    Run_WHONET_NARST(hos_file.pfu_arh_code, hos_file.pfu_prv_code, hos_file.pfu_hos_code, hos_file.pfu_lab, minDate.Year.ToString(), "01", "09", dataFilename09);
                }
                else if (maxDate <= m12)
                {
                    if (minDate <= m03)
                    {
                        Run_WHONET_NARST(hos_file.pfu_arh_code, hos_file.pfu_prv_code, hos_file.pfu_hos_code, hos_file.pfu_lab, minDate.Year.ToString(), "01", "03", dataFilename03);
                    }
                    if (minDate <= m06)
                    {
                        Run_WHONET_NARST(hos_file.pfu_arh_code, hos_file.pfu_prv_code, hos_file.pfu_hos_code, hos_file.pfu_lab, minDate.Year.ToString(), "01", "06", dataFilename06);
                    }
                    if (minDate <= m09)
                    {
                        Run_WHONET_NARST(hos_file.pfu_arh_code, hos_file.pfu_prv_code, hos_file.pfu_hos_code, hos_file.pfu_lab, minDate.Year.ToString(), "01", "09", dataFilename09);
                    }
                    Run_WHONET_NARST(hos_file.pfu_arh_code, hos_file.pfu_prv_code, hos_file.pfu_hos_code, hos_file.pfu_lab, minDate.Year.ToString(), "01", "12", dataFilename12);
                }
            }

            Console.WriteLine("Run_WHONET_NARST_HOS : END");
        }

        private static void Run_WHONET_NARST_NATION()
        {
            Console.WriteLine("Run_WHONET_NARST_NATION : Start");

            var minDate = DateTime.Now;

            string dataFilename03 = string.Join(",", fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "01")).Distinct().ToList());
            dataFilename03 = dataFilename03 + "," + string.Join(",", fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "02")).Distinct().ToList());
            dataFilename03 = dataFilename03 + "," + string.Join(",", fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "03")).Distinct().ToList());
            string dataFilename06 = dataFilename03 + "," + string.Join(",", fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "04")).Distinct().ToList());
            dataFilename06 = dataFilename06 + "," + string.Join(",", fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "05")).Distinct().ToList());
            dataFilename06 = dataFilename06 + "," + string.Join(",", fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "06")).Distinct().ToList());
            string dataFilename09 = dataFilename06 + string.Join(",", fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "07")).Distinct().ToList());
            dataFilename09 = dataFilename09 + string.Join(",", fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "08")).Distinct().ToList());
            dataFilename09 = dataFilename09 + string.Join(",", fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "09")).Distinct().ToList());
            string dataFilename12 = dataFilename09 + string.Join(",", fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "10")).Distinct().ToList());
            dataFilename12 = dataFilename12 + string.Join(",", fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "11")).Distinct().ToList());
            dataFilename12 = dataFilename12 + string.Join(",", fileUploadList.Select(x => Get_DataFilename_Month(x.pfu_hos_code, minDate.Year.ToString(), "12")).Distinct().ToList());

            Run_WHONET_NARST(null, null, null, null, DateTime.Now.Year.ToString(), "01", "03", dataFilename03);
            Run_WHONET_NARST(null, null, null, null, DateTime.Now.Year.ToString(), "01", "06", dataFilename06);
            //Run_WHONET_NARST(null, null, null, null, DateTime.Now.Year.ToString(), "01", "09", dataFilename09);
            //Run_WHONET_NARST(null, null, null, null, DateTime.Now.Year.ToString(), "01", "12", dataFilename12);

            Console.WriteLine("Run_WHONET_NARST_NATION : END");
        }

        private static void Run_WHONET_NARST(string arh_code, string prv_code, string hos_code, string lab_code, string year, string month_start, string month_end, string dataFilename)
        {
            var processName = Get_ProcessName(arh_code, prv_code, hos_code, year, month_start, month_end);

            Create_ProcessRequest(processName, arh_code, prv_code, hos_code, lab_code, year, month_start, month_end);

            Run_WHONET_NARST_Command(processName, dataFilename);
        }

        private static void Create_ProcessRequest(string processName, string arh_code, string prv_code, string hos_code, string lab_code, string year, string month_start, string month_end)
        {
            Console.WriteLine("Create_ProcessRequest : START");

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        var processData = new TRProcessRequest();

                        if (string.IsNullOrEmpty(hos_code) == false)
                        {
                            processData = _db.TRProcessRequests.FirstOrDefault(x => x.pcr_hos_code == hos_code && x.pcr_year == year && x.pcr_month_start == month_start && x.pcr_month_end == month_end);
                        }
                        else if (string.IsNullOrEmpty(prv_code) == false)
                        {
                            processData = _db.TRProcessRequests.FirstOrDefault(x => x.pcr_prv_code == prv_code && string.IsNullOrEmpty(x.pcr_hos_code) && x.pcr_year == year && x.pcr_month_start == month_start && x.pcr_month_end == month_end);
                        }
                        else if (string.IsNullOrEmpty(arh_code) == false)
                        {
                            processData = _db.TRProcessRequests.FirstOrDefault(x => x.pcr_arh_code == arh_code && string.IsNullOrEmpty(x.pcr_prv_code) && string.IsNullOrEmpty(x.pcr_hos_code) && x.pcr_year == year && x.pcr_month_start == month_start && x.pcr_month_end == month_end);
                        }
                        else if (string.IsNullOrEmpty(arh_code))
                        {
                            processData = _db.TRProcessRequests.FirstOrDefault(x => string.IsNullOrEmpty(x.pcr_arh_code) && string.IsNullOrEmpty(x.pcr_prv_code) && string.IsNullOrEmpty(x.pcr_hos_code) && x.pcr_year == year && x.pcr_month_start == month_start && x.pcr_month_end == month_end);
                        }

                        if (processData == null)
                        {
                            var pcr_code_new = "";

                            Console.WriteLine("Create_Process : " + pcr_code_pattern());

                            var lastProcessDataList = _db.TRProcessRequests.Where(x => x.pcr_code.Contains(pcr_code_pattern())).ToList();
                            if (lastProcessDataList != null && lastProcessDataList.Count > 0)
                            {
                                var lastProcessData = lastProcessDataList.OrderByDescending(x => x.pcr_code).FirstOrDefault();
                                var pcr_code_last = lastProcessData.pcr_code;
                                var code_running_next = (Convert.ToInt32(pcr_code_last.Replace(pcr_code_pattern(), "")) + 1).ToString("0000");

                                pcr_code_new = pcr_code_pattern() + code_running_next;
                            }
                            else
                            {
                                pcr_code_new = pcr_code_pattern() + "0001";
                            }

                            Console.WriteLine("Create_ProcessRequest : CREATE NEW " + pcr_code_new);

                            processData = new TRProcessRequest()
                            {
                                pcr_code = pcr_code_new,
                                pcr_arh_code = arh_code,
                                pcr_prv_code = prv_code,
                                pcr_hos_code = hos_code,
                                pcr_lab_code = lab_code,
                                pcr_type = "01",
                                pcr_month_start = month_start,
                                pcr_month_end = month_end,
                                pcr_year = year,
                                pcr_filename = processName + "-NARST.xlsx",
                                pcr_filepath = "/NARST/" + pcr_code_new,
                                pcr_filetype = "application/vnd.ms-excel",
                                pcr_status = "A",
                                pcr_active = true,
                                pcr_createuser = "BATCH",
                                pcr_createdate = DateTime.Now
                            };

                            _db.TRProcessRequests.Add(processData);
                            _db.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine("Create_ProcessRequest : Exist " + processData.pcr_id);
                        }

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
            Console.WriteLine("Create_ProcessRequest : END");
        }

        private static void Run_WHONET_NARST_Command(string processName, string dataFilename)
        {
            Console.WriteLine("Run_WHONET_NARST_Command : Start : " + processName);

            var currentYear = DateTime.Now.Year.ToString();
            if (DateTime.Now.Month < 4)
            {
                currentYear = (DateTime.Now.Year - 1).ToString();
            }

            string[] filePaths = Directory.GetFiles(Path.Combine(whonetPath, "Macros", "Template"), "*.mcr");

            ProcessStartInfo processInfo;
            System.Diagnostics.Process process;
            string command;

            int exitCode;
            string output;
            string error;

            string macroBackupPath = Path.Combine(whonetPath, "Macros", processName);
            if (Directory.Exists(macroBackupPath) == false)
            {
                Directory.CreateDirectory(macroBackupPath);
            }
            else
            {
                Directory.GetFiles(macroBackupPath).ToList().ForEach(macroFile => File.Delete(macroFile));
            }

            string outputBackupPath = Path.Combine(whonetPath, "Output", processName);
            if (Directory.Exists(outputBackupPath) == false)
            {
                Directory.CreateDirectory(outputBackupPath);
            }
            else
            {
                Directory.GetFiles(outputBackupPath).ToList().ForEach(outputFile => File.Delete(outputFile));
            }

            foreach (var macroFile in filePaths)
            {
                //string macrofilename = Path.Combine(whonetPath, "macros", "macrotemplate.mcr");
                //string newmacroname = $"macro-{item.pcr_code}";
                //string newmacrofilename = macrofilename.Replace("macrotemplate", newmacroname);

                string newmacroFilename = macroFile.Replace("Template\\", processName + "_");
                string newmacroName = newmacroFilename.Replace(Path.Combine(whonetPath, "Macros"), "").Replace("\\", "").Replace(".mcr", "");

                //string dataFilename = string.Join(",", fileUploadList.Where(x => x.pfu_hos_code == item.pfu_hos_code).Select(x => $"M_{item.pfu_hos_code}_{x.pfu_id}.sqlite"));
                //string ouputFilename = $"O_{newmacroName}.db";

                if (File.Exists(newmacroFilename)) File.Delete(newmacroFilename);
                File.Copy(macroFile, newmacroFilename);
                //string text = File.ReadAllText(newmacrofilename);
                //text = text.Replace("{MacroName}", newmacroname);
                //text = text.Replace("{Datafile}", datafilename);
                //text = text.Replace("{Output}", "SQLITE File (" + ouputfilename + ")");
                //File.WriteAllText(newmacrofilename, text);

                string[] message = File.ReadAllLines(newmacroFilename);
                message[0] = $"Macro Name = {newmacroName}";
                File.WriteAllLines(newmacroFilename, message);
                string appendText = Environment.NewLine + $"Data file = {dataFilename}";
                File.AppendAllText(newmacroFilename, appendText, System.Text.Encoding.UTF8);
                appendText = Environment.NewLine + $"Output = SQLITE File (\"{ouputFilename(newmacroName)}\")";
                File.AppendAllText(newmacroFilename, appendText, System.Text.Encoding.UTF8);

                command = Path.Combine(whonetPath, "WHONET.EXE") + $" \"{newmacroName}.mcr\"";
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

                if (ouputFilename(newmacroName).Contains(","))
                {
                    var tmpfilename = ouputFilename(newmacroName).Split(',')[0];
                    if (File.Exists(tmpfilename)) File.Move(tmpfilename, ouputFilename(newmacroName));
                }

                string macroBackupFile = Path.Combine(macroBackupPath, $"{newmacroName}.mcr");
                File.Move(newmacroFilename, macroBackupFile);
                Console.WriteLine($"move macroBackupFile {newmacroName}.mcr To macroBackupPath");

                string outputFile = Path.Combine(whonetPath, "Output", ouputFilename(newmacroName));
                string outputBackupFile = Path.Combine(outputBackupPath, ouputFilename(newmacroName));
                if (File.Exists(outputFile))
                {
                    File.Move(outputFile, outputBackupFile);
                    Console.WriteLine($"move outputBackupFile {ouputFilename(newmacroName)} To outputBackupPath");
                }
                else
                {
                    Console.WriteLine($"File not found - {ouputFilename(newmacroName)}");
                }
            }

            Console.WriteLine("Run_WHONET_NARST_Command : End");
        }

        private static void Create_WHONET_Data_Tmp()
        {
            Console.WriteLine("p03_Create_WHONET_Data : Start");

            foreach (var item in processRequestList)
            {
                foreach (var itemDetail in item.processRequestDetails)
                {
                    var processDataList = new List<TRSTGProcessDataHeader>();
                    var processDataDetailList = new List<TRSTGProcessDataDetail>();
                    var whonetFieldList = new Dictionary<string, string>();

                    using (var _db = new ProcessContext())
                    {
                        using (var trans = _db.Database.BeginTransaction())
                        {
                            try
                            {
                                processDataList = _db.TRSTGProcessDataHeaders.Where(x => x.pdh_hos_code == itemDetail.pcd_hos_code && x.pdh_lab == itemDetail.pcd_lab_code).ToList();
                                processDataDetailList = _db.TRSTGProcessDataDetails.Where(x => x.pdd_pcr_code == item.pcr_code).ToList();



                                var tmpWhonetColumnList = _db.TCWHONETColumns.FromSqlRaw<TCWHONETColumn>("sp_GET_TCWHONETColumn_Active @wnc_mst_code = NULL").ToList();
                                tmpWhonetColumnList.ForEach(x =>
                                {
                                    whonetFieldList.Add(x.wnc_code, x.wnc_name);
                                });

                                var tmpWhonetAntibioticColumnList = _db.TCWHONET_AntibioticsModel.FromSqlRaw<TCWHONET_Antibiotics>("sp_GET_TCWHONET_Antibiotics_Active").ToList();
                                tmpWhonetAntibioticColumnList.ForEach(x =>
                                {
                                    whonetFieldList.Add(x.who_ant_code, x.who_ant_name);
                                });

                                //var tmpWhonetFieldList = processDataDetailList.Select(x => x.pdd_whonetfield).Distinct().ToList();
                                //tmpWhonetFieldList.ForEach(x =>
                                //{
                                //    if (whonetFieldList.ContainsKey(x) == false && whonetFieldList.ContainsValue(x) == false)
                                //    {
                                //        whonetFieldList.Add(x, x);
                                //    }
                                //});

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

                    string datafilename = $"{item.pcr_code}-{itemDetail.pcd_hos_code}-{itemDetail.pcd_lab_code}.sqlite";

                    string filepath = Path.Combine(whonetPath, "Data", item.pcr_code);
                    if (Directory.Exists(filepath) == false) Directory.CreateDirectory(filepath);

                    var datafilefullname = Path.Combine(filepath, datafilename);
                    if (File.Exists(datafilefullname) == false) File.Delete(datafilefullname);

                    SQLiteDataAccess.CreateTable(filepath, datafilename, whonetFieldList.Keys.ToList());

                    var dataDicList = new List<Dictionary<string, string>>();

                    foreach (var itemData in processDataList)
                    {
                        var dataList = new Dictionary<string, string>();

                        var dataDetailList = processDataDetailList.Where(x => x.pdd_ldh_id == itemData.pdh_id).ToList();

                        foreach (var itemDataDetail in dataDetailList)
                        {
                            if (whonetFieldList.Any(x => x.Key == itemDataDetail.pdd_whonetfield || x.Value == itemDataDetail.pdd_whonetfield))
                            {
                                var whonetColumn = whonetFieldList.FirstOrDefault(x => x.Key == itemDataDetail.pdd_whonetfield || x.Value == itemDataDetail.pdd_whonetfield).Key;
                                if (dataList.ContainsKey(whonetColumn) == false)
                                {
                                    var fieldValue = itemDataDetail.pdd_mappingvalue ?? itemDataDetail.pdd_originalvalue;
                                    if (whonetColumn.Contains("DATE"))
                                    {
                                        string format1 = "M/d/yyyy H:mm:ss";
                                        string format2 = "M/d/yyyy H:mm:ss tt";
                                        string format3 = "M/d/yyyy H:mm:sss";
                                        string format4 = "M/d/yyyy H:mm:sss tt";
                                        string format5 = "M/d/yyyy";
                                        DateTime tmpFieldValue;
                                        if (DateTime.TryParseExact(fieldValue, format1, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                        {
                                            fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                        }
                                        else if (DateTime.TryParseExact(fieldValue, format2, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                        {
                                            fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                        }
                                        else if (DateTime.TryParseExact(fieldValue, format3, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                        {
                                            fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                        }
                                        else if (DateTime.TryParseExact(fieldValue, format4, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                        {
                                            fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                        }
                                        else if (DateTime.TryParseExact(fieldValue.Split(' ')[0], format5, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                        {
                                            fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                        }
                                        else
                                        {
                                            fieldValue = null;
                                        }
                                    }

                                    dataList.Add(whonetColumn, fieldValue);
                                }
                            }
                        }

                        if (dataList.Any(x => x.Key == "COUNTRY_A") == false)
                        {
                            dataList.Add("COUNTRY_A", "THA");
                        }
                        if (dataList.Any(x => x.Key == "LABORATORY") == false)
                        {
                            dataList.Add("LABORATORY", "H19");
                        }
                        if (dataList.Any(x => x.Key == "ORIGIN") == false)
                        {
                            dataList.Add("ORIGIN", "");
                        }
                        if (dataList.Any(x => x.Key == "INSTITUT") == false)
                        {
                            dataList.Add("INSTITUT", (itemDetail.pcd_hos_code + "." + itemDetail.pcd_lab_code));
                        }
                        if (dataList.Any(x => x.Key == "PATIENT_ID" || x.Value == "PATIENT_ID") == false)
                        {
                            dataList.Add("PATIENT_ID", itemData.pdh_labno);
                        }

                        dataDicList.Add(dataList);
                    }

                    SQLiteDataAccess.InsertData(filepath, datafilename, dataDicList);

                    filepath = Path.Combine(whonetPath, "Data");
                    var copyfilename = Path.Combine(filepath, datafilename);
                    if (File.Exists(copyfilename)) File.Delete(copyfilename);
                    File.Copy(datafilefullname, copyfilename);
                }
            }

            Console.WriteLine("p03_Create_WHONET_Data : End");
        }

        private static void Create_WHONET_Data()
        {
            Console.WriteLine("p03_Create_WHONET_Data : Start");

            foreach (var item in processRequestList)
            {
                foreach (var itemDetail in item.processRequestDetails)
                {
                    DataTable dataTable = new DataTable();

                    var processDataList = new List<TRSTGProcessDataHeader>();
                    var processDataDetailList = new List<TRSTGProcessDataDetail>();
                    var whonetFieldList = new Dictionary<string, string>();

                    using (var _db = new ProcessContext())
                    {
                        //using (var trans = _db.Database.BeginTransaction())
                        //{
                        try
                        {
                            DbConnection connection = _db.Database.GetDbConnection();
                            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                            using (var cmd = dbFactory.CreateCommand())
                            {
                                cmd.Connection = connection;
                                cmd.CommandTimeout = 0;
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandText = "EXEC sp_GET_TRProcessDataList @lfu_id = @p_lfu_id";
                                cmd.Parameters.Add(new SqlParameter("p_lfu_id", SqlDbType.VarChar) { Value = itemDetail.pcd_hos_code });

                                using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                                {
                                    adapter.SelectCommand = cmd;
                                    adapter.Fill(dataTable);
                                }
                            }

                            //processDataList = _db.TRProcessDataHeaders.Where(x => x.pdh_pcr_code == item.pcr_code && x.pdh_hos_code == itemDetail.pcd_hos_code && x.pdh_lab == itemDetail.pcd_lab_code).ToList();
                            //processDataDetailList = _db.TRProcessDataDetails.Where(x => x.pdd_pcr_code == item.pcr_code).ToList();



                            //var tmpWhonetColumnList = _db.TCWHONETColumns.FromSqlRaw<TCWHONETColumn>("sp_GET_TCWHONETColumn_Active @wnc_mst_code = NULL").ToList();
                            //tmpWhonetColumnList.ForEach(x =>
                            //{
                            //    whonetFieldList.Add(x.wnc_code, x.wnc_name);
                            //});

                            //var tmpWhonetAntibioticColumnList = _db.TCWHONET_AntibioticsModel.FromSqlRaw<TCWHONET_Antibiotics>("sp_GET_TCWHONET_Antibiotics_Active").ToList();
                            //tmpWhonetAntibioticColumnList.ForEach(x =>
                            //{
                            //    whonetFieldList.Add(x.who_ant_code, x.who_ant_name);
                            //});

                            //var tmpWhonetFieldList = processDataDetailList.Select(x => x.pdd_whonetfield).Distinct().ToList();
                            //tmpWhonetFieldList.ForEach(x =>
                            //{
                            //    if (whonetFieldList.ContainsKey(x) == false && whonetFieldList.ContainsValue(x) == false)
                            //    {
                            //        whonetFieldList.Add(x, x);
                            //    }
                            //});

                            //trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            //trans.Rollback();
                        }
                        finally
                        {
                            //trans.Dispose();
                            _db.Dispose();
                        }
                        //}
                    }

                    //        string datafilename = $"{item.pcr_code}-{itemDetail.pcd_hos_code}-{itemDetail.pcd_lab_code}.sqlite";

                    //        string filepath = Path.Combine(whonetPath, "Data", item.pcr_code);
                    //        if (Directory.Exists(filepath) == false) Directory.CreateDirectory(filepath);

                    //        var datafilefullname = Path.Combine(filepath, datafilename);
                    //        if (File.Exists(datafilefullname) == false) File.Delete(datafilefullname);

                    //        SQLiteDataAccess.CreateTable(filepath, datafilename, whonetFieldList.Keys.ToList());

                    //        var dataDicList = new List<Dictionary<string, string>>();

                    //        foreach (var itemData in processDataList)
                    //        {
                    //            var dataList = new Dictionary<string, string>();

                    //            var dataDetailList = processDataDetailList.Where(x => x.pdd_ldh_id == itemData.pdh_id).ToList();

                    //            foreach (var itemDataDetail in dataDetailList)
                    //            {
                    //                if (whonetFieldList.Any(x => x.Key == itemDataDetail.pdd_whonetfield || x.Value == itemDataDetail.pdd_whonetfield))
                    //                {
                    //                    var whonetColumn = whonetFieldList.FirstOrDefault(x => x.Key == itemDataDetail.pdd_whonetfield || x.Value == itemDataDetail.pdd_whonetfield).Key;
                    //                    if (dataList.ContainsKey(whonetColumn) == false)
                    //                    {
                    //                        var fieldValue = itemDataDetail.pdd_mappingvalue ?? itemDataDetail.pdd_originalvalue;
                    //                        if (whonetColumn.Contains("DATE"))
                    //                        {
                    //                            string format1 = "M/d/yyyy H:mm:ss";
                    //                            string format2 = "M/d/yyyy H:mm:ss tt";
                    //                            string format3 = "M/d/yyyy H:mm:sss";
                    //                            string format4 = "M/d/yyyy H:mm:sss tt";
                    //                            string format5 = "M/d/yyyy";
                    //                            DateTime tmpFieldValue;
                    //                            if (DateTime.TryParseExact(fieldValue, format1, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    //                            {
                    //                                fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    //                            }
                    //                            else if (DateTime.TryParseExact(fieldValue, format2, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    //                            {
                    //                                fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    //                            }
                    //                            else if (DateTime.TryParseExact(fieldValue, format3, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    //                            {
                    //                                fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    //                            }
                    //                            else if (DateTime.TryParseExact(fieldValue, format4, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    //                            {
                    //                                fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    //                            }
                    //                            else if (DateTime.TryParseExact(fieldValue.Split(' ')[0], format5, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    //                            {
                    //                                fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    //                            }
                    //                            else
                    //                            {
                    //                                fieldValue = null;
                    //                            }
                    //                        }

                    //                        dataList.Add(whonetColumn, fieldValue);
                    //                    }
                    //                }
                    //            }

                    //            if (dataList.Any(x => x.Key == "COUNTRY_A") == false)
                    //            {
                    //                dataList.Add("COUNTRY_A", "THA");
                    //            }
                    //            if (dataList.Any(x => x.Key == "LABORATORY") == false)
                    //            {
                    //                dataList.Add("LABORATORY", "H19");
                    //            }
                    //            if (dataList.Any(x => x.Key == "ORIGIN") == false)
                    //            {
                    //                dataList.Add("ORIGIN", "");
                    //            }
                    //            if (dataList.Any(x => x.Key == "INSTITUT") == false)
                    //            {
                    //                dataList.Add("INSTITUT", (itemDetail.pcd_hos_code + "." + itemDetail.pcd_lab_code));
                    //            }
                    //            if (dataList.Any(x => x.Key == "PATIENT_ID" || x.Value == "PATIENT_ID") == false)
                    //            {
                    //                dataList.Add("PATIENT_ID", itemData.pdh_labno);
                    //            }

                    //            dataDicList.Add(dataList);
                    //        }

                    //        SQLiteDataAccess.InsertData(filepath, datafilename, dataDicList);

                    //        filepath = Path.Combine(whonetPath, "Data");
                    //        var copyfilename = Path.Combine(filepath, datafilename);
                    //        if (File.Exists(copyfilename)) File.Delete(copyfilename);
                    //        File.Copy(datafilefullname, copyfilename);
                    //    }
                    //}

                    foreach (DataColumn dc in dataTable.Columns)
                    {
                        whonetFieldList.Add(dc.ColumnName, dc.ColumnName);
                    }
                    string datafilename = $"{item.pcr_code}-{itemDetail.pcd_hos_code}-{itemDetail.pcd_lab_code}.sqlite";

                    string filepath = Path.Combine(whonetPath, "Data", item.pcr_code);
                    if (Directory.Exists(filepath) == false) Directory.CreateDirectory(filepath);

                    var datafilefullname = Path.Combine(filepath, datafilename);
                    if (File.Exists(datafilefullname) == false) File.Delete(datafilefullname);

                    SQLiteDataAccess.CreateTable(filepath, datafilename, whonetFieldList.Keys.ToList());

                    var dataDicList = new List<Dictionary<string, string>>();

                    foreach (DataRow dr in dataTable.Rows)
                    {
                        var dataList = new Dictionary<string, string>();

                        foreach (DataColumn dc in dataTable.Columns)
                        {
                            var whonetColumn = dc.ColumnName;
                            var fieldValue = (string.IsNullOrEmpty(dr[dc].ToString()) == false) ? dr[dc].ToString() : null;

                            if (whonetColumn.Contains("DATE") && fieldValue != null)
                            {
                                string format1 = "M/d/yyyy H:mm:ss";
                                string format2 = "M/d/yyyy H:mm:ss tt";
                                string format3 = "M/d/yyyy H:mm:sss";
                                string format4 = "M/d/yyyy H:mm:sss tt";
                                string format5 = "M/d/yyyy";
                                DateTime tmpFieldValue;
                                if (DateTime.TryParseExact(fieldValue, format1, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                {
                                    fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                }
                                else if (DateTime.TryParseExact(fieldValue, format2, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                {
                                    fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                }
                                else if (DateTime.TryParseExact(fieldValue, format3, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                {
                                    fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                }
                                else if (DateTime.TryParseExact(fieldValue, format4, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                {
                                    fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                }
                                else if (DateTime.TryParseExact(fieldValue.Split(' ')[0], format5, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                                {
                                    fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                                }
                                else
                                {
                                    fieldValue = null;
                                }
                            }

                            dataList.Add(whonetColumn, fieldValue);
                        }

                        //dataList["COUNTRY_A"] = dataList["pla_ldh_id"];

                        dataDicList.Add(dataList);
                    }

                    SQLiteDataAccess.InsertData(filepath, datafilename, dataDicList);

                    filepath = Path.Combine(whonetPath, "Data");
                    var copyfilename = Path.Combine(filepath, datafilename);
                    if (File.Exists(copyfilename)) File.Delete(copyfilename);
                    File.Copy(datafilefullname, copyfilename);

                }
            }
            Console.WriteLine("p03_Create_WHONET_Data : End");
        }

        private static void LabAlert_Create_WHONET_Data()
        {
            Console.WriteLine("p0101_LabAlert_Create_WHONET_Data : Start");

            DataTable dataTable = new DataTable();

            var processLabFileList = new List<TRProcessLabFileModel>();
            //filelist.Add("583E25A9-C75F-4E89-9B2D-C6161BC2DC60");
            //filelist.Add("6E23192E-33CC-4F4E-8473-15466DF414F5");
            //filelist.Add("6E35FCE8-FCF5-4D22-83FA-169FF6A8289C");

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        processLabFileList = _db.TRProcessLabFile.Where(x => x.plf_status == "A").ToList();

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

            foreach (var item in processLabFileList)
            {
                Console.WriteLine(item.plf_lfu_id.ToString());

                var whonetFieldList = new Dictionary<string, string>();

                using (var _db = new ProcessContext())
                {
                    try
                    {
                        DbConnection connection = _db.Database.GetDbConnection();
                        DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                        using (var cmd = dbFactory.CreateCommand())
                        {
                            cmd.Connection = connection;
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "EXEC sp_GET_TRProcessLabDataList @lfu_id = @p_lfu_id";
                            cmd.Parameters.Add(new SqlParameter("p_lfu_id", SqlDbType.UniqueIdentifier) { Value = item.plf_lfu_id });

                            using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                            {
                                adapter.SelectCommand = cmd;
                                adapter.Fill(dataTable);
                            }
                        }

                        //trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        //trans.Rollback();
                    }
                    finally
                    {
                        //trans.Dispose();
                        _db.Dispose();
                    }
                }

                foreach (DataColumn dc in dataTable.Columns)
                {
                    whonetFieldList.Add(dc.ColumnName, dc.ColumnName);
                }
                string datafilename = $"{item.plf_lfu_id.ToString().ToUpper()}-LabAlertData.sqlite";

                string filepath = Path.Combine(whonetPath, "Data", "LabAlert");
                if (Directory.Exists(filepath) == false) Directory.CreateDirectory(filepath);

                var datafilefullname = Path.Combine(filepath, datafilename);
                if (File.Exists(datafilefullname) == false) File.Delete(datafilefullname);

                SQLiteDataAccess.CreateTable(filepath, datafilename, whonetFieldList.Keys.ToList());

                var dataDicList = new List<Dictionary<string, string>>();

                foreach (DataRow dr in dataTable.Rows)
                {
                    var dataList = new Dictionary<string, string>();

                    foreach (DataColumn dc in dataTable.Columns)
                    {
                        var whonetColumn = dc.ColumnName;
                        var fieldValue = (string.IsNullOrEmpty(dr[dc].ToString()) == false) ? dr[dc].ToString() : null;

                        if (whonetColumn.Contains("DATE") && fieldValue != null)
                        {
                            string format1 = "M/d/yyyy H:mm:ss";
                            string format2 = "M/d/yyyy H:mm:ss tt";
                            string format3 = "M/d/yyyy H:mm:sss";
                            string format4 = "M/d/yyyy H:mm:sss tt";
                            string format5 = "M/d/yyyy";
                            DateTime tmpFieldValue;
                            if (DateTime.TryParseExact(fieldValue, format1, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                            {
                                fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                            }
                            else if (DateTime.TryParseExact(fieldValue, format2, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                            {
                                fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                            }
                            else if (DateTime.TryParseExact(fieldValue, format3, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                            {
                                fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                            }
                            else if (DateTime.TryParseExact(fieldValue, format4, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                            {
                                fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                            }
                            else if (DateTime.TryParseExact(fieldValue.Split(' ')[0], format5, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                            {
                                fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                            }
                            else
                            {
                                fieldValue = null;
                            }
                        }

                        dataList.Add(whonetColumn, fieldValue);
                    }

                    dataList["COUNTRY_A"] = dataList["pla_ldh_id"];

                    dataDicList.Add(dataList);
                }

                SQLiteDataAccess.InsertData(filepath, datafilename, dataDicList);

                filepath = Path.Combine(whonetPath, "Data");
                var copyfilename = Path.Combine(filepath, datafilename);
                if (File.Exists(copyfilename)) File.Delete(copyfilename);
                File.Copy(datafilefullname, copyfilename);

                using (var _db = new ProcessContext())
                {
                    using (var trans = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            var processLabFile = _db.TRProcessLabFile.FirstOrDefault(x => x.plf_lfu_id == item.plf_lfu_id);
                            if (processLabFile != null)
                            {
                                processLabFile.plf_status = "C";
                            }

                            _db.SaveChanges();

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

            Console.WriteLine("p0101_LabAlert_Create_WHONET_Data : End");
        }

        private static void LabAlert_Run_WHONET()
        {
            Console.WriteLine("p0101_LabAlert_Run_WHONET : Start");

            var processLabFileList = new List<TRProcessLabFileModel>();

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        processLabFileList = _db.TRProcessLabFile.Where(x => x.plf_status == "C").ToList();

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

            foreach (var item in processLabFileList)
            {
                Console.WriteLine(item.plf_lfu_id.ToString());

                var labFileId = item.plf_lfu_id.ToString();

                string macroFile = Path.Combine(whonetPath, "Macros", "MacroLabAlertTemplate.mcr");

                ProcessStartInfo processInfo;
                System.Diagnostics.Process process;
                string command;

                int exitCode;
                string output;
                string error;

                string macroBackupPath = Path.Combine(whonetPath, "Macros", "LabAlert");
                if (Directory.Exists(macroBackupPath) == false)
                {
                    Directory.CreateDirectory(macroBackupPath);
                }
                else
                {
                    //Directory.GetFiles(macroBackupPath).ToList().ForEach(macroFile => File.Delete(macroFile));
                }

                string outputBackupPath = Path.Combine(whonetPath, "Output", "LabAlert");
                if (Directory.Exists(outputBackupPath) == false)
                {
                    Directory.CreateDirectory(outputBackupPath);
                }
                else
                {
                    //Directory.GetFiles(outputBackupPath).ToList().ForEach(outputFile => File.Delete(outputFile));
                }

                //string macrofilename = Path.Combine(whonetPath, "macros", "macrotemplate.mcr");
                //string newmacroname = $"macro-{item.pcr_code}";
                //string newmacrofilename = macrofilename.Replace("macrotemplate", newmacroname);

                string newmacroFilename = macroFile.Replace("LabAlertTemplate", ("-" + labFileId + "-" + "LabAlert"));
                string newmacroName = newmacroFilename.Replace(Path.Combine(whonetPath, "Macros"), "").Replace("\\", "").Replace(".mcr", "");

                string dataFilename = $"{labFileId}-LabAlertData.sqlite";
                string ouputFilename = $"Output-{labFileId}-LabAlertData.db";

                if (File.Exists(newmacroFilename)) File.Delete(newmacroFilename);
                File.Copy(macroFile, newmacroFilename);
                //string text = File.ReadAllText(newmacrofilename);
                //text = text.Replace("{MacroName}", newmacroname);
                //text = text.Replace("{Datafile}", datafilename);
                //text = text.Replace("{Output}", "SQLITE File (" + ouputfilename + ")");
                //File.WriteAllText(newmacrofilename, text);

                string[] message = File.ReadAllLines(newmacroFilename);
                message[0] = $"Macro Name = {newmacroName}";
                File.WriteAllLines(newmacroFilename, message);
                string appendText = Environment.NewLine + $"Data file = {dataFilename}";
                File.AppendAllText(newmacroFilename, appendText, System.Text.Encoding.UTF8);
                appendText = Environment.NewLine + $"Output = SQLITE File (\"{ouputFilename}\")";
                File.AppendAllText(newmacroFilename, appendText, System.Text.Encoding.UTF8);

                command = Path.Combine(whonetPath, "WHONET.EXE") + $" \"{newmacroName}.mcr\"";
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

                if (ouputFilename.Contains(","))
                {
                    var tmpfilename = ouputFilename.Split(',')[0];
                    if (File.Exists(tmpfilename)) File.Move(tmpfilename, ouputFilename);
                }

                string macroBackupFile = Path.Combine(macroBackupPath, $"{newmacroName}.mcr");
                File.Move(newmacroFilename, macroBackupFile);
                Console.WriteLine($"move macroBackupFile {newmacroName}.mcr To macroBackupPath");

                string outputFile = Path.Combine(whonetPath, "Output", ouputFilename);
                string outputBackupFile = Path.Combine(outputBackupPath, ouputFilename);
                if (File.Exists(outputFile))
                {
                    File.Move(outputFile, outputBackupFile);
                    Console.WriteLine($"move outputBackupFile {ouputFilename} To outputBackupPath");
                }
                else
                {
                    Console.WriteLine($"File not found - {ouputFilename}");
                }

                using (var _db = new ProcessContext())
                {
                    using (var trans = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            var processLabFile = _db.TRProcessLabFile.FirstOrDefault(x => x.plf_lfu_id == item.plf_lfu_id);
                            if (processLabFile != null)
                            {
                                processLabFile.plf_status = "M";
                            }

                            _db.SaveChanges();

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

            Console.WriteLine("p0101_LabAlert_Run_WHONET : End");
        }

        private static void LabAlert_Insert_Result()
        {
            Console.WriteLine("p0102_LabAlert_Insert_Result : Start");

            var processLabFileList = new List<TRProcessLabFileModel>();

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        processLabFileList = _db.TRProcessLabFile.Where(x => x.plf_status == "M").ToList();

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

            var currentDateTime = DateTime.Now;
            foreach (var item in processLabFileList)
            {
                var objList = new List<TRProcessDataListing>();

                var labFileId = item.plf_lfu_id.ToString();

                string filepath = Path.Combine(whonetPath, "Output", "LabAlert");
                string ouputfilename = $"Output-{labFileId}-LabAlertData.db";

                var data = SQLiteDataAccess.LoadOutputDataLabAlert(filepath, ouputfilename);

                foreach (var outputItem in data)
                {
                    objList.Add(new TRProcessDataListing()
                    {
                        //pdl_pcr_code = item.pcr_code,
                        pdl_arh_code = item.plf_arh_code,
                        pdl_prv_code = item.plf_prv_code,
                        pdl_hos_code = item.plf_hos_code,
                        pdl_lab_code = item.plf_lab,
                        pdl_row_idx = outputItem.ROW_IDX,
                        pdl_country_a = outputItem.COUNTRY_A,
                        pdl_laboratory = outputItem.LABORATORY,
                        pdl_origin = outputItem.ORIGIN,
                        pdl_patient_id = outputItem.PATIENT_ID,
                        pdl_last_name = outputItem.LAST_NAME,
                        pdl_first_name = outputItem.FIRST_NAME,
                        pdl_sex = outputItem.SEX,
                        pdl_date_birth = outputItem.DATE_BIRTH,
                        pdl_age = outputItem.AGE,
                        pdl_pat_type = outputItem.PAT_TYPE,
                        pdl_ward = outputItem.WARD,
                        pdl_institut = outputItem.INSTITUT,
                        pdl_department = outputItem.DEPARTMENT,
                        pdl_ward_type = outputItem.WARD_TYPE,
                        pdl_spec_num = outputItem.SPEC_NUM,
                        pdl_spec_date = outputItem.SPEC_DATE,
                        pdl_spec_type = outputItem.SPEC_TYPE,
                        pdl_spec_code = outputItem.SPEC_CODE,
                        pdl_spec_reas = outputItem.SPEC_REAS,
                        pdl_isol_num = outputItem.ISOL_NUM,
                        pdl_organism = outputItem.ORGANISM,
                        pdl_nosocomial = outputItem.NOSOCOMIAL,
                        pdl_org_type = outputItem.ORG_TYPE,
                        pdl_serotype = outputItem.SEROTYPE,
                        pdl_beta_lact = outputItem.BETA_LACT,
                        pdl_esbl = outputItem.ESBL,
                        pdl_carbapenem = outputItem.CARBAPENEM,
                        pdl_mrsa_scrn = outputItem.MRSA_SCRN,
                        pdl_induc_cli = outputItem.INDUC_CLI,
                        pdl_comment = outputItem.COMMENT,
                        pdl_date_data = outputItem.DATE_DATA,
                        pdl_amk_nd30 = outputItem.AMK_ND30,
                        pdl_amc_nd20 = outputItem.AMC_ND20,
                        pdl_amp_nd10 = outputItem.AMP_ND10,
                        pdl_sam_nd10 = outputItem.SAM_ND10,
                        pdl_azm_nd15 = outputItem.AZM_ND15,
                        pdl_czo_nd30 = outputItem.CZO_ND30,
                        pdl_fep_nd30 = outputItem.FEP_ND30,
                        pdl_cfm_nd5 = outputItem.CFM_ND5,
                        pdl_csl_nd30 = outputItem.CSL_ND30,
                        pdl_ctx_nd30 = outputItem.CTX_ND30,
                        pdl_fox_nd30 = outputItem.FOX_ND30,
                        pdl_caz_nd30 = outputItem.CAZ_ND30,
                        pdl_czx_nd30 = outputItem.CZX_ND30,
                        pdl_cro_nd30 = outputItem.CRO_ND30,
                        pdl_cxa_nd30 = outputItem.CXA_ND30,
                        pdl_cxm_nd30 = outputItem.CXM_ND30,
                        pdl_chl_nd30 = outputItem.CHL_ND30,
                        pdl_cip_nd5 = outputItem.CIP_ND5,
                        pdl_cli_nd2 = outputItem.CLI_ND2,
                        pdl_col_nd10 = outputItem.COL_ND10,
                        pdl_dap_nd30 = outputItem.DAP_ND30,
                        pdl_dor_nd10 = outputItem.DOR_ND10,
                        pdl_etp_nd10 = outputItem.ETP_ND10,
                        pdl_ery_nd15 = outputItem.ERY_ND15,
                        pdl_fos_nd200 = outputItem.FOS_ND200,
                        pdl_fus_nd10 = outputItem.FUS_ND10,
                        pdl_gen_nd10 = outputItem.GEN_ND10,
                        pdl_geh_nd120 = outputItem.GEH_ND120,
                        pdl_ipm_nd10 = outputItem.IPM_ND10,
                        pdl_lvx_nd5 = outputItem.LVX_ND5,
                        pdl_mem_nd10 = outputItem.MEM_ND10,
                        pdl_nal_nd30 = outputItem.NAL_ND30,
                        pdl_net_nd30 = outputItem.NET_ND30,
                        pdl_nit_nd300 = outputItem.NIT_ND300,
                        pdl_nor_nd10 = outputItem.NOR_ND10,
                        pdl_ofx_nd5 = outputItem.OFX_ND5,
                        pdl_oxa_nd1 = outputItem.OXA_ND1,
                        pdl_pen_nd10 = outputItem.PEN_ND10,
                        pdl_pip_nd100 = outputItem.PIP_ND100,
                        pdl_tzp_nd100 = outputItem.TZP_ND100,
                        pdl_pol_nd300 = outputItem.POL_ND300,
                        pdl_sth_nd300 = outputItem.STH_ND300,
                        pdl_str_nd10 = outputItem.STR_ND10,
                        pdl_tec_nd30 = outputItem.TEC_ND30,
                        pdl_tcy_nd30 = outputItem.TCY_ND30,
                        pdl_sxt_nd1_2 = outputItem.SXT_ND1_2,
                        pdl_van_nd30 = outputItem.VAN_ND30,
                        pdl_amx_ne = outputItem.AMX_NE,
                        pdl_ctx_nm = outputItem.CTX_NM,
                        pdl_ctx_ne = outputItem.CTX_NE,
                        pdl_caz_nm = outputItem.CAZ_NM,
                        pdl_caz_ne = outputItem.CAZ_NE,
                        pdl_czx_nm = outputItem.CZX_NM,
                        pdl_czx_ne = outputItem.CZX_NE,
                        pdl_cro_nm = outputItem.CRO_NM,
                        pdl_cro_ne = outputItem.CRO_NE,
                        pdl_chl_nm = outputItem.CHL_NM,
                        pdl_chl_ne = outputItem.CHL_NE,
                        pdl_cip_nm = outputItem.CIP_NM,
                        pdl_cip_ne = outputItem.CIP_NE,
                        pdl_cli_nm = outputItem.CLI_NM,
                        pdl_cli_ne = outputItem.CLI_NE,
                        pdl_col_nm = outputItem.COL_NM,
                        pdl_col_ne = outputItem.COL_NE,
                        pdl_dap_nm = outputItem.DAP_NM,
                        pdl_dap_ne = outputItem.DAP_NE,
                        pdl_etp_nm = outputItem.ETP_NM,
                        pdl_etp_ne = outputItem.ETP_NE,
                        pdl_ery_nm = outputItem.ERY_NM,
                        pdl_ery_ne = outputItem.ERY_NE,
                        pdl_ipm_nm = outputItem.IPM_NM,
                        pdl_ipm_ne = outputItem.IPM_NE,
                        pdl_mem_nm = outputItem.MEM_NM,
                        pdl_mem_ne = outputItem.MEM_NE,
                        pdl_net_nm = outputItem.NET_NM,
                        pdl_net_ne = outputItem.NET_NE,
                        pdl_pen_nm = outputItem.PEN_NM,
                        pdl_pen_ne = outputItem.PEN_NE,
                        pdl_van_nm = outputItem.VAN_NM,
                        pdl_van_ne = outputItem.VAN_NE,
                        pdl_azm_ne = outputItem.AZM_NE,
                        pdl_spt_nd100 = outputItem.SPT_ND100,
                        pdl_gen_nm = outputItem.GEN_NM,
                        pdl_gen_ne = outputItem.GEN_NE,
                        pdl_tcy_nm = outputItem.TCY_NM,
                        pdl_tcy_ne = outputItem.TCY_NE,
                        pdl_cxm_nm = outputItem.CXM_NM,
                        pdl_cfm_ne = outputItem.CFM_NE,
                        pdl_amp_nm = outputItem.AMP_NM,
                        pdl_amp_ne = outputItem.AMP_NE,
                        pdl_fep_nm = outputItem.FEP_NM,
                        pdl_fep_ne = outputItem.FEP_NE,
                        pdl_lvx_nm = outputItem.LVX_NM,
                        pdl_lvx_ne = outputItem.LVX_NE,
                        pdl_amk_nm = outputItem.AMK_NM,
                        pdl_amk_ne = outputItem.AMK_NE,
                        pdl_tzp_nm = outputItem.TZP_NM,
                        pdl_tzp_ne = outputItem.TZP_NE,
                        pdl_sam_nm = outputItem.SAM_NM,
                        pdl_sam_ne = outputItem.SAM_NE,
                        pdl_czo_nm = outputItem.CZO_NM,
                        pdl_czo_ne = outputItem.CZO_NE,
                        pdl_azm_nm = outputItem.AZM_NM,
                        pdl_cxm_ne = outputItem.CXM_NE,
                        pdl_cxa_nm = outputItem.CXA_NM,
                        pdl_cxa_ne = outputItem.CXA_NE,
                        pdl_amc_nm = outputItem.AMC_NM,
                        pdl_amc_ne = outputItem.AMC_NE,
                        pdl_csl_nm = outputItem.CSL_NM,
                        pdl_csl_ne = outputItem.CSL_NE,
                        pdl_oxa_nm = outputItem.OXA_NM,
                        pdl_oxa_ne = outputItem.OXA_NE,
                        pdl_fox_nm = outputItem.FOX_NM,
                        pdl_fox_ne = outputItem.FOX_NE,
                        pdl_nor_nm = outputItem.NOR_NM,
                        pdl_nor_ne = outputItem.NOR_NE,
                        pdl_geh_nm = outputItem.GEH_NM,
                        pdl_geh_ne = outputItem.GEH_NE,
                        pdl_tec_nm = outputItem.TEC_NM,
                        pdl_tec_ne = outputItem.TEC_NE,
                        pdl_fos_nm = outputItem.FOS_NM,
                        pdl_fos_ne = outputItem.FOS_NE,
                        pdl_nit_nm = outputItem.NIT_NM,
                        pdl_nit_ne = outputItem.NIT_NE,
                        pdl_sxt_nm = outputItem.SXT_NM,
                        pdl_sxt_ne = outputItem.SXT_NE,
                        pdl_dor_nm = outputItem.DOR_NM,
                        pdl_dor_ne = outputItem.DOR_NE,
                        pdl_createuser = "BATCH",
                        pdl_createdate = currentDateTime
                    });
                }
                using (var _db = new ProcessContext())
                {
                    using (var trans = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            _db.BulkInsert(objList);

                            var processLabFile = _db.TRProcessLabFile.FirstOrDefault(x => x.plf_lfu_id == item.plf_lfu_id);
                            if (processLabFile != null)
                            {
                                processLabFile.plf_status = "D";
                            }

                            _db.SaveChanges();

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

            Console.WriteLine("p0102_LabAlert_Insert_Result : End");
        }

        private static void DataListing_Run_WHONET()
        {
            Console.WriteLine("p04_Run_WHONET_Macro_DataListing : Start");

            foreach (var item in processRequestList)
            {
                string[] filePaths = Directory.GetFiles(Path.Combine(whonetPath, "Macros"), "DataListing.mcr");

                ProcessStartInfo processInfo;
                System.Diagnostics.Process process;
                string command;

                int exitCode;
                string output;
                string error;

                string macroBackupPath = Path.Combine(whonetPath, "Macros", (item.pcr_code + "-" + "DataListing"));
                if (Directory.Exists(macroBackupPath) == false)
                {
                    Directory.CreateDirectory(macroBackupPath);
                }
                else
                {
                    Directory.GetFiles(macroBackupPath).ToList().ForEach(macroFile => File.Delete(macroFile));
                }

                string outputBackupPath = Path.Combine(whonetPath, "Output", (item.pcr_code + "-" + "DataListing"));
                if (Directory.Exists(outputBackupPath) == false)
                {
                    Directory.CreateDirectory(outputBackupPath);
                }
                else
                {
                    Directory.GetFiles(outputBackupPath).ToList().ForEach(outputFile => File.Delete(outputFile));
                }

                foreach (var macroFile in filePaths)
                {
                    //string macrofilename = Path.Combine(whonetPath, "macros", "macrotemplate.mcr");
                    //string newmacroname = $"macro-{item.pcr_code}";
                    //string newmacrofilename = macrofilename.Replace("macrotemplate", newmacroname);

                    string newmacroFilename = macroFile.Replace("DataListing", (item.pcr_code + "-" + "DataListing"));
                    string newmacroName = newmacroFilename.Replace(Path.Combine(whonetPath, "Macros"), "").Replace("\\", "").Replace(".mcr", "");

                    string dataFilename = string.Join(",", item.processRequestDetails.Select(x => $"{item.pcr_code}-{x.pcd_hos_code}-{x.pcd_lab_code}.sqlite"));
                    string ouputFilename = $"output-{newmacroName}.db";

                    if (File.Exists(newmacroFilename)) File.Delete(newmacroFilename);
                    File.Copy(macroFile, newmacroFilename);
                    //string text = File.ReadAllText(newmacrofilename);
                    //text = text.Replace("{MacroName}", newmacroname);
                    //text = text.Replace("{Datafile}", datafilename);
                    //text = text.Replace("{Output}", "SQLITE File (" + ouputfilename + ")");
                    //File.WriteAllText(newmacrofilename, text);

                    string[] message = File.ReadAllLines(newmacroFilename);
                    message[0] = $"Macro Name = {newmacroName}";
                    File.WriteAllLines(newmacroFilename, message);
                    string appendText = Environment.NewLine + $"Data file = {dataFilename}";
                    File.AppendAllText(newmacroFilename, appendText, System.Text.Encoding.UTF8);
                    appendText = Environment.NewLine + $"Output = SQLITE File (\"{ouputFilename}\")";
                    File.AppendAllText(newmacroFilename, appendText, System.Text.Encoding.UTF8);

                    command = Path.Combine(whonetPath, "WHONET.EXE") + $" \"{newmacroName}.mcr\"";
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

                    if (ouputFilename.Contains(","))
                    {
                        var tmpfilename = ouputFilename.Split(',')[0];
                        if (File.Exists(tmpfilename)) File.Move(tmpfilename, ouputFilename);
                    }

                    string macroBackupFile = Path.Combine(macroBackupPath, $"{newmacroName}.mcr");
                    File.Move(newmacroFilename, macroBackupFile);
                    Console.WriteLine($"move macroBackupFile {newmacroName}.mcr To macroBackupPath");

                    string outputFile = Path.Combine(whonetPath, "Output", ouputFilename);
                    string outputBackupFile = Path.Combine(outputBackupPath, ouputFilename);
                    if (File.Exists(outputFile))
                    {
                        File.Move(outputFile, outputBackupFile);
                        Console.WriteLine($"move outputBackupFile {ouputFilename} To outputBackupPath");
                    }
                    else
                    {
                        Console.WriteLine($"File not found - {ouputFilename}");
                    }
                }
            }

            Console.WriteLine("p04_Run_WHONET_Macro_DataListing : End");
        }

        private static void DataListing_Insert_Result()
        {
            Console.WriteLine("p05_Insert_Result_DataListing : Start");

            var currentDateTime = DateTime.Now;
            foreach (var item in processRequestList)
            {
                //string ouputfilename = $"output-{item.pcr_code}*.db";
                string[] filePaths = Directory.GetFiles(Path.Combine(whonetPath, "Output", (item.pcr_code + "-" + "DataListing")));

                foreach (var outputFile in filePaths)
                {
                    List<TRProcessDataListing> objList = new List<TRProcessDataListing>();
                    List<RPIsolateListing> objIsolateListing = new List<RPIsolateListing>();
                    List<RPIsolateListingDetail> objIsolateListingDetail = new List<RPIsolateListingDetail>();
                    var detailCount = 1;

                    foreach (var itemDetail in item.processRequestDetails)
                    {
                        string filepath = Path.Combine(whonetPath, "Output", (item.pcr_code + "-" + "DataListing"));

                        string ouputfilename = outputFile.Replace(Path.Combine(whonetPath, "Output", (item.pcr_code + "-" + "DataListing")), "").Replace("\\", "");
                        var data = SQLiteDataAccess.LoadOutputDataListing(filepath, ouputfilename, detailCount.ToString());

                        detailCount++;
                        foreach (var outputItem in data)
                        {
                            var header_iso_id = Guid.NewGuid();

                            objIsolateListing.Add(new RPIsolateListing()
                            {
                                pcr_code = item.pcr_code,
                                iso_id = header_iso_id,
                                country = outputItem.COUNTRY_A,
                                laboratory = outputItem.LABORATORY,
                                arh_code = item.pcr_arh_code,
                                prv_code = item.pcr_prv_code,
                                hos_code = item.pcr_hos_code,
                                origin = outputItem.ORIGIN,
                                pid = outputItem.PATIENT_ID,
                                lastname = outputItem.LAST_NAME,
                                firstname = outputItem.FIRST_NAME,
                                sex = outputItem.SEX,
                                birth = ((string.IsNullOrEmpty(outputItem.DATE_BIRTH) == false) ? new DateTime(Convert.ToInt32(outputItem.DATE_BIRTH.Split('/')[2].Split(' ')[0]), Convert.ToInt32(outputItem.DATE_BIRTH.Split('/')[0]), Convert.ToInt32(outputItem.DATE_BIRTH.Split('/')[1])) : (DateTime?)null),
                                age = outputItem.AGE,
                                age_category = outputItem.PAT_TYPE,
                                location = outputItem.WARD,
                                institution = outputItem.INSTITUT,
                                department = outputItem.DEPARTMENT,
                                loc_type = outputItem.WARD_TYPE,
                                admission_date = ((string.IsNullOrEmpty(outputItem.DATE_DATA) == false) ? new DateTime(Convert.ToInt32(outputItem.DATE_DATA.Split('/')[2].Split(' ')[0]), Convert.ToInt32(outputItem.DATE_DATA.Split('/')[0]), Convert.ToInt32(outputItem.DATE_DATA.Split('/')[1])) : (DateTime?)null),
                                spec_no = outputItem.SPEC_CODE,
                                spc_date = ((string.IsNullOrEmpty(outputItem.SPEC_DATE) == false) ? new DateTime(Convert.ToInt32(outputItem.SPEC_DATE.Split('/')[2].Split(' ')[0]), Convert.ToInt32(outputItem.SPEC_DATE.Split('/')[0]), Convert.ToInt32(outputItem.SPEC_DATE.Split('/')[1])) : (DateTime?)null),
                                spc_type = outputItem.SPEC_TYPE,
                                spc_type_num = outputItem.SPEC_NUM,
                                reason = outputItem.SPEC_REAS,
                                isolate_no = outputItem.ISOL_NUM,
                                org_code = outputItem.ORGANISM,
                                org_name = "",
                                nosocomial_infect = outputItem.NOSOCOMIAL,
                                org_type = outputItem.ORG_TYPE,
                                serotype = outputItem.SEROTYPE,
                                beta_lactamase = outputItem.BETA_LACT,
                                ESBL = outputItem.ESBL,
                                carbapenemase = outputItem.CARBAPENEM,
                                MRSA_screening_test = outputItem.MRSA_SCRN,
                                inducible_clindamycin_resistance = outputItem.INDUC_CLI,
                                comment = outputItem.COMMENT,
                                date_of_data_entry = ((string.IsNullOrEmpty(outputItem.DATE_DATA) == false) ? new DateTime(Convert.ToInt32(outputItem.DATE_DATA.Split('/')[2].Split(' ')[0]), Convert.ToInt32(outputItem.DATE_DATA.Split('/')[0]), Convert.ToInt32(outputItem.DATE_DATA.Split('/')[1])) : (DateTime?)null),
                                createuser = "BATCH",
                                createdate = DateTime.Now
                            });

                            if (string.IsNullOrEmpty(outputItem.AMK_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "AMK_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.AMK_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.AMC_ND20) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "AMC_ND20",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.AMC_ND20,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.AMP_ND10) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "AMP_ND10",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.AMP_ND10,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.SAM_ND10) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "SAM_ND10",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.SAM_ND10,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.AZM_ND15) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "AZM_ND15",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.AZM_ND15,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CZO_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CZO_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CZO_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.FEP_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "FEP_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.FEP_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CFM_ND5) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CFM_ND5",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CFM_ND5,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CSL_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CSL_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CSL_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CTX_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CTX_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CTX_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.FOX_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "FOX_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.FOX_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CAZ_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CAZ_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CAZ_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CZX_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CZX_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CZX_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CRO_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CRO_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CRO_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CXA_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CXA_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CXA_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CXM_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CXM_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CXM_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CHL_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CHL_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CHL_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CIP_ND5) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CIP_ND5",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CIP_ND5,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CLI_ND2) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CLI_ND2",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CLI_ND2,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.COL_ND10) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "COL_ND10",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.COL_ND10,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.DAP_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "DAP_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.DAP_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.DOR_ND10) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "DOR_ND10",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.DOR_ND10,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.ETP_ND10) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "ETP_ND10",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.ETP_ND10,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.ERY_ND15) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "ERY_ND15",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.ERY_ND15,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.FOS_ND200) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "FOS_ND200",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.FOS_ND200,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.FUS_ND10) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "FUS_ND10",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.FUS_ND10,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.GEN_ND10) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "GEN_ND10",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.GEN_ND10,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.GEH_ND120) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "GEH_ND120",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.GEH_ND120,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.IPM_ND10) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "IPM_ND10",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.IPM_ND10,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.LVX_ND5) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "LVX_ND5",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.LVX_ND5,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.MEM_ND10) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "MEM_ND10",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.MEM_ND10,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.NAL_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "NAL_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.NAL_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.NET_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "NET_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.NET_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.NIT_ND300) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "NIT_ND300",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.NIT_ND300,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.NOR_ND10) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "NOR_ND10",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.NOR_ND10,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.OFX_ND5) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "OFX_ND5",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.OFX_ND5,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.OXA_ND1) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "OXA_ND1",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.OXA_ND1,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.PEN_ND10) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "PEN_ND10",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.PEN_ND10,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.PIP_ND100) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "PIP_ND100",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.PIP_ND100,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.TZP_ND100) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "TZP_ND100",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.TZP_ND100,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.POL_ND300) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "POL_ND300",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.POL_ND300,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.STH_ND300) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "STH_ND300",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.STH_ND300,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.STR_ND10) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "STR_ND10",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.STR_ND10,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.TEC_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "TEC_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.TEC_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.TCY_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "TCY_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.TCY_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.SXT_ND1_2) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "SXT_ND1_2",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.SXT_ND1_2,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.VAN_ND30) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "VAN_ND30",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.VAN_ND30,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.AMX_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "AMX_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.AMX_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CTX_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CTX_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CTX_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CTX_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CTX_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CTX_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CAZ_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CAZ_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CAZ_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CAZ_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CAZ_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CAZ_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CZX_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CZX_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CZX_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CZX_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CZX_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CZX_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CRO_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CRO_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CRO_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CRO_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CRO_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CRO_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CHL_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CHL_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CHL_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CHL_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CHL_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CHL_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CIP_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CIP_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CIP_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CIP_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CIP_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CIP_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CLI_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CLI_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CLI_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CLI_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CLI_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CLI_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.COL_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "COL_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.COL_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.COL_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "COL_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.COL_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.DAP_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "DAP_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.DAP_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.DAP_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "DAP_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.DAP_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.ETP_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "ETP_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.ETP_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.ETP_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "ETP_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.ETP_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.ERY_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "ERY_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.ERY_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.ERY_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "ERY_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.ERY_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.IPM_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "IPM_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.IPM_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.IPM_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "IPM_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.IPM_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.MEM_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "MEM_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.MEM_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.MEM_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "MEM_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.MEM_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.NET_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "NET_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.NET_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.NET_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "NET_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.NET_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.PEN_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "PEN_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.PEN_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.PEN_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "PEN_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.PEN_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.VAN_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "VAN_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.VAN_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.VAN_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "VAN_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.VAN_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.AZM_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "AZM_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.AZM_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.SPT_ND100) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "SPT_ND100",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.SPT_ND100,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.GEN_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "GEN_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.GEN_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.GEN_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "GEN_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.GEN_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.TCY_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "TCY_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.TCY_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.TCY_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "TCY_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.TCY_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CXM_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CXM_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CXM_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CFM_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CFM_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CFM_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.AMP_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "AMP_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.AMP_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.AMP_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "AMP_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.AMP_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.FEP_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "FEP_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.FEP_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.FEP_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "FEP_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.FEP_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.LVX_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "LVX_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.LVX_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.LVX_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "LVX_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.LVX_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.AMK_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "AMK_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.AMK_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.AMK_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "AMK_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.AMK_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.TZP_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "TZP_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.TZP_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.TZP_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "TZP_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.TZP_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.SAM_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "SAM_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.SAM_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.SAM_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "SAM_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.SAM_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CZO_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CZO_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CZO_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CZO_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CZO_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CZO_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.AZM_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "AZM_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.AZM_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CXM_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CXM_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CXM_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CXA_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CXA_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CXA_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CXA_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CXA_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CXA_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.AMC_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "AMC_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.AMC_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.AMC_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "AMC_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.AMC_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CSL_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CSL_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CSL_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.CSL_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "CSL_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.CSL_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.OXA_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "OXA_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.OXA_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.OXA_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "OXA_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.OXA_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.FOX_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "FOX_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.FOX_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.FOX_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "FOX_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.FOX_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.NOR_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "NOR_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.NOR_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.NOR_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "NOR_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.NOR_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.GEH_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "GEH_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.GEH_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.GEH_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "GEH_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.GEH_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.TEC_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "TEC_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.TEC_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.TEC_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "TEC_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.TEC_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.FOS_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "FOS_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.FOS_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.FOS_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "FOS_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.FOS_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.NIT_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "NIT_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.NIT_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.NIT_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "NIT_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.NIT_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.SXT_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "SXT_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.SXT_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.SXT_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "SXT_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.SXT_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.DOR_NM) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "DOR_NM",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.DOR_NM,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }
                            if (string.IsNullOrEmpty(outputItem.DOR_NE) == false)
                            {
                                objIsolateListingDetail.Add(new RPIsolateListingDetail()
                                {
                                    header_iso_id = header_iso_id,
                                    anti_code = "DOR_NE",
                                    anti_name = "",
                                    result_value = null,
                                    interp_value = outputItem.DOR_NE,
                                    remark = null,
                                    createuser = "BATCH",
                                    createdate = DateTime.Now
                                });
                            }


                            objList.Add(new TRProcessDataListing()
                            {
                                pdl_pcr_code = item.pcr_code,
                                pdl_arh_code = item.pcr_arh_code,
                                pdl_prv_code = item.pcr_prv_code,
                                pdl_hos_code = item.pcr_hos_code,
                                pdl_lab_code = "",
                                pdl_row_idx = outputItem.ROW_IDX,
                                pdl_country_a = outputItem.COUNTRY_A,
                                pdl_laboratory = outputItem.LABORATORY,
                                pdl_origin = outputItem.ORIGIN,
                                pdl_patient_id = outputItem.PATIENT_ID,
                                pdl_last_name = outputItem.LAST_NAME,
                                pdl_first_name = outputItem.FIRST_NAME,
                                pdl_sex = outputItem.SEX,
                                pdl_date_birth = outputItem.DATE_BIRTH,
                                pdl_age = outputItem.AGE,
                                pdl_pat_type = outputItem.PAT_TYPE,
                                pdl_ward = outputItem.WARD,
                                pdl_institut = outputItem.INSTITUT,
                                pdl_department = outputItem.DEPARTMENT,
                                pdl_ward_type = outputItem.WARD_TYPE,
                                pdl_spec_num = outputItem.SPEC_NUM,
                                pdl_spec_date = outputItem.SPEC_DATE,
                                pdl_spec_type = outputItem.SPEC_TYPE,
                                pdl_spec_code = outputItem.SPEC_CODE,
                                pdl_spec_reas = outputItem.SPEC_REAS,
                                pdl_isol_num = outputItem.ISOL_NUM,
                                pdl_organism = outputItem.ORGANISM,
                                pdl_nosocomial = outputItem.NOSOCOMIAL,
                                pdl_org_type = outputItem.ORG_TYPE,
                                pdl_serotype = outputItem.SEROTYPE,
                                pdl_beta_lact = outputItem.BETA_LACT,
                                pdl_esbl = outputItem.ESBL,
                                pdl_carbapenem = outputItem.CARBAPENEM,
                                pdl_mrsa_scrn = outputItem.MRSA_SCRN,
                                pdl_induc_cli = outputItem.INDUC_CLI,
                                pdl_comment = outputItem.COMMENT,
                                pdl_date_data = outputItem.DATE_DATA,
                                pdl_amk_nd30 = outputItem.AMK_ND30,
                                pdl_amc_nd20 = outputItem.AMC_ND20,
                                pdl_amp_nd10 = outputItem.AMP_ND10,
                                pdl_sam_nd10 = outputItem.SAM_ND10,
                                pdl_azm_nd15 = outputItem.AZM_ND15,
                                pdl_czo_nd30 = outputItem.CZO_ND30,
                                pdl_fep_nd30 = outputItem.FEP_ND30,
                                pdl_cfm_nd5 = outputItem.CFM_ND5,
                                pdl_csl_nd30 = outputItem.CSL_ND30,
                                pdl_ctx_nd30 = outputItem.CTX_ND30,
                                pdl_fox_nd30 = outputItem.FOX_ND30,
                                pdl_caz_nd30 = outputItem.CAZ_ND30,
                                pdl_czx_nd30 = outputItem.CZX_ND30,
                                pdl_cro_nd30 = outputItem.CRO_ND30,
                                pdl_cxa_nd30 = outputItem.CXA_ND30,
                                pdl_cxm_nd30 = outputItem.CXM_ND30,
                                pdl_chl_nd30 = outputItem.CHL_ND30,
                                pdl_cip_nd5 = outputItem.CIP_ND5,
                                pdl_cli_nd2 = outputItem.CLI_ND2,
                                pdl_col_nd10 = outputItem.COL_ND10,
                                pdl_dap_nd30 = outputItem.DAP_ND30,
                                pdl_dor_nd10 = outputItem.DOR_ND10,
                                pdl_etp_nd10 = outputItem.ETP_ND10,
                                pdl_ery_nd15 = outputItem.ERY_ND15,
                                pdl_fos_nd200 = outputItem.FOS_ND200,
                                pdl_fus_nd10 = outputItem.FUS_ND10,
                                pdl_gen_nd10 = outputItem.GEN_ND10,
                                pdl_geh_nd120 = outputItem.GEH_ND120,
                                pdl_ipm_nd10 = outputItem.IPM_ND10,
                                pdl_lvx_nd5 = outputItem.LVX_ND5,
                                pdl_mem_nd10 = outputItem.MEM_ND10,
                                pdl_nal_nd30 = outputItem.NAL_ND30,
                                pdl_net_nd30 = outputItem.NET_ND30,
                                pdl_nit_nd300 = outputItem.NIT_ND300,
                                pdl_nor_nd10 = outputItem.NOR_ND10,
                                pdl_ofx_nd5 = outputItem.OFX_ND5,
                                pdl_oxa_nd1 = outputItem.OXA_ND1,
                                pdl_pen_nd10 = outputItem.PEN_ND10,
                                pdl_pip_nd100 = outputItem.PIP_ND100,
                                pdl_tzp_nd100 = outputItem.TZP_ND100,
                                pdl_pol_nd300 = outputItem.POL_ND300,
                                pdl_sth_nd300 = outputItem.STH_ND300,
                                pdl_str_nd10 = outputItem.STR_ND10,
                                pdl_tec_nd30 = outputItem.TEC_ND30,
                                pdl_tcy_nd30 = outputItem.TCY_ND30,
                                pdl_sxt_nd1_2 = outputItem.SXT_ND1_2,
                                pdl_van_nd30 = outputItem.VAN_ND30,
                                pdl_amx_ne = outputItem.AMX_NE,
                                pdl_ctx_nm = outputItem.CTX_NM,
                                pdl_ctx_ne = outputItem.CTX_NE,
                                pdl_caz_nm = outputItem.CAZ_NM,
                                pdl_caz_ne = outputItem.CAZ_NE,
                                pdl_czx_nm = outputItem.CZX_NM,
                                pdl_czx_ne = outputItem.CZX_NE,
                                pdl_cro_nm = outputItem.CRO_NM,
                                pdl_cro_ne = outputItem.CRO_NE,
                                pdl_chl_nm = outputItem.CHL_NM,
                                pdl_chl_ne = outputItem.CHL_NE,
                                pdl_cip_nm = outputItem.CIP_NM,
                                pdl_cip_ne = outputItem.CIP_NE,
                                pdl_cli_nm = outputItem.CLI_NM,
                                pdl_cli_ne = outputItem.CLI_NE,
                                pdl_col_nm = outputItem.COL_NM,
                                pdl_col_ne = outputItem.COL_NE,
                                pdl_dap_nm = outputItem.DAP_NM,
                                pdl_dap_ne = outputItem.DAP_NE,
                                pdl_etp_nm = outputItem.ETP_NM,
                                pdl_etp_ne = outputItem.ETP_NE,
                                pdl_ery_nm = outputItem.ERY_NM,
                                pdl_ery_ne = outputItem.ERY_NE,
                                pdl_ipm_nm = outputItem.IPM_NM,
                                pdl_ipm_ne = outputItem.IPM_NE,
                                pdl_mem_nm = outputItem.MEM_NM,
                                pdl_mem_ne = outputItem.MEM_NE,
                                pdl_net_nm = outputItem.NET_NM,
                                pdl_net_ne = outputItem.NET_NE,
                                pdl_pen_nm = outputItem.PEN_NM,
                                pdl_pen_ne = outputItem.PEN_NE,
                                pdl_van_nm = outputItem.VAN_NM,
                                pdl_van_ne = outputItem.VAN_NE,
                                pdl_azm_ne = outputItem.AZM_NE,
                                pdl_spt_nd100 = outputItem.SPT_ND100,
                                pdl_gen_nm = outputItem.GEN_NM,
                                pdl_gen_ne = outputItem.GEN_NE,
                                pdl_tcy_nm = outputItem.TCY_NM,
                                pdl_tcy_ne = outputItem.TCY_NE,
                                pdl_cxm_nm = outputItem.CXM_NM,
                                pdl_cfm_ne = outputItem.CFM_NE,
                                pdl_amp_nm = outputItem.AMP_NM,
                                pdl_amp_ne = outputItem.AMP_NE,
                                pdl_fep_nm = outputItem.FEP_NM,
                                pdl_fep_ne = outputItem.FEP_NE,
                                pdl_lvx_nm = outputItem.LVX_NM,
                                pdl_lvx_ne = outputItem.LVX_NE,
                                pdl_amk_nm = outputItem.AMK_NM,
                                pdl_amk_ne = outputItem.AMK_NE,
                                pdl_tzp_nm = outputItem.TZP_NM,
                                pdl_tzp_ne = outputItem.TZP_NE,
                                pdl_sam_nm = outputItem.SAM_NM,
                                pdl_sam_ne = outputItem.SAM_NE,
                                pdl_czo_nm = outputItem.CZO_NM,
                                pdl_czo_ne = outputItem.CZO_NE,
                                pdl_azm_nm = outputItem.AZM_NM,
                                pdl_cxm_ne = outputItem.CXM_NE,
                                pdl_cxa_nm = outputItem.CXA_NM,
                                pdl_cxa_ne = outputItem.CXA_NE,
                                pdl_amc_nm = outputItem.AMC_NM,
                                pdl_amc_ne = outputItem.AMC_NE,
                                pdl_csl_nm = outputItem.CSL_NM,
                                pdl_csl_ne = outputItem.CSL_NE,
                                pdl_oxa_nm = outputItem.OXA_NM,
                                pdl_oxa_ne = outputItem.OXA_NE,
                                pdl_fox_nm = outputItem.FOX_NM,
                                pdl_fox_ne = outputItem.FOX_NE,
                                pdl_nor_nm = outputItem.NOR_NM,
                                pdl_nor_ne = outputItem.NOR_NE,
                                pdl_geh_nm = outputItem.GEH_NM,
                                pdl_geh_ne = outputItem.GEH_NE,
                                pdl_tec_nm = outputItem.TEC_NM,
                                pdl_tec_ne = outputItem.TEC_NE,
                                pdl_fos_nm = outputItem.FOS_NM,
                                pdl_fos_ne = outputItem.FOS_NE,
                                pdl_nit_nm = outputItem.NIT_NM,
                                pdl_nit_ne = outputItem.NIT_NE,
                                pdl_sxt_nm = outputItem.SXT_NM,
                                pdl_sxt_ne = outputItem.SXT_NE,
                                pdl_dor_nm = outputItem.DOR_NM,
                                pdl_dor_ne = outputItem.DOR_NE,
                                pdl_createuser = "BATCH",
                                pdl_createdate = currentDateTime
                            });
                        }
                    }
                    using (var _db = new ProcessContext())
                    {
                        using (var trans = _db.Database.BeginTransaction())
                        {
                            try
                            {
                                _db.BulkInsert(objIsolateListing);
                                _db.BulkInsert(objIsolateListingDetail);
                                _db.BulkInsert(objList);
                                _db.SaveChanges();

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
            }

            Console.WriteLine("p05_Insert_Result_DataListing : End");
        }

        private static void NARST_Run_WHONET_SQLITE()
        {
            Console.WriteLine("p04_Run_WHONET : Start");

            foreach (var item in processRequestList)
            {
                string[] filePaths = Directory.GetFiles(Path.Combine(whonetPath, "Macros", "Template"), "*.mcr");

                ProcessStartInfo processInfo;
                System.Diagnostics.Process process;
                string command;

                int exitCode;
                string output;
                string error;

                string macroBackupPath = Path.Combine(whonetPath, "Macros", item.pcr_code);
                if (Directory.Exists(macroBackupPath) == false)
                {
                    Directory.CreateDirectory(macroBackupPath);
                }
                else
                {
                    Directory.GetFiles(macroBackupPath).ToList().ForEach(macroFile => File.Delete(macroFile));
                }

                string outputBackupPath = Path.Combine(whonetPath, "Output", item.pcr_code);
                if (Directory.Exists(outputBackupPath) == false)
                {
                    Directory.CreateDirectory(outputBackupPath);
                }
                else
                {
                    Directory.GetFiles(outputBackupPath).ToList().ForEach(outputFile => File.Delete(outputFile));
                }

                foreach (var macroFile in filePaths)
                {
                    //string macrofilename = Path.Combine(whonetPath, "macros", "macrotemplate.mcr");
                    //string newmacroname = $"macro-{item.pcr_code}";
                    //string newmacrofilename = macrofilename.Replace("macrotemplate", newmacroname);

                    string newmacroFilename = macroFile.Replace("Template\\", item.pcr_code + "-");
                    string newmacroName = newmacroFilename.Replace(Path.Combine(whonetPath, "Macros"), "").Replace("\\", "").Replace(".mcr", "");

                    string dataFilename = string.Join(",", item.processRequestDetails.Select(x => $"{item.pcr_code}-{x.pcd_hos_code}-{x.pcd_lab_code}.sqlite"));
                    string ouputFilename = $"output-{newmacroName}.db";

                    if (File.Exists(newmacroFilename)) File.Delete(newmacroFilename);
                    File.Copy(macroFile, newmacroFilename);
                    //string text = File.ReadAllText(newmacrofilename);
                    //text = text.Replace("{MacroName}", newmacroname);
                    //text = text.Replace("{Datafile}", datafilename);
                    //text = text.Replace("{Output}", "SQLITE File (" + ouputfilename + ")");
                    //File.WriteAllText(newmacrofilename, text);

                    string[] message = File.ReadAllLines(newmacroFilename);
                    message[0] = $"Macro Name = {newmacroName}";
                    File.WriteAllLines(newmacroFilename, message);
                    string appendText = Environment.NewLine + $"Data file = {dataFilename}";
                    File.AppendAllText(newmacroFilename, appendText, System.Text.Encoding.UTF8);
                    appendText = Environment.NewLine + $"Output = SQLITE File (\"{ouputFilename}\")";
                    File.AppendAllText(newmacroFilename, appendText, System.Text.Encoding.UTF8);

                    command = Path.Combine(whonetPath, "WHONET.EXE") + $" \"{newmacroName}.mcr\"";
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

                    if (ouputFilename.Contains(","))
                    {
                        var tmpfilename = ouputFilename.Split(',')[0];
                        if (File.Exists(tmpfilename)) File.Move(tmpfilename, ouputFilename);
                    }

                    string macroBackupFile = Path.Combine(macroBackupPath, $"{newmacroName}.mcr");
                    File.Move(newmacroFilename, macroBackupFile);
                    Console.WriteLine($"move macroBackupFile {newmacroName}.mcr To macroBackupPath");

                    string outputFile = Path.Combine(whonetPath, "Output", ouputFilename);
                    string outputBackupFile = Path.Combine(outputBackupPath, ouputFilename);
                    if (File.Exists(outputFile))
                    {
                        File.Move(outputFile, outputBackupFile);
                        Console.WriteLine($"move outputBackupFile {ouputFilename} To outputBackupPath");
                    }
                    else
                    {
                        Console.WriteLine($"File not found - {ouputFilename}");
                    }
                }
            }

            Console.WriteLine("p04_Run_WHONET : End");
        }

        private static void NARST_Run_WHONET()
        {
            Console.WriteLine("p04_Run_WHONET : Start");

            foreach (var item in processRequestList)
            {
                string[] filePaths = Directory.GetFiles(Path.Combine(whonetPath, "Macros", "Template"), "*.mcr");

                ProcessStartInfo processInfo;
                System.Diagnostics.Process process;
                string command;

                int exitCode;
                string output;
                string error;

                string macroBackupPath = Path.Combine(whonetPath, "Macros", (item.pcr_code));
                if (Directory.Exists(macroBackupPath) == false)
                {
                    Directory.CreateDirectory(macroBackupPath);
                }
                else
                {
                    Directory.GetFiles(macroBackupPath).ToList().ForEach(macroFile => File.Delete(macroFile));
                }

                string outputBackupPath = Path.Combine(whonetPath, "Output", (item.pcr_code));
                if (Directory.Exists(outputBackupPath) == false)
                {
                    Directory.CreateDirectory(outputBackupPath);
                }
                else
                {
                    Directory.GetFiles(outputBackupPath).ToList().ForEach(outputFile => File.Delete(outputFile));
                }

                foreach (var macroFile in filePaths)
                {
                    //string macrofilename = Path.Combine(whonetPath, "macros", "macrotemplate.mcr");
                    //string newmacroname = $"macro-{item.pcr_code}";
                    //string newmacrofilename = macrofilename.Replace("macrotemplate", newmacroname);

                    string newmacroFilename = macroFile.Replace("Template\\", item.pcr_code + "-");
                    string newmacroName = newmacroFilename.Replace(Path.Combine(whonetPath, "Macros"), "").Replace("\\", "").Replace(".mcr", "");

                    string dataFilename = string.Join(",", item.processRequestDetails.Select(x => $"{item.pcr_code}-{x.pcd_hos_code}-{x.pcd_lab_code}.sqlite"));
                    string ouputFilename = $"output-{newmacroName}.db";

                    if (File.Exists(newmacroFilename)) File.Delete(newmacroFilename);
                    File.Copy(macroFile, newmacroFilename);
                    //string text = File.ReadAllText(newmacrofilename);
                    //text = text.Replace("{MacroName}", newmacroname);
                    //text = text.Replace("{Datafile}", datafilename);
                    //text = text.Replace("{Output}", "SQLITE File (" + ouputfilename + ")");
                    //File.WriteAllText(newmacrofilename, text);

                    string[] message = File.ReadAllLines(newmacroFilename);
                    message[0] = $"Macro Name = {newmacroName}";
                    File.WriteAllLines(newmacroFilename, message);
                    string appendText = Environment.NewLine + $"Data file = {dataFilename}";
                    File.AppendAllText(newmacroFilename, appendText, System.Text.Encoding.UTF8);
                    appendText = Environment.NewLine + $"Output = SQLITE File (\"{ouputFilename}\")";
                    File.AppendAllText(newmacroFilename, appendText, System.Text.Encoding.UTF8);

                    command = Path.Combine(whonetPath, "WHONET.EXE") + $" \"{newmacroName}.mcr\"";
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

                    if (ouputFilename.Contains(","))
                    {
                        var tmpfilename = ouputFilename.Split(',')[0];
                        if (File.Exists(tmpfilename)) File.Move(tmpfilename, ouputFilename);
                    }

                    string macroBackupFile = Path.Combine(macroBackupPath, $"{newmacroName}.mcr");
                    File.Move(newmacroFilename, macroBackupFile);
                    Console.WriteLine($"move macroBackupFile {newmacroName}.mcr To macroBackupPath");

                    string outputFile = Path.Combine(whonetPath, "Output", ouputFilename);
                    string outputBackupFile = Path.Combine(outputBackupPath, ouputFilename);
                    if (File.Exists(outputFile))
                    {
                        File.Move(outputFile, outputBackupFile);
                        Console.WriteLine($"move outputBackupFile {ouputFilename} To outputBackupPath");
                    }
                    else
                    {
                        Console.WriteLine($"File not found - {ouputFilename}");
                    }
                }
            }

            Console.WriteLine("p04_Run_WHONET : End");
        }

        private static void NARST_Insert_Result()
        {
            Console.WriteLine("p05_Insert_Result_Data : Start");

            var currentDateTime = DateTime.Now;

            //string ouputfilename = $"output-{item.pcr_code}*.db";
            string[] folderPaths = Directory.GetDirectories(Path.Combine(whonetPath, "Output"), "P_HOS_*");

            foreach (var folder in folderPaths)
            {
                DirectoryInfo directory = new DirectoryInfo(folder);
                FileInfo[] infos = directory.GetFiles();

                foreach (FileInfo file in infos)
                {
                    Console.WriteLine("p05_Insert_Result_Data : " + file.Directory.Name);

                    List<TRProcessDataResult> objList = new List<TRProcessDataResult>();
                    var detailCount = 1;

                    string folderName = file.Directory.Name;
                    var folderNameSplit = folderName.Split('_');
                    //string arh_code = folderName.Substring(6, 2);
                    //string prv_code = folderName.Substring(6, 2);
                    //string hos_code = folderName.Substring(6, 9);
                    //string year = folderName.Substring(20, 4);
                    //string month_start = folderName.Substring(25, 2);
                    //string month_end = folderName.Substring(28, 2);
                    //string hos_code = folderNameSplit[2];
                    string hos_code = folderNameSplit[2];
                    string year = folderNameSplit[4];
                    string month_start = folderNameSplit[5];
                    string month_end = folderNameSplit[6];

                    var processData = new TRProcessRequest();
                    using (var _db = new ProcessContext())
                    {
                        using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            try
                            {
                                //processData = _db.TRProcessRequests.FirstOrDefault(x => x.pcr_arh_code == arh_code && string.IsNullOrEmpty(x.pcr_prv_code) && string.IsNullOrEmpty(x.pcr_hos_code) && x.pcr_year == year && x.pcr_month_start == month_start && x.pcr_month_end == month_end);
                                //processData = _db.TRProcessRequests.FirstOrDefault(x => x.pcr_prv_code == prv_code && x.pcr_hos_code == null && x.pcr_year == year && x.pcr_month_start == month_start && x.pcr_month_end == month_end && x.pcr_status == "W");
                                processData = _db.TRProcessRequests.FirstOrDefault(x => x.pcr_hos_code == hos_code && x.pcr_year == year && x.pcr_month_start == month_start && x.pcr_month_end == month_end && x.pcr_status == "A");

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

                    if (processData != null)
                    {
                        Console.WriteLine("p05_Insert_Result_Data : " + processData.pcr_code);

                        string[] filenameSplit = file.Name.Split('-');
                        string specimen = filenameSplit[4];
                        string pathogen = file.Name.Replace(filenameSplit[0] + "-" + filenameSplit[1] + "-" + filenameSplit[2] + "-" + filenameSplit[3] + "-" + filenameSplit[4] + "-", "").Replace(".db", "");

                        var data = SQLiteDataAccess.LoadOutput(file.DirectoryName, file.Name, detailCount.ToString());

                        detailCount++;
                        foreach (var outputItem in data)
                        {
                            objList.Add(new TRProcessDataResult()
                            {
                                pdr_pcr_code = processData.pcr_code,
                                pdr_arh_code = processData.pcr_arh_code,
                                pdr_prv_code = processData.pcr_prv_code,
                                pdr_hos_code = processData.pcr_hos_code,
                                pdr_lab_code = processData.pcr_lab_code,
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
                                pdr_createdate = currentDateTime
                            });

                            Console.WriteLine("Create_ProcessRequest Insert : " + processData.pcr_code);
                        }
                        using (var _db = new ProcessContext())
                        {
                            using (var trans = _db.Database.BeginTransaction())
                            {
                                try
                                {
                                    _db.BulkInsert(objList);
                                    _db.SaveChanges();

                                    trans.Commit();
                                }
                                catch (Exception ex)
                                {
                                    trans.Rollback();
                                    Console.WriteLine("Create_ProcessRequest BulkInsert : Error " + ex.Message);
                                }
                                finally
                                {
                                    trans.Dispose();
                                    _db.Dispose();

                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();

                                    Console.WriteLine("Create_ProcessRequest BulkInsert : Success");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("p05_Insert_Result_Data : processData Not found");
                    }
                }
            }

            Console.WriteLine("p05_Insert_Result_Data : End");
        }

        private static void GLASS_Run_WHONET()
        {
            Console.WriteLine("p04_Run_WHONET : Start");

            foreach (var item in processRequestList)
            {
                string[] filePaths = Directory.GetFiles(Path.Combine(whonetPath, "Macros", "Template"), "*.mcr");

                ProcessStartInfo processInfo;
                System.Diagnostics.Process process;
                string command;

                int exitCode;
                string output;
                string error;

                string macroBackupPath = Path.Combine(whonetPath, "Macros", item.pcr_code);
                if (Directory.Exists(macroBackupPath) == false)
                {
                    Directory.CreateDirectory(macroBackupPath);
                }
                else
                {
                    Directory.GetFiles(macroBackupPath).ToList().ForEach(macroFile => File.Delete(macroFile));
                }

                string outputBackupPath = Path.Combine(whonetPath, "Output", item.pcr_code);
                if (Directory.Exists(outputBackupPath) == false)
                {
                    Directory.CreateDirectory(outputBackupPath);
                }
                else
                {
                    Directory.GetFiles(outputBackupPath).ToList().ForEach(outputFile => File.Delete(outputFile));
                }

                foreach (var macroFile in filePaths)
                {
                    //string macrofilename = Path.Combine(whonetPath, "macros", "macrotemplate.mcr");
                    //string newmacroname = $"macro-{item.pcr_code}";
                    //string newmacrofilename = macrofilename.Replace("macrotemplate", newmacroname);

                    string newmacroFilename = macroFile.Replace("Template\\", item.pcr_code + "-");
                    string newmacroName = newmacroFilename.Replace(Path.Combine(whonetPath, "Macros"), "").Replace("\\", "").Replace(".mcr", "");

                    string dataFilename = string.Join(",", item.processRequestDetails.Select(x => $"{item.pcr_code}-{x.pcd_hos_code}-{x.pcd_lab_code}.sqlite"));
                    string ouputFilename = $"output-{newmacroName}.db";

                    if (File.Exists(newmacroFilename)) File.Delete(newmacroFilename);
                    File.Copy(macroFile, newmacroFilename);
                    //string text = File.ReadAllText(newmacrofilename);
                    //text = text.Replace("{MacroName}", newmacroname);
                    //text = text.Replace("{Datafile}", datafilename);
                    //text = text.Replace("{Output}", "SQLITE File (" + ouputfilename + ")");
                    //File.WriteAllText(newmacrofilename, text);

                    string[] message = File.ReadAllLines(newmacroFilename);
                    message[0] = $"Macro Name = {newmacroName}";
                    File.WriteAllLines(newmacroFilename, message);
                    string appendText = Environment.NewLine + $"Data file = {dataFilename}";
                    File.AppendAllText(newmacroFilename, appendText, System.Text.Encoding.UTF8);
                    appendText = Environment.NewLine + $"Output = SQLITE File (\"{ouputFilename}\")";
                    File.AppendAllText(newmacroFilename, appendText, System.Text.Encoding.UTF8);

                    command = Path.Combine(whonetPath, "WHONET.EXE") + $" \"{newmacroName}.mcr\"";
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

                    if (ouputFilename.Contains(","))
                    {
                        var tmpfilename = ouputFilename.Split(',')[0];
                        if (File.Exists(tmpfilename)) File.Move(tmpfilename, ouputFilename);
                    }

                    string macroBackupFile = Path.Combine(macroBackupPath, $"{newmacroName}.mcr");
                    File.Move(newmacroFilename, macroBackupFile);
                    Console.WriteLine($"move macroBackupFile {newmacroName}.mcr To macroBackupPath");

                    string outputFile = Path.Combine(whonetPath, "Output", ouputFilename);
                    string outputBackupFile = Path.Combine(outputBackupPath, ouputFilename);
                    if (File.Exists(outputFile))
                    {
                        File.Move(outputFile, outputBackupFile);
                        Console.WriteLine($"move outputBackupFile {ouputFilename} To outputBackupPath");
                    }
                    else
                    {
                        Console.WriteLine($"File not found - {ouputFilename}");
                    }
                }
            }

            Console.WriteLine("p04_Run_WHONET : End");
        }

        private static void GLASS_Insert_Result()
        {
            Console.WriteLine("p05_Insert_Result_Data : Start");

            var currentDateTime = DateTime.Now;
            foreach (var item in processRequestList)
            {
                //string ouputfilename = $"output-{item.pcr_code}*.db";
                string[] filePaths = Directory.GetFiles(Path.Combine(whonetPath, "Output", item.pcr_code));

                foreach (var outputFile in filePaths)
                {
                    List<TRProcessDataResult> objList = new List<TRProcessDataResult>();
                    var detailCount = 1;

                    foreach (var itemDetail in item.processRequestDetails)
                    {
                        string filepath = Path.Combine(whonetPath, "Output");

                        string ouputfilename = outputFile.Replace(Path.Combine(whonetPath, "Output", item.pcr_code), "").Replace("\\", "");
                        var data = SQLiteDataAccess.LoadOutput(filepath, ouputfilename, detailCount.ToString());
                        string specimen = ouputfilename.Split('-')[5];
                        string pathogen = ouputfilename.Split("-" + specimen + "-").Last().Replace(".db", "");

                        detailCount++;
                        foreach (var outputItem in data)
                        {
                            objList.Add(new TRProcessDataResult()
                            {
                                pdr_pcr_code = item.pcr_code,
                                pdr_arh_code = item.pcr_arh_code,
                                pdr_prv_code = item.pcr_prv_code,
                                pdr_hos_code = item.pcr_hos_code,
                                pdr_lab_code = itemDetail.pcd_lab_code,
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
                                pdr_createdate = currentDateTime
                            });
                        }
                    }
                    using (var _db = new ProcessContext())
                    {
                        using (var trans = _db.Database.BeginTransaction())
                        {
                            try
                            {
                                _db.BulkInsert(objList);
                                _db.SaveChanges();

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
            }

            Console.WriteLine("p05_Insert_Result_Data : End");
        }

        private static void Gen_EXCEL_File_New(TRProcessRequest processRequest)
        {
            Console.WriteLine("p06_Gen_EXCEL_File : Start");

            List<TRProcessDataResult> objList = new List<TRProcessDataResult>();

            var currentDateTime = DateTime.Now;
            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        objList = _db.TRProcessDataResults.Where(x => x.pdr_pcr_code == processRequest.pcr_code).ToList();

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

            All_Fill_S_New(processRequest, objList);

            Console.WriteLine("p06_Gen_EXCEL_File : End");
        }

        private static void Gen_EXCEL_File()
        {
            Console.WriteLine("p06_Gen_EXCEL_File : Start");

            List<TRProcessDataResult> objList = new List<TRProcessDataResult>();

            var currentDateTime = DateTime.Now;
            foreach (var item in processRequestList)
            {
                using (var _db = new ProcessContext())
                {
                    using (var trans = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            objList = _db.TRProcessDataResults.Where(x => x.pdr_pcr_code == item.pcr_code).ToList();

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

                All_Fill_S(item, objList);
            }

            Console.WriteLine("p06_Gen_EXCEL_File : End");
        }

        private static void Update_Report_Data()
        {
            Console.WriteLine("p07_Update_Report_Data : Start");



            Console.WriteLine("p07_Update_Report_Data : End");
        }

        public static void All_Fill_S_New(TRProcessRequest item, List<TRProcessDataResult> objList)
        {
            string excelFilename = Path.Combine(whonetPath, "ExcelFile", "template antibiogram 2020.xlsx");
            string newExcelname = $"excel-{item.pcr_code}";
            string newExcelFilename = excelFilename.Replace("template", newExcelname);

            var sheetList = new List<string>() { "All Specimen", "Blood", "Sputum", "Urine", "Stool" };

            var processColumnList = new List<TCProcessExcelColumn>();
            var processRowList = new List<TCProcessExcelRow>();
            var processTemplateList = new List<TCProcessExcelTemplate>();

            if (File.Exists(newExcelFilename)) File.Delete(newExcelFilename);
            File.Copy(excelFilename, newExcelFilename);

            FileInfo file = new FileInfo(newExcelFilename);

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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
                                processColumnList = _db.TCProcessExcelColumnModel.Where(x => x.pec_sheet_name == sheet).ToList();
                                processRowList = _db.TCProcessExcelRowModel.Where(x => x.per_sheet_name == sheet).ToList();
                                processTemplateList = _db.TCProcessExcelTemplateModel.Where(x => x.pet_sheet_name == sheet).ToList();

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

                    foreach (var itemTemplate in processTemplateList.OrderBy(x => x.pet_row_num).ThenBy(x => x.pet_col_num))
                    {
                        var valueRow = itemTemplate.pet_row_num.Value;
                        var valueCol = itemTemplate.pet_col_num.Value;

                        var processColumn = processColumnList.FirstOrDefault(x => x.pec_col_num == valueCol);
                        var processRow = processRowList.FirstOrDefault(x => x.per_row_num == valueRow);

                        var drug_code = processColumn.pec_ant_code;
                        var site_inf = itemTemplate.pet_site_inf;

                        if (processColumn != null && processRow != null)
                        {
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

                            //if (itemTemplate.pet_f == true)
                            //{
                            //    drug_code = "FOX";
                            //}

                            var cellValueList = objList.Where(x => x.pdr_specimen == specimen && x.pdr_pathogen == pathogen && x.pdr_drug_code == drug_code && (string.IsNullOrEmpty(site_inf) || x.pdr_site_inf == site_inf)).ToList();
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
                                worksheet.Cells[valueRow, valueCol, (valueRow + 1), valueCol].Style.Fill.BackgroundColor.SetColor(Get_Background_Color(perc_susc));

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
                }

                package.Save();
            }
        }

        public static void All_Fill_S(TRProcessRequestDTO item, List<TRProcessDataResult> objList)
        {
            string excelFilename = Path.Combine(whonetPath, "ExcelFile", "template antibiogram 2020.xlsx");
            string newExcelname = $"excel-{item.pcr_code}";
            string newExcelFilename = excelFilename.Replace("template", newExcelname);

            var sheetList = new List<string>() { "All Specimen", "Blood", "Sputum", "Urine", "Stool" };

            var processColumnList = new List<TCProcessExcelColumn>();
            var processRowList = new List<TCProcessExcelRow>();
            var processTemplateList = new List<TCProcessExcelTemplate>();

            if (File.Exists(newExcelFilename)) File.Delete(newExcelFilename);
            File.Copy(excelFilename, newExcelFilename);

            FileInfo file = new FileInfo(newExcelFilename);

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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
                                processColumnList = _db.TCProcessExcelColumnModel.Where(x => x.pec_sheet_name == sheet).ToList();
                                processRowList = _db.TCProcessExcelRowModel.Where(x => x.per_sheet_name == sheet).ToList();
                                processTemplateList = _db.TCProcessExcelTemplateModel.Where(x => x.pet_sheet_name == sheet).ToList();

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

                    foreach (var itemTemplate in processTemplateList.OrderBy(x => x.pet_row_num).ThenBy(x => x.pet_col_num))
                    {
                        var valueRow = itemTemplate.pet_row_num.Value;
                        var valueCol = itemTemplate.pet_col_num.Value;

                        var processColumn = processColumnList.FirstOrDefault(x => x.pec_col_num == valueCol);
                        var processRow = processRowList.FirstOrDefault(x => x.per_row_num == valueRow);

                        var drug_code = processColumn.pec_ant_code;
                        var site_inf = itemTemplate.pet_site_inf;

                        if (processColumn != null && processRow != null)
                        {
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

                            //if (itemTemplate.pet_f == true)
                            //{
                            //    drug_code = "FOX";
                            //}

                            var cellValueList = objList.Where(x => x.pdr_specimen == specimen && x.pdr_pathogen == pathogen && x.pdr_drug_code == drug_code && (string.IsNullOrEmpty(site_inf) || x.pdr_site_inf == site_inf)).ToList();
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
                                worksheet.Cells[valueRow, valueCol, (valueRow + 1), valueCol].Style.Fill.BackgroundColor.SetColor(Get_Background_Color(perc_susc));

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
                }

                package.Save();
            }
        }

        private static Color Get_Background_Color(double value)
        {
            if (value > 90)
            {
                return Color.FromArgb(0, 176, 80);
            }
            else if (value > 80)
            {
                return Color.FromArgb(255, 255, 0);
            }
            else if (value > 70)
            {
                return Color.FromArgb(217, 122, 87);
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

        public static void All_Fill_DrugName(ref ExcelWorksheet worksheet)
        {
            worksheet.Cells[3, 4].Value = "PENICILLIN";
            worksheet.Cells[3, 5].Value = "PENICILLIN BY MIC";
            worksheet.Cells[3, 6].Value = "AMPICILLIN";
            worksheet.Cells[3, 7].Value = "AMOXICILLIN/                 CLAVULANIC ACID";
            worksheet.Cells[3, 8].Value = "AMPICILLIN /                  SULBACTAM";
            worksheet.Cells[3, 9].Value = "PIPERACILLIN /                TAZOBACTAM";
            worksheet.Cells[3, 10].Value = "CEFAZOLIN (A)";
            worksheet.Cells[3, 11].Value = "CEFAZOLIN (U)";
            worksheet.Cells[3, 12].Value = "CEFUROXIME SODIUM   (parenteral)";
            worksheet.Cells[3, 13].Value = "CEFUROXIME SODIUM   (Oral)";
            worksheet.Cells[3, 14].Value = "CEFOPERAZONE /           SULBACTAM                    a";
            worksheet.Cells[3, 15].Value = "CEFOTAXIME";
            worksheet.Cells[3, 16].Value = "CEFOTAXIME BY MIC";
            worksheet.Cells[3, 17].Value = "CEFTAZIDIME";
            worksheet.Cells[3, 18].Value = "CEFTAZIDIME BY MIC";
            worksheet.Cells[3, 19].Value = "CEFTRIAXONE ";
            worksheet.Cells[3, 20].Value = "CEFTRIAXONE BY MIC";
            worksheet.Cells[3, 21].Value = "CEFEPIME";
            worksheet.Cells[3, 22].Value = "OXACILLIN";
            worksheet.Cells[3, 23].Value = "CEFOXITIN";
            worksheet.Cells[3, 24].Value = "ERTAPENEM";
            worksheet.Cells[3, 25].Value = "IMIPENEM";
            worksheet.Cells[3, 26].Value = "MEROPENEM";
            worksheet.Cells[3, 27].Value = "COLISTIN BY MIC";
            worksheet.Cells[3, 28].Value = "CIPROFLOXACIN";
            worksheet.Cells[3, 29].Value = "CIPROFLOXACIN BY MIC";
            worksheet.Cells[3, 30].Value = "LEVOFLOXACIN";
            worksheet.Cells[3, 31].Value = "AMIKACIN";
            worksheet.Cells[3, 32].Value = "GENTAMICIN";
            worksheet.Cells[3, 33].Value = "GENTAMICIN 120 mg";
            worksheet.Cells[3, 34].Value = "VANCOMYCIN";
            worksheet.Cells[3, 35].Value = "VANCOMYCIN BY MIC";
            worksheet.Cells[3, 36].Value = "TEICOPLANIN";
            worksheet.Cells[3, 37].Value = "FOSFOMYCIN";
            worksheet.Cells[3, 38].Value = "CLINDAMYCIN";
            worksheet.Cells[3, 39].Value = "CLINDAMYCIN BY MIC";
            worksheet.Cells[3, 40].Value = "ERYTHROMYCIN";
            worksheet.Cells[3, 41].Value = "ERYTHROMYCIN BY MIC";
            worksheet.Cells[3, 42].Value = "NITROFURANTOIN (U)";
            worksheet.Cells[3, 43].Value = "CHLORAMPHENICOL";
            worksheet.Cells[3, 44].Value = "CO-TRIMOXAZOLE";
            worksheet.Cells[3, 45].Value = "TETRACYCLINE";
        }

        public static void All_Fill_DrugCode(ref ExcelWorksheet worksheet)
        {
            worksheet.Cells[3, 4].Value = "PEN";
            worksheet.Cells[3, 5].Value = "PEN_NM";
            worksheet.Cells[3, 6].Value = "AMP";
            worksheet.Cells[3, 7].Value = "AMC";
            worksheet.Cells[3, 8].Value = "SAM";
            worksheet.Cells[3, 9].Value = "TZP";
            worksheet.Cells[3, 10].Value = "CZO";
            worksheet.Cells[3, 11].Value = "CZO";
            worksheet.Cells[3, 12].Value = "CXM";
            worksheet.Cells[3, 13].Value = "CXA";
            worksheet.Cells[3, 14].Value = "CSL";
            worksheet.Cells[3, 15].Value = "CTX";
            worksheet.Cells[3, 16].Value = "CTX_NM";
            worksheet.Cells[3, 17].Value = "CAZ";
            worksheet.Cells[3, 18].Value = "CAZ";
            worksheet.Cells[3, 19].Value = "CRO";
            worksheet.Cells[3, 20].Value = "CRO_NM";
            worksheet.Cells[3, 21].Value = "FEP";
            worksheet.Cells[3, 22].Value = "OXA";
            worksheet.Cells[3, 23].Value = "FOX";
            worksheet.Cells[3, 24].Value = "ETP";
            worksheet.Cells[3, 25].Value = "IPM";
            worksheet.Cells[3, 26].Value = "MEM";
            worksheet.Cells[3, 27].Value = "COL_NM";
            worksheet.Cells[3, 28].Value = "CIP";
            worksheet.Cells[3, 29].Value = "CIP_NM";
            worksheet.Cells[3, 30].Value = "LVX";
            worksheet.Cells[3, 31].Value = "AMK";
            worksheet.Cells[3, 32].Value = "GEN";
            worksheet.Cells[3, 33].Value = "GEH";
            worksheet.Cells[3, 34].Value = "VAN";
            worksheet.Cells[3, 35].Value = "VAN_NM";
            worksheet.Cells[3, 36].Value = "TEC";
            worksheet.Cells[3, 37].Value = "FOS";
            worksheet.Cells[3, 38].Value = "CLI";
            worksheet.Cells[3, 39].Value = "CLI_NM";
            worksheet.Cells[3, 40].Value = "ERY";
            worksheet.Cells[3, 41].Value = "ERY_NM";
            worksheet.Cells[3, 42].Value = "NIT";
            worksheet.Cells[3, 43].Value = "CHL";
            worksheet.Cells[3, 44].Value = "SXT";
            worksheet.Cells[3, 45].Value = "TCY";
        }

        public static void RenameAll_SpecimenPathogen(ref ExcelWorksheet worksheet)
        {
            worksheet.Cells[6, 2].Value = "Acinetobacter calcoaceticus-baumannii complex-ICU";
            worksheet.Cells[8, 2].Value = "Acinetobacter calcoaceticus-baumannii complex-Inpatient";
            worksheet.Cells[10, 2].Value = "Acinetobacter calcoaceticus-baumannii complex-Outpatient";
            worksheet.Cells[28, 2].Value = "Escherichia coli";
            worksheet.Cells[30, 2].Value = "Escherichia coli-Urine-Inpatient";
            worksheet.Cells[32, 2].Value = "Escherichia coli-Urine-Outpatient";
            worksheet.Cells[34, 2].Value = "Haemophilus influenzae";
            worksheet.Cells[36, 2].Value = "Haemophilus influenzae-Age 0-5";
            worksheet.Cells[38, 2].Value = "Haemophilus influenzae-Age 5-";
            worksheet.Cells[40, 2].Value = "Haemophilus influenzae-CSF";
            worksheet.Cells[42, 2].Value = "Haemophilus influenzae-Sterile  sites";
            worksheet.Cells[44, 2].Value = "Haemophilus influenzae-Non-sterile sites";
            worksheet.Cells[46, 2].Value = "Enterobacter aerogenes";
            worksheet.Cells[50, 2].Value = "Klebsiella pneumoniae";
            worksheet.Cells[52, 2].Value = "Klebsiella pneumoniae-ICU";
            worksheet.Cells[54, 2].Value = "Klebsiella pneumoniae-Inpatient";
            worksheet.Cells[56, 2].Value = "Klebsiella pneumoniae-Outpatient";
            worksheet.Cells[60, 2].Value = "Neisseria meningitidis";
            worksheet.Cells[76, 2].Value = "Salmonella spp.";
            worksheet.Cells[84, 2].Value = "Vibrio cholerae-all serotypes";
            worksheet.Cells[100, 2].Value = "Staphylococcus aureus";
            worksheet.Cells[102, 2].Value = "Staphylococcus aureus-MRSA";
            worksheet.Cells[104, 2].Value = "Staphylococcus aureus-MSSA";
            worksheet.Cells[106, 2].Value = "Staphylococcus aureus-ICU";
            worksheet.Cells[108, 2].Value = "Staphylococcus aureus-inpatient";
            worksheet.Cells[110, 2].Value = "Staphylococcus aureus-Outpatient";
            worksheet.Cells[114, 2].Value = "Staphylococcus, coagulase negative-Blood";
            worksheet.Cells[118, 2].Value = "Streptococcus, beta-haem. not Group A,B";
            worksheet.Cells[122, 2].Value = "Streptococcus pneumoniae";
            worksheet.Cells[124, 2].Value = "Streptococcus pneumoniae-Age 0-5";
            worksheet.Cells[126, 2].Value = "Streptococcus pneumoniae-Age 5-";
            worksheet.Cells[128, 2].Value = "Streptococcus pneumoniae-Sterile  sites";
            worksheet.Cells[130, 2].Value = "Streptococcus pneumoniae-Non-sterile  sites";
            worksheet.Cells[132, 2].Value = "Streptococcus pneumoniae-Meningitis";
            worksheet.Cells[134, 2].Value = "Streptococcus pneumoniae-Nonmeningitis";
        }

        public static void RenameAll_SpecimenPathogenBack(ref ExcelWorksheet worksheet)
        {
            worksheet.Cells[6, 2].Value = "                        (ICU)";
            worksheet.Cells[8, 2].Value = "                        (inpatient)";
            worksheet.Cells[10, 2].Value = "                        (outpatient)";
            worksheet.Cells[28, 2].Value = "Escherichia coli (all isolates)";
            worksheet.Cells[30, 2].Value = "                        (Urine-inpatient)";
            worksheet.Cells[32, 2].Value = "                        (Urine-outpatient)";
            worksheet.Cells[34, 2].Value = "Haemophilus influenzae (all isolates)";
            worksheet.Cells[36, 2].Value = "                        (age 0-5 years old)";
            worksheet.Cells[38, 2].Value = "                        (age > 5 years old)";
            worksheet.Cells[40, 2].Value = "                        (CSF)";
            worksheet.Cells[42, 2].Value = "                        (Sterile sites) b";
            worksheet.Cells[44, 2].Value = "                        (Non-sterile sites) c";
            worksheet.Cells[46, 2].Value = "Klebsiella aerogenes";
            worksheet.Cells[50, 2].Value = "Klebsiella pneumoniae (all isolates)";
            worksheet.Cells[52, 2].Value = "                        (ICU)";
            worksheet.Cells[54, 2].Value = "                        (inpatient)";
            worksheet.Cells[56, 2].Value = "                        (outpatient)";
            worksheet.Cells[60, 2].Value = "Neiseria meningitidis";
            worksheet.Cells[76, 2].Value = "Salmonella spp.";
            worksheet.Cells[84, 2].Value = "Vibrio cholerae (all serotypes)";
            worksheet.Cells[100, 2].Value = "Staphylococcus aureus (all isolates)";
            worksheet.Cells[102, 2].Value = "                        (MRSA)";
            worksheet.Cells[104, 2].Value = "                        (MSSA)";
            worksheet.Cells[106, 2].Value = "                        (ICU)";
            worksheet.Cells[108, 2].Value = "                        (inpatient)";
            worksheet.Cells[110, 2].Value = "                        (outpatient)";
            worksheet.Cells[114, 2].Value = "Staphylococcus, coagulase negative (blood)";
            worksheet.Cells[118, 2].Value = "Streptococcus,ß-hemolytic not Group A,B";
            worksheet.Cells[122, 2].Value = "Streptococcus pneumoniae (all isolates)";
            worksheet.Cells[124, 2].Value = "                        (age 0-5 years old)";
            worksheet.Cells[126, 2].Value = "                        (age > 5 years old)";
            worksheet.Cells[128, 2].Value = "                        (Sterile sites) b";
            worksheet.Cells[130, 2].Value = "                        (Non-sterile sites) c";
            worksheet.Cells[132, 2].Value = "                        (Meningitis: by E-test)";
            worksheet.Cells[134, 2].Value = "                        (Nonmeningitis: by E-test)";
        }

        public static void Update_Status()
        {
            Console.WriteLine("p0501_Update_Status : Start");

            var mnu_code = "MNU_0601";

            foreach (var item in processRequestList)
            {
                using (var _db = new ProcessContext())
                {
                    using (var trans = _db.Database.BeginTransaction())
                    {
                        try
                        {

                            var objModel = _db.TRProcessRequests.FirstOrDefault(x => x.pcr_code == item.pcr_code);
                            if (objModel != null)
                            {

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
            }

            Console.WriteLine("p0501_Update_Status : End");
        }



        public void CreateNoticeMessageAll(string mnu_code, string message)
        {
            //log.MethodStart();

            try
            {
                var userLoginList = UserLoginPermission_GetList(new TCUserLoginPermission() { usp_active = true });

                foreach (var item in userLoginList.Select(x => x.usp_usr_userName).Distinct())
                {
                    var objData = new TRNoticeMessage()
                    {
                        noti_username = item,
                        noti_mnu_code = mnu_code,
                        noti_message = message
                    };

                    TRNoticeMessage_Insert(objData);
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle failure

                //log.Error(ex);
            }
            finally
            {

            }

            //log.MethodFinish();

            //return objList;
        }

        public void CreateNoticeMessageByRole(string mnu_code, string rol_code, string message)
        {
            //log.MethodStart();

            try
            {
                var userLoginPermissionList = UserLoginPermission_GetList(new TCUserLoginPermission() { usp_rol_code = rol_code });

                foreach (var item in userLoginPermissionList)
                {
                    var objData = new TRNoticeMessage()
                    {
                        noti_username = item.usp_usr_userName,
                        noti_mnu_code = mnu_code,
                        noti_message = message
                    };

                    TRNoticeMessage_Insert(objData);
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle failure

                //log.Error(ex);
            }
            finally
            {

            }

            //log.MethodFinish();

            //return objList;
        }

        public void CreateNoticeMessageByArea(string mnu_code, string arh_code, string message)
        {
            //log.MethodStart();

            try
            {
                var userLoginPermissionList = UserLoginPermission_GetList(new TCUserLoginPermission() { usp_arh_code = arh_code });

                foreach (var item in userLoginPermissionList)
                {
                    var objData = new TRNoticeMessage()
                    {
                        noti_username = item.usp_usr_userName,
                        noti_mnu_code = mnu_code,
                        noti_message = message
                    };

                    TRNoticeMessage_Insert(objData);
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle failure

                //log.Error(ex);
            }
            finally
            {

            }

            //log.MethodFinish();

            //return objList;
        }

        public void CreateNoticeMessageByHospital(string mnu_code, string hos_code, string message)
        {
            //log.MethodStart();

            try
            {
                var userLoginPermissionList = UserLoginPermission_GetList(new TCUserLoginPermission() { usp_hos_code = hos_code });

                foreach (var item in userLoginPermissionList)
                {
                    var objData = new TRNoticeMessage()
                    {
                        noti_username = item.usp_usr_userName,
                        noti_mnu_code = mnu_code,
                        noti_message = message
                    };

                    TRNoticeMessage_Insert(objData);
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle failure

                //log.Error(ex);
            }
            finally
            {

            }

            //log.MethodFinish();

            //return objList;
        }

        public void CreateNoticeMessageByUser(string mnu_code, string username, string message)
        {
            //log.MethodStart();

            try
            {
                var objData = new TRNoticeMessage()
                {
                    noti_username = username,
                    noti_mnu_code = mnu_code,
                    noti_message = message
                };

                TRNoticeMessage_Insert(objData);
            }
            catch (Exception ex)
            {
                // TODO: Handle failure

                //log.Error(ex);
            }
            finally
            {

            }

            //log.MethodFinish();

            //return objList;
        }

        public List<TCUserLoginPermission> UserLoginPermission_GetList(TCUserLoginPermission searchModel)
        {
            //log.MethodStart();

            var objList = new List<TCUserLoginPermission>();

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        objList = _db.TCUserLoginPermissions.FromSqlRaw<TCUserLoginPermission>("sp_GET_TCUserLoginPermission {0}", searchModel).ToList();

                        _db.SaveChanges();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        // TODO: Handle failure
                        //log.Error(ex);

                        trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }

            //log.MethodFinish();

            return objList;
        }

        public void TRNoticeMessage_Insert(TRNoticeMessage model)
        {
            //log.MethodStart();

            var objData = new TRNoticeMessage();
            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        var result = _db.TRNoticeMessages.Add(model);

                        _db.SaveChanges();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        // TODO: Handle failure
                        //log.Error(ex);

                        trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }

            //log.MethodFinish();
        }

        private static void activeMasterTemplate()
        {
            TCMasterTemplate _currentTemplate = null;

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        //var _objectList = _db.TCMasterTemplates.Where(x => (x.mst_date_from <= DateTime.Now.Date && (x.mst_date_to == null ? DateTime.Now.Date : x.mst_date_to) >= DateTime.Now.Date)).OrderByDescending(x => x.mst_createdate);
                        var _objectList = _db.TCMasterTemplates.ToList();

                        if (_objectList.Count() > 0)
                        {
                            var _activeObject = _objectList.FirstOrDefault(x => x.mst_active == true);
                            var _object = _objectList.OrderByDescending(x => x.mst_date_from).FirstOrDefault(x => x.mst_date_from <= DateTime.Now.Date);
                            if (_activeObject != _object && !_object.mst_active)
                            {
                                Console.WriteLine($"INACTIVE_MASTER_TEMPLATE : START : MST_CODE "+ _activeObject.mst_code);
                                //Inactive previous template
                                _activeObject.mst_active = false;
                                _activeObject.mst_status = "I";
                                _activeObject.mst_updatedate = DateTime.Now;
                                _activeObject.mst_updateuser = "BATCH";
                                _db.TCMasterTemplates.Update(_activeObject);

                                #region Save Log Process ...
                                _db.LogProcesss.Add(new LogProcess()
                                {
                                    log_usr_id = "BATCH",
                                    log_mnu_id = "",
                                    log_mnu_name = "MasterTemplate",
                                    log_tran_id = _activeObject.mst_code,
                                    log_action = "New",
                                    log_desc = "Inactive MasterTemplate " + _activeObject.mst_code + " by batch",
                                    log_createuser = "BATCH",
                                    log_createdate = DateTime.Now
                                });
                                #endregion
                                Console.WriteLine($"INACTIVE_MASTER_TEMPLATE : END : MST_CODE " + _activeObject.mst_code);

                                Console.WriteLine($"ACTIVE_MASTER_TEMPLATE : START : MST_CODE " + _object.mst_code);
                                //Active current template
                                _object.mst_active = true;
                                _object.mst_status = "A";
                                _object.mst_updatedate = DateTime.Now;
                                _object.mst_updateuser = "BATCH";
                                _db.TCMasterTemplates.Update(_object);

                                #region Save Log Process ...
                                _db.LogProcesss.Add(new LogProcess()
                                {
                                    log_usr_id = "BATCH",
                                    log_mnu_id = "",
                                    log_mnu_name = "MasterTemplate",
                                    log_tran_id = _object.mst_code,
                                    log_action = "New",
                                    log_desc = "Active MasterTemplate " + _object.mst_code + " by batch",
                                    log_createuser = "BATCH",
                                    log_createdate = DateTime.Now
                                });
                                #endregion
                                Console.WriteLine($"ACTIVE_MASTER_TEMPLATE : END : MST_CODE " + _object.mst_code);

                                _db.SaveChanges();

                                _currentTemplate = _object;
                            }
                        }

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        // TODO: Handle failure
                        //log.Error(ex);

                        trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }

            var ftp_site = P00_BATCH_Service.GetConfigurationValue("WHONET:PARAM:FTP_SITE");
            if (_currentTemplate != null && (string.IsNullOrEmpty(ftp_site) == false))
            {
                Console.WriteLine("ALISS_FTP : Copy file to WHONET.");

                var whonetPath = P00_BATCH_Service.GetConfigurationValue("WHONET:PARAM:PATH");
                var mst_updatedate = _currentTemplate.mst_updatedate?.ToString("YYYYMMdd");
                var mst_code = _currentTemplate.mst_code;

                var filename = "LABTHA." + _currentTemplate.mst_version;
                var url = ftp_site + "/" + mst_updatedate + "/" + mst_code + "/" + filename;
                var destination = Path.Combine(whonetPath, filename);
                //CopyFileFromFTP(url, destination);

                filename = "DRGLST.TXT";
                url = ftp_site + "/" + mst_updatedate + "/" + mst_code + "/" + filename;
                destination = Path.Combine(whonetPath, "Codes", filename);
                //CopyFileFromFTP(url, destination);

                filename = "ORGLIST.TXT";
                url = ftp_site + "/" + mst_updatedate + "/" + mst_code + "/" + filename;
                destination = Path.Combine(whonetPath, "Codes", filename);
                //CopyFileFromFTP(url, destination);

                filename = "SPCLISTE.TXT";
                url = ftp_site + "/" + mst_updatedate + "/" + mst_code + "/" + filename;
                destination = Path.Combine(whonetPath, "Codes", filename);
                //CopyFileFromFTP(url, destination);

            }

        }

        private static void CopyFileFromFTP(string url, string destination)
        {
            var userName = "ALISS_FTP";
            var password = "#Control001234";

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                request.Credentials = new NetworkCredential(userName, password);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                using (var fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write))
                {
                    responseStream.CopyTo(fileStream);
                }

                responseStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error LABTHA : " + ex.Message);
            }
        }

    }
}
