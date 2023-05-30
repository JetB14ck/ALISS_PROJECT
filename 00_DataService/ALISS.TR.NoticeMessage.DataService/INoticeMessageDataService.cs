using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.TR.NoticeMessage.DataService
{
    public interface INoticeMessageDataService
    {
        void CreateNoticeMessageAll(string mnu_code, string message);
        void CreateNoticeMessageByRole(string mnu_code, string rol_code, string message);
        void CreateNoticeMessageByUser(string mnu_code, string username, string message);
        void CreateNoticeMessageByHospital(string mnu_code, string hos_code, string message);
        void CreateNoticeMessageByArea(string mnu_code, string arh_code, string message);
    }
}
