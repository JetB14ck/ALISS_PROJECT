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
    public class TCUserLoginDAC : ITCUserLoginDAC
    {
        private static readonly ILogService log = new LogService(typeof(TCUserLoginDAC));

        private readonly IMapper _mapper;
        private readonly ALISS_AUTHContext _db;

        public TCUserLoginDAC(IMapper mapper)
        {
            _mapper = mapper;
            _db = new ALISS_AUTHContext();
        }

        public void Insert(TCUserLogin model)
        {
            log.MethodStart();

            var objData = new TCUserLogin();
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var result = _db.TCUserLogins.Add(model);

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

        public void Update(TCUserLogin model)
        {
            log.MethodStart();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objData = _db.TCUserLogins.FirstOrDefault(x => x.usr_id == model.usr_id);

                    if (objData != null)
                    {
                        objData = _mapper.Map<TCUserLogin>(model);
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

        public void Inactive(TCUserLogin model)
        {
            log.MethodStart();

            model.usr_status = "I";
            model.usr_active = false;

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

        public TCUserLogin GetData(TCUserLogin searchModel)
        {
            log.MethodStart();

            var objData = new TCUserLogin();
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

        public List<TCUserLogin> GetList(TCUserLogin searchModel)
        {
            log.MethodStart();

            var objList = new List<TCUserLogin>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    objList = _db.TCUserLogins.FromSqlRaw<TCUserLogin>("sp_GET_TCUserLogin {0}", searchModel).ToList();

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
