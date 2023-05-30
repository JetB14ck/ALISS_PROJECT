using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.STARS.Library.Models
{
    public class TRSTARSSpecimenMapping
    {
        public Guid ssm_id { get; set; }
        public char ssm_status { get; set; }
        public bool? ssm_flagdelete { get; set; }
        public Guid ssm_mappingid { get; set; }
        public string ssm_whonetcode { get; set; }
        public string ssm_localspecimencode { get; set; }
        public string ssm_localspecimendesc { get; set; }        
        public string ssm_createuser { get; set; }
        public DateTime? ssm_createdate { get; set; }
        public string ssm_updateuser { get; set; }
        public DateTime? ssm_updatedate { get; set; }
    }
}
