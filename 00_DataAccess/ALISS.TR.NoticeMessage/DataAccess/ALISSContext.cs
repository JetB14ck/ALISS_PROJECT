using ALISS.TR.NoticeMessage.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ALISS.TR.NoticeMessage.DataAccess
{
    public class ALISSContext : DbContext
    {
        private static IConfiguration _iconfiguration;

        public DbSet<TRNoticeMessage> TRNoticeMessageModel { get; set; }

        //public ALISSContext(DbContextOptions<ALISSContext> options) : base(options)
        //{

        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();

            optionsBuilder.UseSqlServer(_iconfiguration.GetConnectionString("ALISSContext"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TRNoticeMessage>().HasKey(x => x.noti_id);
            builder.Entity<TRNoticeMessage>().ToTable("TRNoticeMessage");

            base.OnModelCreating(builder);
        }
    }
}
