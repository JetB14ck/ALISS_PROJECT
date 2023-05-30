using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ALISS.STARS.DTO
{
    public class STARSMappingDataDTO
    {
        public Guid smp_id { get; set; }
        public string smp_mst_code { get; set; }
        public char smp_status { get; set; }
        public decimal smp_version { get; set; }
        public bool? smp_flagdelete { get; set; }

        [Required(ErrorMessage = "Machine is required")]
        public string smp_machinetype { get; set; }

        [Required(ErrorMessage = "The antibiotics of one isolate require how  many row of data? is required")]
        public bool? smp_AntibioticIsolateOneRec { get; set; }

        [Required(ErrorMessage = "Does the first row of the data file have the names of the data fields? is required")]
        public bool? smp_firstlineisheader { get; set; }

        [Required(ErrorMessage = "Date Format is required")]
        public string smp_dateformat { get; set; }

        [Required(ErrorMessage = "วันที่เริ่มใช้งาน is required")]
        public DateTime? smp_startdate { get; set; }
        public DateTime? smp_enddate { get; set; }
        public string smp_createuser { get; set; }
        public DateTime? smp_createdate { get; set; }
        public string smp_approveduser { get; set; }
        public DateTime? smp_approveddate { get; set; }
        public string smp_updateuser { get; set; }
        public DateTime? smp_updatedate { get; set; }
        public string smp_startdate_str
        {
            get
            {
                return (smp_startdate != null) ? smp_startdate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string smp_enddate_str
        {
            get
            {
                return (smp_enddate != null) ? smp_enddate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string smp_createdate_str
        {
            get
            {
                return (smp_createdate != null) ? smp_createdate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string smp_approveddate_str
        {
            get
            {
                return (smp_approveddate != null) ? smp_approveddate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string smp_updatedate_str
        {
            get
            {
                return (smp_updatedate != null) ? smp_updatedate.Value.ToString("dd/MM/yyyy") : "";
            }
        }

        public string smp_status_str
        {
            get
            {
                string objReturn = "";

                if (smp_status == 'N') objReturn = "New";
                else if (smp_status == 'E') objReturn = "Draft";
                else if (smp_status == 'A') objReturn = "Approved";


                return objReturn;
            }
        }

    }
}
