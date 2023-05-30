using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class TRProcessRequestHISData
    {
        public int pch_id { get; set; }
        public string pch_arh_code { get; set; }
        public string pch_prv_code { get; set; }
        public string pch_hos_code { get; set; }
        public string pch_lab_code { get; set; }
        public Guid? pch_lfu_id { get; set; }
        public int pch_year { get; set; }
        public int? pch_M01_qty { get; set; }
        public int? pch_M02_qty { get; set; }
        public int? pch_M03_qty { get; set; }
        public int? pch_M04_qty { get; set; }
        public int? pch_M05_qty { get; set; }
        public int? pch_M06_qty { get; set; }
        public int? pch_M07_qty { get; set; }
        public int? pch_M08_qty { get; set; }
        public int? pch_M09_qty { get; set; }
        public int? pch_M10_qty { get; set; }
        public int? pch_M11_qty { get; set; }
        public int? pch_M12_qty { get; set; }
        public string pch_status { get; set; }
        public bool pch_active { get; set; }
        public string pch_createuser { get; set; }
        public DateTime? pch_createdate { get; set; }
        public string pch_updateuser { get; set; }
        public DateTime? pch_updatedate { get; set; }
    }
}
