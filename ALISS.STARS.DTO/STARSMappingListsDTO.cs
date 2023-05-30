using System;

namespace ALISS.STARS.DTO
{
    public class STARSMappingListsDTO
    {
        public Guid smp_id { get; set; }     
        public string smp_machinetype { get; set; }
        public DateTime? smp_startdate { get; set; }
        public DateTime? smp_enddate { get; set; }  
        public decimal smp_version { get; set; }
        public DateTime? smp_updatedate { get; set; }
        public char smp_status { get; set; }
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
