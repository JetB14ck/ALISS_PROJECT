using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.HISUpload.Batch.HDCData.Models
{
    public class HISFileUploadDetail
    {
        public string huh_hos_code { get; set; }
        public string huh_hn_no { get; set; }
        public string huh_lab_no { get; set; }
        public DateTime? huh_date { get; set; }
        public int hud_id { get; set; }
        public string hud_field_name { get; set; }
        public string hud_field_value { get; set; }
        public string hdc_field_name { get; set; }
        public string hdc_field_value { get; set; }
    }
}
