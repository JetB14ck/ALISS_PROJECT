using System;

namespace ALISS.STARS.DTO
{
    public class ReceiveSampleListsDTO
    {
        public Guid srr_id { get; set; }     
        public string srr_boxno { get; set; }
        public string srr_stars_labno { get; set; }
        public DateTime? srr_senddate { get; set; }
        public string srr_hnno { get; set; }
        public string srr_name { get; set; }
        public string srr_age { get; set; }
        public string srr_sex { get; set; }
        public string srr_stars_orgnaism { get; set; }
        public string srr_receive_organism { get; set; }
        public string srr_status { get; set; }// 'R'
        public string str_cancelreason { get; set; }
        public string srr_starsno { get; set; }
        public DateTime? srr_recvdate { get; set; }
        public DateTime? srr_tatdate { get; set; }
        public string srr_senddate_str
        {
            get
            {
                return (srr_senddate != null) ? srr_senddate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string srr_recvdate_str
        {
            get
            {
                return (srr_recvdate != null) ? srr_recvdate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string srr_tatdate_str
        {
            get
            {
                return (srr_tatdate != null) ? srr_tatdate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
    }
}
