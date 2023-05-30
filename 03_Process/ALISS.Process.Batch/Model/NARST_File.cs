using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class NARST_File
    {
        public Guid ldh_id { get; set; }
        public Guid? ldh_id_mic { get; set; }
        public Guid? ldh_id_disk { get; set; }
        public Guid? ldh_id_etest { get; set; }
        public string hos_arh_code { get; set; }
        public string hos_prv_code { get; set; }
        public string ldh_hos_code { get; set; }
        public string ldh_lab { get; set; }
        public string ldh_labno { get; set; }
        public DateTime? ldh_cdate { get; set; }
        public string ldh_organism { get; set; }
        public string ldh_specimen { get; set; }
        public string COUNTRY_A { get; set; }
        public string LABORATORY { get; set; }
        public string ORIGIN { get; set; }
        public string PATIENT_ID { get; set; }
        public string SEX { get; set; }
        public string DATE_BIRTH_STR { get; set; }
        public string DATE_BIRTH
        {
            get
            {
                var strReturn = "";
                if (string.IsNullOrEmpty(DATE_BIRTH_STR) == false)
                {
                    string format1 = "M/d/yyyy H:mm:ss";
                    string format2 = "M/d/yyyy H:mm:ss tt";
                    string format3 = "M/d/yyyy H:mm:sss";
                    string format4 = "M/d/yyyy H:mm:sss tt";
                    string format5 = "M/d/yyyy";
                    DateTime tmpFieldValue;
                    if (DateTime.TryParseExact(DATE_BIRTH_STR, format1, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else if (DateTime.TryParseExact(DATE_BIRTH_STR, format2, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else if (DateTime.TryParseExact(DATE_BIRTH_STR, format3, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else if (DateTime.TryParseExact(DATE_BIRTH_STR, format4, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else if (DateTime.TryParseExact(DATE_BIRTH_STR.Split(' ')[0], format5, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else
                    {
                        strReturn = null;
                    }
                }
                return strReturn;
            }
        }
        public string AGE { get; set; }
        public string PAT_TYPE { get; set; }
        public string WARD { get; set; }
        public string INSTITUT { get; set; }
        public string DEPARTMENT { get; set; }
        public string WARD_TYPE { get; set; }
        public string SPEC_NUM { get; set; }
        public string SPEC_DATE_STR { get; set; }
        public string SPEC_DATE
        {
            get
            {
                var strReturn = "";
                if (string.IsNullOrEmpty(SPEC_DATE_STR) == false)
                {
                    string format1 = "M/d/yyyy H:mm:ss";
                    string format2 = "M/d/yyyy H:mm:ss tt";
                    string format3 = "M/d/yyyy H:mm:sss";
                    string format4 = "M/d/yyyy H:mm:sss tt";
                    string format5 = "M/d/yyyy";
                    DateTime tmpFieldValue;
                    if (DateTime.TryParseExact(SPEC_DATE_STR, format1, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else if (DateTime.TryParseExact(SPEC_DATE_STR, format2, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else if (DateTime.TryParseExact(SPEC_DATE_STR, format3, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else if (DateTime.TryParseExact(SPEC_DATE_STR, format4, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else if (DateTime.TryParseExact(SPEC_DATE_STR.Split(' ')[0], format5, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else
                    {
                        strReturn = null;
                    }
                }
                return strReturn;
            }
        }
        public string SPEC_TYPE { get; set; }
        public string SPEC_CODE { get; set; }
        public string SPEC_REAS { get; set; }
        public string ISOL_NUM { get; set; }
        public string ORGANISM { get; set; }
        public string ORG_TYPE { get; set; }
        public string SEROTYPE { get; set; }
        public string BETA_LACT { get; set; }
        public string ESBL { get; set; }
        public string CARBAPENEM { get; set; }
        public string MRSA_SCRN { get; set; }
        public string INDUC_CLI { get; set; }
        public string DATE_DATA_STR { get; set; }
        public string DATE_DATA
        {
            get
            {
                var strReturn = "";
                if (string.IsNullOrEmpty(DATE_DATA_STR) == false)
                {
                    string format1 = "M/d/yyyy H:mm:ss";
                    string format2 = "M/d/yyyy H:mm:ss tt";
                    string format3 = "M/d/yyyy H:mm:sss";
                    string format4 = "M/d/yyyy H:mm:sss tt";
                    string format5 = "M/d/yyyy";
                    DateTime tmpFieldValue;
                    if (DateTime.TryParseExact(DATE_DATA_STR, format1, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else if (DateTime.TryParseExact(DATE_DATA_STR, format2, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else if (DateTime.TryParseExact(DATE_DATA_STR, format3, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else if (DateTime.TryParseExact(DATE_DATA_STR, format4, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else if (DateTime.TryParseExact(DATE_DATA_STR.Split(' ')[0], format5, System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out tmpFieldValue))
                    {
                        strReturn = tmpFieldValue.ToString("yyyy-MM-dd HH:mm:sss");
                    }
                    else
                    {
                        strReturn = null;
                    }
                }
                else if (ldh_cdate != null)
                {
                    strReturn = ldh_cdate.Value.ToString("yyyy-MM-dd HH:mm:sss");
                }
                return strReturn;
            }
        }
        public string COMMENT { get; set; }
        public string AMK_ND30 { get; set; }
        public string AMC_ND20 { get; set; }
        public string AMP_ND10 { get; set; }
        public string SAM_ND10 { get; set; }
        public string AZM_ND15 { get; set; }
        public string CZO_ND30 { get; set; }
        public string FEP_ND30 { get; set; }
        public string CFM_ND5 { get; set; }
        public string CSL_ND30 { get; set; }
        public string CTX_ND30 { get; set; }
        public string FOX_ND30 { get; set; }
        public string CAZ_ND30 { get; set; }
        public string CZX_ND30 { get; set; }
        public string CRO_ND30 { get; set; }
        public string CXA_ND30 { get; set; }
        public string CXM_ND30 { get; set; }
        public string CHL_ND30 { get; set; }
        public string CIP_ND5 { get; set; }
        public string CLI_ND2 { get; set; }
        public string COL_ND10 { get; set; }
        public string DAP_ND30 { get; set; }
        public string DOR_ND10 { get; set; }
        public string ETP_ND10 { get; set; }
        public string ERY_ND15 { get; set; }
        public string FOS_ND200 { get; set; }
        public string FUS_ND10 { get; set; }
        public string GEN_ND10 { get; set; }
        public string GEH_ND120 { get; set; }
        public string IPM_ND10 { get; set; }
        public string LVX_ND5 { get; set; }
        public string MEM_ND10 { get; set; }
        public string NAL_ND30 { get; set; }
        public string NET_ND30 { get; set; }
        public string NIT_ND300 { get; set; }
        public string NOR_ND10 { get; set; }
        public string OFX_ND5 { get; set; }
        public string OXA_ND1 { get; set; }
        public string PEN_ND10 { get; set; }
        public string PIP_ND100 { get; set; }
        public string TZP_ND100 { get; set; }
        public string POL_ND300 { get; set; }
        public string STH_ND300 { get; set; }
        public string STR_ND10 { get; set; }
        public string TEC_ND30 { get; set; }
        public string TCY_ND30 { get; set; }
        public string SXT_ND1_2 { get; set; }
        public string VAN_ND30 { get; set; }
        public string AMX_NE { get; set; }
        public string CTX_NM { get; set; }
        public string CTX_NE { get; set; }
        public string CAZ_NM { get; set; }
        public string CAZ_NE { get; set; }
        public string CZX_NM { get; set; }
        public string CZX_NE { get; set; }
        public string CRO_NM { get; set; }
        public string CRO_NE { get; set; }
        public string CHL_NM { get; set; }
        public string CHL_NE { get; set; }
        public string CIP_NM { get; set; }
        public string CIP_NE { get; set; }
        public string CLI_NM { get; set; }
        public string CLI_NE { get; set; }
        public string COL_NM { get; set; }
        public string COL_NE { get; set; }
        public string DAP_NM { get; set; }
        public string DAP_NE { get; set; }
        public string ETP_NM { get; set; }
        public string ETP_NE { get; set; }
        public string ERY_NM { get; set; }
        public string ERY_NE { get; set; }
        public string IPM_NM { get; set; }
        public string IPM_NE { get; set; }
        public string MEM_NM { get; set; }
        public string MEM_NE { get; set; }
        public string NET_NM { get; set; }
        public string NET_NE { get; set; }
        public string PEN_NM { get; set; }
        public string PEN_NE { get; set; }
        public string VAN_NM { get; set; }
        public string VAN_NE { get; set; }
        public string AZM_NE { get; set; }
        public string SPT_ND100 { get; set; }
        public string GEN_NM { get; set; }
        public string GEN_NE { get; set; }
        public string TCY_NM { get; set; }
        public string TCY_NE { get; set; }
        public string CXM_NM { get; set; }
        public string CFM_NE { get; set; }
        public string AMP_NM { get; set; }
        public string AMP_NE { get; set; }
        public string FEP_NM { get; set; }
        public string FEP_NE { get; set; }
        public string LVX_NM { get; set; }
        public string LVX_NE { get; set; }
        public string AMK_NM { get; set; }
        public string AMK_NE { get; set; }
        public string TZP_NM { get; set; }
        public string TZP_NE { get; set; }
        public string SAM_NM { get; set; }
        public string SAM_NE { get; set; }
        public string CZO_NM { get; set; }
        public string CZO_NE { get; set; }
        public string AZM_NM { get; set; }
        public string CXM_NE { get; set; }
        public string CXA_NM { get; set; }
        public string CXA_NE { get; set; }
        public string AMC_NM { get; set; }
        public string AMC_NE { get; set; }
        public string CSL_NM { get; set; }
        public string CSL_NE { get; set; }
        public string OXA_NM { get; set; }
        public string OXA_NE { get; set; }
        public string FOX_NM { get; set; }
        public string FOX_NE { get; set; }
        public string NOR_NM { get; set; }
        public string NOR_NE { get; set; }
        public string GEH_NM { get; set; }
        public string GEH_NE { get; set; }
        public string TEC_NM { get; set; }
        public string TEC_NE { get; set; }
        public string FOS_NM { get; set; }
        public string FOS_NE { get; set; }
        public string NIT_NM { get; set; }
        public string NIT_NE { get; set; }
        public string SXT_NM { get; set; }
        public string SXT_NE { get; set; }
        public string DOR_NM { get; set; }
        public string DOR_NE { get; set; }
    }
}
