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
    public class TCRoleDAC : ITCRoleDAC
    {
        private static readonly ILogService log = new LogService(typeof(TCRoleDAC));

        private readonly IMapper _mapper;
        private readonly ALISS_AUTHContext _db;

        public TCRoleDAC(IMapper mapper)
        {
            _mapper = mapper;
            _db = new ALISS_AUTHContext();
        }

        public void Insert(TCRole model)
        {
            log.MethodStart();

            var objData = new TCRole();
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var result = _db.TCRoles.Add(model);

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

        public void Update(TCRole model)
        {
            log.MethodStart();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objData = _db.TCRoles.FirstOrDefault(x => x.rol_id == model.rol_id);

                    if (objData != null)
                    {
                        objData = _mapper.Map<TCRole>(model);
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

        public void Inactive(TCRole model)
        {
            log.MethodStart();

            model.rol_status = "I";
            model.rol_active = false;

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

        public TCRole GetData(TCRole searchModel)
        {
            log.MethodStart();

            var objData = new TCRole();
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

        public List<TCRole> GetList(TCRole searchModel)
        {
            log.MethodStart();

            var objList = new List<TCRole>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    objList = _db.TCRoles.FromSqlRaw<TCRole>("sp_GET_TCRole {0}", searchModel).ToList();

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
