using System;
using System.Collections.Generic;
//using System.Text;
using ALISS.LabFileUpload.DTO;
using ALISS.Mapping.DTO;
using Microsoft.EntityFrameworkCore;
using Log4NetLibrary;
using System.Linq;
using ALISS.LabFileUpload.Batch.Models;
using AutoMapper;
//using System.Diagnostics;
using Microsoft.Data.SqlClient;
using System.Data;
using EFCore.BulkExtensions;

namespace ALISS.LabFileUpload.Batch.DataAccess
{
    public class LabDataDAL
    {
        //private static readonly ILogService log = new LogService(typeof(LabDataDAL));
        private readonly IMapper _mapper;
        //private readonly LabDataContext _db;

        //public LabDataDAL(LabDataContext db, IMapper mapper)
        //{
        //    _db = db;
        //    _mapper = mapper;
        //}

        #region TRLabFileUpload
        public List<LabFileUploadDataDTO> Get_NewLabFileUpload(char status)
        {

            List<LabFileUploadDataDTO> objList = new List<LabFileUploadDataDTO>();
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        objList = _db.LabFileUploadDataDTOs.FromSqlRaw<LabFileUploadDataDTO>("sp_GET_TRLabFileUploadList {0} ,{1} ,{2} ,{3}", null, null, null, status).ToList();
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

            }


            return objList;

        }


