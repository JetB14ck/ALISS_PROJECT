using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.DTO
{
    public class Output_Sample
    {
        public int ROW_IDX { get; set; }
        public string ORAGANISMS { get; set; }
        public string ISOLATES { get; set; }
        public string CODE1 { get; set; }
        public string DESCRIPT1 { get; set; }
        public string CODE2 { get; set; }
        public string DESCRIPT2 { get; set; }
        public string CODE3 { get; set; }
        public string DESCRIPT3 { get; set; }
        public int NUMISOLATE { get; set; }
        public int NUMPATIENT { get; set; }
    }
}
