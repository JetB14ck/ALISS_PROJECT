using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Master.DTO
{
    public class TCWHONET_OrganismDTO
    {
        public int who_org_id { get; set; }
        public string who_org_mst_code { get; set; }
        public string who_org_code { get; set; }
        public string who_org { get; set; }
        public string who_gram { get; set; }
        public string who_organism { get; set; }
        public string who_org_clean { get; set; }
        public string who_status { get; set; }
        public string who_common { get; set; }
        public string who_org_group { get; set; }
        public string who_sub_group { get; set; }
        public string who_genus { get; set; }
        public string who_genus_code { get; set; }
        public string who_sct_code { get; set; }
        public string who_sct_text { get; set; }
        public string who_org_status { get; set; }
        public bool? who_org_active { get; set; }
        public string who_org_createuser { get; set; }
        public DateTime? who_org_createdate { get; set; }
        public string who_org_updateuser { get; set; }
        public DateTime? who_org_updatedate { get; set; }
    }
}
