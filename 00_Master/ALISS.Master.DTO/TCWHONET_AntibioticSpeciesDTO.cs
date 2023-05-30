using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Master.DTO
{
    public class TCWHONET_AntibioticSpeciesDTO
    {
        public int who_ant_spe_id { get; set; }
        public string who_mst_code { get; set; }
        public string who_ant_spe_source { get; set; }
        public string who_ant_spe_org_code { get; set; }
        public string who_ant_spe_ant_code { get; set; }
        public string who_ant_spe_I_low { get; set; }
        public string who_ant_spe_I_up { get; set; }
        public string who_ant_spe_spc { get; set; }
        public string who_ant_spe_1 { get; set; }
        public string who_ant_spe_2 { get; set; }
        public string who_ant_spe_status { get; set; }
        public bool? who_ant_spe_active { get; set; }
        public string who_ant_spe_createuser { get; set; }
        public DateTime? who_ant_spe_createdate { get; set; }
        public string who_ant_spe_updateuser { get; set; }
        public DateTime? who_ant_spe_updatedate { get; set; }
    }
}
