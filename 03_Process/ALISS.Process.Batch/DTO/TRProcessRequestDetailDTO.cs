using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.DTO
{
    public class TRProcessRequestDetailDTO
    {
        public string pcd_id { get; set; }
        public string pcd_pcr_code { get; set; }
        public string pcd_hos_code { get; set; }
        public string pcd_status { get; set; }
        public string pcd_active { get; set; }
        public string pcd_createuser { get; set; }
        public string pcd_createdate { get; set; }
        public string pcd_updateuser { get; set; }
        public string pcd_updatedate { get; set; }
    }
}
