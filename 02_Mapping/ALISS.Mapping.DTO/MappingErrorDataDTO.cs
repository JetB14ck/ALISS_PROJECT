using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ALISS.Mapping.DTO
{
    public class TRImportMappingLogDTO
    {
        public int iml_id { get; set; }
        public DateTime iml_import_date { get; set; }
        public string iml_filename { get; set; }
        public int iml_total_record { get; set; }
        public int iml_who_record { get; set; }
        public string iml_status { get; set; }
        public string iml_createduser { get; set; }
        public DateTime iml_createdate { get; set; }
    }

    public class TRImportMappingLogErrorMessageDTO
    {
        public char lfu_status { get; set; }
        public char lfu_Err_type { get; set; }
        public int lfu_Err_no { get; set; }
        public string lfu_Err_Column { get; set; }
        public string lfu_Err_Message { get; set; }
    }

    public class TempImportMappingLogDTO
    {
        public int tme_id { get; set; }
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
        public string feh_lfu_id { get; set; }
        public string lfu_mp_id { get; set; }
        public string whonet_code { get; set; }
    }
}
