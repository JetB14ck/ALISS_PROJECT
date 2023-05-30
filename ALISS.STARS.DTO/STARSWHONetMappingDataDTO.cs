using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ALISS.STARS.DTO
{
    public class STARSWHONetMappingDataDTO
    {
        public Guid swm_id { get; set; }
        public char swm_status{ get; set; }
        public bool? swm_flagdelete { get; set; }
        public Guid swm_mappingid { get; set; }

        [Required(ErrorMessage = "WHONet Field is required")]
        [StringLength(50, ErrorMessage = "WHONet Field is too long.")]
        public string swm_whonetfield { get; set; }

        [Required(ErrorMessage = "Original Field is required")]
        [StringLength(50, ErrorMessage = "Original Field is too long.")]
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
