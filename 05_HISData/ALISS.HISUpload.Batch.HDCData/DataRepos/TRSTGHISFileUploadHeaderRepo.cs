using ALISS.HISUpload.Batch.HDCData.DataAcess;
using ALISS.HISUpload.Batch.HDCData.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.HISUpload.Batch.HDCData.DataRepos
{
    public class TRSTGHISFileUploadHeaderRepo : ITRSTGHISFileUploadHeaderRepo
    {
        public readonly HDCDataContext _context;
        private readonly IConfiguration _config;

        public TRSTGHISFileUploadHeaderRepo(HDCDataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public List<TRSTGHISFileUploadHeader> Get_DataList_Auto()
        {
            //TRSTHISFileUpload
            int backDays = 0;
            if (!string.IsNullOrEmpty(_config["HDCData_RUN_BATCH:HDCData_BACKDAYS"].ToString()))
            {
                int.TryParse(_config["HDCData_RUN_BATCH:HDCData_BACKDAYS"].ToString(), out backDays);
            }
            List<TRSTGHISFileUploadHeader> resultList = new List<TRSTGHISFileUploadHeader>();
            var TRHISFileUploadList = _context.TRHISFileUploads.Where(x => x.hfu_updatedate >= DateTime.Now.AddDays(-backDays).Date && x.hfu_status == 'F' && x.hfu_hdc_status != "F").ToList();
            foreach (TRHISFileUpload item in TRHISFileUploadList)
            {
                resultList.AddRange(Get_DataList_Manual(item.hfu_hos_code));
            }
            return resultList;
        }

        public List<TRSTGHISFileUploadHeader> Get_DataList_Manual(string hos_code)
        {
            return _context.TRSTGHISFileUploadHeaders.Where(x => x.huh_hos_code == hos_code && x.huh_status != 'D').ToList();
        }

    }
}
