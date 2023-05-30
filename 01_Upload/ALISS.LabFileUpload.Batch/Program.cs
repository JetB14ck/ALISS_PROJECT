using DbfDataReader;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using ALISS.LabFileUpload.DTO;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ALISS.LabFileUpload.Batch.DataAccess;
using AutoMapper;
using System.Runtime.InteropServices.ComTypes;
using ALISS.Mapping.DTO;
using ExcelDataReader;
using System.Collections.Generic;
using ALISS.LabFileUpload.Batch.Models;
using System.Net.Sockets;
using System.Globalization;

namespace ALISS.LabFileUpload.Batch
{
    public class Program
    {

        public IConfiguration _configuration { get; }

        static void Main(string[] args)
        {         
           
            var LabDataDAL = new LabDataDAL();
            if (LabDataDAL.ConfigValue("LABFILE:NARST_TABLE") == "Y")
            {
                using(var service = new LAB_BATCH_Service())
                {
                    service.LAB_NARST();
                }
            }
            else
            {
                using (var service = new LAB_BATCH_Service())
                {
                    service.LAB_STGHEADERDETAIL();
                }
            }
          

            
//            var LabFileUpload = LabDataDAL.Get_NewLabFileUpload('N');
//            var LabFileUploadReprocess = LabDataDAL.Get_NewLabFileUpload('R');
//            var LabFileUploadTimeOut = LabDataDAL.Get_NewLabFileUpload('T');

//            if (LabFileUploadReprocess.Count > 0)
//            {
//                LabFileUpload.AddRange(LabFileUploadReprocess);
//            }
//            if (LabFileUploadTimeOut.Count > 0)
//            {
//                LabFileUpload.AddRange(LabFileUploadTimeOut);
//            }
//            string Param_labno = "";
//            string Param_organism = "";
//            string Param_specimen = "";
//            string Param_date = "";
//            string Param_wardtype = "";
//            string lfu_mp_id = "";
//            string labdetail_path = "";

//            //int chkError = 0;
//            int chkDetailError = 0;
//            int nRowError = 0;
//            bool blnRowError = false;
//            Guid iFileUploadID = Guid.Empty;
//            int nfile = 0;
//            List<LabFileUploadDataDTO> LabFile = new List<LabFileUploadDataDTO>();
//            int ErrorAnt = 0;
//            int dataRow = 0;
//            int dataLabFilePerRound = 3;
//            try
//            {

//                if (LabFileUpload != null)
//                {
//                    var sortLab = LabFileUpload.OrderBy(o => o.lfu_createdate).ToList();
//                    if (LabFileUpload.Count() > dataLabFilePerRound)
//                    {
//                        for (var i = 0; i < dataLabFilePerRound; i++)
//                        {
//                            LabFile.Add(sortLab[i]);
//                            if(sortLab[i].lfu_status == 'T')
//                            {
//                                LabDataDAL.Delete_LabFileDataHeaderDetail(sortLab[i].lfu_id.ToString());
//                            }
//                            var rowsReprocDeleteFlag = LabDataDAL.Update_LabFileErrorStatus(sortLab[i].lfu_id.ToString(), 'D', "BATCH");
//                            var rowsAffected = LabDataDAL.Update_LabFileUploadStatus(sortLab[i].lfu_id.ToString(), 'P', 0, "BATCH");
//                            nfile += 1;
//                        }
//                    }
//                    else
//                    {
//                        foreach (LabFileUploadDataDTO Lab in sortLab)
//                        {
//                            LabFile.Add(Lab);
//                            if (Lab.lfu_status == 'T')
//                            {
//                                LabDataDAL.Delete_LabFileDataHeaderDetail(Lab.lfu_id.ToString());
//                            }
//                            var rowsReprocDeleteFlag = LabDataDAL.Update_LabFileErrorStatus(Lab.lfu_id.ToString(), 'D', "BATCH");
//                            var rowsAffected = LabDataDAL.Update_LabFileUploadStatus(Lab.lfu_id.ToString(), 'P', 0, "BATCH");
//                            nfile += 1;
//                        }
//                    }

//                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Batch Lab Start ... ");
//                    var log_start = new LogWriter(string.Format("Batch Lab Start {0}/{1} files ..." , nfile, LabFileUpload.Count()));

//                    List<TCParameter> ParameterKeyList = LabDataDAL.GetParameter("UPLOAD_KEY");
//                    List<TCParameter> ParameterUploadList = LabDataDAL.GetParameter("UPLOAD_MAPPING");
//                    List<TCParameter> ParameterMLABMappingList = LabDataDAL.GetParameter("MLAB_MAPPING_TEMPLATE");
//                    List<TCParameter> ParameterPathList = LabDataDAL.GetParameter("LAB_DETAIL_PATH");

//                    if (ParameterKeyList.Count != 0)
//                    {
//                        Param_labno = ParameterKeyList.FirstOrDefault(x => x.prm_code_minor == "LAB_NO").prm_value;
//                        Param_organism = ParameterKeyList.FirstOrDefault(x => x.prm_code_minor == "ORGANISM").prm_value;
//                        Param_specimen = ParameterKeyList.FirstOrDefault(x => x.prm_code_minor == "SPECIMEN").prm_value;
//                        Param_date = ParameterKeyList.FirstOrDefault(x => x.prm_code_minor == "DATE").prm_value;
//                    }

//                    if (ParameterUploadList.Count != 0)
//                    {
//                        Param_wardtype = ParameterUploadList.FirstOrDefault(x => x.prm_code_minor == "WARD_TYPE").prm_value;
//                    }

//                    if (ParameterMLABMappingList.Count != 0)
//                    {
//                        lfu_mp_id = ParameterMLABMappingList.FirstOrDefault(x => x.prm_code_minor == "mp_id").prm_value;
//                    }

//                    if (ParameterPathList.Count != 0)
//                    {
//                        labdetail_path = ParameterPathList.FirstOrDefault(x => x.prm_code_minor == "PATH").prm_value;
//                    }
//                    //ดัก approve

//                    foreach (LabFileUploadDataDTO selectedLab in LabFile)
//                    {
//                        var log_st = new LogWriter(string.Format(">> Hos : ({0}-{1}) File Name : {2}", selectedLab.lfu_hos_code, selectedLab.lfu_hos_name, selectedLab.lfu_FileName));
                          
//                        chkDetailError = 0;
//                        nRowError = 0;
//                        ErrorAnt = 0;
//                        iFileUploadID = selectedLab.lfu_id;
//                        if (selectedLab.lfu_Program != "MLAB") { lfu_mp_id = selectedLab.lfu_mp_id.ToString(); }
//                        var MappingTemplate = LabDataDAL.GetMappingData(lfu_mp_id);
//                        var whonetmapping = LabDataDAL.GetWHONetMappingList(lfu_mp_id, MappingTemplate.mp_mst_code);
//                        var wardtypmapping = LabDataDAL.GetWardTypeMappingList(lfu_mp_id, MappingTemplate.mp_mst_code, selectedLab.lfu_hos_code);
//                        var specimenmapping = LabDataDAL.GetSpecimeneMappingList(lfu_mp_id, MappingTemplate.mp_mst_code);
//                        var organismmapping = LabDataDAL.GetOrganismMappingList(lfu_mp_id, MappingTemplate.mp_mst_code);

//                        var lstWardType = wardtypmapping.Select(s => s.wdm_warddesc.ToUpper()).Distinct().ToList();
//                        var lstSpecimen = specimenmapping.Select(s => s.spm_localspecimencode.ToUpper()).Distinct().ToList();
//                        var lstOrganism = organismmapping.Select(s => s.ogm_localorganismcode.ToUpper()).Distinct().ToList();

//                        //Get standard field
//                        var WhonetmappingWithoutNM_NE_ND = whonetmapping.Except(whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NM") || x.wnm_whonetfield.Contains("_ND") || x.wnm_whonetfield.Contains("_NE")).ToList());

//                        //Check file type and add whonetmapping
//                        switch (selectedLab.lfu_FileType)
//                        {
//                            case clsLabFileType.MLAB_FileType.MIC:
//                                whonetmapping = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NM")).ToList();
//                                whonetmapping.AddRange(WhonetmappingWithoutNM_NE_ND);
//                                break;
//                            case clsLabFileType.MLAB_FileType.DISK:
//                                whonetmapping = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_ND")).ToList();
//                                whonetmapping.AddRange(WhonetmappingWithoutNM_NE_ND);
//                                break;
//                            case clsLabFileType.MLAB_FileType.ETEST:
//                                whonetmapping = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NE")).ToList();
//                                whonetmapping.AddRange(WhonetmappingWithoutNM_NE_ND);
//                                break;
//                        }


//                        var listexcludeAntibiotic = new List<string>();
//                        string AntiLocalCodeColumnEtest = "MICDR_ITSP";
//                        string AntiValueColumnEtest = "MICIT_TROU";
//                        EnumerableRowCollection<DataRow> AntNotMatch;
//                        var lstAntErrDistinct = new List<string>();
//                        var lstAntErrirCurrentYear = new List<string>();
//                        int cntErrAntiCurrentYear = 0;

//                        if (Path.GetExtension(selectedLab.lfu_FileName) == ".xls" || Path.GetExtension(selectedLab.lfu_FileName) == ".xlsx" || Path.GetExtension(selectedLab.lfu_FileName) == ".csv" || Path.GetExtension(selectedLab.lfu_FileName) == ".txt")
//                        {
//                            Guid feh_id_cdate = Guid.Empty;
//                            char feh_status_cdate = 'N';
//                            int cDateError = 0;
//                            int cSpecDateError = 0;
//                            string FieldDateType = "", DateFormat = "";
//                            string LAB_NO = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_labno).wnm_originalfield;
//                            string ORGANISM = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_organism).wnm_originalfield;
//                            string DATE = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_date).wnm_originalfield;
//                            DateFormat = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_date).wnm_fieldformat;

//                            string WARD_TYPE = "";
//                            if (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_wardtype) != null)
//                                WARD_TYPE = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_wardtype).wnm_originalfield;

//                            string SOURCE = "";
//                            if (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_specimen) != null)
//                                SOURCE = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_specimen).wnm_originalfield;

//                            if (MappingTemplate.mp_firstlineisheader == false)
//                            {
//                                LAB_NO = convertColumn(LAB_NO);
//                                ORGANISM = convertColumn(ORGANISM);

//                                if (SOURCE != "")
//                                    SOURCE = convertColumn(SOURCE);

//                                DATE = convertColumn(DATE);
//                            }

//                            List<TRSTGLabFileDataHeader> LabFileDataHeaderList = new List<TRSTGLabFileDataHeader>();
//                            List<TRSTGLabFileDataDetail> LabFileDataDetailList = new List<TRSTGLabFileDataDetail>();

//                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Read File ... ");
//                            var log_tran = new LogWriter("Start Read File ... ");

//                            using (var stream = File.Open(selectedLab.lfu_Path, FileMode.Open, FileAccess.Read))
//                            {
//                                DataSet result = new DataSet();

//                                if (Path.GetExtension(selectedLab.lfu_FileName) == ".xls" || Path.GetExtension(selectedLab.lfu_FileName) == ".xlsx")
//                                {
//                                    ExcelDataSetConfiguration option = new ExcelDataSetConfiguration();
//                                    var reader = ExcelReaderFactory.CreateReader(stream);
//                                    if (MappingTemplate.mp_firstlineisheader == true)
//                                    {
//                                        result = reader.AsDataSet(new ExcelDataSetConfiguration()
//                                        {
//                                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
//                                            {
//                                                UseHeaderRow = true
//                                            }
//                                        }
//                                        );
//                                    }
//                                    else
//                                    {
//                                        result = reader.AsDataSet();
//                                    }
//                                }
//                                else if (Path.GetExtension(selectedLab.lfu_FileName) == ".csv")
//                                {
//                                    var reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
//                                    {
//                                        FallbackEncoding = Encoding.GetEncoding(1252),
//                                        AutodetectSeparators = new char[] { ',', ';', '\t', '|', '#' },
//                                        LeaveOpen = false,
//                                        AnalyzeInitialCsvRows = 0,
//                                    });

//                                    if (MappingTemplate.mp_firstlineisheader == true)
//                                    {
//                                        result = reader.AsDataSet(new ExcelDataSetConfiguration()
//                                        {
//                                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
//                                            {
//                                                UseHeaderRow = true
//                                            }
//                                        }
//                                        );
//                                    }
//                                    else
//                                    {
//                                        result = reader.AsDataSet();
//                                    }

//                                }
//                                else if (Path.GetExtension(selectedLab.lfu_FileName) == ".txt")
//                                {
//                                    string line;
//                                    DataTable dt = new DataTable();

//                                    using (TextReader tr = File.OpenText(selectedLab.lfu_Path))
//                                    {
//                                        while ((line = tr.ReadLine()) != null)
//                                        {
//                                            string[] items = line.Split('\t');
//                                            if (dt.Columns.Count == 0)
//                                            {
//                                                if (MappingTemplate.mp_firstlineisheader == false)
//                                                {
//                                                    for (int i = 0; i < items.Length; i++)
//                                                        dt.Columns.Add(new DataColumn("Column" + i, typeof(string)));
//                                                }
//                                                else
//                                                {
//                                                    for (int i = 0; i < items.Length; i++)
//                                                        dt.Columns.Add(new DataColumn(items[i].ToString(), typeof(string)));
//                                                }
//                                            }
//                                            dt.Rows.Add(items);
//                                        }
//                                    }

//                                    result.Tables.Add(dt);


//                                }

//                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Read File ... ");
//                                var log_tran1 = new LogWriter("End Read File ...");

//                                if (result.Tables[0].Columns.Contains(DATE) == true)
//                                {
//                                    FieldDateType = result.Tables[0].Columns[DATE].DataType.ToString();
//                                }

                                                    
//                                //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
//                                //>> for Etest : Antibiotic Isolate more than 1 Record
//                                if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
//                                {
//                                    //var MappingAnt = whonetmapping.Where(x => x.wnm_antibiotic != null);
//                                    var MappingAnt = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NE")).ToList(); ;
//                                   // Antibioticcolumn = MappingAnt.FirstOrDefault().wnm_antibioticcolumn;

//                                    if (MappingTemplate.mp_firstlineisheader == false)
//                                    {

//                                        AntiLocalCodeColumnEtest = convertColumn(AntiLocalCodeColumnEtest);
//                                    }

//                                    //var listexcludeAntibiotic = new List<string>();

//                                    foreach (WHONetMappingListsDTO t in MappingAnt)
//                                    {
//                                        //listexcludeAntibiotic.Add(t.wnm_antibiotic);
//                                        listexcludeAntibiotic.Add(t.wnm_originalfield);
//                                    }                                  
//                                }

//                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Header Data ... ");
//                                var log_read = new LogWriter("Start Header Data ...");
//                                dataRow = 1;

//                                if (listexcludeAntibiotic.Count > 0)
//                                {
//                                    AntNotMatch = result.Tables[0].AsEnumerable().Where(row => !listexcludeAntibiotic.Contains(row.Field<string>(AntiLocalCodeColumnEtest)));
//                                    ErrorAnt = AntNotMatch.Count();
//                                    var AntNotMatchDistinct = AntNotMatch.AsEnumerable().Select(s => new { antibiotic = s.Field<string>(AntiLocalCodeColumnEtest), }).Distinct().ToList();
//                                    foreach (var lst in AntNotMatchDistinct)
//                                    {
//                                        lstAntErrDistinct.Add(lst.antibiotic);
//                                    }
//                                }

//                                //Loop Read Data Row
//                                foreach (DataRow row in result.Tables[0].Rows)
//                                {
//                                    string cmethod = "";
//                                    blnRowError = false;                                  

//                                    if (result.Tables[0].Columns.Contains("CMETHOD"))
//                                    {
//                                        cmethod = row["CMETHOD"].ToString();
//                                    }
 
//                                    if ((cmethod == "Aerobic Culture" && selectedLab.lfu_Program == "MLAB")
//                                   || selectedLab.lfu_Program != "MLAB"
//                                   || (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST"))
//                                    {

//                                        #region InsertLabFileDataHeader
//                                        TRSTGLabFileDataHeader objModel = new TRSTGLabFileDataHeader();
//                                        Guid ldh_id = Guid.NewGuid();

//                                        // key is null, continue
//                                        if (!string.IsNullOrEmpty(LAB_NO) && string.IsNullOrEmpty(row[LAB_NO].ToString()))
//                                        {
//                                            continue;
//                                        }
//                                        else if (!string.IsNullOrEmpty(ORGANISM) && string.IsNullOrEmpty(row[ORGANISM].ToString()))
//                                        {
//                                            continue;
//                                        }
//                                        else if (!string.IsNullOrEmpty(DATE) && string.IsNullOrEmpty(row[DATE].ToString()))
//                                        {
//                                            continue;
//                                        }
                              
//                                        if (selectedLab.lfu_FileType != "ETEST")
//                                        {
//                                            if (!string.IsNullOrEmpty(SOURCE) && string.IsNullOrEmpty(row[SOURCE].ToString()))
//                                            {
//                                                continue;
//                                            }
//                                        }
       

//                                        // -- Check Date Format
//                                        objModel.ldh_date = row[DATE].ToString();
//                                        if (FieldDateType == "System.String")
//                                        {
//                                            try
//                                            {
//                                                //objModel.ldh_cdate = DateTime.ParseExact(objModel.ldh_date, DateFormat, CultureInfo.GetCultureInfo("en-US"));
//                                                objModel.ldh_cdate = CovertStringToDate(objModel.ldh_date, DateFormat);
//                                                int year = objModel.ldh_cdate.Value.Year;

//                                                if (year < 1000)
//                                                {
//                                                    if (feh_id_cdate == Guid.Empty)
//                                                    {
//                                                        feh_id_cdate = Guid.NewGuid();
//                                                    }
//                                                    else
//                                                    {
//                                                        feh_status_cdate = 'E';
//                                                    }


//                                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
//                                                    objErrorH.feh_id = feh_id_cdate;
//                                                    objErrorH.feh_status = feh_status_cdate;
//                                                    objErrorH.feh_flagdelete = false;
//                                                    objErrorH.feh_type = "CONVERT_ERROR";
//                                                    objErrorH.feh_field = DATE;
//                                                    objErrorH.feh_message = "Cannot convert date.";
//                                                    cSpecDateError += 1;
//                                                    objErrorH.feh_errorrecord = cSpecDateError;
//                                                    objErrorH.feh_createuser = "BATCH";
//                                                    objErrorH.feh_createdate = DateTime.Now;
//                                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
//                                                    blnRowError = true;
//                                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

                                                    

                                                    
//                                                    continue;
//                                                }

//                                                if (year > DateTime.Now.Year)
//                                                {
//                                                    objModel.ldh_cdate = objModel.ldh_cdate.Value.AddYears(-543);
//                                                }
//                                            }
//                                            catch (Exception ex)
//                                            {
//                                                //chkError++;

//                                                if (feh_id_cdate == Guid.Empty)
//                                                {
//                                                    feh_id_cdate = Guid.NewGuid();
//                                                }
//                                                else
//                                                {
//                                                    feh_status_cdate = 'E';
//                                                }


//                                                TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
//                                                objErrorH.feh_id = feh_id_cdate;
//                                                objErrorH.feh_status = feh_status_cdate;
//                                                objErrorH.feh_flagdelete = false;
//                                                objErrorH.feh_type = "CONVERT_ERROR";
//                                                objErrorH.feh_field = DATE;
//                                                objErrorH.feh_message = "Cannot convert date.";
//                                                cSpecDateError += 1;
//                                                objErrorH.feh_errorrecord = cSpecDateError;
//                                                objErrorH.feh_createuser = "BATCH";
//                                                objErrorH.feh_createdate = DateTime.Now;
//                                                objErrorH.feh_lfu_id = selectedLab.lfu_id;
//                                                blnRowError = true;
//                                                var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

                                               
//                                                TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
//                                                objErrorD.fed_id = Guid.NewGuid();
//                                                objErrorD.fed_status = 'N';
//                                                objErrorD.fed_localvalue = ex.Message;
//                                                objErrorD.fed_feh_id = feh_id_cdate;
//                                                objErrorD.fed_createuser = "BATCH";
//                                                objErrorD.fed_createdate = DateTime.Now;

//                                                var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
//                                                continue;
//                                            }
//                                        }
//                                        else if (FieldDateType == "System.DateTime")
//                                        {
//                                            objModel.ldh_cdate = (DateTime)row[DATE];
//                                            int year = objModel.ldh_cdate.Value.Year;
//                                            if (year > DateTime.Now.Year)
//                                            {
//                                                objModel.ldh_cdate = objModel.ldh_cdate.Value.AddYears(-543);
//                                            }
//                                        }
//                                        else if (FieldDateType == "System.Object") //-- am add 18/01/2021 --
//                                        {
//                                            if (row[DATE].GetType().FullName == "System.DateTime")
//                                            {
//                                                objModel.ldh_cdate = (DateTime)row[DATE];
//                                                int year = objModel.ldh_cdate.Value.Year;
//                                                if (year > DateTime.Now.Year)
//                                                {
//                                                    objModel.ldh_cdate = objModel.ldh_cdate.Value.AddYears(-543);
//                                                }
//                                            }
//                                            else 
//                                            {
//                                                try
//                                                {
//                                                    objModel.ldh_cdate = CovertStringToDate(objModel.ldh_date, DateFormat);
//                                                    //objModel.ldh_cdate = DateTime.ParseExact(objModel.ldh_date, DateFormat, CultureInfo.GetCultureInfo("en-US"));
//                                                    int year = objModel.ldh_cdate.Value.Year;

//                                                    if (year < 1000)
//                                                    {
//                                                        if (feh_id_cdate == Guid.Empty)
//                                                        {
//                                                            feh_id_cdate = Guid.NewGuid();
//                                                        }
//                                                        else
//                                                        {
//                                                            feh_status_cdate = 'E';
//                                                        }


//                                                        TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
//                                                        objErrorH.feh_id = feh_id_cdate;
//                                                        objErrorH.feh_status = feh_status_cdate;
//                                                        objErrorH.feh_flagdelete = false;
//                                                        objErrorH.feh_type = "CONVERT_ERROR";
//                                                        objErrorH.feh_field = DATE;
//                                                        objErrorH.feh_message = "Cannot convert date.";
//                                                        cSpecDateError += 1;
//                                                        objErrorH.feh_errorrecord = cSpecDateError;
//                                                        objErrorH.feh_createuser = "BATCH";
//                                                        objErrorH.feh_createdate = DateTime.Now;
//                                                        objErrorH.feh_lfu_id = selectedLab.lfu_id;
//                                                        blnRowError = true;


//                                                        var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);
//                                                    }

//                                                    if (year > DateTime.Now.Year)
//                                                    {
//                                                        objModel.ldh_cdate = objModel.ldh_cdate.Value.AddYears(-543);
//                                                    }
//                                                }
//                                                catch (Exception ex)
//                                                {
//                                                    //chkError++;

//                                                    if (feh_id_cdate == Guid.Empty)
//                                                    {
//                                                        feh_id_cdate = Guid.NewGuid();
//                                                    }
//                                                    else
//                                                    {
//                                                        feh_status_cdate = 'E';
//                                                    }


//                                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
//                                                    objErrorH.feh_id = feh_id_cdate;
//                                                    objErrorH.feh_status = feh_status_cdate;
//                                                    objErrorH.feh_flagdelete = false;
//                                                    objErrorH.feh_type = "CONVERT_ERROR";
//                                                    objErrorH.feh_field = DATE;
//                                                    objErrorH.feh_message = "Cannot convert date.";
//                                                    cSpecDateError += 1;
//                                                    objErrorH.feh_errorrecord = cSpecDateError;
//                                                    objErrorH.feh_createuser = "BATCH";
//                                                    objErrorH.feh_createdate = DateTime.Now;
//                                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
//                                                    blnRowError = true;


//                                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

//                                                    TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
//                                                    objErrorD.fed_id = Guid.NewGuid();
//                                                    objErrorD.fed_status = 'N';
//                                                    objErrorD.fed_localvalue = ex.Message;
//                                                    objErrorD.fed_feh_id = objTRLabFileErrorHeader;
//                                                    objErrorD.fed_createuser = "BATCH";
//                                                    objErrorD.fed_createdate = DateTime.Now;

//                                                    var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
//                                                    continue;
//                                                }
//                                            }
//                                        }

//                                        // Etest and Data not this year : continue
//                                        if (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST" && objModel.ldh_cdate.Value.Year != selectedLab.lfu_DataYear)
//                                        {
//                                            continue;
//                                        }

//                                        // -- End Check Date Format

//                                        // -- Check Antibiotic                                     
//                                        if (lstAntErrDistinct.Count > 0) 
//                                        {
//                                            var anti = row[AntiLocalCodeColumnEtest].ToString();
//                                            if (lstAntErrDistinct.Contains(anti))
//                                            {
//                                                cntErrAntiCurrentYear += 1;
//                                                blnRowError = true;
//                                                if (!lstAntErrirCurrentYear.Contains(anti)){
//                                                    lstAntErrirCurrentYear.Add(anti);
//                                                }                                               
//                                                continue;
//                                            }                                                                                                                            
//                                        }
//                                        // -- End Check Antibiotic

//                                        objModel.ldh_id = ldh_id;
//                                        objModel.ldh_status = 'N';
//                                        objModel.ldh_flagdelete = false;
//                                        objModel.ldh_hos_code = selectedLab.lfu_hos_code;
//                                        objModel.ldh_lab = selectedLab.lfu_lab;
//                                        objModel.ldh_lfu_id = selectedLab.lfu_id;

//                                        objModel.ldh_labno = row[LAB_NO].ToString();
//                                        objModel.ldh_organism = row[ORGANISM].ToString();

//                                        if (selectedLab.lfu_FileType != "ETEST")
//                                        {
//                                            if (SOURCE != "")
//                                            {
//                                                 objModel.ldh_specimen = row[SOURCE].ToString();
//                                            }                                         
//                                        }
                                           
                                      
//                                        objModel.ldh_createuser = "BATCH";

//                                        int seq = LabDataDAL.Get_CheckExistingHeader(objModel, MappingTemplate.mp_program, selectedLab.lfu_FileType);
//                                        objModel.ldh_sequence = seq;
//                                        objModel.ldh_createdate = DateTime.Now;
//                                        Guid idh_id_related = Guid.Empty;

//                                        bool isNotFirstline = false;
//                                        //ETEST
//                                        //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
//                                        if(selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
//                                        {
//                                            idh_id_related = LabDataDAL.Get_HeaderIdForMultipleline(objModel);
//                                            if (idh_id_related != Guid.Empty)
//                                            {
//                                                isNotFirstline = true;
//                                                ldh_id = idh_id_related;
//                                            }
//                                            else
//                                            {                                                
//                                                //var x = LabDataDAL.Save_LabFileDataHeader(objModel);
//                                                LabFileDataHeaderList.Add(objModel);
                                               
//                                            }
//                                        }
//                                        else
//                                        {
//                                            //var x = LabDataDAL.Save_LabFileDataHeader(objModel);
//                                            LabFileDataHeaderList.Add(objModel);
//                                        }


//                                        #endregion

//                                        #region InsertLabFileDataDetail

//                                        if (whonetmapping != null)
//                                        {
//                                            foreach (WHONetMappingListsDTO item in whonetmapping)
//                                            {


//                                                String wnm_originalfield;
//                                                var Encoding = "";

//                                                if (MappingTemplate.mp_firstlineisheader == false)
//                                                {

//                                                    wnm_originalfield = convertColumn(item.wnm_originalfield.Trim());
//                                                }
//                                                else
//                                                {
//                                                    wnm_originalfield = item.wnm_originalfield.Trim();
//                                                }

//                                                if (isNotFirstline == true && selectedLab.lfu_FileType == "ETEST" && (wnm_originalfield == LAB_NO || wnm_originalfield == DATE || wnm_originalfield == ORGANISM))
//                                                {
//                                                    continue;
//                                                }

//                                                if (selectedLab.lfu_FileType == "ETEST")
//                                                {
//                                                    if (item.wnm_whonetfield.Contains("_NE"))
//                                                    {
//                                                        wnm_originalfield = AntiValueColumnEtest; //MICIT_TROU
//                                                        if (!result.Tables[0].Columns.Contains(wnm_originalfield))
//                                                        {
//                                                            continue;
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        if (!result.Tables[0].Columns.Contains(item.wnm_originalfield.Trim()))
//                                                        {
//                                                            continue;
//                                                        }
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    if (!result.Tables[0].Columns.Contains(wnm_originalfield))
//                                                    {
//                                                        continue;
//                                                    }
//                                                }
//                                                // ETEST
//                                                //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
//                                                if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
//                                                {
//                                                    //if (item.wnm_antibioticcolumn != null)
//                                                    if (AntiLocalCodeColumnEtest != "")
//                                                    {
//                                                        //var antibioticcolumn = "";
//                                                        if (MappingTemplate.mp_firstlineisheader == false)
//                                                        {
//                                                            AntiLocalCodeColumnEtest = convertColumn(AntiLocalCodeColumnEtest);
//                                                        }

//                                                        if (row.IsNull(AntiLocalCodeColumnEtest))
//                                                        {
//                                                            continue;
//                                                        }
//                                                        if (item.wnm_whonetfield.Contains("_NE"))
//                                                        {
//                                                            if (row[AntiLocalCodeColumnEtest].ToString() != item.wnm_originalfield.Trim())
//                                                            {
//                                                                continue;
//                                                            }
//                                                        }
//                                                    }
//                                                }

//                                                if (row.IsNull(wnm_originalfield))
//                                                {
//                                                    continue;
//                                                }

//                                                string tempvalue = "";
//                                                var xx = row[wnm_originalfield].GetType().ToString();
//                                                if (row[wnm_originalfield].GetType().ToString() != "System.DateTime" && item.wnm_type == "Date")
//                                                {
//                                                    try
//                                                    {
//                                                        //var tempfielddate = DateTime.ParseExact(row[wnm_originalfield].ToString(), item.wnm_fieldformat, CultureInfo.GetCultureInfo("en-US"));
//                                                        var tempfielddate = CovertStringToDate(row[wnm_originalfield].ToString(), item.wnm_fieldformat);
//                                                        tempvalue = tempfielddate.ToString();
//                                                    }
//                                                    catch (Exception ex)
//                                                    {
//                                                        chkDetailError++;

//                                                        if (feh_id_cdate == Guid.Empty)
//                                                        {
//                                                            feh_id_cdate = Guid.NewGuid();
//                                                        }
//                                                        else
//                                                        {
//                                                            feh_status_cdate = 'E';
//                                                        }
//                                                        TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
//                                                        objErrorH.feh_id = feh_id_cdate;
//                                                        objErrorH.feh_status = feh_status_cdate;
//                                                        objErrorH.feh_flagdelete = false;
//                                                        objErrorH.feh_type = "CONVERT_ERROR";
//                                                        objErrorH.feh_field = item.wnm_originalfield.Trim();
//                                                        objErrorH.feh_message = "Cannot convert date.";
//                                                        cDateError = cDateError + 1;
//                                                        objErrorH.feh_errorrecord = cDateError;
//                                                        objErrorH.feh_createuser = "BATCH";
//                                                        objErrorH.feh_createdate = DateTime.Now;
//                                                        objErrorH.feh_lfu_id = selectedLab.lfu_id;
//                                                        blnRowError = true;
//                                                        var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

//                                                        TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
//                                                        objErrorD.fed_id = Guid.NewGuid();
//                                                        objErrorD.fed_status = 'N';
//                                                        objErrorD.fed_localvalue = ex.Message;
//                                                        objErrorD.fed_feh_id = feh_id_cdate;
//                                                        objErrorD.fed_createuser = "BATCH";
//                                                        objErrorD.fed_createdate = DateTime.Now;

//                                                        var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
//                                                        continue;
//                                                    }
//                                                }
//                                                else if (row[wnm_originalfield].GetType().ToString() == "System.DateTime" && item.wnm_type == "Date")
//                                                {
//                                                    var tempfielddate = (DateTime)row[wnm_originalfield];
//                                                    tempvalue = tempfielddate.ToString();
//                                                }
//                                                else
//                                                {
//                                                    if ( (!string.IsNullOrEmpty(selectedLab.lfu_FileTypeLabel)) &&  item.wnm_whonetfield.Contains("_NM"))
//                                                    {                                                 
//                                                        if (selectedLab.lfu_FileTypeLabel == clsLabFileType.MLAB_FileType.MIC_SIR)
//                                                        {
//                                                            if (row[wnm_originalfield].ToString().ToUpper() == "S" ||
//                                                                 row[wnm_originalfield].ToString().ToUpper() == "I" ||
//                                                                 row[wnm_originalfield].ToString().ToUpper() == "R")
//                                                            {
//                                                                //MIC-SIR
//                                                                tempvalue = row[wnm_originalfield].ToString();
//                                                            }
//                                                            else
//                                                            {
//                                                                // do nothing
//                                                            }
//                                                        }
//                                                        else if (selectedLab.lfu_FileTypeLabel == clsLabFileType.MLAB_FileType.MIC_NUM)
//                                                        {
//                                                            if (row[wnm_originalfield].ToString() == "S" ||
//                                                                 row[wnm_originalfield].ToString() == "I" ||
//                                                                 row[wnm_originalfield].ToString() == "R")
//                                                            {
//                                                                // do nothing
//                                                            }
//                                                            else
//                                                            {
//                                                                //MIC-NUM
//                                                                tempvalue = row[wnm_originalfield].ToString();
                                                              
//                                                            }
//                                                        }
//                                                        else
//                                                        {
//                                                            tempvalue = row[wnm_originalfield].ToString();
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        tempvalue = row[wnm_originalfield].ToString();
//                                                    }
//                                                    goto AddDetailPoint;
//                                                }

//                                                if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST && item.wnm_whonetfield.Contains("_NE"))
//                                                {
//                                                    string antibiotic = row[AntiLocalCodeColumnEtest].ToString();

//                                                    if (antibiotic == item.wnm_originalfield.Trim())
//                                                    {
//                                                        tempvalue = row[AntiValueColumnEtest].ToString();
//                                                    }
//                                                    else
//                                                    {
//                                                        continue;
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    if (result.Tables[0].Columns.Contains(item.wnm_originalfield.Trim()))
//                                                    {
//                                                        tempvalue = row[item.wnm_originalfield.Trim()].ToString();
//                                                    }
//                                                    else
//                                                    {
//                                                        continue;
//                                                    }
//                                                }
//AddDetailPoint:
//                                                if (item.wnm_encrypt == true)
//                                                {
//                                                    Encoding = CryptoHelper.UnicodeEncoding(tempvalue);
//                                                }
//                                                else
//                                                {
//                                                    Encoding = tempvalue;
//                                                }

//                                                LabFileDataDetailList.Add(new TRSTGLabFileDataDetail
//                                                {
//                                                    ldd_id = Guid.NewGuid(),
//                                                    ldd_status = 'N',
//                                                    ldd_whonetfield = item.wnm_whonetfield,
//                                                    ldd_originalfield = item.wnm_originalfield.Trim(),
//                                                    ldd_originalvalue = Encoding,
//                                                    ldd_ldh_id = ldh_id,
//                                                    ldd_createuser = "BATCH",
//                                                    ldd_createdate = DateTime.Now
//                                                }
//                                                );

//                                            }

//                                        }
//                                        #endregion
//                                    }
//                                    else
//                                    {
//                                        continue;
//                                    }
                                   
//                                    if (!blnRowError && WARD_TYPE != "" && (result.Tables[0].Columns.Contains(WARD_TYPE))) 
//                                    {
//                                        var ward = row[WARD_TYPE].ToString().ToUpper();
//                                        if (ward == "" || !lstWardType.Contains(ward.Trim())) { blnRowError = true; }
//                                    }

//                                    if (selectedLab.lfu_FileType != "ETEST")
//                                    {
//                                        if (!blnRowError &&  SOURCE != "" && (result.Tables[0].Columns.Contains(SOURCE)))
//                                        {
//                                            var localspecimen = row[SOURCE].ToString().ToUpper();
//                                            if (localspecimen == "" || !lstSpecimen.Contains(localspecimen.Trim())) { blnRowError = true; }
//                                        }
//                                    }
                                       

//                                    if (!blnRowError && ORGANISM != "" && (result.Tables[0].Columns.Contains(ORGANISM)))
//                                    {
//                                        var localorganism = row[ORGANISM].ToString().ToUpper();
//                                        if (localorganism == "" || !lstOrganism.Contains(localorganism.Trim())) { blnRowError = true; }
//                                    }
                                   
                               
//                                    if (blnRowError){ nRowError += 1;}
//                                    dataRow += 1;
//                                }// end loop read data row
                                
//                                //Save Antibiotic Error
//                                if (lstAntErrirCurrentYear.Count > 0)
//                                {
//                                    nRowError = cntErrAntiCurrentYear;
//                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
//                                    objErrorH.feh_id = Guid.NewGuid();
//                                    objErrorH.feh_status = 'N';
//                                    objErrorH.feh_flagdelete = false;
//                                    objErrorH.feh_type = "NOT_MATCH";
//                                    objErrorH.feh_field = "Antibiotic";
//                                    objErrorH.feh_message = "Antibiotic not match";
//                                    objErrorH.feh_errorrecord = cntErrAntiCurrentYear;
//                                    objErrorH.feh_createuser = "BATCH";
//                                    objErrorH.feh_createdate = DateTime.Now;
//                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
//                                    blnRowError = true;
//                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

//                                    foreach (var item in lstAntErrirCurrentYear)
//                                    {
//                                        TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
//                                        objErrorD.fed_id = Guid.NewGuid();
//                                        objErrorD.fed_status = 'N';
//                                        objErrorD.fed_localvalue = item;
//                                        objErrorD.fed_feh_id = objErrorH.feh_id;
//                                        objErrorD.fed_createuser = "BATCH";
//                                        objErrorD.fed_createdate = DateTime.Now;

//                                        var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
//                                    }
//                                }

//                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Header Data ... ");
//                                var log_endr = new LogWriter("End Header Data ...");

//                                if (LabFileDataHeaderList.Count() != 0)
//                                {
//                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Save Header List ... ");
//                                    var log_std = new LogWriter("Start Save Header List ...");

//                                    LabDataDAL.InsertBulk_LabFileDataHeader(LabFileDataHeaderList);

//                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Header List... ");
//                                    var log_endd = new LogWriter("End Save Header List ...");
//                                }


//                                if (LabFileDataDetailList.Count != 0)
//                                {
//                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Save Detail ... ");
//                                    var log_std = new LogWriter("Start Save Detail ...");

//                                     //var ldd = LabDataDAL.Save_LabFileDataDetail(LabFileDataDetailList);
//                                    var ldd = Export_LabFileDataDetail(labdetail_path, LabFileDataDetailList, selectedLab);
//                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Detail ... ");
//                                    var log_endd = new LogWriter("End Save Detail ...");
//                                }
//                            }

//                        }
//                        #region comment
                       
//                        #endregion
//                        //DBF + .HOS
//                        else 
//                        {
//                            var options = new DbfDataReaderOptions
//                            {
//                                Encoding = Encoding.GetEncoding(874)
//                            };

//                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Read File (.DBF) ... ");
//                            var log_tran = new LogWriter("Read File (.DBF) ...");
                            
//                            using (var dbfDataReader = new DbfDataReader.DbfDataReader(selectedLab.lfu_Path, options))
//                            {
//                                var x = dbfDataReader.DbfTable.Header.RecordCount;

//                                string LAB_NO = whonetmapping.FirstOrDefault(v => v.wnm_whonetfield == Param_labno).wnm_originalfield;
//                                string ORGANISM = whonetmapping.FirstOrDefault(v => v.wnm_whonetfield == Param_organism).wnm_originalfield;
//                                string DATE = whonetmapping.FirstOrDefault(v => v.wnm_whonetfield == Param_date).wnm_originalfield;

//                                string WARD_TYPE = "";
//                                if (whonetmapping.FirstOrDefault(w => w.wnm_whonetfield == Param_wardtype) != null)
//                                    WARD_TYPE = whonetmapping.FirstOrDefault(w => w.wnm_whonetfield == Param_wardtype).wnm_originalfield;

//                                string SOURCE = "";
//                                if (whonetmapping.FirstOrDefault(v => v.wnm_whonetfield == Param_specimen) != null)
//                                    SOURCE = whonetmapping.FirstOrDefault(v => v.wnm_whonetfield == Param_specimen).wnm_originalfield;

//                                List<TRSTGLabFileDataHeader> LabFileDataHeaderList = new List<TRSTGLabFileDataHeader>();
//                                List<TRSTGLabFileDataDetail> LabFileDataDetailList = new List<TRSTGLabFileDataDetail>();

//                                //string Antibioticcolumn = "";
//                                List<WHONetMappingListsDTO> MappingAnt = new List<WHONetMappingListsDTO>();
//                                //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
//                                if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
//                                {
//                                    MappingAnt = whonetmapping.Where(v => v.wnm_whonetfield.Contains("_NE")).ToList();
//                                    //Antibioticcolumn = MappingAnt.FirstOrDefault().wnm_antibioticcolumn;
//                                }

//                                List<TRLabFileErrorDetail> AntibioticErrorD = new List<TRLabFileErrorDetail>();

//                                ErrorAnt = 0;
//                                dataRow = 0;
//                                //int test = 0;
//                                while (dbfDataReader.Read())
//                                {
//                                    blnRowError = false;
//                                    //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
//                                    if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
//                                    {
//                                        if (dbfDataReader[AntiLocalCodeColumnEtest] == "" || dbfDataReader[AntiLocalCodeColumnEtest] == null)
//                                        {
//                                            continue;
//                                        }
//                                        string antibiotic = dbfDataReader[AntiLocalCodeColumnEtest].ToString();
//                                        if (!MappingAnt.Any(v => v.wnm_originalfield == antibiotic))
//                                        {
//                                            if (!AntibioticErrorD.Any(v => v.fed_localvalue == antibiotic))
//                                            {
//                                                if (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST" 
//                                                    && dbfDataReader.GetDateTime(DATE).Year == selectedLab.lfu_DataYear)
//                                                {
//                                                    AntibioticErrorD.Add(new TRLabFileErrorDetail
//                                                    {
//                                                        fed_id = Guid.NewGuid(),
//                                                        fed_status = 'N',
//                                                        fed_localvalue = antibiotic,
//                                                        fed_createuser = "BATCH",
//                                                        fed_createdate = DateTime.Now
//                                                    });
//                                                    ErrorAnt++;
//                                                    blnRowError = true;
//                                                }
                                                
//                                            }                                           
//                                            continue;
//                                        }
//                                    }

//                                    string cmethod = "";
//                                    if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == "CMETHOD") != null)
//                                        cmethod = dbfDataReader.GetString("CMETHOD");

//                                    if ((cmethod == "Aerobic Culture" && selectedLab.lfu_Program == "MLAB")
//                                    || selectedLab.lfu_Program != "MLAB"
//                                    || (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST"))
//                                    {
//                                        #region InsertLabFileDataHeader

//                                        if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == LAB_NO) != null)
//                                        {
//                                            if (string.IsNullOrEmpty(dbfDataReader[LAB_NO].ToString())){ continue;}
//                                        }
//                                        if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == ORGANISM) != null)
//                                        {
//                                            if (string.IsNullOrEmpty(dbfDataReader[ORGANISM].ToString())){ continue;}
//                                        }
//                                        if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == DATE) != null)
//                                        {
//                                            try
//                                            {
//                                                if (string.IsNullOrEmpty(dbfDataReader.GetDateTime(DATE).ToString())) {
//                                                    continue;
//                                                }
//                                            }
//                                            catch
//                                            {
//                                                continue;
//                                            }
                                           
//                                        }
//                                        if (selectedLab.lfu_FileType != "ETEST")
//                                        {
//                                            if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == SOURCE) != null)
//                                            {                                            
//                                                if (string.IsNullOrEmpty(dbfDataReader[SOURCE].ToString())) {continue;}                                           
//                                            }
//                                        }
                                        
                                      
                                        
//                                        TRSTGLabFileDataHeader objModel = new TRSTGLabFileDataHeader();
//                                        Guid ldh_id = Guid.NewGuid();

//                                        //-- check date
//                                        objModel.ldh_date = dbfDataReader.GetDateTime(DATE).ToString();
//                                        try
//                                        {
//                                            objModel.ldh_cdate = dbfDataReader.GetDateTime(DATE);
//                                        }
//                                        catch (Exception ex)
//                                        {
//                                            var dd = ex.Message;
//                                        }

//                                        if (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST" && objModel.ldh_cdate.Value.Year != selectedLab.lfu_DataYear)
//                                        {
//                                            continue;
//                                        }
//                                        //-- end check date

//                                        objModel.ldh_id = ldh_id;
//                                        objModel.ldh_status = 'N';
//                                        objModel.ldh_flagdelete = false;
//                                        objModel.ldh_hos_code = selectedLab.lfu_hos_code;
//                                        objModel.ldh_lab = selectedLab.lfu_lab;
//                                        objModel.ldh_lfu_id = selectedLab.lfu_id;

//                                        objModel.ldh_labno = dbfDataReader[LAB_NO].ToString();
//                                        objModel.ldh_organism = dbfDataReader.GetString(ORGANISM);

//                                        if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == SOURCE) != null)
//                                            objModel.ldh_specimen = dbfDataReader.GetString(SOURCE);

             
//                                        objModel.ldh_createuser = "BATCH";

//                                        int seq = LabDataDAL.Get_CheckExistingHeader(objModel, MappingTemplate.mp_program, selectedLab.lfu_FileType);
//                                        objModel.ldh_sequence = seq;
//                                        objModel.ldh_createdate = DateTime.Now;

//                                        Guid idh_id_related = Guid.Empty;
//                                        bool isNotFirstline = false;

//                                        //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
//                                        if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
//                                        {
//                                            idh_id_related = LabDataDAL.Get_HeaderIdForMultipleline(objModel);
//                                            if (idh_id_related != Guid.Empty)
//                                            {
//                                                isNotFirstline = true;
//                                                ldh_id = idh_id_related;
//                                            }
//                                            else
//                                            {
//                                                //var c = LabDataDAL.Save_LabFileDataHeader(objModel);
//                                                LabFileDataHeaderList.Add(objModel);
//                                            }
//                                        }
//                                        else
//                                        {
//                                            //var c = LabDataDAL.Save_LabFileDataHeader(objModel);
//                                            LabFileDataHeaderList.Add(objModel);
//                                        }

//                                        //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Lab Header ... ");
//                                        #endregion

//                                        #region InsertLabFileDataDetail
//                                        //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start loop Detail ... ");
//                                        try
//                                        {
//                                            if (whonetmapping != null)
//                                            {
//                                                foreach (WHONetMappingListsDTO item in whonetmapping)
//                                                {
//                                                    if (isNotFirstline == true && selectedLab.lfu_FileType == "ETEST" && (item.wnm_originalfield == LAB_NO || item.wnm_originalfield == DATE || item.wnm_originalfield == ORGANISM))
//                                                    {
//                                                        continue;
//                                                    }
//                                                    if (selectedLab.lfu_FileType == "ETEST")
//                                                    {
//                                                        if (item.wnm_whonetfield.Contains("_NE"))
//                                                        {
//                                                            //wnm_originalfield = AntiValueColumnEtest; //MICIT_TROU
//                                                            if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == "MICIT_TROU") == null) //MICIT_TROU
//                                                            {
//                                                                continue;
//                                                            }
//                                                        }
//                                                        else
//                                                        {
//                                                            if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == item.wnm_originalfield) == null)
//                                                            {
//                                                                continue;
//                                                            }
//                                                        }
//                                                    }

//                                                    var Encoding = "";
//                                                    var tempvalue = "";
//                                                    if (item.wnm_encrypt == true)
//                                                    {
//                                                        tempvalue = CryptoHelper.UnicodeEncoding(dbfDataReader[item.wnm_originalfield.Trim()].ToString());
//                                                    }
//                                                    else
//                                                    {
//                                                        if (selectedLab.lfu_FileType == "ETEST" && item.wnm_whonetfield.Contains("_NE"))
//                                                        { 
//                                                            string antibiotic = dbfDataReader["MICDR_ITSP"].ToString();
//                                                            //var lstAnti = MappingAnt.Select(m => m.wnm_originalfield).ToList();
//                                                            if (antibiotic == item.wnm_originalfield.Trim())
//                                                            {
//                                                                tempvalue = dbfDataReader["MICIT_TROU"].ToString();
//                                                            }
//                                                            else
//                                                            {
//                                                                continue;
//                                                            }
//                                                        }
//                                                        else 
//                                                        {
//                                                            if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == item.wnm_originalfield) != null)
//                                                            {
                                                               
//                                                                if (dbfDataReader[item.wnm_originalfield.Trim()] != null)
//                                                                {
//                                                                    Encoding = dbfDataReader[item.wnm_originalfield.Trim()].ToString();

//                                                                    if((!string.IsNullOrEmpty(selectedLab.lfu_FileTypeLabel)) && item.wnm_whonetfield.Contains("_NM"))
//                                                                    {                                                                        

//                                                                        if (selectedLab.lfu_FileTypeLabel == clsLabFileType.MLAB_FileType.MIC_SIR)
//                                                                        {
//                                                                            if (Encoding.ToUpper() == "S" || Encoding.ToUpper() == "I" || Encoding.ToUpper() == "R")
//                                                                            {
//                                                                                //MIC-SIR
//                                                                                tempvalue = Encoding;
//                                                                            }
//                                                                            else
//                                                                            {
//                                                                                // do nothing
//                                                                            }
//                                                                        }

//                                                                        else if (selectedLab.lfu_FileTypeLabel == clsLabFileType.MLAB_FileType.MIC_NUM)
//                                                                        {
//                                                                            if (Encoding.ToUpper() == "S" || Encoding.ToUpper() == "I" || Encoding.ToUpper() == "R")
//                                                                            {
//                                                                                // do nothing
//                                                                            }
//                                                                            else
//                                                                            {
//                                                                                //MIC-NUM
//                                                                                tempvalue = Encoding;

//                                                                            }
//                                                                        }
                                                                        
//                                                                        else
//                                                                        {
//                                                                            tempvalue = Encoding;
//                                                                        }

//                                                                        goto AddDetailPoint;
//                                                                    }
//                                                                    else
//                                                                    {
//                                                                        tempvalue = Encoding;
//                                                                    }
//                                                                }
//                                                            }
//                                                            else
//                                                            {
//                                                                continue;
//                                                            }
//                                                        }                                                            
//                                                    }
//AddDetailPoint:
//                                                    LabFileDataDetailList.Add(new TRSTGLabFileDataDetail
//                                                    {

//                                                        ldd_id = Guid.NewGuid(),
//                                                        ldd_status = 'N',
//                                                        ldd_whonetfield = item.wnm_whonetfield,
//                                                        ldd_originalfield = item.wnm_originalfield.Trim(),
//                                                        ldd_originalvalue = tempvalue,
//                                                        ldd_ldh_id = ldh_id,
//                                                        ldd_createuser = "BATCH",
//                                                        ldd_createdate = DateTime.Now
//                                                    }
//                                                    );

//                                                }
//                                            }
//                                        }
//                                        catch (Exception ex)
//                                        {

//                                        }
//                                        //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End loop Detail ... ");
//                                        #endregion                                       
//                                    }

//                                    if (!blnRowError  && WARD_TYPE != "" && (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == WARD_TYPE) != null))
//                                    {
//                                        var ward = dbfDataReader[WARD_TYPE].ToString().ToUpper();
//                                        if (ward == "" || !lstWardType.Contains(ward.Trim())) { blnRowError = true; }
//                                    }

//                                    if (!blnRowError && SOURCE != "" && (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == SOURCE) != null))
//                                    {
//                                        var localspecimen = dbfDataReader[SOURCE].ToString().ToUpper();
//                                        if (localspecimen == "" || !lstSpecimen.Contains(localspecimen.Trim())) { blnRowError = true; }
//                                    }

//                                    if (!blnRowError && ORGANISM != "" && (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == ORGANISM) != null))
//                                    {
//                                        var localorganism = dbfDataReader[ORGANISM].ToString().ToUpper();
//                                        if (localorganism == "" || !lstOrganism.Contains(localorganism.Trim())) { blnRowError = true; }
//                                    }

//                                    if (blnRowError) { nRowError += 1; }
//                                    dataRow += 1;
//                                }

//                                //EndWhile:


//                                if (AntibioticErrorD.Count > 0)
//                                {
//                                    //nRowError += ErrorAnt;
//                                    //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Save Error Header ... ");
//                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
//                                    objErrorH.feh_id = Guid.NewGuid();
//                                    objErrorH.feh_status = 'N';
//                                    objErrorH.feh_flagdelete = false;
//                                    objErrorH.feh_type = "NOT_MATCH";
//                                    objErrorH.feh_field = "Antibiotic";
//                                    objErrorH.feh_message = "Antibiotic not match";
//                                    objErrorH.feh_errorrecord = ErrorAnt;
//                                    objErrorH.feh_createuser = "BATCH";
//                                    objErrorH.feh_createdate = DateTime.Now;
//                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
//                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);
//                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Error Header ... ");

//                                    AntibioticErrorD = AntibioticErrorD.Select(a => { a.fed_feh_id = objErrorH.feh_id; return a; }).ToList();
//                                    var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetailList(AntibioticErrorD);
                             
//                                }

//                                if (LabFileDataHeaderList.Count() != 0)
//                                {
//                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Save Header List ... ");
//                                    var log_std = new LogWriter("Start Save Header List ...");

//                                    LabDataDAL.InsertBulk_LabFileDataHeader(LabFileDataHeaderList);

//                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Header List ... ");
//                                    var log_endd = new LogWriter("End Save Header List ...");
//                                }


//                                if (LabFileDataDetailList.Count != 0)
//                                {
//                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Export Detail ... ");
//                                    var log_std = new LogWriter("Start Export Detail ...");
//                                    //var ldd = LabDataDAL.Save_LabFileDataDetail(LabFileDataDetailList);
//                                    var ldd = Export_LabFileDataDetail(labdetail_path,LabFileDataDetailList, selectedLab);

//                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Export Detail ... ");
//                                    var log_endd = new LogWriter("End Export Detail ...");
//                                }
//                            }

//                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Read File (.DBF) ... ");
//                            var log_tranend = new LogWriter("End Read File (.DBF) ...");
//                        }


//                        Finish:
//                        char str;
//                        //if (ErrorAnt == 0)
//                        //{
//                        str = 'Q'; //'M'; 
//                        //str = 'M';

//                        //}
//                        //else
//                        //{
//                        //    str = 'E';
//                        //}
//                        //var rowsAffected2 = LabDataDAL.Update_LabFileUploadStatus(selectedLab.lfu_id.ToString(), str, chkError + chkDetailError, "BATCH");
//                        var rowsAffected2 = LabDataDAL.Update_LabFileUploadStatus(selectedLab.lfu_id.ToString(), str, nRowError, "BATCH");

//                        var log_ed = new LogWriter(">> End File Name : " + selectedLab.lfu_FileName);
//                    }
//                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Batch Lab End ... ");
//                    var log_end = new LogWriter("Batch Lab Complete ...");
//                }

//            }
//            catch (Exception ex)
//            {
//                if (ex.Message.Contains("Timeout Expired"))
//                {
//                    var rowsTimeOut = LabDataDAL.Update_LabFileUploadStatus(iFileUploadID.ToString(), 'T', 0, "BATCH");
//                }
//                else
//                {                   
//                    var exMsg = ex.Message.Substring(0, (ex.Message.Length > 200)? 200: ex.Message.Length);
//                    var rowsAffected = LabDataDAL.Update_LabFileUploadStatus(iFileUploadID.ToString(), 'I', 0, "BATCH", exMsg);
//                }
               
//                var d = string.Format("Row : {0} Detail : {1}", (dataRow+1).ToString(), ex.Message);

//                var logw = new LogWriter(d);
//                Console.WriteLine(d);
//            }
        }

