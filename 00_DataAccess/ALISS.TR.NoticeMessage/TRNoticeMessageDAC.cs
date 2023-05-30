using ALISS.TR.NoticeMessage.DataAccess;
using ALISS.TR.NoticeMessage.Models;
using AutoMapper;
using Log4NetLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALISS.TR.NoticeMessage
{
    public class TRNoticeMessageDAC : ITRNoticeMessageDAC
    {
        private static readonly ILogService log = new LogService(typeof(TRNoticeMessageDAC));

        private readonly IMapper _mapper;
        private readonly ALISSContext _db;

        public TRNoticeMessageDAC(IMapper mapper)
        {
            _mapper = mapper;
            _db = new ALISSContext();
        }

        public void Insert(TRNoticeMessage model)
        {
            log.MethodStart();

            var objData = new TRNoticeMessage();
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var result = _db.TRNoticeMessageModel.Add(model);

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

        public void Update(TRNoticeMessage model)
        {
            log.MethodStart();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objData = _db.TRNoticeMessageModel.FirstOrDefault(x => x.noti_id == model.noti_id);

                    if (objData != null)
                    {
                        objData = _mapper.Map<TRNoticeMessage>(model);
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

        public void Inactive(TRNoticeMessage model)
        {
            log.MethodStart();

            model.noti_status = "I";
            model.noti_active = false;

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

        public TRNoticeMessage GetData(TRNoticeMessage searchModel)
        {
            log.MethodStart();

            var objData = new TRNoticeMessage();
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

        public List<TRNoticeMessage> GetList(TRNoticeMessage searchModel)
        {
            log.MethodStart();

            var objList = new List<TRNoticeMessage>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    objList = _db.TRNoticeMessageModel.FromSqlRaw<TRNoticeMessage>("sp_GET_TRNoticeMessage {0}", searchModel).ToList();

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

        public List<TRNoticeMessage> GetList_Active(TRNoticeMessage searchModel)
        {
            log.MethodStart();

            var objList = new List<TRNoticeMessage>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    objList = _db.TRNoticeMessageModel.FromSqlRaw<TRNoticeMessage>("sp_GET_TRNoticeMessage_Active {0}", searchModel).ToList();

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
