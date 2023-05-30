using ALISS.Process.Batch.DataAccess;
using ALISS.Process.Batch.Model;
using ALISS.Process.Batch.SQLite;
//using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using EFCore.BulkExtensions;

namespace ALISS.Process.Batch
{
    public class P01_LabAlert_Service : IDisposable
    {
        private string whonetPath;
        private static int process_year;

        public void Dispose()
        {

        }

        public P01_LabAlert_Service()
        {
            whonetPath = P00_BATCH_Service.GetConfigurationValue("WHONET:PARAM:PATH");
            process_year = Convert.ToInt32(P00_BATCH_Service.GetConfigurationValue("WHONET:PARAM:PROCESS_YEAR"));
        }

        public void LabAlert_SERVICE_AUTO()
        {

        }

        public void LabAlert_SERVICE_MANUAL(string file_list)
        {
            Console.WriteLine($"LabAlert_SERVICE_MANUAL : START");

            var file_split = file_list.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var file_id in file_split)
            {
                Console.WriteLine($"LabAlert_SERVICE_MANUAL : START : {file_id}");

                var fileUploadData = GET_Data_LabFileUpload(x => x.lfu_id == new Guid(file_id));

                var processLabData = GET_Data_ProcessRequestLabData(x => x.pcl_hos_code == fileUploadData.lfu_hos_code && x.pcl_lab_code == fileUploadData.lfu_lab);

                if (processLabData == null)
                {
                    processLabData = INSERT_ProcessRequestLabData(fileUploadData.lfu_hos_code, fileUploadData.lfu_lab, fileUploadData.lfu_id);
                }

                LabAlert_SERVICE(processLabData);
            }

            Console.WriteLine($"LabAlert_SERVICE_MANUAL : END");
        }

        public void LabAlert_SERVICE(TRProcessRequestLabData processLabData)
        {
            CREATE_LabAlert_FILE_BEFORE(processLabData);

            RUN_LabAlert_COMMAND(processLabData);

            INSERT_RESULT_SERVICE(processLabData);
        }

        public TRLabFileUpload GET_Data_LabFileUpload(Func<TRLabFileUpload, bool> query)
        {
            Console.WriteLine($"GET_Data_LabFileUpload : START");

            var fileUpload = new TRLabFileUpload();

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        fileUpload = _db.TRLabFileUploads.FirstOrDefault(query);

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

            Console.WriteLine($"GET_Data_LabFileUpload : END");

            return fileUpload;
        }

        public TRProcessRequestLabData GET_Data_ProcessRequestLabData(Func<TRProcessRequestLabData, bool> query)
        {
            Console.WriteLine($"GET_LIST_ProcessRequestLabData : START");

            var processLabData= new TRProcessRequestLabData();

            using (var _db = new ProcessContext())
            {
                using (var trans = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        processLabData = _db.TRProcessRequestLabDatas.FirstOrDefault(query);

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

            return processLabData;
        }

        private TRProcessRequestLabData INSERT_ProcessRequestLabData(string hos_code, string lab_code, Guid file_id)
        {
            Console.WriteLine($"INSERT_ProcessRequestLabData : START : {hos_code}");

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
                            processLabData = new TRProcessRequestLabData()
                            {
                                pcl_arh_code = hospital.hos_arh_code,
                                pcl_prv_code = hospital.hos_prv_code,
                                pcl_hos_code = hos_code,
                                pcl_lab_code = lab_code,
                                pcl_lfu_id = file_id,
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

            Console.WriteLine($"INSERT_ProcessRequestLabData : END");

            return processLabData;
        }

        private void CREATE_LabAlert_FILE_BEFORE(TRProcessRequestLabData processLabData)
        {
            Console.WriteLine($"CREATE_LabAlert_FILE_BEFORE : Start : {processLabData.pcl_lfu_id}");

            DataTable dataTable = new DataTable();

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
                        cmd.CommandText = "EXEC sp_BATCH_PROC_Get_NARST_File @lfu_id = @lfu_id, @hos_code = @hos_code, @lab_code = @lab_code, @month = @month, @year = @year";
                        cmd.Parameters.Add(new SqlParameter("lfu_id", SqlDbType.UniqueIdentifier) { Value = processLabData.pcl_lfu_id });
                        cmd.Parameters.Add(new SqlParameter("hos_code", SqlDbType.NVarChar) { Value = null });
                        cmd.Parameters.Add(new SqlParameter("lab_code", SqlDbType.NVarChar) { Value = null });
                        cmd.Parameters.Add(new SqlParameter("month", SqlDbType.Int) { Value = null });
                        cmd.Parameters.Add(new SqlParameter("year", SqlDbType.Int) { Value = null });

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
            string datafilename = $"{processLabData.pcl_lfu_id.ToString().ToUpper()}-LabAlertData.sqlite";

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

            Console.WriteLine("CREATE_LabAlert_FILE_BEFORE : End");
        }

        private void RUN_LabAlert_COMMAND(TRProcessRequestLabData processLabData)
        {
            Console.WriteLine($"RUN_LabAlert_COMMAND : START : {processLabData.pcl_lfu_id}");

            string macroFile = Path.Combine(whonetPath, "Macros", P00_BATCH_Service.GetConfigurationValue("WHONET:NARST:PARAM:MACRO_TEMPLATE_FILE"));

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

            string newmacroFilename = macroFile.Replace("LabAlertTemplate", ("-" + processLabData.pcl_lfu_id + "-" + "LabAlert"));
            string newmacroName = newmacroFilename.Replace(Path.Combine(whonetPath, "Macros"), "").Replace("\\", "").Replace(".mcr", "");

            string dataFilename = $"{processLabData.pcl_lfu_id}-LabAlertData.sqlite";
            string ouputFilename = $"Output-{processLabData.pcl_lfu_id}-LabAlertData.db";

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

            Console.WriteLine($"RUN_LabAlert_COMMAND : END");
        }

        public void INSERT_RESULT_SERVICE(TRProcessRequestLabData processLabData)
        {
            Console.WriteLine($"INSERT_RESULT_SERVICE : Start : {processLabData.pcl_lfu_id}");

            var currentDateTime = DateTime.Now;

            var objList = new List<TRProcessDataListing>();

            var labFileId = processLabData.pcl_lfu_id.ToString();

            string filepath = Path.Combine(whonetPath, "Output", "LabAlert");
            string ouputfilename = $"Output-{labFileId}-LabAlertData.db";

            var data = SQLiteDataAccess.LoadOutputDataLabAlert(filepath, ouputfilename);

            foreach (var outputItem in data)
            {
                objList.Add(new TRProcessDataListing()
                {
                    pdl_pcr_code = processLabData.pcl_lfu_id?.ToString().ToUpper(),
                    pdl_arh_code = processLabData.pcl_arh_code,
                    pdl_prv_code = processLabData.pcl_prv_code,
                    pdl_hos_code = processLabData.pcl_hos_code,
                    pdl_lab_code = processLabData.pcl_lab_code,
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

            Console.WriteLine("INSERT_RESULT_SERVICE : End");
        }
    }
}
