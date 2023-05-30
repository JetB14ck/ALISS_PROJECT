using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.STARS.Library.Models
{
    public class TRSTARSMapping
    {
        public Guid smp_id { get; set; }
        public string smp_mst_code { get; set; }
        public char smp_status { get; set; }
        public decimal smp_version { get; set; }
        public bool? smp_flagdelete { get; set; }
        public string smp_machinetype { get; set; }
        public bool? smp_AntibioticIsolateOneRec { get; set; }
        public bool? smp_firstlineisheader { get; set; }
        public string smp_dateformat { get; set; }
        public DateTime? smp_startdate { get; set; }
        public DateTime? smp_enddate { get; set; }
        public string smp_createuser { get; set; }
        public DateTime? smp_createdate { get; set; }
        public string smp_approveduser { get; set; }
        public DateTime? smp_approveddate { get; set; }
        public string smp_updateuser { get; set; }
        public DateTime? smp_updatedate { get; set; }
    }
}
