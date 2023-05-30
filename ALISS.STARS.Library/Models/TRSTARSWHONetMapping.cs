using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.STARS.Library.Models
{
    public class TRSTARSWHONetMapping
    {
        public Guid swm_id { get; set; }
        public char swm_status { get; set; }
        public bool? swm_flagdelete { get; set; }
        public Guid swm_mappingid { get; set; }
        public string swm_whonetfield { get; set; }
        public string swm_originalfield { get; set; }
        public string swm_type { get; set; }
        public int swm_fieldlength { get; set; }
        public string swm_fieldformat { get; set; }
        public bool? swm_encrypt { get; set; }
        public bool? swm_mandatory { get; set; }
        public string swm_antibioticcolumn { get; set; }
        public string swm_antibiotic { get; set; }
        public string swm_createuser { get; set; }
        public DateTime? swm_createdate { get; set; }
        public string swm_updateuser { get; set; }
        public DateTime? swm_updatedate { get; set; }
    }
}
