using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;
using System.Runtime.InteropServices.ComTypes;
using ExcelDataReader;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Globalization;
using ALISS.HISUpload.Batch.Models;
using ALISS.HISUpload.Batch.DataAccess;
using ALISS.HISUpload.DTO;

namespace ALISS.HISUpload.Batch
{
    class Program
    {
        public IConfiguration Configuration { get; }
        static void Main(string[] args)
        {
            var HISDataDAL = new HISDataDAL();
            var HISFileUpload = HISDataDAL.Get_NewHISFileUpload('N');
            var HISFileUploadReprocess = HISDataDAL.Get_NewHISFileUpload('R');

            if (HISFileUploadReprocess.Count > 0)
            {
                HISFileUpload.AddRange(HISFileUploadReprocess);
            }
            string Param_RefNo = "";
            string Param_HNNo = "";
            string Param_LabNo = "";
            string Param_Date = "";
            int iDupRecord = 0;
            int iMatchRecord = 0;
            bool FIRST_ROW_IS_COLUMN = true;
            int iFileUploadID = 0;
            List<HISUploadDataDTO> HISFiles = new List<HISUploadDataDTO>();
            int dataFilePerRound = 3;
            int nfile = 0;

            try
            {
               
                if (HISFileUpload != null)
                {
                    if (HISFileUpload.Count() > dataFilePerRound)
                    {
                        for (var i = 0; i < dataFilePerRound; i++)
                        {
                            HISFiles.Add(HISFileUpload[i]);
                            var rowsAffected = HISDataDAL.Update_HISFileUploadStatus(HISFileUpload[i].hfu_id, 'P', 0, 0, "BATCH");
                            nfile += 1;
                        }
                    }
                    else
                    {
                        foreach (HISUploadDataDTO HIS in HISFileUpload)
                        {
                            HISFiles.Add(HIS);
                            var rowsAffected = HISDataDAL.Update_HISFileUploadStatus(HIS.hfu_id, 'P', 0, 0, "BATCH");
                            nfile += 1;
                        }
                    }
                        //Update status = P (Processing)
                    // foreach (HISUploadDataDTO HISFile in HISFileUpload)
                    //{
                    //    var rowsAffected = HISDataDAL.Update_HISFileUploadStatus(HISFile.hfu_id, 'P', 0, 0, "BATCH");
                    //}

                    List<HISFileTemplateDTO> TemplateHISActive = HISDataDAL.GetHISFileTemplate_Active();
                    if (TemplateHISActive.Count > 0)
                    {
                        Param_RefNo = TemplateHISActive.FirstOrDefault().hft_field1;
                        Param_HNNo = TemplateHISActive.FirstOrDefault().hft_field2;
                        Param_LabNo = TemplateHISActive.FirstOrDefault().hft_field3;
                        Param_Date = TemplateHISActive.FirstOrDefault().hft_field4;
                    }

                    Console.WriteLine(string.Format(DateTime.Now.ToString("HH:mm:ss") + " : Batch HIS Start {0}/{1} files ... ", nfile, HISFileUpload.Count()));
                    var log_start = new LogWriter(string.Format("Batch HIS Start {0}/{1} files ...", nfile, HISFileUpload.Count()));

                    foreach (HISUploadDataDTO HISFile in HISFiles)
                    {
                        iMatchRecord = 0;
                        iDupRecord = 0;
                        iFileUploadID = HISFile.hfu_id;
                        var strFileName = HISFile.hfu_file_name;
                        var strFilePath = HISFile.hfu_file_path;

                        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : " + string.Format(">> Hos : ({0}-{1}) File Name : {2}", HISFile.hfu_hos_code, HISFile.hfu_lab, HISFile.hfu_file_name));
                        var log_st = new LogWriter(string.Format(">> Hos : ({0}-{1}) File Name : {2}", HISFile.hfu_hos_code, HISFile.hos_name, HISFile.hfu_file_name));

                        if (!File.Exists(strFilePath))
                        {
                            var msgErr = string.Format("Path not found in file {0}", strFileName);
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : " + string.Format("Path not found in file {0}", strFileName));
                            var logw = new LogWriter(msgErr);
                            continue;
                        }                        

                        if (Path.GetExtension(strFileName) == ".xls" || Path.GetExtension(strFileName) == ".xlsx")
                        {

                            List<TRSTGHISFileUploadHeader> HISFileDataHeaderList = new List<TRSTGHISFileUploadHeader>();
                            List<TRSTGHISFileUploadDetail> HISFileDataDetailList = new List<TRSTGHISFileUploadDetail>();

                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Read File ... ");
                            var log_tran = new LogWriter("Start Read File ... ");

                            using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read))
                            {
                                DataSet result = new DataSet();

                                if (Path.GetExtension(strFileName) == ".xls" || Path.GetExtension(strFileName) == ".xlsx")
                                {
                                    IExcelDataReader reader;
                                    if(Path.GetExtension(strFileName) == ".xls")
                                    {
                                        reader = ExcelReaderFactory.CreateReader(stream);
                                    }
                                    else
                                    {
                                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                                    }                 
                                    
                                    if (FIRST_ROW_IS_COLUMN == true)
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
                                }

                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Read File ... ");
                                var log_tran1 = new LogWriter("End Read File ...");

                                var iRow = 0;
                                string[] columnNames = (from dc in result.Tables[0].Columns.Cast<DataColumn>()
                                                        select dc.ColumnName).ToArray();

                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Loop Save Header ... ");
                                var log_tran2 = new LogWriter("Start Loop Save Header ...");

                                foreach (DataRow drRow in result.Tables[0].Rows)
                                {

                                    var strRefNo = drRow[Param_RefNo].ToString();
                                    var strHNNo = drRow[Param_HNNo].ToString();
                                    var strLabNo = drRow[Param_LabNo].ToString();
                                    DateTime? dtDate = null; ;
                                    //string formateDate = "dd/MM/yyyy";
                                    //var strDate = (DateTime)drRow[Param_Date];

                                    var strDate = drRow[Param_Date].ToString();
                                    if (!string.IsNullOrEmpty(strDate))
                                    {
                                        try
                                        {
                                            //dtDate = DateTime.Parse(strDate, new CultureInfo("en-US"));
                                            dtDate = CovertStringToDate(strDate, "dd/mm/yyyy");
                                        }
                                        catch (Exception ex)
                                        {
                                            var logw = new LogWriter(ex.Message);
                                            Console.WriteLine(ex.Message);
                                        }
                                    }

                                    #region InsertHISFileDataHeader

                                    //DateTime StartHeader = DateTime.Now;
                                    TRSTGHISFileUploadHeader objHeader = new TRSTGHISFileUploadHeader();

                                    objHeader.huh_hfu_id = iFileUploadID;
                                    objHeader.huh_template_id = HISFile.hfu_template_id;
                                    objHeader.huh_status = 'F';
                                    objHeader.huh_delete_flag = false;
                                    objHeader.huh_hos_code = HISFile.hfu_hos_code;

                                    objHeader.huh_ref_no = strRefNo;
                                    objHeader.huh_hn_no = strHNNo;
                                    objHeader.huh_lab_no = strLabNo;
                                    //objHeader.huh_date = dtDate;
                                    objHeader.huh_date = dtDate;
                                    objHeader.huh_createuser = "BATCH";
                                    objHeader.huh_strdate = strDate.ToString();

                                    //Tuple<int, Boolean> checkDup = HISDataDAL.Get_CheckExistingHeader(objHeader, HISFile.hfu_file_type);
                                    //int seq = checkDup.Item1;
                                    //if (checkDup.Item2) { iDupRecord += 1; }

                                    int refNextSeq = 1;
                                    Boolean refIsDup = false;
                                   
                                    HISDataDAL.Get_CheckExistingHeaders(objHeader, HISFile.hfu_file_type, ref refNextSeq, ref refIsDup);
                           

                                    int seq = refNextSeq;
                                    if (refIsDup) { iDupRecord += 1; }

                                    objHeader.huh_seq_no = seq;
                                    objHeader.huh_createdate = DateTime.Now;
                                    int huh_id = 0;
                                    if (HISFile.hfu_file_type == "HIS")
                                    {
                                         huh_id = HISDataDAL.Save_HISFileDataHeader(objHeader);
                                    }
                                    else
                                    {
                                        HISFileDataHeaderList.Add(objHeader);
                                    }
                                    //HISFileDataHeaderList.Add(objHeader);
                                    //var huh_id = HISDataDAL.Save_HISFileDataHeader(objHeader);
                                    #endregion

                                    #region InsertHISFileDataDetail
                                    int nextseq = 1;

                                    //DateTime StartDetail = DateTime.Now;

                                    if (HISFile.hfu_file_type == "HIS")
                                    {
                                        for (int c = 4; c <= (columnNames.Length - 1); c++)
                                        {
                                            string value = "";
                                            if (!drRow[columnNames[c]].Equals(null))
                                            {
                                                if (drRow[columnNames[c]].ToString().Length > 200)
                                                {
                                                    value = drRow[columnNames[c]].ToString().Substring(0,200).Trim();
                                                }
                                                else
                                                {
                                                    value = drRow[columnNames[c]].ToString().Trim();
                                                }
                                            }

                                            TRSTGHISFileUploadDetail objDetail = new TRSTGHISFileUploadDetail();
                                            //objDetail.hud_id = (huh_id * 100) + c;
                                            //objDetail.hud_id = DBNull.Value;
                                            objDetail.hud_status = 'F';
                                            objDetail.hud_huh_id = huh_id;
                                            objDetail.hud_field_name = columnNames[c];
                                            objDetail.hud_field_value = value;
                                            objDetail.hud_createuser = "BATCH";
                                            objDetail.hud_createdate = DateTime.Now;


                                            //Tuple<int, Boolean> checkDupDetail = HISDataDAL.Get_CheckExistingDetail(objHeader, objDetail);
                                            //if (!string.IsNullOrEmpty(value))
                                            //{
                                            //    nextseq = checkDupDetail.Item1;
                                            //    //if (checkDupDetail.Item2) { iDupRecord += 1; } // Dup คือนับของ Header
                                            //}

                                            objDetail.hud_seq_no = nextseq;
                                            HISFileDataDetailList.Add(objDetail);

                                        }
                                    }
                                    #endregion

                                    iRow++;
                                } // end for

                                if (HISFile.hfu_file_type == "SP")
                                {
                                   var x = HISDataDAL.Save_HISInsertBulkDataHeader(HISFileDataHeaderList);
                                }

                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Loop Save Header ... ");
                                var log_tran3 = new LogWriter("End Loop Save Header ...");

                                //var objHISHeader = HISDataDAL.Save_HISFileDataHeaderList(HISFileDataHeaderList);

                                
                                if (HISFile.hfu_file_type == "HIS" && HISFileDataDetailList.Count != 0)
                                {
                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Save HIS Detail ... ");
                                    var log_tran4 = new LogWriter("Start Save HIS Detail ...");
                                    //DateTime StartSave_HISFileDataDetail = DateTime.Now;
                                    var ldd = HISDataDAL.Save_HISFileDataDetail(HISFileDataDetailList);

                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save HIS Detail ... ");
                                    var log_tran5 = new LogWriter("End Save HIS Detail ...");
                                    //DateTime EndSave_HISFileDataDetail = DateTime.Now;

                                    //TimeSpan tsSaveDetail = EndSave_HISFileDataDetail - StartSave_HISFileDataDetail;
                                    //Console.WriteLine("Save_HISFileDataDetail = {0}", tsSaveDetail.TotalMilliseconds);
                                }
                            }
                        }
                        else
                        {
                            // support only .xls .xlsx file
                        }

                        iMatchRecord = HISDataDAL.GetLabDataWithHISMatching(HISFile.hfu_hos_code, HISFile.hfu_id);

                        //Update status = F (Finish)
                        var rowsAffected2 = HISDataDAL.Update_HISFileUploadStatus(HISFile.hfu_id, 'F', iMatchRecord, iDupRecord, "BATCH");
                    }
                }
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Batch HIS Complete ... ");
                var log_end = new LogWriter(DateTime.Now.ToString("HH:mm:ss") + " : Batch HIS Complete ... ");
            }

            catch (Exception ex)
            {
                var rowsAffected = HISDataDAL.Update_HISFileUploadStatus(iFileUploadID, 'I', 0, 0, "BATCH");
                var d = ex.Message;
                var logw = new LogWriter(d);
                Console.WriteLine(d);
            }
        }

        public static DateTime CovertStringToDate(string fieldValue, string DateFormat)
        {
            DateTime tmpFieldValue;

            string formatmapping = DateFormat;
            string format1 = "M/d/yyyy H:mm:ss";
            string format2 = "M/d/yyyy H:mm:ss tt";
            string format3 = "M/d/yyyy H:mm:sss";
            string format4 = "M/d/yyyy H:mm:sss tt";
            string format5 = "M/d/yyyy";
            string format6 = "d-M-yyyy";
            string format7 = "yyyy-M-d";

            if (DateTime.TryParseExact(fieldValue, formatmapping, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
            {
                fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
            }
            else if (DateTime.TryParseExact(fieldValue, format1, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
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
            else if (DateTime.TryParseExact(fieldValue, format6, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
            {
                fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
            }
            else if (DateTime.TryParseExact(fieldValue, format7, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
            {
                fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
            }
            else
            {
                fieldValue = null;
            }
            return tmpFieldValue;
        }
    }
}
