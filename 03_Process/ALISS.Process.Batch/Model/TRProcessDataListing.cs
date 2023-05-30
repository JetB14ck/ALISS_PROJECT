﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Process.Batch.Model
{
    public class TRProcessDataListing
    {
        public int pdl_id { get; set; }
        public string pdl_pcr_code { get; set; }
        public string pdl_arh_code { get; set; }
        public string pdl_prv_code { get; set; }
        public string pdl_hos_code { get; set; }
        public string pdl_lab_code { get; set; }
        public string pdl_row_idx { get; set; }
        public string pdl_country_a { get; set; }
        public string pdl_laboratory { get; set; }
        public string pdl_origin { get; set; }
        public string pdl_patient_id { get; set; }
        public string pdl_last_name { get; set; }
        public string pdl_first_name { get; set; }
        public string pdl_sex { get; set; }
        public string pdl_date_birth { get; set; }
        public string pdl_age { get; set; }
        public string pdl_pat_type { get; set; }
        public string pdl_ward { get; set; }
        public string pdl_institut { get; set; }
        public string pdl_department { get; set; }
        public string pdl_ward_type { get; set; }
        public string pdl_spec_num { get; set; }
        public string pdl_spec_date { get; set; }
        public string pdl_spec_type { get; set; }
        public string pdl_spec_code { get; set; }
        public string pdl_spec_reas { get; set; }
        public string pdl_isol_num { get; set; }
        public string pdl_organism { get; set; }
        public string pdl_nosocomial { get; set; }
        public string pdl_org_type { get; set; }
        public string pdl_serotype { get; set; }
        public string pdl_beta_lact { get; set; }
        public string pdl_esbl { get; set; }
        public string pdl_carbapenem { get; set; }
        public string pdl_mrsa_scrn { get; set; }
        public string pdl_induc_cli { get; set; }
        public string pdl_comment { get; set; }
        public string pdl_date_data { get; set; }
        public string pdl_amk_nd30 { get; set; }
        public string pdl_amc_nd20 { get; set; }
        public string pdl_amp_nd10 { get; set; }
        public string pdl_sam_nd10 { get; set; }
        public string pdl_azm_nd15 { get; set; }
        public string pdl_czo_nd30 { get; set; }
        public string pdl_fep_nd30 { get; set; }
        public string pdl_cfm_nd5 { get; set; }
        public string pdl_csl_nd30 { get; set; }
        public string pdl_ctx_nd30 { get; set; }
        public string pdl_fox_nd30 { get; set; }
        public string pdl_caz_nd30 { get; set; }
        public string pdl_czx_nd30 { get; set; }
        public string pdl_cro_nd30 { get; set; }
        public string pdl_cxa_nd30 { get; set; }
        public string pdl_cxm_nd30 { get; set; }
        public string pdl_chl_nd30 { get; set; }
        public string pdl_cip_nd5 { get; set; }
        public string pdl_cli_nd2 { get; set; }
        public string pdl_col_nd10 { get; set; }
        public string pdl_dap_nd30 { get; set; }
        public string pdl_dor_nd10 { get; set; }
        public string pdl_etp_nd10 { get; set; }
        public string pdl_ery_nd15 { get; set; }
        public string pdl_fos_nd200 { get; set; }
        public string pdl_fus_nd10 { get; set; }
        public string pdl_gen_nd10 { get; set; }
        public string pdl_geh_nd120 { get; set; }
        public string pdl_ipm_nd10 { get; set; }
        public string pdl_lvx_nd5 { get; set; }
        public string pdl_mem_nd10 { get; set; }
        public string pdl_nal_nd30 { get; set; }
        public string pdl_net_nd30 { get; set; }
        public string pdl_nit_nd300 { get; set; }
        public string pdl_nor_nd10 { get; set; }
        public string pdl_ofx_nd5 { get; set; }
        public string pdl_oxa_nd1 { get; set; }
        public string pdl_pen_nd10 { get; set; }
        public string pdl_pip_nd100 { get; set; }
        public string pdl_tzp_nd100 { get; set; }
        public string pdl_pol_nd300 { get; set; }
        public string pdl_sth_nd300 { get; set; }
        public string pdl_str_nd10 { get; set; }
        public string pdl_tec_nd30 { get; set; }
        public string pdl_tcy_nd30 { get; set; }
        public string pdl_sxt_nd1_2 { get; set; }
        public string pdl_van_nd30 { get; set; }
        public string pdl_amx_ne { get; set; }
        public string pdl_ctx_nm { get; set; }
        public string pdl_ctx_ne { get; set; }
        public string pdl_caz_nm { get; set; }
        public string pdl_caz_ne { get; set; }
        public string pdl_czx_nm { get; set; }
        public string pdl_czx_ne { get; set; }
        public string pdl_cro_nm { get; set; }
        public string pdl_cro_ne { get; set; }
        public string pdl_chl_nm { get; set; }
        public string pdl_chl_ne { get; set; }
        public string pdl_cip_nm { get; set; }
        public string pdl_cip_ne { get; set; }
        public string pdl_cli_nm { get; set; }
        public string pdl_cli_ne { get; set; }
        public string pdl_col_nm { get; set; }
        public string pdl_col_ne { get; set; }
        public string pdl_dap_nm { get; set; }
        public string pdl_dap_ne { get; set; }
        public string pdl_etp_nm { get; set; }
        public string pdl_etp_ne { get; set; }
        public string pdl_ery_nm { get; set; }
        public string pdl_ery_ne { get; set; }
        public string pdl_ipm_nm { get; set; }
        public string pdl_ipm_ne { get; set; }
        public string pdl_mem_nm { get; set; }
        public string pdl_mem_ne { get; set; }
        public string pdl_net_nm { get; set; }
        public string pdl_net_ne { get; set; }
        public string pdl_pen_nm { get; set; }
        public string pdl_pen_ne { get; set; }
        public string pdl_van_nm { get; set; }
        public string pdl_van_ne { get; set; }
        public string pdl_azm_ne { get; set; }
        public string pdl_spt_nd100 { get; set; }
        public string pdl_gen_nm { get; set; }
        public string pdl_gen_ne { get; set; }
        public string pdl_tcy_nm { get; set; }
        public string pdl_tcy_ne { get; set; }
        public string pdl_cxm_nm { get; set; }
        public string pdl_cfm_ne { get; set; }
        public string pdl_amp_nm { get; set; }
        public string pdl_amp_ne { get; set; }
        public string pdl_fep_nm { get; set; }
        public string pdl_fep_ne { get; set; }
        public string pdl_lvx_nm { get; set; }
        public string pdl_lvx_ne { get; set; }
        public string pdl_amk_nm { get; set; }
        public string pdl_amk_ne { get; set; }
        public string pdl_tzp_nm { get; set; }
        public string pdl_tzp_ne { get; set; }
        public string pdl_sam_nm { get; set; }
        public string pdl_sam_ne { get; set; }
        public string pdl_czo_nm { get; set; }
        public string pdl_czo_ne { get; set; }
        public string pdl_azm_nm { get; set; }
        public string pdl_cxm_ne { get; set; }
        public string pdl_cxa_nm { get; set; }
        public string pdl_cxa_ne { get; set; }
        public string pdl_amc_nm { get; set; }
        public string pdl_amc_ne { get; set; }
        public string pdl_csl_nm { get; set; }
        public string pdl_csl_ne { get; set; }
        public string pdl_oxa_nm { get; set; }
        public string pdl_oxa_ne { get; set; }
        public string pdl_fox_nm { get; set; }
        public string pdl_fox_ne { get; set; }
        public string pdl_nor_nm { get; set; }
        public string pdl_nor_ne { get; set; }
        public string pdl_geh_nm { get; set; }
        public string pdl_geh_ne { get; set; }
        public string pdl_tec_nm { get; set; }
        public string pdl_tec_ne { get; set; }
        public string pdl_fos_nm { get; set; }
        public string pdl_fos_ne { get; set; }
        public string pdl_nit_nm { get; set; }
        public string pdl_nit_ne { get; set; }
        public string pdl_sxt_nm { get; set; }
        public string pdl_sxt_ne { get; set; }
        public string pdl_dor_nm { get; set; }
        public string pdl_dor_ne { get; set; }
        public string pdl_createuser { get; set; }
        public DateTime? pdl_createdate { get; set; }
        public string pdl_updateuser { get; set; }
        public DateTime? pdl_updatedate { get; set; }
    }
}