using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.DTO
{
    public class Output_RIS
    {
        public int ROW_IDX { get; set; }
        public string ORAGANISMS { get; set; }
        public string ISOLATES { get; set; }
        public string DRUG_CODE { get; set; }
        public string DRUG_NAME { get; set; }
        public string CLASS { get; set; }
        public string SUBCLASS { get; set; }
        public string DRUG_CODE1 { get; set; }
        public string METHODS { get; set; }
        public string CODE1 { get; set; }
        public string DESCRIPT1 { get; set; }
        public string CODE2 { get; set; }
        public string DESCRIPT2 { get; set; }
        public string CODE3 { get; set; }
        public string DESCRIPT3 { get; set; }
        public string SITE_INF { get; set; }
        public string BREAKPOINT { get; set; }
        public int NUM_TESTED { get; set; }
        public int NUM_RES { get; set; }
        public int NUM_RESX { get; set; }
        public int NUM_INT { get; set; }
        public int NUM_SUSC { get; set; }
        public int NUM_SUSCX { get; set; }
        public int NUM_QUANT { get; set; }
    }
}
