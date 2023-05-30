using ALISS.LabFileUpload.Batch.Models;
using ALISS.LabFileUpload.DTO;
using ALISS.Mapping.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ALISS.LabFileUpload.Batch.DataAccess
{
    public class LabDataContext : DbContext
    {
        private static IConfiguration _iconfiguration;

        public DbSet<MappingDataDTO> MappingDataDTOs { get; set; }
        public DbSet<WHONetMappingListsDTO> WHONetMappingListsDTOs { get; set; }
        public DbSet<WardTypeMappingListsDTO> WardTypeMappingListsDTOs { get; set; }
        public DbSet<SpecimenMappingListsDTO> SpecimenMappingListsDTOs { get; set; }
        public DbSet<OrganismMappingListsDTO> OrganismMappingListsDTOs { get; set; }
        public DbSet<TRSTGLabFileDataDetail> TRSTGLabFileDataDetails { get; set; }
        public DbSet<TRSTGLabFileDataHeader> TRSTGLabFileDataHeaders { get; set; }
        public DbSet<LabFileUploadDataDTO> LabFileUploadDataDTOs { get; set; }
        public DbSet<TCParameter> TCParameters { get; set; }
        public DbSet<TRLabFileErrorHeader> TRLabFileErrorHeaders { get; set; }
        public DbSet<TRLabFileErrorDetail> TRLabFileErrorDetails { get; set; }
        public DbSet<TRLabFileUpload> TRLabFileUploads { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();

            optionsBuilder.UseSqlServer(_iconfiguration.GetConnectionString("LabFileUploadContext"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<LabFileUploadDataDTO>().HasKey(x => x.lfu_id);
            builder.Entity<WHONetMappingListsDTO>().HasKey(x => x.wnm_id);
            builder.Entity<WardTypeMappingListsDTO>().HasKey(x => x.wdm_id);
            builder.Entity<SpecimenMappingListsDTO>().HasKey(x => x.spm_id);
            builder.Entity<OrganismMappingListsDTO>().HasKey(x => x.ogm_id);
            builder.Entity<MappingDataDTO>().HasKey(x => x.mp_id);

            builder.Entity<TCParameter>().HasKey(x => x.prm_id);
            builder.Entity<TCParameter>().ToTable("TCParameter");

            //builder.Entity<TRSTGLabFileDataHeader>().HasKey(x => x.ldh_id);           
            //builder.Entity<TRSTGLabFileDataHeader>().ToTable("TRSTGLabFileDataHeader");
            builder.Entity<TRSTGLabFileDataHeader>().HasKey(x => x.ldh_id_new);
            builder.Entity<TRSTGLabFileDataHeader>().ToTable("TRSTGLabFileDataHeader_POC");

            //builder.Entity<TRSTGLabFileDataDetail>().HasKey(x => x.ldd_id);
            //builder.Entity<TRSTGLabFileDataDetail>().ToTable("TRSTGLabFileDataDetail");
            builder.Entity<TRSTGLabFileDataDetail>().HasKey(x => x.ldd_id_new);
            builder.Entity<TRSTGLabFileDataDetail>().ToTable("TRSTGLabFileDataDetail_POC");

            builder.Entity<TRLabFileErrorHeader>().HasKey(x => x.feh_id);
            builder.Entity<TRLabFileErrorHeader>().ToTable("TRLabFileErrorHeader");

            builder.Entity<TRLabFileErrorDetail>().HasKey(x => x.fed_id);
            builder.Entity<TRLabFileErrorDetail>().ToTable("TRLabFileErrorDetail");

            builder.Entity<TRLabFileUpload>().HasKey(x => x.lfu_id);
            builder.Entity<TRLabFileUpload>().ToTable("TRLabFileUpload");

            base.OnModelCreating(builder);
        }
    }
}
