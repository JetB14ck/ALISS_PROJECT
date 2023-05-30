using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ALISS.Process.Batch.DTO;
using Microsoft.Extensions.Configuration;
using System.IO;
using ALISS.Process.Batch.Model;

namespace ALISS.Process.Batch.DataAccess
{
    public class ProcessContext : DbContext
    {
        private static IConfiguration _iconfiguration;

        public DbSet<TCWHONETColumn> TCWHONETColumns { get; set; }
        public DbSet<TCWHONET_Antibiotics> TCWHONET_AntibioticsModel { get; set; }
        public DbSet<TCWHONET_Specimen> TCWHONET_SpecimenModel { get; set; }
        public DbSet<ProcessDTO> ProcessDTOs { get; set; }
        public DbSet<TCProvince> TCProvinces { get; set; }
        public DbSet<TRHospital> TRHospitals { get; set; }
        public DbSet<TRHospitalLab> TRHospitalLabs { get; set; }
        public DbSet<TRLabFileUpload> TRLabFileUploads { get; set; }
        public DbSet<TRProcessRequest> TRProcessRequests { get; set; }
        public DbSet<TRProcessRequestDetail> TRProcessRequestDetails { get; set; }
        public DbSet<TRProcessRequestLabData> TRProcessRequestLabDatas { get; set; }
        public DbSet<TRProcessRequestHISData> TRProcessRequestHISDatas { get; set; }
        public DbSet<TRSTGProcessFileUpload> TRSTGProcessFileUploads { get; set; }
        public DbSet<TRSTGProcessDataHeader> TRSTGProcessDataHeaders { get; set; }
        public DbSet<TRSTGProcessDataDetail> TRSTGProcessDataDetails { get; set; }
        public DbSet<TRProcessDataResult> TRProcessDataResults { get; set; }
        public DbSet<TRProcessDataListing> TRProcessDataListings { get; set; }
        public DbSet<TRProcessRequestDTO> TRProcessRequestDTOs { get; set; }
        public DbSet<TCProcessExcelColumn> TCProcessExcelColumnModel { get; set; }
        public DbSet<TCProcessExcelRow> TCProcessExcelRowModel { get; set; }
        public DbSet<TCProcessExcelTemplate> TCProcessExcelTemplateModel { get; set; }
        public DbSet<TRProcessLabFileModel> TRProcessLabFile { get; set; }
        public DbSet<TRProcessLabAlertModel> TRProcessLabAlert { get; set; }
        public DbSet<TRProcessLabAlertDetailModel> TRProcessLabAlertDetail { get; set; }
        public DbSet<TRProcessLabAlertSummaryModel> TRProcessLabAlertSummary { get; set; }
        public DbSet<TRProcessGLASS_RIS> TRProcessGLASS_RISs { get; set; }
        public DbSet<TRProcessGLASS_Sample> TRProcessGLASS_Samples { get; set; }

        public DbSet<TCUserLoginPermission> TCUserLoginPermissions { get; set; }
        public DbSet<TRNoticeMessage> TRNoticeMessages { get; set; }

        public DbSet<RPIsolateListing> RPIsolateListings { get; set; }
        public DbSet<RPIsolateListingDetail> RPIsolateLisingDetails { get; set; }

        public DbSet<V_TRSTGProcessData> V_TRSTGProcessDatas { get; set; }

        public DbSet<NARST_File> NARST_Files { get; set; }
        public DbSet<GLASS_File> GLASS_Files { get; set; }
        public DbSet<TCMasterTemplate> TCMasterTemplates { get; set; }
        public DbSet<LogProcess> LogProcesss { get; set; }

        //public ProcessContext(DbContextOptions<ProcessContext> options) : base(options)
        //{

        //}

