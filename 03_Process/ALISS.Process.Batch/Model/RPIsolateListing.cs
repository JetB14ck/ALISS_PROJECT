using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class RPIsolateListing
    {
        public int id { get; set; }
        public Guid? iso_id { get; set; }
        public string pcr_code { get; set; }
        public string country { get; set; }
        public string laboratory { get; set; }
        public string arh_code { get; set; }
        public string prv_code { get; set; }
        public string hos_code { get; set; }
        public string origin { get; set; }
        public string pid { get; set; }
        public string lastname { get; set; }
        public string firstname { get; set; }
        public string sex { get; set; }
        public DateTime? birth { get; set; }
        public string age { get; set; }
        public string age_category { get; set; }
        public string location { get; set; }
        public string institution { get; set; }
        public string department { get; set; }
        public string loc_type { get; set; }
        public DateTime? admission_date { get; set; }
        public string spec_no { get; set; }
        public DateTime? spc_date { get; set; }
        public string spc_type { get; set; }
        public string spc_type_num { get; set; }
        public string reason { get; set; }
        public string isolate_no { get; set; }
        public string org_code { get; set; }
        public string org_name { get; set; }
        public string nosocomial_infect { get; set; }
        public string org_type { get; set; }
        public string serotype { get; set; }
        public string beta_lactamase { get; set; }
        public string ESBL { get; set; }
        public string carbapenemase { get; set; }
        public string MRSA_screening_test { get; set; }
        public string inducible_clindamycin_resistance { get; set; }
        public string comment { get; set; }
        public DateTime? date_of_data_entry { get; set; }
        public string createuser { get; set; }
        public DateTime? createdate { get; set; }
        public string updateuser { get; set; }
        public DateTime? updatedate { get; set; }
    }
}