        public int Update_LabFileUploadStatus(string lfu_id, char lfu_status, int lfu_error, string lfu_updateuser, string lfu_remark = "")
        {
            int rowsAffected = 0;
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        //rowsAffected = _db.Database.ExecuteSqlCommand("exec sp_UPDATE_TRLabFileUpload_Status {0},{1},{2},{3}", lfu_id, lfu_status, lfu_error, lfu_updateuser);

                        var lab_file = _db.TRLabFileUploads
                                             .FirstOrDefault(b => b.lfu_id == new Guid(lfu_id));

                        lab_file.lfu_status = lfu_status;
                        lab_file.lfu_ErrorRecord = lfu_error;
                        lab_file.lfu_updateuser = lfu_updateuser;
                        lab_file.lfu_updatedate = DateTime.Today;

                        if (!string.IsNullOrEmpty(lfu_remark))
                        {
                            lab_file.lfu_remark = lfu_remark;
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
                    }
                }
            }

            return rowsAffected;
        }
        #endregion

        #region TRMapping
        public MappingDataDTO GetMappingData(string mp_id)
        {

            MappingDataDTO objModel = new MappingDataDTO();

            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        objModel = _db.MappingDataDTOs.FromSqlRaw<MappingDataDTO>("sp_GET_TRMappingByID {0}", mp_id).AsEnumerable().FirstOrDefault();
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

            }
            return objModel;
        }
        #endregion

        #region TRWHONetMapping
        public List<WHONetMappingListsDTO> GetWHONetMappingList(string wnm_mappingid, string mst_code)
        {
            List<WHONetMappingListsDTO> objList = new List<WHONetMappingListsDTO>();
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        objList = _db.WHONetMappingListsDTOs.FromSqlRaw<WHONetMappingListsDTO>("sp_GET_TRWHONetMappingList {0}, {1}", wnm_mappingid, mst_code).ToList();

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

            }
            return objList;
        }
        #endregion

        #region TRWardTypeMapping
        public List<WardTypeMappingListsDTO> GetWardTypeMappingList(string wdm_mappingid, string mst_code, string hos_code = null)
        {
            List<WardTypeMappingListsDTO> objList = new List<WardTypeMappingListsDTO>();
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        objList = _db.WardTypeMappingListsDTOs.FromSqlRaw<WardTypeMappingListsDTO>("sp_GET_TRWardTypeMappingList {0}, {1} , {2}", wdm_mappingid, mst_code, hos_code).ToList();

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

            }
            return objList;
        }
        #endregion

        #region TRSpecimenMapping
        public List<SpecimenMappingListsDTO> GetSpecimeneMappingList(string spm_mappingid, string mst_code)
        {
            List<SpecimenMappingListsDTO> objList = new List<SpecimenMappingListsDTO>();
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        objList = _db.SpecimenMappingListsDTOs.FromSqlRaw<SpecimenMappingListsDTO>("sp_GET_TRSpecimenMappingList {0}, {1}", spm_mappingid, mst_code).ToList();

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

            }
            return objList;
        }
        #endregion

        #region TROrganismMapping
        public List<OrganismMappingListsDTO> GetOrganismMappingList(string ogm_mappingid, string mst_code)
        {
            List<OrganismMappingListsDTO> objList = new List<OrganismMappingListsDTO>();
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        objList = _db.OrganismMappingListsDTOs.FromSqlRaw<OrganismMappingListsDTO>("sp_GET_TROrganismMappingList {0}, {1}", ogm_mappingid, mst_code).ToList();

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

            }
            return objList;
        }
        #endregion

        #region TRSTGLabFileDataHeader
        public Guid Save_LabFileDataHeader(TRSTGLabFileDataHeader model)
        {
            var currentDateTime = DateTime.Now;
            Guid objReturn = Guid.Empty;

            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {

                        _db.TRSTGLabFileDataHeaders.Add(model);

                        _db.SaveChanges();

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

            }

            return objReturn;
        }

        public void InsertBulk_LabFileDataHeader(List<TRSTGLabFileDataHeader> models)
        {         
            Guid objReturn = Guid.Empty;
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        

                        _db.BulkInsert(models);
                        _db.SaveChanges();
                        trans.Commit();

                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            var log  = new LogWriter("BulkInsert exception .." + ex.Message);
                            var log1 = new LogWriter(ex.StackTrace.ToString());
                            trans.Rollback();
                        }
                        catch(Exception ex1)
                        {
                            var log = new LogWriter("Rollback exception .." + ex.Message);
                            var log1 = new LogWriter(ex1.StackTrace.ToString());
                            trans.Dispose();
                        }
                        //trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }

            }
         
        }

        public void InsertBulk_LabNARSTData(List<TRSTGLabNarstData> models)
        {
            Guid objReturn = Guid.Empty;
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        _db.BulkInsert(models);
                        _db.SaveChanges();
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

            }

        }

        public void InsertBulk_LabNARSTData_DISK(List<TRSTGLabNarstData_DISK> models)
        {
            Guid objReturn = Guid.Empty;
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        _db.BulkInsert(models);
                        _db.SaveChanges();
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

            }

        }
        public void InsertBulk_LabNARSTData_MIC(List<TRSTGLabNarstData_MIC> models)
        {
            Guid objReturn = Guid.Empty;
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        _db.BulkInsert(models);
                        _db.SaveChanges();
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

            }

        }
        public void InsertBulk_LabNARSTData_ETEST(List<TRSTGLabNarstData_ETEST> models)
        {
            Guid objReturn = Guid.Empty;
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        _db.BulkInsert(models);
                        _db.SaveChanges();
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

            }

        }
        public int Get_CheckExistingHeader(TRSTGLabFileDataHeader model, string mp_program, string mp_filetype)
        {
            int objReturn = 1;
            var currentDateTime = DateTime.Now;
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        //var objResult2 = _db.TRSTGLabFileDataHeaders.FirstOrDefault(x => x.ldh_hos_code == model.ldh_hos_code
                        //                                                  && x.ldh_lab == model.ldh_lab
                        //                                                  && x.ldh_labno == model.ldh_labno
                        //                                                  && x.ldh_organism == model.ldh_organism
                        //                                                  //&& x.ldh_specimen == model.ldh_specimen
                        //                                                  && x.ldh_date == model.ldh_date
                        //                                                  && x.ldh_lfu_id != model.ldh_lfu_id
                        //                                                  && x.ldh_flagdelete == false);

                        var objResult = _db.TRSTGLabFileDataHeaders.FromSqlRaw<TRSTGLabFileDataHeader>("sp_UPLOAD_CheckExistingHeader {0},{1},{2},{3},{4},{5},{6},{7}",
                            model.ldh_hos_code,
                            model.ldh_lab,
                            model.ldh_labno,
                            model.ldh_organism,
                            model.ldh_date,
                            model.ldh_lfu_id,
                            mp_program,
                            mp_filetype
                            ).AsEnumerable().FirstOrDefault();


                        if (objResult != null)
                        {
                            objReturn = objResult.ldh_sequence + 1;

                            objResult.ldh_status = 'D';
                            objResult.ldh_flagdelete = true;
                            objResult.ldh_updateuser = "BATCH";
                            objResult.ldh_updatedate = currentDateTime;
                            _db.SaveChanges();
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

            }

            return objReturn;
        }

        public Guid Get_HeaderIdForMultipleline(TRSTGLabFileDataHeader model)
        {
            Guid objReturn = Guid.Empty;
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        var objResult = _db.TRSTGLabFileDataHeaders.FirstOrDefault(x => x.ldh_lfu_id == model.ldh_lfu_id
                                                                          && x.ldh_hos_code == model.ldh_hos_code
                                                                          && x.ldh_lab == model.ldh_lab
                                                                          && x.ldh_labno == model.ldh_labno
                                                                          && x.ldh_organism == model.ldh_organism
                                                                          //&& x.ldh_specimen == model.ldh_specimen
                                                                          && x.ldh_date == model.ldh_date
                                                                          && x.ldh_flagdelete == false);

                        if (objResult != null)
                        {
                            objReturn = objResult.ldh_id;
                        }


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

            }

            return objReturn;
        }

        public TRSTGLabNarstData CheckExist(TRSTGLabNarstData objModel)
        {
            TRSTGLabNarstData objResult = new TRSTGLabNarstData();

            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        objResult = _db.TRSTGLabNarstDatas.FirstOrDefault(x => x.lna_lfu_id == objModel.lna_lfu_id
                                                                              && x.lna_hos_code == objModel.lna_hos_code
                                                                              && x.lna_lab == objModel.lna_lab
                                                                              && x.lna_labno == objModel.lna_labno
                                                                              && x.lna_organism_original_value == objModel.lna_organism_original_value
                                                                              && x.lna_cdate == objModel.lna_cdate
                                                                              && x.lna_status != "D");
                      

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
            }
            return objResult;
        }
        public TRSTGLabNarstData_DISK CheckExist_DISK(TRSTGLabNarstData_DISK objModel)
        {
            TRSTGLabNarstData_DISK objResult = new TRSTGLabNarstData_DISK();

            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        objResult = _db.TRSTGLabNarstDatas_DISK.FirstOrDefault(x => x.lnd_lfu_id == objModel.lnd_lfu_id
                                                                              && x.lnd_hos_code == objModel.lnd_hos_code
                                                                              && x.lnd_lab == objModel.lnd_lab
                                                                              && x.lnd_labno == objModel.lnd_labno
                                                                              && x.lnd_organism_original_value == objModel.lnd_organism_original_value
                                                                              && x.lnd_cdate == objModel.lnd_cdate
                                                                              && x.lnd_status != "D");


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
            }
            return objResult;
        }
        public TRSTGLabNarstData_MIC CheckExist_MIC(TRSTGLabNarstData_MIC objModel)
        {
            TRSTGLabNarstData_MIC objResult = new TRSTGLabNarstData_MIC();

            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        objResult = _db.TRSTGLabNarstDatas_MIC.FirstOrDefault(x => x.lnm_lfu_id == objModel.lnm_lfu_id
                                                                                 && x.lnm_hos_code == objModel.lnm_hos_code
                                                                                 && x.lnm_lab == objModel.lnm_lab
                                                                                 && x.lnm_labno == objModel.lnm_labno
                                                                                 && x.lnm_organism_original_value == objModel.lnm_organism_original_value
                                                                                 && x.lnm_cdate == objModel.lnm_cdate
                                                                                 && x.lnm_status != "D");


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
            }
            return objResult;
        }
        public TRSTGLabNarstData_ETEST CheckExist_ETEST(TRSTGLabNarstData_ETEST objModel)
        {
            TRSTGLabNarstData_ETEST objResult = new TRSTGLabNarstData_ETEST();

            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        objResult = _db.TRSTGLabNarstDatas_ETEST.FirstOrDefault(x => x.lne_lfu_id == objModel.lne_lfu_id
                                                                                 && x.lne_hos_code == objModel.lne_hos_code
                                                                                 && x.lne_lab == objModel.lne_lab
                                                                                 && x.lne_labno == objModel.lne_labno
                                                                                 && x.lne_organism_original_value == objModel.lne_organism_original_value
                                                                                 && x.lne_cdate == objModel.lne_cdate
                                                                                 && x.lne_status != "D");


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
            }
            return objResult;
        }
        public void Delete_LabFileDataHeaderDetail(string lfu_id)
        {
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {

                        var errDetail = _db.TRSTGLabFileDataDetails.FromSqlRaw<TRSTGLabFileDataDetail>("EXEC [sp_GET_TRSTGLabFileDataDetail] {0}", lfu_id).ToList();
                        var errHeader = _db.TRSTGLabFileDataHeaders.Where(x => x.ldh_lfu_id.ToString() == lfu_id).ToList();

                        _db.BulkDelete(errDetail);
                        _db.BulkDelete(errHeader);

                        _db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        //_log.LogError("Error : " + ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }

        }

        #endregion

        #region TRSTGLabFileDataDetail
        public STGLabFileDataDetailDTO Save_LabFileDataDetail(List<TRSTGLabFileDataDetail> model)
        {
            var currentDateTime = DateTime.Now;
            STGLabFileDataDetailDTO objReturn = new STGLabFileDataDetailDTO();
            using (var _db = new LabDataContext())
            {
                var maxrecord = 100000;
                var nextidx = 0;
                var modelsplit = new List<TRSTGLabFileDataDetail>();
                var totalrecord = model.Count;
                var loop = Math.Ceiling(Convert.ToDecimal((double)totalrecord / (double)maxrecord));

                for (var i = 1; i <= loop; i++)
                {
                    modelsplit.Clear();
                    if (totalrecord >= (maxrecord * i))
                    {
                        for (var j = nextidx; j < (maxrecord * i); j++)
                        {
                            var m = model[j];
                            modelsplit.Add(m);
                        }
                        _db.BulkInsert(modelsplit);
                        nextidx = (maxrecord * i);
                    }
                    else
                    {
                        for (var j = nextidx; j < totalrecord; j++)
                        {
                            var m = model[j];
                            modelsplit.Add(m);
                        }
                        _db.BulkInsert(modelsplit);
                        break;
                    }

                }

                //_db.BulkInsert(model);

                //using (var trans = _db.Database.BeginTransaction())
                //{
                //    try
                //    {

                //        //_db.TRSTGLabFileDataDetails.
                //       _db.TRSTGLabFileDataDetails.AddRange(model);

                //        _db.SaveChanges();

                //        trans.Commit();

                //        //objReturn = _mapper.Map<STGLabFileDataDetailDTO>(objModel);
                //    }
                //    catch (Exception ex)
                //    {
                //        // TODO: Handle failure
                //        trans.Rollback();
                //    }
                //    finally
                //    {
                //        trans.Dispose();
                //    }
                //}

            }

            return objReturn;
        }
        #endregion

        #region TCParameter
        public List<TCParameter> GetParameter(string prm_code_major)
        {
            List<TCParameter> objReturn = new List<TCParameter>();

            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        objReturn = _db.TCParameters.FromSqlRaw<TCParameter>("sp_GET_TCParameter {0}", prm_code_major).ToList();
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
            }

            return objReturn;
        }
        #endregion

        #region TRLabFileErrorHeader
        public Guid Save_TRLabFileErrorHeader(TRLabFileErrorHeader model)
        {
            var currentDateTime = DateTime.Now;
            Guid objReturn = Guid.Empty;

            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.feh_status == 'N')
                        {
                            _db.TRLabFileErrorHeaders.Add(model);
                        }
                        else if (model.feh_status == 'E')
                        {
                            //var objModel = new TRLabFileErrorHeader();
                            var objModel = _db.TRLabFileErrorHeaders.FirstOrDefault(x => x.feh_lfu_id == model.feh_lfu_id && x.feh_field == model.feh_field);
                            if (objModel != null)
                            {
                                objModel.feh_errorrecord = model.feh_errorrecord;
                                objReturn = objModel.feh_id;
                            }
                            else
                            {
                                model.feh_id = Guid.NewGuid();
                                model.feh_status = 'N';
                                objReturn = model.feh_id;
                                _db.TRLabFileErrorHeaders.Add(model);
                            }

                        }

                        _db.SaveChanges();

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

            }

            return objReturn;
        }

        public int Update_LabFileErrorStatus(string feh_lfu_id, char feh_status, string feh_updateuser)
        {
            int rowsAffected = 0;
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        //rowsAffected = _db.Database.ExecuteSqlRaw("exec sp_UPDATE_TRLabFileUploadError_Status {0},{1},{2}", feh_lfu_id, feh_status, feh_updateuser);
                        var lab_err_header = _db.TRLabFileErrorHeaders
                                             .FromSqlInterpolated($"EXECUTE dbo.sp_TRLabFileErrorHeader {feh_lfu_id}")
                                             .ToList();
                        if (lab_err_header.Count > 0)
                        {
                            foreach (var eh in lab_err_header)
                            {
                                eh.feh_status = feh_status;
                                eh.feh_flagdelete = (feh_status == 'D') ? true : false;
                                eh.feh_updateuser = feh_updateuser;
                                eh.feh_updatedate = DateTime.Today;
                            }

                            var headr_id = lab_err_header.Select(f => f.feh_id).Distinct().ToList();

                            var lab_err_detail = _db.TRLabFileErrorDetails.Include(ed => headr_id.Contains(ed.fed_feh_id))
                                                 .ToList();

                            foreach (var ed in lab_err_detail)
                            {
                                ed.fed_status = feh_status;
                                ed.fed_updateuser = feh_updateuser;
                                ed.fed_updatedate = DateTime.Today;
                            }
                            //var lab_err = from ed in _db.Set<TRLabFileErrorDetail>()
                            //            join eh in _db.Set<TRLabFileErrorHeader>()
                            //               on ed.fed_feh_id equals eh.feh_id
                            //            where eh.feh_lfu_id == new Guid(feh_lfu_id)
                            //            && eh.feh_type == "CONVERT_ERROR"
                            //           select new { ed , eh};

                            _db.SaveChanges();
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
                    }
                }
            }

            return rowsAffected;
        }
        #endregion

        #region TRLabFileErrorDetail
        public Guid Save_TRLabFileErrorDetail(TRLabFileErrorDetail model)
        {
            var currentDateTime = DateTime.Now;
            Guid objReturn = Guid.Empty;

            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {

                        _db.TRLabFileErrorDetails.Add(model);

                        _db.SaveChanges();

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

            }

            return objReturn;
        }

        public TRLabFileErrorDetail Save_TRLabFileErrorDetailList(List<TRLabFileErrorDetail> model)
        {
            var currentDateTime = DateTime.Now;
            TRLabFileErrorDetail objReturn = new TRLabFileErrorDetail();
            using (var _db = new LabDataContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {

                        _db.TRLabFileErrorDetails.AddRange(model);

                        _db.SaveChanges();

                        trans.Commit();

                        //objReturn = _mapper.Map<STGLabFileDataDetailDTO>(objModel);
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

            }

            return objReturn;
        }
        #endregion     

        public static string ConfigValue(string param)
        {           
            return LabDataContext.GetConfigurationValue(param);
        }
    }

    //private void WriteToDatabase()
    //{
    //    // get your connection string
    //    string connString = "";
    //    STGLabFileDataDetailDTO objReturn = new STGLabFileDataDetailDTO();
    //    // connect to SQL
    //    using (SqlConnection connection = new SqlConnection(connString))
    //    {
    //        // make sure to enable triggers
    //        // more on triggers in next post
    //        SqlBulkCopy bulkCopy = new SqlBulkCopy (connection,
    //                                                SqlBulkCopyOptions.TableLock |
    //                                                SqlBulkCopyOptions.FireTriggers |
    //                                                SqlBulkCopyOptions.UseInternalTransaction,
    //                                                null
    //                                                );

    //        // set the destination table name
    //        bulkCopy.DestinationTableName = "dbo.TRSTGLabFileDataDetail";

    //        bulkCopy.ColumnMappings.Add(nameof(objReturn.ldd_status), "ldd_status");
    //        bulkCopy.ColumnMappings.Add(nameof(objReturn.ldd_ldh_id), "ldd_ldh_id");
    //        bulkCopy.ColumnMappings.Add(nameof(objReturn.ldd_whonetfield), "ldd_whonetfield");
    //        bulkCopy.ColumnMappings.Add(nameof(objReturn.ldd_originalfield), "ldd_originalfield");
    //        bulkCopy.ColumnMappings.Add(nameof(objReturn.ldd_originalvalue), "ldd_originalvalue");
    //        bulkCopy.ColumnMappings.Add(nameof(objReturn.ldd_mappingvalue), "ldd_mappingvalue");
    //        bulkCopy.ColumnMappings.Add(nameof(objReturn.ldd_createuser), "ldd_createuser");
    //        bulkCopy.ColumnMappings.Add(nameof(objReturn.ldd_createdate), "ldd_createdate");

    //        connection.Open();

    //        // write the data in the "dataTable"
    //        bulkCopy.WriteToServer(ToDataTable(objReturn));
    //        connection.Close();
    //    }
    //    // reset
    //    this.dataTable.Clear();
    //    this.recordCount = 0;
    //}
}
