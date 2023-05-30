using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.HISUpload.Batch.HDCData.Models
{
    public class TRSTGHDCData
    {
        public int hdc_id { get; set; }
        public string hdc_code { get; set; }
        public int huh_hfu_id { get; set; }
        public string huh_hos_code { get; set; }
        public string huh_hn_no { get; set; }

        public string HospitalCode { get; set; }
        public string IDCard { get; set; }
        public string HN { get; set; }
        public string AN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
        public string Subdistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Occupational { get; set; }
        public string VisitDate { get; set; }
        public string AdmissionDate { get; set; }
        public string ReferStatus { get; set; }
        public string Department { get; set; }
        public string FirstDiagnosis { get; set; }
        public string DischargeStatus { get; set; }
        public string DischargeDiagnosis { get; set; }
        public string DischargeDate { get; set; }

        public string createuser { get; set; }
        public DateTime? createdate { get; set; }
        public string updateuser { get; set; }
        public DateTime? updatedate { get; set; }
    }
}
