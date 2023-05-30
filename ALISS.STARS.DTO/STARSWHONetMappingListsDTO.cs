using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.STARS.DTO
{
    public class STARSWHONetMappingListsDTO
    {
        public Guid swm_id { get; set; }
        public Guid swm_mappingid { get; set; }
        public string swm_whonetfield { get; set; }
        public string swm_originalfield { get; set; }
        public string swm_type { get; set; }
        public string swm_fieldformat { get; set; }
        public int swm_fieldlength { get; set; }
        public bool? swm_encrypt { get; set; }
        public bool? swm_mandatory { get; set; }
        public string swm_antibioticcolumn { get; set; }
        public string swm_antibiotic { get; set; }
    }

    public class STARSWHONetMappingSearch
    {
        public Guid swm_mappingid { get; set; }

        public string swm_mst_code { get; set; }
    }
}
