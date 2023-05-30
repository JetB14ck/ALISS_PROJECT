using ALISS.HISUpload.Batch.HDCData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.HISUpload.Batch.HDCData.DataAcess
{
    public class HDCDataContext : DbContext
    {
        private static IConfiguration _iconfiguration;

        public DbSet<TRLabFileUpload> TRLabFileUploads { get; set; }
        public DbSet<TRSTGLabFileDataHeader> TRSTGLabFileDataHeaders { get; set; }
        public DbSet<TRSTGLabFileDataDetail> TRSTGLabFileDataDetails { get; set; }
        public DbSet<TRSTGHISFileUploadHeader> TRSTGHISFileUploadHeaders { get; set; }
        public DbSet<TRSTGHISFileUploadDetail> TRSTGHISFileUploadDetails { get; set; }
        public DbSet<TRSTGHDCData> TRSTGHDCDatas { get; set; }
        public DbSet<TRHISFileUpload> TRHISFileUploads { get; set; }

        public HDCDataContext(DbContextOptions<HDCDataContext> options) : base(options)
        {
            base.Database.SetCommandTimeout(0);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();

            optionsBuilder.UseSqlServer(_iconfiguration.GetConnectionString("HDCDataContext"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasAnnotation("Relational:Collation", "Thai_CI_AS");

            builder.Entity<TRLabFileUpload>(entity =>
            {
                entity.HasKey(e => e.lfu_id);
                entity.ToTable("TRLabFileUpload");

                entity.HasMany(d => d.TRSTGLabFileDataHeaderList)
                    .WithOne()
                    .HasForeignKey(d => d.ldh_lfu_id);
            });

            builder.Entity<TRSTGLabFileDataHeader>(entity =>
            {
                entity.HasKey(e => e.ldh_id);
                entity.ToTable("TRSTGLabFileDataHeader");

                entity.HasMany(d => d.TRSTGLabFileDataDetailList)
                    .WithOne()
                    .HasForeignKey(d => d.ldd_ldh_id);
            });

            builder.Entity<TRSTGLabFileDataDetail>(entity =>
            {
                entity.HasKey(e => e.ldd_id);
                entity.ToTable("TRSTGLabFileDataDetail");
            });

            builder.Entity<TRSTGHISFileUploadHeader>(entity =>
            {
                entity.HasKey(e => e.huh_id);
                entity.ToTable("TRSTGHISFileUploadHeader");
            });

            builder.Entity<TRSTGHISFileUploadDetail>(entity =>
            {
                entity.HasKey(e => e.hud_id);
                entity.ToTable("TRSTGHISFileUploadDetail");
            });

            builder.Entity<TRSTGHDCData>(entity =>
            {
                entity.HasKey(e => e.hdc_id);
                entity.ToTable("TRSTGHDCData");
            });

            builder.Entity<TRHISFileUpload>(entity =>
            {
                entity.HasKey(e => e.hfu_id);
                entity.ToTable("TRHISFileUpload");
            });
        }
    }
}
