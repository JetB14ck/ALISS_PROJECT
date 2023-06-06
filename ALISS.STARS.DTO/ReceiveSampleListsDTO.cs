using System;

namespace ALISS.STARS.DTO
{
    public class ReceiveSampleListsDTO
    {
        public int srr_id { get; set; }
        public string srr_starsno { get; set; }
        public string srr_starsno_ref { get; set; }
        public string srr_boxno { get; set; }
        public DateTime? srr_senddate { get; set; }
        public string srr_recvuser { get; set; }
        public DateTime? srr_recvdate { get; set; }
        public string srr_hos_code { get; set; }
        public string srr_arh_code { get; set; }
        public string srr_stars_labno { get; set; }
        public string srr_stars_orgnaism { get; set; }
        public string srr_stars_specimen { get; set; }
        public string srr_hnno { get; set; }
        public string srr_idcard { get; set; }
        public string srr_prename { get; set; }
        public string srr_name { get; set; }
        public string srr_age { get; set; }
        public string srr_sex { get; set; }
        public string srr_local_labno { get; set; }
        public string srr_local_organism { get; set; }
        public string srr_ident_org_code { get; set; }
        public string srr_ident_organism { get; set; }
        public string srr_local_specimen { get; set; }
        public string srr_ident_spec_code { get; set; }
        public string srr_ident_spec_name { get; set; }
        public string srr_testuser { get; set; }
        public DateTime? srr_testdate { get; set; }
        public string srr_approveuser { get; set; }
        public DateTime? srr_approvedate { get; set; }
        public string srr_reportno { get; set; }
        public DateTime? srr_reportdate { get; set; }
        public string srr_status { get; set; }
        public DateTime? srr_stars_specimendate { get; set; }
        public string srr_receive_organism { get; set; }
        public DateTime? srr_tatdate { get; set; }
        public string srr_createduser { get; set; }
        public DateTime? srr_createddate { get; set; }
        public string srr_updateuser { get; set; }
        public DateTime? srr_updatedate { get; set; }
        public string str_cancelreason { get; set; }
        public int? str_id { get; set; }
        public string str_cancelremark { get; set; }
        public int arh_days_receive { get; set; }
        public string hos_name { get; set; }
        public string srr_stars_specimendate_str
        {
            get
            {
                return (srr_stars_specimendate != null) ? srr_stars_specimendate.Value.ToString("dd/MM/yyyy") : "";
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
