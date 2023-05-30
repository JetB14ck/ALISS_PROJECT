using ALISS.HISUpload.Batch.HDCData.DataAcess;
using ALISS.HISUpload.Batch.HDCData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.HISUpload.Batch.HDCData.DataRepos
{
    public class TRSTGHISFileUploadDetailRepo : ITRSTGHISFileUploadDetailRepo
    {
        public readonly HDCDataContext _context;

        public TRSTGHISFileUploadDetailRepo(HDCDataContext context)
        {
            _context = context;
        }

        public List<TRSTGHISFileUploadDetail> getTRSTGHISFileUploadDetailListByHosCode(string huh_hos_code)
        {
            List<TRSTGHISFileUploadDetail> objList = new List<TRSTGHISFileUploadDetail>();
            objList = _context.TRSTGHISFileUploadDetails.FromSqlRaw<TRSTGHISFileUploadDetail>("sp_GET_TRSTGHISFileUploadDetailByHosCode {0}", huh_hos_code).ToList();
            return objList;
        }
    }
}
