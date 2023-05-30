using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.LabFileUpload.DTO
{
    public class STGLabFileDataDetailDTO
    {
        public int ldd_id_new { get; set; }
        public char ldd_status { get; set; }
        public string ldd_whonetfield { get; set; }
        public string ldd_originalfield { get; set; }
        public string ldd_originalvalue { get; set; }
        public string ldd_mappingvalue { get; set; }
        public int ldh_id_new { get; set; }
        public string ldd_createuser { get; set; }
        public DateTime? ldd_createdate { get; set; }
        public string ldd_updateuser { get; set; }
        public DateTime? ldd_updatedate { get; set; }

    }
}
