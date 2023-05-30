using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.EXPORT.Library.DTO
{
    public class AMRGraphDTO
    {

    }

    public class AMRGraphSearchDTO
    {
        public int start_year { get; set; }
        public int end_year { get; set; }
        public string sir { get; set; }
        public int graph_format { get; set; }
        public IEnumerable<string> organism { get; set; }
        public IEnumerable<string> antibiotic { get; set; }
        public int sub_graph { get; set; }
        public string hos_code { get; set; }
        public string hos_name { get; set; }
        public string arh_code { get; set; }
        public string arh_name { get; set; }
        public string prv_code { get; set; }
        public string prv_name { get; set; }
        public IEnumerable<string> wardlist { get; set; }
        public IEnumerable<string> specimenlist { get; set; }
    }
}
