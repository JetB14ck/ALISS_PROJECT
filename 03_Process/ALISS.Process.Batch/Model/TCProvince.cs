using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class TCProvince
    {
        public string prv_code { get; set; }
        public string prv_arh_code { get; set; }
        public string prv_name { get; set; }
        public string prv_desc { get; set; }
        public string prv_status { get; set; }
        public string prv_createuser { get; set; }
        public DateTime? prv_createdate { get; set; }
        public string prv_updateuser { get; set; }
        public DateTime? prv_updatedate { get; set; }
    }
}
