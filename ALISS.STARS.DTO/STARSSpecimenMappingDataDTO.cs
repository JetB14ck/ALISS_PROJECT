using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ALISS.STARS.DTO
{
    public class STARSSpecimenMappingDataDTO
    {
        public Guid ssm_id { get; set; }
        public char ssm_status { get; set; }
        public bool? ssm_flagdelete { get; set; }
        public Guid ssm_mappingid { get; set; }

        [Required (ErrorMessage = "WHONet Code is required")]
        [StringLength(50, ErrorMessage = "WHONet Code is too long.")]
        public string ssm_whonetcode { get; set; }

        [Required(ErrorMessage = "Source is required")]
        [StringLength(50, ErrorMessage = "Source is too long.")]
        public string ssm_localspecimencode { get; set; }

        [Required(ErrorMessage = "CSource is required")]
        [StringLength(200, ErrorMessage = "CSource is too long.")]
        public string ssm_localspecimendesc { get; set; }
        public string ssm_createuser { get; set; }
        public DateTime? ssm_createdate { get; set; }
        public string ssm_updateuser { get; set; }
        public DateTime? ssm_updatedate { get; set; }
    }
}
