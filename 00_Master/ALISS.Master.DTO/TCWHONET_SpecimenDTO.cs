using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Master.DTO
{
    public class TCWHONET_SpecimenDTO
    {
        public int who_spc_id { get; set; }
        public string who_spc_mst_code { get; set; }
        public string who_spc_numeric { get; set; }
        public string who_spc_code { get; set; }
        public string who_spc_name { get; set; }
        public string who_spc_desc { get; set; }
        public string who_spc_status { get; set; }
        public bool? who_spc_active { get; set; }
        public bool? who_spc_mapping_active { get; set; }
        public string who_spc_createuser { get; set; }
        public DateTime? who_spc_createdate { get; set; }
        public string who_spc_updateuser { get; set; }
        public DateTime? who_spc_updatedate { get; set; }
    }
}
