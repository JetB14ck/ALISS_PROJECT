using ALISS.Mapping.DTO;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ALISS.LabFileUpload.DTO
{
    public class LabFileUploadDataDTO
    {
        public Guid lfu_id { get; set; }
        public char lfu_status { get; set; }
        public int lfu_version { get; set; }
        public bool? lfu_flagdelete { get; set; }
        public Guid lfu_mp_id { get; set; }
        public decimal lfu_mp_version { get; set; }
        public string lfu_hos_code { get; set; }
        public string lfu_hos_name { get; set; }
        public string lfu_lab { get; set; }
        public string lfu_FileName { get; set; }

        [Required(ErrorMessage = "Program is required")]
        public string lfu_Program { get; set; }
        public int lfu_DataYear { get; set; }
        public string lfu_Path { get; set; }

        [Required(ErrorMessage = "File Type is required")]
        public string lfu_FileType { get; set; }
        public string lfu_FileTypeLabel { get; set; }
        public int lfu_TotalRecord { get; set; }
        public int lfu_AerobicCulture { get; set; }
        public int lfu_ErrorRecord { get; set; }
        public DateTime? lfu_StartDatePeriod { get; set; }
        public DateTime? lfu_EndDatePeriod { get; set; }
        public string lfu_createuser { get; set; }
        public DateTime? lfu_createdate { get; set; } 
        public string lfu_approveduser { get; set; }
        public DateTime? lfu_approveddate { get; set; }
        public string lfu_updateuser { get; set; }
        public DateTime? lfu_updatedate { get; set; }

        public string lfu_program_filetype
        {
            get
            {
                string filetypelabel = lfu_FileType;
                if (!string.IsNullOrEmpty(lfu_FileTypeLabel))
                {
                    if (lfu_FileTypeLabel == clsLabFileType.MLAB_FileType.MIC_SIR)
                    {
                        filetypelabel = "MIC-SIR";
                    }
                    else if (lfu_FileTypeLabel == clsLabFileType.MLAB_FileType.MIC_NUM)
                    {
                        filetypelabel = "MIC-ตัวเลข";
                    }
                }               
                return (lfu_Program != null && lfu_FileType != null) ? string.Concat(lfu_Program, "(", filetypelabel, ")") : lfu_Program;
            }
        }

        public string lfu_StartDatePeriod_str
        {
            get
            {
                return (lfu_StartDatePeriod != null) ? lfu_StartDatePeriod.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string lfu_EndDatePeriod_str
        {
            get
            {
                return (lfu_EndDatePeriod != null) ? lfu_EndDatePeriod.Value.ToString("dd/MM/yyyy") : "";
            }
        }

        public string lfu_DatePeriod
        {
            get
            {
                return (!string.IsNullOrEmpty(lfu_StartDatePeriod_str) ? lfu_StartDatePeriod_str + " - " : "") + (!string.IsNullOrEmpty(lfu_EndDatePeriod_str) ? lfu_EndDatePeriod_str : "") ;
            }
        }

        public string lfu_createdate_str
        {
            get
            {
                return (lfu_createdate != null) ? lfu_createdate.Value.ToString("dd/MM/yyyy") : "";
            }
        }

        public string lfu_status_str
        {
            get
            {
                string objReturn = "";

                if (lfu_status == 'N') objReturn = "New";
                else if (lfu_status == 'R') objReturn = "Waiting Re-Process";
                else if (lfu_status == 'E') objReturn = "Error";
                else if (lfu_status == 'P' || lfu_status == 'M' || lfu_status == 'Q') objReturn = "Processing";
                else if (lfu_status == 'F') objReturn = "Finished";
                else if (lfu_status == 'D') objReturn = "Cancel";
                else if (lfu_status == 'W') objReturn = "WHONET Processed";
                else if (lfu_status == 'L') objReturn = "Re-Processing";
                else if (lfu_status == 'G') objReturn = "Transporting";
                else if (lfu_status == 'V') objReturn = "Received";
                else if (lfu_status == 'A') objReturn = "Lab Processing";
                else if (lfu_status == 'C') objReturn = "Complete";
                //else if (lfu_status == 'I') objReturn = "Invalid";
                else if (lfu_status == 'I')
                {
                    if (!string.IsNullOrEmpty(lfu_remark))
                    {
                        switch (lfu_remark.ToLower())
                        {
                            case string msg when msg.Contains("Object reference not set to an instance of an object".ToLower()):
                                objReturn = "ข้อมูลในไฟล์มีค่าว่าง";
                                break;
                            case string msg when msg.Contains("Unable to cast object of type 'System.DBNull' to type 'System.DateTime'".ToLower()):
                                objReturn = "Column Date มีค่าว่าง";
                                break;
                            case string msg when msg.Contains("does not belong to table".ToLower()):
                                var subs = lfu_remark.Substring(lfu_remark.IndexOf("'"), lfu_remark.IndexOf("d") - lfu_remark.IndexOf("'") - 1);
                                objReturn = "ไม่มี Column " + subs + " ในไฟล์";
                                break;
                            default:
                                objReturn = "Invalid";
                                break;
                        }
                    }
                    //objReturn = "Invalid";
                }
                return objReturn;
            }
        }

        public char lfu_LabFileStatus { get; set; }
        public char lfu_ProcessStatus { get; set; }
        public char lfu_LabAlertStatus { get; set; }
        public string lfu_remark { get; set; }
        public string lfu_TestType { get; set; }
        public string lfu_BoxNo { get; set; }
        public DateTime? lfu_SendDate { get; set; }
        public DateTime? lfu_TatDate { get; set; }
        public string lfu_SendDate_str
        {
            get
            {
                return (lfu_SendDate != null) ? lfu_SendDate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string lfu_TatDate_str
        {
            get
            {
                return (lfu_TatDate != null) ? lfu_TatDate.Value.ToString("dd/MM/yyyy") : "";
            }
        }
        public string lfu_arh_name { get; set; }
    }

    public class LabFileUploadSearchDTO
    {
        public string lfu_Hos { get; set; }
        public string lfu_Province { get; set; }
        public string lfu_Area { get; set; }
        public Guid lfu_id { get; set; }
        public string lfu_lab { get; set; }
        public string lfu_program { get; set; }
        public string lfu_filetype { get; set; }

    }
   
}
