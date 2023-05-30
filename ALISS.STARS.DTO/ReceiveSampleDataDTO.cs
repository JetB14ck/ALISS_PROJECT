using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ALISS.STARS.DTO
{
    public class ReceiveSampleDataDTO
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
        public Char srr_ident_spec_code { get; set; }
        public string srr_ident_spec_name { get; set; }
        public string srr_testuser { get; set; }
        public DateTime? srr_testdate { get; set; }
        public string srr_approveuser { get; set; }
        public DateTime? srr_approvedate { get; set; }
        public string srr_reportno { get; set; }
        public DateTime? srr_reportdate { get; set; }
        public char srr_status { get; set; }
        public DateTime? srr_stars_specimendate { get; set; }
        public string srr_receive_organism { get; set; }
        public DateTime? srr_tatdate { get; set; }
        public string srr_createduser { get; set; }
        public DateTime? srr_createddate { get; set; }
        public string srr_updateuser { get; set; }
        public DateTime? srr_updatedate { get; set; }
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
        public string srr_testdate_str
        {
            get
            {
                return (srr_testdate != null) ? srr_testdate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string srr_approvedate_str
        {
            get
            {
                return (srr_approvedate != null) ? srr_approvedate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string srr_reportdate_str
        {
            get
            {
                return (srr_reportdate != null) ? srr_reportdate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string srr_createddate_str
        {
            get
            {
                return (srr_createddate != null) ? srr_createddate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string srr_updatedate_str
        {
            get
            {
                return (srr_updatedate != null) ? srr_updatedate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string srr_stars_specimendate_str
        {
            get
            {
                return (srr_stars_specimendate != null) ? srr_stars_specimendate.Value.ToString("dd/MM/yyyy") : "";
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