        public ProcessContext() : base()
        {
            this.Database.SetCommandTimeout(0);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();

            optionsBuilder.UseSqlServer(_iconfiguration.GetConnectionString("ProcessContext"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProcessDTO>().HasKey(x => x.prc_id);

            builder.Entity<TCWHONETColumn>().HasKey(x => x.wnc_id);

            builder.Entity<TCWHONET_Antibiotics>().HasKey(x => x.who_ant_id);

            builder.Entity<TCUserLoginPermission>().HasKey(x => x.usp_id);
            builder.Entity<TCUserLoginPermission>().ToTable("TCUserLoginPermission");

            builder.Entity<TCWHONET_Specimen>().HasKey(x => x.who_spc_id);
            builder.Entity<TCWHONET_Specimen>().ToTable("TCWHONET_Specimen");

            builder.Entity<TCProvince>().HasKey(x => x.prv_code);
            builder.Entity<TCProvince>().ToTable("TCProvince");

            builder.Entity<TRHospital>().HasKey(x => x.hos_id);
            builder.Entity<TRHospital>().ToTable("TRHospital");

            builder.Entity<TRHospitalLab>().HasKey(x => x.lab_id);
            builder.Entity<TRHospitalLab>().ToTable("TRHospitalLab");

            builder.Entity<TRLabFileUpload>().HasKey(x => x.lfu_id);
            builder.Entity<TRLabFileUpload>().ToTable("TRLabFileUpload");

            builder.Entity<TRNoticeMessage>().HasKey(x => x.noti_id);
            builder.Entity<TRNoticeMessage>().ToTable("TRNoticeMessage");

            builder.Entity<TRProcessRequest>().HasKey(x => x.pcr_id);
            builder.Entity<TRProcessRequest>().ToTable("TRProcessRequest");

            builder.Entity<TRProcessRequestDetail>().HasKey(x => x.pcd_id);
            builder.Entity<TRProcessRequestDetail>().ToTable("TRProcessRequestDetail");

            builder.Entity<TRSTGProcessDataHeader>().HasKey(x => x.pdh_id);
            builder.Entity<TRSTGProcessDataHeader>().ToTable("TRSTGProcessDataHeader");

            builder.Entity<TRSTGProcessFileUpload>().HasKey(x => x.pfu_id);
            builder.Entity<TRSTGProcessFileUpload>().ToTable("TRSTGProcessFileUpload");

            builder.Entity<TRSTGProcessDataDetail>().HasKey(x => x.pdd_id);
            builder.Entity<TRSTGProcessDataDetail>().ToTable("TRProcessDataDetail");

            builder.Entity<TRProcessDataResult>().HasKey(x => x.pdr_id);
            builder.Entity<TRProcessDataResult>().ToTable("TRProcessDataResult");

            builder.Entity<TRProcessDataListing>().HasKey(x => x.pdl_id);
            builder.Entity<TRProcessDataListing>().ToTable("TRProcessDataListing");

            builder.Entity<TCProcessExcelColumn>().HasKey(x => x.pec_id);
            builder.Entity<TCProcessExcelColumn>().ToTable("TCProcessExcelColumn");

            builder.Entity<TCProcessExcelRow>().HasKey(x => x.per_id);
            builder.Entity<TCProcessExcelRow>().ToTable("TCProcessExcelRow");

            builder.Entity<TCProcessExcelTemplate>().HasKey(x => x.pet_id);
            builder.Entity<TCProcessExcelTemplate>().ToTable("TCProcessExcelTemplate");

            builder.Entity<TRProcessLabFileModel>().HasKey(x => x.plf_id);
            builder.Entity<TRProcessLabFileModel>().ToTable("TRProcessLabFile");

            builder.Entity<TRProcessLabAlertModel>().HasKey(x => x.pla_id);
            builder.Entity<TRProcessLabAlertModel>().ToTable("TRProcessLabAlert");

            builder.Entity<TRProcessLabAlertDetailModel>().HasKey(x => x.plad_id);
            builder.Entity<TRProcessLabAlertDetailModel>().ToTable("TRProcessLabAlertDetail");

            builder.Entity<TRProcessLabAlertSummaryModel>().HasKey(x => x.plas_id);
            builder.Entity<TRProcessLabAlertSummaryModel>().ToTable("TRProcessLabAlertSummary");

            builder.Entity<RPIsolateListing>().HasKey(x => x.id);
            builder.Entity<RPIsolateListing>().ToTable("RPIsolateListing");

            builder.Entity<RPIsolateListingDetail>().HasKey(x => x.id);
            builder.Entity<RPIsolateListingDetail>().ToTable("RPIsolateListingDetail");

            builder.Entity<V_TRSTGProcessData>().HasNoKey();
            builder.Entity<V_TRSTGProcessData>().ToView("V_TRSTGProcessData");

            builder.Entity<TRProcessRequestDTO>().HasKey(x => x.pcr_id);

            builder.Entity<NARST_File>().HasKey(x => x.ldh_id);
            builder.Entity<GLASS_File>().HasKey(x => x.huh_id);

            builder.Entity<TRProcessRequestLabData>().HasKey(x => x.pcl_id);
            builder.Entity<TRProcessRequestLabData>().ToTable("TRProcessRequestLabData");

            builder.Entity<TRProcessRequestHISData>().HasKey(x => x.pch_id);
            builder.Entity<TRProcessRequestHISData>().ToTable("TRProcessRequestHISData");

            builder.Entity<TRProcessGLASS_RIS>().HasKey(x => x.pcg_ris_id);
            builder.Entity<TRProcessGLASS_RIS>().ToTable("TRProcessGLASS_RIS");

            builder.Entity<TRProcessGLASS_Sample>().HasKey(x => x.pcg_sam_id);
            builder.Entity<TRProcessGLASS_Sample>().ToTable("TRProcessGLASS_Sample");

            builder.Entity<TCMasterTemplate>().HasKey(x => x.mst_id);
            builder.Entity<TCMasterTemplate>().ToTable("TCMasterTemplate");

            builder.Entity<LogProcess>().HasKey(x => x.log_id);
            builder.Entity<LogProcess>().ToTable("XLogProcess");

            base.OnModelCreating(builder);
        }
    }
}
