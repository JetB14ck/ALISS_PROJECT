using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ALISS.STARS.DTO;
using ALISS.STARS.Library.Models;
using ALISS.MasterManagement.Library.Models;

namespace ALISS.STARS.Library.DataAccess
{
    public class STARSMappingContext : DbContext
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

        public STARSMappingContext(DbContextOptions<STARSMappingContext> options) : base(options)
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


            #endregion

            builder.Entity<LogProcess>().HasKey(x => x.log_id);
            builder.Entity<LogProcess>().ToTable("XLogProcess");


            base.OnModelCreating(builder);
        }

    }
}
