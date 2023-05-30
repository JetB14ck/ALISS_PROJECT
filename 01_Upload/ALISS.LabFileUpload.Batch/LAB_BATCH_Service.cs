using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ALISS.LabFileUpload.Batch.DataAccess;
using ALISS.LabFileUpload.Batch.Models;
using ALISS.LabFileUpload.DTO;
using ALISS.Mapping.DTO;
using DbfDataReader;
using ExcelDataReader;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using Microsoft.Data.SqlClient;
using ALISS.LabFileUpload.Batch.Helpers;

namespace ALISS.LabFileUpload.Batch
{
    public class LAB_BATCH_Service : IDisposable
    {
        public LAB_BATCH_Service()
        {
            //whonetPath = P00_BATCH_Service.GetConfigurationValue("WHONET:PARAM:PATH");
            
        }
        public void Dispose()
        {

        }
        public void LAB_STGHEADERDETAIL()
        {
            var LabDataDAL = new LabDataDAL();
            var LabFileUpload = LabDataDAL.Get_NewLabFileUpload('N');
            var LabFileUploadReprocess = LabDataDAL.Get_NewLabFileUpload('R');
            var LabFileUploadTimeOut = LabDataDAL.Get_NewLabFileUpload('T');

            if (LabFileUploadReprocess.Count > 0)
            {
                LabFileUpload.AddRange(LabFileUploadReprocess);
            }
            if (LabFileUploadTimeOut.Count > 0)
            {
                LabFileUpload.AddRange(LabFileUploadTimeOut);
            }
            string Param_labno = "";
            string Param_organism = "";
            string Param_specimen = "";
            string Param_date = "";
            string Param_wardtype = "";
            string lfu_mp_id = "";
            string labdetail_path = "";

            //int chkError = 0;
            int chkDetailError = 0;
            int nRowError = 0;
            bool blnRowError = false;
            Guid iFileUploadID = Guid.Empty;
            int nfile = 0;
            List<LabFileUploadDataDTO> LabFile = new List<LabFileUploadDataDTO>();
            int ErrorAnt = 0;
            int dataRow = 0;
            int dataLabFilePerRound = 3;
            try
            {

                if (LabFileUpload != null)
                {
                    var sortLab = LabFileUpload.OrderBy(o => o.lfu_createdate).ToList();
                    if (LabFileUpload.Count() > dataLabFilePerRound)
                    {
                        for (var i = 0; i < dataLabFilePerRound; i++)
                        {
                            LabFile.Add(sortLab[i]);
                            if (sortLab[i].lfu_status == 'T')
                            {
                                LabDataDAL.Delete_LabFileDataHeaderDetail(sortLab[i].lfu_id.ToString());
                            }
                            var rowsReprocDeleteFlag = LabDataDAL.Update_LabFileErrorStatus(sortLab[i].lfu_id.ToString(), 'D', "BATCH");
                            var rowsAffected = LabDataDAL.Update_LabFileUploadStatus(sortLab[i].lfu_id.ToString(), 'P', 0, "BATCH");
                            nfile += 1;
                        }
                    }
                    else
                    {
                        foreach (LabFileUploadDataDTO Lab in sortLab)
                        {
                            LabFile.Add(Lab);
                            if (Lab.lfu_status == 'T')
                            {
                                LabDataDAL.Delete_LabFileDataHeaderDetail(Lab.lfu_id.ToString());
                            }
                            var rowsReprocDeleteFlag = LabDataDAL.Update_LabFileErrorStatus(Lab.lfu_id.ToString(), 'D', "BATCH");
                            var rowsAffected = LabDataDAL.Update_LabFileUploadStatus(Lab.lfu_id.ToString(), 'P', 0, "BATCH");
                            nfile += 1;
                        }
                    }

                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Batch Lab Start ... ");
                    var log_start = new LogWriter(string.Format("Batch Lab Start {0}/{1} files ...", nfile, LabFileUpload.Count()));

                    List<TCParameter> ParameterKeyList = LabDataDAL.GetParameter("UPLOAD_KEY");
                    List<TCParameter> ParameterUploadList = LabDataDAL.GetParameter("UPLOAD_MAPPING");
                    List<TCParameter> ParameterMLABMappingList = LabDataDAL.GetParameter("MLAB_MAPPING_TEMPLATE");
                    List<TCParameter> ParameterPathList = LabDataDAL.GetParameter("LAB_DETAIL_PATH");

                    if (ParameterKeyList.Count != 0)
                    {
                        Param_labno = ParameterKeyList.FirstOrDefault(x => x.prm_code_minor == "LAB_NO").prm_value;
                        Param_organism = ParameterKeyList.FirstOrDefault(x => x.prm_code_minor == "ORGANISM").prm_value;
                        Param_specimen = ParameterKeyList.FirstOrDefault(x => x.prm_code_minor == "SPECIMEN").prm_value;
                        Param_date = ParameterKeyList.FirstOrDefault(x => x.prm_code_minor == "DATE").prm_value;
                    }

                    if (ParameterUploadList.Count != 0)
                    {
                        Param_wardtype = ParameterUploadList.FirstOrDefault(x => x.prm_code_minor == "WARD_TYPE").prm_value;
                    }

                    if (ParameterMLABMappingList.Count != 0)
                    {
                        lfu_mp_id = ParameterMLABMappingList.FirstOrDefault(x => x.prm_code_minor == "mp_id").prm_value;
                    }

                    if (ParameterPathList.Count != 0)
                    {
                        labdetail_path = ParameterPathList.FirstOrDefault(x => x.prm_code_minor == "PATH").prm_value;
                    }
                    //ดัก approve

                    foreach (LabFileUploadDataDTO selectedLab in LabFile)
                    {
                        var log_st = new LogWriter(string.Format(">> Hos : ({0}-{1}) File Name : {2}", selectedLab.lfu_hos_code, selectedLab.lfu_hos_name, selectedLab.lfu_FileName));

                        chkDetailError = 0;
                        nRowError = 0;
                        ErrorAnt = 0;
                        iFileUploadID = selectedLab.lfu_id;
                        if (selectedLab.lfu_Program != "MLAB") { lfu_mp_id = selectedLab.lfu_mp_id.ToString(); }
                        var MappingTemplate = LabDataDAL.GetMappingData(lfu_mp_id);
                        var whonetmapping = LabDataDAL.GetWHONetMappingList(lfu_mp_id, MappingTemplate.mp_mst_code);
                        var wardtypmapping = LabDataDAL.GetWardTypeMappingList(lfu_mp_id, MappingTemplate.mp_mst_code, selectedLab.lfu_hos_code);
                        var specimenmapping = LabDataDAL.GetSpecimeneMappingList(lfu_mp_id, MappingTemplate.mp_mst_code);
                        var organismmapping = LabDataDAL.GetOrganismMappingList(lfu_mp_id, MappingTemplate.mp_mst_code);

                        var lstWardType = wardtypmapping.Select(s => s.wdm_warddesc.ToUpper()).Distinct().ToList();
                        var lstSpecimen = specimenmapping.Select(s => s.spm_localspecimencode.ToUpper()).Distinct().ToList();
                        var lstOrganism = organismmapping.Select(s => s.ogm_localorganismcode.ToUpper()).Distinct().ToList();

                        //Get standard field
                        var WhonetmappingWithoutNM_NE_ND = whonetmapping.Except(whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NM") || x.wnm_whonetfield.Contains("_ND") || x.wnm_whonetfield.Contains("_NE")).ToList());

                        //Check file type and add whonetmapping
                        switch (selectedLab.lfu_FileType)
                        {
                            case clsLabFileType.MLAB_FileType.MIC:
                                whonetmapping = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NM")).ToList();
                                whonetmapping.AddRange(WhonetmappingWithoutNM_NE_ND);
                                break;
                            case clsLabFileType.MLAB_FileType.DISK:
                                whonetmapping = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_ND")).ToList();
                                whonetmapping.AddRange(WhonetmappingWithoutNM_NE_ND);
                                break;
                            case clsLabFileType.MLAB_FileType.ETEST:
                                whonetmapping = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NE")).ToList();
                                whonetmapping.AddRange(WhonetmappingWithoutNM_NE_ND);
                                break;
                        }


                        var listexcludeAntibiotic = new List<string>();
                        string AntiLocalCodeColumnEtest = "MICDR_ITSP";
                        string AntiValueColumnEtest = "MICIT_TROU";
                        EnumerableRowCollection<DataRow> AntNotMatch;
                        var lstAntErrDistinct = new List<string>();
                        var lstAntErrirCurrentYear = new List<string>();
                        int cntErrAntiCurrentYear = 0;

                        if (Path.GetExtension(selectedLab.lfu_FileName) == ".xls" || Path.GetExtension(selectedLab.lfu_FileName) == ".xlsx" || Path.GetExtension(selectedLab.lfu_FileName) == ".csv" || Path.GetExtension(selectedLab.lfu_FileName) == ".txt")
                        {
                            Guid feh_id_cdate = Guid.Empty;
                            char feh_status_cdate = 'N';
                            int cDateError = 0;
                            int cSpecDateError = 0;
                            string FieldDateType = "", DateFormat = "";
                            string LAB_NO = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_labno).wnm_originalfield;
                            string ORGANISM = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_organism).wnm_originalfield;
                            string DATE = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_date).wnm_originalfield;
                            DateFormat = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_date).wnm_fieldformat;

                            string WARD_TYPE = "";
                            if (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_wardtype) != null)
                                WARD_TYPE = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_wardtype).wnm_originalfield;

