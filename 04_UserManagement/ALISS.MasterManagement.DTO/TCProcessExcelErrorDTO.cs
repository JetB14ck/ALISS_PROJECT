using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.MasterManagement.DTO
{
    public class TCProcessExcelErrorDTO
    {
        public char tcp_status { get; set; }
        public char tcp_Err_type { get; set; }
        public string tcp_Err_SheetName { get; set; }
        public string tcp_Err_No { get; set; }
        public string tcp_Err_Message { get; set; }
    }
}
