using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.STARS.Library.Models
{
    public class TRStarsReceiveSample
    {
        public int str_id { get; set; }
        public int srr_id { get; set; }
        public string str_starsno { get; set; }
        public DateTime? str_recvdate { get; set; }
        public DateTime? str_tatdate { get; set; }
        public string str_boxno { get; set; }
        public Guid str_lfu_id { get; set; }
        public string str_hos_code { get; set; }
        public string str_labno { get; set; }
        public string str_date { get; set; }
        public DateTime? str_cdate { get; set; }
        public string str_hnno { get; set; }
        public string str_idcard { get; set; }
        public string str_susp_organism { get; set; }
        public string str_ident_organism { get; set; }
        public string str_ident_organismdesc { get; set; }
        public string str_specimen { get; set; }
        public string str_prename { get; set; }
        public string str_fullname { get; set; }
        public string str_age { get; set; }
        public DateTime? str_canceldate { get; set; }
        public string str_cancelreason { get; set; }
        public string str_cancelremark { get; set; }
        public bool str_receiveflag { get; set; }
        public string str_createduser { get; set; }
        public DateTime? str_createdate { get; set; }
        public string str_updateuser { get; set; }
        public DateTime? str_updatedate { get; set; }
    }
}