                            string SOURCE = "";
                            if (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_specimen) != null)
                                SOURCE = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_specimen).wnm_originalfield;

                            if (MappingTemplate.mp_firstlineisheader == false)
                            {
                                LAB_NO = convertColumn(LAB_NO);
                                ORGANISM = convertColumn(ORGANISM);

                                if (SOURCE != "")
                                    SOURCE = convertColumn(SOURCE);

                                DATE = convertColumn(DATE);
                            }

                            List<TRSTGLabFileDataHeader> LabFileDataHeaderList = new List<TRSTGLabFileDataHeader>();
                            List<TRSTGLabFileDataDetail> LabFileDataDetailList = new List<TRSTGLabFileDataDetail>();

                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Read File ... ");
                            var log_tran = new LogWriter("Start Read File ... ");

                            using (var stream = File.Open(selectedLab.lfu_Path, FileMode.Open, FileAccess.Read))
                            {
                                DataSet result = new DataSet();

                                if (Path.GetExtension(selectedLab.lfu_FileName) == ".xls" || Path.GetExtension(selectedLab.lfu_FileName) == ".xlsx")
                                {
                                    ExcelDataSetConfiguration option = new ExcelDataSetConfiguration();
                                    var reader = ExcelReaderFactory.CreateReader(stream);
                                    if (MappingTemplate.mp_firstlineisheader == true)
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
                                else if (Path.GetExtension(selectedLab.lfu_FileName) == ".csv")
                                {
                                    var reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
                                    {
                                        FallbackEncoding = Encoding.GetEncoding(1252),
                                        AutodetectSeparators = new char[] { ',', ';', '\t', '|', '#' },
                                        LeaveOpen = false,
                                        AnalyzeInitialCsvRows = 0,
                                    });

                                    if (MappingTemplate.mp_firstlineisheader == true)
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
                                else if (Path.GetExtension(selectedLab.lfu_FileName) == ".txt")
                                {
                                    string line;
                                    DataTable dt = new DataTable();

                                    using (TextReader tr = File.OpenText(selectedLab.lfu_Path))
                                    {
                                        while ((line = tr.ReadLine()) != null)
                                        {
                                            string[] items = line.Split('\t');
                                            if (dt.Columns.Count == 0)
                                            {
                                                if (MappingTemplate.mp_firstlineisheader == false)
                                                {
                                                    for (int i = 0; i < items.Length; i++)
                                                        dt.Columns.Add(new DataColumn("Column" + i, typeof(string)));
                                                }
                                                else
                                                {
                                                    for (int i = 0; i < items.Length; i++)
                                                        dt.Columns.Add(new DataColumn(items[i].ToString(), typeof(string)));
                                                }
                                            }
                                            dt.Rows.Add(items);
                                        }
                                    }

                                    result.Tables.Add(dt);


                                }

                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Read File ... ");
                                var log_tran1 = new LogWriter("End Read File ...");

                                if (result.Tables[0].Columns.Contains(DATE) == true)
                                {
                                    FieldDateType = result.Tables[0].Columns[DATE].DataType.ToString();
                                }


                                //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
                                //>> for Etest : Antibiotic Isolate more than 1 Record
                                if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
                                {
                                    //var MappingAnt = whonetmapping.Where(x => x.wnm_antibiotic != null);
                                    var MappingAnt = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NE")).ToList();
                                    // Antibioticcolumn = MappingAnt.FirstOrDefault().wnm_antibioticcolumn;

                                    if (MappingTemplate.mp_firstlineisheader == false)
                                    {
                                        AntiLocalCodeColumnEtest = convertColumn(AntiLocalCodeColumnEtest);
                                    }

                                    //var listexcludeAntibiotic = new List<string>();

                                    foreach (WHONetMappingListsDTO t in MappingAnt)
                                    {
                                        //listexcludeAntibiotic.Add(t.wnm_antibiotic);
                                        listexcludeAntibiotic.Add(t.wnm_originalfield);
                                    }
                                }
                                else
                                {
                                    if (MappingTemplate.mp_AntibioticIsolateOneRec == false) //Other type but Antibiotic in many rows (Automate)
                                    {
                                        var MappingAnt = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NM")).ToList();
                                        AntiLocalCodeColumnEtest = (from s in whonetmapping
                                                                    where !string.IsNullOrEmpty(s.wnm_antibioticcolumn)
                                                                    select s.wnm_antibioticcolumn).Distinct().FirstOrDefault();
                                        if (MappingTemplate.mp_firstlineisheader == false)
                                        {                                            
                                            AntiLocalCodeColumnEtest = convertColumn(AntiLocalCodeColumnEtest);
                                        }
                                        foreach (WHONetMappingListsDTO t in MappingAnt)
                                        {

                                            listexcludeAntibiotic.Add(t.wnm_antibiotic);
                                        }
                                    }
                                }

                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Header Data ... ");
                                var log_read = new LogWriter("Start Header Data ...");
                                dataRow = 1;

                                if (listexcludeAntibiotic.Count > 0)
                                {
                                    AntNotMatch = result.Tables[0].AsEnumerable().Where(row => !listexcludeAntibiotic.Contains(row.Field<string>(AntiLocalCodeColumnEtest)));
                                    ErrorAnt = AntNotMatch.Count();
                                    var AntNotMatchDistinct = AntNotMatch.AsEnumerable().Select(s => new { antibiotic = s.Field<string>(AntiLocalCodeColumnEtest), }).Distinct().ToList();
                                    foreach (var lst in AntNotMatchDistinct)
                                    {
                                        lstAntErrDistinct.Add(lst.antibiotic);
                                    }
                                }

                                //Loop Read Data Row
                                foreach (DataRow row in result.Tables[0].Rows)
                                {
                                    string cmethod = "";
                                    blnRowError = false;

                                    if (result.Tables[0].Columns.Contains("CMETHOD"))
                                    {
                                        cmethod = row["CMETHOD"].ToString();
                                    }

                                    if ((cmethod == "Aerobic Culture" && selectedLab.lfu_Program == "MLAB")
                                   || selectedLab.lfu_Program != "MLAB"
                                   || (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST"))
                                    {

                                        #region InsertLabFileDataHeader
                                        TRSTGLabFileDataHeader objModel = new TRSTGLabFileDataHeader();
                                        Guid ldh_id = Guid.NewGuid();

                                        // key is null, continue
                                        if (!string.IsNullOrEmpty(LAB_NO) && string.IsNullOrEmpty(row[LAB_NO].ToString()))
                                        {
                                            continue;
                                        }
                                        else if (!string.IsNullOrEmpty(ORGANISM) && string.IsNullOrEmpty(row[ORGANISM].ToString()))
                                        {
                                            continue;
                                        }
                                        else if (!string.IsNullOrEmpty(DATE) && string.IsNullOrEmpty(row[DATE].ToString()))
                                        {
                                            continue;
                                        }

                                        if (selectedLab.lfu_FileType != "ETEST")
                                        {
                                            if (!string.IsNullOrEmpty(SOURCE) && string.IsNullOrEmpty(row[SOURCE].ToString()))
                                            {
                                                continue;
                                            }
                                        }


                                        // -- Check Date Format
                                        objModel.ldh_date = row[DATE].ToString();
                                        if (FieldDateType == "System.String")
                                        {
                                            try
                                            {
                                                //objModel.ldh_cdate = DateTime.ParseExact(objModel.ldh_date, DateFormat, CultureInfo.GetCultureInfo("en-US"));
                                                objModel.ldh_cdate = CovertStringToDate(objModel.ldh_date, DateFormat);
                                                int year = objModel.ldh_cdate.Value.Year;

                                                if (year < 1000)
                                                {
                                                    if (feh_id_cdate == Guid.Empty)
                                                    {
                                                        feh_id_cdate = Guid.NewGuid();
                                                    }
                                                    else
                                                    {
                                                        feh_status_cdate = 'E';
                                                    }


                                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                    objErrorH.feh_id = feh_id_cdate;
                                                    objErrorH.feh_status = feh_status_cdate;
                                                    objErrorH.feh_flagdelete = false;
                                                    objErrorH.feh_type = "CONVERT_ERROR";
                                                    objErrorH.feh_field = DATE;
                                                    objErrorH.feh_message = "Cannot convert date.";
                                                    cSpecDateError += 1;
                                                    objErrorH.feh_errorrecord = cSpecDateError;
                                                    objErrorH.feh_createuser = "BATCH";
                                                    objErrorH.feh_createdate = DateTime.Now;
                                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                    blnRowError = true;
                                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);




                                                    continue;
                                                }

                                                if (year > DateTime.Now.Year)
                                                {
                                                    objModel.ldh_cdate = objModel.ldh_cdate.Value.AddYears(-543);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                //chkError++;

                                                if (feh_id_cdate == Guid.Empty)
                                                {
                                                    feh_id_cdate = Guid.NewGuid();
                                                }
                                                else
                                                {
                                                    feh_status_cdate = 'E';
                                                }


                                                TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                objErrorH.feh_id = feh_id_cdate;
                                                objErrorH.feh_status = feh_status_cdate;
                                                objErrorH.feh_flagdelete = false;
                                                objErrorH.feh_type = "CONVERT_ERROR";
                                                objErrorH.feh_field = DATE;
                                                objErrorH.feh_message = "Cannot convert date.";
                                                cSpecDateError += 1;
                                                objErrorH.feh_errorrecord = cSpecDateError;
                                                objErrorH.feh_createuser = "BATCH";
                                                objErrorH.feh_createdate = DateTime.Now;
                                                objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                blnRowError = true;
                                                var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);


                                                TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
                                                objErrorD.fed_id = Guid.NewGuid();
                                                objErrorD.fed_status = 'N';
                                                objErrorD.fed_localvalue = ex.Message;
                                                objErrorD.fed_feh_id = feh_id_cdate;
                                                objErrorD.fed_createuser = "BATCH";
                                                objErrorD.fed_createdate = DateTime.Now;

                                                var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
                                                continue;
                                            }
                                        }
                                        else if (FieldDateType == "System.DateTime")
                                        {
                                            objModel.ldh_cdate = (DateTime)row[DATE];
                                            int year = objModel.ldh_cdate.Value.Year;
                                            if (year > DateTime.Now.Year)
                                            {
                                                objModel.ldh_cdate = objModel.ldh_cdate.Value.AddYears(-543);
                                            }
                                        }
                                        else if (FieldDateType == "System.Object") //-- am add 18/01/2021 --
                                        {
                                            if (row[DATE].GetType().FullName == "System.DateTime")
                                            {
                                                objModel.ldh_cdate = (DateTime)row[DATE];
                                                int year = objModel.ldh_cdate.Value.Year;
                                                if (year > DateTime.Now.Year)
                                                {
                                                    objModel.ldh_cdate = objModel.ldh_cdate.Value.AddYears(-543);
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    objModel.ldh_cdate = CovertStringToDate(objModel.ldh_date, DateFormat);
                                                    //objModel.ldh_cdate = DateTime.ParseExact(objModel.ldh_date, DateFormat, CultureInfo.GetCultureInfo("en-US"));
                                                    int year = objModel.ldh_cdate.Value.Year;

                                                    if (year < 1000)
                                                    {
                                                        if (feh_id_cdate == Guid.Empty)
                                                        {
                                                            feh_id_cdate = Guid.NewGuid();
                                                        }
                                                        else
                                                        {
                                                            feh_status_cdate = 'E';
                                                        }


                                                        TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                        objErrorH.feh_id = feh_id_cdate;
                                                        objErrorH.feh_status = feh_status_cdate;
                                                        objErrorH.feh_flagdelete = false;
                                                        objErrorH.feh_type = "CONVERT_ERROR";
                                                        objErrorH.feh_field = DATE;
                                                        objErrorH.feh_message = "Cannot convert date.";
                                                        cSpecDateError += 1;
                                                        objErrorH.feh_errorrecord = cSpecDateError;
                                                        objErrorH.feh_createuser = "BATCH";
                                                        objErrorH.feh_createdate = DateTime.Now;
                                                        objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                        blnRowError = true;


                                                        var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);
                                                    }

                                                    if (year > DateTime.Now.Year)
                                                    {
                                                        objModel.ldh_cdate = objModel.ldh_cdate.Value.AddYears(-543);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    //chkError++;

                                                    if (feh_id_cdate == Guid.Empty)
                                                    {
                                                        feh_id_cdate = Guid.NewGuid();
                                                    }
                                                    else
                                                    {
                                                        feh_status_cdate = 'E';
                                                    }


                                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                    objErrorH.feh_id = feh_id_cdate;
                                                    objErrorH.feh_status = feh_status_cdate;
                                                    objErrorH.feh_flagdelete = false;
                                                    objErrorH.feh_type = "CONVERT_ERROR";
                                                    objErrorH.feh_field = DATE;
                                                    objErrorH.feh_message = "Cannot convert date.";
                                                    cSpecDateError += 1;
                                                    objErrorH.feh_errorrecord = cSpecDateError;
                                                    objErrorH.feh_createuser = "BATCH";
                                                    objErrorH.feh_createdate = DateTime.Now;
                                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                    blnRowError = true;


                                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

                                                    TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
                                                    objErrorD.fed_id = Guid.NewGuid();
                                                    objErrorD.fed_status = 'N';
                                                    objErrorD.fed_localvalue = ex.Message;
                                                    objErrorD.fed_feh_id = objTRLabFileErrorHeader;
                                                    objErrorD.fed_createuser = "BATCH";
                                                    objErrorD.fed_createdate = DateTime.Now;

                                                    var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
                                                    continue;
                                                }
                                            }
                                        }

                                        // Etest and Data not this year : continue
                                        if (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST" && objModel.ldh_cdate.Value.Year != selectedLab.lfu_DataYear)
                                        {
                                            continue;
                                        }

                                        // -- End Check Date Format

                                        // -- Check Antibiotic                                     
                                        if (lstAntErrDistinct.Count > 0)
                                        {
                                            var anti = row[AntiLocalCodeColumnEtest].ToString();
                                            if (lstAntErrDistinct.Contains(anti))
                                            {
                                                cntErrAntiCurrentYear += 1;
                                                blnRowError = true;
                                                if (!lstAntErrirCurrentYear.Contains(anti))
                                                {
                                                    lstAntErrirCurrentYear.Add(anti);
                                                }
                                                continue;
                                            }
                                        }
                                        // -- End Check Antibiotic

                                        objModel.ldh_id = ldh_id;
                                        objModel.ldh_status = 'N';
                                        objModel.ldh_flagdelete = false;
                                        objModel.ldh_hos_code = selectedLab.lfu_hos_code;
                                        objModel.ldh_lab = selectedLab.lfu_lab;
                                        objModel.ldh_lfu_id = selectedLab.lfu_id;

                                        objModel.ldh_labno = row[LAB_NO].ToString();
                                        objModel.ldh_organism = row[ORGANISM].ToString();

                                        if (selectedLab.lfu_FileType != "ETEST")
                                        {
                                            if (SOURCE != "")
                                            {
                                                objModel.ldh_specimen = row[SOURCE].ToString();
                                            }
                                        }


                                        objModel.ldh_createuser = "BATCH";

                                        int seq = LabDataDAL.Get_CheckExistingHeader(objModel, MappingTemplate.mp_program, selectedLab.lfu_FileType);
                                        objModel.ldh_sequence = seq;
                                        objModel.ldh_createdate = DateTime.Now;
                                        Guid idh_id_related = Guid.Empty;

                                        bool isNotFirstline = false;
                                        //ETEST
                                        //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
                                        if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST || (MappingTemplate.mp_AntibioticIsolateOneRec == false))
                                        {
                                            //idh_id_related = LabDataDAL.Get_HeaderIdForMultipleline(objModel);
                                            TRSTGLabFileDataHeader dup = new TRSTGLabFileDataHeader();
                                            if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
                                            {
                                                dup = LabFileDataHeaderList.FirstOrDefault(x => x.ldh_lfu_id == objModel.ldh_lfu_id
                                                                          && x.ldh_hos_code == objModel.ldh_hos_code
                                                                          && x.ldh_lab == objModel.ldh_lab
                                                                          && x.ldh_labno == objModel.ldh_labno
                                                                          && x.ldh_organism == objModel.ldh_organism
                                                                          //&& x.ldh_specimen == objModel.ldh_specimen
                                                                          && x.ldh_date == objModel.ldh_date);
                                            }
                                            else
                                            {
                                                dup = LabFileDataHeaderList.FirstOrDefault(x => x.ldh_lfu_id == objModel.ldh_lfu_id
                                                                        && x.ldh_hos_code == objModel.ldh_hos_code
                                                                        && x.ldh_lab == objModel.ldh_lab
                                                                        && x.ldh_labno == objModel.ldh_labno
                                                                        && x.ldh_organism == objModel.ldh_organism
                                                                        && x.ldh_specimen == objModel.ldh_specimen
                                                                        && x.ldh_date == objModel.ldh_date);
                                            }                                                                                     
                                            
                                            if (dup != null)
                                            {
                                                idh_id_related = dup.ldh_id;
                                            }

                                            if (idh_id_related != Guid.Empty)
                                            {
                                                isNotFirstline = true;
                                                ldh_id = idh_id_related;
                                            }
                                            else
                                            {
                                                //var x = LabDataDAL.Save_LabFileDataHeader(objModel);
                                                LabFileDataHeaderList.Add(objModel);

                                            }
                                        }
                                        else
                                        {
                                            //var x = LabDataDAL.Save_LabFileDataHeader(objModel);
                                            LabFileDataHeaderList.Add(objModel);
                                        }


                                        #endregion

                                        #region InsertLabFileDataDetail

                                        if (whonetmapping != null)
                                        {
                                            foreach (WHONetMappingListsDTO item in whonetmapping)
                                            {
                                                String wnm_originalfield;
                                                var Encoding = "";

                                                if (MappingTemplate.mp_firstlineisheader == false)
                                                {

                                                    wnm_originalfield = convertColumn(item.wnm_originalfield.Trim());
                                                }
                                                else
                                                {
                                                    wnm_originalfield = item.wnm_originalfield.Trim();
                                                }

                                                if (isNotFirstline == true && selectedLab.lfu_FileType == "ETEST" && (wnm_originalfield == LAB_NO || wnm_originalfield == DATE || wnm_originalfield == ORGANISM))
                                                {
                                                    continue;
                                                }
                                                if (isNotFirstline == true && selectedLab.lfu_FileType != "ETEST" && (MappingTemplate.mp_AntibioticIsolateOneRec == false)
                                                    && (wnm_originalfield == LAB_NO || wnm_originalfield == DATE || wnm_originalfield == ORGANISM || wnm_originalfield == SOURCE || wnm_originalfield == WARD_TYPE))
                                                {
                                                    continue;
                                                }


                                                if (selectedLab.lfu_FileType == "ETEST" || (MappingTemplate.mp_AntibioticIsolateOneRec == false))
                                                {
                                                    if (selectedLab.lfu_FileType == "ETEST")
                                                    {
                                                        if (item.wnm_whonetfield.Contains("_NE"))
                                                        {
                                                            wnm_originalfield = AntiValueColumnEtest; //MICIT_TROU
                                                            if (!result.Tables[0].Columns.Contains(wnm_originalfield))
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (!result.Tables[0].Columns.Contains(wnm_originalfield.Trim()))
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                    }
                                                    else //MappingTemplate.mp_AntibioticIsolateOneRec == false
                                                    {
                                                        if (item.wnm_whonetfield.Contains("_NM"))
                                                        {
                                                            AntiValueColumnEtest = (from s in whonetmapping
                                                                                        where s.wnm_whonetfield.Contains("_NM")
                                                                                        select s.wnm_originalfield).Distinct().FirstOrDefault(); //Column20

                                                            if (MappingTemplate.mp_firstlineisheader == false)
                                                            {
                                                                AntiValueColumnEtest = convertColumn(AntiValueColumnEtest);
                                                            }
                                                            wnm_originalfield = AntiValueColumnEtest;
                                                            if (!result.Tables[0].Columns.Contains(wnm_originalfield))
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (!result.Tables[0].Columns.Contains(wnm_originalfield.Trim()))
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    if (!result.Tables[0].Columns.Contains(wnm_originalfield))
                                                    {
                                                        continue;
                                                    }
                                                }
                                                // ETEST
                                                //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
                                                if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST || (MappingTemplate.mp_AntibioticIsolateOneRec == false))
                                                {
                                                    if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
                                                    {
                                                        if (AntiLocalCodeColumnEtest != "")
                                                        {
                                                            //var antibioticcolumn = "";
                                                            if (MappingTemplate.mp_firstlineisheader == false)
                                                            {
                                                                AntiLocalCodeColumnEtest = convertColumn(AntiLocalCodeColumnEtest);
                                                            }

                                                            if (row.IsNull(AntiLocalCodeColumnEtest))
                                                            {
                                                                continue;
                                                            }
                                                            if (item.wnm_whonetfield.Contains("_NE"))
                                                            {
                                                                if (row[AntiLocalCodeColumnEtest].ToString() != item.wnm_originalfield.Trim())
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else //MappingTemplate.mp_AntibioticIsolateOneRec == false
                                                    {
                                                        if (AntiLocalCodeColumnEtest != "") //Column20
                                                        {
                                                            //var antibioticcolumn = "";
                                                            if (MappingTemplate.mp_firstlineisheader == false)
                                                            {
                                                                //AntiLocalCodeColumnEtest = convertColumn(AntiLocalCodeColumnEtest);
                                                            }

                                                            if (row.IsNull(AntiLocalCodeColumnEtest))
                                                            {
                                                                continue;
                                                            }
                                                            if (item.wnm_whonetfield.Contains("_NM"))
                                                            {
                                                                if (row[AntiLocalCodeColumnEtest].ToString() != item.wnm_antibiotic.Trim())
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                        }
                                                    }

                                                }

                                                if (row.IsNull(wnm_originalfield))
                                                {
                                                    continue;
                                                }

                                                string tempvalue = "";
                                                var xx = row[wnm_originalfield].GetType().ToString();
                                                if (row[wnm_originalfield].GetType().ToString() != "System.DateTime" && item.wnm_type == "Date")
                                                {
                                                    try
                                                    {
                                                        //var tempfielddate = DateTime.ParseExact(row[wnm_originalfield].ToString(), item.wnm_fieldformat, CultureInfo.GetCultureInfo("en-US"));
                                                        var tempfielddate = CovertStringToDate(row[wnm_originalfield].ToString(), item.wnm_fieldformat);
                                                        tempvalue = tempfielddate.ToString();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        chkDetailError++;

                                                        if (feh_id_cdate == Guid.Empty)
                                                        {
                                                            feh_id_cdate = Guid.NewGuid();
                                                        }
                                                        else
                                                        {
                                                            feh_status_cdate = 'E';
                                                        }
                                                        TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                        objErrorH.feh_id = feh_id_cdate;
                                                        objErrorH.feh_status = feh_status_cdate;
                                                        objErrorH.feh_flagdelete = false;
                                                        objErrorH.feh_type = "CONVERT_ERROR";
                                                        objErrorH.feh_field = item.wnm_originalfield.Trim();
                                                        objErrorH.feh_message = "Cannot convert date.";
                                                        cDateError = cDateError + 1;
                                                        objErrorH.feh_errorrecord = cDateError;
                                                        objErrorH.feh_createuser = "BATCH";
                                                        objErrorH.feh_createdate = DateTime.Now;
                                                        objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                        blnRowError = true;
                                                        var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

                                                        TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
                                                        objErrorD.fed_id = Guid.NewGuid();
                                                        objErrorD.fed_status = 'N';
                                                        objErrorD.fed_localvalue = ex.Message;
                                                        objErrorD.fed_feh_id = feh_id_cdate;
                                                        objErrorD.fed_createuser = "BATCH";
                                                        objErrorD.fed_createdate = DateTime.Now;

                                                        var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
                                                        continue;
                                                    }
                                                }
                                                else if (row[wnm_originalfield].GetType().ToString() == "System.DateTime" && item.wnm_type == "Date")
                                                {
                                                    var tempfielddate = (DateTime)row[wnm_originalfield];
                                                    tempvalue = tempfielddate.ToString();
                                                }
                                                else
                                                {
                                                    if ((!string.IsNullOrEmpty(selectedLab.lfu_FileTypeLabel)) && item.wnm_whonetfield.Contains("_NM"))
                                                    {
                                                        if (selectedLab.lfu_FileTypeLabel == clsLabFileType.MLAB_FileType.MIC_SIR)
                                                        {
                                                            if (row[wnm_originalfield].ToString().ToUpper() == "S" ||
                                                                 row[wnm_originalfield].ToString().ToUpper() == "I" ||
                                                                 row[wnm_originalfield].ToString().ToUpper() == "R")
                                                            {
                                                                //MIC-SIR
                                                                tempvalue = row[wnm_originalfield].ToString();
                                                            }
                                                            else
                                                            {
                                                                // do nothing
                                                            }
                                                        }
                                                        else if (selectedLab.lfu_FileTypeLabel == clsLabFileType.MLAB_FileType.MIC_NUM)
                                                        {
                                                            if (row[wnm_originalfield].ToString() == "S" ||
                                                                 row[wnm_originalfield].ToString() == "I" ||
                                                                 row[wnm_originalfield].ToString() == "R")
                                                            {
                                                                // do nothing
                                                            }
                                                            else
                                                            {
                                                                //MIC-NUM
                                                                tempvalue = row[wnm_originalfield].ToString();

                                                            }
                                                        }
                                                        else
                                                        {
                                                            tempvalue = row[wnm_originalfield].ToString();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        tempvalue = row[wnm_originalfield].ToString();
                                                    }
                                                    goto AddDetailPoint;
                                                }

                                                if ((selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST && item.wnm_whonetfield.Contains("_NE"))
                                                    || ((MappingTemplate.mp_AntibioticIsolateOneRec == false) && item.wnm_whonetfield.Contains("_NM"))
                                                    )
                                                {
                                                    string antibiotic = row[AntiLocalCodeColumnEtest].ToString();

                                                    if (antibiotic == item.wnm_originalfield.Trim())
                                                    {
                                                        tempvalue = row[AntiValueColumnEtest].ToString();
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    if (result.Tables[0].Columns.Contains(wnm_originalfield.Trim()))
                                                    {
                                                        tempvalue = row[wnm_originalfield.Trim()].ToString();
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }

                                            AddDetailPoint:
                                                if (item.wnm_encrypt == true)
                                                {
                                                    Encoding = CryptoHelper.UnicodeEncoding(tempvalue);
                                                }
                                                else
                                                {
                                                    Encoding = tempvalue;
                                                }

                                                LabFileDataDetailList.Add(new TRSTGLabFileDataDetail
                                                {
                                                    ldd_id = Guid.NewGuid(),
                                                    ldd_status = 'N',
                                                    ldd_whonetfield = item.wnm_whonetfield,
                                                    ldd_originalfield = item.wnm_originalfield.Trim(),
                                                    ldd_originalvalue = Encoding,
                                                    ldd_ldh_id = ldh_id,
                                                    ldd_createuser = "BATCH",
                                                    ldd_createdate = DateTime.Now
                                                }
                                                );

                                            }

                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    if (!blnRowError && WARD_TYPE != "" && (result.Tables[0].Columns.Contains(WARD_TYPE)))
                                    {
                                        var ward = row[WARD_TYPE].ToString().ToUpper();
                                        if (ward == "" || !lstWardType.Contains(ward.Trim())) { blnRowError = true; }
                                    }

                                    if (selectedLab.lfu_FileType != "ETEST")
                                    {
                                        if (!blnRowError && SOURCE != "" && (result.Tables[0].Columns.Contains(SOURCE)))
                                        {
                                            var localspecimen = row[SOURCE].ToString().ToUpper();
                                            if (localspecimen == "" || !lstSpecimen.Contains(localspecimen.Trim())) { blnRowError = true; }
                                        }
                                    }


                                    if (!blnRowError && ORGANISM != "" && (result.Tables[0].Columns.Contains(ORGANISM)))
                                    {
                                        var localorganism = row[ORGANISM].ToString().ToUpper();
                                        if (localorganism == "" || !lstOrganism.Contains(localorganism.Trim())) { blnRowError = true; }
                                    }


                                    if (blnRowError) { nRowError += 1; }
                                    dataRow += 1;
                                }// end loop read data row

                                //Save Antibiotic Error
                                if (lstAntErrirCurrentYear.Count > 0)
                                {
                                    nRowError = cntErrAntiCurrentYear;
                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                    objErrorH.feh_id = Guid.NewGuid();
                                    objErrorH.feh_status = 'N';
                                    objErrorH.feh_flagdelete = false;
                                    objErrorH.feh_type = "NOT_MATCH";
                                    objErrorH.feh_field = "Antibiotic";
                                    objErrorH.feh_message = "Antibiotic not match";
                                    objErrorH.feh_errorrecord = cntErrAntiCurrentYear;
                                    objErrorH.feh_createuser = "BATCH";
                                    objErrorH.feh_createdate = DateTime.Now;
                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                    blnRowError = true;
                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

                                    foreach (var item in lstAntErrirCurrentYear)
                                    {
                                        TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
                                        objErrorD.fed_id = Guid.NewGuid();
                                        objErrorD.fed_status = 'N';
                                        objErrorD.fed_localvalue = item;
                                        objErrorD.fed_feh_id = objErrorH.feh_id;
                                        objErrorD.fed_createuser = "BATCH";
                                        objErrorD.fed_createdate = DateTime.Now;

                                        var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
                                    }
                                }

                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Header Data ... ");
                                var log_endr = new LogWriter("End Header Data ...");

                                if (LabFileDataHeaderList.Count() != 0)
                                {
                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Save Header List ... ");
                                    var log_std = new LogWriter("Start Save Header List ...");

                                    LabDataDAL.InsertBulk_LabFileDataHeader(LabFileDataHeaderList);

                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Header List... ");
                                    var log_endd = new LogWriter("End Save Header List ...");
                                }


                                if (LabFileDataDetailList.Count != 0)
                                {
                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Save Detail ... ");
                                    var log_std = new LogWriter("Start Save Detail ...");

                                    //var ldd = LabDataDAL.Save_LabFileDataDetail(LabFileDataDetailList);
                                    var ldd = Export_LabFileDataDetail(labdetail_path, LabFileDataDetailList, selectedLab);
                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Detail ... ");
                                    var log_endd = new LogWriter("End Save Detail ...");
                                }
                            }

                        }
                        
                        #region sqlite
                        if (Path.GetExtension(selectedLab.lfu_FileName) == ".sqlite")
                        {
                            Guid feh_id_cdate = Guid.Empty;
                            char feh_status_cdate = 'N';
                            int cDateError = 0;
                            int cSpecDateError = 0;
                            string FieldDateType = "", DateFormat = "";
                            string LAB_NO = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_labno).wnm_originalfield;
                            string ORGANISM = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_organism).wnm_originalfield;
                            string DATE = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_date).wnm_originalfield;
                            DateFormat = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_date).wnm_fieldformat;

                            string WARD_TYPE = "";
                            if (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_wardtype) != null)
                                WARD_TYPE = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_wardtype).wnm_originalfield;

                            string SOURCE = "";
                            if (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_specimen) != null)
                                SOURCE = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_specimen).wnm_originalfield;

                            if (MappingTemplate.mp_firstlineisheader == false)
                            {
                                LAB_NO = convertColumn(LAB_NO);
                                ORGANISM = convertColumn(ORGANISM);

                                if (SOURCE != "")
                                    SOURCE = convertColumn(SOURCE);

                                DATE = convertColumn(DATE);
                            }

                            List<TRSTGLabFileDataHeader> LabFileDataHeaderList = new List<TRSTGLabFileDataHeader>();
                            List<TRSTGLabFileDataDetail> LabFileDataDetailList = new List<TRSTGLabFileDataDetail>();

                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Read File ... ");
                            var log_tran = new LogWriter("Start Read File ... ");

                            //using (var stream = File.Open(selectedLab.lfu_Path, FileMode.Open, FileAccess.Read))
                            //{
                                Batteries.Init();
                                DataSet result = new DataSet();

                                string connectionString = string.Format("Data Source={0}", selectedLab.lfu_Path);
                                using (SqliteConnection connection = new SqliteConnection(connectionString))
                                {
                                    connection.Open();

                                    string query = "SELECT * FROM Isolates";
                                    using (SqliteDataAdapter dataAdapter = new SqliteDataAdapter())
                                    {
                                        dataAdapter.SelectCommand = new SqliteCommand(query, connection);
                                        dataAdapter.Fill(result);
                                    }
                                }

                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Read File ... ");
                                var log_tran1 = new LogWriter("End Read File ...");

                                if (result.Tables[0].Columns.Contains(DATE) == true)
                                {
                                    FieldDateType = result.Tables[0].Columns[DATE].DataType.ToString();
                                }


                                //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
                                //>> for Etest : Antibiotic Isolate more than 1 Record
                                if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
                                {
                                    //var MappingAnt = whonetmapping.Where(x => x.wnm_antibiotic != null);
                                    var MappingAnt = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NE")).ToList();
                                    // Antibioticcolumn = MappingAnt.FirstOrDefault().wnm_antibioticcolumn;

                                    if (MappingTemplate.mp_firstlineisheader == false)
                                    {
                                        AntiLocalCodeColumnEtest = convertColumn(AntiLocalCodeColumnEtest);
                                    }

                                    //var listexcludeAntibiotic = new List<string>();

                                    foreach (WHONetMappingListsDTO t in MappingAnt)
                                    {
                                        //listexcludeAntibiotic.Add(t.wnm_antibiotic);
                                        listexcludeAntibiotic.Add(t.wnm_originalfield);
                                    }
                                }
                                else
                                {
                                    if (MappingTemplate.mp_AntibioticIsolateOneRec == false) //Other type but Antibiotic in many rows (Automate)
                                    {
                                        var MappingAnt = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NM")).ToList();
                                        AntiLocalCodeColumnEtest = (from s in whonetmapping
                                                                    where !string.IsNullOrEmpty(s.wnm_antibioticcolumn)
                                                                    select s.wnm_antibioticcolumn).Distinct().FirstOrDefault();
                                        if (MappingTemplate.mp_firstlineisheader == false)
                                        {
                                            AntiLocalCodeColumnEtest = convertColumn(AntiLocalCodeColumnEtest);
                                        }
                                        foreach (WHONetMappingListsDTO t in MappingAnt)
                                        {

                                            listexcludeAntibiotic.Add(t.wnm_antibiotic);
                                        }
                                    }
                                }

                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Header Data ... ");
                                var log_read = new LogWriter("Start Header Data ...");
                                dataRow = 1;

                                if (listexcludeAntibiotic.Count > 0)
                                {
                                    AntNotMatch = result.Tables[0].AsEnumerable().Where(row => !listexcludeAntibiotic.Contains(row.Field<string>(AntiLocalCodeColumnEtest)));
                                    ErrorAnt = AntNotMatch.Count();
                                    var AntNotMatchDistinct = AntNotMatch.AsEnumerable().Select(s => new { antibiotic = s.Field<string>(AntiLocalCodeColumnEtest), }).Distinct().ToList();
                                    foreach (var lst in AntNotMatchDistinct)
                                    {
                                        lstAntErrDistinct.Add(lst.antibiotic);
                                    }
                                }

                                //Loop Read Data Row
                                foreach (DataRow row in result.Tables[0].Rows)
                                {
                                    string cmethod = "";
                                    blnRowError = false;

                                    if (result.Tables[0].Columns.Contains("CMETHOD"))
                                    {
                                        cmethod = row["CMETHOD"].ToString();
                                    }

                                    if ((cmethod == "Aerobic Culture" && selectedLab.lfu_Program == "MLAB")
                                   || selectedLab.lfu_Program != "MLAB"
                                   || (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST"))
                                    {

                                        #region InsertLabFileDataHeader
                                        TRSTGLabFileDataHeader objModel = new TRSTGLabFileDataHeader();
                                        Guid ldh_id = Guid.NewGuid();

                                        // key is null, continue
                                        if (!string.IsNullOrEmpty(LAB_NO) && string.IsNullOrEmpty(row[LAB_NO].ToString()))
                                        {
                                            continue;
                                        }
                                        else if (!string.IsNullOrEmpty(ORGANISM) && string.IsNullOrEmpty(row[ORGANISM].ToString()))
                                        {
                                            continue;
                                        }
                                        else if (!string.IsNullOrEmpty(DATE) && string.IsNullOrEmpty(row[DATE].ToString()))
                                        {
                                            continue;
                                        }

                                        if (selectedLab.lfu_FileType != "ETEST")
                                        {
                                            if (!string.IsNullOrEmpty(SOURCE) && string.IsNullOrEmpty(row[SOURCE].ToString()))
                                            {
                                                continue;
                                            }
                                        }


                                        // -- Check Date Format
                                        objModel.ldh_date = row[DATE].ToString();
                                        if (FieldDateType == "System.String")
                                        {
                                            try
                                            {
                                                //objModel.ldh_cdate = DateTime.ParseExact(objModel.ldh_date, DateFormat, CultureInfo.GetCultureInfo("en-US"));
                                                objModel.ldh_cdate = CovertStringToDate(objModel.ldh_date, DateFormat);
                                                int year = objModel.ldh_cdate.Value.Year;

                                                if (year < 1000)
                                                {
                                                    if (feh_id_cdate == Guid.Empty)
                                                    {
                                                        feh_id_cdate = Guid.NewGuid();
                                                    }
                                                    else
                                                    {
                                                        feh_status_cdate = 'E';
                                                    }


                                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                    objErrorH.feh_id = feh_id_cdate;
                                                    objErrorH.feh_status = feh_status_cdate;
                                                    objErrorH.feh_flagdelete = false;
                                                    objErrorH.feh_type = "CONVERT_ERROR";
                                                    objErrorH.feh_field = DATE;
                                                    objErrorH.feh_message = "Cannot convert date.";
                                                    cSpecDateError += 1;
                                                    objErrorH.feh_errorrecord = cSpecDateError;
                                                    objErrorH.feh_createuser = "BATCH";
                                                    objErrorH.feh_createdate = DateTime.Now;
                                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                    blnRowError = true;
                                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);




                                                    continue;
                                                }

                                                if (year > DateTime.Now.Year)
                                                {
                                                    objModel.ldh_cdate = objModel.ldh_cdate.Value.AddYears(-543);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                //chkError++;

                                                if (feh_id_cdate == Guid.Empty)
                                                {
                                                    feh_id_cdate = Guid.NewGuid();
                                                }
                                                else
                                                {
                                                    feh_status_cdate = 'E';
                                                }


                                                TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                objErrorH.feh_id = feh_id_cdate;
                                                objErrorH.feh_status = feh_status_cdate;
                                                objErrorH.feh_flagdelete = false;
                                                objErrorH.feh_type = "CONVERT_ERROR";
                                                objErrorH.feh_field = DATE;
                                                objErrorH.feh_message = "Cannot convert date.";
                                                cSpecDateError += 1;
                                                objErrorH.feh_errorrecord = cSpecDateError;
                                                objErrorH.feh_createuser = "BATCH";
                                                objErrorH.feh_createdate = DateTime.Now;
                                                objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                blnRowError = true;
                                                var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);


                                                TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
                                                objErrorD.fed_id = Guid.NewGuid();
                                                objErrorD.fed_status = 'N';
                                                objErrorD.fed_localvalue = ex.Message;
                                                objErrorD.fed_feh_id = feh_id_cdate;
                                                objErrorD.fed_createuser = "BATCH";
                                                objErrorD.fed_createdate = DateTime.Now;

                                                var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
                                                continue;
                                            }
                                        }
                                        else if (FieldDateType == "System.DateTime")
                                        {
                                            objModel.ldh_cdate = (DateTime)row[DATE];
                                            int year = objModel.ldh_cdate.Value.Year;
                                            if (year > DateTime.Now.Year)
                                            {
                                                objModel.ldh_cdate = objModel.ldh_cdate.Value.AddYears(-543);
                                            }
                                        }
                                        else if (FieldDateType == "System.Object") //-- am add 18/01/2021 --
                                        {
                                            if (row[DATE].GetType().FullName == "System.DateTime")
                                            {
                                                objModel.ldh_cdate = (DateTime)row[DATE];
                                                int year = objModel.ldh_cdate.Value.Year;
                                                if (year > DateTime.Now.Year)
                                                {
                                                    objModel.ldh_cdate = objModel.ldh_cdate.Value.AddYears(-543);
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    objModel.ldh_cdate = CovertStringToDate(objModel.ldh_date, DateFormat);
                                                    //objModel.ldh_cdate = DateTime.ParseExact(objModel.ldh_date, DateFormat, CultureInfo.GetCultureInfo("en-US"));
                                                    int year = objModel.ldh_cdate.Value.Year;

                                                    if (year < 1000)
                                                    {
                                                        if (feh_id_cdate == Guid.Empty)
                                                        {
                                                            feh_id_cdate = Guid.NewGuid();
                                                        }
                                                        else
                                                        {
                                                            feh_status_cdate = 'E';
                                                        }


                                                        TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                        objErrorH.feh_id = feh_id_cdate;
                                                        objErrorH.feh_status = feh_status_cdate;
                                                        objErrorH.feh_flagdelete = false;
                                                        objErrorH.feh_type = "CONVERT_ERROR";
                                                        objErrorH.feh_field = DATE;
                                                        objErrorH.feh_message = "Cannot convert date.";
                                                        cSpecDateError += 1;
                                                        objErrorH.feh_errorrecord = cSpecDateError;
                                                        objErrorH.feh_createuser = "BATCH";
                                                        objErrorH.feh_createdate = DateTime.Now;
                                                        objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                        blnRowError = true;


                                                        var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);
                                                    }

                                                    if (year > DateTime.Now.Year)
                                                    {
                                                        objModel.ldh_cdate = objModel.ldh_cdate.Value.AddYears(-543);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    //chkError++;

                                                    if (feh_id_cdate == Guid.Empty)
                                                    {
                                                        feh_id_cdate = Guid.NewGuid();
                                                    }
                                                    else
                                                    {
                                                        feh_status_cdate = 'E';
                                                    }


                                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                    objErrorH.feh_id = feh_id_cdate;
                                                    objErrorH.feh_status = feh_status_cdate;
                                                    objErrorH.feh_flagdelete = false;
                                                    objErrorH.feh_type = "CONVERT_ERROR";
                                                    objErrorH.feh_field = DATE;
                                                    objErrorH.feh_message = "Cannot convert date.";
                                                    cSpecDateError += 1;
                                                    objErrorH.feh_errorrecord = cSpecDateError;
                                                    objErrorH.feh_createuser = "BATCH";
                                                    objErrorH.feh_createdate = DateTime.Now;
                                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                    blnRowError = true;


                                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

                                                    TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
                                                    objErrorD.fed_id = Guid.NewGuid();
                                                    objErrorD.fed_status = 'N';
                                                    objErrorD.fed_localvalue = ex.Message;
                                                    objErrorD.fed_feh_id = objTRLabFileErrorHeader;
                                                    objErrorD.fed_createuser = "BATCH";
                                                    objErrorD.fed_createdate = DateTime.Now;

                                                    var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
                                                    continue;
                                                }
                                            }
                                        }

                                        // Etest and Data not this year : continue
                                        if (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST" && objModel.ldh_cdate.Value.Year != selectedLab.lfu_DataYear)
                                        {
                                            continue;
                                        }

                                        // -- End Check Date Format

                                        // -- Check Antibiotic                                     
                                        if (lstAntErrDistinct.Count > 0)
                                        {
                                            var anti = row[AntiLocalCodeColumnEtest].ToString();
                                            if (lstAntErrDistinct.Contains(anti))
                                            {
                                                cntErrAntiCurrentYear += 1;
                                                blnRowError = true;
                                                if (!lstAntErrirCurrentYear.Contains(anti))
                                                {
                                                    lstAntErrirCurrentYear.Add(anti);
                                                }
                                                continue;
                                            }
                                        }
                                        // -- End Check Antibiotic

                                        objModel.ldh_id = ldh_id;
                                        objModel.ldh_status = 'N';
                                        objModel.ldh_flagdelete = false;
                                        objModel.ldh_hos_code = selectedLab.lfu_hos_code;
                                        objModel.ldh_lab = selectedLab.lfu_lab;
                                        objModel.ldh_lfu_id = selectedLab.lfu_id;

                                        objModel.ldh_labno = row[LAB_NO].ToString();
                                        objModel.ldh_organism = row[ORGANISM].ToString();

                                        if (selectedLab.lfu_FileType != "ETEST")
                                        {
                                            if (SOURCE != "")
                                            {
                                                objModel.ldh_specimen = row[SOURCE].ToString();
                                            }
                                        }


                                        objModel.ldh_createuser = "BATCH";

                                        int seq = LabDataDAL.Get_CheckExistingHeader(objModel, MappingTemplate.mp_program, selectedLab.lfu_FileType);
                                        objModel.ldh_sequence = seq;
                                        objModel.ldh_createdate = DateTime.Now;
                                        Guid idh_id_related = Guid.Empty;

                                        bool isNotFirstline = false;
                                        //ETEST
                                        //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
                                        if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST || (MappingTemplate.mp_AntibioticIsolateOneRec == false))
                                        {
                                            //idh_id_related = LabDataDAL.Get_HeaderIdForMultipleline(objModel);
                                            TRSTGLabFileDataHeader dup = new TRSTGLabFileDataHeader();
                                            if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
                                            {
                                                dup = LabFileDataHeaderList.FirstOrDefault(x => x.ldh_lfu_id == objModel.ldh_lfu_id
                                                                          && x.ldh_hos_code == objModel.ldh_hos_code
                                                                          && x.ldh_lab == objModel.ldh_lab
                                                                          && x.ldh_labno == objModel.ldh_labno
                                                                          && x.ldh_organism == objModel.ldh_organism
                                                                          //&& x.ldh_specimen == objModel.ldh_specimen
                                                                          && x.ldh_date == objModel.ldh_date);
                                            }
                                            else
                                            {
                                                dup = LabFileDataHeaderList.FirstOrDefault(x => x.ldh_lfu_id == objModel.ldh_lfu_id
                                                                        && x.ldh_hos_code == objModel.ldh_hos_code
                                                                        && x.ldh_lab == objModel.ldh_lab
                                                                        && x.ldh_labno == objModel.ldh_labno
                                                                        && x.ldh_organism == objModel.ldh_organism
                                                                        && x.ldh_specimen == objModel.ldh_specimen
                                                                        && x.ldh_date == objModel.ldh_date);
                                            }

                                            if (dup != null)
                                            {
                                                idh_id_related = dup.ldh_id;
                                            }

                                            if (idh_id_related != Guid.Empty)
                                            {
                                                isNotFirstline = true;
                                                ldh_id = idh_id_related;
                                            }
                                            else
                                            {
                                                //var x = LabDataDAL.Save_LabFileDataHeader(objModel);
                                                LabFileDataHeaderList.Add(objModel);

                                            }
                                        }
                                        else
                                        {
                                            //var x = LabDataDAL.Save_LabFileDataHeader(objModel);
                                            LabFileDataHeaderList.Add(objModel);
                                        }


                                        #endregion

                                        #region InsertLabFileDataDetail

                                        if (whonetmapping != null)
                                        {
                                            foreach (WHONetMappingListsDTO item in whonetmapping)
                                            {
                                                String wnm_originalfield;
                                                var Encoding = "";

                                                if (MappingTemplate.mp_firstlineisheader == false)
                                                {

                                                    wnm_originalfield = convertColumn(item.wnm_originalfield.Trim());
                                                }
                                                else
                                                {
                                                    wnm_originalfield = item.wnm_originalfield.Trim();
                                                }

                                                if (isNotFirstline == true && selectedLab.lfu_FileType == "ETEST" && (wnm_originalfield == LAB_NO || wnm_originalfield == DATE || wnm_originalfield == ORGANISM))
                                                {
                                                    continue;
                                                }
                                                if (isNotFirstline == true && selectedLab.lfu_FileType != "ETEST" && (MappingTemplate.mp_AntibioticIsolateOneRec == false)
                                                    && (wnm_originalfield == LAB_NO || wnm_originalfield == DATE || wnm_originalfield == ORGANISM || wnm_originalfield == SOURCE || wnm_originalfield == WARD_TYPE))
                                                {
                                                    continue;
                                                }


                                                if (selectedLab.lfu_FileType == "ETEST" || (MappingTemplate.mp_AntibioticIsolateOneRec == false))
                                                {
                                                    if (selectedLab.lfu_FileType == "ETEST")
                                                    {
                                                        if (item.wnm_whonetfield.Contains("_NE"))
                                                        {
                                                            wnm_originalfield = AntiValueColumnEtest; //MICIT_TROU
                                                            if (!result.Tables[0].Columns.Contains(wnm_originalfield))
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (!result.Tables[0].Columns.Contains(wnm_originalfield.Trim()))
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                    }
                                                    else //MappingTemplate.mp_AntibioticIsolateOneRec == false
                                                    {
                                                        if (item.wnm_whonetfield.Contains("_NM"))
                                                        {
                                                            AntiValueColumnEtest = (from s in whonetmapping
                                                                                    where s.wnm_whonetfield.Contains("_NM")
                                                                                    select s.wnm_originalfield).Distinct().FirstOrDefault(); //Column20

                                                            if (MappingTemplate.mp_firstlineisheader == false)
                                                            {
                                                                AntiValueColumnEtest = convertColumn(AntiValueColumnEtest);
                                                            }
                                                            wnm_originalfield = AntiValueColumnEtest;
                                                            if (!result.Tables[0].Columns.Contains(wnm_originalfield))
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (!result.Tables[0].Columns.Contains(wnm_originalfield.Trim()))
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    if (!result.Tables[0].Columns.Contains(wnm_originalfield))
                                                    {
                                                        continue;
                                                    }
                                                }
                                                // ETEST
                                                //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
                                                if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST || (MappingTemplate.mp_AntibioticIsolateOneRec == false))
                                                {
                                                    if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
                                                    {
                                                        if (AntiLocalCodeColumnEtest != "")
                                                        {
                                                            //var antibioticcolumn = "";
                                                            if (MappingTemplate.mp_firstlineisheader == false)
                                                            {
                                                                AntiLocalCodeColumnEtest = convertColumn(AntiLocalCodeColumnEtest);
                                                            }

                                                            if (row.IsNull(AntiLocalCodeColumnEtest))
                                                            {
                                                                continue;
                                                            }
                                                            if (item.wnm_whonetfield.Contains("_NE"))
                                                            {
                                                                if (row[AntiLocalCodeColumnEtest].ToString() != item.wnm_originalfield.Trim())
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else //MappingTemplate.mp_AntibioticIsolateOneRec == false
                                                    {
                                                        if (AntiLocalCodeColumnEtest != "") //Column20
                                                        {
                                                            //var antibioticcolumn = "";
                                                            if (MappingTemplate.mp_firstlineisheader == false)
                                                            {
                                                                //AntiLocalCodeColumnEtest = convertColumn(AntiLocalCodeColumnEtest);
                                                            }

                                                            if (row.IsNull(AntiLocalCodeColumnEtest))
                                                            {
                                                                continue;
                                                            }
                                                            if (item.wnm_whonetfield.Contains("_NM"))
                                                            {
                                                                if (row[AntiLocalCodeColumnEtest].ToString() != item.wnm_antibiotic.Trim())
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                        }
                                                    }

                                                }

                                                if (row.IsNull(wnm_originalfield))
                                                {
                                                    continue;
                                                }

                                                string tempvalue = "";
                                                var xx = row[wnm_originalfield].GetType().ToString();
                                                if (row[wnm_originalfield].GetType().ToString() != "System.DateTime" && item.wnm_type == "Date")
                                                {
                                                    try
                                                    {
                                                        //var tempfielddate = DateTime.ParseExact(row[wnm_originalfield].ToString(), item.wnm_fieldformat, CultureInfo.GetCultureInfo("en-US"));
                                                        var tempfielddate = CovertStringToDate(row[wnm_originalfield].ToString(), item.wnm_fieldformat);
                                                        tempvalue = tempfielddate.ToString();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        chkDetailError++;

                                                        if (feh_id_cdate == Guid.Empty)
                                                        {
                                                            feh_id_cdate = Guid.NewGuid();
                                                        }
                                                        else
                                                        {
                                                            feh_status_cdate = 'E';
                                                        }
                                                        TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                        objErrorH.feh_id = feh_id_cdate;
                                                        objErrorH.feh_status = feh_status_cdate;
                                                        objErrorH.feh_flagdelete = false;
                                                        objErrorH.feh_type = "CONVERT_ERROR";
                                                        objErrorH.feh_field = item.wnm_originalfield.Trim();
                                                        objErrorH.feh_message = "Cannot convert date.";
                                                        cDateError = cDateError + 1;
                                                        objErrorH.feh_errorrecord = cDateError;
                                                        objErrorH.feh_createuser = "BATCH";
                                                        objErrorH.feh_createdate = DateTime.Now;
                                                        objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                        blnRowError = true;
                                                        var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

                                                        TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
                                                        objErrorD.fed_id = Guid.NewGuid();
                                                        objErrorD.fed_status = 'N';
                                                        objErrorD.fed_localvalue = ex.Message;
                                                        objErrorD.fed_feh_id = feh_id_cdate;
                                                        objErrorD.fed_createuser = "BATCH";
                                                        objErrorD.fed_createdate = DateTime.Now;

                                                        var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
                                                        continue;
                                                    }
                                                }
                                                else if (row[wnm_originalfield].GetType().ToString() == "System.DateTime" && item.wnm_type == "Date")
                                                {
                                                    var tempfielddate = (DateTime)row[wnm_originalfield];
                                                    tempvalue = tempfielddate.ToString();
                                                }
                                                else
                                                {
                                                    if ((!string.IsNullOrEmpty(selectedLab.lfu_FileTypeLabel)) && item.wnm_whonetfield.Contains("_NM"))
                                                    {
                                                        if (selectedLab.lfu_FileTypeLabel == clsLabFileType.MLAB_FileType.MIC_SIR)
                                                        {
                                                            if (row[wnm_originalfield].ToString().ToUpper() == "S" ||
                                                                 row[wnm_originalfield].ToString().ToUpper() == "I" ||
                                                                 row[wnm_originalfield].ToString().ToUpper() == "R")
                                                            {
                                                                //MIC-SIR
                                                                tempvalue = row[wnm_originalfield].ToString();
                                                            }
                                                            else
                                                            {
                                                                // do nothing
                                                            }
                                                        }
                                                        else if (selectedLab.lfu_FileTypeLabel == clsLabFileType.MLAB_FileType.MIC_NUM)
                                                        {
                                                            if (row[wnm_originalfield].ToString() == "S" ||
                                                                 row[wnm_originalfield].ToString() == "I" ||
                                                                 row[wnm_originalfield].ToString() == "R")
                                                            {
                                                                // do nothing
                                                            }
                                                            else
                                                            {
                                                                //MIC-NUM
                                                                tempvalue = row[wnm_originalfield].ToString();

                                                            }
                                                        }
                                                        else
                                                        {
                                                            tempvalue = row[wnm_originalfield].ToString();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        tempvalue = row[wnm_originalfield].ToString();
                                                    }
                                                    goto AddDetailPoint;
                                                }

                                                if ((selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST && item.wnm_whonetfield.Contains("_NE"))
                                                    || ((MappingTemplate.mp_AntibioticIsolateOneRec == false) && item.wnm_whonetfield.Contains("_NM"))
                                                    )
                                                {
                                                    string antibiotic = row[AntiLocalCodeColumnEtest].ToString();

                                                    if (antibiotic == item.wnm_originalfield.Trim())
                                                    {
                                                        tempvalue = row[AntiValueColumnEtest].ToString();
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    if (result.Tables[0].Columns.Contains(wnm_originalfield.Trim()))
                                                    {
                                                        tempvalue = row[wnm_originalfield.Trim()].ToString();
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }

                                            AddDetailPoint:
                                                if (item.wnm_encrypt == true)
                                                {
                                                    Encoding = CryptoHelper.UnicodeEncoding(tempvalue);
                                                }
                                                else
                                                {
                                                    Encoding = tempvalue;
                                                }

                                                LabFileDataDetailList.Add(new TRSTGLabFileDataDetail
                                                {
                                                    ldd_id = Guid.NewGuid(),
                                                    ldd_status = 'N',
                                                    ldd_whonetfield = item.wnm_whonetfield,
                                                    ldd_originalfield = item.wnm_originalfield.Trim(),
                                                    ldd_originalvalue = Encoding,
                                                    ldd_ldh_id = ldh_id,
                                                    ldd_createuser = "BATCH",
                                                    ldd_createdate = DateTime.Now
                                                }
                                                );

                                            }

                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    if (!blnRowError && WARD_TYPE != "" && (result.Tables[0].Columns.Contains(WARD_TYPE)))
                                    {
                                        var ward = row[WARD_TYPE].ToString().ToUpper();
                                        if (ward == "" || !lstWardType.Contains(ward.Trim())) { blnRowError = true; }
                                    }

                                    if (selectedLab.lfu_FileType != "ETEST")
                                    {
                                        if (!blnRowError && SOURCE != "" && (result.Tables[0].Columns.Contains(SOURCE)))
                                        {
                                            var localspecimen = row[SOURCE].ToString().ToUpper();
                                            if (localspecimen == "" || !lstSpecimen.Contains(localspecimen.Trim())) { blnRowError = true; }
                                        }
                                    }


                                    if (!blnRowError && ORGANISM != "" && (result.Tables[0].Columns.Contains(ORGANISM)))
                                    {
                                        var localorganism = row[ORGANISM].ToString().ToUpper();
                                        if (localorganism == "" || !lstOrganism.Contains(localorganism.Trim())) { blnRowError = true; }
                                    }


                                    if (blnRowError) { nRowError += 1; }
                                    dataRow += 1;
                                }// end loop read data row

                                //Save Antibiotic Error
                                if (lstAntErrirCurrentYear.Count > 0)
                                {
                                    nRowError = cntErrAntiCurrentYear;
                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                    objErrorH.feh_id = Guid.NewGuid();
                                    objErrorH.feh_status = 'N';
                                    objErrorH.feh_flagdelete = false;
                                    objErrorH.feh_type = "NOT_MATCH";
                                    objErrorH.feh_field = "Antibiotic";
                                    objErrorH.feh_message = "Antibiotic not match";
                                    objErrorH.feh_errorrecord = cntErrAntiCurrentYear;
                                    objErrorH.feh_createuser = "BATCH";
                                    objErrorH.feh_createdate = DateTime.Now;
                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                    blnRowError = true;
                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

                                    foreach (var item in lstAntErrirCurrentYear)
                                    {
                                        TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
                                        objErrorD.fed_id = Guid.NewGuid();
                                        objErrorD.fed_status = 'N';
                                        objErrorD.fed_localvalue = item;
                                        objErrorD.fed_feh_id = objErrorH.feh_id;
                                        objErrorD.fed_createuser = "BATCH";
                                        objErrorD.fed_createdate = DateTime.Now;

                                        var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
                                    }
                                }

                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Header Data ... ");
                                var log_endr = new LogWriter("End Header Data ...");

                                if (LabFileDataHeaderList.Count() != 0)
                                {
                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Save Header List ... ");
                                    var log_std = new LogWriter("Start Save Header List ...");

                                    LabDataDAL.InsertBulk_LabFileDataHeader(LabFileDataHeaderList);

                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Header List... ");
                                    var log_endd = new LogWriter("End Save Header List ...");
                                }


                                if (LabFileDataDetailList.Count != 0)
                                {
                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Save Detail ... ");
                                    var log_std = new LogWriter("Start Save Detail ...");

                                    //var ldd = LabDataDAL.Save_LabFileDataDetail(LabFileDataDetailList);
                                    var ldd = Export_LabFileDataDetail(labdetail_path, LabFileDataDetailList, selectedLab);
                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Detail ... ");
                                    var log_endd = new LogWriter("End Save Detail ...");
                                }
                            //}
                        }
                        #endregion
                        //DBF + .HOS
                        else
                        {
                            var options = new DbfDataReaderOptions
                            {
                                Encoding = Encoding.GetEncoding(874)
                            };

                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Read File (.DBF) ... ");
                            var log_tran = new LogWriter("Read File (.DBF) ...");

                            using (var dbfDataReader = new DbfDataReader.DbfDataReader(selectedLab.lfu_Path, options))
                            {
                                var x = dbfDataReader.DbfTable.Header.RecordCount;

                                string LAB_NO = whonetmapping.FirstOrDefault(v => v.wnm_whonetfield == Param_labno).wnm_originalfield;
                                string ORGANISM = whonetmapping.FirstOrDefault(v => v.wnm_whonetfield == Param_organism).wnm_originalfield;
                                string DATE = whonetmapping.FirstOrDefault(v => v.wnm_whonetfield == Param_date).wnm_originalfield;

                                string WARD_TYPE = "";
                                if (whonetmapping.FirstOrDefault(w => w.wnm_whonetfield == Param_wardtype) != null)
                                    WARD_TYPE = whonetmapping.FirstOrDefault(w => w.wnm_whonetfield == Param_wardtype).wnm_originalfield;

                                string SOURCE = "";
                                if (whonetmapping.FirstOrDefault(v => v.wnm_whonetfield == Param_specimen) != null)
                                    SOURCE = whonetmapping.FirstOrDefault(v => v.wnm_whonetfield == Param_specimen).wnm_originalfield;

                                if (MappingTemplate.mp_firstlineisheader == false)
                                {
                                    LAB_NO = convertColumn(LAB_NO);
                                    ORGANISM = convertColumn(ORGANISM);

                                    if (SOURCE != "")
                                        SOURCE = convertColumn(SOURCE);

                                    DATE = convertColumn(DATE);
                                }

                                List<TRSTGLabFileDataHeader> LabFileDataHeaderList = new List<TRSTGLabFileDataHeader>();
                                List<TRSTGLabFileDataDetail> LabFileDataDetailList = new List<TRSTGLabFileDataDetail>();

                                //string Antibioticcolumn = "";
                                List<WHONetMappingListsDTO> MappingAnt = new List<WHONetMappingListsDTO>();
                                //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
                                if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
                                {
                                    MappingAnt = whonetmapping.Where(v => v.wnm_whonetfield.Contains("_NE")).ToList();
                                }
                                else
                                {
                                    if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
                                    {
                                        MappingAnt = whonetmapping.Where(v => v.wnm_whonetfield.Contains("_NM")).ToList();
                                    }
                                }

                                List<TRLabFileErrorDetail> AntibioticErrorD = new List<TRLabFileErrorDetail>();

                                ErrorAnt = 0;
                                dataRow = 0;
                                //int test = 0;
                                while (dbfDataReader.Read())
                                {
                                    blnRowError = false;
                                    //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
                                    if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
                                    {
                                        if (dbfDataReader[AntiLocalCodeColumnEtest] == "" || dbfDataReader[AntiLocalCodeColumnEtest] == null)
                                        {
                                            continue;
                                        }
                                        string antibiotic = dbfDataReader[AntiLocalCodeColumnEtest].ToString();
                                        if (!MappingAnt.Any(v => v.wnm_originalfield == antibiotic))
                                        {
                                            if (!AntibioticErrorD.Any(v => v.fed_localvalue == antibiotic))
                                            {
                                                if (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST"
                                                    && dbfDataReader.GetDateTime(DATE).Year == selectedLab.lfu_DataYear)
                                                {
                                                    AntibioticErrorD.Add(new TRLabFileErrorDetail
                                                    {
                                                        fed_id = Guid.NewGuid(),
                                                        fed_status = 'N',
                                                        fed_localvalue = antibiotic,
                                                        fed_createuser = "BATCH",
                                                        fed_createdate = DateTime.Now
                                                    });
                                                    ErrorAnt++;
                                                    blnRowError = true;
                                                }

                                            }
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (MappingTemplate.mp_AntibioticIsolateOneRec == false) //Automate
                                        {
                                            AntiLocalCodeColumnEtest = (from s in whonetmapping
                                                                        where !string.IsNullOrEmpty(s.wnm_antibioticcolumn)
                                                                        select s.wnm_antibioticcolumn).Distinct().FirstOrDefault();

                                            if(MappingTemplate.mp_firstlineisheader == false)
                                            {
                                                AntiLocalCodeColumnEtest = convertColumn(AntiLocalCodeColumnEtest);
                                            }

                                            if (dbfDataReader[AntiLocalCodeColumnEtest] == "" || dbfDataReader[AntiLocalCodeColumnEtest] == null)
                                            {
                                                continue;
                                            }

                                            string antibiotic = dbfDataReader[AntiLocalCodeColumnEtest].ToString();
                                            if (!MappingAnt.Any(v => v.wnm_originalfield == antibiotic))
                                            {
                                                if (!AntibioticErrorD.Any(v => v.fed_localvalue == antibiotic))
                                                {
                                                    if (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST"
                                                        && dbfDataReader.GetDateTime(DATE).Year == selectedLab.lfu_DataYear)
                                                    {
                                                        AntibioticErrorD.Add(new TRLabFileErrorDetail
                                                        {
                                                            fed_id = Guid.NewGuid(),
                                                            fed_status = 'N',
                                                            fed_localvalue = antibiotic,
                                                            fed_createuser = "BATCH",
                                                            fed_createdate = DateTime.Now
                                                        });
                                                        ErrorAnt++;
                                                        blnRowError = true;
                                                    }

                                                }
                                                continue;
                                            }
                                        }
                                    }

                                    string cmethod = "";
                                    if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == "CMETHOD") != null)
                                        cmethod = dbfDataReader.GetString("CMETHOD");

                                    if ((cmethod == "Aerobic Culture" && selectedLab.lfu_Program == "MLAB")
                                    || selectedLab.lfu_Program != "MLAB"
                                    || (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST"))
                                    {
                                        #region InsertLabFileDataHeader

                                        if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == LAB_NO) != null)
                                        {
                                            if (string.IsNullOrEmpty(dbfDataReader[LAB_NO].ToString())) { continue; }
                                        }
                                        if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == ORGANISM) != null)
                                        {
                                            if (string.IsNullOrEmpty(dbfDataReader[ORGANISM].ToString())) { continue; }
                                        }
                                        if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == DATE) != null)
                                        {
                                            try
                                            {
                                                if (string.IsNullOrEmpty(dbfDataReader.GetDateTime(DATE).ToString()))
                                                {
                                                    continue;
                                                }
                                            }
                                            catch
                                            {
                                                continue;
                                            }

                                        }
                                        if (selectedLab.lfu_FileType != "ETEST")
                                        {
                                            if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == SOURCE) != null)
                                            {
                                                if (string.IsNullOrEmpty(dbfDataReader[SOURCE].ToString())) { continue; }
                                            }
                                        }



                                        TRSTGLabFileDataHeader objModel = new TRSTGLabFileDataHeader();
                                        Guid ldh_id = Guid.NewGuid();

                                        //-- check date
                                        objModel.ldh_date = dbfDataReader.GetDateTime(DATE).ToString();
                                        try
                                        {
                                            objModel.ldh_cdate = dbfDataReader.GetDateTime(DATE);
                                        }
                                        catch (Exception ex)
                                        {
                                            var dd = ex.Message;
                                        }

                                        if (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST" && objModel.ldh_cdate.Value.Year != selectedLab.lfu_DataYear)
                                        {
                                            continue;
                                        }
                                       
                                        //-- end check date

                                        objModel.ldh_id = ldh_id;
                                        objModel.ldh_status = 'N';
                                        objModel.ldh_flagdelete = false;
                                        objModel.ldh_hos_code = selectedLab.lfu_hos_code;
                                        objModel.ldh_lab = selectedLab.lfu_lab;
                                        objModel.ldh_lfu_id = selectedLab.lfu_id;

                                        objModel.ldh_labno = dbfDataReader[LAB_NO].ToString();
                                        objModel.ldh_organism = dbfDataReader.GetString(ORGANISM);

                                        if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == SOURCE) != null)
                                            objModel.ldh_specimen = dbfDataReader.GetString(SOURCE);


                                        objModel.ldh_createuser = "BATCH";

                                        int seq = LabDataDAL.Get_CheckExistingHeader(objModel, MappingTemplate.mp_program, selectedLab.lfu_FileType);
                                        objModel.ldh_sequence = seq;
                                        objModel.ldh_createdate = DateTime.Now;

                                        Guid idh_id_related = Guid.Empty;
                                        bool isNotFirstline = false;

                                        //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
                                        if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST || (MappingTemplate.mp_AntibioticIsolateOneRec == false))
                                        {
                                            //idh_id_related = LabDataDAL.Get_HeaderIdForMultipleline(objModel);
                                            TRSTGLabFileDataHeader dup = new TRSTGLabFileDataHeader();
                                            if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
                                            {
                                                dup = LabFileDataHeaderList.FirstOrDefault(x => x.ldh_lfu_id == objModel.ldh_lfu_id
                                                                          && x.ldh_hos_code == objModel.ldh_hos_code
                                                                          && x.ldh_lab == objModel.ldh_lab
                                                                          && x.ldh_labno == objModel.ldh_labno
                                                                          && x.ldh_organism == objModel.ldh_organism
                                                                          //&& x.ldh_specimen == objModel.ldh_specimen
                                                                          && x.ldh_date == objModel.ldh_date);
                                            }
                                            else
                                            {
                                                dup = LabFileDataHeaderList.FirstOrDefault(x => x.ldh_lfu_id == objModel.ldh_lfu_id
                                                                        && x.ldh_hos_code == objModel.ldh_hos_code
                                                                        && x.ldh_lab == objModel.ldh_lab
                                                                        && x.ldh_labno == objModel.ldh_labno
                                                                        && x.ldh_organism == objModel.ldh_organism
                                                                        && x.ldh_specimen == objModel.ldh_specimen
                                                                        && x.ldh_date == objModel.ldh_date);
                                            }
                                            if (dup != null)
                                            {
                                                idh_id_related = dup.ldh_id;
                                            }

                                            if (idh_id_related != Guid.Empty)
                                            {
                                                isNotFirstline = true;
                                                ldh_id = idh_id_related;
                                            }
                                            else
                                            {
                                                //var c = LabDataDAL.Save_LabFileDataHeader(objModel);                                              
                                                LabFileDataHeaderList.Add(objModel);
                                            }
                                        }
                                        else
                                        {
                                            //var c = LabDataDAL.Save_LabFileDataHeader(objModel);
                                            LabFileDataHeaderList.Add(objModel);
                                        }

                                        //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Lab Header ... ");
                                        #endregion

                                        #region InsertLabFileDataDetail
                                        //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start loop Detail ... ");
                                        try
                                        {
                                            if (whonetmapping != null)
                                            {
                                                foreach (WHONetMappingListsDTO item in whonetmapping)
                                                {
                                                    String wnm_originalfield;

                                                    if (MappingTemplate.mp_firstlineisheader == false)
                                                    {
                                                        wnm_originalfield = convertColumn(item.wnm_originalfield.Trim());
                                                    }
                                                    else
                                                    {
                                                        wnm_originalfield = item.wnm_originalfield.Trim();
                                                    }

                                                    if (isNotFirstline == true && selectedLab.lfu_FileType == "ETEST" && (wnm_originalfield == LAB_NO || wnm_originalfield == DATE || wnm_originalfield == ORGANISM))
                                                    {
                                                        continue;
                                                    }
                                                    if (isNotFirstline == true && selectedLab.lfu_FileType != "ETEST"  && (MappingTemplate.mp_AntibioticIsolateOneRec == false) 
                                                        && (wnm_originalfield == LAB_NO || wnm_originalfield == DATE || wnm_originalfield == ORGANISM || wnm_originalfield == SOURCE || wnm_originalfield == WARD_TYPE))
                                                    {
                                                        continue;
                                                    }
                                                    if (selectedLab.lfu_FileType == "ETEST" || (MappingTemplate.mp_AntibioticIsolateOneRec == false))
                                                    {
                                                        if(selectedLab.lfu_FileType == "ETEST")
                                                        {
                                                            if (item.wnm_whonetfield.Contains("_NE"))
                                                            {
                                                                //wnm_originalfield = AntiValueColumnEtest; //MICIT_TROU
                                                                if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == "MICIT_TROU") == null) //MICIT_TROU
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == wnm_originalfield) == null)
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                        }
                                                        else //Automate
                                                        {
                                                            if (item.wnm_whonetfield.Contains("_NM"))
                                                            {
                                                                AntiValueColumnEtest = (from s in whonetmapping
                                                                                        where s.wnm_whonetfield.Contains("_NM")
                                                                                        select s.wnm_originalfield).Distinct().FirstOrDefault(); //Column20
                                                                if (MappingTemplate.mp_firstlineisheader == false)
                                                                {
                                                                    AntiValueColumnEtest = convertColumn(AntiValueColumnEtest);
                                                                }
                                                                wnm_originalfield = AntiValueColumnEtest; //MICIT_TROU
                                                                if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == wnm_originalfield) == null) //MICIT_TROU
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == wnm_originalfield) == null)
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                        }
                                                    }

                                                    var Encoding = "";
                                                    var tempvalue = "";
                                                    if (item.wnm_encrypt == true)
                                                    {
                                                        tempvalue = CryptoHelper.UnicodeEncoding(dbfDataReader[wnm_originalfield.Trim()].ToString());
                                                    }
                                                    else
                                                    {
                                                        if ((selectedLab.lfu_FileType == "ETEST" && item.wnm_whonetfield.Contains("_NE"))
                                                           || ((MappingTemplate.mp_AntibioticIsolateOneRec == false) && item.wnm_whonetfield.Contains("_NM")))
                                                        {
                                                            string antibiotic = dbfDataReader[AntiLocalCodeColumnEtest].ToString(); //"MICDR_ITSP"
                                                            //var lstAnti = MappingAnt.Select(m => m.wnm_originalfield).ToList();
                                                            if (antibiotic == wnm_originalfield.Trim())
                                                            {
                                                                tempvalue = dbfDataReader[AntiValueColumnEtest].ToString(); //"MICIT_TROU"
                                                            }
                                                            else
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == wnm_originalfield) != null)
                                                            {

                                                                if (dbfDataReader[wnm_originalfield.Trim()] != null)
                                                                {
                                                                    Encoding = dbfDataReader[wnm_originalfield.Trim()].ToString();

                                                                    if ((!string.IsNullOrEmpty(selectedLab.lfu_FileTypeLabel)) && item.wnm_whonetfield.Contains("_NM"))
                                                                    {

                                                                        if (selectedLab.lfu_FileTypeLabel == clsLabFileType.MLAB_FileType.MIC_SIR)
                                                                        {
                                                                            if (Encoding.ToUpper() == "S" || Encoding.ToUpper() == "I" || Encoding.ToUpper() == "R")
                                                                            {
                                                                                //MIC-SIR
                                                                                tempvalue = Encoding;
                                                                            }
                                                                            else
                                                                            {
                                                                                // do nothing
                                                                            }
                                                                        }

                                                                        else if (selectedLab.lfu_FileTypeLabel == clsLabFileType.MLAB_FileType.MIC_NUM)
                                                                        {
                                                                            if (Encoding.ToUpper() == "S" || Encoding.ToUpper() == "I" || Encoding.ToUpper() == "R")
                                                                            {
                                                                                // do nothing
                                                                            }
                                                                            else
                                                                            {
                                                                                //MIC-NUM
                                                                                tempvalue = Encoding;

                                                                            }
                                                                        }

                                                                        else
                                                                        {
                                                                            tempvalue = Encoding;
                                                                        }

                                                                        goto AddDetailPoint;
                                                                    }
                                                                    else
                                                                    {
                                                                        tempvalue = Encoding;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                    }
                                                AddDetailPoint:
                                                    LabFileDataDetailList.Add(new TRSTGLabFileDataDetail
                                                    {

                                                        ldd_id = Guid.NewGuid(),
                                                        ldd_status = 'N',
                                                        ldd_whonetfield = item.wnm_whonetfield,
                                                        ldd_originalfield = item.wnm_originalfield.Trim(),
                                                        ldd_originalvalue = tempvalue,
                                                        ldd_ldh_id = ldh_id,
                                                        ldd_createuser = "BATCH",
                                                        ldd_createdate = DateTime.Now
                                                    }
                                                    );

                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End loop Detail ... ");
                                        #endregion                                       
                                    }

                                    if (!blnRowError && WARD_TYPE != "" && (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == WARD_TYPE) != null))
                                    {
                                        var ward = dbfDataReader[WARD_TYPE].ToString().ToUpper();
                                        if (ward == "" || !lstWardType.Contains(ward.Trim())) { blnRowError = true; }
                                    }

                                    if (!blnRowError && SOURCE != "" && (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == SOURCE) != null))
                                    {
                                        var localspecimen = dbfDataReader[SOURCE].ToString().ToUpper();
                                        if (localspecimen == "" || !lstSpecimen.Contains(localspecimen.Trim())) { blnRowError = true; }
                                    }

                                    if (!blnRowError && ORGANISM != "" && (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == ORGANISM) != null))
                                    {
                                        var localorganism = dbfDataReader[ORGANISM].ToString().ToUpper();
                                        if (localorganism == "" || !lstOrganism.Contains(localorganism.Trim())) { blnRowError = true; }
                                    }

                                    if (blnRowError) { nRowError += 1; }
                                    dataRow += 1;
                                }

                                //EndWhile:


                                if (AntibioticErrorD.Count > 0)
                                {
                                    //nRowError += ErrorAnt;
                                    //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Save Error Header ... ");
                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                    objErrorH.feh_id = Guid.NewGuid();
                                    objErrorH.feh_status = 'N';
                                    objErrorH.feh_flagdelete = false;
                                    objErrorH.feh_type = "NOT_MATCH";
                                    objErrorH.feh_field = "Antibiotic";
                                    objErrorH.feh_message = "Antibiotic not match";
                                    objErrorH.feh_errorrecord = ErrorAnt;
                                    objErrorH.feh_createuser = "BATCH";
                                    objErrorH.feh_createdate = DateTime.Now;
                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);
                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Error Header ... ");

                                    AntibioticErrorD = AntibioticErrorD.Select(a => { a.fed_feh_id = objErrorH.feh_id; return a; }).ToList();
                                    var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetailList(AntibioticErrorD);

                                }

                                if (LabFileDataHeaderList.Count() != 0)
                                {
                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Save Header List ... ");
                                    var log_std = new LogWriter("Start Save Header List ...");

                                    LabDataDAL.InsertBulk_LabFileDataHeader(LabFileDataHeaderList);

                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Header List ... ");
                                    var log_endd = new LogWriter("End Save Header List ...");
                                }


                                if (LabFileDataDetailList.Count != 0)
                                {
                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Export Detail ... ");
                                    var log_std = new LogWriter("Start Export Detail ...");
                                    //var ldd = LabDataDAL.Save_LabFileDataDetail(LabFileDataDetailList);
                                    var ldd = Export_LabFileDataDetail(labdetail_path, LabFileDataDetailList, selectedLab);

                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Export Detail ... ");
                                    var log_endd = new LogWriter("End Export Detail ...");
                                }
                            }

                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Read File (.DBF) ... ");
                            var log_tranend = new LogWriter("End Read File (.DBF) ...");
                        }


                    Finish:
                        char str;
                        //if (ErrorAnt == 0)
                        //{
                        str = 'Q'; //'M'; 
                        //str = 'M';

                        //}
                        //else
                        //{
                        //    str = 'E';
                        //}
                        //var rowsAffected2 = LabDataDAL.Update_LabFileUploadStatus(selectedLab.lfu_id.ToString(), str, chkError + chkDetailError, "BATCH");
                        var rowsAffected2 = LabDataDAL.Update_LabFileUploadStatus(selectedLab.lfu_id.ToString(), str, nRowError, "BATCH");

                        var log_ed = new LogWriter(">> End File Name : " + selectedLab.lfu_FileName);
                    }
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Batch Lab End ... ");
                    var log_end = new LogWriter("Batch Lab Complete ...");
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Timeout Expired"))
                {
                    var rowsTimeOut = LabDataDAL.Update_LabFileUploadStatus(iFileUploadID.ToString(), 'T', 0, "BATCH");
                }
                else
                {
                    var exMsg = ex.Message.Substring(0, (ex.Message.Length > 200) ? 200 : ex.Message.Length);
                    var rowsAffected = LabDataDAL.Update_LabFileUploadStatus(iFileUploadID.ToString(), 'I', 0, "BATCH", exMsg);
                }

                var d = string.Format("Row : {0} Detail : {1}", (dataRow + 1).ToString(), ex.Message);

                var logw = new LogWriter(d);
                Console.WriteLine(d);
            }
        }

        public void LAB_NARST()
        {
            var LabDataDAL = new LabDataDAL();
            var LabFileUpload = LabDataDAL.Get_NewLabFileUpload('N');
            var LabFileUploadReprocess = LabDataDAL.Get_NewLabFileUpload('R');
            var LabFileUploadTimeOut = LabDataDAL.Get_NewLabFileUpload('T');

            if (LabFileUploadReprocess.Count > 0)
            {
                LabFileUpload.AddRange(LabFileUploadReprocess);
            }
            if (LabFileUploadTimeOut.Count > 0)
            {
                LabFileUpload.AddRange(LabFileUploadTimeOut);
            }
            string Param_labno = "";
            string Param_organism = "";
            string Param_specimen = "";
            string Param_date = "";
            string Param_wardtype = "";
            string lfu_mp_id = "";
            string labdetail_path = "";
          
            int nRowError = 0;
            bool blnRowError = false;
            Guid iFileUploadID = Guid.Empty;
            int nfile = 0;
            List<LabFileUploadDataDTO> LabFile = new List<LabFileUploadDataDTO>();
            int ErrorAnt = 0;
            int dataRow = 0;
            int dataLabFilePerRound = 3;

            try
            {
                if (LabFileUpload != null)
                {
                    var sortLab = LabFileUpload.OrderBy(o => o.lfu_createdate).ToList();
                    if (LabFileUpload.Count() > dataLabFilePerRound)
                    {
                        for (var i = 0; i < dataLabFilePerRound; i++)
                        {
                            LabFile.Add(sortLab[i]);
                            if (sortLab[i].lfu_status == 'T')
                            {
                                LabDataDAL.Delete_LabFileDataHeaderDetail(sortLab[i].lfu_id.ToString());
                            }
                            var rowsReprocDeleteFlag = LabDataDAL.Update_LabFileErrorStatus(sortLab[i].lfu_id.ToString(), 'D', "BATCH");
                            var rowsAffected = LabDataDAL.Update_LabFileUploadStatus(sortLab[i].lfu_id.ToString(), 'P', 0, "BATCH");
                            nfile += 1;
                        }
                    }
                    else
                    {
                        foreach (LabFileUploadDataDTO Lab in sortLab)
                        {
                            LabFile.Add(Lab);
                            if (Lab.lfu_status == 'T')
                            {
                                LabDataDAL.Delete_LabFileDataHeaderDetail(Lab.lfu_id.ToString());
                            }
                            var rowsReprocDeleteFlag = LabDataDAL.Update_LabFileErrorStatus(Lab.lfu_id.ToString(), 'D', "BATCH");
                            var rowsAffected = LabDataDAL.Update_LabFileUploadStatus(Lab.lfu_id.ToString(), 'P', 0, "BATCH");
                            nfile += 1;
                        }
                    }

                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Batch Lab Start ... ");
                    var log_start = new LogWriter(string.Format("Batch Lab Start {0}/{1} files ...", nfile, LabFileUpload.Count()));

                    List<TCParameter> ParameterKeyList = LabDataDAL.GetParameter("UPLOAD_KEY");
                    List<TCParameter> ParameterUploadList = LabDataDAL.GetParameter("UPLOAD_MAPPING");
                    List<TCParameter> ParameterMLABMappingList = LabDataDAL.GetParameter("MLAB_MAPPING_TEMPLATE");
                    List<TCParameter> ParameterPathList = LabDataDAL.GetParameter("LAB_DETAIL_PATH");

                    if (ParameterKeyList.Count != 0)
                    {
                        Param_labno = ParameterKeyList.FirstOrDefault(x => x.prm_code_minor == "LAB_NO").prm_value;
                        Param_organism = ParameterKeyList.FirstOrDefault(x => x.prm_code_minor == "ORGANISM").prm_value;
                        Param_specimen = ParameterKeyList.FirstOrDefault(x => x.prm_code_minor == "SPECIMEN").prm_value;
                        Param_date = ParameterKeyList.FirstOrDefault(x => x.prm_code_minor == "DATE").prm_value;
                    }

                    if (ParameterUploadList.Count != 0)
                    {
                        Param_wardtype = ParameterUploadList.FirstOrDefault(x => x.prm_code_minor == "WARD_TYPE").prm_value;
                    }

                    if (ParameterMLABMappingList.Count != 0)
                    {
                        lfu_mp_id = ParameterMLABMappingList.FirstOrDefault(x => x.prm_code_minor == "mp_id").prm_value;
                    }

                    if (ParameterPathList.Count != 0)
                    {
                        labdetail_path = ParameterPathList.FirstOrDefault(x => x.prm_code_minor == "PATH").prm_value;
                    }

                    foreach (LabFileUploadDataDTO selectedLab in LabFile)
                    {
                        var log_st = new LogWriter(string.Format(">> Hos : ({0}-{1}) File Name : {2}", selectedLab.lfu_hos_code, selectedLab.lfu_hos_name, selectedLab.lfu_FileName));
                        
                        nRowError = 0;
                        ErrorAnt = 0;
                        iFileUploadID = selectedLab.lfu_id;
                        if (selectedLab.lfu_Program != "MLAB") { lfu_mp_id = selectedLab.lfu_mp_id.ToString(); }
                        var MappingTemplate = LabDataDAL.GetMappingData(lfu_mp_id);
                        var whonetmapping = LabDataDAL.GetWHONetMappingList(lfu_mp_id, MappingTemplate.mp_mst_code);
                        var wardtypmapping = LabDataDAL.GetWardTypeMappingList(lfu_mp_id, MappingTemplate.mp_mst_code, selectedLab.lfu_hos_code);
                        var specimenmapping = LabDataDAL.GetSpecimeneMappingList(lfu_mp_id, MappingTemplate.mp_mst_code);
                        var organismmapping = LabDataDAL.GetOrganismMappingList(lfu_mp_id, MappingTemplate.mp_mst_code);

                        var lstWardType = wardtypmapping.Select(s => s.wdm_warddesc.ToUpper()).Distinct().ToList();
                        var lstSpecimen = specimenmapping.Select(s => s.spm_localspecimencode.ToUpper()).Distinct().ToList();
                        var lstOrganism = organismmapping.Select(s => s.ogm_localorganismcode.ToUpper()).Distinct().ToList();

                        //Get standard field
                        var WhonetmappingWithoutNM_NE_ND = whonetmapping.Except(whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NM") || x.wnm_whonetfield.Contains("_ND") || x.wnm_whonetfield.Contains("_NE")).ToList());

                        //Check file type and add whonetmapping
                        switch (selectedLab.lfu_FileType)
                        {
                            case clsLabFileType.MLAB_FileType.MIC:
                                whonetmapping = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NM")).ToList();
                                whonetmapping.AddRange(WhonetmappingWithoutNM_NE_ND);
                                break;
                            case clsLabFileType.MLAB_FileType.DISK:
                                whonetmapping = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_ND")).ToList();
                                whonetmapping.AddRange(WhonetmappingWithoutNM_NE_ND);
                                break;
                            case clsLabFileType.MLAB_FileType.ETEST:
                                whonetmapping = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NE")).ToList();
                                whonetmapping.AddRange(WhonetmappingWithoutNM_NE_ND);
                                break;
                        }


                        var listexcludeAntibiotic = new List<string>();
                        string AntiLocalCodeColumnEtest = "MICDR_ITSP";
                        string AntiValueColumnEtest = "MICIT_TROU";
                        EnumerableRowCollection<DataRow> AntNotMatch;
                        var lstAntErrDistinct = new List<string>();
                        var lstAntErrirCurrentYear = new List<string>();
                        int cntErrAntiCurrentYear = 0;

                        Guid feh_id_cdate = Guid.Empty;
                        char feh_status_cdate = 'N';                      
                        int cSpecDateError = 0;
                        string FieldDateType = "", DateFormat = "";
                        string LAB_NO = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_labno).wnm_originalfield;
                        string ORGANISM = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_organism).wnm_originalfield;
                        string DATE = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_date).wnm_originalfield;
                        DateFormat = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_date).wnm_fieldformat;

                        string WARD_TYPE = "";
                        if (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_wardtype) != null)
                            WARD_TYPE = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_wardtype).wnm_originalfield;

                        string SOURCE = "";
                        if (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_specimen) != null)
                            SOURCE = whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == Param_specimen).wnm_originalfield;

                        if (MappingTemplate.mp_firstlineisheader == false)
                        {
                            LAB_NO = convertColumn(LAB_NO);
                            ORGANISM = convertColumn(ORGANISM);

                            if (SOURCE != "")
                                SOURCE = convertColumn(SOURCE);

                            DATE = convertColumn(DATE);
                        }

                        //List<TRSTGLabFileDataHeader> LabFileDataHeaderList = new List<TRSTGLabFileDataHeader>();
                        //List<TRSTGLabFileDataDetail> LabFileDataDetailList = new List<TRSTGLabFileDataDetail>();
                        List<TRSTGLabNarstData> LabNarstList = new List<TRSTGLabNarstData>();
                        List<TRSTGLabNarstData_DISK> LabNarstList_DISK = new List<TRSTGLabNarstData_DISK>();
                        List<TRSTGLabNarstData_MIC> LabNarstList_MIC = new List<TRSTGLabNarstData_MIC>();
                        List<TRSTGLabNarstData_ETEST> LabNarstList_ETEST = new List<TRSTGLabNarstData_ETEST>();

                        //if (MappingTemplate.mp_AntibioticIsolateOneRec == false)
                        //>> for Etest : Antibiotic Isolate more than 1 Record
                        List<WHONetMappingListsDTO> MappingAntETEST = new List<WHONetMappingListsDTO>();
                        if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
                        {
                            MappingAntETEST = whonetmapping.Where(x => x.wnm_whonetfield.Contains("_NE")).ToList(); ;
                            if (MappingTemplate.mp_firstlineisheader == false)
                            {

                                AntiLocalCodeColumnEtest = convertColumn(AntiLocalCodeColumnEtest);
                            }

                            foreach (WHONetMappingListsDTO t in MappingAntETEST)
                            {
                                listexcludeAntibiotic.Add(t.wnm_originalfield);
                            }
                        }
                        #region xlsx
                        if (Path.GetExtension(selectedLab.lfu_FileName) == ".xls" || Path.GetExtension(selectedLab.lfu_FileName) == ".xlsx" || Path.GetExtension(selectedLab.lfu_FileName) == ".csv" || Path.GetExtension(selectedLab.lfu_FileName) == ".txt")
                        {
                            
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Read File ... ");
                            var log_tran = new LogWriter("Start Read File ... ");

                            using (var stream = File.Open(selectedLab.lfu_Path, FileMode.Open, FileAccess.Read))
                            {
                                DataSet result = new DataSet();

                                if (Path.GetExtension(selectedLab.lfu_FileName) == ".xls" || Path.GetExtension(selectedLab.lfu_FileName) == ".xlsx")
                                {
                                    ExcelDataSetConfiguration option = new ExcelDataSetConfiguration();
                                    var reader = ExcelReaderFactory.CreateReader(stream);
                                    if (MappingTemplate.mp_firstlineisheader == true)
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
                                else if (Path.GetExtension(selectedLab.lfu_FileName) == ".csv")
                                {
                                    var reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
                                    {
                                        FallbackEncoding = Encoding.GetEncoding(1252),
                                        AutodetectSeparators = new char[] { ',', ';', '\t', '|', '#' },
                                        LeaveOpen = false,
                                        AnalyzeInitialCsvRows = 0,
                                    });

                                    if (MappingTemplate.mp_firstlineisheader == true)
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
                                else if (Path.GetExtension(selectedLab.lfu_FileName) == ".txt")
                                {
                                    string line;
                                    DataTable dt = new DataTable();

                                    using (TextReader tr = File.OpenText(selectedLab.lfu_Path))
                                    {
                                        while ((line = tr.ReadLine()) != null)
                                        {
                                            string[] items = line.Split('\t');
                                            if (dt.Columns.Count == 0)
                                            {
                                                if (MappingTemplate.mp_firstlineisheader == false)
                                                {
                                                    for (int i = 0; i < items.Length; i++)
                                                        dt.Columns.Add(new DataColumn("Column" + i, typeof(string)));
                                                }
                                                else
                                                {
                                                    for (int i = 0; i < items.Length; i++)
                                                        dt.Columns.Add(new DataColumn(items[i].ToString(), typeof(string)));
                                                }
                                            }
                                            dt.Rows.Add(items);
                                        }
                                    }

                                    result.Tables.Add(dt);


                                }

                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Read File ... ");
                                var log_tran1 = new LogWriter("End Read File ...");

                                if (result.Tables[0].Columns.Contains(DATE) == true)
                                {
                                    FieldDateType = result.Tables[0].Columns[DATE].DataType.ToString();
                                }
                             
                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Header Data ... ");
                                var log_read = new LogWriter("Start Header Data ...");
                                dataRow = 1;

                                if (listexcludeAntibiotic.Count > 0)
                                {
                                    AntNotMatch = result.Tables[0].AsEnumerable().Where(row => !listexcludeAntibiotic.Contains(row.Field<string>(AntiLocalCodeColumnEtest)));
                                    ErrorAnt = AntNotMatch.Count();
                                    var AntNotMatchDistinct = AntNotMatch.AsEnumerable().Select(s => new { antibiotic = s.Field<string>(AntiLocalCodeColumnEtest), }).Distinct().ToList();
                                    foreach (var lst in AntNotMatchDistinct)
                                    {
                                        lstAntErrDistinct.Add(lst.antibiotic);
                                    }
                                }

                                //Loop Read Data Row
                                foreach (DataRow row in result.Tables[0].Rows)
                                {
                                    string cmethod = "";
                                    blnRowError = false;

                                    if (result.Tables[0].Columns.Contains("CMETHOD"))
                                    {
                                        cmethod = row["CMETHOD"].ToString();
                                    }

                                    if ((cmethod == "Aerobic Culture" && selectedLab.lfu_Program == "MLAB")
                                   || selectedLab.lfu_Program != "MLAB"
                                   || (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST"))
                                    {

                                        #region InsertLabFileDataHeader
                                        //TRSTGLabFileDataHeader objModel = new TRSTGLabFileDataHeader();
                                      

                                        //Guid ldh_id = Guid.NewGuid();
                                        //---> key is null, continue
                                        if (!string.IsNullOrEmpty(LAB_NO) && string.IsNullOrEmpty(row[LAB_NO].ToString()))
                                        {
                                            continue;
                                        }
                                        else if (!string.IsNullOrEmpty(ORGANISM) && string.IsNullOrEmpty(row[ORGANISM].ToString()))
                                        {
                                            continue;
                                        }
                                        else if (!string.IsNullOrEmpty(DATE) && string.IsNullOrEmpty(row[DATE].ToString()))
                                        {
                                            continue;
                                        }
                                        if (selectedLab.lfu_FileType != "ETEST")
                                        {
                                            if (!string.IsNullOrEmpty(SOURCE) && string.IsNullOrEmpty(row[SOURCE].ToString()))
                                            {
                                                continue;
                                            }
                                        }
                                        //---> key is null, continue

                                        //---> Check Date Format
                                        var strCDATE = row[DATE].ToString();
                                        DateTime? dtSPEC_DATE = new DateTime();
                                        if (FieldDateType == "System.String")
                                        {
                                            try
                                            {                                               
                                                dtSPEC_DATE = CovertStringToDate(strCDATE, DateFormat);
                                                int year = dtSPEC_DATE.Value.Year;

                                                if (year < 1000)
                                                {
                                                    if (feh_id_cdate == Guid.Empty)
                                                    {
                                                        feh_id_cdate = Guid.NewGuid();
                                                    }
                                                    else
                                                    {
                                                        feh_status_cdate = 'E';
                                                    }


                                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                    objErrorH.feh_id = feh_id_cdate;
                                                    objErrorH.feh_status = feh_status_cdate;
                                                    objErrorH.feh_flagdelete = false;
                                                    objErrorH.feh_type = "CONVERT_ERROR";
                                                    objErrorH.feh_field = DATE;
                                                    objErrorH.feh_message = "Cannot convert date.";
                                                    cSpecDateError += 1;
                                                    objErrorH.feh_errorrecord = cSpecDateError;
                                                    objErrorH.feh_createuser = "BATCH";
                                                    objErrorH.feh_createdate = DateTime.Now;
                                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                    blnRowError = true;
                                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

                                                    continue;
                                                }

                                                if (year > DateTime.Now.Year)
                                                {
                                                    dtSPEC_DATE = dtSPEC_DATE.Value.AddYears(-543);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                //chkError++;

                                                if (feh_id_cdate == Guid.Empty)
                                                {
                                                    feh_id_cdate = Guid.NewGuid();
                                                }
                                                else
                                                {
                                                    feh_status_cdate = 'E';
                                                }


                                                TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                objErrorH.feh_id = feh_id_cdate;
                                                objErrorH.feh_status = feh_status_cdate;
                                                objErrorH.feh_flagdelete = false;
                                                objErrorH.feh_type = "CONVERT_ERROR";
                                                objErrorH.feh_field = DATE;
                                                objErrorH.feh_message = "Cannot convert date.";
                                                cSpecDateError += 1;
                                                objErrorH.feh_errorrecord = cSpecDateError;
                                                objErrorH.feh_createuser = "BATCH";
                                                objErrorH.feh_createdate = DateTime.Now;
                                                objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                blnRowError = true;
                                                var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);


                                                TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
                                                objErrorD.fed_id = Guid.NewGuid();
                                                objErrorD.fed_status = 'N';
                                                objErrorD.fed_localvalue = ex.Message;
                                                objErrorD.fed_feh_id = feh_id_cdate;
                                                objErrorD.fed_createuser = "BATCH";
                                                objErrorD.fed_createdate = DateTime.Now;

                                                var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
                                                continue;
                                            }
                                        }
                                        else if (FieldDateType == "System.DateTime")
                                        {
                                            dtSPEC_DATE = (DateTime)row[DATE];
                                            int year = dtSPEC_DATE.Value.Year;
                                            if (year > DateTime.Now.Year)
                                            {
                                                dtSPEC_DATE = dtSPEC_DATE.Value.AddYears(-543);
                                            }
                                        }
                                        else if (FieldDateType == "System.Object") //-- am add 18/01/2021 --
                                        {
                                            if (row[DATE].GetType().FullName == "System.DateTime")
                                            {
                                                dtSPEC_DATE = (DateTime)row[DATE];
                                                int year = dtSPEC_DATE.Value.Year;
                                                if (year > DateTime.Now.Year)
                                                {
                                                    dtSPEC_DATE = dtSPEC_DATE.Value.AddYears(-543);
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    dtSPEC_DATE = CovertStringToDate(strCDATE, DateFormat);
                                                    //objModel.ldh_cdate = DateTime.ParseExact(objModel.ldh_date, DateFormat, CultureInfo.GetCultureInfo("en-US"));
                                                    int year = dtSPEC_DATE.Value.Year;

                                                    if (year < 1000)
                                                    {
                                                        if (feh_id_cdate == Guid.Empty)
                                                        {
                                                            feh_id_cdate = Guid.NewGuid();
                                                        }
                                                        else
                                                        {
                                                            feh_status_cdate = 'E';
                                                        }


                                                        TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                        objErrorH.feh_id = feh_id_cdate;
                                                        objErrorH.feh_status = feh_status_cdate;
                                                        objErrorH.feh_flagdelete = false;
                                                        objErrorH.feh_type = "CONVERT_ERROR";
                                                        objErrorH.feh_field = DATE;
                                                        objErrorH.feh_message = "Cannot convert date.";
                                                        cSpecDateError += 1;
                                                        objErrorH.feh_errorrecord = cSpecDateError;
                                                        objErrorH.feh_createuser = "BATCH";
                                                        objErrorH.feh_createdate = DateTime.Now;
                                                        objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                        blnRowError = true;


                                                        var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);
                                                    }

                                                    if (year > DateTime.Now.Year)
                                                    {
                                                        dtSPEC_DATE = dtSPEC_DATE.Value.AddYears(-543);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    //chkError++;

                                                    if (feh_id_cdate == Guid.Empty)
                                                    {
                                                        feh_id_cdate = Guid.NewGuid();
                                                    }
                                                    else
                                                    {
                                                        feh_status_cdate = 'E';
                                                    }


                                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                                    objErrorH.feh_id = feh_id_cdate;
                                                    objErrorH.feh_status = feh_status_cdate;
                                                    objErrorH.feh_flagdelete = false;
                                                    objErrorH.feh_type = "CONVERT_ERROR";
                                                    objErrorH.feh_field = DATE;
                                                    objErrorH.feh_message = "Cannot convert date.";
                                                    cSpecDateError += 1;
                                                    objErrorH.feh_errorrecord = cSpecDateError;
                                                    objErrorH.feh_createuser = "BATCH";
                                                    objErrorH.feh_createdate = DateTime.Now;
                                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                                    blnRowError = true;


                                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

                                                    TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
                                                    objErrorD.fed_id = Guid.NewGuid();
                                                    objErrorD.fed_status = 'N';
                                                    objErrorD.fed_localvalue = ex.Message;
                                                    objErrorD.fed_feh_id = objTRLabFileErrorHeader;
                                                    objErrorD.fed_createuser = "BATCH";
                                                    objErrorD.fed_createdate = DateTime.Now;

                                                    var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
                                                    continue;
                                                }
                                            }
                                        }

                                        // Etest and Data not this year : continue
                                        if (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST" && dtSPEC_DATE.Value.Year != selectedLab.lfu_DataYear)
                                        {
                                            continue;
                                        }

                                        //<-- End Check Date Format

                                        //---> Check Antibiotic                                     
                                        if (lstAntErrDistinct.Count > 0)
                                        {
                                            var anti = row[AntiLocalCodeColumnEtest].ToString();
                                            if (lstAntErrDistinct.Contains(anti))
                                            {
                                                cntErrAntiCurrentYear += 1;
                                                blnRowError = true;
                                                if (!lstAntErrirCurrentYear.Contains(anti))
                                                {
                                                    lstAntErrirCurrentYear.Add(anti);
                                                }
                                                continue;
                                            }
                                        }
                                        //<-- End Check Antibiotic

                                        if (selectedLab.lfu_Program != "MLAB")
                                        {
                                            TRSTGLabNarstData objModel = new TRSTGLabNarstData();

                                            objModel.COUNTRY_A = "THA";
                                            objModel.LABORATORY = "H21";

                                            objModel.lna_lfu_id = selectedLab.lfu_id;
                                            objModel.lna_lfu_Program = selectedLab.lfu_Program;
                                            objModel.lna_lfu_FileType = selectedLab.lfu_FileType;
                                            objModel.lna_hos_code = selectedLab.lfu_hos_code;
                                            objModel.lna_lab = selectedLab.lfu_lab;
                                            objModel.lna_labno = row[LAB_NO].ToString();
                                            objModel.lna_organism_original_value = row[ORGANISM].ToString();
                                            objModel.lna_cdate = strCDATE;
                                            objModel.SPEC_DATE = dtSPEC_DATE;
                                            objModel.lna_status = "N";
                                            objModel.lna_sequence = 1; // Get Lastest
                                            objModel.lna_createuser = "BATCH";
                                            objModel.lna_createdate = DateTime.Now;

                                            if (WARD_TYPE != "" && (result.Tables[0].Columns.Contains(WARD_TYPE)))
                                            {
                                                var ward = row[WARD_TYPE].ToString().ToUpper();
                                                if (!string.IsNullOrEmpty(ward))
                                                {
                                                    objModel.lna_wardtype_original_value = ward;
                                                }                                                                                             
                                            }

                                            if (SOURCE != "" && (result.Tables[0].Columns.Contains(SOURCE)))
                                            {
                                                var localspecimen = row[SOURCE].ToString();
                                                if (!string.IsNullOrEmpty(localspecimen))
                                                {
                                                    objModel.lna_specimen_original_value = localspecimen;
                                                }                                              
                                            }

                                            if((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name") != null) 
                                                && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_originalfield)))
                                            {
                                                var val = row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_originalfield].ToString();
                                                objModel.FIRST_NAME = ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_encrypt == true)? CryptoHelper.UnicodeEncoding(val) : val);
                                            }

                                            if ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name") != null)
                                                && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_originalfield)))
                                            {
                                                var val = row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_originalfield].ToString();
                                                objModel.LAST_NAME = ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_encrypt == true) ? CryptoHelper.UnicodeEncoding(val) : val);
                                            }

                                            objModel.PATIENT_ID = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number").wnm_originalfield].ToString();
                                            objModel.SEX = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex").wnm_originalfield].ToString();
                                            objModel.AGE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age").wnm_originalfield].ToString();
                                            objModel.WARD = (whonetmapping.FirstOrDefault(x => x.wnm_originalfield == "Location") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location").wnm_originalfield].ToString();
                                            objModel.DEPARTMENT = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Department") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Department").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Department").wnm_originalfield].ToString();
                                            objModel.ORG_TYPE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Organism type") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Organism type").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Organism type").wnm_originalfield].ToString();
                                            objModel.ISOL_NUM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Isolate number") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Isolate number").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Isolate number").wnm_originalfield].ToString();
                                            objModel.NOSOCOMIAL = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Nosocomial infection") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Nosocomial infection").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Nosocomial infection").wnm_originalfield].ToString();

                                            if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth")) != null) && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield))
                                                 && row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield] != null)
                                            {
                                                try
                                                {
                                                    objModel.DATE_BIRTH = (DateTime)row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield];
                                                    if (objModel.DATE_BIRTH.Value.Year <= 1753 || objModel.DATE_BIRTH.Value.Year >= 9999) { objModel.DATE_BIRTH = null; }
                                                }
                                                catch
                                                {
                                                    objModel.lna_date_birth_str = row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield].ToString();
                                                }
                                            }
                                            if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry")) != null) && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield))
                                                && row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield] != null)
                                            {
                                                try
                                                {
                                                    objModel.DATE_DATA = (DateTime)row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield];
                                                    if (objModel.DATE_DATA.Value.Year <= 1753 || objModel.DATE_DATA.Value.Year >= 9999) { objModel.DATE_DATA = null; }
                                                }
                                                catch
                                                {
                                                }
                                            }
                                            if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission")) != null) && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield))
                                                && row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield] != null)
                                            {
                                                try
                                                {
                                                    objModel.DATE_ADMIS = (DateTime)row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield];
                                                    if (objModel.DATE_ADMIS.Value.Year <= 1753 || objModel.DATE_ADMIS.Value.Year >= 9999) { objModel.DATE_ADMIS = null; }
                                                }
                                                catch
                                                {
                                                }
                                            }                                                                                                                             

                                            AddAntibiotic(row, whonetmapping, selectedLab.lfu_Program, objModel,null,null, result.Tables[0]);
                                            
                                            LabNarstList.Add(objModel);
                                        }
                                        else
                                        {
                                            if (selectedLab.lfu_FileType == "DISK")
                                            {
                                                TRSTGLabNarstData_DISK objModel = new TRSTGLabNarstData_DISK();

                                                objModel.COUNTRY_A = "THA";
                                                objModel.LABORATORY = "H21";

                                                objModel.lnd_lfu_id = selectedLab.lfu_id;
                                                objModel.lnd_lfu_Program = selectedLab.lfu_Program;
                                                objModel.lnd_lfu_FileType = selectedLab.lfu_FileType;
                                                objModel.lnd_hos_code = selectedLab.lfu_hos_code;
                                                objModel.lnd_lab = selectedLab.lfu_lab;
                                                objModel.lnd_labno = row[LAB_NO].ToString();
                                                objModel.lnd_organism_original_value = row[ORGANISM].ToString();
                                                objModel.lnd_cdate = strCDATE;
                                                objModel.SPEC_DATE = dtSPEC_DATE;
                                                objModel.lnd_status = "N";
                                                objModel.lnd_sequence = 1; // Get Lastest
                                                objModel.lnd_createuser = "BATCH";
                                                objModel.lnd_createdate = DateTime.Now;

                                                if (WARD_TYPE != "" && (result.Tables[0].Columns.Contains(WARD_TYPE)))
                                                {
                                                    var ward = row[WARD_TYPE].ToString();
                                                    if (!string.IsNullOrEmpty(ward))
                                                    {
                                                        objModel.lnd_wardtype_original_value = ward;                                                        
                                                    }
                                                }

                                                if (SOURCE != "" && (result.Tables[0].Columns.Contains(SOURCE)))
                                                {
                                                    var localspecimen = row[SOURCE].ToString();
                                                    if (!string.IsNullOrEmpty(localspecimen))
                                                    {
                                                        objModel.lnd_specimen_original_value = localspecimen;
                                                    }
                                                }

                                                if ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name") != null)
                                                  && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_originalfield)))
                                                {
                                                    var val = row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_originalfield].ToString();
                                                    objModel.FIRST_NAME = ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_encrypt == true) ? CryptoHelper.UnicodeEncoding(val) : val);
                                                }

                                                if ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name") != null)
                                                    && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_originalfield)))
                                                {
                                                    var val = row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_originalfield].ToString();
                                                    objModel.LAST_NAME = ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_encrypt == true) ? CryptoHelper.UnicodeEncoding(val) : val);
                                                }
                                                
                                                objModel.PATIENT_ID = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number").wnm_originalfield].ToString();
                                                objModel.SEX = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex").wnm_originalfield))  ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex").wnm_originalfield].ToString();
                                                objModel.AGE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age").wnm_originalfield].ToString();
                                                objModel.WARD = (whonetmapping.FirstOrDefault(x => x.wnm_originalfield == "Location") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location").wnm_originalfield].ToString();
                                                objModel.lnd_csource = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Local specimen code") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Local specimen code").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Local specimen code").wnm_originalfield].ToString();
                                                objModel.lnd_corganism = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Comment") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Comment").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Comment").wnm_originalfield].ToString();

                                                if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth")) != null) && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield))
                                                 && row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield] != null)
                                                {
                                                    try
                                                    {
                                                        objModel.DATE_BIRTH = (DateTime)row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield];
                                                    }
                                                    catch
                                                    {
                                                        objModel.lnd_date_birth_str = row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield].ToString();
                                                    }
                                                }
                                                if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry")) != null) && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield))
                                                    && row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield] != null)
                                                {
                                                    try
                                                    {
                                                        objModel.DATE_DATA = (DateTime)row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield];
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission")) != null) && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield))
                                                    && row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield] != null)
                                                {
                                                    try
                                                    {
                                                        objModel.DATE_ADMIS = (DateTime)row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield];
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                AddAntibiotic(row, whonetmapping, selectedLab.lfu_FileType, null, objModel, null, result.Tables[0]);
                                                LabNarstList_DISK.Add(objModel);
                                            }

                                            else if (selectedLab.lfu_FileType == "MIC")
                                            {
                                                TRSTGLabNarstData_MIC objModel = new TRSTGLabNarstData_MIC();

                                                objModel.COUNTRY_A = "THA";
                                                objModel.LABORATORY = "H21";

                                                objModel.lnm_lfu_id = selectedLab.lfu_id;
                                                objModel.lnm_lfu_Program = selectedLab.lfu_Program;
                                                objModel.lnm_lfu_FileType = selectedLab.lfu_FileType;
                                                objModel.lnm_lfu_FileTypeLabel = (!string.IsNullOrEmpty(selectedLab.lfu_FileTypeLabel)? selectedLab.lfu_FileTypeLabel : null) ;
                                                objModel.lnm_hos_code = selectedLab.lfu_hos_code;
                                                objModel.lnm_lab = selectedLab.lfu_lab;
                                                objModel.lnm_labno = row[LAB_NO].ToString();
                                                objModel.lnm_organism_original_value = row[ORGANISM].ToString();
                                                objModel.lnm_cdate = strCDATE;
                                                objModel.SPEC_DATE = dtSPEC_DATE;
                                                objModel.lnm_status = "N";
                                                objModel.lnm_sequence = 1; // Get Lastest
                                                objModel.lnm_createuser = "BATCH";
                                                objModel.lnm_createdate = DateTime.Now;

                                                if (WARD_TYPE != "" && (result.Tables[0].Columns.Contains(WARD_TYPE)))
                                                {
                                                    var ward = row[WARD_TYPE].ToString().ToUpper();
                                                    if (!string.IsNullOrEmpty(ward))
                                                    {
                                                        objModel.lnm_wardtype_original_value = ward;
                                                    }
                                                }

                                                if (SOURCE != "" && (result.Tables[0].Columns.Contains(SOURCE)))
                                                {
                                                    var localspecimen = row[SOURCE].ToString().ToUpper();
                                                    if (!string.IsNullOrEmpty(localspecimen))
                                                    {
                                                        objModel.lnm_specimen_original_value = localspecimen;
                                                    }
                                                }

                                                if ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name") != null)
                                                    && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_originalfield)))
                                                {
                                                    var val = row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_originalfield].ToString();
                                                    objModel.FIRST_NAME = ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_encrypt == true) ? CryptoHelper.UnicodeEncoding(val) : val);
                                                }

                                                if ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name") != null)
                                                    && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_originalfield)))
                                                {
                                                    var val = row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_originalfield].ToString();
                                                    objModel.LAST_NAME = ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_encrypt == true) ? CryptoHelper.UnicodeEncoding(val) : val);
                                                }

                                                objModel.PATIENT_ID = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number").wnm_originalfield].ToString();
                                                objModel.SEX = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex").wnm_originalfield].ToString();
                                                objModel.AGE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age").wnm_originalfield].ToString();
                                                objModel.WARD = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location").wnm_originalfield].ToString();
                                                objModel.lnm_csource = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Local specimen code") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Local specimen code").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Local specimen code").wnm_originalfield].ToString();
                                                objModel.lnm_corganism = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Comment") == null) || (!result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Comment").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Comment").wnm_originalfield].ToString();

                                                if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth")) != null) && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield))
                                                    && row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield] != null)
                                                {
                                                    try
                                                    {
                                                        objModel.DATE_BIRTH = (DateTime)row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield];
                                                        if (objModel.DATE_BIRTH.Value.Year <= 1753 || objModel.DATE_BIRTH.Value.Year >= 9999) { objModel.DATE_BIRTH = null; }
                                                    }
                                                    catch
                                                    {
                                                        objModel.lnm_date_birth_str = row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield].ToString();
                                                    }
                                                }
                                                if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry")) != null) && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield))
                                                    && row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield] != null)
                                                {
                                                    try
                                                    {
                                                        objModel.DATE_DATA = (DateTime)row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield];
                                                        if (objModel.DATE_DATA.Value.Year <= 1753 || objModel.DATE_DATA.Value.Year >= 9999) { objModel.DATE_DATA = null; }
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission")) != null) && (result.Tables[0].Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield))
                                                    && row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield] != null)
                                                {
                                                    try
                                                    {
                                                        objModel.DATE_ADMIS = (DateTime)row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield];
                                                        if (objModel.DATE_ADMIS.Value.Year <= 1753 || objModel.DATE_ADMIS.Value.Year >= 9999) { objModel.DATE_ADMIS = null; }
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                AddAntibiotic(row, whonetmapping, selectedLab.lfu_FileType, null, null, objModel, result.Tables[0]);
                                                LabNarstList_MIC.Add(objModel);
                                            }

                                            else if (selectedLab.lfu_FileType == "ETEST")
                                            {
                                                TRSTGLabNarstData_ETEST objModel = new TRSTGLabNarstData_ETEST();

                                                var drugETEST = row[AntiLocalCodeColumnEtest].ToString(); //MICDR_ITSP
                                                var valueETEST = row[AntiValueColumnEtest].ToString(); //MICIT_TROU

                                                var inFile = LabNarstList_ETEST.FirstOrDefault(x => x.lne_hos_code == selectedLab.lfu_hos_code
                                                                                 && x.lne_labno == row[LAB_NO].ToString()
                                                                                 && x.lne_organism_original_value == row[ORGANISM].ToString()
                                                                                 && x.lne_cdate == strCDATE);

                                                if (inFile != null)
                                                {
                                                    objModel = inFile;
                                                    objModel.lne_updatedate = DateTime.Now;
                                                }
                                                else
                                                {
                                                    objModel.COUNTRY_A = "THA";
                                                    objModel.LABORATORY = "H21";

                                                    objModel.lne_lfu_id = selectedLab.lfu_id;
                                                    objModel.lne_lfu_Program = selectedLab.lfu_Program;
                                                    objModel.lne_lfu_FileType = selectedLab.lfu_FileType;
                                                    objModel.lne_hos_code = selectedLab.lfu_hos_code;
                                                    objModel.lne_lab = selectedLab.lfu_lab;
                                                    objModel.lne_labno = row[LAB_NO].ToString();
                                                    objModel.lne_organism_original_value = row[ORGANISM].ToString();
                                                    objModel.lne_cdate = strCDATE;
                                                    objModel.SPEC_DATE = dtSPEC_DATE;
                                                    objModel.lne_status = "N";
                                                    objModel.lne_sequence = 1; // Get Lastest
                                                    objModel.lne_createuser = "BATCH";
                                                    objModel.lne_createdate = DateTime.Now;
                                                }                                               

                                                //var objCheck = LabDataDAL.CheckExist_ETEST(objModel);
                                                //if (objCheck != null)
                                                //{
                                                //    objModel = objCheck;
                                                //}

                                                if ((MappingAntETEST.FirstOrDefault(x => x.wnm_originalfield == drugETEST) != null))
                                                {
                                                    var whonetAnti = MappingAntETEST.FirstOrDefault(x => x.wnm_originalfield == drugETEST).wnm_whonetfield;
                                                    if (whonetAnti == "AMX_NE") { objModel.AMX_NE = ((string.IsNullOrEmpty(objModel.AMX_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.AMX_NE) && objModel.AMX_NE != valueETEST)) ? valueETEST : objModel.AMX_NE; }
                                                    if (whonetAnti == "CTX_NE") { objModel.CTX_NE = ((string.IsNullOrEmpty(objModel.CTX_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CTX_NE) && objModel.CTX_NE != valueETEST)) ? valueETEST : objModel.CTX_NE; }
                                                    if (whonetAnti == "CAZ_NE") { objModel.CAZ_NE = ((string.IsNullOrEmpty(objModel.CAZ_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CAZ_NE) && objModel.CAZ_NE != valueETEST)) ? valueETEST : objModel.CAZ_NE; }
                                                    if (whonetAnti == "CZX_NE") { objModel.CZX_NE = ((string.IsNullOrEmpty(objModel.CZX_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CZX_NE) && objModel.CZX_NE != valueETEST)) ? valueETEST : objModel.CZX_NE; }
                                                    if (whonetAnti == "CRO_NE") { objModel.CRO_NE = ((string.IsNullOrEmpty(objModel.CRO_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CRO_NE) && objModel.CRO_NE != valueETEST)) ? valueETEST : objModel.CRO_NE; }
                                                    if (whonetAnti == "CHL_NE") { objModel.CHL_NE = ((string.IsNullOrEmpty(objModel.CHL_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CHL_NE) && objModel.CHL_NE != valueETEST)) ? valueETEST : objModel.CHL_NE; }
                                                    if (whonetAnti == "CIP_NE") { objModel.CIP_NE = ((string.IsNullOrEmpty(objModel.CIP_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CIP_NE) && objModel.CIP_NE != valueETEST)) ? valueETEST : objModel.CIP_NE; }
                                                    if (whonetAnti == "CLI_NE") { objModel.CLI_NE = ((string.IsNullOrEmpty(objModel.CLI_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CLI_NE) && objModel.CLI_NE != valueETEST)) ? valueETEST : objModel.CLI_NE; }
                                                    if (whonetAnti == "COL_NE") { objModel.COL_NE = ((string.IsNullOrEmpty(objModel.COL_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.COL_NE) && objModel.COL_NE != valueETEST)) ? valueETEST : objModel.COL_NE; }
                                                    if (whonetAnti == "DAP_NE") { objModel.DAP_NE = ((string.IsNullOrEmpty(objModel.DAP_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.DAP_NE) && objModel.DAP_NE != valueETEST)) ? valueETEST : objModel.DAP_NE; }
                                                    if (whonetAnti == "ETP_NE") { objModel.ETP_NE = ((string.IsNullOrEmpty(objModel.ETP_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.ETP_NE) && objModel.ETP_NE != valueETEST)) ? valueETEST : objModel.ETP_NE; }
                                                    if (whonetAnti == "ERY_NE") { objModel.ERY_NE = ((string.IsNullOrEmpty(objModel.ERY_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.ERY_NE) && objModel.ERY_NE != valueETEST)) ? valueETEST : objModel.ERY_NE; }
                                                    if (whonetAnti == "IPM_NE") { objModel.IPM_NE = ((string.IsNullOrEmpty(objModel.IPM_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.IPM_NE) && objModel.IPM_NE != valueETEST)) ? valueETEST : objModel.IPM_NE; }
                                                    if (whonetAnti == "MEM_NE") { objModel.MEM_NE = ((string.IsNullOrEmpty(objModel.MEM_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.MEM_NE) && objModel.MEM_NE != valueETEST)) ? valueETEST : objModel.MEM_NE; }
                                                    if (whonetAnti == "NET_NE") { objModel.NET_NE = ((string.IsNullOrEmpty(objModel.NET_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.NET_NE) && objModel.NET_NE != valueETEST)) ? valueETEST : objModel.NET_NE; }
                                                    if (whonetAnti == "PEN_NE") { objModel.PEN_NE = ((string.IsNullOrEmpty(objModel.PEN_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.PEN_NE) && objModel.PEN_NE != valueETEST)) ? valueETEST : objModel.PEN_NE; }
                                                    if (whonetAnti == "VAN_NE") { objModel.VAN_NE = ((string.IsNullOrEmpty(objModel.VAN_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.VAN_NE) && objModel.VAN_NE != valueETEST)) ? valueETEST : objModel.VAN_NE; }
                                                    if (whonetAnti == "AZM_NE") { objModel.AZM_NE = ((string.IsNullOrEmpty(objModel.AZM_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.AZM_NE) && objModel.AZM_NE != valueETEST)) ? valueETEST : objModel.AZM_NE; }
                                                    if (whonetAnti == "GEN_NE") { objModel.GEN_NE = ((string.IsNullOrEmpty(objModel.GEN_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.GEN_NE) && objModel.GEN_NE != valueETEST)) ? valueETEST : objModel.GEN_NE; }
                                                    if (whonetAnti == "TCY_NE") { objModel.TCY_NE = ((string.IsNullOrEmpty(objModel.TCY_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.TCY_NE) && objModel.TCY_NE != valueETEST)) ? valueETEST : objModel.TCY_NE; }
                                                    if (whonetAnti == "CFM_NE") { objModel.CFM_NE = ((string.IsNullOrEmpty(objModel.CFM_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CFM_NE) && objModel.CFM_NE != valueETEST)) ? valueETEST : objModel.CFM_NE; }
                                                    if (whonetAnti == "AMP_NE") { objModel.AMP_NE = ((string.IsNullOrEmpty(objModel.AMP_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.AMP_NE) && objModel.AMP_NE != valueETEST)) ? valueETEST : objModel.AMP_NE; }
                                                    if (whonetAnti == "FEP_NE") { objModel.FEP_NE = ((string.IsNullOrEmpty(objModel.FEP_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.FEP_NE) && objModel.FEP_NE != valueETEST)) ? valueETEST : objModel.FEP_NE; }
                                                    if (whonetAnti == "LVX_NE") { objModel.LVX_NE = ((string.IsNullOrEmpty(objModel.LVX_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.LVX_NE) && objModel.LVX_NE != valueETEST)) ? valueETEST : objModel.LVX_NE; }
                                                    if (whonetAnti == "AMK_NE") { objModel.AMK_NE = ((string.IsNullOrEmpty(objModel.AMK_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.AMK_NE) && objModel.AMK_NE != valueETEST)) ? valueETEST : objModel.AMK_NE; }
                                                    if (whonetAnti == "TZP_NE") { objModel.TZP_NE = ((string.IsNullOrEmpty(objModel.TZP_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.TZP_NE) && objModel.TZP_NE != valueETEST)) ? valueETEST : objModel.TZP_NE; }
                                                    if (whonetAnti == "SAM_NE") { objModel.SAM_NE = ((string.IsNullOrEmpty(objModel.SAM_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.SAM_NE) && objModel.SAM_NE != valueETEST)) ? valueETEST : objModel.SAM_NE; }
                                                    if (whonetAnti == "CZO_NE") { objModel.CZO_NE = ((string.IsNullOrEmpty(objModel.CZO_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CZO_NE) && objModel.CZO_NE != valueETEST)) ? valueETEST : objModel.CZO_NE; }
                                                    if (whonetAnti == "CXM_NE") { objModel.CXM_NE = ((string.IsNullOrEmpty(objModel.CXM_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CXM_NE) && objModel.CXM_NE != valueETEST)) ? valueETEST : objModel.CXM_NE; }
                                                    if (whonetAnti == "CXA_NE") { objModel.CXA_NE = ((string.IsNullOrEmpty(objModel.CXA_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CXA_NE) && objModel.CXA_NE != valueETEST)) ? valueETEST : objModel.CXA_NE; }
                                                    if (whonetAnti == "AMC_NE") { objModel.AMC_NE = ((string.IsNullOrEmpty(objModel.AMC_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.AMC_NE) && objModel.AMC_NE != valueETEST)) ? valueETEST : objModel.AMC_NE; }
                                                    if (whonetAnti == "CSL_NE") { objModel.CSL_NE = ((string.IsNullOrEmpty(objModel.CSL_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CSL_NE) && objModel.CSL_NE != valueETEST)) ? valueETEST : objModel.CSL_NE; }
                                                    if (whonetAnti == "OXA_NE") { objModel.OXA_NE = ((string.IsNullOrEmpty(objModel.OXA_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.OXA_NE) && objModel.OXA_NE != valueETEST)) ? valueETEST : objModel.OXA_NE; }
                                                    if (whonetAnti == "FOX_NE") { objModel.FOX_NE = ((string.IsNullOrEmpty(objModel.FOX_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.FOX_NE) && objModel.FOX_NE != valueETEST)) ? valueETEST : objModel.FOX_NE; }
                                                    if (whonetAnti == "NOR_NE") { objModel.NOR_NE = ((string.IsNullOrEmpty(objModel.NOR_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.NOR_NE) && objModel.NOR_NE != valueETEST)) ? valueETEST : objModel.NOR_NE; }
                                                    if (whonetAnti == "GEH_NE") { objModel.GEH_NE = ((string.IsNullOrEmpty(objModel.GEH_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.GEH_NE) && objModel.GEH_NE != valueETEST)) ? valueETEST : objModel.GEH_NE; }
                                                    if (whonetAnti == "TEC_NE") { objModel.TEC_NE = ((string.IsNullOrEmpty(objModel.TEC_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.TEC_NE) && objModel.TEC_NE != valueETEST)) ? valueETEST : objModel.TEC_NE; }
                                                    if (whonetAnti == "FOS_NE") { objModel.FOS_NE = ((string.IsNullOrEmpty(objModel.FOS_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.FOS_NE) && objModel.FOS_NE != valueETEST)) ? valueETEST : objModel.FOS_NE; }
                                                    if (whonetAnti == "NIT_NE") { objModel.NIT_NE = ((string.IsNullOrEmpty(objModel.NIT_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.NIT_NE) && objModel.NIT_NE != valueETEST)) ? valueETEST : objModel.NIT_NE; }
                                                    if (whonetAnti == "SXT_NE") { objModel.SXT_NE = ((string.IsNullOrEmpty(objModel.SXT_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.SXT_NE) && objModel.SXT_NE != valueETEST)) ? valueETEST : objModel.SXT_NE; }
                                                    if (whonetAnti == "DOR_NE") { objModel.DOR_NE = ((string.IsNullOrEmpty(objModel.DOR_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.DOR_NE) && objModel.DOR_NE != valueETEST)) ? valueETEST : objModel.DOR_NE; }
                                                    if (whonetAnti == "ANI_NE") { objModel.ANI_NE = ((string.IsNullOrEmpty(objModel.ANI_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.ANI_NE) && objModel.ANI_NE != valueETEST)) ? valueETEST : objModel.ANI_NE; }
                                                    if (whonetAnti == "CAS_NE") { objModel.CAS_NE = ((string.IsNullOrEmpty(objModel.CAS_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CAS_NE) && objModel.CAS_NE != valueETEST)) ? valueETEST : objModel.CAS_NE; }
                                                    if (whonetAnti == "CDR_NE") { objModel.CDR_NE = ((string.IsNullOrEmpty(objModel.CDR_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CDR_NE) && objModel.CDR_NE != valueETEST)) ? valueETEST : objModel.CDR_NE; }
                                                    if (whonetAnti == "CPA_NE") { objModel.CPA_NE = ((string.IsNullOrEmpty(objModel.CPA_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CPA_NE) && objModel.CPA_NE != valueETEST)) ? valueETEST : objModel.CPA_NE; }
                                                    if (whonetAnti == "CZA_NE") { objModel.CZA_NE = ((string.IsNullOrEmpty(objModel.CZA_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CZA_NE) && objModel.CZA_NE != valueETEST)) ? valueETEST : objModel.CZA_NE; }
                                                    if (whonetAnti == "DOX_NE") { objModel.DOX_NE = ((string.IsNullOrEmpty(objModel.DOX_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.DOX_NE) && objModel.DOX_NE != valueETEST)) ? valueETEST : objModel.DOX_NE; }
                                                    if (whonetAnti == "FLU_NE") { objModel.FLU_NE = ((string.IsNullOrEmpty(objModel.FLU_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.FLU_NE) && objModel.FLU_NE != valueETEST)) ? valueETEST : objModel.FLU_NE; }
                                                    if (whonetAnti == "ITR_NE") { objModel.ITR_NE = ((string.IsNullOrEmpty(objModel.ITR_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.ITR_NE) && objModel.ITR_NE != valueETEST)) ? valueETEST : objModel.ITR_NE; }
                                                    if (whonetAnti == "LNZ_NE") { objModel.LNZ_NE = ((string.IsNullOrEmpty(objModel.LNZ_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.LNZ_NE) && objModel.LNZ_NE != valueETEST)) ? valueETEST : objModel.LNZ_NE; }
                                                    if (whonetAnti == "MIF_NE") { objModel.MIF_NE = ((string.IsNullOrEmpty(objModel.MIF_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.MIF_NE) && objModel.MIF_NE != valueETEST)) ? valueETEST : objModel.MIF_NE; }
                                                    if (whonetAnti == "POS_NE") { objModel.POS_NE = ((string.IsNullOrEmpty(objModel.POS_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.POS_NE) && objModel.POS_NE != valueETEST)) ? valueETEST : objModel.POS_NE; }
                                                    if (whonetAnti == "VOR_NE") { objModel.VOR_NE = ((string.IsNullOrEmpty(objModel.VOR_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.VOR_NE) && objModel.VOR_NE != valueETEST)) ? valueETEST : objModel.VOR_NE; }

                                                    if (inFile == null)
                                                    {
                                                        LabNarstList_ETEST.Add(objModel);
                                                    }                                                   
                                                }

                                            }

                                        }                                                                                                                                                                                                        
                                        #endregion                                  
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    if (!blnRowError && WARD_TYPE != "" && (result.Tables[0].Columns.Contains(WARD_TYPE)))
                                    {
                                        var ward = row[WARD_TYPE].ToString().ToUpper();
                                        if (ward == "" || !lstWardType.Contains(ward.Trim())) { blnRowError = true; }
                                    }

                                    if (selectedLab.lfu_FileType != "ETEST")
                                    {
                                        if (!blnRowError && SOURCE != "" && (result.Tables[0].Columns.Contains(SOURCE)))
                                        {
                                            var localspecimen = row[SOURCE].ToString().ToUpper();
                                            if (localspecimen == "" || !lstSpecimen.Contains(localspecimen.Trim())) { blnRowError = true; }
                                        }
                                    }


                                    if (!blnRowError && ORGANISM != "" && (result.Tables[0].Columns.Contains(ORGANISM)))
                                    {
                                        var localorganism = row[ORGANISM].ToString().ToUpper();
                                        if (localorganism == "" || !lstOrganism.Contains(localorganism.Trim())) { blnRowError = true; }
                                    }


                                    if (blnRowError) { nRowError += 1; }
                                    dataRow += 1;
                                }// end loop read data row

                                //Save Antibiotic Error
                                if (lstAntErrirCurrentYear.Count > 0)
                                {
                                    nRowError = cntErrAntiCurrentYear;
                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                    objErrorH.feh_id = Guid.NewGuid();
                                    objErrorH.feh_status = 'N';
                                    objErrorH.feh_flagdelete = false;
                                    objErrorH.feh_type = "NOT_MATCH";
                                    objErrorH.feh_field = "Antibiotic";
                                    objErrorH.feh_message = "Antibiotic not match";
                                    objErrorH.feh_errorrecord = cntErrAntiCurrentYear;
                                    objErrorH.feh_createuser = "BATCH";
                                    objErrorH.feh_createdate = DateTime.Now;
                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                    blnRowError = true;
                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);

                                    foreach (var item in lstAntErrirCurrentYear)
                                    {
                                        TRLabFileErrorDetail objErrorD = new TRLabFileErrorDetail();
                                        objErrorD.fed_id = Guid.NewGuid();
                                        objErrorD.fed_status = 'N';
                                        objErrorD.fed_localvalue = item;
                                        objErrorD.fed_feh_id = objErrorH.feh_id;
                                        objErrorD.fed_createuser = "BATCH";
                                        objErrorD.fed_createdate = DateTime.Now;

                                        var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetail(objErrorD);
                                    }
                                }

                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Header Data ... ");
                                var log_endr = new LogWriter("End Header Data ...");

                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Save Header List ... ");
                                var log_std = new LogWriter("Start Save NARST List ...");

                                if (LabNarstList.Count() != 0)
                                {
                                    LabDataDAL.InsertBulk_LabNARSTData(LabNarstList);                                
                                }

                                if (LabNarstList_DISK.Count() != 0)
                                {
                                    LabDataDAL.InsertBulk_LabNARSTData_DISK(LabNarstList_DISK);
                                }

                                if (LabNarstList_MIC.Count() != 0)
                                {                                 
                                    LabDataDAL.InsertBulk_LabNARSTData_MIC(LabNarstList_MIC);
                                }

                                if (LabNarstList_ETEST.Count() != 0)
                                {
                                    LabDataDAL.InsertBulk_LabNARSTData_ETEST(LabNarstList_ETEST);
                                }

                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Header List... ");
                                var log_endd = new LogWriter("End Save NARST List ...");
                            }

                        }                      
                        #endregion
                        //DBF + .HOS
                        #region DBF                       
                        else
                        {
                            var options = new DbfDataReaderOptions
                            {
                                Encoding = Encoding.GetEncoding(874)
                            };

                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Read File (.DBF) ... ");
                            var log_tran = new LogWriter("Read File (.DBF) ...");
                          

                            using (var dbfDataReader = new DbfDataReader.DbfDataReader(selectedLab.lfu_Path, options))
                            {
                                var x = dbfDataReader.DbfTable.Header.RecordCount;                                
                           
                                List<TRLabFileErrorDetail> AntibioticErrorD = new List<TRLabFileErrorDetail>();

                                ErrorAnt = 0;
                                dataRow = 0;
                                //int test = 0;
                                while (dbfDataReader.Read())
                                {
                                    blnRowError = false;
                                    var dbfTable = dbfDataReader.DbfTable;

                                    if (selectedLab.lfu_FileType == clsLabFileType.MLAB_FileType.ETEST)
                                    {
                                        if (dbfDataReader[AntiLocalCodeColumnEtest] == "" || dbfDataReader[AntiLocalCodeColumnEtest] == null)
                                        {
                                            continue;
                                        }
                                        string antibiotic = dbfDataReader[AntiLocalCodeColumnEtest].ToString();
                                        if (!MappingAntETEST.Any(v => v.wnm_originalfield == antibiotic))
                                        {
                                            if (!AntibioticErrorD.Any(v => v.fed_localvalue == antibiotic))
                                            {
                                                if (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST"
                                                    && dbfDataReader.GetDateTime(DATE).Year == selectedLab.lfu_DataYear)
                                                {
                                                    AntibioticErrorD.Add(new TRLabFileErrorDetail
                                                    {
                                                        fed_id = Guid.NewGuid(),
                                                        fed_status = 'N',
                                                        fed_localvalue = antibiotic,
                                                        fed_createuser = "BATCH",
                                                        fed_createdate = DateTime.Now
                                                    });
                                                    ErrorAnt++;
                                                    blnRowError = true;
                                                }

                                            }
                                            continue;
                                        }
                                    }

                                    string cmethod = "";
                                    if (dbfTable.Columns.FirstOrDefault(v => v.Name == "CMETHOD") != null)
                                        cmethod = dbfDataReader.GetString("CMETHOD");

                                    if ((cmethod == "Aerobic Culture" && selectedLab.lfu_Program == "MLAB")
                                    || selectedLab.lfu_Program != "MLAB"
                                    || (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST"))
                                    {
                                        #region InsertLabFileDataHeader

                                        if (dbfTable.Columns.FirstOrDefault(v => v.Name == LAB_NO) != null)
                                        {
                                            if (string.IsNullOrEmpty(dbfDataReader[LAB_NO].ToString())) { continue; }
                                        }
                                        if (dbfTable.Columns.FirstOrDefault(v => v.Name == ORGANISM) != null)
                                        {
                                            if (string.IsNullOrEmpty(dbfDataReader[ORGANISM].ToString())) { continue; }
                                        }
                                        if (dbfTable.Columns.FirstOrDefault(v => v.Name == DATE) != null)
                                        {
                                            try
                                            {
                                                if (string.IsNullOrEmpty(dbfDataReader.GetDateTime(DATE).ToString())) { continue; }
                                            }
                                            catch
                                            {
                                                continue;
                                            }

                                        }
                                        if (selectedLab.lfu_FileType != "ETEST")
                                        {
                                            if (dbfTable.Columns.FirstOrDefault(v => v.Name == SOURCE) != null)
                                            {
                                                if (string.IsNullOrEmpty(dbfDataReader[SOURCE].ToString())) { continue; }
                                            }
                                        }


                                        //TRSTGLabFileDataHeader objModel = new TRSTGLabFileDataHeader();
                                        //Guid ldh_id = Guid.NewGuid();

                                        //-- check date
                                        var strCDATE = dbfDataReader.GetDateTime(DATE).ToString();
                                        DateTime? dtSPEC_DATE = new DateTime();                               
                                        try
                                        {
                                            dtSPEC_DATE = dbfDataReader.GetDateTime(DATE);
                                        }
                                        catch (Exception ex)
                                        {
                                            var dd = ex.Message;
                                        }

                                        if (selectedLab.lfu_Program == "MLAB" && selectedLab.lfu_FileType == "ETEST" && dtSPEC_DATE.Value.Year != selectedLab.lfu_DataYear)
                                        {
                                            continue;
                                        }
                                        //-- end check date

                                        if (selectedLab.lfu_Program != "MLAB")
                                        {
                                            TRSTGLabNarstData objModel = new TRSTGLabNarstData();

                                            objModel.COUNTRY_A = "THA";
                                            objModel.LABORATORY = "H21";

                                            objModel.lna_lfu_id = selectedLab.lfu_id;
                                            objModel.lna_lfu_Program = selectedLab.lfu_Program;
                                            objModel.lna_lfu_FileType = selectedLab.lfu_FileType;
                                            objModel.lna_hos_code = selectedLab.lfu_hos_code;
                                            objModel.lna_lab = selectedLab.lfu_lab;
                                            objModel.lna_labno = dbfDataReader[LAB_NO].ToString();
                                            objModel.lna_organism_original_value = dbfDataReader[ORGANISM].ToString();
                                            objModel.lna_cdate = strCDATE;
                                            objModel.SPEC_DATE = dtSPEC_DATE;
                                            objModel.lna_status = "N";
                                            objModel.lna_sequence = 1; // Get Lastest
                                            objModel.lna_createuser = "BATCH";
                                            objModel.lna_createdate = DateTime.Now;

                                            if ((WARD_TYPE != "") && (dbfTable.Columns.FirstOrDefault(v => v.Name == WARD_TYPE) != null))
                                            {
                                                var ward = dbfDataReader[WARD_TYPE].ToString();
                                                if (!string.IsNullOrEmpty(ward))
                                                {
                                                    objModel.lna_wardtype_original_value = ward;
                                                }
                                            }

                                            if (SOURCE != "" && (dbfTable.Columns.FirstOrDefault(v => v.Name == SOURCE) != null))
                                            {
                                                var localspecimen = dbfDataReader[SOURCE].ToString();
                                                if (!string.IsNullOrEmpty(localspecimen))
                                                {
                                                    objModel.lna_specimen_original_value = localspecimen;
                                                }
                                            }                                         

                                            if ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name") != null)
                                                && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_originalfield)) != null))
                                            {
                                                var val = dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_originalfield].ToString();
                                                objModel.FIRST_NAME = ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_encrypt == true) ? CryptoHelper.UnicodeEncoding(val) : val);
                                            }

                                            if ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name") != null)
                                                && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_originalfield)) != null))
                                            {
                                                var val = dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_originalfield].ToString();
                                                objModel.LAST_NAME = ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_encrypt == true) ? CryptoHelper.UnicodeEncoding(val) : val);
                                            }

                                            objModel.PATIENT_ID = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number") == null)
                                                                || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number").wnm_originalfield].ToString();
                                            objModel.SEX = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex") == null)
                                                                || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex").wnm_originalfield].ToString();
                                            objModel.AGE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age") == null)
                                                                || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age").wnm_originalfield].ToString();
                                            objModel.WARD = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location") == null)
                                                                || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location").wnm_originalfield].ToString();
                                            objModel.DEPARTMENT = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Department") == null)
                                                                || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Department").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Department").wnm_originalfield].ToString();
                                            objModel.ORG_TYPE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Organism type") == null)
                                                                || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Organism type").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Organism type").wnm_originalfield].ToString();
                                            objModel.ISOL_NUM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Isolate number") == null)
                                                                || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Isolate number").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Isolate number").wnm_originalfield].ToString();
                                            objModel.NOSOCOMIAL = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Nosocomial infection") == null)
                                                                || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Nosocomial infection").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Nosocomial infection").wnm_originalfield].ToString();

                                            if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth")) != null)
                                                  && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield)) != null)
                                                  && dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield] != null)
                                            {
                                                try
                                                {
                                                    objModel.DATE_BIRTH = (DateTime)dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield];
                                                    if (objModel.DATE_BIRTH.Value.Year <= 1753 || objModel.DATE_BIRTH.Value.Year >= 9999) { objModel.DATE_BIRTH = null; }
                                                }
                                                catch
                                                {
                                                    objModel.lna_date_birth_str = dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield].ToString();
                                                }
                                            }

                                            if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry")) != null)
                                                  && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield)) != null)
                                                  && dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield] != null)
                                            {
                                                try
                                                {
                                                    objModel.DATE_DATA = (DateTime)dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield];
                                                    if (objModel.DATE_DATA.Value.Year <= 1753 || objModel.DATE_DATA.Value.Year >= 9999) { objModel.DATE_DATA = null; }
                                                }
                                                catch
                                                {

                                                }
                                            }

                                            if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission")) != null)
                                                  && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield)) != null)
                                                  && dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield] != null)
                                            {
                                                try
                                                {
                                                    objModel.DATE_ADMIS = (DateTime)dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield];
                                                    if (objModel.DATE_ADMIS.Value.Year <= 1753 || objModel.DATE_ADMIS.Value.Year >= 9999) { objModel.DATE_ADMIS = null; }
                                                }
                                                catch
                                                {

                                                }
                                            }

                                            AddAntibiotic_BaseType(dbfDataReader, whonetmapping, selectedLab.lfu_Program, objModel, null, null, dbfTable);
                                            LabNarstList.Add(objModel);
                                        }
                                        else
                                        {
                                            if(selectedLab.lfu_FileType == "DISK")
                                            {                                              
                                                TRSTGLabNarstData_DISK objModel = new TRSTGLabNarstData_DISK();

                                                objModel.COUNTRY_A = "THA";
                                                objModel.LABORATORY = "H21";

                                                objModel.lnd_lfu_id = selectedLab.lfu_id;
                                                objModel.lnd_lfu_Program = selectedLab.lfu_Program;
                                                objModel.lnd_lfu_FileType = selectedLab.lfu_FileType;
                                                objModel.lnd_hos_code = selectedLab.lfu_hos_code;
                                                objModel.lnd_lab = selectedLab.lfu_lab;
                                                objModel.lnd_labno = dbfDataReader[LAB_NO].ToString();
                                                objModel.lnd_organism_original_value = dbfDataReader[ORGANISM].ToString();
                                                objModel.lnd_cdate = strCDATE;
                                                objModel.SPEC_DATE = dtSPEC_DATE;
                                                objModel.lnd_status = "N";
                                                objModel.lnd_sequence = 1; // Get Lastest
                                                objModel.lnd_createuser = "BATCH";
                                                objModel.lnd_createdate = DateTime.Now;

                                                if ((WARD_TYPE != "") && (dbfTable.Columns.FirstOrDefault(v => v.Name == WARD_TYPE) != null))
                                                {
                                                    var ward = dbfDataReader[WARD_TYPE].ToString();
                                                    if (!string.IsNullOrEmpty(ward))
                                                    {
                                                        objModel.lnd_wardtype_original_value = ward;
                                                    }
                                                }

                                                if (SOURCE != "" && (dbfTable.Columns.FirstOrDefault(v => v.Name == SOURCE) != null))
                                                {
                                                    var localspecimen = dbfDataReader[SOURCE].ToString();
                                                    if (!string.IsNullOrEmpty(localspecimen))
                                                    {
                                                        objModel.lnd_specimen_original_value = localspecimen;
                                                    }
                                                }

                                                if ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name") != null)
                                                    && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_originalfield)) != null))
                                                {
                                                    var val = dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_originalfield].ToString();
                                                    objModel.FIRST_NAME = ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_encrypt == true) ? CryptoHelper.UnicodeEncoding(val) : val);
                                                }

                                                if ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name") != null)
                                                    && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_originalfield)) != null))
                                                {
                                                    var val = dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_originalfield].ToString();
                                                    objModel.LAST_NAME = ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_encrypt == true) ? CryptoHelper.UnicodeEncoding(val) : val);
                                                }

                                                objModel.PATIENT_ID = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number") == null) 
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number").wnm_originalfield].ToString();
                                                objModel.SEX = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex") == null) 
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex").wnm_originalfield].ToString();
                                                objModel.AGE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age") == null) 
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age").wnm_originalfield].ToString();
                                                objModel.WARD = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location") == null) 
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location").wnm_originalfield].ToString();
                                                objModel.DEPARTMENT = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Department") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Department").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Department").wnm_originalfield].ToString();
                                                objModel.ORG_TYPE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Organism type") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Organism type").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Organism type").wnm_originalfield].ToString();
                                                objModel.ISOL_NUM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Isolate number") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Isolate number").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Isolate number").wnm_originalfield].ToString();
                                                objModel.NOSOCOMIAL = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Nosocomial infection") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Nosocomial infection").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Nosocomial infection").wnm_originalfield].ToString();
                                                objModel.lnd_csource = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Local specimen code") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Local specimen code").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Local specimen code").wnm_originalfield].ToString();
                                                objModel.lnd_corganism = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Comment") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Comment").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Comment").wnm_originalfield].ToString();

                                                if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth")) != null) 
                                                      && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield)) != null)
                                                      && dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield] != null)
                                                {
                                                    try
                                                    {
                                                        objModel.DATE_BIRTH = (DateTime)dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield];
                                                        if (objModel.DATE_BIRTH.Value.Year <= 1753 || objModel.DATE_BIRTH.Value.Year >= 9999) { objModel.DATE_BIRTH = null; }
                                                    }
                                                    catch
                                                    {
                                                        objModel.lnd_date_birth_str = dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield].ToString();
                                                    }
                                                }

                                                if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry")) != null)
                                                  && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield)) != null)
                                                  && dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield] != null)
                                                {
                                                    try
                                                    {
                                                        objModel.DATE_DATA = (DateTime)dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield];
                                                        if (objModel.DATE_DATA.Value.Year <= 1753 || objModel.DATE_DATA.Value.Year >= 9999) { objModel.DATE_DATA = null; }
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }

                                                if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission")) != null)
                                                      && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield)) != null)
                                                      && dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield] != null)
                                                {
                                                    try
                                                    {
                                                        objModel.DATE_ADMIS = (DateTime)dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield];
                                                        if (objModel.DATE_ADMIS.Value.Year <= 1753 || objModel.DATE_ADMIS.Value.Year >= 9999) { objModel.DATE_ADMIS = null; }
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }

                                                AddAntibiotic_BaseType(dbfDataReader, whonetmapping, selectedLab.lfu_FileType, null, objModel, null, dbfTable);
                                                LabNarstList_DISK.Add(objModel);
                                            }

                                            else if (selectedLab.lfu_FileType == "MIC")
                                            {
                                                TRSTGLabNarstData_MIC objModel = new TRSTGLabNarstData_MIC();

                                                objModel.COUNTRY_A = "THA";
                                                objModel.LABORATORY = "H21";

                                                objModel.lnm_lfu_id = selectedLab.lfu_id;
                                                objModel.lnm_lfu_Program = selectedLab.lfu_Program;
                                                objModel.lnm_lfu_FileType = selectedLab.lfu_FileType;
                                                objModel.lnm_hos_code = selectedLab.lfu_hos_code;
                                                objModel.lnm_lab = selectedLab.lfu_lab;
                                                objModel.lnm_labno = dbfDataReader[LAB_NO].ToString();
                                                objModel.lnm_organism_original_value = dbfDataReader[ORGANISM].ToString();
                                                objModel.lnm_cdate = strCDATE;
                                                objModel.SPEC_DATE = dtSPEC_DATE;
                                                objModel.lnm_status = "N";
                                                objModel.lnm_sequence = 1; // Get Lastest
                                                objModel.lnm_createuser = "BATCH";
                                                objModel.lnm_createdate = DateTime.Now;

                                                if ((WARD_TYPE != "") && (dbfTable.Columns.FirstOrDefault(v => v.Name == WARD_TYPE) != null))
                                                {
                                                    var ward = dbfDataReader[WARD_TYPE].ToString();
                                                    if (!string.IsNullOrEmpty(ward))
                                                    {
                                                        objModel.lnm_wardtype_original_value = ward;
                                                    }
                                                }

                                                if (SOURCE != "" && (dbfTable.Columns.FirstOrDefault(v => v.Name == SOURCE) != null))
                                                {
                                                    var localspecimen = dbfDataReader[SOURCE].ToString();
                                                    if (!string.IsNullOrEmpty(localspecimen))
                                                    {
                                                        objModel.lnm_specimen_original_value = localspecimen;
                                                    }
                                                }

                                                if ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name") != null)
                                                    && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_originalfield)) != null))
                                                {
                                                    var val = dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_originalfield].ToString();
                                                    objModel.FIRST_NAME = ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "First name").wnm_encrypt == true) ? CryptoHelper.UnicodeEncoding(val) : val);
                                                }

                                                if ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name") != null)
                                                    && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_originalfield)) != null))
                                                {
                                                    var val = dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_originalfield].ToString();
                                                    objModel.LAST_NAME = ((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Last name").wnm_encrypt == true) ? CryptoHelper.UnicodeEncoding(val) : val);
                                                }
                                                objModel.PATIENT_ID = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Identification number").wnm_originalfield].ToString();
                                                objModel.SEX = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Sex").wnm_originalfield].ToString();
                                                objModel.AGE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Age").wnm_originalfield].ToString();
                                                objModel.WARD = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Location").wnm_originalfield].ToString();
                                                objModel.DEPARTMENT = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Department") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Department").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Department").wnm_originalfield].ToString();
                                                objModel.ORG_TYPE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Organism type") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Organism type").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Organism type").wnm_originalfield].ToString();
                                                objModel.ISOL_NUM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Isolate number") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Isolate number").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Isolate number").wnm_originalfield].ToString();
                                                objModel.NOSOCOMIAL = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Nosocomial infection") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Nosocomial infection").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Nosocomial infection").wnm_originalfield].ToString();
                                                objModel.lnm_csource = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Local specimen code") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Local specimen code").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Local specimen code").wnm_originalfield].ToString();
                                                objModel.lnm_corganism = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Comment") == null)
                                                                    || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Comment").wnm_originalfield)) == null) ? null : dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Comment").wnm_originalfield].ToString();
                                                
                                                if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth")) != null)
                                                      && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield)) != null)
                                                      && dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield] != null)
                                                {
                                                    try
                                                    {
                                                        objModel.DATE_BIRTH = (DateTime)dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield];
                                                        if (objModel.DATE_BIRTH.Value.Year <= 1753 || objModel.DATE_BIRTH.Value.Year >= 9999) { objModel.DATE_BIRTH = null; }
                                                    }
                                                    catch
                                                    {
                                                        objModel.lnm_date_birth_str = dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of birth").wnm_originalfield].ToString();
                                                    }
                                                }

                                                if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry")) != null)
                                                  && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield)) != null)
                                                  && dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield] != null)
                                                {
                                                    try
                                                    {
                                                        objModel.DATE_DATA = (DateTime)dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of data entry").wnm_originalfield];
                                                        if (objModel.DATE_DATA.Value.Year <= 1753 || objModel.DATE_DATA.Value.Year >= 9999) { objModel.DATE_DATA = null; }
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }

                                                if (((whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission")) != null)
                                                      && (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield)) != null)
                                                      && dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield] != null)
                                                {
                                                    try
                                                    {
                                                        objModel.DATE_ADMIS = (DateTime)dbfDataReader[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "Date of admission").wnm_originalfield];
                                                        if (objModel.DATE_ADMIS.Value.Year <= 1753 || objModel.DATE_ADMIS.Value.Year >= 9999) { objModel.DATE_ADMIS = null; }
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }

                                                AddAntibiotic_BaseType(dbfDataReader, whonetmapping, selectedLab.lfu_FileType, null, null, objModel, dbfTable);
                                                LabNarstList_MIC.Add(objModel);
                                            }

                                            else if (selectedLab.lfu_FileType == "ETEST")
                                            {
                                                TRSTGLabNarstData_ETEST objModel = new TRSTGLabNarstData_ETEST();
                                                var drugETEST = dbfDataReader[AntiLocalCodeColumnEtest].ToString(); //MICDR_ITSP
                                                var valueETEST = dbfDataReader[AntiValueColumnEtest].ToString(); //MICIT_TROU
                                              
                                                //var objCheckExistDB = LabDataDAL.CheckExist_ETEST(objModel);
                                                //if (objCheckExistDB != null)
                                                //{
                                                //    objModel = objCheckExistDB;
                                                //}

                                                var inFile = LabNarstList_ETEST.FirstOrDefault(x => x.lne_hos_code == selectedLab.lfu_hos_code
                                                                                 && x.lne_labno == dbfDataReader[LAB_NO].ToString()
                                                                                 && x.lne_organism_original_value == dbfDataReader[ORGANISM].ToString()
                                                                                 && x.lne_cdate == strCDATE);

                                                if (inFile!= null)
                                                {
                                                    objModel = inFile;
                                                    objModel.lne_updatedate = DateTime.Now;
                                                }
                                                else
                                                {
                                                    objModel.COUNTRY_A = "THA";
                                                    objModel.LABORATORY = "H21";

                                                    objModel.lne_lfu_id = selectedLab.lfu_id;
                                                    objModel.lne_lfu_Program = selectedLab.lfu_Program;
                                                    objModel.lne_lfu_FileType = selectedLab.lfu_FileType;
                                                    objModel.lne_hos_code = selectedLab.lfu_hos_code;
                                                    objModel.lne_lab = selectedLab.lfu_lab;
                                                    objModel.lne_labno = dbfDataReader[LAB_NO].ToString();
                                                    objModel.lne_organism_original_value = dbfDataReader[ORGANISM].ToString();
                                                    objModel.lne_cdate = strCDATE;
                                                    objModel.SPEC_DATE = dtSPEC_DATE;
                                                    objModel.lne_status = "N";
                                                    objModel.lne_sequence = 1; // Get Lastest
                                                    objModel.lne_createuser = "BATCH";
                                                    objModel.lne_createdate = DateTime.Now;
                                                }
                                                                                                                                                                                                                                      

                                                if ((MappingAntETEST.FirstOrDefault(x => x.wnm_originalfield == drugETEST) != null))
                                                {
                                                    var whonetAnti = MappingAntETEST.FirstOrDefault(x => x.wnm_originalfield == drugETEST).wnm_whonetfield;
                                                    if (whonetAnti == "AMX_NE") { objModel.AMX_NE = ((string.IsNullOrEmpty(objModel.AMX_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.AMX_NE) && objModel.AMX_NE != valueETEST))? valueETEST : objModel.AMX_NE;}
                                                    if (whonetAnti == "CTX_NE") { objModel.CTX_NE = ((string.IsNullOrEmpty(objModel.CTX_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CTX_NE) && objModel.CTX_NE != valueETEST))? valueETEST : objModel.CTX_NE;}
                                                    if (whonetAnti == "CAZ_NE") { objModel.CAZ_NE = ((string.IsNullOrEmpty(objModel.CAZ_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CAZ_NE) && objModel.CAZ_NE != valueETEST))? valueETEST : objModel.CAZ_NE;}
                                                    if (whonetAnti == "CZX_NE") { objModel.CZX_NE = ((string.IsNullOrEmpty(objModel.CZX_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CZX_NE) && objModel.CZX_NE != valueETEST))? valueETEST : objModel.CZX_NE;}
                                                    if (whonetAnti == "CRO_NE") { objModel.CRO_NE = ((string.IsNullOrEmpty(objModel.CRO_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CRO_NE) && objModel.CRO_NE != valueETEST))? valueETEST : objModel.CRO_NE;}
                                                    if (whonetAnti == "CHL_NE") { objModel.CHL_NE = ((string.IsNullOrEmpty(objModel.CHL_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CHL_NE) && objModel.CHL_NE != valueETEST))? valueETEST : objModel.CHL_NE;}
                                                    if (whonetAnti == "CIP_NE") { objModel.CIP_NE = ((string.IsNullOrEmpty(objModel.CIP_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CIP_NE) && objModel.CIP_NE != valueETEST))? valueETEST : objModel.CIP_NE;}
                                                    if (whonetAnti == "CLI_NE") { objModel.CLI_NE = ((string.IsNullOrEmpty(objModel.CLI_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CLI_NE) && objModel.CLI_NE != valueETEST))? valueETEST : objModel.CLI_NE;}
                                                    if (whonetAnti == "COL_NE") { objModel.COL_NE = ((string.IsNullOrEmpty(objModel.COL_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.COL_NE) && objModel.COL_NE != valueETEST))? valueETEST : objModel.COL_NE;}
                                                    if (whonetAnti == "DAP_NE") { objModel.DAP_NE = ((string.IsNullOrEmpty(objModel.DAP_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.DAP_NE) && objModel.DAP_NE != valueETEST))? valueETEST : objModel.DAP_NE;}
                                                    if (whonetAnti == "ETP_NE") { objModel.ETP_NE = ((string.IsNullOrEmpty(objModel.ETP_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.ETP_NE) && objModel.ETP_NE != valueETEST))? valueETEST : objModel.ETP_NE;}
                                                    if (whonetAnti == "ERY_NE") { objModel.ERY_NE = ((string.IsNullOrEmpty(objModel.ERY_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.ERY_NE) && objModel.ERY_NE != valueETEST))? valueETEST : objModel.ERY_NE;}
                                                    if (whonetAnti == "IPM_NE") { objModel.IPM_NE = ((string.IsNullOrEmpty(objModel.IPM_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.IPM_NE) && objModel.IPM_NE != valueETEST))? valueETEST : objModel.IPM_NE;}
                                                    if (whonetAnti == "MEM_NE") { objModel.MEM_NE = ((string.IsNullOrEmpty(objModel.MEM_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.MEM_NE) && objModel.MEM_NE != valueETEST))? valueETEST : objModel.MEM_NE;}
                                                    if (whonetAnti == "NET_NE") { objModel.NET_NE = ((string.IsNullOrEmpty(objModel.NET_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.NET_NE) && objModel.NET_NE != valueETEST))? valueETEST : objModel.NET_NE;}
                                                    if (whonetAnti == "PEN_NE") { objModel.PEN_NE = ((string.IsNullOrEmpty(objModel.PEN_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.PEN_NE) && objModel.PEN_NE != valueETEST))? valueETEST : objModel.PEN_NE;}
                                                    if (whonetAnti == "VAN_NE") { objModel.VAN_NE = ((string.IsNullOrEmpty(objModel.VAN_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.VAN_NE) && objModel.VAN_NE != valueETEST))? valueETEST : objModel.VAN_NE;}
                                                    if (whonetAnti == "AZM_NE") { objModel.AZM_NE = ((string.IsNullOrEmpty(objModel.AZM_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.AZM_NE) && objModel.AZM_NE != valueETEST))? valueETEST : objModel.AZM_NE;}
                                                    if (whonetAnti == "GEN_NE") { objModel.GEN_NE = ((string.IsNullOrEmpty(objModel.GEN_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.GEN_NE) && objModel.GEN_NE != valueETEST))? valueETEST : objModel.GEN_NE;}
                                                    if (whonetAnti == "TCY_NE") { objModel.TCY_NE = ((string.IsNullOrEmpty(objModel.TCY_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.TCY_NE) && objModel.TCY_NE != valueETEST))? valueETEST : objModel.TCY_NE;}
                                                    if (whonetAnti == "CFM_NE") { objModel.CFM_NE = ((string.IsNullOrEmpty(objModel.CFM_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CFM_NE) && objModel.CFM_NE != valueETEST))? valueETEST : objModel.CFM_NE;}
                                                    if (whonetAnti == "AMP_NE") { objModel.AMP_NE = ((string.IsNullOrEmpty(objModel.AMP_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.AMP_NE) && objModel.AMP_NE != valueETEST))? valueETEST : objModel.AMP_NE;}
                                                    if (whonetAnti == "FEP_NE") { objModel.FEP_NE = ((string.IsNullOrEmpty(objModel.FEP_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.FEP_NE) && objModel.FEP_NE != valueETEST))? valueETEST : objModel.FEP_NE;}
                                                    if (whonetAnti == "LVX_NE") { objModel.LVX_NE = ((string.IsNullOrEmpty(objModel.LVX_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.LVX_NE) && objModel.LVX_NE != valueETEST))? valueETEST : objModel.LVX_NE;}
                                                    if (whonetAnti == "AMK_NE") { objModel.AMK_NE = ((string.IsNullOrEmpty(objModel.AMK_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.AMK_NE) && objModel.AMK_NE != valueETEST))? valueETEST : objModel.AMK_NE;}
                                                    if (whonetAnti == "TZP_NE") { objModel.TZP_NE = ((string.IsNullOrEmpty(objModel.TZP_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.TZP_NE) && objModel.TZP_NE != valueETEST))? valueETEST : objModel.TZP_NE;}
                                                    if (whonetAnti == "SAM_NE") { objModel.SAM_NE = ((string.IsNullOrEmpty(objModel.SAM_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.SAM_NE) && objModel.SAM_NE != valueETEST))? valueETEST : objModel.SAM_NE;}
                                                    if (whonetAnti == "CZO_NE") { objModel.CZO_NE = ((string.IsNullOrEmpty(objModel.CZO_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CZO_NE) && objModel.CZO_NE != valueETEST))? valueETEST : objModel.CZO_NE;}
                                                    if (whonetAnti == "CXM_NE") { objModel.CXM_NE = ((string.IsNullOrEmpty(objModel.CXM_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CXM_NE) && objModel.CXM_NE != valueETEST))? valueETEST : objModel.CXM_NE;}
                                                    if (whonetAnti == "CXA_NE") { objModel.CXA_NE = ((string.IsNullOrEmpty(objModel.CXA_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CXA_NE) && objModel.CXA_NE != valueETEST))? valueETEST : objModel.CXA_NE;}
                                                    if (whonetAnti == "AMC_NE") { objModel.AMC_NE = ((string.IsNullOrEmpty(objModel.AMC_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.AMC_NE) && objModel.AMC_NE != valueETEST))? valueETEST : objModel.AMC_NE;}
                                                    if (whonetAnti == "CSL_NE") { objModel.CSL_NE = ((string.IsNullOrEmpty(objModel.CSL_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CSL_NE) && objModel.CSL_NE != valueETEST))? valueETEST : objModel.CSL_NE;}
                                                    if (whonetAnti == "OXA_NE") { objModel.OXA_NE = ((string.IsNullOrEmpty(objModel.OXA_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.OXA_NE) && objModel.OXA_NE != valueETEST))? valueETEST : objModel.OXA_NE;}
                                                    if (whonetAnti == "FOX_NE") { objModel.FOX_NE = ((string.IsNullOrEmpty(objModel.FOX_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.FOX_NE) && objModel.FOX_NE != valueETEST))? valueETEST : objModel.FOX_NE;}
                                                    if (whonetAnti == "NOR_NE") { objModel.NOR_NE = ((string.IsNullOrEmpty(objModel.NOR_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.NOR_NE) && objModel.NOR_NE != valueETEST))? valueETEST : objModel.NOR_NE;}
                                                    if (whonetAnti == "GEH_NE") { objModel.GEH_NE = ((string.IsNullOrEmpty(objModel.GEH_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.GEH_NE) && objModel.GEH_NE != valueETEST))? valueETEST : objModel.GEH_NE;}
                                                    if (whonetAnti == "TEC_NE") { objModel.TEC_NE = ((string.IsNullOrEmpty(objModel.TEC_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.TEC_NE) && objModel.TEC_NE != valueETEST))? valueETEST : objModel.TEC_NE;}
                                                    if (whonetAnti == "FOS_NE") { objModel.FOS_NE = ((string.IsNullOrEmpty(objModel.FOS_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.FOS_NE) && objModel.FOS_NE != valueETEST))? valueETEST : objModel.FOS_NE;}
                                                    if (whonetAnti == "NIT_NE") { objModel.NIT_NE = ((string.IsNullOrEmpty(objModel.NIT_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.NIT_NE) && objModel.NIT_NE != valueETEST))? valueETEST : objModel.NIT_NE;}
                                                    if (whonetAnti == "SXT_NE") { objModel.SXT_NE = ((string.IsNullOrEmpty(objModel.SXT_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.SXT_NE) && objModel.SXT_NE != valueETEST))? valueETEST : objModel.SXT_NE;}
                                                    if (whonetAnti == "DOR_NE") { objModel.DOR_NE = ((string.IsNullOrEmpty(objModel.DOR_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.DOR_NE) && objModel.DOR_NE != valueETEST))? valueETEST : objModel.DOR_NE;}
                                                    if (whonetAnti == "ANI_NE") { objModel.ANI_NE = ((string.IsNullOrEmpty(objModel.ANI_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.ANI_NE) && objModel.ANI_NE != valueETEST))? valueETEST : objModel.ANI_NE;}
                                                    if (whonetAnti == "CAS_NE") { objModel.CAS_NE = ((string.IsNullOrEmpty(objModel.CAS_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CAS_NE) && objModel.CAS_NE != valueETEST))? valueETEST : objModel.CAS_NE;}
                                                    if (whonetAnti == "CDR_NE") { objModel.CDR_NE = ((string.IsNullOrEmpty(objModel.CDR_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CDR_NE) && objModel.CDR_NE != valueETEST))? valueETEST : objModel.CDR_NE;}
                                                    if (whonetAnti == "CPA_NE") { objModel.CPA_NE = ((string.IsNullOrEmpty(objModel.CPA_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CPA_NE) && objModel.CPA_NE != valueETEST))? valueETEST : objModel.CPA_NE;}
                                                    if (whonetAnti == "CZA_NE") { objModel.CZA_NE = ((string.IsNullOrEmpty(objModel.CZA_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.CZA_NE) && objModel.CZA_NE != valueETEST))? valueETEST : objModel.CZA_NE;}
                                                    if (whonetAnti == "DOX_NE") { objModel.DOX_NE = ((string.IsNullOrEmpty(objModel.DOX_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.DOX_NE) && objModel.DOX_NE != valueETEST))? valueETEST : objModel.DOX_NE;}
                                                    if (whonetAnti == "FLU_NE") { objModel.FLU_NE = ((string.IsNullOrEmpty(objModel.FLU_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.FLU_NE) && objModel.FLU_NE != valueETEST))? valueETEST : objModel.FLU_NE;}
                                                    if (whonetAnti == "ITR_NE") { objModel.ITR_NE = ((string.IsNullOrEmpty(objModel.ITR_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.ITR_NE) && objModel.ITR_NE != valueETEST))? valueETEST : objModel.ITR_NE;}
                                                    if (whonetAnti == "LNZ_NE") { objModel.LNZ_NE = ((string.IsNullOrEmpty(objModel.LNZ_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.LNZ_NE) && objModel.LNZ_NE != valueETEST))? valueETEST : objModel.LNZ_NE;}
                                                    if (whonetAnti == "MIF_NE") { objModel.MIF_NE = ((string.IsNullOrEmpty(objModel.MIF_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.MIF_NE) && objModel.MIF_NE != valueETEST))? valueETEST : objModel.MIF_NE;}
                                                    if (whonetAnti == "POS_NE") { objModel.POS_NE = ((string.IsNullOrEmpty(objModel.POS_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.POS_NE) && objModel.POS_NE != valueETEST))? valueETEST : objModel.POS_NE;}
                                                    if (whonetAnti == "VOR_NE") { objModel.VOR_NE = ((string.IsNullOrEmpty(objModel.VOR_NE) && !string.IsNullOrEmpty(valueETEST)) || (!string.IsNullOrEmpty(objModel.VOR_NE) && objModel.VOR_NE != valueETEST))? valueETEST : objModel.VOR_NE;}

                                                    if (inFile == null)
                                                    {
                                                        LabNarstList_ETEST.Add(objModel);
                                                    }
                                                        
                                                }
                                            }
                                        }
                                    
                                        #endregion
                                                                    
                                    }

                                    if (!blnRowError && WARD_TYPE != "" && (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == WARD_TYPE) != null))
                                    {
                                        var ward = dbfDataReader[WARD_TYPE].ToString().ToUpper();
                                        if (ward == "" || !lstWardType.Contains(ward.Trim())) { blnRowError = true; }
                                    }

                                    if (!blnRowError && SOURCE != "" && (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == SOURCE) != null))
                                    {
                                        var localspecimen = dbfDataReader[SOURCE].ToString().ToUpper();
                                        if (localspecimen == "" || !lstSpecimen.Contains(localspecimen.Trim())) { blnRowError = true; }
                                    }

                                    if (!blnRowError && ORGANISM != "" && (dbfDataReader.DbfTable.Columns.FirstOrDefault(v => v.Name == ORGANISM) != null))
                                    {
                                        var localorganism = dbfDataReader[ORGANISM].ToString().ToUpper();
                                        if (localorganism == "" || !lstOrganism.Contains(localorganism.Trim())) { blnRowError = true; }
                                    }

                                    if (blnRowError) { nRowError += 1; }
                                    dataRow += 1;

                                } //EndWhile:
                            
                               
                                if (AntibioticErrorD.Count > 0)
                                {
                                    //nRowError += ErrorAnt;
                                    //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Start Save Error Header ... ");
                                    TRLabFileErrorHeader objErrorH = new TRLabFileErrorHeader();
                                    objErrorH.feh_id = Guid.NewGuid();
                                    objErrorH.feh_status = 'N';
                                    objErrorH.feh_flagdelete = false;
                                    objErrorH.feh_type = "NOT_MATCH";
                                    objErrorH.feh_field = "Antibiotic";
                                    objErrorH.feh_message = "Antibiotic not match";
                                    objErrorH.feh_errorrecord = ErrorAnt;
                                    objErrorH.feh_createuser = "BATCH";
                                    objErrorH.feh_createdate = DateTime.Now;
                                    objErrorH.feh_lfu_id = selectedLab.lfu_id;
                                    var objTRLabFileErrorHeader = LabDataDAL.Save_TRLabFileErrorHeader(objErrorH);
                                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Save Error Header ... ");

                                    AntibioticErrorD = AntibioticErrorD.Select(a => { a.fed_feh_id = objErrorH.feh_id; return a; }).ToList();
                                    var objTRLabFileErrorDetail = LabDataDAL.Save_TRLabFileErrorDetailList(AntibioticErrorD);

                                }                                                             

                                if (LabNarstList.Count() != 0)
                                {
                                    LabDataDAL.InsertBulk_LabNARSTData(LabNarstList);
                                }

                                if (LabNarstList_DISK.Count() != 0)
                                {
                                    LabDataDAL.InsertBulk_LabNARSTData_DISK(LabNarstList_DISK);
                                }

                                if (LabNarstList_MIC.Count() != 0)
                                {
                                    LabDataDAL.InsertBulk_LabNARSTData_MIC(LabNarstList_MIC);
                                }

                                if (LabNarstList_ETEST.Count() != 0)
                                {
                                    LabDataDAL.InsertBulk_LabNARSTData_ETEST(LabNarstList_ETEST);
                                }
                            }

                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : End Read File (.DBF) ... ");
                            var log_tranend = new LogWriter("End Read File (.DBF) ...");
                        }
                        #endregion

                    Finish:
                        char str = 'M';                        
                        var rowsAffected2 = LabDataDAL.Update_LabFileUploadStatus(selectedLab.lfu_id.ToString(), str, nRowError, "BATCH");

                        var log_ed = new LogWriter(">> End File Name : " + selectedLab.lfu_FileName);
                    }
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : Batch Lab End ... ");
                    var log_end = new LogWriter("Batch Lab Complete ...");

                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Timeout Expired"))
                {
                    var rowsTimeOut = LabDataDAL.Update_LabFileUploadStatus(iFileUploadID.ToString(), 'T', 0, "BATCH");
                }
                else
                {
                    var exMsg = ex.Message.Substring(0, (ex.Message.Length > 200) ? 200 : ex.Message.Length);
                    var rowsAffected = LabDataDAL.Update_LabFileUploadStatus(iFileUploadID.ToString(), 'I', 0, "BATCH", exMsg);
                }

                var d = string.Format("Row : {0} Detail : {1}", (dataRow + 1).ToString(), ex.Message);

                var logw = new LogWriter(d);
                Console.WriteLine(d);
            }
        }

        public void AddAntibiotic(DataRow row, List<WHONetMappingListsDTO> whonetmapping , string fileType
            , TRSTGLabNarstData objAll , TRSTGLabNarstData_DISK objDisk, TRSTGLabNarstData_MIC objMIC, DataTable dt)
        {
 
            if (fileType == "WHONET" || fileType == "OTHER")
            {
                objAll.ANI_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_ND").wnm_originalfield].ToString();
                objAll.AXS_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AXS_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AXS_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AXS_ND").wnm_originalfield].ToString();
                objAll.BIA_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_ND").wnm_originalfield].ToString();
                objAll.CAP_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAP_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAP_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAP_ND").wnm_originalfield].ToString();
                objAll.CNX_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CNX_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CNX_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CNX_ND").wnm_originalfield].ToString();
                objAll.CYC_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CYC_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CYC_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CYC_ND").wnm_originalfield].ToString();
                objAll.ETH_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETH_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETH_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETH_ND").wnm_originalfield].ToString();
                objAll.ETI_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETI_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETI_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETI_ND").wnm_originalfield].ToString();
                objAll.INH_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "INH_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "INH_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "INH_ND").wnm_originalfield].ToString();
                objAll.PAS_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PAS_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PAS_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PAS_ND").wnm_originalfield].ToString();
                objAll.CFM_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_ND5").wnm_originalfield].ToString();
                objAll.CIP_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_ND5").wnm_originalfield].ToString();
                objAll.CLI_ND2 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_ND2") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_ND2").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_ND2").wnm_originalfield].ToString();
                objAll.LVX_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND5").wnm_originalfield].ToString();
                objAll.OFX_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_ND5").wnm_originalfield].ToString();
                objAll.OXA_ND1 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_ND1") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_ND1").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_ND1").wnm_originalfield].ToString();
                objAll.CAS_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_ND5").wnm_originalfield].ToString();
                objAll.CLR_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND5").wnm_originalfield].ToString();
                objAll.CDR_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_ND5").wnm_originalfield].ToString();
                objAll.CLO_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLO_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLO_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLO_ND5").wnm_originalfield].ToString();
                objAll.OPT_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OPT_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OPT_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OPT_ND5").wnm_originalfield].ToString();
                objAll.PEF_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEF_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEF_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEF_ND5").wnm_originalfield].ToString();
                objAll.POS_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_ND5").wnm_originalfield].ToString();
                objAll.NOV_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND5").wnm_originalfield].ToString();
                objAll.LVX_ND1 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND1") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND1").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND1").wnm_originalfield].ToString();
                objAll.MET_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_ND5").wnm_originalfield].ToString();
                objAll.MFX_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_ND5").wnm_originalfield].ToString();
                objAll.RIF_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_ND5").wnm_originalfield].ToString();
                objAll.VOR_ND1 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_ND1") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_ND1").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_ND1").wnm_originalfield].ToString();
                objAll.AMK_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_ND30").wnm_originalfield].ToString();
                objAll.AMC_ND20 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND20") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND20").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND20").wnm_originalfield].ToString();
                objAll.AMP_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_ND10").wnm_originalfield].ToString();
                objAll.SAM_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_ND10").wnm_originalfield].ToString();
                objAll.AZM_ND15 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_ND15") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_ND15").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_ND15").wnm_originalfield].ToString();
                objAll.CZO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_ND30").wnm_originalfield].ToString();
                objAll.FEP_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_ND30").wnm_originalfield].ToString();
                objAll.CSL_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_ND30").wnm_originalfield].ToString();
                objAll.CTX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_ND30").wnm_originalfield].ToString();
                objAll.FOX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_ND30").wnm_originalfield].ToString();
                objAll.CAZ_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_ND30").wnm_originalfield].ToString();
                objAll.CZX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_ND30").wnm_originalfield].ToString();
                objAll.CRO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_ND30").wnm_originalfield].ToString();
                objAll.CXA_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_ND30").wnm_originalfield].ToString();
                objAll.CXM_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_ND30").wnm_originalfield].ToString();
                objAll.CHL_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_ND30").wnm_originalfield].ToString();
                objAll.COL_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_ND10").wnm_originalfield].ToString();
                objAll.DAP_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_ND30").wnm_originalfield].ToString();
                objAll.DOR_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_ND10").wnm_originalfield].ToString();
                objAll.ETP_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_ND10").wnm_originalfield].ToString();
                objAll.ERY_ND15 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_ND15") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_ND15").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_ND15").wnm_originalfield].ToString();
                objAll.FUS_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FUS_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FUS_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FUS_ND10").wnm_originalfield].ToString();
                objAll.GEN_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_ND10").wnm_originalfield].ToString();
                objAll.IPM_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_ND10").wnm_originalfield].ToString();
                objAll.MEM_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_ND10").wnm_originalfield].ToString();
                objAll.NAL_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_ND30").wnm_originalfield].ToString();
                objAll.NET_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_ND30").wnm_originalfield].ToString();
                objAll.NOR_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_ND10").wnm_originalfield].ToString();
                objAll.PEN_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_ND10").wnm_originalfield].ToString();
                objAll.STR_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STR_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STR_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STR_ND10").wnm_originalfield].ToString();
                objAll.TEC_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_ND30").wnm_originalfield].ToString();
                objAll.TCY_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_ND30").wnm_originalfield].ToString();
                objAll.VAN_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_ND30").wnm_originalfield].ToString();
                objAll.AMC_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND30").wnm_originalfield].ToString();
                objAll.AMX_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND10").wnm_originalfield].ToString();
                objAll.AMX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND30").wnm_originalfield].ToString();
                objAll.ATM_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_ND30").wnm_originalfield].ToString();
                objAll.CCV_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_ND30").wnm_originalfield].ToString();
                objAll.CEP_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_ND30").wnm_originalfield].ToString();
                objAll.CFP_ND75 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_ND75") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_ND75").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_ND75").wnm_originalfield].ToString();
                objAll.CLR_ND15 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND15") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND15").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND15").wnm_originalfield].ToString();
                objAll.CPD_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPD_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPD_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPD_ND10").wnm_originalfield].ToString();
                objAll.CPO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_ND30").wnm_originalfield].ToString();
                objAll.CPT_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_ND30").wnm_originalfield].ToString();
                objAll.CTC_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTC_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTC_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTC_ND30").wnm_originalfield].ToString();
                objAll.CZT_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_ND30").wnm_originalfield].ToString();
                objAll.DOX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_ND30").wnm_originalfield].ToString();
                objAll.FLU_ND25 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_ND25") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_ND25").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_ND25").wnm_originalfield].ToString();
                objAll.FOS_ND50 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND50") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND50").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND50").wnm_originalfield].ToString();
                objAll.KAN_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "KAN_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "KAN_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "KAN_ND30").wnm_originalfield].ToString();
                objAll.PIS_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIS_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIS_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIS_ND30").wnm_originalfield].ToString();
                objAll.NEO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NEO_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NEO_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NEO_ND30").wnm_originalfield].ToString();
                objAll.MIF_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_ND10").wnm_originalfield].ToString();
                objAll.TOB_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_ND10").wnm_originalfield].ToString();
                objAll.FOS_ND200 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND200") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND200").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND200").wnm_originalfield].ToString();
                objAll.NOV_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND100") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND100").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND100").wnm_originalfield].ToString();
                objAll.POP_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POP_ND300") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POP_ND300").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POP_ND300").wnm_originalfield].ToString();
                objAll.SPT_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SPT_ND100") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SPT_ND100").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SPT_ND100").wnm_originalfield].ToString();
                objAll.PIP_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIP_ND100") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIP_ND100").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIP_ND100").wnm_originalfield].ToString();
                objAll.TZP_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_ND100") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_ND100").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_ND100").wnm_originalfield].ToString();
                objAll.POL_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POL_ND300") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POL_ND300").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POL_ND300").wnm_originalfield].ToString();
                objAll.STH_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STH_ND300") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STH_ND300").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STH_ND300").wnm_originalfield].ToString();
                objAll.GEH_ND120 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_ND120") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_ND120").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_ND120").wnm_originalfield].ToString();
                objAll.NIT_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_ND300") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_ND300").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_ND300").wnm_originalfield].ToString();
                objAll.SXT_ND1_2 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_ND1_2") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_ND1_2").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_ND1_2").wnm_originalfield].ToString();


                objAll.CTX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NM").wnm_originalfield].ToString();
                objAll.CAZ_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NM").wnm_originalfield].ToString();
                objAll.CZX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NM").wnm_originalfield].ToString();
                objAll.CRO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NM").wnm_originalfield].ToString();
                objAll.CHL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NM").wnm_originalfield].ToString();
                objAll.CIP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NM").wnm_originalfield].ToString();
                objAll.CLI_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NM").wnm_originalfield].ToString();
                objAll.COL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NM").wnm_originalfield].ToString();
                objAll.DAP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NM").wnm_originalfield].ToString();
                objAll.ETP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NM").wnm_originalfield].ToString();
                objAll.ERY_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NM").wnm_originalfield].ToString();
                objAll.IPM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NM").wnm_originalfield].ToString();
                objAll.MEM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NM").wnm_originalfield].ToString();
                objAll.NET_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NM").wnm_originalfield].ToString();
                objAll.PEN_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NM").wnm_originalfield].ToString();
                objAll.VAN_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NM").wnm_originalfield].ToString();
                objAll.GEN_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NM").wnm_originalfield].ToString();
                objAll.TCY_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NM").wnm_originalfield].ToString();
                objAll.CXM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NM").wnm_originalfield].ToString();
                objAll.AMP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NM").wnm_originalfield].ToString();
                objAll.FEP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NM").wnm_originalfield].ToString();
                objAll.LVX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NM").wnm_originalfield].ToString();
                objAll.AMK_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NM").wnm_originalfield].ToString();
                objAll.TZP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NM").wnm_originalfield].ToString();
                objAll.SAM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NM").wnm_originalfield].ToString();
                objAll.CZO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NM").wnm_originalfield].ToString();
                objAll.AZM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NM").wnm_originalfield].ToString();
                objAll.CXA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NM").wnm_originalfield].ToString();
                objAll.AMC_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NM").wnm_originalfield].ToString();
                objAll.CSL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NM").wnm_originalfield].ToString();
                objAll.OXA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NM").wnm_originalfield].ToString();
                objAll.FOX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NM").wnm_originalfield].ToString();
                objAll.NOR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NM").wnm_originalfield].ToString();
                objAll.GEH_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NM").wnm_originalfield].ToString();
                objAll.TEC_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NM").wnm_originalfield].ToString();
                objAll.FOS_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NM").wnm_originalfield].ToString();
                objAll.NIT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NM").wnm_originalfield].ToString();
                objAll.SXT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NM").wnm_originalfield].ToString();
                objAll.DOR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NM").wnm_originalfield].ToString();
                objAll.AMX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NM").wnm_originalfield].ToString();
                objAll.ATM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_NM").wnm_originalfield].ToString();
                objAll.BIA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_NM").wnm_originalfield].ToString();
                objAll.CAS_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NM").wnm_originalfield].ToString();
                objAll.CCV_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_NM").wnm_originalfield].ToString();
                objAll.CEC_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEC_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEC_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEC_NM").wnm_originalfield].ToString();
                objAll.CEP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_NM").wnm_originalfield].ToString();
                objAll.CFP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_NM").wnm_originalfield].ToString();
                objAll.CID_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CID_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CID_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CID_NM").wnm_originalfield].ToString();
                objAll.CLR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_NM").wnm_originalfield].ToString();
                objAll.CMZ_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CMZ_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CMZ_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CMZ_NM").wnm_originalfield].ToString();
                objAll.CPO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_NM").wnm_originalfield].ToString();
                objAll.CPT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_NM").wnm_originalfield].ToString();
                objAll.CZA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NM").wnm_originalfield].ToString();
                objAll.CZT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_NM").wnm_originalfield].ToString();
                objAll.DOX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NM").wnm_originalfield].ToString();
                objAll.FLU_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NM").wnm_originalfield].ToString();
                objAll.GEM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEM_NM").wnm_originalfield].ToString();
                objAll.ITR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NM").wnm_originalfield].ToString();
                objAll.LND_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LND_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LND_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LND_NM").wnm_originalfield].ToString();
                objAll.LNZ_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NM").wnm_originalfield].ToString();
                objAll.MET_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_NM").wnm_originalfield].ToString();
                objAll.MFX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_NM").wnm_originalfield].ToString();
                objAll.MIF_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NM").wnm_originalfield].ToString();
                objAll.MNO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MNO_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MNO_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MNO_NM").wnm_originalfield].ToString();
                objAll.MUP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MUP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MUP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MUP_NM").wnm_originalfield].ToString();
                objAll.NAL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_NM").wnm_originalfield].ToString();
                objAll.OFX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_NM").wnm_originalfield].ToString();
                objAll.POS_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NM").wnm_originalfield].ToString();
                objAll.QDA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "QDA_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "QDA_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "QDA_NM").wnm_originalfield].ToString();
                objAll.RIF_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_NM").wnm_originalfield].ToString();
                objAll.TMP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TMP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TMP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TMP_NM").wnm_originalfield].ToString();
                objAll.TOB_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_NM").wnm_originalfield].ToString();
                objAll.VOR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NM").wnm_originalfield].ToString();

                objAll.AMX_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NE").wnm_originalfield].ToString();
                objAll.CTX_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NE").wnm_originalfield].ToString();
                objAll.CAZ_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NE").wnm_originalfield].ToString();
                objAll.CZX_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NE").wnm_originalfield].ToString();
                objAll.CRO_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NE").wnm_originalfield].ToString();
                objAll.CHL_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NE").wnm_originalfield].ToString();
                objAll.CIP_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NE").wnm_originalfield].ToString();
                objAll.CLI_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NE").wnm_originalfield].ToString();
                objAll.COL_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NE").wnm_originalfield].ToString();
                objAll.DAP_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NE").wnm_originalfield].ToString();
                objAll.ETP_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NE").wnm_originalfield].ToString();
                objAll.ERY_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NE").wnm_originalfield].ToString();
                objAll.IPM_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NE").wnm_originalfield].ToString();
                objAll.MEM_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NE").wnm_originalfield].ToString();
                objAll.NET_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NE").wnm_originalfield].ToString();
                objAll.PEN_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NE").wnm_originalfield].ToString();
                objAll.VAN_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NE").wnm_originalfield].ToString();
                objAll.AZM_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NE").wnm_originalfield].ToString();
                objAll.GEN_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NE").wnm_originalfield].ToString();
                objAll.TCY_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NE").wnm_originalfield].ToString();
                objAll.CFM_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_NE").wnm_originalfield].ToString();
                objAll.AMP_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NE").wnm_originalfield].ToString();
                objAll.FEP_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NE").wnm_originalfield].ToString();
                objAll.LVX_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NE").wnm_originalfield].ToString();
                objAll.AMK_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NE").wnm_originalfield].ToString();
                objAll.TZP_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NE").wnm_originalfield].ToString();
                objAll.SAM_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NE").wnm_originalfield].ToString();
                objAll.CZO_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NE").wnm_originalfield].ToString();
                objAll.CXM_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NE").wnm_originalfield].ToString();
                objAll.CXA_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NE").wnm_originalfield].ToString();
                objAll.AMC_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NE").wnm_originalfield].ToString();
                objAll.CSL_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NE").wnm_originalfield].ToString();
                objAll.OXA_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NE").wnm_originalfield].ToString();
                objAll.FOX_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NE").wnm_originalfield].ToString();
                objAll.NOR_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NE").wnm_originalfield].ToString();
                objAll.GEH_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NE").wnm_originalfield].ToString();
                objAll.TEC_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NE").wnm_originalfield].ToString();
                objAll.FOS_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NE").wnm_originalfield].ToString();
                objAll.NIT_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NE").wnm_originalfield].ToString();
                objAll.SXT_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NE").wnm_originalfield].ToString();
                objAll.DOR_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NE").wnm_originalfield].ToString();
                objAll.ANI_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_NE").wnm_originalfield].ToString();
                objAll.CAS_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NE").wnm_originalfield].ToString();
                objAll.CDR_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_NE").wnm_originalfield].ToString();
                objAll.CPA_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPA_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPA_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPA_NE").wnm_originalfield].ToString();
                objAll.CZA_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NE").wnm_originalfield].ToString();
                objAll.DOX_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NE").wnm_originalfield].ToString();
                objAll.FLU_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NE").wnm_originalfield].ToString();
                objAll.ITR_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NE").wnm_originalfield].ToString();
                objAll.LNZ_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NE").wnm_originalfield].ToString();
                objAll.MIF_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NE").wnm_originalfield].ToString();
                objAll.POS_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NE").wnm_originalfield].ToString();
                objAll.VOR_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NE") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NE").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NE").wnm_originalfield].ToString();

            }

            else if (fileType == "DISK")
            {
                objDisk.ANI_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_ND").wnm_originalfield].ToString();
                objDisk.AXS_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AXS_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AXS_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AXS_ND").wnm_originalfield].ToString();
                objDisk.BIA_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_ND").wnm_originalfield].ToString();
                objDisk.CAP_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAP_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAP_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAP_ND").wnm_originalfield].ToString();
                objDisk.CNX_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CNX_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CNX_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CNX_ND").wnm_originalfield].ToString();
                objDisk.CYC_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CYC_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CYC_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CYC_ND").wnm_originalfield].ToString();
                objDisk.ETH_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETH_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETH_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETH_ND").wnm_originalfield].ToString();
                objDisk.ETI_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETI_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETI_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETI_ND").wnm_originalfield].ToString();
                objDisk.INH_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "INH_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "INH_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "INH_ND").wnm_originalfield].ToString();
                objDisk.PAS_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PAS_ND") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PAS_ND").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PAS_ND").wnm_originalfield].ToString();

                objDisk.CFM_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_ND5").wnm_originalfield].ToString();
                objDisk.CIP_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_ND5").wnm_originalfield].ToString();
                objDisk.CLI_ND2 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_ND2") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_ND2").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_ND2").wnm_originalfield].ToString();
                objDisk.LVX_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND5").wnm_originalfield].ToString();
                objDisk.OFX_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_ND5").wnm_originalfield].ToString();
                objDisk.OXA_ND1 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_ND1") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_ND1").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_ND1").wnm_originalfield].ToString();
                objDisk.CAS_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_ND5").wnm_originalfield].ToString();
                objDisk.CLR_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND5").wnm_originalfield].ToString();
                objDisk.CDR_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_ND5").wnm_originalfield].ToString();
                objDisk.CLO_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLO_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLO_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLO_ND5").wnm_originalfield].ToString();
                objDisk.OPT_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OPT_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OPT_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OPT_ND5").wnm_originalfield].ToString();
                objDisk.PEF_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEF_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEF_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEF_ND5").wnm_originalfield].ToString();
                objDisk.POS_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_ND5").wnm_originalfield].ToString();
                objDisk.NOV_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND5").wnm_originalfield].ToString();
                objDisk.LVX_ND1 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND1") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND1").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND1").wnm_originalfield].ToString();
                objDisk.MET_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_ND5").wnm_originalfield].ToString();
                objDisk.MFX_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_ND5").wnm_originalfield].ToString();
                objDisk.RIF_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_ND5") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_ND5").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_ND5").wnm_originalfield].ToString();
                objDisk.VOR_ND1 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_ND1") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_ND1").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_ND1").wnm_originalfield].ToString();

                objDisk.AMK_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_ND30").wnm_originalfield].ToString();
                objDisk.AMC_ND20 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND20") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND20").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND20").wnm_originalfield].ToString();
                objDisk.AMP_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_ND10").wnm_originalfield].ToString();
                objDisk.SAM_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_ND10").wnm_originalfield].ToString();
                objDisk.AZM_ND15 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_ND15") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_ND15").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_ND15").wnm_originalfield].ToString();
                objDisk.CZO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_ND30").wnm_originalfield].ToString();
                objDisk.FEP_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_ND30").wnm_originalfield].ToString();
                objDisk.CSL_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_ND30").wnm_originalfield].ToString();
                objDisk.CTX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_ND30").wnm_originalfield].ToString();
                objDisk.FOX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_ND30").wnm_originalfield].ToString();
                objDisk.CAZ_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_ND30").wnm_originalfield].ToString();
                objDisk.CZX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_ND30").wnm_originalfield].ToString();
                objDisk.CRO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_ND30").wnm_originalfield].ToString();
                objDisk.CXA_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_ND30").wnm_originalfield].ToString();
                objDisk.CXM_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_ND30").wnm_originalfield].ToString();
                objDisk.CHL_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_ND30").wnm_originalfield].ToString();
                objDisk.COL_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_ND10").wnm_originalfield].ToString();
                objDisk.DAP_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_ND30").wnm_originalfield].ToString();
                objDisk.DOR_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_ND10").wnm_originalfield].ToString();
                objDisk.ETP_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_ND10").wnm_originalfield].ToString();
                objDisk.ERY_ND15 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_ND15") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_ND15").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_ND15").wnm_originalfield].ToString();
                objDisk.FUS_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FUS_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FUS_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FUS_ND10").wnm_originalfield].ToString();
                objDisk.GEN_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_ND10").wnm_originalfield].ToString();
                objDisk.IPM_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_ND10").wnm_originalfield].ToString();
                objDisk.MEM_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_ND10").wnm_originalfield].ToString();
                objDisk.NAL_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_ND30").wnm_originalfield].ToString();
                objDisk.NET_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_ND30").wnm_originalfield].ToString();
                objDisk.NOR_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_ND10").wnm_originalfield].ToString();
                objDisk.PEN_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_ND10").wnm_originalfield].ToString();
                objDisk.STR_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STR_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STR_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STR_ND10").wnm_originalfield].ToString();
                objDisk.TEC_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_ND30").wnm_originalfield].ToString();
                objDisk.TCY_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_ND30").wnm_originalfield].ToString();
                objDisk.VAN_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_ND30").wnm_originalfield].ToString();
                objDisk.AMC_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND30").wnm_originalfield].ToString();
                objDisk.AMX_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND10").wnm_originalfield].ToString();
                objDisk.AMX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND30").wnm_originalfield].ToString();
                objDisk.ATM_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_ND30").wnm_originalfield].ToString();
                objDisk.CCV_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_ND30").wnm_originalfield].ToString();
                objDisk.CEP_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_ND30").wnm_originalfield].ToString();
                objDisk.CFP_ND75 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_ND75") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_ND75").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_ND75").wnm_originalfield].ToString();
                objDisk.CLR_ND15 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND15") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND15").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND15").wnm_originalfield].ToString();
                objDisk.CPD_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPD_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPD_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPD_ND10").wnm_originalfield].ToString();
                objDisk.CPO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_ND30").wnm_originalfield].ToString();
                objDisk.CPT_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_ND30").wnm_originalfield].ToString();
                objDisk.CTC_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTC_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTC_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTC_ND30").wnm_originalfield].ToString();
                objDisk.CZT_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_ND30").wnm_originalfield].ToString();
                objDisk.DOX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_ND30").wnm_originalfield].ToString();
                objDisk.FLU_ND25 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_ND25") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_ND25").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_ND25").wnm_originalfield].ToString();
                objDisk.FOS_ND50 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND50") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND50").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND50").wnm_originalfield].ToString();
                objDisk.KAN_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "KAN_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "KAN_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "KAN_ND30").wnm_originalfield].ToString();
                objDisk.PIS_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIS_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIS_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIS_ND30").wnm_originalfield].ToString();
                objDisk.NEO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NEO_ND30") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NEO_ND30").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NEO_ND30").wnm_originalfield].ToString();
                objDisk.MIF_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_ND10").wnm_originalfield].ToString();
                objDisk.TOB_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_ND10") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_ND10").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_ND10").wnm_originalfield].ToString();

                objDisk.FOS_ND200 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND200") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND200").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND200").wnm_originalfield].ToString();
                objDisk.NOV_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND100") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND100").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND100").wnm_originalfield].ToString();
                objDisk.POP_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POP_ND300") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POP_ND300").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POP_ND300").wnm_originalfield].ToString();
                objDisk.SPT_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SPT_ND100") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SPT_ND100").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SPT_ND100").wnm_originalfield].ToString();
                objDisk.PIP_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIP_ND100") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIP_ND100").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIP_ND100").wnm_originalfield].ToString();
                objDisk.TZP_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_ND100") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_ND100").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_ND100").wnm_originalfield].ToString();
                objDisk.POL_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POL_ND300") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POL_ND300").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POL_ND300").wnm_originalfield].ToString();
                objDisk.STH_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STH_ND300") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STH_ND300").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STH_ND300").wnm_originalfield].ToString();
                objDisk.GEH_ND120 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_ND120") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_ND120").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_ND120").wnm_originalfield].ToString();
                objDisk.NIT_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_ND300") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_ND300").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_ND300").wnm_originalfield].ToString();
                objDisk.SXT_ND1_2 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_ND1_2") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_ND1_2").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_ND1_2").wnm_originalfield].ToString();

            }

            else if (fileType == "MIC")
            {
                objMIC.CTX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NM").wnm_originalfield].ToString();
                objMIC.CAZ_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NM").wnm_originalfield].ToString();
                objMIC.CZX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NM").wnm_originalfield].ToString();
                objMIC.CRO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NM").wnm_originalfield].ToString();
                objMIC.CHL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NM").wnm_originalfield].ToString();
                objMIC.CIP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NM").wnm_originalfield].ToString();
                objMIC.CLI_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NM").wnm_originalfield].ToString();
                objMIC.COL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NM").wnm_originalfield].ToString();
                objMIC.DAP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NM").wnm_originalfield].ToString();
                objMIC.ETP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NM").wnm_originalfield].ToString();
                objMIC.ERY_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NM").wnm_originalfield].ToString();
                objMIC.IPM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NM").wnm_originalfield].ToString();
                objMIC.MEM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NM").wnm_originalfield].ToString();
                objMIC.NET_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NM").wnm_originalfield].ToString();
                objMIC.PEN_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NM").wnm_originalfield].ToString();
                objMIC.VAN_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NM").wnm_originalfield].ToString();
                objMIC.GEN_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NM").wnm_originalfield].ToString();
                objMIC.TCY_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NM").wnm_originalfield].ToString();
                objMIC.CXM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NM").wnm_originalfield].ToString();
                objMIC.AMP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NM").wnm_originalfield].ToString();
                objMIC.FEP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NM").wnm_originalfield].ToString();
                objMIC.LVX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NM").wnm_originalfield].ToString();
                objMIC.AMK_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NM").wnm_originalfield].ToString();
                objMIC.TZP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NM").wnm_originalfield].ToString();
                objMIC.SAM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NM").wnm_originalfield].ToString();
                objMIC.CZO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NM").wnm_originalfield].ToString();
                objMIC.AZM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NM").wnm_originalfield].ToString();
                objMIC.CXA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NM").wnm_originalfield].ToString();
                objMIC.AMC_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NM").wnm_originalfield].ToString();
                objMIC.CSL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NM").wnm_originalfield].ToString();
                objMIC.OXA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NM").wnm_originalfield].ToString();
                objMIC.FOX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NM").wnm_originalfield].ToString();
                objMIC.NOR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NM").wnm_originalfield].ToString();
                objMIC.GEH_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NM").wnm_originalfield].ToString();
                objMIC.TEC_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NM").wnm_originalfield].ToString();
                objMIC.FOS_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NM").wnm_originalfield].ToString();
                objMIC.NIT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NM").wnm_originalfield].ToString();
                objMIC.SXT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NM").wnm_originalfield].ToString();
                objMIC.DOR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NM").wnm_originalfield].ToString();
                objMIC.AMX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NM").wnm_originalfield].ToString();
                objMIC.ATM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_NM").wnm_originalfield].ToString();
                objMIC.BIA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_NM").wnm_originalfield].ToString();
                objMIC.CAS_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NM").wnm_originalfield].ToString();
                objMIC.CCV_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_NM").wnm_originalfield].ToString();
                objMIC.CEC_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEC_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEC_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEC_NM").wnm_originalfield].ToString();
                objMIC.CEP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_NM").wnm_originalfield].ToString();
                objMIC.CFP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_NM").wnm_originalfield].ToString();
                objMIC.CID_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CID_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CID_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CID_NM").wnm_originalfield].ToString();
                objMIC.CLR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_NM").wnm_originalfield].ToString();
                objMIC.CMZ_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CMZ_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CMZ_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CMZ_NM").wnm_originalfield].ToString();
                objMIC.CPO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_NM").wnm_originalfield].ToString();
                objMIC.CPT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_NM").wnm_originalfield].ToString();
                objMIC.CZA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NM").wnm_originalfield].ToString();
                objMIC.CZT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_NM").wnm_originalfield].ToString();
                objMIC.DOX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NM").wnm_originalfield].ToString();
                objMIC.FLU_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NM").wnm_originalfield].ToString();
                objMIC.GEM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEM_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEM_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEM_NM").wnm_originalfield].ToString();
                objMIC.ITR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NM").wnm_originalfield].ToString();
                objMIC.LND_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LND_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LND_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LND_NM").wnm_originalfield].ToString();
                objMIC.LNZ_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NM").wnm_originalfield].ToString();
                objMIC.MET_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_NM").wnm_originalfield].ToString();
                objMIC.MFX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_NM").wnm_originalfield].ToString();
                objMIC.MIF_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NM").wnm_originalfield].ToString();
                objMIC.MNO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MNO_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MNO_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MNO_NM").wnm_originalfield].ToString();
                objMIC.MUP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MUP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MUP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MUP_NM").wnm_originalfield].ToString();
                objMIC.NAL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_NM").wnm_originalfield].ToString();
                objMIC.OFX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_NM").wnm_originalfield].ToString();
                objMIC.POS_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NM").wnm_originalfield].ToString();
                objMIC.QDA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "QDA_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "QDA_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "QDA_NM").wnm_originalfield].ToString();
                objMIC.RIF_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_NM").wnm_originalfield].ToString();
                objMIC.TMP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TMP_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TMP_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TMP_NM").wnm_originalfield].ToString();
                objMIC.TOB_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_NM").wnm_originalfield].ToString();
                objMIC.VOR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NM") == null) || (!dt.Columns.Contains(whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NM").wnm_originalfield)) ? null : row[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NM").wnm_originalfield].ToString();

            }

        }

        public void AddAntibiotic_BaseType(DbfDataReader.DbfDataReader dbfRow, List<WHONetMappingListsDTO> whonetmapping, string fileType
           , TRSTGLabNarstData objAll, TRSTGLabNarstData_DISK objDisk, TRSTGLabNarstData_MIC objMIC, DbfTable dbfTable)
        {

            if (fileType == "WHONET" || fileType == "OTHER")
            {
                objAll.ANI_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_ND").wnm_originalfield].ToString();
                objAll.AXS_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AXS_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AXS_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AXS_ND").wnm_originalfield].ToString();
                objAll.BIA_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_ND").wnm_originalfield].ToString();
                objAll.CAP_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAP_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAP_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAP_ND").wnm_originalfield].ToString();
                objAll.CNX_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CNX_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CNX_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CNX_ND").wnm_originalfield].ToString();
                objAll.CYC_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CYC_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CYC_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CYC_ND").wnm_originalfield].ToString();
                objAll.ETH_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETH_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETH_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETH_ND").wnm_originalfield].ToString();
                objAll.ETI_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETI_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETI_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETI_ND").wnm_originalfield].ToString();
                objAll.INH_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "INH_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "INH_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "INH_ND").wnm_originalfield].ToString();
                objAll.PAS_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PAS_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PAS_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PAS_ND").wnm_originalfield].ToString();
                objAll.CFM_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_ND5").wnm_originalfield].ToString();
                objAll.CIP_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_ND5").wnm_originalfield].ToString();
                objAll.CLI_ND2 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_ND2") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_ND2").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_ND2").wnm_originalfield].ToString();
                objAll.LVX_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND5").wnm_originalfield].ToString();
                objAll.OFX_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_ND5").wnm_originalfield].ToString();
                objAll.OXA_ND1 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_ND1") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_ND1").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_ND1").wnm_originalfield].ToString();
                objAll.CAS_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_ND5").wnm_originalfield].ToString();
                objAll.CLR_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND5").wnm_originalfield].ToString();
                objAll.CDR_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_ND5").wnm_originalfield].ToString();
                objAll.CLO_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLO_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLO_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLO_ND5").wnm_originalfield].ToString();
                objAll.OPT_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OPT_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OPT_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OPT_ND5").wnm_originalfield].ToString();
                objAll.PEF_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEF_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEF_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEF_ND5").wnm_originalfield].ToString();
                objAll.POS_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_ND5").wnm_originalfield].ToString();
                objAll.NOV_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND5").wnm_originalfield].ToString();
                objAll.LVX_ND1 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND1") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND1").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND1").wnm_originalfield].ToString();
                objAll.MET_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_ND5").wnm_originalfield].ToString();
                objAll.MFX_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_ND5").wnm_originalfield].ToString();
                objAll.RIF_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_ND5").wnm_originalfield].ToString();
                objAll.VOR_ND1 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_ND1") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_ND1").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_ND1").wnm_originalfield].ToString();
                objAll.AMK_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_ND30").wnm_originalfield].ToString();
                objAll.AMC_ND20 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND20") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND20").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND20").wnm_originalfield].ToString();
                objAll.AMP_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_ND10").wnm_originalfield].ToString();
                objAll.SAM_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_ND10").wnm_originalfield].ToString();
                objAll.AZM_ND15 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_ND15") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_ND15").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_ND15").wnm_originalfield].ToString();
                objAll.CZO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_ND30").wnm_originalfield].ToString();
                objAll.FEP_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_ND30").wnm_originalfield].ToString();
                objAll.CSL_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_ND30").wnm_originalfield].ToString();
                objAll.CTX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_ND30").wnm_originalfield].ToString();
                objAll.FOX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_ND30").wnm_originalfield].ToString();
                objAll.CAZ_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_ND30").wnm_originalfield].ToString();
                objAll.CZX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_ND30").wnm_originalfield].ToString();
                objAll.CRO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_ND30").wnm_originalfield].ToString();
                objAll.CXA_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_ND30").wnm_originalfield].ToString();
                objAll.CXM_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_ND30").wnm_originalfield].ToString();
                objAll.CHL_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_ND30").wnm_originalfield].ToString();
                objAll.COL_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_ND10").wnm_originalfield].ToString();
                objAll.DAP_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_ND30").wnm_originalfield].ToString();
                objAll.DOR_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_ND10").wnm_originalfield].ToString();
                objAll.ETP_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_ND10").wnm_originalfield].ToString();
                objAll.ERY_ND15 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_ND15") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_ND15").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_ND15").wnm_originalfield].ToString();
                objAll.FUS_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FUS_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FUS_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FUS_ND10").wnm_originalfield].ToString();
                objAll.GEN_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_ND10").wnm_originalfield].ToString();
                objAll.IPM_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_ND10").wnm_originalfield].ToString();
                objAll.MEM_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_ND10").wnm_originalfield].ToString();
                objAll.NAL_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_ND30").wnm_originalfield].ToString();
                objAll.NET_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_ND30").wnm_originalfield].ToString();
                objAll.NOR_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_ND10").wnm_originalfield].ToString();
                objAll.PEN_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_ND10").wnm_originalfield].ToString();
                objAll.STR_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STR_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STR_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STR_ND10").wnm_originalfield].ToString();
                objAll.TEC_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_ND30").wnm_originalfield].ToString();
                objAll.TCY_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_ND30").wnm_originalfield].ToString();
                objAll.VAN_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_ND30").wnm_originalfield].ToString();
                objAll.AMC_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND30").wnm_originalfield].ToString();
                objAll.AMX_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND10").wnm_originalfield].ToString();
                objAll.AMX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND30").wnm_originalfield].ToString();
                objAll.ATM_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_ND30").wnm_originalfield].ToString();
                objAll.CCV_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_ND30").wnm_originalfield].ToString();
                objAll.CEP_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_ND30").wnm_originalfield].ToString();
                objAll.CFP_ND75 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_ND75") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_ND75").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_ND75").wnm_originalfield].ToString();
                objAll.CLR_ND15 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND15") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND15").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND15").wnm_originalfield].ToString();
                objAll.CPD_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPD_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPD_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPD_ND10").wnm_originalfield].ToString();
                objAll.CPO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_ND30").wnm_originalfield].ToString();
                objAll.CPT_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_ND30").wnm_originalfield].ToString();
                objAll.CTC_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTC_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTC_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTC_ND30").wnm_originalfield].ToString();
                objAll.CZT_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_ND30").wnm_originalfield].ToString();
                objAll.DOX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_ND30").wnm_originalfield].ToString();
                objAll.FLU_ND25 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_ND25") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_ND25").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_ND25").wnm_originalfield].ToString();
                objAll.FOS_ND50 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND50") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND50").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND50").wnm_originalfield].ToString();
                objAll.KAN_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "KAN_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "KAN_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "KAN_ND30").wnm_originalfield].ToString();
                objAll.PIS_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIS_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIS_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIS_ND30").wnm_originalfield].ToString();
                objAll.NEO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NEO_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NEO_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NEO_ND30").wnm_originalfield].ToString();
                objAll.MIF_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_ND10").wnm_originalfield].ToString();
                objAll.TOB_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_ND10").wnm_originalfield].ToString();
                objAll.FOS_ND200 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND200") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND200").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND200").wnm_originalfield].ToString();
                objAll.NOV_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND100") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND100").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND100").wnm_originalfield].ToString();
                objAll.POP_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POP_ND300") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POP_ND300").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POP_ND300").wnm_originalfield].ToString();
                objAll.SPT_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SPT_ND100") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SPT_ND100").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SPT_ND100").wnm_originalfield].ToString();
                objAll.PIP_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIP_ND100") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIP_ND100").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIP_ND100").wnm_originalfield].ToString();
                objAll.TZP_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_ND100") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_ND100").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_ND100").wnm_originalfield].ToString();
                objAll.POL_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POL_ND300") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POL_ND300").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POL_ND300").wnm_originalfield].ToString();
                objAll.STH_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STH_ND300") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STH_ND300").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STH_ND300").wnm_originalfield].ToString();
                objAll.GEH_ND120 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_ND120") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_ND120").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_ND120").wnm_originalfield].ToString();
                objAll.NIT_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_ND300") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_ND300").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_ND300").wnm_originalfield].ToString();
                objAll.SXT_ND1_2 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_ND1_2") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_ND1_2").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_ND1_2").wnm_originalfield].ToString();


                objAll.CTX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NM").wnm_originalfield].ToString();
                objAll.CAZ_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NM").wnm_originalfield].ToString();
                objAll.CZX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NM").wnm_originalfield].ToString();
                objAll.CRO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NM").wnm_originalfield].ToString();
                objAll.CHL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NM").wnm_originalfield].ToString();
                objAll.CIP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NM").wnm_originalfield].ToString();
                objAll.CLI_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NM").wnm_originalfield].ToString();
                objAll.COL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NM").wnm_originalfield].ToString();
                objAll.DAP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NM").wnm_originalfield].ToString();
                objAll.ETP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NM").wnm_originalfield].ToString();
                objAll.ERY_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NM").wnm_originalfield].ToString();
                objAll.IPM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NM").wnm_originalfield].ToString();
                objAll.MEM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NM").wnm_originalfield].ToString();
                objAll.NET_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NM").wnm_originalfield].ToString();
                objAll.PEN_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NM").wnm_originalfield].ToString();
                objAll.VAN_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NM").wnm_originalfield].ToString();
                objAll.GEN_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NM").wnm_originalfield].ToString();
                objAll.TCY_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NM").wnm_originalfield].ToString();
                objAll.CXM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NM").wnm_originalfield].ToString();
                objAll.AMP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NM").wnm_originalfield].ToString();
                objAll.FEP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NM").wnm_originalfield].ToString();
                objAll.LVX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NM").wnm_originalfield].ToString();
                objAll.AMK_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NM").wnm_originalfield].ToString();
                objAll.TZP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NM").wnm_originalfield].ToString();
                objAll.SAM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NM").wnm_originalfield].ToString();
                objAll.CZO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NM").wnm_originalfield].ToString();
                objAll.AZM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NM").wnm_originalfield].ToString();
                objAll.CXA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NM").wnm_originalfield].ToString();
                objAll.AMC_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NM").wnm_originalfield].ToString();
                objAll.CSL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NM").wnm_originalfield].ToString();
                objAll.OXA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NM").wnm_originalfield].ToString();
                objAll.FOX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NM").wnm_originalfield].ToString();
                objAll.NOR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NM").wnm_originalfield].ToString();
                objAll.GEH_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NM").wnm_originalfield].ToString();
                objAll.TEC_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NM").wnm_originalfield].ToString();
                objAll.FOS_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NM").wnm_originalfield].ToString();
                objAll.NIT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NM").wnm_originalfield].ToString();
                objAll.SXT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NM").wnm_originalfield].ToString();
                objAll.DOR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NM").wnm_originalfield].ToString();
                objAll.AMX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NM").wnm_originalfield].ToString();
                objAll.ATM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_NM").wnm_originalfield].ToString();
                objAll.BIA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_NM").wnm_originalfield].ToString();
                objAll.CAS_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NM").wnm_originalfield].ToString();
                objAll.CCV_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_NM").wnm_originalfield].ToString();
                objAll.CEC_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEC_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEC_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEC_NM").wnm_originalfield].ToString();
                objAll.CEP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_NM").wnm_originalfield].ToString();
                objAll.CFP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_NM").wnm_originalfield].ToString();
                objAll.CID_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CID_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CID_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CID_NM").wnm_originalfield].ToString();
                objAll.CLR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_NM").wnm_originalfield].ToString();
                objAll.CMZ_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CMZ_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CMZ_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CMZ_NM").wnm_originalfield].ToString();
                objAll.CPO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_NM").wnm_originalfield].ToString();
                objAll.CPT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_NM").wnm_originalfield].ToString();
                objAll.CZA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NM").wnm_originalfield].ToString();
                objAll.CZT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_NM").wnm_originalfield].ToString();
                objAll.DOX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NM").wnm_originalfield].ToString();
                objAll.FLU_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NM").wnm_originalfield].ToString();
                objAll.GEM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEM_NM").wnm_originalfield].ToString();
                objAll.ITR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NM").wnm_originalfield].ToString();
                objAll.LND_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LND_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LND_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LND_NM").wnm_originalfield].ToString();
                objAll.LNZ_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NM").wnm_originalfield].ToString();
                objAll.MET_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_NM").wnm_originalfield].ToString();
                objAll.MFX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_NM").wnm_originalfield].ToString();
                objAll.MIF_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NM").wnm_originalfield].ToString();
                objAll.MNO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MNO_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MNO_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MNO_NM").wnm_originalfield].ToString();
                objAll.MUP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MUP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MUP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MUP_NM").wnm_originalfield].ToString();
                objAll.NAL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_NM").wnm_originalfield].ToString();
                objAll.OFX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_NM").wnm_originalfield].ToString();
                objAll.POS_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NM").wnm_originalfield].ToString();
                objAll.QDA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "QDA_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "QDA_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "QDA_NM").wnm_originalfield].ToString();
                objAll.RIF_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_NM").wnm_originalfield].ToString();
                objAll.TMP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TMP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TMP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TMP_NM").wnm_originalfield].ToString();
                objAll.TOB_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_NM").wnm_originalfield].ToString();
                objAll.VOR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NM").wnm_originalfield].ToString();

                objAll.AMX_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NE").wnm_originalfield].ToString();
                objAll.CTX_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NE").wnm_originalfield].ToString();
                objAll.CAZ_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NE").wnm_originalfield].ToString();
                objAll.CZX_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NE").wnm_originalfield].ToString();
                objAll.CRO_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NE").wnm_originalfield].ToString();
                objAll.CHL_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NE").wnm_originalfield].ToString();
                objAll.CIP_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NE").wnm_originalfield].ToString();
                objAll.CLI_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NE").wnm_originalfield].ToString();
                objAll.COL_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NE").wnm_originalfield].ToString();
                objAll.DAP_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NE").wnm_originalfield].ToString();
                objAll.ETP_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NE").wnm_originalfield].ToString();
                objAll.ERY_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NE").wnm_originalfield].ToString();
                objAll.IPM_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NE").wnm_originalfield].ToString();
                objAll.MEM_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NE").wnm_originalfield].ToString();
                objAll.NET_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NE").wnm_originalfield].ToString();
                objAll.PEN_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NE").wnm_originalfield].ToString();
                objAll.VAN_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NE").wnm_originalfield].ToString();
                objAll.AZM_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NE").wnm_originalfield].ToString();
                objAll.GEN_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NE").wnm_originalfield].ToString();
                objAll.TCY_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NE").wnm_originalfield].ToString();
                objAll.CFM_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_NE").wnm_originalfield].ToString();
                objAll.AMP_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NE").wnm_originalfield].ToString();
                objAll.FEP_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NE").wnm_originalfield].ToString();
                objAll.LVX_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NE").wnm_originalfield].ToString();
                objAll.AMK_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NE").wnm_originalfield].ToString();
                objAll.TZP_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NE").wnm_originalfield].ToString();
                objAll.SAM_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NE").wnm_originalfield].ToString();
                objAll.CZO_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NE").wnm_originalfield].ToString();
                objAll.CXM_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NE").wnm_originalfield].ToString();
                objAll.CXA_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NE").wnm_originalfield].ToString();
                objAll.AMC_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NE").wnm_originalfield].ToString();
                objAll.CSL_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NE").wnm_originalfield].ToString();
                objAll.OXA_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NE").wnm_originalfield].ToString();
                objAll.FOX_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NE").wnm_originalfield].ToString();
                objAll.NOR_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NE").wnm_originalfield].ToString();
                objAll.GEH_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NE").wnm_originalfield].ToString();
                objAll.TEC_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NE").wnm_originalfield].ToString();
                objAll.FOS_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NE").wnm_originalfield].ToString();
                objAll.NIT_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NE").wnm_originalfield].ToString();
                objAll.SXT_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NE").wnm_originalfield].ToString();
                objAll.DOR_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NE").wnm_originalfield].ToString();
                objAll.ANI_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_NE").wnm_originalfield].ToString();
                objAll.CAS_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NE").wnm_originalfield].ToString();
                objAll.CDR_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_NE").wnm_originalfield].ToString();
                objAll.CPA_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPA_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPA_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPA_NE").wnm_originalfield].ToString();
                objAll.CZA_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NE").wnm_originalfield].ToString();
                objAll.DOX_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NE").wnm_originalfield].ToString();
                objAll.FLU_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NE").wnm_originalfield].ToString();
                objAll.ITR_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NE").wnm_originalfield].ToString();
                objAll.LNZ_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NE").wnm_originalfield].ToString();
                objAll.MIF_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NE").wnm_originalfield].ToString();
                objAll.POS_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NE").wnm_originalfield].ToString();
                objAll.VOR_NE = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NE") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NE").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NE").wnm_originalfield].ToString();

            }

            else if (fileType == "DISK")
            {
                objDisk.ANI_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ANI_ND").wnm_originalfield].ToString();
                objDisk.AXS_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AXS_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AXS_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AXS_ND").wnm_originalfield].ToString();
                objDisk.BIA_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_ND").wnm_originalfield].ToString();
                objDisk.CAP_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAP_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAP_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAP_ND").wnm_originalfield].ToString();
                objDisk.CNX_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CNX_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CNX_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CNX_ND").wnm_originalfield].ToString();
                objDisk.CYC_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CYC_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CYC_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CYC_ND").wnm_originalfield].ToString();
                objDisk.ETH_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETH_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETH_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETH_ND").wnm_originalfield].ToString();
                objDisk.ETI_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETI_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETI_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETI_ND").wnm_originalfield].ToString();
                objDisk.INH_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "INH_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "INH_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "INH_ND").wnm_originalfield].ToString();
                objDisk.PAS_ND = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PAS_ND") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PAS_ND").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PAS_ND").wnm_originalfield].ToString();

                objDisk.CFM_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFM_ND5").wnm_originalfield].ToString();
                objDisk.CIP_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_ND5").wnm_originalfield].ToString();
                objDisk.CLI_ND2 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_ND2") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_ND2").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_ND2").wnm_originalfield].ToString();
                objDisk.LVX_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND5").wnm_originalfield].ToString();
                objDisk.OFX_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_ND5").wnm_originalfield].ToString();
                objDisk.OXA_ND1 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_ND1") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_ND1").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_ND1").wnm_originalfield].ToString();
                objDisk.CAS_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_ND5").wnm_originalfield].ToString();
                objDisk.CLR_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND5").wnm_originalfield].ToString();
                objDisk.CDR_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CDR_ND5").wnm_originalfield].ToString();
                objDisk.CLO_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLO_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLO_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLO_ND5").wnm_originalfield].ToString();
                objDisk.OPT_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OPT_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OPT_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OPT_ND5").wnm_originalfield].ToString();
                objDisk.PEF_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEF_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEF_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEF_ND5").wnm_originalfield].ToString();
                objDisk.POS_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_ND5").wnm_originalfield].ToString();
                objDisk.NOV_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND5").wnm_originalfield].ToString();
                objDisk.LVX_ND1 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND1") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND1").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_ND1").wnm_originalfield].ToString();
                objDisk.MET_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_ND5").wnm_originalfield].ToString();
                objDisk.MFX_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_ND5").wnm_originalfield].ToString();
                objDisk.RIF_ND5 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_ND5") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_ND5").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_ND5").wnm_originalfield].ToString();
                objDisk.VOR_ND1 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_ND1") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_ND1").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_ND1").wnm_originalfield].ToString();

                objDisk.AMK_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_ND30").wnm_originalfield].ToString();
                objDisk.AMC_ND20 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND20") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND20").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND20").wnm_originalfield].ToString();
                objDisk.AMP_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_ND10").wnm_originalfield].ToString();
                objDisk.SAM_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_ND10").wnm_originalfield].ToString();
                objDisk.AZM_ND15 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_ND15") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_ND15").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_ND15").wnm_originalfield].ToString();
                objDisk.CZO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_ND30").wnm_originalfield].ToString();
                objDisk.FEP_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_ND30").wnm_originalfield].ToString();
                objDisk.CSL_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_ND30").wnm_originalfield].ToString();
                objDisk.CTX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_ND30").wnm_originalfield].ToString();
                objDisk.FOX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_ND30").wnm_originalfield].ToString();
                objDisk.CAZ_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_ND30").wnm_originalfield].ToString();
                objDisk.CZX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_ND30").wnm_originalfield].ToString();
                objDisk.CRO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_ND30").wnm_originalfield].ToString();
                objDisk.CXA_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_ND30").wnm_originalfield].ToString();
                objDisk.CXM_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_ND30").wnm_originalfield].ToString();
                objDisk.CHL_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_ND30").wnm_originalfield].ToString();
                objDisk.COL_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_ND10").wnm_originalfield].ToString();
                objDisk.DAP_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_ND30").wnm_originalfield].ToString();
                objDisk.DOR_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_ND10").wnm_originalfield].ToString();
                objDisk.ETP_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_ND10").wnm_originalfield].ToString();
                objDisk.ERY_ND15 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_ND15") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_ND15").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_ND15").wnm_originalfield].ToString();
                objDisk.FUS_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FUS_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FUS_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FUS_ND10").wnm_originalfield].ToString();
                objDisk.GEN_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_ND10").wnm_originalfield].ToString();
                objDisk.IPM_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_ND10").wnm_originalfield].ToString();
                objDisk.MEM_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_ND10").wnm_originalfield].ToString();
                objDisk.NAL_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_ND30").wnm_originalfield].ToString();
                objDisk.NET_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_ND30").wnm_originalfield].ToString();
                objDisk.NOR_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_ND10").wnm_originalfield].ToString();
                objDisk.PEN_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_ND10").wnm_originalfield].ToString();
                objDisk.STR_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STR_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STR_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STR_ND10").wnm_originalfield].ToString();
                objDisk.TEC_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_ND30").wnm_originalfield].ToString();
                objDisk.TCY_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_ND30").wnm_originalfield].ToString();
                objDisk.VAN_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_ND30").wnm_originalfield].ToString();
                objDisk.AMC_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_ND30").wnm_originalfield].ToString();
                objDisk.AMX_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND10").wnm_originalfield].ToString();
                objDisk.AMX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_ND30").wnm_originalfield].ToString();
                objDisk.ATM_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_ND30").wnm_originalfield].ToString();
                objDisk.CCV_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_ND30").wnm_originalfield].ToString();
                objDisk.CEP_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_ND30").wnm_originalfield].ToString();
                objDisk.CFP_ND75 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_ND75") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_ND75").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_ND75").wnm_originalfield].ToString();
                objDisk.CLR_ND15 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND15") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND15").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_ND15").wnm_originalfield].ToString();
                objDisk.CPD_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPD_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPD_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPD_ND10").wnm_originalfield].ToString();
                objDisk.CPO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_ND30").wnm_originalfield].ToString();
                objDisk.CPT_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_ND30").wnm_originalfield].ToString();
                objDisk.CTC_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTC_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTC_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTC_ND30").wnm_originalfield].ToString();
                objDisk.CZT_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_ND30").wnm_originalfield].ToString();
                objDisk.DOX_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_ND30").wnm_originalfield].ToString();
                objDisk.FLU_ND25 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_ND25") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_ND25").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_ND25").wnm_originalfield].ToString();
                objDisk.FOS_ND50 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND50") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND50").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND50").wnm_originalfield].ToString();
                objDisk.KAN_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "KAN_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "KAN_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "KAN_ND30").wnm_originalfield].ToString();
                objDisk.PIS_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIS_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIS_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIS_ND30").wnm_originalfield].ToString();
                objDisk.NEO_ND30 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NEO_ND30") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NEO_ND30").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NEO_ND30").wnm_originalfield].ToString();
                objDisk.MIF_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_ND10").wnm_originalfield].ToString();
                objDisk.TOB_ND10 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_ND10") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_ND10").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_ND10").wnm_originalfield].ToString();

                objDisk.FOS_ND200 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND200") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND200").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_ND200").wnm_originalfield].ToString();
                objDisk.NOV_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND100") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND100").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOV_ND100").wnm_originalfield].ToString();
                objDisk.POP_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POP_ND300") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POP_ND300").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POP_ND300").wnm_originalfield].ToString();
                objDisk.SPT_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SPT_ND100") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SPT_ND100").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SPT_ND100").wnm_originalfield].ToString();
                objDisk.PIP_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIP_ND100") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIP_ND100").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PIP_ND100").wnm_originalfield].ToString();
                objDisk.TZP_ND100 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_ND100") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_ND100").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_ND100").wnm_originalfield].ToString();
                objDisk.POL_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POL_ND300") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POL_ND300").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POL_ND300").wnm_originalfield].ToString();
                objDisk.STH_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STH_ND300") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STH_ND300").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "STH_ND300").wnm_originalfield].ToString();
                objDisk.GEH_ND120 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_ND120") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_ND120").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_ND120").wnm_originalfield].ToString();
                objDisk.NIT_ND300 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_ND300") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_ND300").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_ND300").wnm_originalfield].ToString();
                objDisk.SXT_ND1_2 = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_ND1_2") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_ND1_2").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_ND1_2").wnm_originalfield].ToString();

            }

            else if (fileType == "MIC")
            {
                objMIC.CTX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CTX_NM").wnm_originalfield].ToString();
                objMIC.CAZ_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAZ_NM").wnm_originalfield].ToString();
                objMIC.CZX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZX_NM").wnm_originalfield].ToString();
                objMIC.CRO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CRO_NM").wnm_originalfield].ToString();
                objMIC.CHL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CHL_NM").wnm_originalfield].ToString();
                objMIC.CIP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CIP_NM").wnm_originalfield].ToString();
                objMIC.CLI_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLI_NM").wnm_originalfield].ToString();
                objMIC.COL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "COL_NM").wnm_originalfield].ToString();
                objMIC.DAP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DAP_NM").wnm_originalfield].ToString();
                objMIC.ETP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ETP_NM").wnm_originalfield].ToString();
                objMIC.ERY_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ERY_NM").wnm_originalfield].ToString();
                objMIC.IPM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "IPM_NM").wnm_originalfield].ToString();
                objMIC.MEM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MEM_NM").wnm_originalfield].ToString();
                objMIC.NET_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NET_NM").wnm_originalfield].ToString();
                objMIC.PEN_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "PEN_NM").wnm_originalfield].ToString();
                objMIC.VAN_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VAN_NM").wnm_originalfield].ToString();
                objMIC.GEN_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEN_NM").wnm_originalfield].ToString();
                objMIC.TCY_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TCY_NM").wnm_originalfield].ToString();
                objMIC.CXM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXM_NM").wnm_originalfield].ToString();
                objMIC.AMP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMP_NM").wnm_originalfield].ToString();
                objMIC.FEP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FEP_NM").wnm_originalfield].ToString();
                objMIC.LVX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LVX_NM").wnm_originalfield].ToString();
                objMIC.AMK_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMK_NM").wnm_originalfield].ToString();
                objMIC.TZP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TZP_NM").wnm_originalfield].ToString();
                objMIC.SAM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SAM_NM").wnm_originalfield].ToString();
                objMIC.CZO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZO_NM").wnm_originalfield].ToString();
                objMIC.AZM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AZM_NM").wnm_originalfield].ToString();
                objMIC.CXA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CXA_NM").wnm_originalfield].ToString();
                objMIC.AMC_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMC_NM").wnm_originalfield].ToString();
                objMIC.CSL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CSL_NM").wnm_originalfield].ToString();
                objMIC.OXA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OXA_NM").wnm_originalfield].ToString();
                objMIC.FOX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOX_NM").wnm_originalfield].ToString();
                objMIC.NOR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NOR_NM").wnm_originalfield].ToString();
                objMIC.GEH_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEH_NM").wnm_originalfield].ToString();
                objMIC.TEC_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TEC_NM").wnm_originalfield].ToString();
                objMIC.FOS_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FOS_NM").wnm_originalfield].ToString();
                objMIC.NIT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NIT_NM").wnm_originalfield].ToString();
                objMIC.SXT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "SXT_NM").wnm_originalfield].ToString();
                objMIC.DOR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOR_NM").wnm_originalfield].ToString();
                objMIC.AMX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "AMX_NM").wnm_originalfield].ToString();
                objMIC.ATM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ATM_NM").wnm_originalfield].ToString();
                objMIC.BIA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "BIA_NM").wnm_originalfield].ToString();
                objMIC.CAS_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CAS_NM").wnm_originalfield].ToString();
                objMIC.CCV_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CCV_NM").wnm_originalfield].ToString();
                objMIC.CEC_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEC_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEC_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEC_NM").wnm_originalfield].ToString();
                objMIC.CEP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CEP_NM").wnm_originalfield].ToString();
                objMIC.CFP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CFP_NM").wnm_originalfield].ToString();
                objMIC.CID_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CID_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CID_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CID_NM").wnm_originalfield].ToString();
                objMIC.CLR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CLR_NM").wnm_originalfield].ToString();
                objMIC.CMZ_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CMZ_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CMZ_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CMZ_NM").wnm_originalfield].ToString();
                objMIC.CPO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPO_NM").wnm_originalfield].ToString();
                objMIC.CPT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CPT_NM").wnm_originalfield].ToString();
                objMIC.CZA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZA_NM").wnm_originalfield].ToString();
                objMIC.CZT_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "CZT_NM").wnm_originalfield].ToString();
                objMIC.DOX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "DOX_NM").wnm_originalfield].ToString();
                objMIC.FLU_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "FLU_NM").wnm_originalfield].ToString();
                objMIC.GEM_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEM_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEM_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "GEM_NM").wnm_originalfield].ToString();
                objMIC.ITR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "ITR_NM").wnm_originalfield].ToString();
                objMIC.LND_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LND_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LND_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LND_NM").wnm_originalfield].ToString();
                objMIC.LNZ_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "LNZ_NM").wnm_originalfield].ToString();
                objMIC.MET_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MET_NM").wnm_originalfield].ToString();
                objMIC.MFX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MFX_NM").wnm_originalfield].ToString();
                objMIC.MIF_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MIF_NM").wnm_originalfield].ToString();
                objMIC.MNO_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MNO_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MNO_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MNO_NM").wnm_originalfield].ToString();
                objMIC.MUP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MUP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MUP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "MUP_NM").wnm_originalfield].ToString();
                objMIC.NAL_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "NAL_NM").wnm_originalfield].ToString();
                objMIC.OFX_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "OFX_NM").wnm_originalfield].ToString();
                objMIC.POS_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "POS_NM").wnm_originalfield].ToString();
                objMIC.QDA_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "QDA_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "QDA_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "QDA_NM").wnm_originalfield].ToString();
                objMIC.RIF_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "RIF_NM").wnm_originalfield].ToString();
                objMIC.TMP_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TMP_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TMP_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TMP_NM").wnm_originalfield].ToString();
                objMIC.TOB_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "TOB_NM").wnm_originalfield].ToString();
                objMIC.VOR_NM = (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NM") == null) || (dbfTable.Columns.FirstOrDefault(v => v.Name == (whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NM").wnm_originalfield)) == null) ? null : dbfRow[whonetmapping.FirstOrDefault(x => x.wnm_whonetfield == "VOR_NM").wnm_originalfield].ToString();

            }

        }

        public static string convertColumn(string originalColumn)
        {
            string objReturn = "";
            int index = 0;
            Int32.TryParse(originalColumn.Replace("Column", ""), out index);

            objReturn = "Column" + (index - 1);

            return objReturn;
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
        public static string Export_LabFileDataDetail(string labdetail_path, List<TRSTGLabFileDataDetail> model, LabFileUploadDataDTO selectedLab)
        {
            var strStatus = "";
            var hos_code = selectedLab.lfu_hos_code;
            var lfu_FileName = selectedLab.lfu_FileName;
            var lfu_id = selectedLab.lfu_id.ToString();

            lfu_FileName = lfu_FileName.Replace(Path.GetExtension(lfu_FileName), ".csv");
            var filename = string.Format("{0}__{1}", lfu_id, lfu_FileName);
            string str_CurrentDate = selectedLab.lfu_createdate.Value.ToString("yyyyMMdd");
            var strFilePath = Path.Combine(labdetail_path, str_CurrentDate, hos_code);
            var idx = 0;
            if (!File.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }

            var strFullPath = Path.Combine(strFilePath, filename);
            StreamWriter sw = new StreamWriter(strFullPath, false);//, Encoding.UTF8

            foreach (var lst in model)
            {
                sw.Write(lst.ldd_id + ","
                        + lst.ldd_status + ","
                        + lst.ldd_whonetfield + ","
                        + lst.ldd_originalfield + ","
                        + lst.ldd_originalvalue.Replace(",", "*-*") + ","
                        + ","
                        + lst.ldd_ldh_id + ","
                        + lst.ldd_createuser + ",,,"
                        );//, Encoding.UTF8
                idx += 1;
                if (idx != model.Count()) { sw.Write(sw.NewLine); }
            }

            sw.Close();
            strStatus = "OK";
            return strStatus;
        }
    }
}
