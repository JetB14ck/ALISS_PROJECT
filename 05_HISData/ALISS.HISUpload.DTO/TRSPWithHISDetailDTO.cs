using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace ALISS.HISUpload.DTO
{
    public class TRSPWithHISSearchDTO
    {
        public string lfu_id { get; set; }
        public string hos_code { get; set; }
        public string lab_code { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string start_date_str
        {
            get
            {
                //return (start_date != null) ? start_date.Value.ToString("yyyy/MM/dd", new CultureInfo("en-US")) : null;
                return (start_date != null) ? start_date.Value.ToString("yyyy/MM/dd") : "";
            }
        }
        public string end_date_str
        {
            get
            {
                //return (end_date != null) ? end_date.Value.ToString("yyyy/MM/dd", new CultureInfo("en-US")) : null;
                return (end_date != null) ? end_date.Value.ToString("yyyy/MM/dd") : "";
            }
        }
        public string hos_name { get; set; }
        public string hos_name_only
        {
            get
            {
                return (!string.IsNullOrEmpty(hos_name)) ? hos_name.Replace("โรงพยาบาล", "") : "";
            }
        }

    }

    public class TRSPWithHISDetailDTO
    {
        public string ref_no { get; set; }
        public string hn { get; set; }
        public string lab_no { get; set; }
        public DateTime? date { get; set; }
        public string country_a { get; set; }
        public string laboratory { get; set; }
        public string origin { get; set; }
        public string patient_id { get; set; }
        public string sex { get; set; }
        public DateTime? date_birth_str { get; set; }
        public string pat_type { get; set; }
        public string ward { get; set; }
        public string institut { get; set; }
        public string department { get; set; }
        public string ward_type { get; set; }
        public string spec_num { get; set; }
        public DateTime? spec_date_str { get; set; }
        public string spec_type { get; set; }
        public string organism { get; set; }
        public string org_type { get; set; }
        public string serotype { get; set; }
        public string comment { get; set; }
        public DateTime? date_data_str { get; set; }
        public string fullname { get; set; }
        public string spec_code { get; set; }
        public string nosocomial { get; set; }
        public string date_admis { get; set; }

    }
}
