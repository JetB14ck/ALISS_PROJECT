using ALISS_AUTH.TC.Role.DataAccess;
using ALISS_AUTH.TC.Role.Models;
using AutoMapper;
using Log4NetLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALISS_AUTH.TC.Role
{
    public class TCRolePermissionDAC : ITCRolePermissionDAC
    {
        private static readonly ILogService log = new LogService(typeof(TCRolePermissionDAC));

        private readonly IMapper _mapper;
        private readonly ALISS_AUTHContext _db;

        public TCRolePermissionDAC(IMapper mapper)
        {
            _mapper = mapper;
            _db = new ALISS_AUTHContext();
        }

        public void Insert(TCRolePermission model)
        {
            log.MethodStart();

            var objData = new TCRolePermission();
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var result = _db.TCRolePermissions.Add(model);

                    _db.SaveChanges();

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    log.Error(ex);

                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();
        }

        public void Update(TCRolePermission model)
        {
            log.MethodStart();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objData = _db.TCRolePermissions.FirstOrDefault(x => x.rop_id == model.rop_id);

                    if (objData != null)
                    {
                        objData = _mapper.Map<TCRolePermission>(model);
                    }

                    _db.SaveChanges();

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    log.Error(ex);

                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();
        }

        public void Inactive(TCRolePermission model)
        {
            log.MethodStart();

            model.rop_status = "I";
            model.rop_active = false;

            try
            {
                Update(model);
            }
            catch (Exception ex)
            {
                // TODO: Handle failure
                log.Error(ex);
            }
            finally
            {
            }

            log.MethodFinish();
        }

        public TCRolePermission GetData(TCRolePermission searchModel)
        {
            log.MethodStart();

            var objData = new TCRolePermission();
            try
            {
                objData = GetList(searchModel)?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // TODO: Handle failure
                log.Error(ex);
            }
            finally
            {
            }

            log.MethodFinish();

            return objData;
        }

        public List<TCRolePermission> GetList(TCRolePermission searchModel)
        {
            log.MethodStart();

            var objList = new List<TCRolePermission>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    objList = _db.TCRolePermissions.FromSqlRaw<TCRolePermission>("sp_GET_TCRolePermission {0}", searchModel).ToList();

                    _db.SaveChanges();

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    log.Error(ex);

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

    }
}
