using ALISS.HISUpload.Batch.HDCData.DataAcess;
using ALISS.HISUpload.Batch.HDCData.Models;
using System.Collections.Generic;

namespace ALISS.HISUpload.Batch.HDCData.DataRepos
{
    public interface ITRSTGHISFileUploadDetailRepo
    {
        List<TRSTGHISFileUploadDetail> getTRSTGHISFileUploadDetailListByHosCode(string huh_hos_code);
    }
}