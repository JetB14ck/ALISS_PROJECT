using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Log4NetLibrary;
using ALISS.STARS.Library.DataAccess;
using ALISS.STARS.DTO;
using ALISS.MasterManagement.Library.Models;
using ALISS.STARS.Library.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using EFCore.BulkExtensions;

namespace ALISS.STARS.Library
{
    public class STARSMappingDataService : ISTARSMappingDataService
    {

        private static readonly ILogService log = new LogService(typeof(STARSMappingDataService));

        private readonly STARSContext _db;
        private readonly IMapper _mapper;

        public STARSMappingDataService(STARSContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        #region Mapping

        public List<STARSMappingListsDTO> GetList()
        {
            log.MethodStart();
            List<STARSMappingListsDTO> objList = new List<STARSMappingListsDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {

                try
                {
                    objList = _db.STARSMappingListDTOs.FromSqlRaw<STARSMappingListsDTO>("sp_GET_TRSTARSMappingList").ToList();
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
        public List<STARSMappingListsDTO> GetMappingList(string Param)
        {
            log.MethodStart();


            List<STARSMappingListsDTO> objList = new List<STARSMappingListsDTO>();

            var searchModel = JsonSerializer.Deserialize<STARSMappingSearchDTO>(Param);

            using (var trans = _db.Database.BeginTransaction())
            {

                try
                {                    
                    objList = _db.STARSMappingListDTOs.FromSqlRaw<STARSMappingListsDTO>("sp_GET_TRSTARSMappingList {0}", searchModel.smp_machinetype).ToList(); 
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
        
        public STARSMappingDataDTO GetMappingDataById(string smp_id)
        {
            log.MethodStart();
            STARSMappingDataDTO objModel = new STARSMappingDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    objModel = _db.STARSMappingDataDTOs.FromSqlRaw<STARSMappingDataDTO>("sp_GET_TRSTARSMappingByID {0}", smp_id).AsEnumerable().FirstOrDefault();
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

        public STARSMappingDataDTO SaveMappingData(STARSMappingDataDTO model)
        {
            log.MethodStart();

            var currentDateTime = DateTime.Now;
            STARSMappingDataDTO objReturn = new STARSMappingDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objModel = new TRSTARSMapping();

                    if (model.smp_status == 'N')
                    {
                        objModel = _mapper.Map<TRSTARSMapping>(model);

                        objModel.smp_createdate = currentDateTime;
                        objModel.smp_updatedate = currentDateTime;

                        _db.TRSTARSMappings.Add(objModel);
                    }
                    else if(model.smp_status == 'E')
                    {
                        objModel = _db.TRSTARSMappings.FirstOrDefault(x => x.smp_id == model.smp_id);
                        objModel.smp_status = model.smp_status;
                        objModel.smp_version = model.smp_version;
                        objModel.smp_startdate = model.smp_startdate;
                        objModel.smp_enddate = model.smp_enddate;
                        objModel.smp_machinetype = model.smp_machinetype;
                        objModel.smp_dateformat = model.smp_dateformat;
                        objModel.smp_AntibioticIsolateOneRec = model.smp_AntibioticIsolateOneRec;
                        objModel.smp_firstlineisheader = model.smp_firstlineisheader;
                        objModel.smp_updateuser = model.smp_updateuser;
                        objModel.smp_updatedate = currentDateTime;
                    }
                    else if(model.smp_status == 'A')
                    {
                        objModel = _db.TRSTARSMappings.FirstOrDefault(x => x.smp_id == model.smp_id);
                        objModel.smp_status = model.smp_status;
                        objModel.smp_version = objModel.smp_version + 0.01M;
                        objModel.smp_approveduser = model.smp_approveduser;
                        objModel.smp_approveddate = currentDateTime;
                        objModel.smp_updateuser = model.smp_updateuser;
                        objModel.smp_updatedate = currentDateTime;

                        var oldVersion = _db.TRSTARSMappings.OrderByDescending(x => x.smp_version).FirstOrDefault(x => x.smp_status == 'A'
                                                           && x.smp_machinetype == model.smp_machinetype
                                                           && x.smp_startdate < model.smp_startdate
                                                           && x.smp_enddate == null
                                                           && x.smp_flagdelete == false);
                        if (oldVersion != null)
                        { 
                            oldVersion.smp_enddate = model.smp_startdate.Value.AddDays(-1);                         
                        }
                    }

                  
                    #region Save Log Process ...
                    _db.LogProcesss.Add(new LogProcess()
                    {
                        log_usr_id = (objModel.smp_updateuser ?? objModel.smp_createuser),
                        log_mnu_id = "",
                        log_mnu_name = "Mapping",
                        log_tran_id = objModel.smp_id.ToString(),
                        log_action = (objModel.smp_status == 'N' ? "New" : "Update"),
                        log_desc = "Update Mapping ",
                        log_createuser = "SYSTEM",
                        log_createdate = currentDateTime
                    });
                    #endregion

                    _db.SaveChanges();
                    
                    trans.Commit();

                    objReturn = _mapper.Map<STARSMappingDataDTO>(objModel);
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

        public TRSTARSMapping PrepareMappingUpdateVersion(TRSTARSMapping objTRMapping,String UpdateUser,DateTime UpdateDate)
        {
            //objTRMapping.smp_version = objTRMapping.smp_version + 0.01M;
            objTRMapping.smp_status = 'E';
            objTRMapping.smp_updateuser = UpdateUser;
            objTRMapping.smp_updatedate = UpdateDate;

            return objTRMapping;
        }

        public STARSMappingDataDTO CopyMappingData(STARSMappingDataDTO objParam)
        {
            log.MethodStart();

            var currentDateTime = DateTime.Now;
            STARSMappingDataDTO objReturn = new STARSMappingDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                #region Create Mapping...
                var Mapping =  _db.TRSTARSMappings.FirstOrDefault(x => x.smp_id == objParam.smp_id);
                Mapping.smp_id = Guid.NewGuid();
                Mapping.smp_mst_code = objParam.smp_mst_code;
                Mapping.smp_status = 'E';
                Mapping.smp_version = Math.Floor(objParam.smp_version);
                Mapping.smp_machinetype = objParam.smp_machinetype;
                Mapping.smp_startdate = objParam.smp_startdate;
                Mapping.smp_enddate = null;
                Mapping.smp_createuser = objParam.smp_createuser;
                Mapping.smp_createdate = currentDateTime;
                Mapping.smp_updateuser = objParam.smp_createuser;
                Mapping.smp_updatedate = currentDateTime;
                _db.TRSTARSMappings.Add(Mapping);
                #endregion

                #region Create WHONetMapping...

                var WHONetMapping = _db.TRSTARSWHONetMappings.Where(w => w.swm_mappingid == objParam.smp_id
                                                               && w.swm_flagdelete == false);


                if (WHONetMapping != null)
                {
                    foreach (var w in WHONetMapping)
                    {
                        w.swm_id = Guid.NewGuid();
                        w.swm_mappingid = Mapping.smp_id;
                        w.swm_status = 'N';
                        w.swm_createuser = objParam.smp_createuser;
                        w.swm_createdate = currentDateTime;
                        w.swm_updateuser = objParam.smp_createuser;
                        w.swm_updatedate = null;
                    }

                    _db.TRSTARSWHONetMappings.AddRange(WHONetMapping);
                }
                #endregion

                #region Create SpecimenMapping...
                var SpecimenMapping = _db.TRSTARSSpecimenMappings.Where(s => s.ssm_mappingid == objParam.smp_id
                                                               && s.ssm_flagdelete == false);

                if (SpecimenMapping != null)
                {
                    foreach (var s in SpecimenMapping)
                    {
                        s.ssm_id = Guid.NewGuid();
                        s.ssm_mappingid = Mapping.smp_id;
                        s.ssm_status = 'N';
                        s.ssm_createuser = objParam.smp_createuser;
                        s.ssm_createdate = currentDateTime;
                        s.ssm_updateuser = objParam.smp_createuser;
                        s.ssm_updatedate = currentDateTime;
                    }

                    _db.TRSTARSSpecimenMappings.AddRange(SpecimenMapping);
                }
                #endregion

                #region Create OrganismMapping...
                var OrganismMapping = _db.TRSTARSOrganismMappings.Where(o => o.som_mappingid == objParam.smp_id
                                                               && o.som_flagdelete == false);

                if (OrganismMapping != null)
                {
                    foreach (var o in OrganismMapping)
                    {
                        o.som_id = Guid.NewGuid();
                        o.som_mappingid = Mapping.smp_id;
                        o.som_status = 'N';
                        o.som_createuser = objParam.smp_createuser;
                        o.som_createdate = currentDateTime;
                        o.som_updateuser = objParam.smp_createuser;
                        o.som_updatedate = currentDateTime;
                    }

                    _db.TRSTARSOrganismMappings.AddRange(OrganismMapping);
                }
                #endregion

                #region Save Log Process ...
                _db.LogProcesss.Add(new LogProcess()
                {
                    log_usr_id = (Mapping.smp_updateuser ?? Mapping.smp_createuser),
                    log_mnu_id = "",
                    log_mnu_name = "Mapping",
                    log_tran_id = Mapping.smp_id.ToString(),                   
                    log_action = (Mapping.smp_status == 'N' ? "New" : "Update"),
                    log_desc = "Copy Mapping " ,
                    log_createuser = "SYSTEM",
                    log_createdate = currentDateTime
                });
                #endregion

                _db.SaveChanges();

                trans.Commit();

                objReturn = _mapper.Map<STARSMappingDataDTO>(Mapping);
            }
            log.MethodFinish();
                return objReturn;
        }

        public STARSMappingDataDTO GetMappingDataWithModel(STARSMappingDataDTO model)
        {
            log.MethodStart();
            STARSMappingDataDTO objModel = new STARSMappingDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objReturn1 = _db.TRSTARSMappings.OrderByDescending(x => x.smp_version).FirstOrDefault(x => x.smp_machinetype == model.smp_machinetype
                                                             && x.smp_enddate == null
                                                             && x.smp_flagdelete == false);


                    if (objReturn1 != null)
                        objModel = _mapper.Map<STARSMappingDataDTO>(objReturn1);

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

        public STARSMappingDataDTO chkDuplicateMappingApproved(STARSMappingDataDTO model)
        {
            log.MethodStart();
            STARSMappingDataDTO objModel = new STARSMappingDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objReturn1 = _db.TRSTARSMappings.OrderByDescending(x => x.smp_version).FirstOrDefault(x => x.smp_machinetype == model.smp_machinetype
                                                             && x.smp_startdate > model.smp_startdate
                                                             && x.smp_status == 'A'
                                                             && x.smp_flagdelete == false);


                    if (objReturn1 != null)
                        objModel = _mapper.Map<STARSMappingDataDTO>(objReturn1);

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
        
        public STARSMappingDataDTO GetMappingDataActiveWithModel(STARSMappingDataDTO model)
        {
            log.MethodStart();
            STARSMappingDataDTO objModel = new STARSMappingDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                	var objDataList = _db.TRSTARSMappings.FromSqlRaw<TRSTARSMapping>("sp_GET_TRSTARSMapping_Approve_Active {0} ,{1}", model.smp_machinetype, model.smp_id).ToList();
                   // var objDataList = new List<TRMapping>();
                   // if (model.smp_program == "MLAB")
                   // {
                   //      objDataList = _db.TRSTARSMappings.FromSqlRaw<TRMapping>("sp_GET_TRMapping_Approve_Active {0} ,{1} ,{2} ,{3} ,{4}", model.smp_hos_code, model.smp_lab, model.smp_program, model.smp_filetype, model.smp_id).ToList();
                   // }
                   // else
                   // {
                   //      objDataList = _db.TRSTARSMappings.FromSqlRaw<TRMapping>("sp_GET_TRMapping_Approve_Active {0} ,{1} ,{2} ,{3} ", model.smp_hos_code, model.smp_lab, model.smp_program, model.smp_filetype).ToList();
                   // }
                    

                    var objListMapping = _mapper.Map<List<STARSMappingDataDTO>>(objDataList);

                    if (objListMapping.Count > 0)
                    {
                        objModel = objListMapping.FirstOrDefault();
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

            return objModel;
        }

        #endregion

        #region WHONetMapping

        public STARSWHONetMappingDataDTO SaveWHONetMappingData(STARSWHONetMappingDataDTO model)
        {
            log.MethodStart();

            var currentDateTime = DateTime.Now;
            STARSWHONetMappingDataDTO objReturn = new STARSWHONetMappingDataDTO();
            bool chkUpdate = false;

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objModel = new TRSTARSWHONetMapping();
                    objModel = _db.TRSTARSWHONetMappings.FirstOrDefault(x => x.swm_id == model.swm_id);
                    if (objModel == null)
                    {
                        objModel = _mapper.Map<TRSTARSWHONetMapping>(model);

                        objModel.swm_createdate = currentDateTime;
                        objModel.swm_updatedate = currentDateTime;

                        _db.TRSTARSWHONetMappings.Add(objModel);
                        chkUpdate = true;
                    }
                    else
                    {
                        if (
                            objModel.swm_flagdelete != model.swm_flagdelete ||
                            objModel.swm_whonetfield != model.swm_whonetfield ||
                            objModel.swm_originalfield != model.swm_originalfield ||
                            objModel.swm_type != model.swm_type ||
                            objModel.swm_fieldlength != model.swm_fieldlength ||
                            objModel.swm_fieldformat != model.swm_fieldformat ||
                            objModel.swm_encrypt != model.swm_encrypt ||
                            objModel.swm_mandatory != model.swm_mandatory ||
                            objModel.swm_antibioticcolumn != model.swm_antibioticcolumn ||
                            objModel.swm_antibiotic != model.swm_antibiotic)
                        {
                            objModel.swm_status = model.swm_status;
                            objModel.swm_flagdelete = model.swm_flagdelete;
                            objModel.swm_whonetfield = model.swm_whonetfield;
                            objModel.swm_originalfield = model.swm_originalfield;
                            objModel.swm_type = model.swm_type;
                            objModel.swm_fieldlength = model.swm_fieldlength;
                            objModel.swm_fieldformat = model.swm_fieldformat;
                            objModel.swm_encrypt = model.swm_encrypt;
                            objModel.swm_mandatory = model.swm_mandatory;
                            objModel.swm_antibioticcolumn = model.swm_antibioticcolumn;
                            objModel.swm_antibiotic = model.swm_antibiotic;
                            objModel.swm_updateuser = model.swm_updateuser;
                            objModel.swm_updatedate = currentDateTime;
                            chkUpdate = true;
                        }
                    }
                    if (chkUpdate == true)
                    {
                        #region Update Mapping Version..

                        var objMapping = _db.TRSTARSMappings.FirstOrDefault(x => x.smp_id == model.swm_mappingid);
                        if (objMapping != null)
                        {
                            objMapping = PrepareMappingUpdateVersion(objMapping, model.swm_updateuser, currentDateTime);
                        }

                        #endregion

                        #region Save Log Process ...
                        _db.LogProcesss.Add(new LogProcess()
                        {
                            log_usr_id = (objModel.swm_updateuser ?? objModel.swm_createuser),
                            log_mnu_id = "",
                            log_mnu_name = "WHONetMapping",
                            log_tran_id = objModel.swm_id.ToString(),
                            log_action = (objModel.swm_status == 'N' ? "New" : "Update"),
                            log_desc = "Update WHONetMapping ",
                            log_createuser = "SYSTEM",
                            log_createdate = currentDateTime
                        });
                        #endregion
                    }
                    _db.SaveChanges();

                    trans.Commit();

                    objReturn = _mapper.Map<STARSWHONetMappingDataDTO>(objModel);
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

        public List<STARSWHONetMappingListsDTO> GetWHONetMappingListWithModel(STARSWHONetMappingSearch searchModel)
        {
            log.MethodStart();
            List<STARSWHONetMappingListsDTO> objList = new List<STARSWHONetMappingListsDTO>();
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    objList = _db.STARSWHONetMappingListsDTOs.FromSqlRaw<STARSWHONetMappingListsDTO>("sp_GET_TRSTARSWHONetMappingList {0}, {1}", searchModel.swm_mappingid, searchModel.swm_mst_code).ToList();

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


        public STARSWHONetMappingDataDTO GetWHONetMappingData(string swm_id)
        {
            log.MethodStart();
            STARSWHONetMappingDataDTO objModel = new STARSWHONetMappingDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objReturn1 = _db.TRSTARSWHONetMappings.FirstOrDefault(x => x.swm_id.ToString() == swm_id);

                    objModel = _mapper.Map<STARSWHONetMappingDataDTO>(objReturn1);

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


        public STARSWHONetMappingDataDTO GetWHONetMappingDataWithModel(STARSWHONetMappingDataDTO model)
        {
            log.MethodStart();
            STARSWHONetMappingDataDTO objModel = new STARSWHONetMappingDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objReturn1 = _db.TRSTARSWHONetMappings.FirstOrDefault(x => x.swm_mappingid == model.swm_mappingid
                                                             && (x.swm_whonetfield == model.swm_whonetfield
                                                             || x.swm_originalfield == model.swm_originalfield)
                                                             && x.swm_flagdelete == false);

                    if (objReturn1 != null)
                        objModel = _mapper.Map<STARSWHONetMappingDataDTO>(objReturn1);

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

        #endregion

        #region SpecimenMapping
        public STARSSpecimenMappingDataDTO SaveSpecimenMappingData(STARSSpecimenMappingDataDTO model)
        {
            log.MethodStart();

            var currentDateTime = DateTime.Now;
            STARSSpecimenMappingDataDTO objReturn = new STARSSpecimenMappingDataDTO();
            bool chkUpdate = false;

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objModel = new TRSTARSSpecimenMapping();
                    objModel = _db.TRSTARSSpecimenMappings.FirstOrDefault(x => x.ssm_id == model.ssm_id);
                    if (objModel == null)
                    {
                        objModel = _mapper.Map<TRSTARSSpecimenMapping>(model);

                        objModel.ssm_createdate = currentDateTime;
                        objModel.ssm_updatedate = currentDateTime;
                        chkUpdate = true;
                        _db.TRSTARSSpecimenMappings.Add(objModel);
                    }
                    else
                    {
                        if (
                        objModel.ssm_flagdelete != model.ssm_flagdelete ||
                        objModel.ssm_whonetcode != model.ssm_whonetcode ||
                        objModel.ssm_localspecimencode != model.ssm_localspecimencode ||
                        objModel.ssm_localspecimendesc != model.ssm_localspecimendesc)
                        {
                            objModel.ssm_status = model.ssm_status;
                            objModel.ssm_flagdelete = model.ssm_flagdelete;
                            objModel.ssm_whonetcode = model.ssm_whonetcode;
                            objModel.ssm_localspecimencode = model.ssm_localspecimencode;
                            objModel.ssm_localspecimendesc = model.ssm_localspecimendesc;
                            objModel.ssm_updateuser = model.ssm_updateuser;
                            objModel.ssm_updatedate = currentDateTime;
                            chkUpdate = true;
                        }
                    }

                    if (chkUpdate == true)
                    {
                        #region Update Mapping Version..

                        var objMapping = _db.TRSTARSMappings.FirstOrDefault(x => x.smp_id == model.ssm_mappingid);
                        if (objMapping != null)
                        {
                            objMapping = PrepareMappingUpdateVersion(objMapping, model.ssm_updateuser, currentDateTime);
                        }
                        #endregion


                        #region Save Log Process ...
                        _db.LogProcesss.Add(new LogProcess()
                        {
                            log_usr_id = (objModel.ssm_updateuser ?? objModel.ssm_createuser),
                            log_mnu_id = "",
                            log_mnu_name = "SpecimenMapping",
                            log_tran_id = objModel.ssm_id.ToString(),
                            log_action = (objModel.ssm_status == 'N' ? "New" : "Update"),
                            log_desc = "Update SpecimenMapping ",
                            log_createuser = "SYSTEM",
                            log_createdate = currentDateTime
                        });
                        #endregion
                    }

                    _db.SaveChanges();

                    trans.Commit();

                    objReturn = _mapper.Map<STARSSpecimenMappingDataDTO>(objModel);
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

        public List<STARSSpecimenMappingListsDTO> GetSpecimenMappingListWithModel(STARSSpecimenMappingSearch searchModel)
        {
            log.MethodStart();
            List<STARSSpecimenMappingListsDTO> objList = new List<STARSSpecimenMappingListsDTO>();
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    objList = _db.STARSSpecimenMappingListsDTOs.FromSqlRaw<STARSSpecimenMappingListsDTO>("sp_GET_TRSTARSSpecimenMappingList {0}, {1}", searchModel.ssm_mappingid, searchModel.ssm_mst_code).ToList();

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

        public STARSSpecimenMappingDataDTO GetSpecimenMappingData(string ssm_id)
        {
            log.MethodStart();
            STARSSpecimenMappingDataDTO objModel = new STARSSpecimenMappingDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objReturn1 = _db.TRSTARSSpecimenMappings.FirstOrDefault(x => x.ssm_id.ToString() == ssm_id);

                    objModel = _mapper.Map<STARSSpecimenMappingDataDTO>(objReturn1);

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

        public STARSSpecimenMappingDataDTO GetSpecimenMappingDataWithModel(STARSSpecimenMappingDataDTO model)
        {
            log.MethodStart();
            STARSSpecimenMappingDataDTO objModel = new STARSSpecimenMappingDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objReturn1 = _db.TRSTARSSpecimenMappings.FirstOrDefault(x => x.ssm_mappingid == model.ssm_mappingid
                                                             //&& x.ssm_whonetcode == model.ssm_whonetcode
                                                             && x.ssm_localspecimencode.ToUpper() == model.ssm_localspecimencode.ToUpper()
                                                             //&& x.ssm_localspecimendesc == model.ssm_localspecimendesc
                                                             && x.ssm_flagdelete == false);

                    if (objReturn1 != null)
                        objModel = _mapper.Map<STARSSpecimenMappingDataDTO>(objReturn1);

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
        #endregion

        #region OrganismMapping

        public STARSOrganismMappingDataDTO SaveOrganismMappingData(STARSOrganismMappingDataDTO model)
        {
            log.MethodStart();

            var currentDateTime = DateTime.Now;
            STARSOrganismMappingDataDTO objReturn = new STARSOrganismMappingDataDTO();
            bool chkUpdate = false;

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objModel = new TRSTARSOrganismMapping();
                    objModel = _db.TRSTARSOrganismMappings.FirstOrDefault(x => x.som_id == model.som_id);
                    if (objModel == null)
                    {
                        objModel = _mapper.Map<TRSTARSOrganismMapping>(model);

                        objModel.som_createdate = currentDateTime;
                        objModel.som_updatedate = currentDateTime;
                        chkUpdate = true;
                        _db.TRSTARSOrganismMappings.Add(objModel);
                    }
                    else
                    {
                        if (
                        objModel.som_flagdelete != model.som_flagdelete ||
                        objModel.som_whonetcode != model.som_whonetcode ||
                        objModel.som_whonetdesc != model.som_whonetdesc ||
                        objModel.som_localorganismcode != model.som_localorganismcode ||
                        objModel.som_localorganismdesc != model.som_localorganismdesc)
                        {
                            objModel.som_status = model.som_status;
                            objModel.som_flagdelete = model.som_flagdelete;
                            objModel.som_whonetcode = model.som_whonetcode;
                            objModel.som_whonetdesc = model.som_whonetdesc;
                            objModel.som_localorganismcode = model.som_localorganismcode;
                            objModel.som_localorganismdesc = model.som_localorganismdesc;
                            objModel.som_updateuser = model.som_updateuser;
                            objModel.som_updatedate = currentDateTime;
                            chkUpdate = true;
                        }
                    }

                    if (chkUpdate == true)
                    {
                        #region Update Mapping Version..

                        var objMapping = _db.TRSTARSMappings.FirstOrDefault(x => x.smp_id == model.som_mappingid);
                        if (objMapping != null)
                        {
                            objMapping = PrepareMappingUpdateVersion(objMapping, model.som_updateuser, currentDateTime);
                        }
                        #endregion

                        #region Save Log Process ...
                        _db.LogProcesss.Add(new LogProcess()
                        {
                            log_usr_id = (objModel.som_updateuser ?? objModel.som_createuser),
                            log_mnu_id = "",
                            log_mnu_name = "OrganismMapping",
                            log_tran_id = objModel.som_id.ToString(),
                            log_action = (objModel.som_status == 'N' ? "New" : "Update"),
                            log_desc = "Update OrganismMapping ",
                            log_createuser = "SYSTEM",
                            log_createdate = currentDateTime
                        });
                        #endregion
                    }
                    _db.SaveChanges();

                    trans.Commit();

                    objReturn = _mapper.Map<STARSOrganismMappingDataDTO>(objModel);
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


        public List<STARSOrganismMappingListsDTO> GetOrganismMappingListWithModel(STARSOrganismMappingSearch searchModel)
        {
            log.MethodStart();
            List<STARSOrganismMappingListsDTO> objList = new List<STARSOrganismMappingListsDTO>();
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    objList = _db.STARSOrganismMappingListsDTOs.FromSqlRaw<STARSOrganismMappingListsDTO>("sp_GET_TRSTARSOrganismMappingList {0}, {1}", searchModel.som_mappingid, searchModel.som_mst_code).ToList();

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

        public STARSOrganismMappingDataDTO GetOrganismMappingData(string som_id)
        {
            log.MethodStart();
            STARSOrganismMappingDataDTO objModel = new STARSOrganismMappingDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objReturn1 = _db.TRSTARSOrganismMappings.FirstOrDefault(x => x.som_id.ToString() == som_id);

                    objModel = _mapper.Map<STARSOrganismMappingDataDTO>(objReturn1);

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

        public STARSOrganismMappingDataDTO GetOrganismMappingDataWithModel(STARSOrganismMappingDataDTO model)
        {
            log.MethodStart();
            STARSOrganismMappingDataDTO objModel = new STARSOrganismMappingDataDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objReturn1 = _db.TRSTARSOrganismMappings.FirstOrDefault(x => x.som_mappingid == model.som_mappingid
                                                             //&& x.som_whonetcode == model.som_whonetcode
                                                             && x.som_localorganismcode.ToUpper() == model.som_localorganismcode.ToUpper()
                                                             //&& x.som_localorganismdesc == model.som_localorganismdesc
                                                             && x.som_flagdelete == false);

                    if (objReturn1 != null)
                        objModel = _mapper.Map<STARSOrganismMappingDataDTO>(objReturn1);

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

        #endregion
    }
}


