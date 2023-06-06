using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.START.Report.Api.Models
{
    public class ReceiveLogbookModel
    {
        public string srr_hos_code { get; set; }
        public string hos_name { get; set; }
        public string srr_starsno { get; set; }
        public string srr_name { get; set; }
        public string srr_stars_labno { get; set; }
        public string srr_hnno { get; set; }
        public string srr_age { get; set; }
        public DateTime srr_recvdate { get; set; }
        public DateTime srr_tatdate { get; set; }
    }
}