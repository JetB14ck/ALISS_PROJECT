using ALISS_AUTH.TC.UserLogin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ALISS_AUTH.TC.UserLogin.DataAccess
{
    public class ALISS_AUTHContext : DbContext
    {
        private static IConfiguration _iconfiguration;

        public DbSet<TCUserLogin> TCUserLogins { get; set; }
        public DbSet<TCUserLoginPermission> TCUserLoginPermissions { get; set; }

        //public ALISS_AUTHContext(DbContextOptions<ALISS_AUTHContext> options) : base(options)
        //{

        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();

            optionsBuilder.UseSqlServer(_iconfiguration.GetConnectionString("ALISS_AUTHContext"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TCUserLogin>().HasKey(x => x.usr_id);
            builder.Entity<TCUserLogin>().ToTable("TCUserLogin");

            builder.Entity<TCUserLoginPermission>().HasKey(x => x.usp_id);
            builder.Entity<TCUserLoginPermission>().ToTable("TCUserLoginPermission");

            base.OnModelCreating(builder);
        }
    }
}
