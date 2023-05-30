using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class TRProcessDataDetail
    {
        public Guid pdd_id { get; set; }
        public string pdd_status { get; set; }
        public string pdd_whonetfield { get; set; }
        public string pdd_originalfield { get; set; }
        public string pdd_originalvalue { get; set; }
        public string pdd_mappingvalue { get; set; }
        public Guid? pdd_ldh_id { get; set; }
        public string pdd_pcr_code { get; set; }
        public string pdd_createuser { get; set; }
        public DateTime? pdd_createdate { get; set; }
        public string pdd_updateuser { get; set; }
        public DateTime? pdd_updatedate { get; set; }
    }
}
