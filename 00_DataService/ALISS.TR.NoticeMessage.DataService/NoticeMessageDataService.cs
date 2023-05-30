using ALISS.TR.NoticeMessage.DataAccess;
using ALISS.TR.NoticeMessage.Models;
using ALISS_AUTH.TC.UserLogin;
using ALISS_AUTH.TC.UserLogin.DataAccess;
using ALISS_AUTH.TC.UserLogin.Models;
using AutoMapper;
using Log4NetLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ALISS.TR.NoticeMessage.DataService
{
    public class NoticeMessageDataService : INoticeMessageDataService
    {
        private static readonly ILogService log = new LogService(typeof(NoticeMessageDataService));

        private readonly IMapper _mapper;
        private readonly ITCUserLoginPermissionDAC _userLoginPermissionDac;
        private readonly ITRNoticeMessageDAC _dac;

        public NoticeMessageDataService(IMapper mapper)
        {
            _mapper = mapper;
            _userLoginPermissionDac = new TCUserLoginPermissionDAC(mapper);
            _dac = new TRNoticeMessageDAC(mapper);
        }

        public void CreateNoticeMessageAll(string mnu_code, string message)
        {
            log.MethodStart();

            try
            {
                var userLoginList = _userLoginPermissionDac.GetList(new TCUserLoginPermission() { usp_active = true });

                foreach (var item in userLoginList.Select(x => x.usp_usr_userName).Distinct())
                {
                    var objData = new TRNoticeMessage()
                    {
                        noti_username = item,
                        noti_mnu_code = mnu_code,
                        noti_message = message
                    };

                    _dac.Insert(objData);
                }
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

            //return objList;
        }

        public void CreateNoticeMessageByRole(string mnu_code, string rol_code, string message)
        {
            log.MethodStart();

            try
            {
                var userLoginPermissionList = _userLoginPermissionDac.GetList(new TCUserLoginPermission() { usp_rol_code = rol_code });

                foreach (var item in userLoginPermissionList)
                {
                    var objData = new TRNoticeMessage()
                    {
                        noti_username = item.usp_usr_userName,
                        noti_mnu_code = mnu_code,
                        noti_message = message
                    };

                    _dac.Insert(objData);
                }
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

            //return objList;
        }

        public void CreateNoticeMessageByArea(string mnu_code, string arh_code, string message)
        {
            log.MethodStart();

            try
            {
                var userLoginPermissionList = _userLoginPermissionDac.GetList(new TCUserLoginPermission() { usp_arh_code = arh_code });

                foreach (var item in userLoginPermissionList)
                {
                    var objData = new TRNoticeMessage()
                    {
                        noti_username = item.usp_usr_userName,
                        noti_mnu_code = mnu_code,
                        noti_message = message
                    };

                    _dac.Insert(objData);
                }
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

            //return objList;
        }

        public void CreateNoticeMessageByHospital(string mnu_code, string hos_code, string message)
        {
            log.MethodStart();

            try
            {
                var userLoginPermissionList = _userLoginPermissionDac.GetList(new TCUserLoginPermission() { usp_hos_code = hos_code });

                foreach (var item in userLoginPermissionList)
                {
                    var objData = new TRNoticeMessage()
                    {
                        noti_username = item.usp_usr_userName,
                        noti_mnu_code = mnu_code,
                        noti_message = message
                    };

                    _dac.Insert(objData);
                }
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

            //return objList;
        }

        public void CreateNoticeMessageByUser(string mnu_code, string username, string message)
        {
            log.MethodStart();

            try
            {
                var objData = new TRNoticeMessage()
                {
                    noti_username = username,
                    noti_mnu_code = mnu_code,
                    noti_message = message
                };

                _dac.Insert(objData);
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

            //return objList;
        }
    }
}
