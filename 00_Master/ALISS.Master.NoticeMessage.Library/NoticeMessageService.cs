using AutoMapper;
using Log4NetLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Master.NoticeMessage.Library
{
    public class NoticeMessageService : INoticeMessageService
    {
        private static readonly ILogService log = new LogService(typeof(NoticeMessageService));

        private readonly IMapper _mapper;

        public NoticeMessageService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public void CreateNoticeMessageAll(string mnu_code, string message)
        {
            log.MethodStart();

            try
            {

            }
            catch (Exception ex)
            {
                // TODO: Handle failure

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

            }
            catch (Exception ex)
            {
                // TODO: Handle failure

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

            }
            catch (Exception ex)
            {
                // TODO: Handle failure

            }
            finally
            {

            }

            log.MethodFinish();

            //return objList;
        }
    }
}
