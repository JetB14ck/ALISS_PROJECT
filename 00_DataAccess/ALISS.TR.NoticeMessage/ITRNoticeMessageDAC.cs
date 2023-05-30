using ALISS.TR.NoticeMessage.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.TR.NoticeMessage
{
    public interface ITRNoticeMessageDAC
    {
        void Insert(TRNoticeMessage model);
        void Update(TRNoticeMessage model);
        void Inactive(TRNoticeMessage model);
        TRNoticeMessage GetData(TRNoticeMessage searchModel);
        List<TRNoticeMessage> GetList(TRNoticeMessage searchModel);
        List<TRNoticeMessage> GetList_Active(TRNoticeMessage searchModel);
    }
}
