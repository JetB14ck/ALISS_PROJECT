using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.STARS.DTO
{
    public class STARSOrganismMappingListsDTO
    {
        public Guid som_id { get; set; }
        public Guid som_mappingid { get; set; }
        public string som_whonetcode { get; set; }
        public string som_whonetdesc { get; set; }
        public string som_localorganismcode { get; set; }
        public string som_localorganismdesc { get; set; }
    }
    public class STARSOrganismMappingSearch
    {
        public Guid som_mappingid { get; set; }
        public string som_mst_code { get; set; }
    }

 }
