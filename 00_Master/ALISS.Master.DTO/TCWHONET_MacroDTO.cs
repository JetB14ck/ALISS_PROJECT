using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Master.DTO
{
    public class TCWHONET_MacroDTO
    {
        public int who_mac_id { get; set; }
        public string who_mac_mst_code { get; set; }
        public string who_mac_name { get; set; }
        public string who_mac_status { get; set; }
        public bool? who_mac_active { get; set; }
        public string who_mac_createuser { get; set; }
        public DateTime? who_mac_createdate { get; set; }
        public string who_mac_updateuser { get; set; }
        public DateTime? who_mac_updatedate { get; set; }
    }
}
