using ALISS.HISUpload.Batch.HDCData.Models;
using System.Collections.Generic;

namespace ALISS.HISUpload.Batch.HDCData.DataRepos
{
    public interface ITRSTGHISFileUploadHeaderRepo
    {
        List<TRSTGHISFileUploadHeader> Get_DataList_Auto();
        List<TRSTGHISFileUploadHeader> Get_DataList_Manual(string hos_code);
    }
}