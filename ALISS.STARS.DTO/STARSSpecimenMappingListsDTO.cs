using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.STARS.DTO
{
    public class STARSSpecimenMappingListsDTO
    {
        public Guid ssm_id { get; set; }
        public Guid ssm_mappingid { get; set; }
        public string ssm_whonetcode { get; set; }
        public string ssm_localspecimencode { get; set; }
        public string ssm_localspecimendesc { get; set; }
    }

    public class STARSSpecimenMappingSearch
    {
        public Guid ssm_mappingid { get; set; }       
        public string ssm_mst_code { get; set; }
    }
}
