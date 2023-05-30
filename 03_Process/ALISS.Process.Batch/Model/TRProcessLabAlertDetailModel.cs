using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class TRProcessLabAlertDetailModel
    {
        public int plad_id { get; set; }
        public Guid? plad_ldd_id { get; set; }
        public Guid? plad_ldh_id { get; set; }
        public Guid? plad_lfu_id { get; set; }
        public string plad_originalfield { get; set; }
        public string plad_whonetfield { get; set; }
        public string plad_originalvalue { get; set; }
        public string plad_whonet_result { get; set; }
        public string plad_status { get; set; }
        public string plad_createuser { get; set; }
        public DateTime? plad_createdate { get; set; }
        public string plad_updateuser { get; set; }
        public DateTime? plad_updatedate { get; set; }
    }
}
