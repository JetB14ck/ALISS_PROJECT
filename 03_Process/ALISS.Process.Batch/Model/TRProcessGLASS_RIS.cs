using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class TRProcessGLASS_RIS
    {
        public int pcg_ris_id { get; set; }
        public string pcg_pcr_code { get; set; }
        public string pcg_country { get; set; }
        public string pcg_year { get; set; }
        public string pcg_specimen { get; set; }
        public string pcg_pathogen { get; set; }
        public string pcg_gender { get; set; }
        public string pcg_origin { get; set; }
        public string pcg_agegroup { get; set; }
        public string pcg_antibiotic { get; set; }
        public string pcg_resistant { get; set; }
        public string pcg_intermediate { get; set; }
        public string pcg_nonsusceptible { get; set; }
        public string pcg_susceptible { get; set; }
        public string pcg_unknown_no_ast { get; set; }
        public string pcg_unknown_no_breakpoints { get; set; }
        public string pcg_batchid { get; set; }
    }
}
