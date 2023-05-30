using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.MasterManagement.DTO
{
    public class TCProcessExcelTemplateDTO
    {
        public int pet_id { get; set; }
        public string pet_mst_code { get; set; }
        public string pet_sheet_name { get; set; }
        public int? pet_col_num { get; set; }
        public int? pet_row_num { get; set; }
        public bool? pet_a { get; set; }
        public bool? pet_b { get; set; }
        public bool? pet_c { get; set; }
        public bool? pet_d { get; set; }
        public bool? pet_e { get; set; }
        public bool? pet_f { get; set; }
        public bool? pet_h { get; set; }
        public bool? pet_i { get; set; }
        public bool? pet_u { get; set; }
        public bool? pet_wt { get; set; }
        public bool? pet_r { get; set; }
        public string pet_site_inf { get; set; }
        public string pet_fix_value { get; set; }
        public string pet_cell_sup { get; set; }
        public bool? pet_merge { get; set; }
        public string pet_createuser { get; set; }
        public DateTime? pet_createdate { get; set; }
        public string pet_updateuser { get; set; }
        public DateTime? pet_updatedate { get; set; }
    }
}
