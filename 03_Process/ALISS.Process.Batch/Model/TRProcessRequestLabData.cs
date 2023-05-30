using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class TRProcessRequestLabData
    {
        public int pcl_id { get; set; }
        public string pcl_arh_code { get; set; }
        public string pcl_prv_code { get; set; }
        public string pcl_hos_code { get; set; }
        public string pcl_lab_code { get; set; }
        public Guid? pcl_lfu_id { get; set; }
        public int pcl_year { get; set; }
        public int? pcl_M01_qty { get; set; }
        public int? pcl_M02_qty { get; set; }
        public int? pcl_M03_qty { get; set; }
        public int? pcl_M04_qty { get; set; }
        public int? pcl_M05_qty { get; set; }
        public int? pcl_M06_qty { get; set; }
        public int? pcl_M07_qty { get; set; }
        public int? pcl_M08_qty { get; set; }
        public int? pcl_M09_qty { get; set; }
        public int? pcl_M10_qty { get; set; }
        public int? pcl_M11_qty { get; set; }
        public int? pcl_M12_qty { get; set; }
        public string pcl_HQ01 { get; set; }
        public string pcl_HQ02 { get; set; }
        public string pcl_HQ03 { get; set; }
        public string pcl_HQ04 { get; set; }
        public string pcl_PQ01 { get; set; }
        public string pcl_PQ02 { get; set; }
        public string pcl_PQ03 { get; set; }
        public string pcl_PQ04 { get; set; }
        public string pcl_AQ01 { get; set; }
        public string pcl_AQ02 { get; set; }
        public string pcl_AQ03 { get; set; }
        public string pcl_AQ04 { get; set; }
        public string pcl_NQ01 { get; set; }
        public string pcl_NQ02 { get; set; }
        public string pcl_NQ03 { get; set; }
        public string pcl_NQ04 { get; set; }
        public string pcl_status { get; set; }
        public bool pcl_active { get; set; }
        public string pcl_createuser { get; set; }
        public DateTime? pcl_createdate { get; set; }
        public string pcl_updateuser { get; set; }
        public DateTime? pcl_updatedate { get; set; }
    }
}
