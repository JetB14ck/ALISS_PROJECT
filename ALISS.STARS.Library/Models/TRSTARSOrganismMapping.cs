using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.STARS.Library.Models
{
    public class TRSTARSOrganismMapping
    {
        public Guid som_id { get; set; }
        public char som_status { get; set; }
        public bool? som_flagdelete { get; set; }
        public Guid som_mappingid { get; set; }
        public string som_whonetcode { get; set; }
        public string som_whonetdesc { get; set; }
        public string som_localorganismcode { get; set; }
        public string som_localorganismdesc { get; set; }
        public string som_createuser { get; set; }
        public DateTime? som_createdate { get; set; }
        public string som_updateuser { get; set; }
        public DateTime? som_updatedate { get; set; }

    }
}
