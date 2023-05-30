using ALISS_AUTH.TC.UserLogin.DataAccess;
using ALISS_AUTH.TC.UserLogin.Models;
using AutoMapper;
using Log4NetLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALISS_AUTH.TC.UserLogin
{
    public class TCUserLoginPermissionDAC : ITCUserLoginPermissionDAC
    {
        private static readonly ILogService log = new LogService(typeof(TCUserLoginPermissionDAC));

        private readonly IMapper _mapper;
        private readonly ALISS_AUTHContext _db;

        public TCUserLoginPermissionDAC(IMapper mapper)
        {
            _mapper = mapper;
            _db = new ALISS_AUTHContext();
        }

        public void Insert(TCUserLoginPermission model)
        {
            log.MethodStart();

            var objData = new TCUserLoginPermission();
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var result = _db.TCUserLoginPermissions.Add(model);

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

        public void Update(TCUserLoginPermission model)
        {
            log.MethodStart();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objData = _db.TCUserLoginPermissions.FirstOrDefault(x => x.usp_id == model.usp_id);

                    if (objData != null)
                    {
                        objData = _mapper.Map<TCUserLoginPermission>(model);
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

        public void Inactive(TCUserLoginPermission model)
        {
            log.MethodStart();

            model.usp_status = "I";
            model.usp_active = false;

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

        public TCUserLoginPermission GetData(TCUserLoginPermission searchModel)
        {
            log.MethodStart();

            var objData = new TCUserLoginPermission();
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

        public List<TCUserLoginPermission> GetList(TCUserLoginPermission searchModel)
        {
            log.MethodStart();

            var objList = new List<TCUserLoginPermission>();

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
