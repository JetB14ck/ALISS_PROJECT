using AutoMapper;
using Log4NetLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ALISS.MasterManagement.DTO;
using ALISS.MasterManagement.Library.DataAccess;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ALISS.MasterManagement.Library.Models;

namespace ALISS.MasterManagement.Library
{
    public class ProcessExcelService : IProcessExcelService
    {
        private static readonly ILogService log = new LogService(typeof(MasterTemplateService));

        private readonly MasterManagementContext _db;
        private readonly IMapper _mapper;

        public ProcessExcelService(MasterManagementContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public List<TCProcessExcelColumnDTO> Get_ProcessExcelColumn(TCProcessExcelColumnDTO searchModel)
        {
            log.MethodStart();

            List<TCProcessExcelColumnDTO> objList = new List<TCProcessExcelColumnDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    objList = _db.TCProcessExcelColumnDTOs.FromSqlRaw<TCProcessExcelColumnDTO>("EXEC [dbo].[sp_GET_TCProcessExcelColumn] {0}, {1}", searchModel.pec_mst_code, searchModel.pec_id).ToList();

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

        public List<TCProcessExcelRowDTO> Get_ProcessExcelRow(TCProcessExcelRowDTO searchModel)
        {
            log.MethodStart();

            List<TCProcessExcelRowDTO> objList = new List<TCProcessExcelRowDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    objList = _db.TCProcessExcelRowDTOs.FromSqlRaw<TCProcessExcelRowDTO>("EXEC [dbo].[sp_GET_TCProcessExcelRow] {0}, {1}", searchModel.per_mst_code, searchModel.per_id).ToList();

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

        public List<TCProcessExcelTemplateDTO> Get_ProcessExcelTemplate(TCProcessExcelTemplateDTO searchModel)
        {
            log.MethodStart();

            List<TCProcessExcelTemplateDTO> objList = new List<TCProcessExcelTemplateDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    objList = _db.TCProcessExcelTemplateDTOs.FromSqlRaw<TCProcessExcelTemplateDTO>("EXEC [dbo].[sp_GET_TCProcessExcelTemplate] {0}, {1}", searchModel.pet_mst_code, searchModel.pet_id).ToList();

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

        public TCProcessExcelColumnDTO SaveProcessExcelColumnData(TCProcessExcelColumnDTO objModel)
        {
            log.MethodStart();

            var currentDateTime = DateTime.Now;
            TCProcessExcelColumnDTO objReturn = new TCProcessExcelColumnDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    if (objModel != null)
                    {
                        objModel = _mapper.Map<TCProcessExcelColumnDTO>(objModel);
                        objModel.pec_createdate = currentDateTime;
                        _db.TCProcessExcelColumnDTOs.Add(objModel);

                        #region Save Log Process ...
                        _db.LogProcesss.Add(new LogProcess()
                        {
                            log_usr_id = objModel.pec_createuser,
                            log_mnu_id = "",
                            log_mnu_name = "ProcessExcelColumn",
                            log_tran_id = $"{objModel.pec_id}:{objModel.pec_id}",
                            log_action = "New",
                            log_desc = "New" + " ProcessExcelColumn " + objModel.pec_col_name,
                            log_createuser = objModel.pec_createuser,
                            log_createdate = currentDateTime
                        });
                        #endregion

                        _db.SaveChanges();

                        trans.Commit();
                    }

                    objReturn = _mapper.Map<TCProcessExcelColumnDTO>(objModel);
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

        public TCProcessExcelRowDTO SaveProcessExcelRowData(TCProcessExcelRowDTO objModel)
        {
            log.MethodStart();

            var currentDateTime = DateTime.Now;
            TCProcessExcelRowDTO objReturn = new TCProcessExcelRowDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    if (objModel != null)
                    {
                        objModel = _mapper.Map<TCProcessExcelRowDTO>(objModel);
                        objModel.per_createdate = currentDateTime;
                        _db.TCProcessExcelRowDTOs.Add(objModel);

                        #region Save Log Process ...
                        _db.LogProcesss.Add(new LogProcess()
                        {
                            log_usr_id = objModel.per_createuser,
                            log_mnu_id = "",
                            log_mnu_name = "ProcessExcelRow",
                            log_tran_id = $"{objModel.per_id}:{objModel.per_id}",
                            log_action = "New",
                            log_desc = "New" + " ProcessExcelRow " + objModel.per_row_name,
                            log_createuser = objModel.per_createuser,
                            log_createdate = currentDateTime
                        });
                        #endregion

                        _db.SaveChanges();

                        trans.Commit();
                    }

                    objReturn = _mapper.Map<TCProcessExcelRowDTO>(objModel);
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

        public TCProcessExcelTemplateDTO SaveProcessExcelTemplateData(TCProcessExcelTemplateDTO objModel)
        {
            log.MethodStart();

            var currentDateTime = DateTime.Now;
            TCProcessExcelTemplateDTO objReturn = new TCProcessExcelTemplateDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    if (objModel != null)
                    {
                        objModel = _mapper.Map<TCProcessExcelTemplateDTO>(objModel);
                        objModel.pet_createdate = currentDateTime;
                        _db.TCProcessExcelTemplateDTOs.Add(objModel);

                        #region Save Log Process ...
                        _db.LogProcesss.Add(new LogProcess()
                        {
                            log_usr_id = objModel.pet_createuser,
                            log_mnu_id = "",
                            log_mnu_name = "ProcessExcelTemplate",
                            log_tran_id = $"{objModel.pet_id}:{objModel.pet_id}",
                            log_action = "New",
                            log_desc = "New" + " ProcessExcelTemplate " + objModel.pet_cell_sup,
                            log_createuser = objModel.pet_createuser,
                            log_createdate = currentDateTime
                        });
                        #endregion

                        _db.SaveChanges();

                        trans.Commit();
                    }

                    objReturn = _mapper.Map<TCProcessExcelTemplateDTO>(objModel);
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

        public TCProcessExcelColumnDTO SaveProcessExcelColumnListData(List<TCProcessExcelColumnDTO> objModels)
        {
            log.MethodStart();

            var currentDateTime = DateTime.Now;
            TCProcessExcelColumnDTO objReturn = new TCProcessExcelColumnDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    // remove old
                    

                    // save new
                    foreach (var objModel in objModels)
                    {
                        _db.TCProcessExcelColumnDTOs.Add(objModel);
                    }

                    _db.SaveChanges();

                    trans.Commit();

                    var objM = _db.TCProcessExcelColumnDTOs.FirstOrDefault();
                    objReturn = _mapper.Map<TCProcessExcelColumnDTO>(objM);
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

        public TCProcessExcelRowDTO SaveProcessExcelRowListData(List<TCProcessExcelRowDTO> objModels)
        {
            log.MethodStart();

            var currentDateTime = DateTime.Now;
            TCProcessExcelRowDTO objReturn = new TCProcessExcelRowDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var objModel in objModels)
                    {
                        _db.TCProcessExcelRowDTOs.Add(objModel);
                    }

                    _db.SaveChanges();

                    trans.Commit();

                    var objM = _db.TCProcessExcelRowDTOs.FirstOrDefault();
                    objReturn = _mapper.Map<TCProcessExcelRowDTO>(objM);
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

        public TCProcessExcelTemplateDTO SaveProcessExcelTemplateListData(List<TCProcessExcelTemplateDTO> objModels)
        {
            log.MethodStart();

            var currentDateTime = DateTime.Now;
            TCProcessExcelTemplateDTO objReturn = new TCProcessExcelTemplateDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var objModel in objModels)
                    {
                        _db.TCProcessExcelTemplateDTOs.Add(objModel);
                    }

                    _db.SaveChanges();

                    trans.Commit();

                    var objM = _db.TCProcessExcelTemplateDTOs.FirstOrDefault();
                    objReturn = _mapper.Map<TCProcessExcelTemplateDTO>(objM);
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

        public TCProcessExcelColumnDTO DeleteProcessExcelColumnListData(List<TCProcessExcelColumnDTO> objModels)
        {
            log.MethodStart();

            var obj = objModels.FirstOrDefault();
            TCProcessExcelColumnDTO objReturn = new TCProcessExcelColumnDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {

                    var objList = _db.TCProcessExcelColumnDTOs.FromSqlRaw<TCProcessExcelColumnDTO>("EXEC [dbo].[sp_GET_TCProcessExcelColumn] {0}", obj.pec_mst_code).ToList();
                   
                    foreach (var objModel in objList)
                    {
                        _db.TCProcessExcelColumnDTOs.Remove(objModel);
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

            log.MethodFinish();

            return objReturn;
        }

        public TCProcessExcelRowDTO DeleteProcessExcelRowListData(List<TCProcessExcelRowDTO> objModels)
        {
            log.MethodStart();

            var obj = objModels.FirstOrDefault();
            TCProcessExcelRowDTO objReturn = new TCProcessExcelRowDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objList = _db.TCProcessExcelRowDTOs.FromSqlRaw<TCProcessExcelRowDTO>("EXEC [dbo].[sp_GET_TCProcessExcelRow] {0}", obj.per_mst_code).ToList();
                   
                    _db.TCProcessExcelRowDTOs.RemoveRange(objList);                  
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

            log.MethodFinish();

            return objReturn;
        }
        public TCProcessExcelTemplateDTO DeleteProcessExcelTemplateListData(List<TCProcessExcelTemplateDTO> objModels)
        {
            log.MethodStart();

            var obj = objModels.FirstOrDefault();
            TCProcessExcelTemplateDTO objReturn = new TCProcessExcelTemplateDTO();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objList = _db.TCProcessExcelTemplateDTOs.FromSqlRaw<TCProcessExcelTemplateDTO>("EXEC [dbo].[sp_GET_TCProcessExcelTemplate] {0}", obj.pet_mst_code).ToList();

                   
                    _db.TCProcessExcelTemplateDTOs.RemoveRange(objList);                   
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

            log.MethodFinish();

            return objReturn;
        }
    }
}
