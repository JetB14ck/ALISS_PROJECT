using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class TRProcessLabAlertModel
    {
        public int pla_id { get; set; }
        public Guid? pla_ldh_id { get; set; }
        public Guid? pla_lfu_id { get; set; }
        public string pla_sequence { get; set; }
        public string pla_arh_code { get; set; }
        public string pla_prv_code { get; set; }
        public string pla_hos_code { get; set; }
        public string pla_lab { get; set; }
        public string pla_labno { get; set; }
        public string pla_date { get; set; }
        public DateTime? pla_cdate { get; set; }
        public string pla_organism { get; set; }
        public string pla_organism_whonetfield { get; set; }
        public string pla_specimen { get; set; }
        public string pla_specimen_whonetfield { get; set; }
        public string pla_whonet_result { get; set; }
        public bool? pla_alert { get; set; }
        public string pla_piority { get; set; }
        public string pla_organisms { get; set; }
        public string pla_isolate_alerts { get; set; }
        public bool? pla_quality_control { get; set; }
        public bool? pla_important_species { get; set; }
        public bool? pla_save_the_isolate { get; set; }
        public bool? pla_send_to_a_reference_laboratory { get; set; }
        public bool? pla_infection_control { get; set; }
        public bool? pla_therapy_comment { get; set; }
        public bool? pla_other_alert { get; set; }
        public string pla_ldh_createuser { get; set; }
        public DateTime? pla_ldh_createdate { get; set; }
        public string pla_ldh_updateuser { get; set; }
        public DateTime? pla_ldh_updatedate { get; set; }
        public string pla_status { get; set; }
        public string pla_createuser { get; set; }
        public DateTime? pla_createdate { get; set; }
        public string pla_updateuser { get; set; }
        public DateTime? pla_updatedate { get; set; }
    }
}
