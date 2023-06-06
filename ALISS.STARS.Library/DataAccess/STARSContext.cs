using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ALISS.STARS.DTO;
using ALISS.STARS.Library.Models;
using ALISS.MasterManagement.Library.Models;
using ALISS.LabFileUpload.DTO;

namespace ALISS.STARS.Library.DataAccess
{
    public class STARSContext : DbContext
    {
        public DbSet<LogProcess> LogProcesss { get; set; }
        #region STARSMapping
        public DbSet<TRSTARSMapping> TRSTARSMappings { get; set; }
        public DbSet<STARSMappingListsDTO> STARSMappingListDTOs { get; set; }
        public DbSet<STARSMappingDataDTO> STARSMappingDataDTOs { get; set; }

        public DbSet<TRSTARSWHONetMapping> TRSTARSWHONetMappings { get; set; }
        public DbSet<STARSWHONetMappingListsDTO> STARSWHONetMappingListsDTOs { get; set; }
        public DbSet<STARSWHONetMappingDataDTO> STARSWHONetMappingDataDTOs { get; set; }

        public DbSet<TRSTARSSpecimenMapping> TRSTARSSpecimenMappings { get; set; }
        public DbSet<STARSSpecimenMappingListsDTO> STARSSpecimenMappingListsDTOs { get; set; }
        public DbSet<STARSSpecimenMappingDataDTO> STARSSpecimenMappingDataDTOs { get; set; }


        public DbSet<TRSTARSOrganismMapping> TRSTARSOrganismMappings { get; set; }
        public DbSet<STARSOrganismMappingListsDTO> STARSOrganismMappingListsDTOs { get; set; }
        public DbSet<STARSOrganismMappingDataDTO> STARSOrganismMappingDataDTOs { get; set; }
        #endregion

        #region Receive Sample

        public DbSet<TRStarsResult> TRStarsResults { get; set; }
        public DbSet<TRStarsReceiveSample> TRStarsReceiveSamples { get; set; }
        public DbSet<ReceiveSampleListsDTO> ReceiveSampleListsDTOs { get; set; }
        public DbSet<TrRunningNoDTO> TrRunningNoDTOs { get; set; }

        #endregion

        public STARSContext(DbContextOptions<STARSContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region STARSMapping
            builder.Entity<STARSMappingListsDTO>().HasKey(x => x.smp_id);
            builder.Entity<STARSMappingDataDTO>().HasKey(x => x.smp_id);

            builder.Entity<TRSTARSMapping>().HasKey(x => x.smp_id);
            builder.Entity<TRSTARSMapping>().ToTable("TRStarsMapping");


            builder.Entity<STARSWHONetMappingListsDTO>().HasKey(x => x.swm_id);
            builder.Entity<STARSWHONetMappingDataDTO>().HasKey(x => x.swm_id);

            builder.Entity<TRSTARSWHONetMapping>().HasKey(x => x.swm_id);
            builder.Entity<TRSTARSWHONetMapping>().ToTable("TRStarsWHONetMapping");


            builder.Entity<STARSSpecimenMappingListsDTO>().HasKey(x => x.ssm_id);
            builder.Entity<STARSSpecimenMappingDataDTO>().HasKey(x => x.ssm_id);

            builder.Entity<TRSTARSSpecimenMapping>().HasKey(x => x.ssm_id);
            builder.Entity<TRSTARSSpecimenMapping>().ToTable("TRStarsSpecimenMapping");

            builder.Entity<STARSOrganismMappingListsDTO>().HasKey(x => x.som_id);
            builder.Entity<STARSOrganismMappingDataDTO>().HasKey(x => x.som_id);

            builder.Entity<TRSTARSOrganismMapping>().HasKey(x => x.som_id);
            builder.Entity<TRSTARSOrganismMapping>().ToTable("TRStarsOrganismMapping");

            #endregion

            #region ReceiveSample
            builder.Entity<TRStarsResult>().HasKey(x => x.srr_id);
            builder.Entity<TRStarsResult>().ToTable("TRStarsResult");

            builder.Entity<TRStarsReceiveSample>().HasKey(x => x.str_id);
            builder.Entity<TRStarsReceiveSample>().ToTable("TRStarsReceiveSample");

            builder.Entity<ReceiveSampleListsDTO>().HasKey(x => x.srr_id);
            builder.Entity<ReceiveSampleDataDTO>().HasKey(x => x.srr_id);

            #endregion

            builder.Entity<LogProcess>().HasKey(x => x.log_id);
            builder.Entity<LogProcess>().ToTable("XLogProcess");

            builder.Entity<TrRunningNoDTO>().HasKey(x => x.trn_id);
            builder.Entity<TrRunningNoDTO>().ToTable("TR_RUNNING_NUMBER");


            base.OnModelCreating(builder);
        }

    }
}
