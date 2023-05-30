using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.MasterManagement.DTO
{
    public class TCProcessExcelRowDTO
    {
        public int per_id { get; set; }
        public string per_mst_code { get; set; }
        public string per_sheet_name { get; set; }
        public int? per_row_num { get; set; }
        public string per_macro_name { get; set; }
        public int? per_row_group_num { get; set; }
        public string per_row_group_name { get; set; }
        public string per_row_name { get; set; }
        public string per_createuser { get; set; }
        public DateTime? per_createdate { get; set; }
        public string per_updateuser { get; set; }
        public DateTime? per_updatedate { get; set; }
    }
}
