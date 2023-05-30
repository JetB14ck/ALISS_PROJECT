using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.LabFileUpload.DTO
{
    public class LabFileLabAlertSummaryListDTO
    {
        public int plas_id { get; set; }
        public Guid? plas_lfu_id { get; set; }
        public int? plas_row_idx { get; set; }
        public int? plas_alert_num { get; set; }
        public string plas_alert_org { get; set; }
        public string plas_alert_text { get; set; }
        public int? plas_alert_tot { get; set; }
        public string plas_piority { get; set; }
        public bool? plas_qual_cont { get; set; }
        public bool? plas_imp_specie { get; set; }
        public bool? plas_imp_resist { get; set; }
        public bool? plas_save_isol { get; set; }
        public bool? plas_send_ref { get; set; }
        public bool? plas_inf_cont { get; set; }
        public bool? plas_rx_comment { get; set; }
        public bool? plas_other_al { get; set; }
    }
}
