using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Master.DTO
{
    public class TCWHONET_DepartmentsDTO
    {
        public int who_dep_id { get; set; }
        public string who_mst_code { get; set; }
        public string who_dep_code { get; set; }
        public string who_dep_name { get; set; }
        public string who_dep_desc { get; set; }
        public string who_dep_status { get; set; }
        public bool? who_dep_active { get; set; }
        public string who_dep_createuser { get; set; }
        public DateTime? who_dep_createdate { get; set; }
        public string who_dep_updateuser { get; set; }
        public DateTime? who_dep_updatedate { get; set; }
    }
}