        //public static DateTime CovertStringToDate(string fieldValue, string DateFormat)
        //{
        //    DateTime tmpFieldValue;

        //    string formatmapping = DateFormat;
        //    string format1 = "M/d/yyyy H:mm:ss";
        //    string format2 = "M/d/yyyy H:mm:ss tt";
        //    string format3 = "M/d/yyyy H:mm:sss";
        //    string format4 = "M/d/yyyy H:mm:sss tt";
        //    string format5 = "M/d/yyyy";
        //    string format6 = "d-M-yyyy";
        //    string format7 = "yyyy-M-d";

        //    if (DateTime.TryParseExact(fieldValue, formatmapping, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
        //    {
        //        fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
        //    }
        //    else if(DateTime.TryParseExact(fieldValue, format1, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
        //    {
        //        fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
        //    }
        //    else if (DateTime.TryParseExact(fieldValue, format2, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
        //    {
        //        fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
        //    }
        //    else if (DateTime.TryParseExact(fieldValue, format3, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
        //    {
        //        fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
        //    }
        //    else if (DateTime.TryParseExact(fieldValue, format4, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
        //    {
        //        fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
        //    }
        //    else if (DateTime.TryParseExact(fieldValue.Split(' ')[0], format5, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
        //    {
        //        fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
        //    }
        //    else if (DateTime.TryParseExact(fieldValue, format6, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
        //    {
        //        fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
        //    }
        //    else if (DateTime.TryParseExact(fieldValue, format7, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
        //    {
        //        fieldValue = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
        //    }
        //    else
        //    {
        //        fieldValue = null;
        //    }
        //    return tmpFieldValue;
        //}

