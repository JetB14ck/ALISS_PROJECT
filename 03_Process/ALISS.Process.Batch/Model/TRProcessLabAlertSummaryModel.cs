using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class TRProcessLabAlertSummaryModel
    {
        public int plas_id { get; set; }
        public Guid? plas_lfu_id { get; set; }
        public string plas_rule_number { get; set; }
        public string plas_piority { get; set; }
        public string plas_organisms { get; set; }
        public string plas_isolate_alerts { get; set; }
        public string plas_number_of_isolate { get; set; }
        public bool? plas_quality_control { get; set; }
        public bool? plas_important_species { get; set; }
        public bool? plas_save_the_isolate { get; set; }
        public bool? plas_send_to_a_reference_laboratory { get; set; }
        public bool? plas_infection_control { get; set; }
        public bool? plas_therapy_comment { get; set; }
        public bool? plas_other_comment { get; set; }
        public bool? plas_other_alert { get; set; }
        public string plas_status { get; set; }
        public string plas_createuser { get; set; }
        public DateTime? plas_createdate { get; set; }
        public string plas_updateuser { get; set; }
        public DateTime? plas_updatedate { get; set; }
    }
}
