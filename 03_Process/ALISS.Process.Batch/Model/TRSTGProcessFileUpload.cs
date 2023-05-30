using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class TRSTGProcessFileUpload
    {
        public Guid pfu_id { get; set; }
        public char pfu_status { get; set; }
        public int pfu_version { get; set; }
        public bool? pfu_flagdelete { get; set; }
        public Guid pfu_mp_id { get; set; }
        public decimal pfu_mp_version { get; set; }
        public string pfu_arh_code { get; set; }
        public string pfu_prv_code { get; set; }
        public string pfu_hos_code { get; set; }
        public string pfu_lab { get; set; }
        public string pfu_FileName { get; set; }
        public string pfu_Program { get; set; }
        public string pfu_Path { get; set; }
        public string pfu_FileType { get; set; }
        public int pfu_TotalRecord { get; set; }
        public int pfu_ErrorRecord { get; set; }
        public DateTime? pfu_StartDatePeriod { get; set; }
        public DateTime? pfu_EndDatePeriod { get; set; }
        public string pfu_createuser { get; set; }
        public DateTime? pfu_createdate { get; set; }
        public string pfu_approveduser { get; set; }
        public DateTime? pfu_approveddate { get; set; }
        public string pfu_updateuser { get; set; }
        public DateTime? pfu_updatedate { get; set; }
    }
}
