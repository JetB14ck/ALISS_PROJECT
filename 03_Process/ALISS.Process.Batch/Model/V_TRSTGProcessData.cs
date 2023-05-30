using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class V_TRSTGProcessData
    {
        public Guid pdd_id { get; set; }
        public Guid? pdd_ldh_id { get; set; }
        public string pdd_status { get; set; }
        public string pdd_whonetfield { get; set; }
        public string pdd_originalfield { get; set; }
        public string pdd_originalvalue { get; set; }
        public string pdd_mappingvalue { get; set; }
        public string pdd_createuser { get; set; }
        public DateTime? pdd_createdate { get; set; }
        public string pdd_updateuser { get; set; }
        public DateTime? pdd_updatedate { get; set; }
        public Guid pdh_id { get; set; }
        public string pdh_status { get; set; }
        public int? pdh_sequence { get; set; }
        public string pdh_hos_code { get; set; }
        public string pdh_lab { get; set; }
        public string pdh_labno { get; set; }
        public string pdh_date { get; set; }
        public DateTime? pdh_cdate { get; set; }
        public string pdh_organism { get; set; }
        public string pdh_specimen { get; set; }
        public Guid? pdh_lfu_id { get; set; }
        public string pdh_createuser { get; set; }
        public DateTime? pdh_createdate { get; set; }
        public string pdh_updateuser { get; set; }
        public DateTime? pdh_updatedate { get; set; }
    }
}
