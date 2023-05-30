using ALISS.HISUpload.DTO;
using ALISS.HISUpload.Library.DataAccess;
using ALISS.HISUpload.Library.Models;
using ALISS.MasterManagement.DTO;
using ALISS.MasterManagement.Library.Models;
using AutoMapper;
using Log4NetLibrary;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.HISUpload.Library
{
    public class HISFileUploadService : IHISFileUploadService
    {
        private static readonly ILogService log = new LogService(typeof(HISFileUploadService));
        private readonly HISFileUploadContext _db;
        private readonly IMapper _mapper;
        public HISFileUploadService(HISFileUploadContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public HISUploadDataDTO GetHISFileUploadDataById(int hfu_id)
        {
            log.MethodStart();
            HISUploadDataDTO objModel = new HISUploadDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    //var objReturn1 = _db.TRHISFileUploads.FirstOrDefault(x => x.hfu_id == hfu_id);

                    var objDataList = _db.HISFileUploadDataDTOs.FromSqlRaw<HISUploadDataDTO>("sp_GET_TRHISFileUploadList  {0} ,{1} ,{2} ,{3}, {4}, {5}, {6} , {7}"
                                                                                                    , null , null, null , null , 0 , null , null
                                                                                                    , hfu_id
                                                                                                    ).ToList();

                    var objData = objDataList.FirstOrDefault();
                    objModel = _mapper.Map<HISUploadDataDTO>(objData);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }


            log.MethodFinish();

            return objModel;
        }

        public List<HISUploadDataDTO> GetHISFileUploadListWithModel(HISUploadDataSearchDTO searchModel)
        {
            log.MethodStart();

            List<HISUploadDataDTO> objList = new List<HISUploadDataDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.HISFileUploadDataDTOs.FromSqlRaw<HISUploadDataDTO>("sp_GET_TRHISFileUploadList  {0} ,{1} ,{2} ,{3}, {4}, {5}, {6}"
                                                                                                    , searchModel.hfu_hos_code
                                                                                                    , searchModel.hfu_prv_code
                                                                                                    , searchModel.hfu_arh_code
                                                                                                    , null
                                                                                                    , 0
                                                                                                    , searchModel.hfu_upload_date_from_str
                                                                                                    , searchModel.hfu_upload_date_to_str
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<HISUploadDataDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }
        public List<HISFileTemplateDTO> GetHISFileTemplate_Actice_WithModel(HISFileTemplateDTO searchModel)
        {
            log.MethodStart();

            List<HISFileTemplateDTO> objList = new List<HISFileTemplateDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.HISFileTemplateDTOs.FromSqlRaw<HISFileTemplateDTO>("sp_GET_TCHISFileTemplate_Active").ToList();

                    objList = _mapper.Map<List<HISFileTemplateDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }
        public HISUploadDataDTO SaveLabFileUploadData(HISUploadDataDTO model)
        {
            log.MethodStart();

            var currentDateTime = DateTime.Now;
            HISUploadDataDTO objReturn = new HISUploadDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objModel = new TRHISFileUpload();

                    if (model.hfu_status == 'N')
                    {
                        objModel = _mapper.Map<TRHISFileUpload>(model);

                        objModel.hfu_createdate = currentDateTime;
                        //objModel.hfu_updatedate = currentDateTime;

                        _db.TRHISFileUploads.Add(objModel);
                    }
                    else
                    {
                        objModel = _db.TRHISFileUploads.FirstOrDefault(x => x.hfu_id == model.hfu_id);
                        objModel.hfu_status = model.hfu_status;
                        //objModel.hfu_version = model.hfu_version;
                        //objModel.hfu_delete_flag = model.hfu_delete_flag;
                        //objModel.hfu_lab = model.hfu_lab;
                        //objModel.hfu_file_name = model.hfu_file_name;
                        //objModel.hfu_file_path = model.hfu_file_path;
                        //objModel.hfu_file_type = model.hfu_file_type;
                        //objModel.hfu_total_records = model.hfu_total_records;
                        //objModel.hfu_error_records = model.hfu_error_records;
                        objModel.hfu_approveduser = model.hfu_approveduser;
                        objModel.hfu_approveddate = model.hfu_approveddate;
                        objModel.hfu_updateuser = model.hfu_approveduser;
                        objModel.hfu_updatedate = currentDateTime;
                    }

                    _db.SaveChanges();

                    trans.Commit();

                    objReturn = _mapper.Map<HISUploadDataDTO>(objModel);
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            return objReturn;
        }

        public HISFileUploadSummaryDTO SaveFileUploadSummary(List<HISFileUploadSummaryDTO> models)
        {
            log.MethodStart();

            var currentDateTime = DateTime.Now;
            HISFileUploadSummaryDTO objReturn = new HISFileUploadSummaryDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach(var objModel in models)
                    {
                        var obj = new TRHISFileUploadSummary();

                        obj.hus_hfu_id = objModel.hus_hfu_id;
                        obj.hus_error_fieldname = objModel.hus_error_fieldname;
                        obj.hus_error_fielddescr = objModel.hus_error_fielddescr;
                        obj.hus_error_fieldrecord = objModel.hus_error_fieldrecord;
                        obj.hus_createuser = objModel.hus_createuser;
                        obj.hus_createdate = currentDateTime;

                        _db.TRHISFileUploadSummarys.Add(obj);
                    }
                
                    _db.SaveChanges();

                    trans.Commit();

                    var objM = _db.TRHISFileUploadSummarys.FirstOrDefault();
                    objReturn = _mapper.Map<HISFileUploadSummaryDTO>(objM);
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            return objReturn;
        }
        public List<HISFileUploadSummaryDTO> GetHISFileUploadSummaryByUploadId(int HISUploadFileid)
        {
            log.MethodStart();
            List<HISFileUploadSummaryDTO> objList = new List<HISFileUploadSummaryDTO> ();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.HISFileUploadSummaryDTOs.FromSqlRaw<HISFileUploadSummaryDTO>("sp_GET_TRHISFileUploadSummary {0}"
                                                                                                        , HISUploadFileid).ToList();

                    objList = _mapper.Map<List<HISFileUploadSummaryDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }


            log.MethodFinish();

            return objList;
        }

        public List<LabDataWithHISDTO> GetLabDataWithHIS(LabDataWithHISSearchDTO searchModel)
        {
            log.MethodStart();

            List<LabDataWithHISDTO> objList = new List<LabDataWithHISDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.LabDataWithHISDTOs.FromSqlRaw<LabDataWithHISDTO>("sp_GET_LabDataWithHIS {0} ,{1} ,{2}"
                                                                                            , searchModel.hos_code
                                                                                            , searchModel.start_date_str
                                                                                            , searchModel.end_date_str).ToList();

                    objList = _mapper.Map<List<LabDataWithHISDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }

        public List<HISDetailDTO> GetSTGHISUploadDetail(LabDataWithHISSearchDTO searchModel)
        {
            log.MethodStart();

            List<HISDetailDTO> objList = new List<HISDetailDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.HISDetailDTOs.FromSqlRaw<HISDetailDTO>("sp_GET_TRSTGHISFileUploadDetail {0} ,{1} ,{2}"
                                                                                            , searchModel.hos_code
                                                                                            , searchModel.start_date_str
                                                                                            , searchModel.end_date_str).ToList();

                    objList = _mapper.Map<List<HISDetailDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }

        public List<STGLabFileDataDetailDTO> GetLabDataWithHISDetail(LabDataWithHISSearchDTO searchModel)
        {
            log.MethodStart();
            
            List<STGLabFileDataDetailDTO> objList = new List<STGLabFileDataDetailDTO>();
            
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.STGLabFileDataDetailDTOs.FromSqlRaw<STGLabFileDataDetailDTO>("sp_GET_LabDataWithHISDetail {0}, {1}, {2} "
                                                                                                , searchModel.hos_code
                                                                                                , searchModel.start_date_str
                                                                                                , searchModel.end_date_str).ToList();

                    objList = _mapper.Map<List<STGLabFileDataDetailDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }
            
            log.MethodFinish();

            return objList;
        }

        public byte[] ExportLabDataWithHIS(LabDataWithHISSearchDTO model)
        {
            //MasterTemplateDTO ActiveMasterTemplate = await _apiHelper.GetDataByModelAsync<MasterTemplateDTO, MasterTemplateSearchDTO>("mastertemplate_api/Get_List_Active_ByModel", model);
            MasterTemplateDTO ActiveMasterTemplate = GetList_MasterTemplate_Active_WithModel(new MasterTemplateSearchDTO());
            //HISFileTemplateDTO HISTemplateActive = await _apiHelper.GetDataListByModelAsync<HISFileTemplateDTO, HISFileTemplateDTO>("his_api/GetHISTemplateActive", model);
            HISFileTemplateDTO HISTemplateActive = GetHISFileTemplate_Actice_WithModel(new HISFileTemplateDTO())?.FirstOrDefault();

            byte[] fileContents;
            const int idxColLabNo = 3;
            const int idxColSpcDate = 4;
            const int idxColHN = 2;
            const int idxColRef = 1;

            int idcColWhonetConfig = 5;
            var lstDynamicColumn = new List<int>();
            var dctWhonetColumn = new Dictionary<string, int>();

            //const int idxColAdmisDate = 6;
            //const int idxColCINI = 7;
            int idxRowCurrent = 1;

            string COL_REF_NO = HISTemplateActive.hft_field1; // "Ref No";
            string COL_HN_NO = HISTemplateActive.hft_field2; //"HN";
            string COL_LAB_NO = HISTemplateActive.hft_field3; // "Lab";
            string COL_DATE = HISTemplateActive.hft_field4; //"Date";

            List<LabDataWithHISDTO> objLabWithRef = new List<LabDataWithHISDTO>();
            List<HISDetailDTO> objHISDetail = new List<HISDetailDTO>();
            List<WHONETColumnDTO> objWhonetActive = new List<WHONETColumnDTO>();
            List<string> objWhonetHISColumn = new List<string>();
            WHONETColumnDTO objSearchWhonet = new WHONETColumnDTO();
            List<STGLabFileDataDetailDTO> objLabDetail = new List<STGLabFileDataDetailDTO>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //sp_GET_LabDataWithHIS
            //objLabWithRef = await _apiHelper.GetDataListByModelAsync<LabDataWithHISDTO, LabDataWithHISSearchDTO>("his_api/GetLabDataWithHIS", model);
            objLabWithRef = GetLabDataWithHIS(model);
            //objHISDetail = await _apiHelper.GetDataListByModelAsync<HISDetailDTO, LabDataWithHISSearchDTO>("his_api/GetHISDetail", model);
            objHISDetail = GetSTGHISUploadDetail(model);
            //objLabDetail = await _apiHelper.GetDataListByModelAsync<STGLabFileDataDetailDTO, LabDataWithHISSearchDTO>("his_api/GetLabDataWithHISDetail", model);
            objLabDetail = GetLabDataWithHISDetail(model);

            if (objLabWithRef.Count == 0)
            {
                // Show message No data to export
            }
            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("REF_HIS");

                // Header Column
                workSheet.Cells[idxRowCurrent, idxColRef].Value = COL_REF_NO;
                workSheet.Cells[idxRowCurrent, idxColHN].Value = COL_HN_NO;
                workSheet.Cells[idxRowCurrent, idxColLabNo].Value = COL_LAB_NO;
                workSheet.Cells[idxRowCurrent, idxColSpcDate].Value = COL_DATE;

                objSearchWhonet.wnc_mst_code = ActiveMasterTemplate.mst_code;

                //objWhonetActive = await _apiHelper.GetDataListByModelAsync<WHONETColumnDTO, WHONETColumnDTO>("whonetcolumn_api/Get_List_Active_ByModel", objSearchWhonet);
                objWhonetActive = GetList_WHONETColumn_Active_WithModel(objSearchWhonet);
                if (objWhonetActive != null)
                {
                    // ToDo : change wnc_mendatory to wnc_his_export_flag
                    var objWhonetHIS = objWhonetActive.Where(w => w.wnc_mendatory == true).ToList();
                    objWhonetHISColumn = objWhonetHIS.Select(s => s.wnc_name).ToList();
                    //objWhonetHISColumn = new List<string> { "Last name", "First name", "Sex", "Date of birth", "Age", "Location" };
                    foreach (var obj in objWhonetHISColumn)
                    {
                        // Dynamic Column From Whonet Setting
                        workSheet.Cells[idxRowCurrent, idcColWhonetConfig].Value = obj;

                        if (!dctWhonetColumn.ContainsKey(obj))
                        {
                            dctWhonetColumn.Add(obj, idcColWhonetConfig);
                        }

                        //lstDynamicColumn.Add(idcColWhonetConfig);
                        idcColWhonetConfig++;
                    }
                }

                foreach (LabDataWithHISDTO objLab in objLabWithRef)
                {
                    //if (idxRowCurrent == 11) { break; } //for test
                    idxRowCurrent += 1;
                    workSheet.Cells[idxRowCurrent, idxColRef].Value = objLab.ref_no;
                    workSheet.Cells[idxRowCurrent, idxColHN].Value = objLab.hn_no;
                    workSheet.Cells[idxRowCurrent, idxColLabNo].Value = objLab.lab_no;

                    workSheet.Cells[idxRowCurrent, idxColSpcDate].Value = objLab.spec_date;
                    workSheet.Column(idxColSpcDate).Style.Numberformat.Format = "dd/MM/yyyy";

                    // Note : obj.huh_id ที่เอามา where ต้องหาจาก query มาแล้วว่าเป็นตัวล่าสุด ที่จะ export ออกมา สถานะไม่ใช่ delete ...    
                    //(First) Find HIS Data from Upload HIS
                    var objHISDetailFromHIS = objHISDetail.Where(w => w.ref_no == objLab.ref_no
                                                            && w.hn_no == objLab.hn_no
                                                            && w.lab_no == objLab.lab_no
                                                            && w.spec_date == objLab.spec_date).ToList();
                    foreach (var objHIS in objHISDetailFromHIS)
                    {
                        if (objWhonetHISColumn.Contains(objHIS.hud_field_name))
                        {
                            if (!string.IsNullOrEmpty(objHIS.hud_field_value))
                            {
                                workSheet.Cells[idxRowCurrent, dctWhonetColumn[objHIS.hud_field_name]].Value = objHIS.hud_field_value;
                            }
                        }
                    }

                    //(Second) Find HIS Data from Lab 
                    Guid guidHeaderId = new Guid(objLab.lab_header_id);
                    var objHISDetailFromLab = objLabDetail.Where(w => w.ldd_ldh_id == guidHeaderId).ToList();
                    foreach (var obj in objHISDetailFromLab)
                    {
                        if (objWhonetHISColumn.Contains(obj.ldd_whonetfield))
                        {
                            if (!string.IsNullOrEmpty(obj.ldd_originalvalue))
                            {
                                //var DecodeValue = CryptoHelper.UnicodeDecoding(obj.ldd_originalvalue);
                                if (workSheet.Cells[idxRowCurrent, dctWhonetColumn[obj.ldd_whonetfield]].Value == null)
                                {
                                    workSheet.Cells[idxRowCurrent, dctWhonetColumn[obj.ldd_whonetfield]].Value = obj.ldd_originalvalue;
                                }

                            }
                        }
                    }

                    var idxRef_split = 5 + dctWhonetColumn.Count();
                    // Header
                    workSheet.Cells[1, idxRef_split].Value = "REF_1";
                    workSheet.Cells[1, idxRef_split + 1].Value = "REF_2";

                    if (string.IsNullOrEmpty(objLab.ref_no)) { continue; }

                    var lstRefNo = objLab.ref_no.Split(',');
                    foreach (var lst in lstRefNo)
                    {
                        workSheet.Cells[idxRowCurrent, idxRef_split].Value = lst;
                        idxRef_split += 1;
                    }
                } // End Loop Lab Data

                workSheet.Column(idxColRef).AutoFit();
                workSheet.Column(idxColHN).AutoFit();
                workSheet.Column(idxColLabNo).AutoFit();
                workSheet.Column(idxColSpcDate).AutoFit();
                workSheet.Protection.IsProtected = true; // Protect whole sheet
                foreach (var colHIS in dctWhonetColumn.Values)
                {
                    workSheet.Column(colHIS).Style.Locked = false;//unlock workSheet
                    workSheet.Column(1).AutoFit();
                }

                fileContents = package.GetAsByteArray();
            }

            return fileContents;
            //try
            //{
            //    await iJSRuntime.InvokeAsync<HISFileUploadService>(
            //          "saveAsFile",
            //          string.Format("{0}_{1}.xlsx", model.hos_name_only, DateTime.Today.ToString("yyyyMMdd")),
            //          Convert.ToBase64String(fileContents)
            //          );



            //}
            //catch (Exception ex)
            //{

            //}

        }

        public List<TRSPWithHISDetailDTO> GetTRSPWithHisDetail(TRSPWithHISSearchDTO searchModel)
        {
            log.MethodStart();

            List<TRSPWithHISDetailDTO> tRSPWithHISDetails = new List<TRSPWithHISDetailDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Database.SetCommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds);
                    var objDataList = _db.TRSPWithHISDetailDTOs.FromSqlRaw<TRSPWithHISDetailDTO>("exec [dbo].[sp_GET_TRSPWithHIS] null, {0}, {1}, {2}, {3}"
                                                                                                , searchModel.hos_code
                                                                                                , searchModel.lab_code
                                                                                                , searchModel.start_date_str
                                                                                                , searchModel.end_date_str).ToList();

                    tRSPWithHISDetails = _mapper.Map<List<TRSPWithHISDetailDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return tRSPWithHISDetails;
        }

        public MasterTemplateDTO GetList_MasterTemplate_Active_WithModel(MasterTemplateSearchDTO searchModel)
        {
            log.MethodStart();

            MasterTemplateDTO objReturn = new MasterTemplateDTO();

            //var searchModel = JsonSerializer.Deserialize<MenuSearchDTO>(param);

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.TCMasterTemplates.FromSqlRaw<TCMasterTemplate>("sp_GET_TCMasterTemplate_Active {0}, {1}, {2}", searchModel.mst_code, searchModel.mst_date_from, searchModel.mst_date_to).ToList();

                    var objListMapping = _mapper.Map<List<MasterTemplateDTO>>(objDataList);

                    if (objListMapping.Count > 0)
                    {
                        objReturn = objListMapping.FirstOrDefault();
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objReturn;
        }

        public List<WHONETColumnDTO> GetList_WHONETColumn_Active_WithModel(WHONETColumnDTO searchModel)
        {
            log.MethodStart();

            List<WHONETColumnDTO> objList = new List<WHONETColumnDTO>();

            //var searchModel = JsonSerializer.Deserialize<MenuSearchDTO>(param);

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.TCWHONETColumns.FromSqlRaw<TCWHONETColumn>("sp_GET_TCWHONETColumn_Active {0}", searchModel.wnc_mst_code).ToList();

                    objList = _mapper.Map<List<WHONETColumnDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }

        public byte[] ExportHIS_WithStore(TRSPWithHISSearchDTO model)
        {
            byte[] fileContents;
            const int idxColLabNo = 3;
            const int idxColSpcDate = 4;
            const int idxColHN = 2;
            const int idxColRef = 1;

            var lstDynamicColumn = new List<int>();
            var dctWhonetColumn = new Dictionary<string, int>();

            int idxRowCurrent = 1;       
            
            List<TRSPWithHISDetailDTO> tRSPWithHISDetails = new List<TRSPWithHISDetailDTO>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            HISFileTemplateDTO HISTemplateActive = GetHISFileTemplate_Actice_WithModel(new HISFileTemplateDTO())?.FirstOrDefault();

            string COL_REF_NO = HISTemplateActive.hft_field1; //            "REF_NO";
            string COL_HN = HISTemplateActive.hft_field2; //                "HN";
            string COL_LAB_NO = HISTemplateActive.hft_field3; //            "LAB_NO";
            string COL_DATE = HISTemplateActive.hft_field4; //              "DATE";
            string COL_DATE_FORMAT = HISTemplateActive.hft_date_format; //  "dd/MM/yyyy"

            tRSPWithHISDetails = GetTRSPWithHisDetail(model);

            if (tRSPWithHISDetails.Count == 0)
            {
                // Show message No data to export
            }
            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("REF_HIS");

                int idxHColCurrent = 1;
                foreach (var props in typeof(TRSPWithHISDetailDTO).GetProperties())
                {
                    workSheet.Cells[idxRowCurrent, idxHColCurrent].Value = props.Name.ToUpper();
                    idxHColCurrent++;
                }

                foreach (TRSPWithHISDetailDTO tRSPWithHISDetail in tRSPWithHISDetails)
                {
                    idxRowCurrent++;
                    workSheet.Cells[idxRowCurrent, idxColRef].Value = COL_REF_NO; //tRSPWithHISDetail.ref_no;
                    workSheet.Cells[idxRowCurrent, idxColHN].Value = COL_HN;//tRSPWithHISDetail.hn;
                    workSheet.Cells[idxRowCurrent, idxColLabNo].Value = COL_LAB_NO;//tRSPWithHISDetail.lab_no;
                    workSheet.Cells[idxRowCurrent, idxColSpcDate].Value = COL_DATE;//tRSPWithHISDetail.date;
                    workSheet.Cells[idxRowCurrent, idxColSpcDate].Style.Numberformat.Format = COL_DATE_FORMAT; //"dd/MM/yyyy";

                    int idxColCurrent = 1;
                    foreach (var props in typeof(TRSPWithHISDetailDTO).GetProperties())
                    {
                        if (typeof(TRSPWithHISDetailDTO).GetProperty(props.Name) != null)
                        {
                            workSheet.Cells[idxRowCurrent, idxColCurrent].Value = typeof(TRSPWithHISDetailDTO).GetProperty(props.Name).GetValue(tRSPWithHISDetail);
                            if (typeof(TRSPWithHISDetailDTO).GetProperty(props.Name).PropertyType == typeof(DateTime?))
                                workSheet.Cells[idxRowCurrent, idxColCurrent].Style.Numberformat.Format = COL_DATE_FORMAT;//"dd/MM/yyyy";
                            //workSheet.Cells[idxRowCurrent, idxColCurrent].Style.Numberformat.Format = "dd-MM-yyyy";
                        }
                        idxColCurrent++;
                    }
                }
             
                //workSheet.Column(idxColRef).AutoFit();
                //workSheet.Column(idxColHN).AutoFit();
                //workSheet.Column(idxColLabNo).AutoFit();
                //workSheet.Column(idxColSpcDate).AutoFit();
                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
                workSheet.Protection.IsProtected = true; // Protect whole sheet
                for (int i = 5; i <= workSheet.Dimension.End.Column; i++)
                {
                    workSheet.Column(i).Style.Locked = false;//unlock workSheet
                    workSheet.Column(1).AutoFit();
                }
                //foreach (var colHIS in dctWhonetColumn.Values)
                //{
                //    workSheet.Column(colHIS).Style.Locked = false;//unlock workSheet
                //    workSheet.Column(1).AutoFit();
                //}


                fileContents = package.GetAsByteArray();
            }

            return fileContents;
        }
    }
}
