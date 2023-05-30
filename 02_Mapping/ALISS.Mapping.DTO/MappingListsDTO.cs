using System;

namespace ALISS.Mapping.DTO
{
    public class MappingListsDTO
    {
        public Guid mp_id { get; set; }     
        public string mp_lab { get; set; }
        public string mp_labname { get; set; }
        public string mp_hos_name { get; set; }
        public string mp_program { get; set; }
        public string mp_filetype { get; set; }

        public string mp_program_filetype
        {
            get
            {
                string filetypelabel = mp_filetype;
                //if (mp_filetype == clsLabFileType.MLAB_FileType.MIC_SIR)
                //{
                //    filetypelabel = "DISK(ตัวเลข)+MIC(SIR)";
                //}
                //else if (mp_filetype == clsLabFileType.MLAB_FileType.MIC_NUM)
                //{
                //    filetypelabel = "DISK(SIR)+MIC(ตัวเลข)";
                //}
                return (mp_program != null && mp_filetype != null) ? string.Concat(mp_program, "(", filetypelabel, ")") : mp_program;
            }
        }

        public DateTime? mp_startdate { get; set; }
        public DateTime? mp_enddate { get; set; }  
        public decimal mp_version { get; set; }
        public DateTime? mp_updatedate { get; set; }
        public char mp_status { get; set; }
        public string mp_startdate_str
        {
            get
            {
                return (mp_startdate != null) ? mp_startdate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string mp_enddate_str
        {
            get
            {
                return (mp_enddate != null) ? mp_enddate.Value.ToString("dd/MM/yyyy") : "";
            }
        }

        public string mp_updatedate_str
        {
            get
            {
                return (mp_updatedate != null) ? mp_updatedate.Value.ToString("dd/MM/yyyy") : "";
            }
        }


        public string mp_status_str
        {
            get
            {
                string objReturn = "";

                if (mp_status == 'N') objReturn = "New";
                else if (mp_status == 'E') objReturn = "Draft";
                else if (mp_status == 'A') objReturn = "Approved";


                return objReturn;
            }
        }
    }
}
