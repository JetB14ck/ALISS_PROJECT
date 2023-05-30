using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class TCProcessExcelColumn
    {
        public int pec_id { get; set; }
        public string pec_mst_code { get; set; }
        public string pec_sheet_name { get; set; }
        public int? pec_col_num { get; set; }
        public string pec_ant_code { get; set; }
        public int? pec_col_group_num { get; set; }
        public string pec_col_group_name { get; set; }
        public string pec_col_name { get; set; }
        public bool? pec_MIC { get; set; }
        public bool? pec_Urine { get; set; }
        public string pec_createuser { get; set; }
        public DateTime? pec_createdate { get; set; }
        public string pec_updateuser { get; set; }
        public DateTime? pec_updatedate { get; set; }
    }
}
