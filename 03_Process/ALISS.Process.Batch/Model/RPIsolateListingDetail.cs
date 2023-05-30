using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class RPIsolateListingDetail
    {
        public int id { get; set; }
        public int? header_id { get; set; }
        public Guid? header_iso_id { get; set; }
        public string anti_code { get; set; }
        public string anti_name { get; set; }
        public float? result_value { get; set; }
        public string interp_value { get; set; }
        public string remark { get; set; }
        public string createuser { get; set; }
        public DateTime? createdate { get; set; }
        public string updateuser { get; set; }
        public DateTime? updatedate { get; set; }
    }
}
