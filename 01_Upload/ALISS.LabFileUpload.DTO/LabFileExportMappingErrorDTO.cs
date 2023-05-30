using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.LabFileUpload.DTO
{
    public class LabFileExportMappingErrorDTO
    {
        public string hos_code { get; set; }
        public string hos_name { get; set; }
        public string hos_arh_code { get; set; }
        public string lfu_FileType { get; set; }
        public string lfu_Program { get; set; }
        public string lfu_FileName { get; set; }
        public string feh_field { get; set; }
        public string feh_message { get; set; }
        public string fed_localvalue { get; set; }
        public string fed_localdescr { get; set; }
        public Guid feh_lfu_id { get; set; }
        public Guid lfu_mp_id { get; set; }
        public string whonet_code { get; set; }
    }
}