        //public static string convertColumn(string originalColumn)
        //{
        //    string objReturn = "";
        //    int index = 0;
        //    Int32.TryParse(originalColumn.Replace("Column", ""), out index);

        //    objReturn = "Column" + (index - 1);

        //    return objReturn;
        //}
        //public static string Export_LabFileDataDetail(string labdetail_path, List<TRSTGLabFileDataDetail> model, LabFileUploadDataDTO selectedLab)
        //{
        //    var strStatus = "";
        //    var hos_code = selectedLab.lfu_hos_code;
        //    var lfu_FileName = selectedLab.lfu_FileName;
        //    var lfu_id = selectedLab.lfu_id.ToString();

        //    lfu_FileName = lfu_FileName.Replace(Path.GetExtension(lfu_FileName), ".csv");
        //    var filename = string.Format("{0}__{1}", lfu_id, lfu_FileName);
        //    string str_CurrentDate = selectedLab.lfu_createdate.Value.ToString("yyyyMMdd");
        //    var strFilePath = Path.Combine(labdetail_path, str_CurrentDate, hos_code);           
        //    var idx = 0;
        //    if (!File.Exists(strFilePath))
        //    {
        //        System.IO.Directory.CreateDirectory(strFilePath);
        //    }

        //    var strFullPath = Path.Combine(strFilePath, filename);
        //    StreamWriter sw = new StreamWriter(strFullPath, false);//, Encoding.UTF8

        //    foreach (var lst in model)
        //    {
        //        sw.Write(lst.ldd_id + ","
        //                + lst.ldd_status + ","
        //                + lst.ldd_whonetfield + ","
        //                + lst.ldd_originalfield + ","
        //                + lst.ldd_originalvalue.Replace(",", "*-*") + ","
        //                + ","
        //                + lst.ldd_ldh_id + ","
        //                + lst.ldd_createuser + ",,,"
        //                );//, Encoding.UTF8
        //        idx += 1;
        //        if (idx != model.Count()) { sw.Write(sw.NewLine); }            
        //    }

        //    sw.Close();
        //    strStatus = "OK";
        //    return strStatus;
        //}
           
    }
}
