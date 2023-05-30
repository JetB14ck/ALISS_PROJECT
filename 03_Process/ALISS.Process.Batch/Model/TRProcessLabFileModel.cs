using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class TRProcessLabFileModel
    {
        public int plf_id { get; set; }
        public Guid? plf_lfu_id { get; set; }
        public string plf_file_type { get; set; }
        public string plf_arh_code { get; set; }
        public string plf_prv_code { get; set; }
        public string plf_hos_code { get; set; }
        public string plf_lab { get; set; }
        public string plf_status { get; set; }
        public string plf_createuser { get; set; }
        public DateTime? plf_createdate { get; set; }
        public string plf_updateuser { get; set; }
        public DateTime? plf_updatedate { get; set; }
    }
}
